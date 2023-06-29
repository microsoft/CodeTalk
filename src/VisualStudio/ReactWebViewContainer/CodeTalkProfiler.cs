using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace ReactWebViewContainer
{
    public partial class CodeTalkProfiler : Form
    {

        public ChromiumWebBrowser browser;
        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("http://localhost:3000");
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }
        public CodeTalkProfiler()
        {
            InitializeComponent();
            InitBrowser();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
