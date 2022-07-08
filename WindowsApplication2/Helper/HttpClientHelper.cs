using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WindowsApplication2.VO;

namespace WindowsApplication2.Helper
{
    class HttpClientHelper
    {


        public static string DoPost(string strJson,string strAction)
        {

            string url = commUntil.GetConntionSetting("U9API");
            //string url = "http://localhost/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/DO";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            strJson = strJson.Replace("\"", "'").Replace("$","\\\"");
            string OrgCode = DataHelper.getStr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
            string UserCode = DataHelper.getStr(Login.u9ContentHt["UserCode"]);//上下文用户编码
            string EntCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseID"].ToString();
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"" + EntCode + "\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + strJson + "\",\"action\":\"" + strAction + "\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Dictionary<string, object> d = new Dictionary<string, object>();

            d = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            return d["d"] == null ? "" : d["d"].ToString();


            //Dictionary<string, string> d = new Dictionary<string, string>();
            //Dictionary<string, string> result = new Dictionary<string, string>();
            //d = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            //return response.Content;
            //result = (JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(d["d"]));
            //return result["msg"];
            //return response.Content;


            ////string strURL = string.Format("http://{0}/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/Do", XMLUtilHelper.GetConfigUrl("URL"));
            //string strUrl = commUntil.GetConntionSetting("U9API");
            //System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            //request.Method = "POST";
            //HttpWebRequest.DefaultWebProxy = null;
            //request.ContentType = "application/json;charset=utf-8";
            //string EntCode =DataHelper.getStr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
            //string UserCode = DataHelper.getStr(Login.u9ContentHt["UserCode"]);//上下文用户编码
            //strJson = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"01\",\"OrgCode\":\"" + EntCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":'" + strJson + "',\"action\":\"" + strAction + "\"}";
            //byte[] param = System.Text.Encoding.UTF8.GetBytes(strJson);

            //request.ContentLength = param.Length;
            //System.IO.Stream writer = request.GetRequestStream();
            //writer.Write(param, 0, param.Length);
            //writer.Close();
            //System.Net.HttpWebResponse response;
            //try
            //{
            //    response = (System.Net.HttpWebResponse)request.GetResponse();
            //}
            //catch (WebException ex)
            //{
            //    response = (System.Net.HttpWebResponse)ex.Response;
            //}

            //System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //string strResult = myreader.ReadToEnd();
            //myreader.Close();
            //ResultVO vo = JsonConvert.DeserializeObject<ResultVO>(strResult);
            //return vo.Success ? "" : vo.Message;
        }
    }
}
