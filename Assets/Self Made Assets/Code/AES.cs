using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using System.Text;

public class AES
{
    public static void encrypt(string s)
    {
        byte[] salt = Encoding.UTF8.GetBytes(s);
        int iterations = 1024;
        var rfc2898 =
        new Rfc2898DeriveBytes("JX19-2", salt, iterations);
        byte[] key = rfc2898.GetBytes(16);
        String keyB64 = Convert.ToBase64String(key);
        System.Console.WriteLine("Key: " + keyB64);

        AesManaged aesCipher = new AesManaged();
        aesCipher.KeySize = 128;
        aesCipher.BlockSize = 128;
        aesCipher.Mode = CipherMode.CBC;
        aesCipher.Padding = PaddingMode.PKCS7;
        aesCipher.Key = key;
        byte[] b = System.Text.Encoding.UTF8.GetBytes("JX19-2");
        ICryptoTransform encryptTransform = aesCipher.CreateEncryptor();
        byte[] ctext = encryptTransform.TransformFinalBlock(b, 0, b.Length);
        Debug.Log("IV:" + Convert.ToBase64String(aesCipher.IV));
        Debug.Log("Cipher text: " + Convert.ToBase64String(ctext));
    }
}
