using System; 
using System.Security; 
using System.Security.Cryptography; 
using System.IO; 
using System.Text; 

namespace KK.Frame.Util
{
    public class CryptHelper
    {
        // 
        const string sKey = "qMzGxh6hESZDVJeCnetGuxzaiBnNLQM3";
        //
        const string sIV = "qcDY6X+aPLw=";
        // 
        #region public string EncryptString(string Value) 
        /// <summary> 
        ///  
        /// </summary> 
        /// <param name="Value"></param> 
        /// <returns></returns> 
        public static string EncryptString(string Value)
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            // 
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            // 
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion

        #region public string DecryptString(string Value) 
        /// <summary> 
        ///  
        /// </summary> 
        /// <param name="Value"></param> 
        /// <returns></returns> 
        public static string DecryptString(string Value)
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion
    }
}