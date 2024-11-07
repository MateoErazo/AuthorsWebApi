using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace AuthorsWebApi.Services
{
    public class HashService
    {
        public HashResultDTO Hash(string plainText)
        {
            byte[] salt = new byte[16];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(plainText, salt);
        }

        public HashResultDTO Hash(string plainText, byte[] salt)
        {
            byte[] derivedKey = KeyDerivation.Pbkdf2(
                password: plainText,
                salt: salt, 
                prf: KeyDerivationPrf.HMACSHA1, 
                iterationCount: 10000, 
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashResultDTO
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
