using System;
using System.Security.Cryptography;
using System.Text;
using ArokiaIT.Framework.Architecture;
using System.IO;

namespace ArokiaIT.Skipper.MobileService
{
    public class Security
    {
        public static string PAGE_NAME = "Security.cs : : ";

        public Security()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static string EncryptPassword(string Password)
        {
            string passwordHashSha1 = ComputeHash(Password, "SHA256", null);
            return passwordHashSha1;
        }

        public static bool ValidateUser(string Password, string EncryptedPassword)
        {
            bool validate = VerifyHash(Password, "SHA256", EncryptedPassword);
            return validate;
        }
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {
            string strComputeHashResult = string.Empty;
            try
            {
                if (saltBytes == null)
                {
                    int minSaltSize = 4;
                    int maxSaltSize = 8;
                    Random random = new Random();
                    int saltSize = random.Next(minSaltSize, maxSaltSize);
                    saltBytes = new byte[saltSize];
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    rng.GetNonZeroBytes(saltBytes);
                }
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] plainTextWithSaltBytes =
                        new byte[plainTextBytes.Length + saltBytes.Length];
                for (int i = 0; i < plainTextBytes.Length; i++)
                    plainTextWithSaltBytes[i] = plainTextBytes[i];
                for (int i = 0; i < saltBytes.Length; i++)
                    plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];
                HashAlgorithm hash;
                if (hashAlgorithm == null)
                    hashAlgorithm = "";
                switch (hashAlgorithm.ToUpper())
                {
                    case "SHA1":
                        hash = new SHA1Managed();
                        break;
                    case "SHA256":
                        hash = new SHA256Managed();
                        break;
                    case "SHA384":
                        hash = new SHA384Managed();
                        break;
                    case "SHA512":
                        hash = new SHA512Managed();
                        break;
                    default:
                        hash = new MD5CryptoServiceProvider();
                        break;
                }
                byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
                byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                    saltBytes.Length];
                for (int i = 0; i < hashBytes.Length; i++)
                    hashWithSaltBytes[i] = hashBytes[i];
                for (int i = 0; i < saltBytes.Length; i++)
                    hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];
                string hashValue = Convert.ToBase64String(hashWithSaltBytes);
                strComputeHashResult = hashValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.WriteErrorToPhysicalPath(PAGE_NAME + "ComputeHash() :" + ex.Message);
            }
            return strComputeHashResult;
        }
        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue)
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);
            int hashSizeInBits, hashSizeInBytes;
            if (hashAlgorithm == null)
                hashAlgorithm = "";
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }
            hashSizeInBytes = hashSizeInBits / 8;
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
            string expectedHashString =
                        ComputeHash(plainText, hashAlgorithm, saltBytes);
            return (hashValue == expectedHashString);
        }

        public static string Decryption(string ciphetText, string key)
        {
            if (ciphetText == null || ciphetText.Length < 8)
            {
                return null;
            }
            if (ciphetText == null || ciphetText.Length <= 0)
            {
                return null;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int charCode;
            for (int i = 0; i < key.Length; i++)
            {
                charCode = (Int32)key[i];
                sb.Append(charCode);
            }
            string tprand = sb.ToString();
            double prand = Double.Parse(sb.ToString());
            int sPos = Int32.Parse(Math.Floor(Double.Parse(tprand.ToString().Length.ToString()) / 5).ToString());
            string checkStr = String.Empty;
            for (int j = 1; j <= 5; j++)
            {
                try
                {
                    checkStr += tprand[sPos * j].ToString();
                }
                catch { }
            }
            checkStr = checkStr.Trim();
            double mult = 0;
            if (checkStr.StartsWith("0"))
                mult = Convert.ToInt32("1" + checkStr.Substring(1, checkStr.Length - 1));
            else
                mult = Int32.Parse(checkStr);
            int incr = Int32.Parse(Math.Round(Double.Parse(key.ToString().Length.ToString()) / 2, MidpointRounding.AwayFromZero).ToString());
            double modu = Math.Pow(2, 31) - 1;
            int salt = Int32.Parse(ciphetText.Substring(ciphetText.Length - 8, 8), System.Globalization.NumberStyles.HexNumber);
            // 16        
            prand += salt;
            tprand += salt.ToString();
            int iter = 0;
            string firstHalf = String.Empty;
            string secondHalf = String.Empty;
            Int64 iFirst = 0;
            Int64 iSecond = 0;
            Int64 addResult = 0;
            while (tprand.ToString().Length > 10)
            {
                firstHalf = tprand.ToString().Substring(0, 10);
                secondHalf = tprand.ToString().Substring(10, tprand.ToString().Length - 10);
                if (firstHalf.StartsWith("0"))
                    iFirst = Convert.ToInt64(firstHalf, 8);
                else if (firstHalf.StartsWith("8"))
                    iFirst = Convert.ToInt64(firstHalf, 10);
                else
                    iFirst = Int64.Parse(firstHalf);
                addResult = iFirst + iSecond;
                addResult = Convert.ToInt64(addResult.ToString(), 10);
                tprand = Convert.ToString(iFirst);
                iter++;
            }
            tprand = Convert.ToString((mult * Int64.Parse(tprand) + incr) % modu);
            Int64 enc_chr = 0;
            string enc_str = "";
            for (int i = 0; i < ciphetText.Length - 8; i += 2)
            {
                enc_chr = Int64.Parse((Int32.Parse(ciphetText.Substring(i, 2), System.Globalization.NumberStyles.HexNumber) ^ Int32.Parse((Math.Floor((Double.Parse(tprand) / modu) * 255)).ToString())).ToString());
                //16            
                enc_str += Convert.ToChar(enc_chr);
                tprand = Convert.ToString((mult * Int64.Parse(tprand) + incr) % modu);
            }
            return enc_str.ToString();
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Md5 Decrypt
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}