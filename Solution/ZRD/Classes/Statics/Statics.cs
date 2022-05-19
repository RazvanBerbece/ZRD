/*
 * Class which acts as API interface to some util functions
 */

using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

namespace ZRD.Classes.Statics
{
    public class Statics
    {
        public static string CreateHashSHA256(string data)
        {
            // Create SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Create SHA256 byte array data as byte[]; ComputeHash() works on byte[]
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; ++i)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /**
         * TODO: Could be built with template values ?
         */
        public static string TransactionsToJSONString(Transaction.Transaction[] transactions)
        {
            return JsonSerializer.Serialize(transactions);
        }
    }
}
