using com.MovieAssistant.core;
using com.MovieAssistant.core.DataStructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            Model modle = new Model()
            {
                BaseURL = "https://subscene.com",
                SiteNmae = "subscene",
                SearchEng = @"https://subscene.com/subtitles/searchbytitle"
            };
            modle.SetRoot(3);
            modle.AddNameValueCollection(modle.RootGuid, "query", "@SearchWord");

            modle.AddItem(modle.RootGuid, "//div[@class='title']/a", "Name", LeafType.Data, false);
            modle.AddItem(modle.RootGuid, @"//div[@class='subtle count']", "Count", LeafType.Data, false);
            modle.AddXpath(modle.RootGuid, "//div[@class='title']/a/@href", 3);

            modle.AddItem("//div[@class='title']/a/@href", @"//td[@class='a1']/a/span[not(@class)]", "Subtitle Name", LeafType.Data, false);
            modle.AddItem("//div[@class='title']/a/@href", "//td[@class='a1']/a/span[@class]", "Language", LeafType.Data, false);
            modle.AddXpath("//div[@class='title']/a/@href", "//td[@class='a1']/a/@href", 1);

            modle.AddItem("//td[@class='a1']/a/@href", @"//*[@id='downloadButton']/a/@href", "subtitle.zip", LeafType.Downloadable, true);

            modle.Save("kian.json", SaveType.JSON);

            Data = new Data(modle, textBox1.Text);
            Data.SetFilter("//div[@class='title']/a", "//td[@class='a1']/a/span[@class]");
            Data.onFilter = (list) => { listBox1.Items.Clear();listBox1.Items.AddRange(list); };
            Data.onFinish = () => { MessageBox.Show("Test"); };
            Data.Start(false);
            //var model = Model.Load("kian.json");
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            Data.Filter(listBox1.SelectedItem as string);
            Data.Continue();
        }

        private void ListBox2_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}
