using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace ZinioReaderWin8.Util
{
	// Token: 0x020000AF RID: 175
	internal class CryptoUtils
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x00025C1C File Offset: 0x00023E1C
		public static string GetPdfPassword(string cipher, string password, string deviceId, string uuid)
		{
			IBuffer iv = CryptographicBuffer.DecodeFromBase64String(cipher);
			IBuffer data = CryptographicBuffer.DecodeFromBase64String(password);
			string s = "8D}[" + deviceId.Substring(0, 4) + "i)|z" + uuid.Substring(0, 4);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			IBuffer keyMaterial = CryptographicBuffer.CreateFromByteArray(bytes);
			SymmetricKeyAlgorithmProvider symmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");
			CryptographicKey key = symmetricKeyAlgorithmProvider.CreateSymmetricKey(keyMaterial);
			IBuffer buffer = CryptographicEngine.Decrypt(key, data, iv);
			return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffer).Substring(0, 32);
		}
	}
}
