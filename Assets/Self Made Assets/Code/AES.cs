using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace AES
{
	public class General
	{

		/// <summary>
		/// Generate salt from password.
		/// </summary>
		/// <param name="password">Password string.</param>
		/// <returns>Salt bytes.</returns>
		static private byte[] SaltFromPassword(string password)
		{
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			HMACSHA1 hmac;
			hmac = new HMACSHA1(passwordBytes);
			byte[] salt = hmac.ComputeHash(passwordBytes);
			return salt;
		}

		/// <summary>
		/// Encrypt/Decrypt with Write method.
		/// </summary>
		/// <param name="cryptor"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		static private byte[] CipherStreamWrite(ICryptoTransform cryptor, byte[] input)
		{
			byte[] inputBuffer = new byte[input.Length];
			// Copy data bytes to input buffer.
			System.Buffer.BlockCopy(input, 0, inputBuffer, 0, inputBuffer.Length);
			// Create a MemoryStream to hold the output bytes.
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			// Create a CryptoStream through which we are going to be processing our data.
			CryptoStreamMode mode;
			mode = CryptoStreamMode.Write;
			System.Security.Cryptography.CryptoStream cryptoStream;
			cryptoStream = new System.Security.Cryptography.CryptoStream(stream, cryptor, mode);
			// Start the crypting process.
			cryptoStream.Write(inputBuffer, 0, inputBuffer.Length);
			// Finish crypting.
			cryptoStream.FlushFinalBlock();
			// Convert data from a memoryStream into a byte array.
			byte[] outputBuffer = stream.ToArray();
			// Close both streams.
			stream.Close();
			cryptoStream.Close();
			return outputBuffer;
		}

		#region Encrypt
		/// <summary>
		/// Encrypt string with AES-256 by using password.
		/// </summary>
		/// <param name="password">Password string.</param>
		/// <param name="s">String to encrypt.</param>
		/// <returns>Encrypted Base64 string.</returns>
		static public string EncryptToBase64(string password, string s)
		{
			// Turn input strings into a byte array.
			byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
			// Get encrypted bytes.
			byte[] encryptedBytes = Encrypt(password, bytes);
			// Convert encrypted data into a base64-encoded string.
			string base64String = System.Convert.ToBase64String(encryptedBytes);
			// Return encrypted string.
			return base64String;
		}

		/// <summary>
		/// Encrypt string with AES-256 by using password.
		/// </summary>
		/// <param name="password">String password.</param>
		/// <param name="bytes">Bytes to encrypt.</param>
		/// <returns>Encrypted bytes.</returns>
		static public byte[] Encrypt(string password, byte[] bytes)
		{
			// Create a encryptor.
			ICryptoTransform encryptor = GetTransform(password);
			// Return encrypted bytes.
			return CipherStreamWrite(encryptor, bytes);
		}

		static private ICryptoTransform GetTransform(string password)
		{
			RijndaelManaged cipher = new RijndaelManaged();
			byte[] salt = SaltFromPassword(password);
			Rfc2898DeriveBytes secretKey = new Rfc2898DeriveBytes(password, salt, 10);
			byte[] key = secretKey.GetBytes(32);
			byte[] iv = secretKey.GetBytes(16);
			ICryptoTransform cryptor = cipher.CreateEncryptor(key, iv);
			return cryptor;
		}
		#endregion

		#region Decrypt
		private static ICryptoTransform GetDecryptorTransform(string password)
		{
			var cipher = new RijndaelManaged();
			var salt = SaltFromPassword(password);
			var secretKey = new Rfc2898DeriveBytes(password, salt, 10);
			return cipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
		}

		/// <summary>
		/// Decrypt string with AES-256 by using password key.
		/// </summary>
		/// <param name="password">String password.</param>
		/// <param name="base64String">Encrypted Base64 string.</param>
		/// <returns>Decrypted string.</returns>
		public static string DecryptFromBase64(string password, string base64String)
		{
			try
			{
				var encryptedBytes = Convert.FromBase64String(base64String);
				var bytes = CipherStreamWrite(GetDecryptorTransform(password), encryptedBytes);
				return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
			}
			catch { return ""; }
		}
		#endregion
	}

}
