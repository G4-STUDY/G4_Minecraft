using Menu.Classes;
using Menu.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        public Pages.Forest forest;
        public Pages.Mine mine;
        public Frame MainFrameAccessor => MainFrame;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            MyCharacter = new Character();
            MessageBox.Show(MyCharacter.Money.ToString()+"dd");
            mainpage =new Pages.Mainpage();
            minepage = new Pages.Minepage();
            shoppage = new Pages.Shoppage();
            forest = new Pages.Forest();
            mine = new Pages.Mine();
            SwitchPage(mainpage);
            curpage = mainpage;
        }
        public static Character MyCharacter;
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
        public void Charactor_refresh(Character c)
        {
            MyCharacter.Hp = c.Hp;
            MyCharacter.Health = c.Health;
            MyCharacter.Attack = c.Attack;
            MyCharacter.Money = c.Money;
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
            string msg;
            bool flag = true;
            byte[] buffer = new byte[1024];
            int bytesRead;
            string json;
            while (flag)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                JObject root = JObject.Parse(json);
                string type = root["Type"]?.ToString();
                MessageBox.Show("갱신완료?????");
                if (type == "Prop")
                {
                    Prop received = root["Data"].ToObject<Prop>();
                    string act = root["Act"]?.ToString();

                    switch (act)
                    {
                        case "1":
                            // 상점에다 자원파는경우
                            MyCharacter.Money += received.Price;
                            SendCharacter(MyCharacter, 2);//act 2는 갱신요청
                            break;
                        case "2":
                            // 
                            break;

                    }
                }
                else if (type == "charactor")
                {
                    MessageBox.Show("갱신완료?????2323");
                    string act = root["Act"]?.ToString();
                    if (act == "1")
                    {   
                        //??
                    }
                    else //내캐릭터 갱신해줘
                    {
                        Character chac = root["Data"].ToObject<Character>();
                        Charactor_refresh(chac);
                        MessageBox.Show("갱신완료"+MyCharacter.Money);
                        //캐릭터 갱신 메소드
                    }
                }
            }
                client.Close();
        }

        public void SendProp(Prop p, int act)//act 1 구매해서 보내는거 2. 자원캐서 보내는거
        {
            var wrapper = new
            {
                Type = "Prop",
                Act = act.ToString(),//1이면 buy 2면 획득 바꿔서 사용
                Data = p
            };
            string json = JsonConvert.SerializeObject(wrapper);
            byte[] data = Encoding.UTF8.GetBytes(json);
            if (act == 1)
            {
                SendCharacter(MyCharacter, 2);//상점에서 구매했으면 돈깎고 캐릭터갱신해라
            }
            stream.Write(data, 0, data.Length);
        }


        public void SendCharacter(Character c, int act) //act 1.캐릭터정보 보내주세요 2. 캐릭터 갱신요청
        {
            var wrapper = new
            {
                Type = "charactor",
                Act = act,
                Data = c  // 기존 객체 p 포함
            };
            string json = JsonConvert.SerializeObject(wrapper);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

        public void SwitchPage(Page page)
        {
            curpage = page;
            MainFrameAccessor.Navigate(page);
            if (page is Forest)
            {
                Forest fore = (Forest)page;
                fore.CharHp.Value=MyCharacter.Hp;
            }
            else if(page is Mine)
            {
                Mine mine = (Mine)page;
                mine.CharHp.Value = MyCharacter.Hp;
            }
            SendCharacter(MyCharacter, 2);
        }

        //다른 페이지에서 쓸 prop전달 메소드 
        //private void Button_sendProp(object sender, RoutedEventArgs e)
        //{
        //    //Prop p = new Prop();
        //    //_mainWindow.SendProp(p); //_mainWindow = Page생성자에 this로 전달
        //}

    }
}