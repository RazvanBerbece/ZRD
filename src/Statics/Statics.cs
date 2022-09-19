/*
 * Class which acts as API interface to some util functions
 */

using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Collections.Generic;
using TransactionNS;
using System;
using System.Net;
using System.Net.NetworkInformation;
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
            
            // Guards
            ArgumentNullException.ThrowIfNull(data);
            
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
            
            // Guards
            ArgumentNullException.ThrowIfNull(transaction);
            
            var dataString =
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
            
            // Guards
            ArgumentNullException.ThrowIfNull(transactions);

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
                // Get IPv4 IP here and return it: a.b.c.d, excluding 127.0.0.1
                Regex expression = new Regex(
                    @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$", 
                    RegexOptions.Compiled
                    );
                var results = expression.Matches(addr[i].ToString());
                if (results.Count != 0 && !addr[i].ToString().Equals("127.0.0.1"))
                {
                    return addr[i];
                }
            }

            return null;
        }
        
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static bool CanPingHost(string host, int timeoutInMs)
        {
            
            // Guards
            ArgumentNullException.ThrowIfNull(host);
            if (host == "") throw new ArgumentException("Error in CanPingHost: host param cannot be empty");
            if (timeoutInMs <= 0) throw new ArgumentOutOfRangeException(nameof(timeoutInMs));
            
            Ping myPing = new Ping();
            PingReply reply;
            try
            {
                reply = myPing.Send(host, timeoutInMs);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while pinging {host} : {e}");
                return false;
            }
            return false;
        }
        
        
        public static string GetPeerPublicIp(TcpClient peer)
        {
            
            // Guards
            ArgumentNullException.ThrowIfNull(peer);
            
            var peerEndpoint = peer.Client.RemoteEndPoint as IPEndPoint;
            var localAddress = peerEndpoint!.Address.ToString();
            return localAddress;
        }

    }
    
}
