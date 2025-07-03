using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Menu.Classes;
namespace Menu.Pages
{
    /// <summary>
    /// Forest.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Forest : Page
    {
        List<Monster> ForestMonsters = new();
        List<ProgressBar> Bars = new();

        public Forest()
        {
            InitializeComponent();
            CharHp.Value = MainWindow.MyCharacter.Hp;

            ForestMonsters.Add(new Monster("썩은 나무", 100, 1));
            ForestMonsters.Add(new Monster("그냥 나무", 500, 10));
            ForestMonsters.Add(new Monster("사과 나무", 1000, 50));
            ForestMonsters.Add(new Monster("세계수", 100000, 1000));

            Bars.Add(Tree1Hp);
            Bars.Add(Tree2Hp);
            Bars.Add(Tree3Hp);
            Bars.Add(Tree4Hp);

            for (int idx = 0; idx < 4; idx++)
            {
                Bars[idx].Maximum = ForestMonsters[idx].Hp;
                Bars[idx].Value = ForestMonsters[idx].Hp;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? Btn = sender as Button; //이벤트 발생시킨 버튼 
            if (Btn == null) return;

            string? BtnTag = Btn.Tag as string;
            int idx = int.Parse(BtnTag);


            if (MainWindow.MyCharacter.Hp < MainWindow.MyCharacter.Health)
            {
                MessageBox.Show("피로도가 부족합니다.");

                //상태 원복
                Bars[idx].Visibility = Visibility.Collapsed;
                Bars[idx].Value = ForestMonsters[idx].Hp;
                return;
            }

            else
            {
                //채집시작 -> 체력바 표시
                Bars[idx].Visibility = Visibility.Visible;

                if (Bars[idx].Value > MainWindow.MyCharacter.Attack)
                {
                    Bars[idx].Value -= MainWindow.MyCharacter.Attack; //몬스터 체력 감소 
                    MainWindow.MyCharacter.Hp -= MainWindow.MyCharacter.Health; //캐릭터 체력 감소
                    CharHp.Value -= MainWindow.MyCharacter.Health;
                    return;
                }
                else
                {
                    Bars[idx].Value = 0;
                    //자원 획득

                    //
                    MessageBox.Show("나무" + ForestMonsters[idx].RewardAmout + "개 획득");
                    //원복
                    Bars[idx].Visibility = Visibility.Collapsed;
                    Bars[idx].Value = ForestMonsters[idx].Hp;


                }
            }


            MessageBox.Show("썪은 나무 채집 완료, 썪은나무조각 획득");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.SwitchPage(MainWindow.mainWindow.minepage);
        }
    }
}
