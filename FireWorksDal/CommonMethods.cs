using System;
using System.Security.Cryptography;
using System.Text;


namespace FireWorksDal
{
    public class CommonMethods
    {
        #region Check DB NULL

        /// <summary>
        /// check for null values
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public object GetDBNullOrValue(object val)
        {
            if (val == null || val == DBNull.Value)
                return DBNull.Value;

            if (val.GetType().Name == "DBNull")
                return "NULL";

            if (val.GetType() == typeof(string))
            {
                if (val.ToString().Trim() == "")
                    return DBNull.Value;
                else
                    return val.ToString();
            }


            //if ((val.GetType() == typeof(byte)) || (val.GetType() == typeof(int)) || (val.GetType() == typeof(long)) || (val.GetType() == typeof(double)))
            //{
            //    if (val.ToString().Trim() == "0")
            //        return DBNull.Value;
            //    else
            //        return val;
            //}
            return val;
        }

        /// <summary>
        /// Check if the object is DBNull or NULL.
        /// </summary>
        /// <param name="val">Object to check.</param>
        /// <returns>True if object is DBNull or NULL</returns>
        public bool IsDBNull(object val)
        {
            if (val == null)
                return true;

            if (val == DBNull.Value)
                return true;

            return false;
        }

        /// <summary>
        /// to retun the messages according to the code passed
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetErrorMessage(long errCode, string str)
        {
            string retMessage = "";
            switch (errCode)
            {
                case -1:
                    retMessage = "Already exists";
                    break;
                case 0:
                    retMessage = "Unable to Submit";
                    break;
                case -2:
                    retMessage = "Failed to update with given details.";
                    break;
                case 100:
                    retMessage = "Updated Successfully";
                    break;
                case -5:
                    retMessage = "Invalid Details.";
                    break;

                default:
                    retMessage = "Submited Successfully";
                    break;
            }
            return retMessage;
        }

        /// <summary>
        /// method to replace the single code to double code for the database insert and update purpose
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public string ReplaceQuotes(string pValue)
        {
            if (!(pValue == null))
            {
                if (pValue.Trim().Length > 0)
                {

                    return pValue.Trim().Replace("'", "''");
                }
                else
                {
                    return "";
                }
            }
            return pValue;
        }
        #endregion

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

    }
}
