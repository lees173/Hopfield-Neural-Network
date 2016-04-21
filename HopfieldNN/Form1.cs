using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;


/*
 * Training Neural Network(NN) using Train image 
 * Get Input data from http://yann.lecun.com/exdb/mnist/
 * MNIST handwritten digits
 * Match Training image to training label
 * 
 * Select Test image and Test label  to test NN.
 * 
 * */






namespace HopfieldNN
{
   
  
    public partial class Form1 : Form
    {
        int[,] wMatrix = new int[28*28,28*28];
        string path = Application.StartupPath + @"\..\train-images\";
        string imageFilePath = "";
        string  testImgFile = "";
        string testlabelPath = "";
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "test";
        

        }




        Dictionary<string, string> answer = new Dictionary<string, string>();
        private void trainButton_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "train file";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";
            

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                imageFilePath = openFileDlg.FileName;

            }
            else
            {
                imageFilePath = path+"train-images.idx3-ubyte";
            }


      //      string labelFilePath = path + "train-labels.idx1-ubyte";     

            FileStream fs = new FileStream(@imageFilePath, FileMode.OpenOrCreate);
     //       FileStream fs1 = new FileStream(@labelFilePath, FileMode.OpenOrCreate);

            BinaryReader r = new BinaryReader(fs);
     //       BinaryReader r1 = new BinaryReader(fs1);


            for (Int32 i = 0; i < 4; i++)
            {

                BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
            }

         

            int numImage = 60000;
            if(textBox2.Text != null)
            {
                numImage = Int32.Parse(textBox2.Text);
            }

            for (int numberofimages = 0; numberofimages < numImage; numberofimages++)
            {
                byte[] data = new byte[28 * 28];

                for (int i = 0; i < 28 * 28; i++)
                {
                   
                    data[i] = r.ReadByte();//array[0];        
                }

                Bitmap bitmap = new Bitmap(28, 28);
                int[] tempData = new int[28 * 28];


                for (int row = 0; row < 28; row++)
                {
                    for (int col = 0; col < 28; col++)
                    {
                        bitmap.SetPixel(col, row, Color.FromArgb(255 - data[row * 28 + col], 255 - data[row * 28 + col], 255 - data[row * 28 + col]));

                        if (data[row * 28 + col] == 0)
                        {
                            tempData[row * 28 + col] = -1;
                        }
                        else
                        {
                            tempData[row * 28 + col] = 1;
                        }

                    }
                }

                
                for (int row = 0; row < 28*28; row++)
                {
                    for (int col = 0; col < 28 * 28; col++)
                    {
                        wMatrix[row, col] = wMatrix[row, col] + tempData[row] * tempData[col];
                        if (row == col)
                        {
                            wMatrix[row, col] = 0; 
                        }
                    }
                    
                    
                }

                textBox3.Text = "Making Matrix: "+(numberofimages+1).ToString() +" / " + numImage.ToString() ;
                textBox3.Refresh();
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
                           
            }
            
            r.Close();
            fs.Close();


        }
        private void button5_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Title = "open saveFileDlg";
            //openFileDlg.DefaultExt = "*.*";
            saveFileDlg.Filter = " All Files (*.*) | *.*";
            string saveFile = "";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                saveFile = saveFileDlg.FileName;

            }
            else
            {
                saveFile = path+"saveTrainedNN.txt";
            }


            FileStream fs = File.OpenWrite(saveFile);
            BinaryWriter writer = new BinaryWriter(fs);

            writer.Write(answer.Count);
            // Write pairs.
            foreach (var pair in answer)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.ToString());
            }


            writer.Close();
            fs.Close();

            textBox3.Text = "save Mapping label";
            textBox3.Refresh();

        }
        private void button6_Click(object sender, EventArgs e)
        {


            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "open file";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";
            string openFilePath = "";

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                openFilePath = openFileDlg.FileName;

            }
            else
            {
                openFilePath = path + "1";
            }



            FileStream fs = File.OpenRead(openFilePath);
            BinaryReader reader = new BinaryReader(fs);

            int count = reader.ReadInt32();
            // Read in all pairs.
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                if (!answer.ContainsKey(key))
                {

                    answer[key] = value;
                }

            }
            

            textBox3.Text = "Mapping Load";
            textBox3.Refresh();
            reader.Close();
            fs.Close();
     

        }

       
        private void button3_Click(object sender, EventArgs e)
        {


         //   string imageFilePath = path + "train-images.idx3-ubyte";

            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "label file";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";

            string labelFilePath = "";
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                labelFilePath = openFileDlg.FileName;

            }
            else
            {
                 labelFilePath = path + "train-labels.idx1-ubyte";
            }



            FileStream fs = new FileStream(@imageFilePath, FileMode.OpenOrCreate);
            FileStream fs1 = new FileStream(@labelFilePath, FileMode.OpenOrCreate);

            BinaryReader r = new BinaryReader(fs);
            BinaryReader r1 = new BinaryReader(fs1);

            for (Int32 i = 0; i < 4; i++)
            {

                BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
            }

            for (Int32 i = 0; i < 2; i++)
            {

                BitConverter.ToInt32(r1.ReadBytes(4).Reverse().ToArray(), 0);
            }
            int numImage = 60000;
            if (textBox2.Text != null)
            {
                numImage = Int32.Parse(textBox2.Text);
            }
            for (int numberofimages = 0; numberofimages < numImage; numberofimages++)
            {
                byte[] data = new byte[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {
                    data[i] = r.ReadByte();
                }

                Bitmap bitmap = new Bitmap(28, 28);
                int[] tempData = new int[28 * 28];

                for (int row = 0; row < 28; row++)
                {
                    for (int col = 0; col < 28; col++)
                    {
                        bitmap.SetPixel(col, row, Color.FromArgb(255 - data[row * 28 + col], 255 - data[row * 28 + col], 255 - data[row * 28 + col]));

                        if (data[row * 28 + col] == 0)
                        {
                            tempData[row * 28 + col] = -1;
                        }
                        else
                        {
                            tempData[row * 28 + col] = 1;
                        }

                    }
                }


                int[] output = new int[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {
                    //   output[i] = tempData[i];
                    int sum = tempData[i];
                    for (int j = 0; j < 28 * 28; j++)
                    {
                        sum = sum + tempData[j] * wMatrix[j, i];   // j row i col
                    }

                    output[i] = sum;

                    if (output[i] > 0)
                    {
                        output[i] = 1;
                    }
                    else if (output[i] < 0)
                    {
                        output[i] = -1;
                    }
                    else if (output[i] == 0)
                    {
                        output[i] = tempData[i];
                    }

                }


                int imageLabel = Convert.ToInt32(r1.ReadByte());
                if (!answer.ContainsKey(string.Join("", output)))
                {
                    answer.Add(string.Join("", output), imageLabel.ToString());
                }
                textBox1.Text = imageLabel.ToString();
                textBox1.Refresh();


                textBox3.Text = "Mapping Label: " + (numberofimages + 1).ToString() + " / " + numImage.ToString();
                textBox3.Refresh();
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
            }
            r.Close();
            fs.Close();

            r1.Close();
            fs1.Close();
          

        }



        

        private void testButton_Click(object sender, EventArgs e)
        {
            
            
            int checkImage = Convert.ToInt32(textBox5.Text);
            OpenFileDialog openFileDlg = new OpenFileDialog();
            
            openFileDlg.Title = "test label file";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";
            string labelFilePath = "";

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                testlabelPath = openFileDlg.FileName;

            }
            else
            {
                testlabelPath = "t10k-labels.idx1-ubyte";
            }

           // string openFilePath = path + "t10k-images.idx3-ubyte";

           

            FileStream fs = new FileStream(testImgFile, FileMode.OpenOrCreate);

            BinaryReader r = new BinaryReader(fs);


            FileStream fs1 = new FileStream(testlabelPath, FileMode.Open);
            BinaryReader r1 = new BinaryReader(fs1);


            //long length = r1.BaseStream.Length;
            for (Int32 i = 0; i < 2; i++)
            {

                BitConverter.ToInt32(r1.ReadBytes(4).Reverse().ToArray(), 0);
            }


            for (Int32 i = 0; i < 4; i++)
            {

                BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
            }

            Bitmap bitmap = new Bitmap(28, 28);
            byte[] data = new byte[28 * 28];
            int error = 0;
            for (int numberofimages = 0; numberofimages < checkImage; numberofimages++)
            {
                

                int[] tempData = new int[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {

                    data[i] = r.ReadByte();//array[0];

                }


                for (int col = 0; col < 28; col++)
                {

                    for (int row = 0; row < 28; row++)
                    {
                        bitmap.SetPixel(col, row, Color.FromArgb(255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col]));


                        if (data[(row) * 28 + col] == 0)
                        {
                            tempData[(row) * 28 + col] = -1;
                        }
                        else
                        {
                            tempData[(row) * 28 + col] = 1;
                        }
                    }


                }

                pictureBox3.Image = bitmap;

                pictureBox3.Refresh();

                int[] output = new int[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {
                        //   output[i] = tempData[i];
                        int sum = tempData[i];
                        for (int j = 0; j < 28 * 28; j++)
                        {
                            sum = sum + tempData[j] * wMatrix[j, i];
                        }

                        output[i] = sum;

                        if (output[i] > 0)
                        {
                            output[i] = 1;
                        }
                        else if (output[i] < 0)
                        {
                            output[i] = -1;
                        }
                        else if (output[i] == 0)
                        {
                            output[i] = tempData[i];
                        }

                }
                


                int answerNumber = -1;

                if (answer.ContainsKey(string.Join("", output)))
                {

                    textBoxAnswer.Text = answer[string.Join("", output)];
                    textBoxAnswer.Refresh();
                    answerNumber = Convert.ToInt32(answer[string.Join("", output)]);
     
                }
                else
                {
                    textBoxAnswer.Text = "?";
                    textBoxAnswer.Refresh();

                }

               int testNumber= Convert.ToInt32(r1.ReadByte());
              
                if (!testNumber.Equals(answerNumber))
                {
                   error++;
                }


                float errorRate = (float) error / (float)(numberofimages + 1);

               

                textBox4.Text = "wrong Rate: " + (errorRate*100).ToString("F2") + "%";

                textBox1.Text = "Number of wrong:" + error.ToString() + " Number of Image: " +(numberofimages+1).ToString();
                textBox1.Refresh();

                textBox4.Refresh();
                textBoxAnswer.Refresh();



                int timer = 0;

                if (!textBox6.Text.Equals(""))
                {
                    timer = Convert.ToInt32(textBox6.Text);
                }
                System.Threading.Thread.Sleep(timer);


            }
            r.Close();
            r1.Close();
            fs1.Close();
            fs.Close();


        }

       

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "Select Matrix";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";
            string openFilePath = "";

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                openFilePath = openFileDlg.FileName;

            }

            TextReader rw = new StreamReader(openFilePath);


            for (int row = 0; row < 28 * 28; row++)
            {
                String line = rw.ReadLine();
                int[] getInt = line.Split(',').Select(Int32.Parse).ToArray();

                for (int col = 0; col < 28 * 28; col++)
                {
                    wMatrix[row, col] = getInt[col];
                }
            }
            textBox3.Text = "Matrix Load";
            textBox3.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Title = "open saveFileDlg";
            //openFileDlg.DefaultExt = "*.*";
            saveFileDlg.Filter = " All Files (*.*) | *.*";
            string saveFile = "";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                saveFile = saveFileDlg.FileName;

            }
            else
            {
                saveFile = path+ "saveMatrix.txt";
            }

            TextWriter tw = new StreamWriter(saveFile, false);
            for (int row = 0; row < 28 * 28; row++)
            {
                for (int col = 0; col < 28 * 28; col++)
                {

                    tw.Write(wMatrix[row, col]);
                    if (col < 28 * 28 - 1)
                    {
                        tw.Write(",");
                    }

                }
                tw.WriteLine("");
            }
            tw.Close();

            textBox3.Text = "save Matrix";
            textBox3.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "test image file";
            openFileDlg.DefaultExt = "*.*";
            openFileDlg.Filter = " All Files (*.*) | *.*";
             testImgFile = "";

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                testImgFile = openFileDlg.FileName;

            }
            else
            {
                testImgFile = path + "t10k-images.idx3-ubyte";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // string openFilePath = path + "t10k-images.idx3-ubyte";


            int checkImage = Convert.ToInt32(textBox5.Text);

            if (testImgFile.Equals("") || testlabelPath.Equals(""))
            {

            }
            else
            {



                FileStream fs = new FileStream(testImgFile, FileMode.OpenOrCreate);

                BinaryReader r = new BinaryReader(fs);


                FileStream fs1 = new FileStream(testlabelPath, FileMode.Open);
                BinaryReader r1 = new BinaryReader(fs1);


                //long length = r1.BaseStream.Length;
                for (Int32 i = 0; i < 2; i++)
                {

                    BitConverter.ToInt32(r1.ReadBytes(4).Reverse().ToArray(), 0);
                }


                for (Int32 i = 0; i < 4; i++)
                {

                    BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
                }

                Bitmap bitmap = new Bitmap(28, 28);
                byte[] data = new byte[28 * 28];
                int error = 0;
                for (int numberofimages = 0; numberofimages < checkImage; numberofimages++)
                {


                    int[] tempData = new int[28 * 28];
                    for (int i = 0; i < 28 * 28; i++)
                    {

                        data[i] = r.ReadByte();//array[0];

                    }


                    for (int col = 0; col < 28; col++)
                    {

                        for (int row = 0; row < 28; row++)
                        {
                            bitmap.SetPixel(col, row, Color.FromArgb(255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col]));


                            if (data[(row) * 28 + col] == 0)
                            {
                                tempData[(row) * 28 + col] = -1;
                            }
                            else
                            {
                                tempData[(row) * 28 + col] = 1;
                            }
                        }


                    }

                    pictureBox3.Image = bitmap;

                    pictureBox3.Refresh();
  

                    int[] output = new int[28 * 28];
                    for (int i = 0; i < 28 * 28; i++)
                    {
                        int sum = tempData[i];
                        for (int j = 0; j < 28 * 28; j++)
                        {
                            sum = sum + tempData[j] * wMatrix[j, i];
                        }

                        output[i] = sum;

                        if (output[i] > 0)
                        {
                            output[i] = 1;
                        }
                        else if (output[i] < 0)
                        {
                            output[i] = -1;
                        }
                        else if (output[i] == 0)
                        {
                            output[i] = tempData[i];
                        }

                    }



                    int answerNumber = -1;

                    if (answer.ContainsKey(string.Join("", output)))
                    {

                        textBoxAnswer.Text = answer[string.Join("", output)];
                        textBoxAnswer.Refresh();
                        answerNumber = Convert.ToInt32(answer[string.Join("", output)]);
                    
                    }
                    else
                    {
                        textBoxAnswer.Text = "?";
                        textBoxAnswer.Refresh();

                    }

                    int testNumber = Convert.ToInt32(r1.ReadByte());

                    if (!testNumber.Equals(answerNumber))
                    {
                        error++;
                    }


                    float errorRate = (float)error / (float)(numberofimages + 1);



                    textBox4.Text = "wrong Rate: " + (errorRate * 100).ToString("F2") + "%";

                    textBox1.Text = "Number of wrong:" + error.ToString() + " Number of Image: " + (numberofimages + 1).ToString();
                    textBox1.Refresh();

                    textBox4.Refresh();
                    textBoxAnswer.Refresh();



                    int timer = 0;

                    if (!textBox6.Text.Equals(""))
                    {
                        timer = Convert.ToInt32(textBox6.Text);
                    }
                    System.Threading.Thread.Sleep(timer);


                }
                r.Close();
                r1.Close();
                fs1.Close();
                fs.Close();
            }


        }

       


    }
}
