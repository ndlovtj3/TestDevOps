using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Configuration;

namespace testDevOps
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DynamicGlobalLinks();
                DynamicHamburgerLinks();
                DynamicGlobalFooter();
            }
        }


        public void DynamicGlobalLinks()
        {

            string FileRef = String.Empty;
            string SiteName = String.Empty;
            string RedirectLink = String.Empty;
            string ElementType = String.Empty;
            string GlobalURL = ConfigurationManager.AppSettings["GlobalSearchURL"];
            string AccountUsername = ConfigurationManager.AppSettings["AccountUsername"];
            string AccountPassword = ConfigurationManager.AppSettings["AccountPassword"];
            string AccountDomain = ConfigurationManager.AppSettings["AccountDomain"];

            //string CurrentURL = ConfigurationManager.AppSettings["GlobalSearchURL"];
            HtmlGenericControl oCSS = new HtmlGenericControl(); /*Set the Css from the Library Elements */


            using (ClientContext clientContext = new ClientContext(GlobalURL))
            {
                NetworkCredential Credentials = new System.Net.NetworkCredential(AccountUsername, AccountPassword, AccountDomain);

                clientContext.Credentials = Credentials;

                Web web = clientContext.Web;

                clientContext.Load(web);

                Microsoft.SharePoint.Client.List siteList = web.Lists.GetByTitle("CentralNavigationAssets");

                clientContext.Load(siteList);

                CamlQuery camlQuery = new CamlQuery();


                camlQuery.ViewXml = @"<View><Query><Where><And><Eq><FieldRef Name='NavigationType'/><Value Type='Lookup'>Global Navigation</Value></Eq><Eq><FieldRef Name='Status'/><Value Type='Choice'>Active</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy></Query></View>";


                Microsoft.SharePoint.Client.ListItemCollection listCol = siteList.GetItems(camlQuery);

                clientContext.Load(listCol);

                clientContext.ExecuteQuery();

                // Now update the list.



                foreach (Microsoft.SharePoint.Client.ListItem oListItem in listCol)
                {
                    GlobalNav.InnerHtml += "<li> <a href=" + oListItem["RedirectLink"] + ">" + (oListItem.FieldValues["SiteName"] as FieldLookupValue).LookupValue.ToString() + "</a>";
                    Microsoft.SharePoint.Client.List subList = web.Lists.GetByTitle("SubCentralNavigationAssets");

                    clientContext.Load(subList);

                    CamlQuery subcamlQuery = new CamlQuery();


                    subcamlQuery.ViewXml = @"<View><Query><Where><And><Eq><FieldRef Name='SiteName'/><Value Type='Lookup'>" + (oListItem.FieldValues["SiteName"] as FieldLookupValue).LookupValue.ToString() + "</Value></Eq><Eq><FieldRef Name='Status'/><Value Type='Choice'>Active</Value></Eq></And></Where></Query></View>";


                    Microsoft.SharePoint.Client.ListItemCollection sublistCol = subList.GetItems(subcamlQuery);

                    clientContext.Load(sublistCol);

                    clientContext.ExecuteQuery();
                    if (sublistCol.Count > 0)
                    {

                        GlobalNav.InnerHtml += " <ul class=\"dropdown-menu\">";

                        foreach (Microsoft.SharePoint.Client.ListItem SubListItem in sublistCol)
                        {

                            GlobalNav.InnerHtml += "<li> <a href=" + SubListItem["RedirectLink"] + ">" + (SubListItem.FieldValues["SubSite"] as FieldLookupValue).LookupValue.ToString() + "</a> </li>";

                        }

                        GlobalNav.InnerHtml += "</ul>";
                    }
                    GlobalNav.InnerHtml += "</li>";

                }

                GlobalNav.InnerHtml += "<li><div class=\"findTelkom_site ui-widget\"><label>Find a Site</label><input type=\"text\" class=\"text findTelkom_siteInput sites\" value=\"Start typing here to search for a site...\" />"
                                     + "<p class=\"empty\"></p></div></li>";

            }
            //   return Json(globalNavLinks, JsonRequestBehavior.AllowGet);

        }
        public void DynamicHamburgerLinks()
        {
            string FileRef = String.Empty;
            string SiteName = String.Empty;
            string RedirectLink = String.Empty;
            string ElementType = String.Empty;
            string GlobalURL = ConfigurationManager.AppSettings["GlobalSearchURL"];
            // string CurrentURL = SPContext.Current.Site.Url; /*Gets the current URL of the Sitecollection*/
            string AccountUsername = ConfigurationManager.AppSettings["AccountUsername"];
            string AccountPassword = ConfigurationManager.AppSettings["AccountPassword"];
            string AccountDomain = ConfigurationManager.AppSettings["AccountDomain"];
            HtmlGenericControl oCSS = new HtmlGenericControl(); /*Set the Css from the Library Elements */


            using (ClientContext clientContext = new ClientContext(GlobalURL))
            {
                NetworkCredential Credentials = new System.Net.NetworkCredential(AccountUsername, AccountPassword, AccountDomain);

                clientContext.Credentials = Credentials;

                Web web = clientContext.Web;

                clientContext.Load(web);

                Microsoft.SharePoint.Client.List siteList = web.Lists.GetByTitle("CentralNavigationAssets");

                clientContext.Load(siteList);

                CamlQuery camlQuery = new CamlQuery();


                camlQuery.ViewXml = @"<View><Query><Where><And><Eq><FieldRef Name='NavigationType'/><Value Type='Lookup'>Hamburger Menu Item</Value></Eq><Eq><FieldRef Name='Status'/><Value Type='Choice'>Active</Value></Eq></And></Where></Query></View>";


                Microsoft.SharePoint.Client.ListItemCollection listCol = siteList.GetItems(camlQuery);

                clientContext.Load(listCol);

                clientContext.ExecuteQuery();

                // Now update the list.




                foreach (Microsoft.SharePoint.Client.ListItem oListItem in listCol)
                {




                    HamBurgerMenu.InnerHtml += "<li> <a href=" + oListItem["RedirectLink"] + " target=\"_blank\">" + (oListItem.FieldValues["SiteName"] as FieldLookupValue).LookupValue.ToString() + "</a> </li>";


                }

                HamBurgerMenu.InnerHtml += "<li class=\"searchItem\"><input type=\"text\" name=\"q\" class=\"form-control\" placeholder=\"Search\" id=\"txtSearch\" onkeypress=\"handle(event)\"><span class=\"glyphicon glyphicon-search\" style=\"color:white\" onclick=\"RouteToSearch($('#txtSearch').val());\"> </span></li>";

            }

        }

        public void DynamicGlobalFooter()
        {
            string GlobalURL = ConfigurationManager.AppSettings["GlobalSearchURL"];
            string AccountUsername = ConfigurationManager.AppSettings["AccountUsername"];
            string AccountPassword = ConfigurationManager.AppSettings["AccountPassword"];
            string AccountDomain = ConfigurationManager.AppSettings["AccountDomain"];
            string WebTitle = "TelkomDotNetFooter";
            HtmlGenericControl oCSS = new HtmlGenericControl(); /*Set the Css from the Library Elements */
            using (ClientContext clientContext = new ClientContext(GlobalURL))
            {
                NetworkCredential Credentials = new NetworkCredential(AccountUsername, AccountPassword, AccountDomain); //Credentials does not expire 

                clientContext.Credentials = Credentials;

                Web web = clientContext.Web;

                clientContext.Load(web);

                Microsoft.SharePoint.Client.List siteList1 = web.Lists.GetByTitle("Footer Templates");

                clientContext.Load(siteList1);

                CamlQuery camlQuery1 = new CamlQuery();


                camlQuery1.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + WebTitle + "</Value></Eq></Where></Query></View>";


                Microsoft.SharePoint.Client.ListItemCollection listCol1 = siteList1.GetItems(camlQuery1);

                clientContext.Load(listCol1);

                clientContext.ExecuteQuery();

                foreach (Microsoft.SharePoint.Client.ListItem oListItem1 in listCol1)
                {
                    Footer.InnerHtml += WebUtility.HtmlDecode(oListItem1["HTML"].ToString());
                }

            }
        }
        protected void OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                if (e.Item.Text == SiteMap.CurrentNode.Title)
                {
                    if (e.Item.Parent != null)
                    {
                        e.Item.Parent.Selected = true;
                    }
                    else
                    {
                        e.Item.Selected = true;
                    }
                }
            }
        }
    }
}