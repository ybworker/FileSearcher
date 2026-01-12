using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace FileSearcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Page[] pages;
        public MainWindow()
        {
            InitializeComponent();
            Init();
           
        }
        private void Init() 
        {
            pages=new Page[] {
                new FileAndTextCheckPage1(),
               new FileFindAndCopy_Page2()
            };
            MainFrame.Navigate(pages[0]);
            FileAndTextCheck.IsEnabled = false;
            FileFindAndCopy.IsEnabled = true;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("V2.2 ;\nv2.1更新：输出文件描述修正。\nv2.2更新：增加文件.pdf与.dwg文件复制功能");
        }
        private void MenuItem_Click3(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/ybworker") { UseShellExecute=true});
        }
        private void FileAndTextCheck_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(pages[0]);
            FileAndTextCheck.IsEnabled = false;
            FileFindAndCopy.IsEnabled = true;
        }
        private void FileFindAndCopy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(pages[1]);
            FileAndTextCheck.IsEnabled = true;
            FileFindAndCopy.IsEnabled = false;
        }
    }
}