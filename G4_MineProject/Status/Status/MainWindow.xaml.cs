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
    public class RowData
    {
        public Prop P1 { get; set; } = null;
        public Prop P2 { get; set; } = null;
        public Prop P3 { get; set; } = null;
        public Prop P4 { get; set; } = null;
        public Prop P5 { get; set; } = null;
        public Prop P6 { get; set; } = null;
        public Prop P7 { get; set; } = null;
        public Prop P8 { get; set; } = null;
        public Prop P9 { get; set; } = null;
        public Prop P10 { get; set; } = null;
    }
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

        //MJ
        private List<Prop> Inventory = new List<Prop>();
        private int? sourceRow = null;
        private int? sourceCol = null;
        //~Mj
        public MainWindow()
        {
            InitializeComponent();
            inventory.SelectionUnit = DataGridSelectionUnit.Cell;
            inventory.CurrentCellChanged += inventory_CurrentCellChanged;
            // 열 정의
            for (int i = 0; i < 10; i++)
            {
                inventory.Columns.Add(new DataGridTextColumn
                {
                    Binding = new Binding($"P{i + 1}.Name"), // Product 객체의 Name 속성만 표시
                    Width = 50
                });
            }
            inventory.RowHeight = 60;

            var rowDataList = new List<RowData>();
            for (int i = 0; i < 3; i++)
            {
                rowDataList.Add(new RowData());
            }
            inventory.ItemsSource = rowDataList;
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
                //body.Text += str + "\n";
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        //MJ
        private void AddProp(Prop newProp)
        {
            Inventory.Add(newProp);
            int index = Inventory.Count - 1;
            int row = index / 10;
            int col = index % 10;

            RowData rowData = ((List<RowData>)inventory.ItemsSource)[row];
            typeof(RowData).GetProperty($"P{col + 1}")?.SetValue(rowData, newProp);
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (Inventory.Count >= 30)
            {
                MessageBox.Show("더 이상 추가할 수 없습니다.");
                return;
            }
            string name;
            int price;
            int amount;
            int force;
            int damage;
            int health;

            string imgsource = "a";

            if (forcev.Text == "" && damagev.Text == "" && healthv.Text == "")
            {
                name = namev.Text;
                price = Int32.Parse(pricev.Text);
                amount = Int32.Parse(amountv.Text);

                Resource newProp = new Resource(name, price, amount, imgsource);
                AddProp(newProp);

            }
            else if (healthv.Text == "")
            {
                name = namev.Text;
                price = Int32.Parse(pricev.Text);
                amount = Int32.Parse(amountv.Text);
                force = Int32.Parse(forcev.Text);
                damage = Int32.Parse(damagev.Text);

                Equipment newProp = new Equipment(name, price, amount, imgsource, force, damage);
                AddProp(newProp);
            }
            else if (forcev.Text == "" && damagev.Text == "")
            {
                name = namev.Text;
                price = Int32.Parse(pricev.Text);
                amount = Int32.Parse(amountv.Text);
                health = Int32.Parse(healthv.Text);

                Food newProp = new Food(name, price, amount, imgsource,health);
                AddProp(newProp);
            }

            // 30개 칸 중 몇 번째 칸인지 계산

            inventory.Items.Refresh();

            namev.Text = "";
            pricev.Text = "";
            amountv.Text = "";
            forcev.Text = "";
            damagev.Text = "";
            healthv.Text = "";
        }

        private void inventory_CurrentCellChanged(object sender, EventArgs e)
        {
            var dg = (DataGrid)sender;
            if (dg.CurrentCell == null || dg.CurrentCell.Item == null) return;
            if (dg.CurrentCell.Column == null) return;

            int currentCol = dg.CurrentCell.Column.DisplayIndex;
            int currentRow = dg.Items.IndexOf(dg.CurrentCell.Item);

            var rowList = (List<RowData>)inventory.ItemsSource;
            RowData currentRowData = rowList[currentRow];
            Prop currentProp = (Prop)typeof(RowData).GetProperty($"P{currentCol + 1}")?.GetValue(currentRowData);

            // 이전 셀과 현재 셀이 다르면 이동/교환 시도
            if (sourceRow != null && sourceCol != null && (sourceRow != currentRow || sourceCol != currentCol))
            {
                RowData sourceRowData = rowList[sourceRow.Value];
                Prop sourceProp = (Prop)typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.GetValue(sourceRowData);

                if (sourceProp != null)
                {
                    if (currentProp == null)
                    {
                        // 빈 셀: 이동
                        typeof(RowData).GetProperty($"P{currentCol + 1}")?.SetValue(currentRowData, sourceProp);
                        typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.SetValue(sourceRowData, null);
                    }
                    else
                    {
                        // 데이터가 있는 셀: 교환
                        typeof(RowData).GetProperty($"P{currentCol + 1}")?.SetValue(currentRowData, sourceProp);
                        typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.SetValue(sourceRowData, currentProp);
                    }

                    // DataGrid 새로고침
                    inventory.Items.Refresh();
                    Namev.Text = "";
                    Pricev.Text = "";
                    Amountv.Text = "";
                    Forcev.Text = "";
                    Damagev.Text = "";
                    Healthv.Text = "";
                    sourceRow = null;
                    sourceCol = null;
                    return;
                }
            }

            // 현재 클릭한 셀의 Product 표시
            if (currentProp != null)
            {
                if (currentProp is Resource)
                {
                    Namev.Text = currentProp.Name;
                    Pricev.Text = currentProp.Price.ToString();
                    Amountv.Text = currentProp.Amount.ToString();
                }
                else if (currentProp is Equipment)
                {
                    Equipment cP = (Equipment)currentProp;
                    Namev.Text = cP.Name;
                    Pricev.Text = cP.Price.ToString();
                    Amountv.Text = cP.Amount.ToString();
                    Forcev.Text = cP.Force.ToString();
                    Damagev.Text = cP.Damage.ToString();
                }
                else if (currentProp is Food)
                {
                    Food cP = (Food)currentProp;
                    Namev.Text = cP.Name;
                    Pricev.Text = cP.Price.ToString();
                    Amountv.Text = cP.Amount.ToString();
                    Healthv.Text = cP.Healnum.ToString();
                }
            }
            else
            {
                Namev.Text = "";
                Pricev.Text = "";
                Amountv.Text = "";
                Forcev.Text = "";
                Damagev.Text = "";
                Healthv.Text = "";
            }

            // 새 기준 셀 설정
            sourceRow = currentRow;
            sourceCol = currentCol;
        }
        //~Mj
    }
}