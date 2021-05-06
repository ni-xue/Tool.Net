using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tool.Utils.Encryption
{
    /// <summary>
    /// 公共加密类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TextEncrypt
    {
        private TextEncrypt()
        {
        }

        /// <summary>
        /// 用于处理密码不够或密码过长的处理
        /// </summary>
        /// <param name="encryptKey">原密码密钥</param>
        /// <param name="length">密码要求长度</param>
        /// <returns>返回实际密码</returns>
        public static string GetPassword(string encryptKey, int length)
        {
            encryptKey = TextUtility.CutLeft(encryptKey, length);
            encryptKey = encryptKey.PadRight(length, ' ');

            return encryptKey.Substring(0, length);
        }

        /// <summary>
        /// Base64Decode
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Base64Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Base64Encode
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Base64Encode(string message)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// DSA
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DSAEncryptPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            using DSACryptoServiceProvider dSACryptoServiceProvider = new DSACryptoServiceProvider();
            string text = BitConverter.ToString(dSACryptoServiceProvider.SignData(Encoding.UTF8.GetBytes(password)));
            dSACryptoServiceProvider.Clear();
            return text.Replace("-", null);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptPassword(string password)
        {
            return TextEncrypt.MD5EncryptPassword(password);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5EncryptPassword(string password)
        {
            return TextEncrypt.MD5EncryptPassword(password, MD5ResultMode.Strong);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="password"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string MD5EncryptPassword(string password, MD5ResultMode mode)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            using MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();

            byte[] passd = Encoding.UTF8.GetBytes(password);

            byte[] md5bytes = mD5CryptoServiceProvider.ComputeHash(passd);

            string text0;
            if (mode == MD5ResultMode.Strong)
            {
                text0 = BitConverter.ToString(md5bytes);
            }
            else
            {
                text0 = BitConverter.ToString(md5bytes, 4, 8);
            }

            string text1 = text0.Replace("-", null);

            //string text = BitConverter.ToString(mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(password)));
            mD5CryptoServiceProvider.Clear();

            return text1;

            //if (mode != MD5ResultMode.Strong)
            //{
            //    return text.Replace("-", null).Substring(8, 16);
            //}
            //return text.Replace("-", null);
        }

        /// <summary>
        /// SHA1
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string SHA1EncryptPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            using SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            string text = BitConverter.ToString(sHA1CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(password)));
            sHA1CryptoServiceProvider.Clear();
            return text.Replace("-", null);
        }

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string SHA256(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            using SHA256Managed sHA256Managed = new SHA256Managed();
            return Convert.ToBase64String(sHA256Managed.ComputeHash(bytes));
        }
    }

    /// <summary>
    /// 加密类型
    /// </summary>
    public enum MD5ResultMode : byte
    {
        /// <summary>
        /// 强的加密
        /// </summary>
        Strong,
        /// <summary>
        /// 弱的加密
        /// </summary>
        Weak
    }
}
