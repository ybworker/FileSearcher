using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileSearcher
{
    public class Core
    {
        public event EventHandler FinishWriterHandler;

        public event EventHandler ResultTxtNullHandler;
        public event Func<bool, bool, TxTData, Dictionary<FileResultFormEnum, List<string>>, object> A;
        public Core() { }
        public Core(EventHandler finishWriterHandler, EventHandler resultTxtNullHandler)
        {
            FinishWriterHandler += finishWriterHandler;
            ResultTxtNullHandler += resultTxtNullHandler;
        }

        public void RemoveEevent()
        {
            FinishWriterHandler -= this.FinishWriterHandler;
            ResultTxtNullHandler -= this.ResultTxtNullHandler;
            A -= this.A;
        }
        //获取路径

        //获取全部文件及文件夹名称

        //读取文件名文本

        //对比

        //输出结果
        public static List<TxTData> GetDrawingDataList(string[] userInputString)
        {
            List<TxTData> drawingTxTData = new List<TxTData>();
            for (int i = 0; i < userInputString.Length; i++)
            {
                drawingTxTData.Add(new TxTData(userInputString[i], i));
            }
            return drawingTxTData;
        }
        public static void GetFileNameAndFilePath(string path, out string[] filePathNames, out string[] fileNames)
        {
            filePathNames = Directory.GetFileSystemEntries(path);
            fileNames = new string[filePathNames.Length];
            for (int i = 0; i < filePathNames.Length; i++)
            {
                fileNames[i] = filePathNames[i].Substring(filePathNames[i].LastIndexOf("\\") + 1);
            }
        }
        /// <summary>
        /// 去除文件后缀,"."后去除（包含"."）
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string RemoveSuffix(string name)
        {
            int index = name.LastIndexOf(".");
            if (index == -1)
                return name;
            string newName = name.Substring(0, index);
            return newName;
        }
        /// <summary>
        /// 文本与文件对比
        /// </summary>
        /// <param name="drawingAndPath">单一文本与文件路径集合 字典</param>
        /// <param name="drawingNullList">不存在文件路径文本集合</param>
        /// <param name="fileNames">单独文件名数组</param>
        /// <param name="userInputList">文本集合</param>
        /// <param name="filePathNames">带有完整路径的文件名数组</param>
        public static void TXTContrastFile(Dictionary<TxTData, List<string>> drawingAndPath, List<TxTData> drawingNullList, string[] fileNames,
            List<TxTData> userInputList, string[] filePathNames)
        {
            foreach (TxTData drawingString in userInputList)
            {
                drawingAndPath.Add(drawingString, new List<string>());
                for (int i = 0; i < fileNames.Length; i++)
                {
                    if (drawingString.Name != string.Empty && fileNames[i].Replace(" ", "").IndexOf(drawingString.Name.Replace(" ", "")) >= 0)
                    {
                        drawingAndPath[drawingString].Add(fileNames[i] + ":" + filePathNames[i]);
                    }
                }
                if (drawingAndPath[drawingString].Count <= 0)
                {
                    drawingNullList.Add(drawingString);
                }
            }
        }
        /// <summary>
        /// 文件与文本对比
        /// </summary>
        /// <param name="drawingAndPath">单一文本与文件路径集合 字典</param>
        /// <param name="drawingNullList">不存在文件路径文本集合</param>
        /// <param name="fileNames">单独文件名数组</param>
        /// <param name="userInputList">文本集合</param>
        /// <param name="filePathNames">带有完整路径的文件名数组</param>
        public static void FileContrastTXT(Dictionary<TxTData, List<string>> pathAndDrawing, List<TxTData> drawingNullList, string[] fileNames,
            List<TxTData> userInputList, string[] filePathNames)
        {
            for (int i = 0; i < fileNames.Length; i++)
            {
                var temp = new TxTData(fileNames[i], i);
                pathAndDrawing.Add(temp, new List<string>());
                foreach (TxTData drawingString in userInputList)
                {
                    if (drawingString.Name != string.Empty && fileNames[i].Replace(" ", "").IndexOf(drawingString.Name.Replace(" ", "")) >= 0)
                    {
                        pathAndDrawing[temp].Add(drawingString + ":" + filePathNames[i]);
                    }
                }
                if (pathAndDrawing[temp].Count <= 0)
                {
                    drawingNullList.Add(temp);
                }
            }
        }
        /// <summary>
        /// 输出结果为文本
        /// </summary>
        /// <param name="drawingAndPath">单一文本与文件路径集合</param>
        /// <param name="drawingNullList">不存在文件路径文本集合</param>
        public void ResultToTXT(Dictionary<TxTData, List<string>> drawingAndPath, List<TxTData> drawingNullList, ResultFormEnum resultFormEnum)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ((resultFormEnum == ResultFormEnum.TXTVSFILE) ? "\\文本为主对比文件.txt" : "\\文件为主对比文本.txt");
            File.Create(path).Close();
            if (!File.Exists(path))
            {
                this.ResultTxtNullHandler?.Invoke(this, new EventArgs());
                return;
            }
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(((resultFormEnum == ResultFormEnum.TXTVSFILE) ? "无文件的文本名：" : "无文本的文件名："));
            foreach (TxTData drawingNull in drawingNullList)
            {
                streamWriter.WriteLine(drawingNull.Name);
            }
            streamWriter.WriteLine(((resultFormEnum == ResultFormEnum.TXTVSFILE) ? "无文件的文本数量：" : "无文本的文件数量：") + drawingNullList.Count);
            streamWriter.WriteLine();
            streamWriter.WriteLine();
            streamWriter.WriteLine("-----------------------------------------------------------------------------");
            streamWriter.WriteLine("---------------------------------明细----------------------------------------");
            streamWriter.WriteLine("-----------------------------------------------------------------------------");
            streamWriter.WriteLine();
            foreach (TxTData key in drawingAndPath.Keys)
            {
                streamWriter.WriteLine(key.Name);
                foreach (string item in drawingAndPath[key])
                {
                    streamWriter.WriteLine(item);
                }
                streamWriter.WriteLine(key.Name + ((resultFormEnum == ResultFormEnum.TXTVSFILE) ? "相同文件数量：" : "相同文本数量：") + drawingAndPath[key].Count);
                streamWriter.WriteLine();
                streamWriter.WriteLine();
            }
            streamWriter.Close();
            Thread.Sleep(1);
            try
            {
                streamWriter.WriteLine();
                streamWriter.Close();
            }
            catch (Exception)
            {
                this.FinishWriterHandler?.Invoke(this, new EventArgs());
            }
        }




        /// <summary>
        /// 复制功能
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pdfFiles"></param>
        /// <param name="dwgFiles"></param>
        public static void GetAllFileDWGandPDF(string path, out FileInfo[] pdfFiles, out FileInfo[] dwgFiles)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            pdfFiles = directoryInfo.GetFiles("*pdf", SearchOption.AllDirectories);
            dwgFiles = directoryInfo.GetFiles("*dwg", SearchOption.AllDirectories);
        }
        public Dictionary<string, List<FileInfo>> FindFirstPDFandDWGFile(List<TxTData> fileName, FileInfo[] pdfFiles, FileInfo[] dwgFiles, out Dictionary<FileResultFormEnum, List<string>> nullFileName)
        {
            Dictionary<string, List<FileInfo>> result = new Dictionary<string, List<FileInfo>>();
            nullFileName = new Dictionary<FileResultFormEnum, List<string>>();
            nullFileName.Add(FileResultFormEnum.没有PDF, new List<string>());
            nullFileName.Add(FileResultFormEnum.没有DWG, new List<string>());
            nullFileName.Add(FileResultFormEnum.都没有, new List<string>());
            nullFileName.Add(FileResultFormEnum.都有, new List<string>());
            result.Add("pdf", new List<FileInfo>(fileName.Count));
            result.Add("dwg", new List<FileInfo>(fileName.Count));
            FileInfo[] temp = pdfFiles.Concat(dwgFiles).ToArray();
            for (int i = 0; i < fileName.Count; i++)
            {
                bool isPDFFile = false;
                bool isdwgFile = false;
                for (int j = 0; j < temp.Length; j++)
                {
                    if ((!isPDFFile) && j < pdfFiles.Length && temp[j].Name.Replace(" ", "").IndexOf(fileName[i].Name.Replace(" ", "")) >= 0)
                    {
                        result["pdf"].Add(temp[j]);
                        isPDFFile = true;
                    }
                    if ((!isdwgFile) && temp.Length - 1 - j - pdfFiles.Length >= 0 && temp[temp.Length - 1 - j].Name.Replace(" ", "").IndexOf(fileName[i].Name.Replace(" ", "")) >= 0)
                    {
                        result["dwg"].Add(temp[temp.Length - 1 - j]);
                        isdwgFile = true;
                    }
                    if (isdwgFile && isPDFFile)
                    {
                        continue;
                    }

                }
                this.A?.Invoke(isPDFFile, isdwgFile, fileName[i], nullFileName);
            }
            return result;
        }
 
    }
    public class FileData : TxTData
    {
        public FileData(string name, int index) : base(name, index)
        {
        }
    }
    public class TxTData
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public TxTData(string name, int index)
        {
            Name = name;
            Index = index;
        }
    }
    public enum FileResultFormEnum
    {
        没有PDF,
        没有DWG,
        都没有,
        都有
    }
    public enum ResultFormEnum
    {
        TXTVSFILE,
        FILEVSTXT
    }

}
