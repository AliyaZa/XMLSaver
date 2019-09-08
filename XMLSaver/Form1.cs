using System;
using System.Text;
using System.Windows.Forms;


namespace XMLSaver
{
    public partial class Form1 : Form
    {
        int i = 1;
        public Form1()
        {
            InitializeComponent();
            Logger.InitLogger();
            openFileDialog1.Filter = "Text files (*.xml)|*.xml" ;
            saveFileDialog1.Filter = "Text files (*.xml)|*.xml";
            textBox1.Multiline = true;
            textBox1.Height = 220; 
        }

        private void Save_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = openFileDialog1.FileName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    i++;
                    var WriterFile = new System.IO.StreamWriter(saveFileDialog1.FileName, false, System.Text.Encoding.GetEncoding(1251));
                    //WriterFile.Write("<File FileVersion=«{0}»>", FileVersionInfo.GetVersionInfo(saveFileDialog1.FileName).ToString() + "\r\n");
                    WriterFile.Write("<Version> Version = " + i +"</Version>" + "\r\n");
                    WriterFile.Write("<Name>" + System.IO.Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "</Name>" + "\r\n");
                    WriterFile.Write("<DateTime>" + DateTime.Now.ToShortDateString() + "</DateTime>" + "\r\n");
                    WriterFile.Write(textBox1.Text);
                    WriterFile.Write("</File>" + "\r\n");
                    Logger.Log.Info("Сохранение файла" + System.IO.Path.GetFileNameWithoutExtension(saveFileDialog1.FileName));
                    WriterFile.Close();
                    
                }
                catch (Exception err)
                { // отчет о других ошибках
                    MessageBox.Show(err.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Logger.Log.Error(err.Message);
                }
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            i = 1;
            if (openFileDialog1.FileName == String.Empty)
            {
                Logger.Log.Info("Пустая строка имени, метод закрылся");
                return;
            }

            // Чтение текстового файла
            try
            {
                var ReaderFile = new System.IO.StreamReader(openFileDialog1.FileName, Encoding.GetEncoding(1251));
                is_Correct(System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName));
                textBox1.Text = ReaderFile.ReadToEnd();
                
                ReaderFile.Close();
                Logger.Log.Info("Чтение текстового файла ");
            }
            catch (System.IO.FileNotFoundException err)
            {
                MessageBox.Show(err.Message + "\nНет такого файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Logger.Log.Error("Нет такого файла");
            }
            catch (NameIsNotCorrectException err)
            {
                MessageBox.Show(err.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Logger.Log.Error(err.Message);
            }
            catch (Exception err)
            { // отчет о других ошибках
                MessageBox.Show(err.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Logger.Log.Error(err.Message);
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void is_Correct(string fileName)
        {
            string[] words = fileName.Split(new char[] { '_' }, 3);
            bool isCorrect = true;
            if (words.Length != 3)
            {
                isCorrect = false;
                Logger.Log.Error("В файле '" + fileName+ "' названии должно быть 3 '_'");
            }
            for (int i = 0; (i < 3) && (isCorrect); i++)
            {
                for (int j = 0; j < words[i].Length && isCorrect; j++)
                {
                    if (i == 0)
                    {
                        if (words[i].Length < 100)
                        {
                            if (((words[i][j] >= 'А') && (words[i][j] <= 'Я')) || ((words[i][j] >= 'а') && (words[i][j] <= 'я')))
                                isCorrect = true;
                            else
                            {
                                isCorrect = false;
                                Logger.Log.Error("Символы в названии '" +words[i]+"' не являются буквами русского алфавита");
                            }
                        }
                        else
                        {
                            isCorrect = false;
                            Logger.Log.Error("Количество символов в названии '" + words[i] + "' больше 100");
                        }
                    }
                    if (i == 1)
                    {
                        if ((words[i].Length == 1) || (words[i].Length == 10) || ((words[i].Length >= 14) && (words[i].Length <= 20)))
                        {
                            if ((words[i][j] >= '0') && (words[i][j] <= '9'))
                                isCorrect = true;
                            else
                            {
                                isCorrect = false;
                                Logger.Log.Error("Символы в названии '" + words[i] + "' не являются цифрами");
                            }
                        }
                        else
                        {
                            isCorrect = false;
                            Logger.Log.Error("Количество символов в названии '" + words[i] + "' должно быть = либо 1, либо 10, либо между 14 и 20");
                        }
                    }
                    if (i == 2)
                    {
                        if (words[i].Length < 7)
                            isCorrect = true;
                        else
                        {
                            isCorrect = false;
                            Logger.Log.Error("Количество символов в названии '" + words[i] + "' должно быть = 7");
                        }
                    }
                }
            }

            if (!isCorrect) throw new NameIsNotCorrectException("Имя файла не соответствующее");
        }

        public class NameIsNotCorrectException : ApplicationException
        {
            public NameIsNotCorrectException() { }
            public NameIsNotCorrectException (string message): base(message) { }
            public NameIsNotCorrectException (string message, Exception inner): base(message, inner) { }
            
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
