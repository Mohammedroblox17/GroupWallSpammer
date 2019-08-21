using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GroupWallSpammer
{
    class Program
    {

        static string CSRFToken(string cookie)
        {
            try
            {
                WebClient wb = new WebClient();
                wb.Headers["Cookie"] = ".ROBLOSECURITY=" + cookie;
                    Regex regex = new Regex("Roblox.XsrfToken.*?\'(.*)\'");
                    Match matched = regex.Match(wb.DownloadString("https://www.roblox.com/groups/4539492/The-Republic-Navy#!/about"));

                    if (matched.Success)
                    {
                        return matched.Groups[1].Value;
                    }
                else
                {
                    return "Failed";
                }

                
            }
            catch (WebException ex)
            {
              return "Failed";

            }
         

        }
        static void SendGroupMessage(string cookie,long id, string message)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://groups.roblox.com/v1/groups/{id.ToString()}/wall/posts");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add(HttpRequestHeader.Cookie, string.Format(".ROBLOSECURITY={0}", cookie));
            httpWebRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken(cookie));
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"body\":\"" + message + "\"}";

                streamWriter.Write(json);
            }
            var hi = (HttpWebResponse)httpWebRequest.GetResponse();
            Console.WriteLine("Sent message");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Message you wanna send");
            string Input = Console.ReadLine();
            Console.WriteLine("Enter Group Id");
            long Iput = long.Parse(Console.ReadLine());
            new Thread(() =>
            {
                using (var streamReader = File.OpenText("Cookies.txt"))
                {
                    var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        SendGroupMessage(line, Iput, Input);

                    }
                }

            }).Start();
        }
    }
}
