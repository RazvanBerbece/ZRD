/*
 * Class which acts as API interface to some util functions
 */

using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Collections.Generic;
using TransactionNS;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace StaticsNS
{
    public class Statics
    {
        public static string CreateHashSha256(string data)
        {
            // Create SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Create SHA256 byte array data as byte[]; ComputeHash() works on byte[]
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to string
                StringBuilder builder = new();
                for (int i = 0; i < bytes.Length; ++i)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string CreateHashSha256FromTransaction(Transaction transaction)
        {
            string dataString =
                transaction.Id +
                transaction.Hash +
                transaction.Sender +
                transaction.Receiver +
                transaction.Amount.ToString();

            return CreateHashSha256(dataString);
        }

        public static bool SignatureIsValid(byte[] data, byte[] signature, string publicKeyBase64String)
        {

            using (var sha256 = SHA256.Create())
            {
                // Get RSA params from base64 encoded public key

                // Get bytes array of publicKeyBase64String
                byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64String);

                // Use BouncyCastle 3rd party lib to create a RsaKeyParameters (c# std) from asymmetricKeyParameter (BouncyCastle)
                AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(publicKeyBytes);
                RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

                RSAParameters rsaParameters = new()
                {
                    Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                    Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
                };

                RSACryptoServiceProvider rsa = new();
                rsa.ImportParameters(rsaParameters);

                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /**
         * TODO: Could be built with template values ?
         */ 
        public static string TransactionsToJsonString(List<Transaction> transactions)
        {
            return JsonSerializer.Serialize(transactions);
        }

        public static IPAddress GetExternalPublicIpAddress()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(externalIpString);
        }
        
        public static IPAddress GetLocalIpAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                // Get 192.168.x.x IP here and return it
                string regexHashPattern = "192.168.*.*";
                Regex hashExpression = new Regex(regexHashPattern, RegexOptions.Compiled);
                MatchCollection hashMatches = hashExpression.Matches(addr[i].ToString());
                if (hashMatches.Count != 0)
                {
                    return addr[i];
                }
            }

            return null;
        }

    }
    
}
