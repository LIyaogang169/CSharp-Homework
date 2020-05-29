Crawler.cs文件
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CrawlerForm
{
    class Crawler
    {
        public string HostFilter { get; set; }
        public string FileFilter { get; set; }
        public int MaxPage { get; set; }
        public string StartURL { get; set; }
        public Encoding HtmlEncoding { get; set; }
        public event Action<Crawler> CrawlerStopped;
        public event Action<Crawler, string, string> PageDownloaded;

        private Dictionary<string, bool> done = new Dictionary<string, bool>();
        private Queue<string> pending = new Queue<string>();

        private readonly string urlDetectRegex = @"(href|HREF)[]*=[]*[""'](?<url>[^""'#>]+)[""']";
        public static readonly string urlParseRegex = @"^(?<site>https?://(?<host>[\w.-]+)(:\d+)?($|/))(\w+/)*(?<file>[^#?]*)";
      
        public Dictionary<string, bool> HaveDownloadedPages { get => done; } 

        public Crawler()
        {
            MaxPage = 100;
            HtmlEncoding = Encoding.UTF8;
        }

        public void Start()
        {
            done.Clear();
            pending.Clear();
            pending.Enqueue(StartURL);

            while (done.Count < MaxPage && pending.Count > 0)
            {
                string url = pending.Dequeue();
                try
                {
                    string html = DownLoad(url); 
                    done[url] = true;
                    PageDownloaded(this, url, "成功");
                    Parse(html, url);//解析
                }
                catch (Exception ex)
                {
                    PageDownloaded(this, url, "  错误:" + ex.Message);
                }
            }
            CrawlerStopped(this);
        }

        private string DownLoad(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string html = webClient.DownloadString(url);
            string fileName = done.Count.ToString();
            File.WriteAllText(fileName, html, Encoding.UTF8);
            return html;
        }

        private void Parse(string html, string pageUrl)
        {
            var matches = new Regex(urlDetectRegex).Matches(html);
            foreach (Match match in matches)
            {
                string linkUrl = match.Groups["url"].Value;
                if (linkUrl == null || linkUrl == "") continue;
                linkUrl = FixUrl(linkUrl, pageUrl);

                Match linkUrlMatch = Regex.Match(linkUrl, urlParseRegex);
                string host = linkUrlMatch.Groups["host"].Value;
                string file = linkUrlMatch.Groups["file"].Value;
                if (Regex.IsMatch(host, HostFilter) && Regex.IsMatch(file, FileFilter)
                  && !done.ContainsKey(linkUrl))
                {
                    pending.Enqueue(linkUrl);
                }
            }
        }

        //将相对路径转为绝对路径
        static private string FixUrl(string url, string pageUrl)
        {
            if (url.Contains("://"))
            {
                return url;
            }
            if (url.StartsWith("/"))
            {
                Match urlMatch = Regex.Match(pageUrl, urlParseRegex);
                String site = urlMatch.Groups["site"].Value;
                return site.EndsWith("/") ? site + url.Substring(1) : site + url;
            }

            if (url.StartsWith("../"))
            {
                url = url.Substring(3);
                int idx = pageUrl.LastIndexOf('/');
                return FixUrl(url, pageUrl.Substring(0, idx));
            }

            if (url.StartsWith("./"))
            {
                return FixUrl(url.Substring(2), pageUrl);
            }

            int end = pageUrl.LastIndexOf("/");
            return pageUrl.Substring(0, end) + "/" + url;
        }

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}




Form1.Designer.cs文件
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CrawlerForm
{
    class Crawler
    {
        public string HostFilter { get; set; }
        public string FileFilter { get; set; }
        public int MaxPage { get; set; }
        public string StartURL { get; set; }
        public Encoding HtmlEncoding { get; set; }
        public event Action<Crawler> CrawlerStopped;
        public event Action<Crawler, string, string> PageDownloaded;

        private Dictionary<string, bool> done = new Dictionary<string, bool>();
        private Queue<string> pending = new Queue<string>();

        private readonly string urlDetectRegex = @"(href|HREF)[]*=[]*[""'](?<url>[^""'#>]+)[""']";
        public static readonly string urlParseRegex = @"^(?<site>https?://(?<host>[\w.-]+)(:\d+)?($|/))(\w+/)*(?<file>[^#?]*)";
      
        public Dictionary<string, bool> HaveDownloadedPages { get => done; } 

        public Crawler()
        {
            MaxPage = 100;
            HtmlEncoding = Encoding.UTF8;
        }

        public void Start()
        {
            done.Clear();
            pending.Clear();
            pending.Enqueue(StartURL);

            while (done.Count < MaxPage && pending.Count > 0)
            {
                string url = pending.Dequeue();
                try
                {
                    string html = DownLoad(url); 
                    done[url] = true;
                    PageDownloaded(this, url, "成功");
                    Parse(html, url);//解析
                }
                catch (Exception ex)
                {
                    PageDownloaded(this, url, "  错误:" + ex.Message);
                }
            }
            CrawlerStopped(this);
        }

        private string DownLoad(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string html = webClient.DownloadString(url);
            string fileName = done.Count.ToString();
            File.WriteAllText(fileName, html, Encoding.UTF8);
            return html;
        }

        private void Parse(string html, string pageUrl)
        {
            var matches = new Regex(urlDetectRegex).Matches(html);
            foreach (Match match in matches)
            {
                string linkUrl = match.Groups["url"].Value;
                if (linkUrl == null || linkUrl == "") continue;
                linkUrl = FixUrl(linkUrl, pageUrl);

                Match linkUrlMatch = Regex.Match(linkUrl, urlParseRegex);
                string host = linkUrlMatch.Groups["host"].Value;
                string file = linkUrlMatch.Groups["file"].Value;
                if (Regex.IsMatch(host, HostFilter) && Regex.IsMatch(file, FileFilter)
                  && !done.ContainsKey(linkUrl))
                {
                    pending.Enqueue(linkUrl);
                }
            }
        }

        //将相对路径转为绝对路径
        static private string FixUrl(string url, string pageUrl)
        {
            if (url.Contains("://"))
            {
                return url;
            }
            if (url.StartsWith("/"))
            {
                Match urlMatch = Regex.Match(pageUrl, urlParseRegex);
                String site = urlMatch.Groups["site"].Value;
                return site.EndsWith("/") ? site + url.Substring(1) : site + url;
            }

            if (url.StartsWith("../"))
            {
                url = url.Substring(3);
                int idx = pageUrl.LastIndexOf('/');
                return FixUrl(url, pageUrl.Substring(0, idx));
            }

            if (url.StartsWith("./"))
            {
                return FixUrl(url.Substring(2), pageUrl);
            }

            int end = pageUrl.LastIndexOf("/");
            return pageUrl.Substring(0, end) + "/" + url;
        }

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}




Form1.cs文件
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CrawlerForm
{
    public partial class Form1 : Form
    {
        BindingSource ResultBindingSource = new BindingSource();
        Crawler crawler = new Crawler();

        public bool InvokeRequired { get; private set; }

        public Form1()
        {
            InitializeComponent();
            dgvResult.DataSource = ResultBindingSource;
            crawler.PageDownloaded += Crawler_PageDownloaded;
            crawler.CrawlerStopped += Crawler_CrawlerStopped;
        }

        private void Crawler_CrawlerStopped(Crawler obj)
        {
            Action action = () => lblInfo.Text = "爬虫停止";
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void Invoke(Action action)
        {
            throw new NotImplementedException();
        }

        private void Crawler_PageDownloaded(Crawler crawler, string url, string info)
        {
            var pageInfo = new { Index = ResultBindingSource.Count + 1, URL = url, Status = info };
            Action action = () => { ResultBindingSource.Add(pageInfo); };
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            ResultBindingSource.Clear();
            crawler.StartURL = txtUrl.Text;

            Match match = Regex.Match(crawler.StartURL, Crawler.urlParseRegex);
            if (match.Length == 0) return;
            string host = match.Groups["host"].Value;
            crawler.HostFilter = "^" + host + "$";
            crawler.FileFilter = ".html?$";

            Task task = Task.Run(() => crawler.Start());
            lblInfo.Text = "爬虫启动";
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
