using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RSAExtensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.SecurityProxyApi
{
    /// <summary>
    /// 加解密/签名/验签
    /// <p>
    /// 
    /// </p>
    /// </summary>
    public class SecretHelper
    {
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="certificate">X509 V3公钥证书</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(string data, string publicKey, string encoding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
            {
                return ((RSA)cer.PublicKey.Key).EncryptBigData(data, RSAEncryptionPadding.Pkcs1, Encoding.GetEncoding(encoding));
            }
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="privateKey">私钥 pem编码,Pkcs8</param>
        /// <param name="encoding"></param>+
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey, string encoding)
        {
            using (var rsa = RSA.Create(1024))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                return rsa.DecryptBigData(data, RSAEncryptionPadding.Pkcs1, Encoding.GetEncoding(encoding));
            }
        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Sign(string data, string privateKey, string encoding)
        {
            using (var rsa = RSA.Create(1024))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                var signed = rsa.SignData(Encoding.GetEncoding(encoding).GetBytes(data), HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signed);
            }
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        /// <param name="data"></param>
        /// <param name="certificate"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool Verify(string data, string signature, string publicKey, string encoding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
                return ((RSA)cer.PublicKey.Key)
               .VerifyData(
                    Encoding.GetEncoding(encoding).GetBytes(data),
                    Convert.FromBase64String(signature),
                    HashAlgorithmName.MD5,
                    RSASignaturePadding.Pkcs1
                );

        }
    }
}
