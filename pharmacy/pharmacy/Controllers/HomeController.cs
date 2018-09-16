using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pharmacy.Extensions;
using pharmacy.Models;

namespace pharmacy.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string city="adana")
        {
            city = city.ToLower().ClearText();
            List<Pharmacy> pharm = new List<Pharmacy>();
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://localhost:36098/api/pharmacy/"+city+"");
                pharm = JsonConvert.DeserializeObject<List<Pharmacy>>(json);
            }
            Uri uri = new Uri("https://maps.googleapis.com/maps/api/place/textsearch/xml?query=" + city.ToString()+"&key=AIzaSyCvxhdhk_fokH7HYKUF-2zndju8LAc7V9w&callback=initMap");

            string xmlStr;
            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(uri);
            }
            XDocument doc = XDocument.Parse(xmlStr);
            try
            {
                ViewBag.lat = doc.XPathSelectElement("/PlaceSearchResponse/result/geometry/location/lat").Value;
                ViewBag.lng = doc.XPathSelectElement("/PlaceSearchResponse/result/geometry/location/lng").Value;
            }
            catch (Exception)
            {

            }

            return View(pharm);
        }
        public JsonResult GetPharmacies(string pharm,string city)
        {
            Uri uri = new Uri("https://maps.googleapis.com/maps/api/place/textsearch/xml?query=" + city.ClearText() + " "+ pharm.Trim()+ "&key=AIzaSyCvxhdhk_fokH7HYKUF-2zndju8LAc7V9w&callback=initMap");

            string xmlStr;
            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(uri);
            }
            XDocument doc = XDocument.Parse(xmlStr);
            LatLong latLng = new LatLong();

            try
            {
                latLng.Lat = doc.XPathSelectElement("/PlaceSearchResponse/result/geometry/location/lat").Value;
                latLng.Lng = doc.XPathSelectElement("/PlaceSearchResponse/result/geometry/location/lng").Value;
                return Json(latLng);
            }
            catch (Exception)
            {
                return Json(false);

            }

            
        }
        private class LatLong
        {
            public string Lat { get; set; }
            public string Lng { get; set; }
        }
    }
   
}
