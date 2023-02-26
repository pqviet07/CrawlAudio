using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrawlAudio
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://phatphapungdung.com/sach-noi/suc-manh-cua-hien-tai-37047.html");
            HtmlNode listAdudio = document.DocumentNode.SelectSingleNode("//body/div[@id='td-outer-wrap']/div[2]/div[1]/div[2]/div[1]/div[1]/article[1]/div[2]/div[3]");

            int i = 0;
            string[] listUrl = new string[104];
            List<Task> tasks = new List<Task>();
            foreach (HtmlNode node in listAdudio.ChildNodes)
            {
                if (node.Name.Contains("a"))
                {
                    string strJson = node.GetAttributeValue("data-item", "");
                    JObject item = JObject.Parse(strJson);
                    JArray arr = JArray.FromObject(item["sources"]);
                    listUrl[i] = arr[0]["src"].ToString();
                    string title = node.ChildNodes[1].SelectSingleNode("span").GetDirectInnerText();
                    using (var client = new WebClient())
                    {
                        tasks.Add(client.DownloadFileTaskAsync(listUrl[i], i + ". " + title + ".mp3"));
                    }
                    i++;
                }
            }

            Task.WaitAll(tasks.ToArray());

        }

    }

}
