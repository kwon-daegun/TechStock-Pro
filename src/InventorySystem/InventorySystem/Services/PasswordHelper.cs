using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;


namespace InventorySystem.Services {
    public static class PasswordHelper {
        // Encrypts "1234" -> "a87g87..."
        public static string HashPassword(string password) {
            using (var sha256 = SHA256.Create()) {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes) {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Checks if the typed password matches the saved hash
        public static bool VerifyPassword(string inputPassword, string storedHash) {
            var inputHash = HashPassword(inputPassword);
            return inputHash == storedHash;
        }
    }
}