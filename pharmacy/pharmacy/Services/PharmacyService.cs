using HtmlAgilityPack;
using pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace pharmacy.Services
{
    public class PharmacyService
    {
        #region Singleton
        private static object lock_obj = new object();
        private static PharmacyService _instance;
        public static PharmacyService Instance
        {
            get
            {
                lock (lock_obj)
                {
                    return (_instance == null) ? _instance = new PharmacyService() : _instance;
                }
            }
        }
        private PharmacyService() { }
        #endregion

        public async Task<List<Pharmacy>> GetList(string city)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            string url = $"http://www.netdata.com/demo/nobetci-eczane/{city}-nobetci-eczaneleri";
            using (HttpClient client = new HttpClient())
            {
                var resposne = await client.GetAsync(url);
                if (resposne != null && resposne.IsSuccessStatusCode)
                {
                    PharmacyService ps = new PharmacyService();

                    string html = await resposne.Content?.ReadAsStringAsync();
                    return ParseList(html);

                }
            }
            return null;
        }

        private List<Pharmacy> ParseList(string html)
        {
            List<Pharmacy> pharmacies = new List<Pharmacy>();

            HtmlDocument hmtldoc = new HtmlDocument();
            hmtldoc.LoadHtml(html);
            var divs = hmtldoc.DocumentNode.SelectNodes("//div[@class='panel panel-danger']");
            if (divs.Count == 0) return pharmacies;
            foreach (var div in divs)      
            {

                try
                {
                    Pharmacy pharmacy = new Pharmacy();
                    var body = div.SelectSingleNode(".//div[@class='panel-body']");
                    pharmacy.Address = ReplaceChar(body.ChildNodes[1].ChildNodes[2].InnerText.Trim().Remove(0, 7));
                    if (string.IsNullOrEmpty(pharmacy.Address)) continue;
                    pharmacy.Name = ReplaceChar(div.ChildNodes[1].InnerText.Trim());
                    pharmacy.Phone = body.ChildNodes[1].ChildNodes[3].InnerText.Trim().Remove(0, 9);
                    pharmacy.City= ReplaceChar(body.ChildNodes[1].ChildNodes[0].InnerText.Trim().Remove(0, 4));
                    pharmacy.District= ReplaceChar(body.ChildNodes[1].ChildNodes[1].InnerText.Trim().Remove(0, 6));


                    pharmacies.Add(pharmacy);
                }
                catch (Exception)
                {

                    continue;
                }
            }

            return pharmacies;
        }

        private string ReplaceChar(string text)
        {
            if (text == "") return "";

            if (text.Contains("&#246;"))
            {
                text = text.Replace("&#246;", "ö");

            }
            if (text.Contains("&#252;"))
            {
                text = text.Replace("&#252;", "ü");
            }
            if (text.Contains("&#231;"))
            {
                text = text.Replace("&#231;", "ç");
            }
            if (text.Contains("&#220;"))
            {
                text = text.Replace("&#220;", "Ü");
            }
            if (text.Contains("&#199;"))
            {
                text = text.Replace("&#199;", "Ç");
            }
            if (text.Contains("&#214;"))
            {
                text = text.Replace("&#214;", "Ö");
            }
            return text;
        }
    }
}
