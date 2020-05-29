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