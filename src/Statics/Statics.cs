/*
 * Class which acts as API interface to some util functions
 */

using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Collections.Generic;
using TransactionNS;
using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace StaticsNS
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
                StringBuilder builder = new();
                for (int i = 0; i < bytes.Length; ++i)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string CreateHashSHA256FromTransaction(Transaction transaction)
        {
            string dataString =
                transaction.id +
                transaction.hash +
                transaction.Sender +
                transaction.Receiver +
                transaction.Amount.ToString();

            return CreateHashSHA256(dataString);
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
        public static string TransactionsToJSONString(List<Transaction> transactions)
        {
            return JsonSerializer.Serialize(transactions);
        }

    }
}
