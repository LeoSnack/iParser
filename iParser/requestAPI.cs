using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iParser
{
    class requestAPI
    {
        dataControl data = new dataControl();

        private string getHTML()
        {
            string url = "https://yandex.by/maps/213/moscow/?mode=search&text=кино";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Accept = "application/json, text/plain, */*";
            request.Headers.Add("Accept-Language: ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Host = "yandex.by";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36";
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding encode = Encoding.UTF8;

            string html;
            using (var reader = new StreamReader(response.GetResponseStream(), encode))
            {
                var myStr = reader.ReadToEnd();
                byte[] bytes = Encoding.UTF8.GetBytes(myStr);
                html = Encoding.Default.GetString(bytes); 
            }
            return html;
        }

        private string getToken(string token)
        {
            int index = token.IndexOf("csrfToken");
            token = token.Remove(0, index + 12);
            token = token.Remove(token.IndexOf("\""), token.Length - token.IndexOf("\""));

            return token;
        }

        private string getSessionId(string si)
        {
            int index = si.IndexOf("sessionId");
            si = si.Remove(0, index + 12);
            si = si.Remove(si.IndexOf("\""), si.Length - si.IndexOf("\""));

            return si;
        }

        private string getYandexuid(string uid)
        {
            int index = uid.IndexOf("yandexuid");
            uid = uid.Remove(0, index + 10);
            uid = uid.Remove(uid.IndexOf("&"), uid.Length - uid.IndexOf("&"));

            return uid;
        }

        public void mainReq()
        {   

            string html = getHTML();

            string url = "https://yandex.by/maps/api/search?ajax=1&csrfToken=" + getToken(html) + "&experimental_maxadv=5&lang=ru_UA&ll=37.62039306999999%2C55.75395964346406&origin=maps-scroll&results=40&sessionId=" + getSessionId(html) + "&skip=40&spn=2.515869140624993%2C0.8317851593357162&text=кино";
            

            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("_ym_d", data.getUnixTime(), "/", "yandex.by"));
            cookieContainer.Add(new Cookie("yandexuid", getYandexuid(html), "/", "yandex.by"));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookieContainer;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json, text/plain, */*";
            request.Headers.Add("Accept-Language: ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Host = "yandex.by";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.146 Safari/537.36";


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding encode = Encoding.UTF8;


            using (var reader = new StreamReader(response.GetResponseStream(), encode))
            {
                var myStr = reader.ReadToEnd();
                byte[] bytes = Encoding.UTF8.GetBytes(myStr);
                string responseText = Encoding.UTF8.GetString(bytes);

                dynamic json = JObject.Parse(responseText);
            }
        }
    }
}
