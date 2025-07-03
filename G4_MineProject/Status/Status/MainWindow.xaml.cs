using System.IO;
using System.Net;
using System.Net.Sockets;
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
using Menu;
using Menu.Classes;
using Menu.Pages;
using Newtonsoft.Json;

namespace Status
{

    public class CServer
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        private string id;
        public CServer(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }
        public void run()
        {
            string msg;
            bool flag = true;
            byte[] buffer = new byte[1024];
            int bytesRead;
            string json;

            while (flag)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Prop received = JsonConvert.DeserializeObject<Prop>(json);
                msg = received.ToString();



                if (msg.StartsWith("/stop"))
                {
                    write(msg);//클라이언트의 서버메시지 받는 쓰레드의 종료값으로 전달
                    msg = " 님이 퇴장하셨습니다.";
                    flag = false;
                }
                MainWindow.curwindow.writetext(msg); //
                lock (MainWindow.servers)
                {
                    foreach (CServer cServer in MainWindow.servers)
                    {
                        cServer.SendProp(received);
                    }
                }

            }
            client.Close();
            lock (MainWindow.servers)
            {
                MainWindow.servers.Remove(this);
            }
        }
        public void SendProp(Prop p)
        {
            string json = JsonConvert.SerializeObject(p);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }
        public void write(string msg)
        {
            writer.WriteLine(msg);
            writer.Flush();
        }
    }

    public partial class MainWindow : Window
    {
        public static MainWindow curwindow;
        public static List<CServer> servers = new List<CServer>();
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            curwindow = this;

            Thread serverThread = new Thread(() =>
            {
                IPAddress addr = IPAddress.Any;
                TcpListener listener = new TcpListener(addr, 1234);
                listener.Start();
                MessageBox.Show("서버실행");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    CServer s = new CServer(client);
                    if (s != null)
                    {
                        MessageBox.Show("유저접속");
                        lock (servers) servers.Add(s);
                        try
                        {
                            new Thread(new ThreadStart(s.run)).Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("스레드 시작 오류: " + ex.Message);
                        }
                    }
                }
                // listener.Stop(); ← 이 부분은 필요 없거나 종료 제어를 따로 해야 함
            });
            serverThread.IsBackground = true;
            serverThread.Start(); // ⭐ UI 스레드와 분리

        }
        public void writetext(string str)
        {
            Dispatcher.Invoke(() =>
            {
                body.Text += str + "\n";
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }
    }
}