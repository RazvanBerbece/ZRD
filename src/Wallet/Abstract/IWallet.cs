using System.Security.Cryptography;

namespace WalletNS.Abstract
{
    public interface IWallet
    {
        public string GetPublicKeyStringBase64();
        public string GetPrivateKeyStringBase64();
        public byte[] GetPrivateKeyBytesArray();
        public RSAParameters GetKeyPairParams();
    }
}