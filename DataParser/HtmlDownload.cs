using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using xNet;
using System.Timers;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace DataParser
{
    public static class HtmlDownload
    {
        public static string GetHtmlPage(string url)
        {
            //var request = new HttpRequest();

            //CookieDictionary cookie = new CookieDictionary(false);

            //request.Cookies = cookie;
            //request.UserAgent = Http.ChromeUserAgent();
            //request.KeepAlive = true;
            //request.AllowAutoRedirect = true;

            //var response = request.Get(GetCorrectUrl(url)).ToString();
            //if (response.Contains("<meta name=\"fragment\" content=\"!\""))
            //{
            //    response = request.Get(GetAjaxUrl(url)).ToString();
            //}

            //request.Close();

            //return response;

            IWebDriver driver = new FirefoxDriver();
            driver.Url = url;
            var a = driver.PageSource;
            return a;
        }

        static string GetCorrectUrl(string url)
        {
            if (!url.Contains("http"))
            {
                url = "http://" + url;
            }

            if (url.StartsWith("https://"))
            {
                url.Replace("https://", "http://");
            }

            return url;
        }

        static string GetAjaxUrl(string url)
        {
            url += url[url.Length - 1] != '/' ? "/" : String.Empty;
            url += "?_escaped_fragment_=";

            return url;
        }
    }
}
