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
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class FileAndTextCheckPage1 : Page
    {
        public bool IsPathTrue { get; private set; }
        private string filePath;
        private string[] filePathNames;
        private string[] fileNames;
        private List<TxTData> userInputList;
        private StreamWriter streamWriter;
        public Core core;

        public FileAndTextCheckPage1()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Button_TXTVsFile.IsEnabled = false;
            Button_FileVsTXT.IsEnabled = false;
            core = new Core(FinishWriterHandler, ResultTxtNullHandler);
        }

       

        private void ResultTxtNullHandler(object? sender, EventArgs e)
        {
            MessageBox.Show("创建结果文件失败。");
        }
        private void FinishWriterHandler(object? sender, EventArgs e)
        {
            MessageBox.Show("完成");
        }
        private void TextBox_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_TXTVsFile.IsEnabled = false;
            Button_FileVsTXT.IsEnabled = false;
        }
        private void Button_TXTVsFile_Click(object sender, RoutedEventArgs e)
        {
            userInputList = GetTxTDataList();
            List<TxTData> drawingNullList = new List<TxTData>();
            Dictionary<TxTData, List<string>> drawingAndPath = new Dictionary<TxTData, List<string>>();
            Core.TXTContrastFile(drawingAndPath, drawingNullList, fileNames, userInputList, filePathNames);
            core.ResultToTXT(drawingAndPath, drawingNullList, ResultFormEnum.TXTVSFILE);
        }
        private void Button_FileVsTXT_Click(object sender, RoutedEventArgs e)
        {
            userInputList = GetTxTDataList();
            List<TxTData> drawingNullList = new List<TxTData>();
            Dictionary<TxTData, List<string>> drawingAndPath = new Dictionary<TxTData, List<string>>();
            Core.FileContrastTXT(drawingAndPath, drawingNullList, fileNames, userInputList, filePathNames);
            core.ResultToTXT(drawingAndPath, drawingNullList, ResultFormEnum.FILEVSTXT);
        }
        private void Button_IsPath_Click(object sender, RoutedEventArgs e)
        {
            filePath = TextBox_Path.Text;
            filePath = GetFilePath();
            if (IsPathTrue)
            {
                Core.GetFileNameAndFilePath(filePath, out filePathNames, out fileNames);
                Button_TXTVsFile.IsEnabled = true;
                Button_FileVsTXT.IsEnabled = true;
            }
        }
        public string GetFilePath()
        {
            IsPathTrue = Directory.Exists(filePath);
            if (!IsPathTrue)
            {
                MessageBox.Show("路径有误，请重新输入");
                return string.Empty;
            }
            return filePath;
        }
        public string[] GetUserInputStrings()
        {
            return TextBox_txtData.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }
        private List<TxTData> GetTxTDataList()
        {
            return Core.GetDrawingDataList(GetUserInputStrings());
        }

    }
}
