using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Web.Services;
using System.Configuration;
namespace testDevOps
{
    public partial class FindTelkom : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]

        public static List<tdata> SearchData(string search)
        {
            List<tdata> Results = new List<tdata>();

            string AccountUsername = ConfigurationManager.AppSettings["AccountUsername"];
            string AccountPassword = ConfigurationManager.AppSettings["AccountPassword"];
            string AccountDomain = ConfigurationManager.AppSettings["AccountDomain"];
            //HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://spsmigratetest.telkom.co.za/sites/GraphicDev/_vti_bin/listdata.svc/IntranetAlphaIndex?$filter=substringof('" + search + "',Title) and (StatusValue eq 'Active') &$orderby=Title asc");
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://telkomintranet.telkom.co.za/_vti_bin/listdata.svc/IntranetAlphaIndex?$filter=substringof('" + search + "',Title) and (StatusValue eq 'Active') &$orderby=Title asc");

            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";


            NetworkCredential cred = new System.Net.NetworkCredential(AccountUsername, AccountPassword, AccountDomain);
            endpointRequest.Credentials = cred;
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            try
            {
                WebResponse webResponse = endpointRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];
                foreach (JObject j in jarr)
                {
                    Results.Add(new tdata() { title = j["Title"].ToString(), url = j["URL"].ToString() });
                }
                responseReader.Close();


            }
            catch (Exception ex)
            {

            }

            return Results.ToList();
        }
        public class tdata
        {
            public string title { get; set; }
            public string url { get; set; }
        }
    }
}