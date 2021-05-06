using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tool.Utils.Encryption
{
    /// <summary>
    /// AES 加密 解密,秘钥长度为32位，不足时系统自动补足空字符。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class AES
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        private AES() { }

        ///// <summary>
        ///// 解密成字符串
        ///// </summary>
        ///// <param name="cipherText">密文</param>
        ///// <param name="cipherkey">密码密钥</param>
        ///// <returns>返回字符串</returns>
        //public static string Decrypt(string cipherText, string cipherkey)
        //{
        //    string result;
        //    try
        //    {
        //        cipherkey = TextUtility.CutLeft(cipherkey, 32);
        //        cipherkey = cipherkey.PadRight(32, ' ');
        //        ICryptoTransform cryptoTransform = new RijndaelManaged
        //        {
        //            Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
        //            IV = AES.Keys
        //        }.CreateDecryptor();
        //        byte[] array = Convert.FromBase64String(cipherText);
        //        byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
        //        result = Encoding.UTF8.GetString(bytes);
        //    }
        //    catch
        //    {
        //        result = "";
        //    }
        //    return result;
        //}

        /// <summary>
        /// 解密成字符串
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="cipherkey">密码密钥</param>
        /// <returns>返回字符串</returns>
        public static string Decrypt(string cipherText, string cipherkey)
        {
            byte[] bytes = DecryptBuffer(Convert.FromBase64String(cipherText), cipherkey);
            if (bytes == null) return null;
            return Encoding.UTF8.GetString(bytes);

            //string result;
            //try
            //{
            //    cipherkey = TextUtility.CutLeft(cipherkey, 32);
            //    cipherkey = cipherkey.PadRight(32, ' ');
            //    ICryptoTransform cryptoTransform = new RijndaelManaged
            //    {
            //        Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
            //        IV = AES.Keys
            //    }.CreateDecryptor();
            //    byte[] array = Convert.FromBase64String(cipherText);
            //    byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
            //    result = Encoding.UTF8.GetString(bytes);
            //}
            //catch
            //{
            //    result = "";
            //}
            //return result;
        }

        ///// <summary>
        ///// 解密byte[]
        ///// </summary>
        ///// <param name="cipherText">密文内容</param>
        ///// <param name="cipherkey">密码密钥</param>
        ///// <returns></returns>
        //public static byte[] DecryptBuffer(byte[] cipherText, string cipherkey)
        //{
        //    byte[] result;
        //    try
        //    {
        //        cipherkey = TextUtility.CutLeft(cipherkey, 32);
        //        cipherkey = cipherkey.PadRight(32, ' ');
        //        RijndaelManaged rijndaelManaged = new RijndaelManaged
        //        {
        //            Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
        //            IV = AES.Keys
        //        };
        //        result = rijndaelManaged.CreateDecryptor().TransformFinalBlock(cipherText, 0, cipherText.Length);
        //    }
        //    catch
        //    {
        //        result = null;
        //    }
        //    return result;
        //}

        /// <summary>
        /// 解密byte[]
        /// </summary>
        /// <param name="cipherText">密文内容</param>
        /// <param name="cipherkey">密码密钥</param>
        /// <returns></returns>
        public static byte[] DecryptBuffer(byte[] cipherText, string cipherkey)
        {
            byte[] result;
            try
            {
                //cipherkey = TextUtility.CutLeft(cipherkey, 32); cipherkey.PadRight(32, ' ');
                cipherkey = AES.GetPassword(cipherkey);

                using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
                using ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor(Encoding.UTF8.GetBytes(cipherkey), AES.Keys);

                result = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        ///// <summary>
        ///// 加密字符串
        ///// </summary>
        ///// <param name="plainText">原字符串</param>
        ///// <param name="cipherkey">密码密钥</param>
        ///// <returns>返回加密的字符串</returns>
        //public static string Encrypt(string plainText, string cipherkey)
        //{
        //    cipherkey = TextUtility.CutLeft(cipherkey, 32);
        //    cipherkey = cipherkey.PadRight(32, ' ');
        //    ICryptoTransform cryptoTransform = new RijndaelManaged
        //    {
        //        Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
        //        IV = AES.Keys
        //    }.CreateEncryptor();
        //    byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        //    return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
        //}

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="plainText">原字符串</param>
        /// <param name="cipherkey">密码密钥</param>
        /// <returns>返回加密的字符串</returns>
        public static string Encrypt(string plainText, string cipherkey)
        {
            return Convert.ToBase64String(EncryptBuffer(Encoding.UTF8.GetBytes(plainText), cipherkey));

            //cipherkey = TextUtility.CutLeft(cipherkey, 32);
            //cipherkey = cipherkey.PadRight(32, ' ');

            //using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
            //ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor(Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)), AES.Keys);

            //byte[] bytes = Encoding.UTF8.GetBytes(plainText);

            //return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
        }

        ///// <summary>
        ///// 加密byte[]
        ///// </summary>
        ///// <param name="plainText">原内容</param>
        ///// <param name="cipherkey">密码密钥</param>
        ///// <returns></returns>
        //public static byte[] EncryptBuffer(byte[] plainText, string cipherkey)
        //{
        //    cipherkey = TextUtility.CutLeft(cipherkey, 32);
        //    cipherkey = cipherkey.PadRight(32, ' ');
        //    RijndaelManaged rijndaelManaged = new RijndaelManaged
        //    {
        //        Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
        //        IV = AES.Keys
        //    };
        //    return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainText, 0, plainText.Length);
        //}

        /// <summary>
        /// 加密byte[]
        /// </summary>
        /// <param name="plainText">原内容</param>
        /// <param name="cipherkey">密码密钥</param>
        /// <returns></returns>
        public static byte[] EncryptBuffer(byte[] plainText, string cipherkey)
        {
            //cipherkey = TextUtility.CutLeft(cipherkey, 32); cipherkey.PadRight(32, ' ');
            cipherkey = AES.GetPassword(cipherkey);

            using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
            using ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor(Encoding.UTF8.GetBytes(cipherkey), AES.Keys);

            return cryptoTransform.TransformFinalBlock(plainText, 0, plainText.Length);

            //RijndaelManaged rijndaelManaged = new RijndaelManaged
            //{
            //    Key = Encoding.UTF8.GetBytes(cipherkey.Substring(0, 32)),
            //    IV = AES.Keys
            //};
            //return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainText, 0, plainText.Length);
        }

        /// <summary>
        /// 获取AES实际加密密码
        /// </summary>
        /// <param name="encryptKey">原密码密钥</param>
        /// <returns></returns>
        public static string GetPassword(string encryptKey)
        {
            return TextEncrypt.GetPassword(encryptKey, 32);
        }

        /// <summary>
        /// 对称算法,
        /// 65,
        ///	114,
        ///	101,
        ///	121,
        ///	111,
        ///	117,
        ///	109,
        ///	121,
        ///	83,
        ///	110,
        ///	111,
        ///	119,
        ///	109,
        ///	97,
        ///	110,
        ///	63
        /// </summary>
        private static readonly byte[] Keys = new byte[]
        {
            65,
            114,
            101,
            121,
            111,
            117,
            109,
            121,
            83,
            110,
            111,
            119,
            109,
            97,
            110,
            63
        };
    }
}
