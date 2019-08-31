using com.MovieAssistant.core;
using com.MovieAssistant.core.DataStructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test1
{
    public partial class Form1 : Form
    {
        Data Data;
        public Form1()
        {
            InitializeComponent();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Model model = new Model()
            {
                BaseURL = "https://subscene.com",
                SiteNmae = "subscene",
                SearchEng = @"https://subscene.com/subtitles/searchbytitle"
            };
            model.SetRoot(3);
            model.AddNameValueCollection(model.RootGuid, "query", "@SearchWord");

            model.AddItem(model.RootGuid, "//div[@class='title']/a", "Name", LeafType.Data, false);
            model.AddItem(model.RootGuid, @"//div[@class='subtle count']", "Count", LeafType.Data, false);
            model.AddXpath(model.RootGuid, "//div[@class='title']/a/@href", 3);

            model.AddItem("//div[@class='title']/a/@href", @"//td[@class='a1']/a/span[not(@class)]", "Subtitle Name", LeafType.Data, false);
            model.AddItem("//div[@class='title']/a/@href", "//td[@class='a1']/a/span[@class]", "Language", LeafType.Data, false);
            model.AddXpath("//div[@class='title']/a/@href", "//td[@class='a1']/a/@href", 1);

            model.AddItem("//td[@class='a1']/a/@href", "//div [@class='download']/a/@href", "subtitle.zip", LeafType.Downloadable, true);

            model.Save("kian.json", SaveType.JSON);
            //var model = Model.Load("kian.json");

            Data = new Data(model, textBox1.Text);
            Data.SetFilter("//div[@class='title']/a", "//td[@class='a1']/a/span[@class]");
            Data.onFilter = (list) => { listBox1.Items.Clear(); listBox1.Items.AddRange(list); };
            string path = textBox2.Text;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Data.onFinish = () =>
            {
                var res = Data.ToList();
                var i = 0;
                foreach (var item in res)
                {
                    var newPath = "";
                    foreach (var item2 in item)
                        if (item2.Xpath == "//td[@class='a1']/a/span[@class]")
                        {
                            newPath = path + "\\" + item2.Value;
                            Directory.CreateDirectory(newPath);
                            break;
                        }
                    foreach (var item2 in item)
                        if (item2.Xpath == "//div [@class='download']/a/@href")
                        {
                            var client = new WebClient();
                            client.Encoding = Encoding.UTF8;
                            var str = newPath + @"\" + i + ".zip";
                            client.DownloadFile(model.BaseURL + "/" + item2.Value, str);
                            break;
                        }
                }
            };
            Data.Start(false);
        }
        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            Data.Filter(listBox1.SelectedItem as string);
            Data.Continue();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            var di = new FolderBrowserDialog() { SelectedPath = textBox2.Text };
            di.ShowDialog();
            textBox2.Text = di.SelectedPath;
        }
    }
}
