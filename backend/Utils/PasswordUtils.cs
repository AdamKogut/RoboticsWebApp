using Backend.Interfaces.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Utils
{
  public static class PasswordUtils
  {
    private const int SALT_LENGTH = 32;
    private const int HASH_LENGTH = 32;

    public static string GenerateHash(string plainText, string email, byte[] saltBytes = null!)
    {
      if (saltBytes == null)
      {
        saltBytes = new byte[SALT_LENGTH];
        var rngProvider = RandomNumberGenerator.Create();

        rngProvider.GetBytes(saltBytes);
      }

      string key = "C8NC6zGnlH" + email;

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

    public static bool VerifyHash(string plainText, string encrypted, string email)
    {
      var hashedTextBytes = Convert.FromBase64String(encrypted);
      var saltBytes = hashedTextBytes.Skip(HASH_LENGTH).ToArray();

      var plainTextHash = GenerateHash(plainText, email, saltBytes);

      return encrypted == plainTextHash;
    }
  }
}