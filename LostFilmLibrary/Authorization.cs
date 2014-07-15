using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace LostFilmLibrary
{    
    public static class Authorization
    {
        public class IncorrectData : Exception{}
        //public static string Result { get; private set; }
        //public static string ResponseMsg { get; private set; }
        //private static HttpResponseMessage Response { get; set; }
        //private static Uri url { get; set; }
        //private static HttpClient client { get; set; }
        //private static HttpContent PostData { get; set; }

        private class LoginData
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        private class BogiData
        {
            public string actionUrl { get; set; }
            public string iehack { get; set; }
            public string sid { get; set; }
            public string uid { get; set; }
            public string first { get; set; }
            public string last { get; set; }
            public string nick { get; set; }
            public string ava_small { get; set; }
            public string ava_medium { get; set; }
            public string ava_big { get; set; }
            public string sex { get; set; }
            public string transfer { get; set; }
            public string email { get; set; }
            public string sig { get; set; }
        }
   
        public static async Task<CookieContainer> TryLogin(string login, string password)
        {
            var data = new LoginData { Login = login, Password = password };
            var url = new Uri("http://login1.bogi.ru/login.php?referer=http%3A%2F%2Fwww.lostfilm.tv%2F");

            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.AllowAutoRedirect = false;            

            var client = new HttpClient(handler);
            

            HttpContent postData = GetPostLoginData(data);
            HttpResponseMessage response = null;

            try
            {
                response = await client.PostAsync(url, postData);
                response.EnsureSuccessStatusCode();

                //string headerLoaction = response.Headers.First(n => n.Key == "Location").Value.ElementAt(0);

                //if (headerLoaction == "http://www.lostfilm.tv/blg.php?code=6&text=incorrect login/password")
                //    throw new IncorrectData();
                //Result = await Response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                if (response != null && response.ReasonPhrase == "Moved Temporarily")
                {
                    string headerLoaction = response.Headers.First(n => n.Key == "Location").Value.ElementAt(0);

                    if (headerLoaction == "http://www.lostfilm.tv/blg.php?code=6&text=incorrect%20login/password")
                        throw new IncorrectData();
                }
            }

            string result = await response.Content.ReadAsStringAsync();

            //string result = new StreamReader(await response.Content.ReadAsStreamAsync(),
            //    Encoding.GetEncoding(1251)).ReadToEnd();

            return await GetCookiesAsync(result);
        }

        private static async Task<CookieContainer> GetCookiesAsync(string responseStr)
        {
            responseStr = responseStr.Replace("</form>", "");
            var bogiData = SetBogiData(responseStr);
            var url = new Uri(bogiData.actionUrl);
            
            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.UseCookies = true;
            handler.UseDefaultCredentials = false;
            handler.CookieContainer.Add(new Uri("http://bogi.ru/"), new Cookie("bsid", bogiData.sid));
            handler.AllowAutoRedirect = false;            

            var client = new HttpClient(handler);
            SetHeaders(client);
            
            HttpContent postData = GetPostDataForLostFilm(bogiData);
            HttpResponseMessage response = null;

            try
            {
                response = await client.PostAsync(url, postData);                
            }
            finally
            {
            }

            return GetCookieContainer(response);
        }
                
        private static CookieContainer GetCookieContainer(HttpResponseMessage response)
        {
            var tempCookies = new CookieContainer();

            var setCookieNodes = response.Headers
                .First(n => n.Key == "Set-Cookie").Value;

            string cookieName;
            string cookieValue;
            var cookieUri = new Uri("http://www.lostfilm.tv/");

            foreach (var item in setCookieNodes)
            {
                int sign = item.IndexOf('=');

                cookieName = item.Substring(0, sign);
                cookieValue = item.Substring(sign + 1,
                    item.IndexOf(';') - sign - 1);

                tempCookies.Add(cookieUri,
                    new Cookie(cookieName, cookieValue));                
            }

            return tempCookies;
        }
        
        //public static string GetHeaders()
        //{
        //    string s = "";
        //    foreach (var item in Response.Headers)
        //    {
        //        s += item.Key + "\n";

        //        foreach (var item_value in item.Value)
        //        {
        //            s += "\t" + item_value + "\n";
        //        }
        //    }
        //    return s;
        //}
        
        private static HttpContent GetPostDataForLostFilm(BogiData data)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("iehack", data.iehack));
            postData.Add(new KeyValuePair<string, string>("sid", data.sid));
            postData.Add(new KeyValuePair<string, string>("uid", data.uid));
            postData.Add(new KeyValuePair<string, string>("first", data.first));
            postData.Add(new KeyValuePair<string, string>("last", data.last));
            postData.Add(new KeyValuePair<string, string>("nick", data.nick));
            postData.Add(new KeyValuePair<string, string>("ava_small", data.ava_small));
            postData.Add(new KeyValuePair<string, string>("ava_medium", data.ava_medium));
            postData.Add(new KeyValuePair<string, string>("ava_big", data.ava_big));
            postData.Add(new KeyValuePair<string, string>("sex", data.sex));
            postData.Add(new KeyValuePair<string, string>("transfer", data.transfer));
            postData.Add(new KeyValuePair<string, string>("email", data.email));
            postData.Add(new KeyValuePair<string, string>("sig", data.sig));

            return new FormUrlEncodedContent(postData);
        }

        private static BogiData SetBogiData(string htmlData)
        {            
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlData);

            var body = doc.DocumentNode.Descendants()
                .First(n => n.Name == "body");
            
            var bogiData = new BogiData();
            bogiData.actionUrl = body.ChildNodes
                .First(n => n.Name == "form").Attributes["action"].Value;
            bogiData.iehack = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "iehack").Attributes["value"].Value;
            bogiData.sid = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")  
                && n.Attributes["name"].Value == "sid").Attributes["value"].Value;
            bogiData.uid = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "uid").Attributes["value"].Value;
            bogiData.first = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "first").Attributes["value"].Value;
            bogiData.last = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "last").Attributes["value"].Value;
            bogiData.nick = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "nick").Attributes["value"].Value;
            bogiData.ava_small = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "ava_small").Attributes["value"].Value;
            bogiData.ava_medium = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "ava_medium").Attributes["value"].Value;
            bogiData.ava_big = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "ava_big").Attributes["value"].Value;
            bogiData.sex = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "sex").Attributes["value"].Value;
            bogiData.transfer = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "transfer").Attributes["value"].Value;
            bogiData.email = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "email").Attributes["value"].Value;
            bogiData.sig = body.ChildNodes.First(n => n.Name == "input" && n.Attributes.Contains("name")
                && n.Attributes["name"].Value == "sig").Attributes["value"].Value;

            return bogiData;
        }


        private static void SetHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            client.DefaultRequestHeaders.Add("Origin" ,"http://login.bogi.ru");
            client.DefaultRequestHeaders.Add("Referer", "http://login.bogi.ru/login.php?referer=http%3A%2F%2Fwww.lostfilm.tv%2F");
        }

        private static HttpContent GetPostLoginData(LoginData data)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("login", data.Login));
            postData.Add(new KeyValuePair<string, string>("password", data.Password));
            postData.Add(new KeyValuePair<string, string>("module", "1"));
            postData.Add(new KeyValuePair<string, string>("target", "http://lostfilm.tv/"));
            postData.Add(new KeyValuePair<string, string>("repage", "user"));
            postData.Add(new KeyValuePair<string, string>("act", "login"));

            return new FormUrlEncodedContent(postData);
        }
    }
}
