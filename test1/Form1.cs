using com.MovieAssistant.core.DataStructure;
using Library.DataStructure.DataGrab;
using Library.DataStructure.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ValueType = com.MovieAssistant.core.DataStructure.ValueType;

namespace test1
{
    public partial class Form1 : Form
    {
        Data data;
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
            //model.AddNameValueCollection(model.RootGuid, "query", "@SearchWord");
            var m = new Method();
            m.URL.Add("strhttps://subscene.com/subtitles/searchbytitle");
            m.Keys.Add(new List<string>() { "strquery" });
            m.Values.Add(new List<string>() { "xpath" });
            var rootGuid = model.SetRoot(m);

            var f1Guid = model.AddItem(rootGuid, "//div[@class='title']/a", "Name", LeafType.Data, false);
            model.AddItem(rootGuid, @"//div[@class='subtle count']", "Count", LeafType.Data, false);
            var guid1 = model.AddXpath(rootGuid, "//div[@class='title']/a/@href");

            model.AddItem(guid1, @"//td[@class='a1']/a/span[not(@class)]", "Subtitle Name", LeafType.Data, false);
            var f2Guid = model.AddItem(guid1, "//td[@class='a1']/a/span[@class]", "Language", LeafType.Data, false);
            var guid2 = model.AddXpath(guid1, "//td[@class='a1']/a/@href");

            model.AddItem(guid2, "//div [@class='download']/a/@href", "subtitle.zip", LeafType.Downloadable, true);

            DataGrab dataGrab = new DataGrab(model, "Friends");
            dataGrab.SetFilter(f1Guid, f2Guid);
            dataGrab.Start();

            // model.Save("kian.json", SaveType.JSON);
            //var model = Model.Load("kian.json");

            /* data = new Data(model, textBox1.Text);
             data.SetFilter("//div[@class='title']/a", "//td[@class='a1']/a/span[@class]");
             data.onFilter = (list) => { listBox1.Items.Clear(); listBox1.Items.AddRange(list); };
             string path = textBox2.Text;*/


        }
        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            data.Filter(listBox1.SelectedItem as string);
            data.Continue();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            var di = new FolderBrowserDialog() { SelectedPath = textBox2.Text };
            di.ShowDialog();
            textBox2.Text = di.SelectedPath;
        }
    }
}
