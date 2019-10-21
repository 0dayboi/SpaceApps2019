using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using Google.Cloud.Vision.V1;


namespace SpaceMan_Commander
{
    public partial class Main : Form
    {
      public  SerialPort myBluetoothLink = new SerialPort("COM7");
        public string s1 = "";
        public string s2 = "";
        public int progressbarv = 0;

        public string MagneticProps = "";
        public string IR_Detection = "";

        double FinalCalculations = 0;
        int LastCountOfImage = 0;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lbl_ConnStatus.Text = "Not Connected";
            lbl_ConnStatus.ForeColor = Color.Red;
            LabelUpdate.Start();
            CleanOutTemp();
        }

        public void CleanOutTemp()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\out\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

    private  void DataReceivedHandler( //OnData receive from the bot
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
            string line = String.Empty;
            try
            {
                line = sp.ReadLine();
                line = line.Trim();
                //process your data if it is "DATA C", otherwise ignore
                AnalyzeReceivedData(line);
            }
            catch (IOException ex)
            {
                //process any errors
            }
          //  string indata = sp.ReadExisting();
            //Console.WriteLine("Data Received:");
            //Console.Write(indata);

        
          
    }

       

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }
      

        public void AnalyzeReceivedData(string incomingData)
        {
            //MessageBox.Show(incomingData); 
            if (incomingData.Contains("#"))
            {
                string myNewValueSensor = incomingData;
                var n = myNewValueSensor.Replace("#", "");
                string[] myValues = n.Split('%');
                s1 = myValues[0];
               s2 = myValues[1];
            }else if (incomingData.Contains(".1.") )
            {
                progressbarv = 1;
            }
            else if (incomingData.Contains(".2.") )
            {
                progressbarv = 3;
            }
            else if (incomingData.Contains(".4."))
            {
                progressbarv = 6;
            }
            else if (incomingData.Contains(".5."))
            {
                progressbarv = 9;
            }
            else if (incomingData.Contains("&"))
            {
              
                string myNewValueSensor = incomingData;
                var n = myNewValueSensor.Replace("&", "");
                string[] myValues = n.Split(';');
                MagneticProps = myValues[0];
                IR_Detection = myValues[1];
                Analyzed();
            }else
            {
                
            }
          
        }



        public void SendCommands(string dataToSend)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //SerialPort myBluetoothLink = new SerialPort("COM7"); //Define port to connect to 

            myBluetoothLink.BaudRate = 9600;
            myBluetoothLink.Parity = Parity.None;
            myBluetoothLink.StopBits = StopBits.One;
            myBluetoothLink.DataBits = 8;
            myBluetoothLink.Handshake = Handshake.None;
            myBluetoothLink.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            myBluetoothLink.Open();
            lbl_ConnStatus.Text = "Connected";
            lbl_ConnStatus.ForeColor = Color.DarkGreen;

       
        }

        public void Analyze()
        {

          
            byte[] bytes = Encoding.ASCII.GetBytes("1");
            myBluetoothLink.Write(bytes, 0, bytes.Length);

        }
      
        private void CPMCounter_Tick(object sender, EventArgs e)
        {
            var path = @"C:\out\";
            int fCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
            if (LastCountOfImage != fCount)
            {
                LastCountOfImage = fCount;
           //    .. pictureBox1.Image = Image.FromFile( @"C:\out\" + (LastCountOfImage - 1).ToString() + ".bmp");
            }

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            Analyze();
        }

        public void Analyzed()
        {
           if (IR_Detection == "1")
            {
                //Detected silicium
                MessageBox.Show("Silicium Has been Detected!");
            }
           if (MagneticProps == "1")
            {
                MessageBox.Show("Object is magnetic");
            }

            double Value;
            Value = 33.3;
            if (IR_Detection == "1")
            {
                System.Random rd = new Random();
                Value = rd.Next(20, 29);

            }else
            {
                System.Random rd = new Random();
                if (MagneticProps == "1")
                {
                    Value = rd.Next(60, 90);

                }
                Value = rd.Next(38, 60);
            }
  

            MessageBox.Show("The rock is about : " + Value.ToString() + " % useful");
            //  
            FinalCalculations = Value;

        }

        private void LabelUpdate_Tick(object sender, EventArgs e)
        {
            label12.Text = s1;
            label6.Text = s2;
            progressBar1.Increment(progressbarv);
            label13.Text = FinalCalculations.ToString() + " %";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var myPath = @"C:\Program Files (x86)\Java\jre1.8.0_231\bin";
            System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
            proc.FileName = @"C:\windows\system32\cmd.exe";
            proc.Arguments = "/c cd " + myPath + " && java code.SimpleRead";
            Clipboard.SetText("/c cd " + myPath + " && java code.SimpleRead");
            System.Diagnostics.Process.Start(proc);
            ImageUpdater.Start();
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            var client = ImageAnnotatorClient.Create();
           // var image = Image.FromFile("");


        }
    }
}
