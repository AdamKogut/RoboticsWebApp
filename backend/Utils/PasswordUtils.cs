using System.Text;
using System.Security.Cryptography;

namespace Backend.Utils
{
  public static class PasswordUtils
  {
    private const int SALT_LENGTH = 32;
    private const int HASH_LENGTH = 32;

    public static string GenerateHash(string plainText, Guid id, byte[] saltBytes = null!)
    {
      if (saltBytes == null)
      {
        saltBytes = new byte[SALT_LENGTH];
        var rngProvider = RandomNumberGenerator.Create();

        rngProvider.GetBytes(saltBytes);
      }

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

    public static bool VerifyHash(string plainText, string encrypted, Guid id)
    {
      var hashedTextBytes = Convert.FromBase64String(encrypted);
      var saltBytes = hashedTextBytes.Skip(HASH_LENGTH).ToArray();

      var plainTextHash = GenerateHash(plainText, id, saltBytes);

      return encrypted == plainTextHash;
    }
  }
}