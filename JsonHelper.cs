using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Aqua_Lamp_Client
{
    public class JsonHelper
    {
        public static string ToJson(Object obj, Type type)
        {

            MemoryStream ms = new MemoryStream();

            DataContractJsonSerializer seralizer = new DataContractJsonSerializer(type);


            seralizer.WriteObject(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);

            StreamReader sr = new StreamReader(ms);
            string jsonstr = sr.ReadToEnd();

            //jsonstr = jsonstr.Replace("\"", "\\\"");

            sr.Close();
            ms.Close();
            return jsonstr;
        }
        public static Object FromJson(String jsonstr, Type type)
        {

            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonstr));

            DataContractJsonSerializer seralizer = new DataContractJsonSerializer(type);

            ms.Seek(0, SeekOrigin.Begin);

            Object res = seralizer.ReadObject(ms);


            ms.Close();
            return res;
        }
    }
}
