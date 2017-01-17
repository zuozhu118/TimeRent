using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeRent.utils;

namespace TimeRent.utils
{
    public class PhoneMessage
    {

        public string GoMessage(string PhoneNumber,string[] Context) {
            string ret = "";
          //  CCPRestSDK api = new CCPRestSDK();
            CCPRestSDK api = new CCPRestSDK();
            bool isInit = api.init("app.cloopen.com", "8883");
            api.setAccount("8a48b5514ff923b4015003a9bafb1b0c",
            "d2b4ee9faec44ac6966e55206db36f52");
            api.setAppId("aaf98f894ff91386015003b407ac0984");
            try
            {
                if (isInit)
                {
                    Dictionary<string, object> retData = api.SendTemplateSMS(PhoneNumber,"40757",Context);
                    ret = getDictionaryData(retData);
                }
                else
                {
                    ret = "初始化失败";
                }
                return ret;
            }
            catch (Exception exc)
            {
                return ret = exc.Message;
            }
            finally
            {
                
            }
        }
        private string getDictionaryData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += getDictionaryData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret; 
        
        }
    }
}