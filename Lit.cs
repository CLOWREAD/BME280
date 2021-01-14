using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
namespace Aqua_Lamp_Client
{
    class Lit
    {
        public async Task<Aqua_Lamp_Lit_Json> Get_Aqua_Lamp_Lit(String id)
        {
            HttpClient hc = new HttpClient();
            var res = await hc.GetAsync(Config_Client.m_Config.server_endpoint + "/api/lit/query_by_id/" + id);
            var res_str = await res.Content.ReadAsStringAsync();
            Aqua_Lamp_Lit_Json allj= (Aqua_Lamp_Lit_Json) JsonHelper.FromJson(res_str, typeof(Aqua_Lamp_Lit_Json));
            return allj;
        }

    }


    [System.Runtime.Serialization.DataContract]
    public class Aqua_Lamp_Lit_Json
    {
        [System.Runtime.Serialization.DataMember]
        public String Lamp_ID = "";
        [System.Runtime.Serialization.DataMember]
        public String Lit = "";
        [System.Runtime.Serialization.DataMember]
        public String Temperature = "";
        [System.Runtime.Serialization.DataMember]
        public String Script = "";
        [System.Runtime.Serialization.DataMember]
        public String Extra = "";

        [System.Runtime.Serialization.DataMember]
        public String Hello_Text = "";

        public void NotNull()
        {
            Type user_type = this.GetType();
            var prop_array = user_type.GetFields();
            for (int i = 0; i < prop_array.Length; i++)
            {
                if (prop_array[i].GetValue(this) == null)
                {
                    prop_array[i].SetValue(this, "");
                }
            }
        }
    }
}
