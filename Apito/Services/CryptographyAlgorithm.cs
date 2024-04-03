namespace Apito.Services;

using System.Security.Cryptography;
using System.Text;

public class CryptographyAlgorithm
{
    public CryptographyAlgorithm()
    {
        _cipherMode = CipherMode.CBC;
    }

    public string DecryptCipherTextToPlainText(string cipherText, string securityKey, string vectorAlgorithm)
    {
        byte[] toEncryptArray = Convert.FromBase64String(cipherText);
        byte[] iv = Convert.FromBase64String(vectorAlgorithm);
        var newDecrypt = Decrypt(toEncryptArray, CreateKey(securityKey), iv!);
        return UTF8Encoding.UTF8.GetString(newDecrypt);
    }

    private byte[] CreateKey(string key)
    {
        var objMD5CryptoService = MD5.Create();
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        objMD5CryptoService.Clear();
        return securityKeyArray;
    }

    private readonly CipherMode _cipherMode;
    private byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Mode = _cipherMode;
        aes.Key = key;
        aes.IV = iv;
        using (var cryptoTransform = aes.CreateDecryptor())
        {
            return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
        }
    }
}