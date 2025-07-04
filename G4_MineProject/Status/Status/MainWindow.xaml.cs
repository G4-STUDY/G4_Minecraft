using Menu;
using Menu.Classes;
using Menu.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

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

        public void Charactor_refresh(Character c)
        {
            MainWindow.character.Hp = c.Hp;
            MainWindow.character.Health = c.Health;
            MainWindow.character.Attack = c.Attack;
            MainWindow.character.Money = c.Money;
            MainWindow.curwindow.Dispatcher.Invoke(() =>
            {
                MainWindow.curwindow.money.Text = c.Money.ToString();
            });
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
                JObject root = JObject.Parse(json);
                string type = root["Type"]?.ToString();
                if (type == "Prop")
                {
                    Prop received = root["Data"].ToObject<Prop>();
                    string act = root["Act"]?.ToString();
                    switch (act)
                    {
                        case "1":
                            MainWindow.curwindow.AddProp_Inventory(received);
                            break;
                        case "2":

                            MainWindow.curwindow.AddProp_Inventory(received);
                            // Prop 획득 처리 로직

                            break;

                    }
                }
                else if (type == "charactor")
                {
                    string act = root["Act"]?.ToString();
                    if (act == "1")
                    {   //get메서드
                        SendCharacter(MainWindow.character, 2);
                    }
                    else
                    {
                        Character chac = root["Data"].ToObject<Character>();
                        Charactor_refresh(chac);
                    }
                    //캐릭터 갱신 메소드
                }


                ////MainWindow.curwindow.writetext(msg); //
                //lock (MainWindow.servers) 
                //{  
                //    foreach (CServer cServer in MainWindow.servers)
                //    {   //클라이언트에 보낼작업하기
                //        //보낼게있나..?
                //        cServer.SendProp(received);
                //    }
                //}

            }
            client.Close();
            lock (MainWindow.servers)
            {
                MainWindow.servers.Remove(this);
            }
        }
        public void SendProp(Prop p, int act)//act 1 구매해서 보내는거 2. 자원캐서 보내는거
        {
            var wrapper = new
            {
                Type = "Prop",
                Act = "1",//1이면 buy 2면 획득 바꿔서 사용
                Data = p
            };
            string json = JsonConvert.SerializeObject(wrapper);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

        public void SendCharacter(Character c, int act) //act 1 구매해서 보내는거 2. 자원캐서 보내는거
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

        public void write(string msg)
        {
            writer.WriteLine(msg);
            writer.Flush();
        }
    }

    public partial class MainWindow : Window
    {
        public static int curidx;
        public static Character character;
        public static MainWindow curwindow;
        public static List<CServer> servers = new List<CServer>();
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        public static CServer s;
        //MJ
        private List<Prop> Inventory = new List<Prop>();
        private List<Prop> Equipped = new List<Prop>();
        private int? sourceRow = null;
        private int? sourceCol = null;
        //~Mj
        public MainWindow()
        {
            InitializeComponent();
            character = new Character();
            curwindow = this;
            inventory.SelectionUnit = DataGridSelectionUnit.Cell;
            inventory.CurrentCellChanged += inventory_CurrentCellChanged;
            for (int i = 0; i < 10; i++)
            {
                inventory.Columns.Add(new DataGridTextColumn
                {
                    Binding = new Binding($"P{i + 1}.Name"), // Product 객체의 Name 속성만 표시
                    Width = 50
                });
            }
            inventory.RowHeight = 50;

            var rowDataList = new List<RowData>();
            for (int i = 0; i < 3; i++)
            {
                rowDataList.Add(new RowData());
            }
            inventory.ItemsSource = rowDataList;


            Equipped_Grid.SelectionUnit = DataGridSelectionUnit.Cell;
            Equipped_Grid.CurrentCellChanged += Equipped_Grid_CurrentCellChanged;


            Equipped_Grid.Columns.Add(new DataGridTextColumn
            {
                Binding = new Binding($"P{1}.Name"), // Product 객체의 Name 속성만 표시
                Width = 40
            });

            Equipped_Grid.RowHeight = 40;

            var rowDataList1 = new List<RowData>();
            for (int i = 0; i < 4; i++)
            {
                rowDataList1.Add(new RowData());
            }
            Equipped_Grid.ItemsSource = rowDataList1;

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
                    s = new CServer(client);
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

        //MJ
        public void AddProp_Inventory(Prop newProp)
        {
            Dispatcher.Invoke(() =>
            {
                Inventory.Add(newProp);
                int index = Inventory.Count - 1; 
                int row = index / 10;
                int col = index % 10;

                RowData rowData = ((List<RowData>)inventory.ItemsSource)[row];
                typeof(RowData).GetProperty($"P{col + 1}")?.SetValue(rowData, newProp);

                inventory.Items.Refresh();
            });

        }

        public void Remove_Inventory(int idx)
        {

            int row = idx / 10;
            int col = idx % 10;
            RowData rowData = ((List<RowData>)inventory.ItemsSource)[row];
            typeof(RowData).GetProperty($"P{col + 1}")?.SetValue(rowData, null);
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
                AddProp_Inventory(newProp);

            }
            else if (healthv.Text == "")
            {
                name = namev.Text;
                price = Int32.Parse(pricev.Text);
                amount = Int32.Parse(amountv.Text);
                force = Int32.Parse(forcev.Text);
                damage = Int32.Parse(damagev.Text);

                Equipment newProp = new Equipment(name, price, amount, imgsource, force, damage);
                AddProp_Inventory(newProp);
            }
            else if (forcev.Text == "" && damagev.Text == "")
            {
                name = namev.Text;
                price = Int32.Parse(pricev.Text);
                amount = Int32.Parse(amountv.Text);
                health = Int32.Parse(healthv.Text);

                Food newProp = new Food(name, price, amount, imgsource, health);
                AddProp_Inventory(newProp);
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
        public void swap(Prop x, Prop y)
        {
            Prop tmp = x;
            x = y;
            y = tmp;
            return;
        }
        private void inventory_CurrentCellChanged(object sender, EventArgs e)
        {
            var dg = (DataGrid)sender;
            if (dg.CurrentCell == null || dg.CurrentCell.Item == null) return;
            if (dg.CurrentCell.Column == null) return;

            int currentCol = dg.CurrentCell.Column.DisplayIndex;
            int currentRow = dg.Items.IndexOf(dg.CurrentCell.Item);
            curidx = (currentRow) * 10 + currentCol;
            var rowList = (List<RowData>)inventory.ItemsSource;
            RowData currentRowData = rowList[currentRow];
            Prop currentProp = (Prop)typeof(RowData).GetProperty($"P{currentCol + 1}")?.GetValue(currentRowData);

            //// 이전 셀과 현재 셀이 다르면 이동/교환 시도
            //if (sourceRow != null && sourceCol != null && (sourceRow != currentRow || sourceCol != currentCol))
            //{
            //    RowData sourceRowData = rowList[sourceRow.Value];
            //    Prop sourceProp = (Prop)typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.GetValue(sourceRowData);

            //    if (sourceProp != null)
            //    {
            //        if (currentProp == null)
            //        {
            //            //// 빈 셀: 이동
            //            //typeof(RowData).GetProperty($"P{currentCol + 1}")?.SetValue(currentRowData, sourceProp);
            //            //typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.SetValue(sourceRowData, null);
            //        }
            //        else
            //        {
            //            // 데이터가 있는 셀: 교환
            //            typeof(RowData).GetProperty($"P{currentCol + 1}")?.SetValue(currentRowData, sourceProp);
            //            typeof(RowData).GetProperty($"P{sourceCol.Value + 1}")?.SetValue(sourceRowData, currentProp);
            //            swap(Inventory[currentRow], Inventory[currentRow]);
            //        }

            //        // DataGrid 새로고침
            //        inventory.Items.Refresh();
            //        Namev.Text = "";
            //        Pricev.Text = "";
            //        Amountv.Text = "";
            //        Forcev.Text = "";
            //        Damagev.Text = "";
            //        Healthv.Text = "";
            //        sourceRow = null;
            //        sourceCol = null;
            //        return;
            //    }
            //}

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
        private void Equipped_Grid_CurrentCellChanged(object sender, EventArgs e)
        {

        }
        private void AddProp_Equipped(Prop newProp)
        {
            Equipped.Add(newProp);

            var rowList = (List<RowData>)Equipped_Grid.ItemsSource;
            int testidx = 0;
            foreach (var row in rowList)
            {
                var existing = (Prop)typeof(RowData).GetProperty("P1")?.GetValue(row);
                if (existing == null)
                {
                    typeof(RowData).GetProperty("P1")?.SetValue(row, newProp);
                    break;
                }
            }

            Equipped_Grid.Items.Refresh();
        }
        private void Equip_Button_Click(object sender, RoutedEventArgs e)
        {
            //Prop newProp = (Prop)inventory.SelectedItem;
            //MessageBox.Show("확인용12222" + newProp.Name);
            string name = Namev.Text;
            int price = Int32.Parse(Pricev.Text);
            int amount = Int32.Parse(Amountv.Text);
            string imgsource = "a";
            if (Forcev.Text != "") { //장착:장비
                int force = Int32.Parse(Forcev.Text);
                int damage = Int32.Parse(Damagev.Text);
                Equipment newProp = new Equipment(name, price, amount, imgsource, force, damage);
                UseProp(newProp, curidx);
            }
            Namev.Text = "";
            Pricev.Text = "";
            Amountv.Text = "";
            Forcev.Text = "";
            Damagev.Text = "";

        }
        //~Mj


        public void UseProp(Prop prop, int idx)
        {
            if (prop is Equipment)
            {
                Equipment equipment = (Equipment)prop;
                AddProp_Equipped(equipment);
                character.Attack += equipment.Force;
                character.Health += equipment.Damage;
            }
            else if (prop is Food)
            {
                Food food = (Food)prop;
                character.Hp += food.Healnum;
                Inventory.Remove(food);
            }
            Inventory.RemoveAt(idx);
            Remove_Inventory(idx);
            inventory.Items.Refresh();
            MainWindow.s.SendCharacter(character, 2);
            //반대쪽에 알려줘라

        }

        private void Sell_Button_Click(object sender, RoutedEventArgs e)
        {
            int price = Int32.Parse(Pricev.Text);
            character.Money += price;
            Inventory.RemoveAt(curidx);
            Remove_Inventory(curidx);
            inventory.Items.Refresh();
            s.SendCharacter(character, 2);
        }
    }
}