using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RSAExtensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.SecurityProxyClient
{
    /// <summary>
    /// 加解密/签名/验签
    /// <p>
    /// 公钥X509 V3;私钥pkcs8/pem
    /// </p>
    /// </summary>
    public static class SecretHelper
    {
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="publicKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(string data, string publicKey, Encoding encoding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
            {
                return ((RSA) cer.PublicKey.Key).EncryptBigData(data, RSAEncryptionPadding.Pkcs1, encoding);
            }
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="privateKey">私钥 pem编码,Pkcs8</param>
        /// <param name="encoding"></param>+
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey, Encoding encoding)
        {
            using (var rsa = RSA.Create(1024))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                return rsa.DecryptBigData(data, RSAEncryptionPadding.Pkcs1, encoding);
            }
        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sign(string data, string privateKey, Encoding encoding)
        {
            using (var rsa = RSA.Create(1024))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                var signed = rsa.SignData(encoding.GetBytes(data), HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signed);
            }
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <param name="encoding"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool Verify(string data, string signature, string publicKey, Encoding encoding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
                return ((RSA) cer.PublicKey.Key)
                    .VerifyData(
                        encoding.GetBytes(data),
                        Convert.FromBase64String(signature),
                        HashAlgorithmName.MD5,
                        RSASignaturePadding.Pkcs1
                    );
        }
    }
}