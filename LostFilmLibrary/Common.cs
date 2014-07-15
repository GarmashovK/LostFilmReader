using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LostFilmLibrary
{
    public static class Common
    {
        public static Encoding LFEncoding { get; set; }

        public static async Task<string> GetPage(string url)
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = LFOptions.Cookies;

            var client = new HttpClient(
                handler
                );

            var responseBytes = await client.GetByteArrayAsync(url);

            if (Common.LFEncoding == null)
                Common.LFEncoding = Encoding.GetEncoding("windows-1251");

            return Common.LFEncoding.GetString(responseBytes, 0, responseBytes.Length - 1);
        }


        internal static string ReplaceSomeSymbols(string str)
        {
            string[] oldSym =
            {
                "&#1042;","&mdash;","&#39;","&rdquo;","&ldquo;","&laquo;","&raquo;","&nbsp;","&quot;","amp;","&#039;","&ndash;"
            };
            string[] newSym = { "В", "-", "'", "”", "“", "«", "»", " ", "\"", "", "'", "–" };

            for (int i = 0; i < oldSym.Length; i++)
            {
                str = str.Replace(oldSym[i], newSym[i]);
            }

            return str;
        }
    }
}
