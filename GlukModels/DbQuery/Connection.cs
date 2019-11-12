using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace GlukLibrary.DbQuery
{
    public static class Connection
    {

        private static readonly string EncryptionKey = "Zh80OpeH9Y0fSZcjpElGgeVnXIhzlWbp1MDHvmoYGdHflD5L";
        private static readonly string Salt = "NZRUJ48wLd2g+3BS0tVmjA==";


        public static readonly string FileName = "\\dbConfig.xml";

        public static Dictionary<string, string> GetConnectionProperties(string pathDir)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>(20);
            FileStream fileStream = new FileStream(pathDir + FileName, FileMode.Open);

            XDocument xmlDoc = XDocument.Load(fileStream);
            if (xmlDoc == null)
            {
                throw new FileNotFoundException("File not found >> " + pathDir + FileName);
            }
            var elements = xmlDoc.Root?.Elements();
            if (elements == null)
            {
                throw new XmlSchemaException();
            }
            foreach (var element in elements)
            {
                properties.Add(element.Name.LocalName, 
                    element.Name.LocalName == "password" ? Decrypt(element.Value) : element.Value);
            }

            return properties;
        }

        public static void SaveConnectionProperties(Dictionary<string, string> connectionProperties, string pathDir)
        {
            List<XElement> elements = new List<XElement>(connectionProperties.Count);
            foreach (var property in connectionProperties)
            {
                elements.Add(new XElement(property.Key,property.Key == "password" ? Encrypt(property.Value) : property.Value));
            }
            XDocument xmlDoc = new XDocument(new XElement("DbConfig", elements));
            xmlDoc.Save(pathDir + FileName);
        }

        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                byte[] saltBytes = Convert.FromBase64String(Salt);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, saltBytes);
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

        //TODO make private
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                byte[] saltBytes = Convert.FromBase64String(Salt);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, saltBytes);
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

        public static string GetConnectionString(string dirPath)
        {
            var props = GetConnectionProperties(dirPath);
            return "server=" + props["server"] + ";database=" + props["dbName"] + ";port=" + props["port"] + ";user=" + props["user"] + ";password=" + props["password"];
        }

        public static string GetConnectionStringBase(string dirPath)
        {
            var props = GetConnectionProperties(dirPath);
            return "server=" + props["server"] + ";port=" + props["port"] + ";user=" + props["user"] + ";password=" + props["password"];
        }
    }
}
