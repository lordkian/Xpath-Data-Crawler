using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XpathDataCrawler.DataGrab;
using XpathDataCrawler.DataStructure.Model;

namespace test1
{
    public partial class Form1 : Form
    {
        DataGrab dataGrab = null;
        Guid Guid = new Guid();
        public Form1()
        {
            InitializeComponent();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Model model = new Model()
            {
                BaseURL = "https://subscene.com",
                SiteNmae = "subscene"
            };
            var m = new Method();
            m.URL.Add("strhttps://subscene.com/subtitles/searchbytitle");
            m.Keys.Add(new List<string>() { "strquery" });
            m.Values.Add(new List<string>() { "xpath" });
            var rootGuid = model.SetRoot(m);

            var f1Guid = model.AddItem(rootGuid, "//div[@class='title']/a", "Name", LeafType.Data, false);
            model.AddItem(rootGuid, @"//div[@class='subtle count']", "Count", LeafType.Data, false);
            var guid1 = model.AddXpath(rootGuid, "//div[@class='title']/a/@href");

            model.AddItem(guid1, @"//td[@class='a1']/a/span[not(@class)]", "Subtitle Name", LeafType.Data, false);
            var f2Guid = model.AddItem(guid1, "//td[@class='a1']/a/span[@class='l r positive-icon']", "Language", LeafType.Data, false);
            var guid2 = model.AddXpath(guid1, "//td[@class='a1']/a/@href");

            model.AddItem(guid2, "//div [@class='download']/a/@href", "subtitle.zip", LeafType.Downloadable, true);

            dataGrab = new DataGrab(model, textBox1.Text);
            dataGrab.SetFilter(f1Guid, f2Guid);
            dataGrab.onFilter = OnFilter;
            dataGrab.onFinish = Finish;
            dataGrab.Start();

            button1.Click -= Button1_Click;

        }
        private void Button1_Click2(object sender, EventArgs e)
        {
            var data = new List<string>();
            foreach (var item in checkedListBox1.SelectedItems)
            {
                data.Add(item.ToString());
            }
            dataGrab.Filter(Guid, true, data.ToArray());
            button1.Click -= Button1_Click2;
            dataGrab.Continue();
        }
        private void OnFilter(Guid id, string xpath, string[] data)
        {
            Guid = id;
            if (button1.Text != "continue")
                button1.Text = "continue";
            button1.Click += Button1_Click2;
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.AddRange(data.Distinct().ToArray());
        }
        private void Finish(DataGrab dataGrab)
        {
            if (!Directory.Exists(textBox2.Text))
                Directory.CreateDirectory(textBox2.Text);
            dataGrab.Download(textBox2.Text);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            var di = new FolderBrowserDialog() { SelectedPath = textBox2.Text };
            di.ShowDialog();
            textBox2.Text = di.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var n = new System.Collections.Specialized.NameValueCollection();
            n.Add("query", textBox1.Text);
            DataGrab.DownloadData("https://subscene.com/subtitles/searchbytitle", n, textBox2.Text);

        }
    }
}
