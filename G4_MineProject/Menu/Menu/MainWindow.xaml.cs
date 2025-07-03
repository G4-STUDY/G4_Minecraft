using Menu.Classes;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menu
{
    public partial class MainWindow : Window
    {

        static public Page curpage;
        static public MainWindow mainWindow;
        public Pages.Mainpage mainpage;
        public Pages.Minepage minepage;
        public Pages.Shoppage shoppage;
        public Frame MainFrameAccessor => MainFrame;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            mainpage=new Pages.Mainpage();
            minepage = new Pages.Minepage();
            shoppage = new Pages.Shoppage();
            SwitchPage(mainpage);
            curpage = mainpage;
        }

        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        private TcpClient client;
        public string GetLocalIP()
        {
            string myIP = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    myIP = ip.ToString();
                }
            }
            return myIP;
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            client = new TcpClient();
            string serverIP = GetLocalIP();
            client.Connect(serverIP, 1234);
            if (client != null)
            {
                MessageBox.Show("서버접속완료");
            }
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            Thread th = new Thread(new ThreadStart(readMsg));
            th.Start();

        }

        public void readMsg()
        {
            string json, str;
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Prop received = JsonConvert.DeserializeObject<Prop>(json);
                str = received.ToString();
                if (str.StartsWith("/stop"))
                {
                    break;
                }
                //백그라운드 작업에서 ui에 직접 접근 불가
                //Current.Dispatcher 객체를 통해 ui에 접근
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //if (curpage is MainPage)
                    //    page1.body.Text += str + "\n";
                    //if (curpage is Page1)
                    //    page1.body.Text += str + "\n";
                    //if (curpage is Page2)
                    //    page1.body.Text += str + "\n";
                });

            }
            client.Close();
        }

        public void SendProp(Prop p)
        {
            string json = JsonConvert.SerializeObject(p);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

        public void SwitchPage(Page page)
        {
            curpage = page;
            MainFrameAccessor.Navigate(page);
        }

        //다른 페이지에서 쓸 prop전달 메소드 
        //private void Button_sendProp(object sender, RoutedEventArgs e)
        //{
        //    //Prop p = new Prop();
        //    //_mainWindow.SendProp(p); //_mainWindow = Page생성자에 this로 전달
        //}

    }
}