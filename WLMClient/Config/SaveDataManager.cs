using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace WLMClient.Config
{
    class SaveDataManager
    {
        public static SaveData GetConfiguration()
        {
            try
            {
                XmlDocument configDoc;
                XmlNode configNode;

                configDoc = new XmlDocument();

                configDoc.Load("SaveData.xml");
                configNode = configDoc.DocumentElement.SelectNodes("/config")[0];

                string rememberId = configNode.SelectSingleNode("OPTION_REMEMBER_ID").InnerText;
                string rememberPassword = configNode.SelectSingleNode("OPTION_REMEMBER_PASSWORD").InnerText;
                string autoLogin = configNode.SelectSingleNode("OPTION_LOGIN_AUTO").InnerText;
                string saveID = configNode.SelectSingleNode("SAVE_ID").InnerText;
                string savePass = Base64Decode(configNode.SelectSingleNode("SAVE_PASS").InnerText);

                return new SaveData(Convert.ToBoolean(Convert.ToInt16(rememberId)), Convert.ToBoolean(Convert.ToInt16(rememberPassword)),
                    Convert.ToBoolean(Convert.ToInt16(autoLogin)), saveID, savePass);
            }
            catch
            {
                CreateXMLFile();

                return new SaveData(false, false, false, "", "");
            }
        }

        public static void CreateXMLFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"ISO - 8859 - 1\"?><config><OPTION_REMEMBER_ID>0</OPTION_REMEMBER_ID><OPTION_REMEMBER_PASSWORD>0</OPTION_REMEMBER_PASSWORD><OPTION_LOGIN_AUTO>0</OPTION_LOGIN_AUTO><SAVE_ID></SAVE_ID><SAVE_PASS></SAVE_PASS></config>"); //Your string here
            
            XmlTextWriter writer = new XmlTextWriter("SaveData.xml", null);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);
        }

        public static void SaveConfiguration(SaveData configuration)
        {
            try
            {
                XmlDocument configDoc;
                XmlNode configNode;

                configDoc = new XmlDocument();

                configDoc.Load("SaveData.xml");

                configNode = configDoc.DocumentElement.SelectNodes("/config")[0];

                configNode.SelectSingleNode("OPTION_REMEMBER_ID").InnerText = Convert.ToInt32(configuration.rememberId).ToString();
                configNode.SelectSingleNode("OPTION_REMEMBER_PASSWORD").InnerText = Convert.ToInt32(configuration.rememberPassword).ToString();
                configNode.SelectSingleNode("OPTION_LOGIN_AUTO").InnerText = Convert.ToInt32(configuration.autoLogin).ToString();
                configNode.SelectSingleNode("SAVE_ID").InnerText = configuration.saveId;
                configNode.SelectSingleNode("SAVE_PASS").InnerText = Base64Encode(configuration.savePass);

                configDoc.Save("SaveData.xml");
            }
            catch (Exception e)
            {
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
