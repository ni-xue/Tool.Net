using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tool.Utils.Encryption
{
	/// <summary>
	/// DES加密,秘钥长度为8位，不足时系统自动补足空字符。
	/// </summary>
	/// <remarks>代码由逆血提供支持</remarks>
	public class DES
	{
		///// <summary>
		///// 解密
		///// </summary>
		///// <param name="decryptString">密文</param>
		///// <param name="decryptKey">秘钥</param>
		///// <returns>返回明文</returns>
		//public static string Decrypt(string decryptString, string decryptKey)
		//{
		//	string result;
		//	try
		//	{
		//		decryptKey = TextUtility.CutLeft(decryptKey, 8);
		//		decryptKey = decryptKey.PadRight(8, ' ');
		//		byte[] bytes = Encoding.UTF8.GetBytes(decryptKey);
		//		byte[] keys = DES.Keys;
		//		byte[] array = Convert.FromBase64String(decryptString);
		//		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		//		MemoryStream memoryStream = new MemoryStream();
		//		CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(bytes, keys), CryptoStreamMode.Write);
		//		cryptoStream.Write(array, 0, array.Length);
		//		cryptoStream.FlushFinalBlock();
		//		result = Encoding.UTF8.GetString(memoryStream.ToArray());
		//	}
		//	catch
		//	{
		//		result = "";
		//	}
		//	return result;
		//}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="decryptString">密文</param>
		/// <param name="decryptKey">秘钥</param>
		/// <returns>返回明文</returns>
		public static string Decrypt(string decryptString, string decryptKey)
		{
			byte[] bytes = DecryptBuffer(Convert.FromBase64String(decryptString), decryptKey);
			if (bytes == null) return null;
			return Encoding.UTF8.GetString(bytes);
		}

		///// <summary>
		///// 加密
		///// </summary>
		///// <param name="encryptString">明文</param>
		///// <param name="encryptKey">秘钥</param>
		///// <returns>返回密文</returns>
		//public static string Encrypt(string encryptString, string encryptKey)
		//{
		//	encryptKey = TextUtility.CutLeft(encryptKey, 8);
		//	encryptKey = encryptKey.PadRight(8, ' ');
		//	byte[] bytes = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
		//	byte[] keys = DES.Keys;
		//	byte[] bytes2 = Encoding.UTF8.GetBytes(encryptString);
		//	DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		//	MemoryStream memoryStream = new MemoryStream();
		//	CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, keys), CryptoStreamMode.Write);
		//	cryptoStream.Write(bytes2, 0, bytes2.Length);
		//	cryptoStream.FlushFinalBlock();
		//	return Convert.ToBase64String(memoryStream.ToArray());
		//}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="encryptString">明文</param>
		/// <param name="encryptKey">秘钥</param>
		/// <returns>返回密文</returns>
		public static string Encrypt(string encryptString, string encryptKey)
		{
			return Convert.ToBase64String(EncryptBuffer(Encoding.UTF8.GetBytes(encryptString), encryptKey));

			//encryptKey = TextUtility.CutLeft(encryptKey, 8);
			//encryptKey = encryptKey.PadRight(8, ' ');
			//byte[] bytes = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
			//byte[] keys = DES.Keys;
			//byte[] bytes2 = Encoding.UTF8.GetBytes(encryptString);
			//DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			//MemoryStream memoryStream = new MemoryStream();
			//CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, keys), CryptoStreamMode.Write);
			//cryptoStream.Write(bytes2, 0, bytes2.Length);
			//cryptoStream.FlushFinalBlock();
			//return Convert.ToBase64String(memoryStream.ToArray());
		}

		/// <summary>
		/// 解密byte[]
		/// </summary>
		/// <param name="plainText">密文内容</param>
		/// <param name="encryptKey">密码密钥</param>
		/// <returns></returns>
		public static byte[] DecryptBuffer(byte[] plainText, string encryptKey)
		{
			byte[] result;
			try
			{
				//cipherkey = TextUtility.CutLeft(cipherkey, 8); cipherkey.PadRight(8, ' ');
				encryptKey = DES.GetPassword(encryptKey);

				using System.Security.Cryptography.DES myAes = System.Security.Cryptography.DES.Create(); //DESCryptoServiceProvider aesCryptoServiceProvider = new DESCryptoServiceProvider();
				using ICryptoTransform cryptoTransform = myAes.CreateDecryptor(Encoding.UTF8.GetBytes(encryptKey), DES.Keys);

				result = cryptoTransform.TransformFinalBlock(plainText, 0, plainText.Length);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// 加密byte[]
		/// </summary>
		/// <param name="plainText">原内容</param>
		/// <param name="encryptKey">密码密钥</param>
		/// <returns></returns>
		public static byte[] EncryptBuffer(byte[] plainText, string encryptKey)
        {
			//cipherkey = TextUtility.CutLeft(cipherkey, 8); cipherkey.PadRight(8, ' ');
			encryptKey = DES.GetPassword(encryptKey);
			using System.Security.Cryptography.DES myAes = System.Security.Cryptography.DES.Create(); //DESCryptoServiceProvider aesCryptoServiceProvider = new();
			using ICryptoTransform cryptoTransform = myAes.CreateEncryptor(Encoding.UTF8.GetBytes(encryptKey), DES.Keys);

			return cryptoTransform.TransformFinalBlock(plainText, 0, plainText.Length);
		}

		/// <summary>
		/// 获取DES实际加密密码
		/// </summary>
		/// <param name="encryptKey">原密码密钥</param>
		/// <returns></returns>
		public static string GetPassword(string encryptKey) 
		{
			return TextEncrypt.GetPassword(encryptKey, 8);
		}

		/// <summary>
		/// 加密规则,
		/// 18,
		///	52,
		///	86,
		///	120,
		///	144,
		///	171,
		///	205,
		///	239
		/// </summary>
		public static readonly byte[] Keys = new byte[]
		{
			18,
			52,
			86,
			120,
			144,
			171,
			205,
			239
		};
	}
}
