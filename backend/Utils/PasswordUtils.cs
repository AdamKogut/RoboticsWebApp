using System.Text;
using System.Security.Cryptography;

namespace Backend.Utils
{
  public static class PasswordUtils
  {
    private const int SALT_LENGTH = 32;
    private const int HASH_LENGTH = 32;

    /// <summary>
    ///   Generates hash for password
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="id"></param>
    /// <param name="saltBytes"></param>
    /// <returns></returns>
    public static string GenerateHash(string plainText, Guid id, byte[] saltBytes = null!)
    {
      if (saltBytes == null)
      {
        saltBytes = new byte[SALT_LENGTH];
        var rngProvider = RandomNumberGenerator.Create();

        rngProvider.GetBytes(saltBytes);
      }

      // TODO: Change prior to deployment
      string key = "C8NC6zGnlH" + id;

      var keyBytes = Encoding.UTF8.GetBytes(key);
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

      var plainTextByteCount = plainTextBytes.Count();
      var saltByteCount = saltBytes.Count();
      var keyByteCount = keyBytes.Count();

      var plainTextWithSaltBytes = plainTextBytes.Concat(saltBytes).Concat(keyBytes).ToArray();

      var algorithm = SHA256.Create();

      var hashedBytes = algorithm.ComputeHash(plainTextWithSaltBytes);

      var hashAndSalt = hashedBytes.Concat(saltBytes).ToArray();

      return Convert.ToBase64String(hashAndSalt);
    }

    /// <summary>
    ///   Verifies password hashes are the same
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="encrypted"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool VerifyHash(string plainText, string encrypted, Guid id)
    {
      var hashedTextBytes = Convert.FromBase64String(encrypted);
      var saltBytes = hashedTextBytes.Skip(HASH_LENGTH).ToArray();

      var plainTextHash = GenerateHash(plainText, id, saltBytes);

      return encrypted == plainTextHash;
    }

    /// <summary>
    ///   Encrypts plaintext using AES and the AES key with RSA
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static string AesEncrypt(string plainText, IConfiguration config)
    {
      using (var aes = Aes.Create())
      {
        aes.GenerateKey();
        // Get AES key
        var key = aes.Key;
        var iv = aes.IV;

        // Encrypt key using RSA
        var encryptedKey = new byte[0];
        using (var rsa = new RSACryptoServiceProvider())
        {
          var publicKey = Convert.FromBase64String(config["RsaPublic"]);
          rsa.ImportRSAPublicKey(publicKey, out _);
          encryptedKey = rsa.Encrypt(key.Concat(iv).ToArray(), false);
        }

        // Encode plainText with AES Key
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        var b = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

        // Concat the encrypted info and encrypted key together
        return Convert.ToBase64String(b.Concat(encryptedKey).ToArray());
      }
    }

    /// <summary>
    ///   Decrypts encrypted string opposite of AesEncrypt()
    /// </summary>
    /// <param name="encrypted"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static string AesDecrypt(string encrypted, IConfiguration config)
    {
      using (var aes = Aes.Create())
      {
        var encryptedBytes = Convert.FromBase64String(encrypted);

        // separate encrypted info and encrypted key (128 is encrypted key length)
        var infoNumBytes = encryptedBytes.Length - 128;
        var infoBytes = new byte[infoNumBytes];
        var rsaInfoBytes = new byte[128];
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
          if (i < infoNumBytes)
          {
            infoBytes[i] = encryptedBytes[i];
          }
          else
          {
            rsaInfoBytes[i - infoNumBytes] = encryptedBytes[i];
          }
        }

        // Decrypt key and iv using RSA
        var aesKey = new byte[32];
        var aesIv = new byte[16];
        using (var rsa = new RSACryptoServiceProvider())
        {
          var privateKey = Convert.FromBase64String(config["RsaPrivate"]);
          rsa.ImportRSAPrivateKey(privateKey, out _);
          var decryptedAes = rsa.Decrypt(rsaInfoBytes, false);
          for (int i = 0; i < decryptedAes.Length; i++)
          {
            if (i < 32)
            {
              aesKey[i] = decryptedAes[i];
            }
            else
            {
              aesIv[i - 32] = decryptedAes[i];
            }
          }
        }

        // decrypt info
        var decryptor = aes.CreateDecryptor(aesKey, aesIv);
        return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(infoBytes, 0, infoBytes.Length));
      }
    }
  }
}