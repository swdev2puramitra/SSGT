using System;
using System.Security.Cryptography;
using System.Text;

namespace FireWorksDal.Constants
{
    public class Hashing
    {
        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public string getSha1Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            SHA1 sha1Hasher = SHA1.Create();

            // Convert the input string to a byte array and compute the hash.
            //byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            byte[] data = sha1Hasher.ComputeHash(Encoding.Default.GetBytes(input));


            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public bool verifySha1Hash(string input, string storedPwd)
        {
            // Hash the input.
            string hashOfInput = getSha1Hash(input);

            // Hash the stored password
            string hashOfstoredPwd = getSha1Hash(storedPwd);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hashOfstoredPwd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // SHA-256 Hash Code Generator Method
        public static string SHA256Generator(string strInput)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();

            //provide the string in byte format to the ComputeHash method. 
            //This method returns the SHA-256 hash code in byte array
            byte[] arrHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(strInput));

            // use a Stringbuilder to append the bytes from the array to create a SHA-256 hash code string.
            StringBuilder sbHash = new StringBuilder();

            // Loop through byte array of the hashed code and format each byte as a hexadecimal code.
            for (int i = 0; i < arrHash.Length; i++)
            {
                sbHash.Append(arrHash[i].ToString("x2"));
            }

            // Return the hexadecimal SHA-256 hash code string.
            return sbHash.ToString();
        }

        // SHA-512 Hash Code Generator Method
        public static string SHA512Generator(string strInput)
        {
            SHA512 sha512 = new SHA512CryptoServiceProvider();

            //provide the string in byte format to the ComputeHash method. 
            //This method returns the SHA-512 hash code in byte array
            byte[] arrHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(strInput));

            // use a Stringbuilder to append the bytes from the array to create a SHA-512 hash code string.
            StringBuilder sbHash = new StringBuilder();

            // Loop through byte array of the hashed code and format each byte as a hexadecimal code.
            for (int i = 0; i < arrHash.Length; i++)
            {
                sbHash.Append(arrHash[i].ToString("x2"));
            }

            // Return the hexadecimal SHA-512 hash code string.
            return sbHash.ToString();
        }

        // SHA-384 Hash Code Generator Method
        public static string SHA384Generator(string strInput)
        {
            SHA384 sha384 = new SHA384CryptoServiceProvider();

            //provide the string in byte format to the ComputeHash method. 
            //This method returns the SHA-384 hash code in byte array
            byte[] arrHash = sha384.ComputeHash(Encoding.UTF8.GetBytes(strInput));

            // use a Stringbuilder to append the bytes from the array to create a SHA-384 hash code string.
            StringBuilder sbHash = new StringBuilder();

            // Loop through byte array of the hashed code and format each byte as a hexadecimal code.
            for (int i = 0; i < arrHash.Length; i++)
            {
                sbHash.Append(arrHash[i].ToString("x2"));
            }

            // Return the hexadecimal SHA-384 hash code string.
            return sbHash.ToString();
        }
    }
}
