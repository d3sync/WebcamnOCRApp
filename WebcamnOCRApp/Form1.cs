using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using IronOcr;

namespace WebcamnOCRApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FilterInfoCollection filterinfoCol;
        VideoCaptureDevice videocptdev;

        public TesseractConfiguration Configuration { get; private set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WebCamSample.WebCamDialog dlg = new WebCamSample.WebCamDialog();
            //dlg.ShowDialog();
            filterinfoCol = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterinfo in filterinfoCol)
                cboWebcam.Items.Add(filterinfo.Name);
            cboWebcam.SelectedIndex= 0;
            videocptdev = new VideoCaptureDevice();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            videocptdev = new VideoCaptureDevice(filterinfoCol[cboWebcam.SelectedIndex].MonikerString);
            videocptdev.NewFrame += Videocptdev_NewFrame;
            videocptdev.Start();
        }

        private void Videocptdev_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pic.Image = (Bitmap)eventArgs.Frame.Clone();
            //Tessaract((Bitmap)eventArgs.Frame.Clone());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videocptdev.IsRunning == true)
                videocptdev.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (videocptdev.IsRunning == true)
                videocptdev.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            picSnapped.Image = pic.Image;
            Tessaract(pic.Image);

        }
        private void Tessaract(Image img)
        {
            var Ocr = new IronTesseract();
            using (var Input = new OcrInput(img))
            {
                //Ocr.Language = OcrLanguage.EnglishBest;
                //Input.Deskew();  // use if image not straight
                //Input.DeNoise(); // use if image contains digital noise
                Ocr.Language = OcrLanguage.English;
                Ocr.AddSecondaryLanguage(OcrLanguage.Greek);
                Configuration = new TesseractConfiguration()
                {
                    ReadBarCodes = true,
                    //BlackListCharacters = "`ë|^",
                    //RenderSearchablePdfsAndHocr = true,
                    PageSegmentationMode = TesseractPageSegmentationMode.AutoOsd,
                };
                Input.Deskew();
                //Input.DeNoise();
                //Input.Dilate();
                Input.Sharpen();
                Input.ToGrayScale();
                Input.SaveAsImages("test", OcrInput.ImageType.PNG);
                //Process.Start("test_0.PNG");
                var Result = Ocr.Read(Input);
                Console.WriteLine(Result.Text);
                //richTextBox1.Clear();
                richTextBox1.Text = Result.Text;
                Console.WriteLine(Result.Lines.Length);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (videocptdev.IsRunning == true)
                button1.PerformClick();
        }
    }
}
