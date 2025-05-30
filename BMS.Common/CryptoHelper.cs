﻿using BMS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Common
{
    public static  class CryptoHelper
    {
        /// <summary>
        /// using SHA256 to compute hash of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ComputeHash(string input)
        {
            try
            {

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    return BitConverter.ToString(bytes).Replace("-", "").ToLower();

                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in ComputeHash: {ex.Message}", true);
                throw new Exception("Error in ComputeHash", ex);
            }
        }

        /// <summary>
        /// Encrypts a string using AES encryption
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string AesEncrypt(string input)
        {

            byte[] Key = GetValidatedKey("AESkey");
            byte[] IV = GetValidatedKey("AESIV");

            try
            {

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var msEncryptor = new MemoryStream())
                    {
                        using (var csEncryptor = new CryptoStream(msEncryptor, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncryptor = new StreamWriter(csEncryptor))
                            {
                                swEncryptor.Write(input);
                            }
                            return Convert.ToBase64String(msEncryptor.ToArray());
                        }
                    }

                }
            }catch (Exception ex)
            {
                Logger.LogError($"Error in AesEncrypt: {ex.Message}", true);
                throw new Exception("Error in AesEncrypt", ex);
            }
        }


        /// <summary>
        /// Decrypts a string using AES decryption
        /// </summary>
        /// <param name="encryptedinput"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string AesDecrypt(string encryptedinput)
        {
            if (string.IsNullOrEmpty(encryptedinput))
            {
                throw new ArgumentNullException(nameof(encryptedinput), "Input cannot be null or empty");
            }



            byte[] Key = GetValidatedKey("AESkey");
            byte[] IV = GetValidatedKey("AESIV");

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var msDecryptor = new MemoryStream(Convert.FromBase64String(encryptedinput)))
                    {
                        using (var csDecryptor = new CryptoStream(msDecryptor, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecryptor = new StreamReader(csDecryptor))
                            {

                                return srDecryptor.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in AesDecrypt: {ex.Message}", true);
                throw new Exception("Error in AesDecrypt", ex);
            }
        }


        private static byte[] GetValidatedKey(string settingKey)
        {
            string base64 = ConfigReader.GetSettings(settingKey);
            if (string.IsNullOrWhiteSpace(base64))
                throw new InvalidOperationException($"{settingKey} is not configured.");

            return Convert.FromBase64String(base64);
        }
    }
}
