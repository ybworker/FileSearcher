using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSearcher
{
    /// <summary>
    /// FileFindAndCopy.xaml 的交互逻辑
    /// </summary>
    public partial class FileFindAndCopy_Page2 : Page
    {

        public bool IsPathTrue { get; private set; }
        private string filePath;
        private List<TxTData> userInputList;
        private StreamWriter streamWriter;
        private FileInfo[] pdffiles, dwgfiles;
        public Core core;
        public FileFindAndCopy_Page2()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            CheckBox_Overwrite.IsChecked = false;
            Button_CopyFile.IsEnabled = false;
            core = new Core(FinishWriterHandler, ResultTxtNullHandler);
            core.A += Core_A;
        }



        private void ResultTxtNullHandler(object? sender, EventArgs e)
        {
            MessageBox.Show("创建结果文件失败。");
        }
        private void FinishWriterHandler(object? sender, EventArgs e)
        {
            MessageBox.Show("完成");
        }

        private void TextBox_FilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_CopyFile.IsEnabled = false;
        }

        private void Button_IsPath_Click(object sender, RoutedEventArgs e)
        {
            filePath = TextBox_FilePath.Text;
            IsPathTrue = GetFilePath(filePath);
            if (IsPathTrue)
            {
                userInputList = GetTxTDataList();
                Core.GetAllFileDWGandPDF(filePath, out pdffiles, out dwgfiles);
                Button_CopyFile.IsEnabled = true;
            }
        }
        public bool GetFilePath(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show("路径有误，请重新输入");
                return false;
            }
            return true;
        }
        public string[] GetUserInputStrings()
        {
            return TextBox_txtData.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }
        private List<TxTData> GetTxTDataList()
        {
            return Core.GetDrawingDataList(GetUserInputStrings());
        }

        private object Core_A(bool isPDFFile, bool isdwgFile, TxTData fileName, Dictionary<FileResultFormEnum, List<string>> nullFileName)
        {
            switch (isdwgFile && isPDFFile)
            {
                case true:
                    nullFileName[FileResultFormEnum.都有].Add(fileName.Name);
                    break;
                case false:
                    if (!isPDFFile && !isdwgFile)
                    {
                        nullFileName[FileResultFormEnum.都没有].Add(fileName.Name);
                    }
                    else if (!isdwgFile)
                    {
                        nullFileName[FileResultFormEnum.没有DWG].Add(fileName.Name);
                    }
                    else if (!isPDFFile)
                    {
                        nullFileName[FileResultFormEnum.没有PDF].Add(fileName.Name);
                    }
                    break;
            }
            streamWriter.WriteLine((isdwgFile || isPDFFile ? "有" : "没有"));
            return null;
        }

        private void TextBox_CopyPath_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        public  void CopyFileTo(object objl)
        {
            TYEU obj = objl as TYEU;
            
            int i = 0;
            TextBlock_FindPlan.Text =i.ToString()+"/"+obj.copyFiles.Count.ToString();
            foreach (var item in obj.copyFiles.Keys)
            {
                foreach (var file in obj.copyFiles[item])
                {
                    if (obj.isOverwrite)
                    {
                        file.CopyTo(obj.path + "\\" + file.Name, obj.isOverwrite);
                    }
                    else
                    {
                        if (!File.Exists(obj.path + "\\" + file.Name))
                        {
                            file.CopyTo(obj.path + "\\" + file.Name, obj.isOverwrite);
                        }
                    }
                    i++;
                    TextBlock_FindPlan.Text = i.ToString() + "/" + obj.copyFiles.Count.ToString();
                    Thread.Sleep(100);
                }
            }
        }
        private void Button_CopyFile_Click(object sender, RoutedEventArgs e)
        {
            if (!GetFilePath(TextBox_CopyPath.Text))
                return;
            MessageBoxResult copyResult = MessageBox.Show("是否复制到此路径？", "复制确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (copyResult == MessageBoxResult.Yes)
            {
                string temp = TextBox_CopyPath.Text;
                streamWriter = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\文件情况.txt");
                Dictionary<FileResultFormEnum, List<string>> nullFileNames;
                var c = core.FindFirstPDFandDWGFile(userInputList, pdffiles, dwgfiles, out nullFileNames);
                streamWriter.Close();
                TYEU tYEU = new TYEU();
                tYEU.path = temp;
                tYEU.copyFiles = c;
                tYEU.isOverwrite = (bool)CheckBox_Overwrite.IsChecked;
                Thread th = new Thread(CopyFileTo);
                th.Start(tYEU);
               // CopyFileTo(temp, c, (bool)CheckBox_Overwrite.IsChecked);
            }
            else
            {
                return;
            }
        }

    }
    public class TYEU 
    {
       public string path;
       public Dictionary<string, List<FileInfo>> copyFiles;
       public bool isOverwrite;
    }
}
