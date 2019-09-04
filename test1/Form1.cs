using com.MovieAssistant.core.DataStructure;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ValueType = com.MovieAssistant.core.DataStructure.ValueType;

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
            Data.onFinish = () =>
            {
                var res = Data.ToList(ValueType.Xpath);
                var i = 0;
                foreach (var item in res)
                {
                    var newPath = $"{path}\\{item.Get("//td[@class='a1']/a/span[@class]")}\\";
                    if (!Directory.Exists($"{path}\\{item.Get("//td[@class='a1']/a/span[@class]")}"))
                        Directory.CreateDirectory($"{path}\\{item.Get("//td[@class='a1']/a/span[@class]")}");
                    var client = new WebClient();
                    client.Encoding = Encoding.UTF8;
                    client.OpenRead(model.BaseURL + item.Get("//div [@class='download']/a/@href"));
                    if (!string.IsNullOrEmpty(client.ResponseHeaders["Content-Disposition"]))
                        newPath += client.ResponseHeaders["Content-Disposition"].Substring(client.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
                    else
                        newPath += i++ + ".zip";
                    client.DownloadFile(model.BaseURL + item.Get("//div [@class='download']/a/@href"),newPath);
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
