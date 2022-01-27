using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Reportman.Drawing
{
    public static class Encrypter
    {

        public static class RijndaelSimple
        {

            #region Encriptar

            /// <summary>

            /// Método para encriptar un texto plano usando el algoritmo (Rijndael).

            /// Este es el mas simple posible, muchos de los datos necesarios los

            /// definimos como constantes.

            /// </summary>

            /// <param name="textToEncrypt">texto a encriptar</param>

            /// <returns>Texto encriptado</returns>

            public static string Encrypt(string textToEncrypt, string key)
            {

                return Encrypt(textToEncrypt,

                  key, "s@lAvz", "MD5", 1, "@1B2c3D4e5F6g7H8", 128);

            }

            /// <summary>

            /// Método para encriptar un texto plano usando el algoritmo (Rijndael)

            /// </summary>

            /// <returns>Texto encriptado</returns>

            public static string Encrypt(string textoQueEncriptaremos,

              string passBase, string saltValue, string hashAlgorithm,

              int passwordIterations, string initVector, int keySize)
            {

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                byte[] plainTextBytes = Encoding.UTF8.GetBytes(textoQueEncriptaremos);

                PasswordDeriveBytes password = new PasswordDeriveBytes(passBase,

                  saltValueBytes, hashAlgorithm, passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged()
                {

                    Mode = CipherMode.CBC
                };

                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes,

                  initVectorBytes);

                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor,

                 CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();

                byte[] cipherTextBytes = memoryStream.ToArray();

                memoryStream.Close();

                cryptoStream.Close();

                string cipherText = Convert.ToBase64String(cipherTextBytes);

                return cipherText;

            }

            #endregion

            #region Desencriptar

            /// <summary>

            /// Método para desencriptar un texto encriptado.

            /// </summary>

            /// <returns>Texto desencriptado</returns>

            public static string DeCrypt(string encryptedText, string key)
            {

                return Decrypt(encryptedText, key, "s@lAvz", "MD5",

                  1, "@1B2c3D4e5F6g7H8", 128);

            }

            /// <summary>

            /// Método para desencriptar un texto encriptado (Rijndael)

            /// </summary>

            /// <returns>Texto desencriptado</returns>

            public static string Decrypt(string textoEncriptado, string passBase,

              string saltValue, string hashAlgorithm, int passwordIterations,

              string initVector, int keySize)
            {

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                byte[] cipherTextBytes = Convert.FromBase64String(textoEncriptado);

                PasswordDeriveBytes password = new PasswordDeriveBytes(passBase,

                  saltValueBytes, hashAlgorithm, passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged()
                {

                    Mode = CipherMode.CBC
                };

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes,

                  initVectorBytes);

                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor,

                  CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0,

                  plainTextBytes.Length);

                memoryStream.Close();

                cryptoStream.Close();

                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0,

                  decryptedByteCount);

                return plainText;

            }

            #endregion

        }

    }
}
