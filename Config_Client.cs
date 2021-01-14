using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
namespace Aqua_Lamp_Client
{
    [System.Runtime.Serialization.DataContract]
    public class Config_Client_Json
    {
        [System.Runtime.Serialization.DataMember]
        public string mongodb = "";

        [System.Runtime.Serialization.DataMember]
        public string client_id = "";

        [System.Runtime.Serialization.DataMember]
        public string server_endpoint = "";

        [System.Runtime.Serialization.DataMember]
        public int GPIO_BME280_SDA = 2;

        [System.Runtime.Serialization.DataMember]
        public int GPIO_BME280_SCL = 3;

        [System.Runtime.Serialization.DataMember]
        public int GPIO_BME280_ADDR = 0;

        [System.Runtime.Serialization.DataMember]
        public int GPIO_LIT = 4;

    }
    public class Config_Client
    {
        public static Config_Client_Json m_Config;
        public static void Load()
        {
            string path= AppContext.BaseDirectory;

            FileStream fs = File.Open(Path.Combine(path,"./config_client.json"), FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string jsonstr = sr.ReadToEnd();
            fs.Close();
            Config_Client_Json conf = (Config_Client_Json)JsonHelper.FromJson(jsonstr, typeof(Config_Client_Json));
            m_Config = conf;
          
        }
    }
}
