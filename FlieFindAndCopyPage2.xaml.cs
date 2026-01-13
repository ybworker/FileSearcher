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

        private void GetFileInfo()
        {
             
            userInputList = GetTxTDataList();
            Core.GetAllFileDWGandPDF(TextBox_FilePath.Text, out pdffiles, out dwgfiles);
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
        public bool GetFilePath(string path, string prefixName)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show(prefixName + "路径有误，请重新输入");
                return false;
            }
            return true;
        }
        private List<TxTData> GetTxTDataList()
        {
            return Core.GetDrawingDataList(TextBox_txtData.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries));
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
            streamWriter.WriteLine(fileName.Name+"                   " + (isdwgFile  ? "有" : "没有")+ "                   " + (isPDFFile ? "有" : "没有"));
            return null;
        }

        private void TextBox_CopyPath_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        public void CopyFileTo(string path, Dictionary<string, List<FileInfo>> copyFiles, bool isOverwrite)
        {
            foreach (var item in copyFiles.Keys)
            {
                foreach (var file in copyFiles[item])
                {
                    if (!File.Exists(file.FullName))
                        continue;
                    if (isOverwrite)
                    {
                        file.CopyTo(path + "\\" + file.Name,isOverwrite);
                    }
                    else
                    {
                        if (!File.Exists(path + "\\" + file.Name))
                        {
                            file.CopyTo(path + "\\" + file.Name, isOverwrite);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            MessageBox.Show("复制完成");
        }
        private void Button_CopyFile_Click(object sender, RoutedEventArgs e)
        {
            if (!(GetFilePath(TextBox_FilePath.Text, "文件") && GetFilePath(TextBox_CopyPath.Text, "复制")))
                return;
            GetFileInfo();
            MessageBoxResult copyResult = MessageBox.Show("是否复制到此路径？", "复制确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (copyResult == MessageBoxResult.Yes)
            {
                string temp = TextBox_CopyPath.Text;
                streamWriter = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\文件情况.txt");
                streamWriter.WriteLine("名称-------------------.dwg-------------------.pdf");
                Dictionary<FileResultFormEnum, List<string>> nullFileNames;
                var c = core.FindFirstPDFandDWGFile(userInputList, pdffiles, dwgfiles, out nullFileNames);
                streamWriter.Close();
                CopyFileTo(temp, c, (bool)CheckBox_Overwrite.IsChecked);
            }
            else
            {
                return;
            }
        }
    }
}
