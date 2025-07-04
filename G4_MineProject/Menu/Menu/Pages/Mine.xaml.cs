using Menu.Classes;
using System;
using System.Collections.Generic;
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

namespace Menu.Pages
{
    /// <summary>
    /// Mine.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Mine : Page
    {
        List<Monster> ForestMonsters = new();
        List<ProgressBar> Bars = new();

        public Mine()
        {
            InitializeComponent();
            CharHp.Value = MainWindow.MyCharacter.Hp;

            ForestMonsters.Add(new Monster("구리 광산", 100, 1));
            ForestMonsters.Add(new Monster("은 광산", 500, 10));
            ForestMonsters.Add(new Monster("금 광산", 1000, 50));
            ForestMonsters.Add(new Monster("다이아몬드 광산", 100000, 1000));

            Bars.Add(Mineral1Hp);
            Bars.Add(Mineral2Hp);
            Bars.Add(Mineral3Hp);
            Bars.Add(Mineral4Hp);

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
                MessageBox.Show("피로도가 부족합니다. 음식을 먹고 오세요");

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
                    //
                    Resource resource = new Resource("돌", 10, ForestMonsters[idx].RewardAmout, "");
                    MainWindow.mainWindow.SendProp(resource, 2);
                    MessageBox.Show("광물" + ForestMonsters[idx].RewardAmout + "개 획득");
                    //원복
                    Bars[idx].Visibility = Visibility.Collapsed;
                    Bars[idx].Value = ForestMonsters[idx].Hp;


                }
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.SwitchPage(MainWindow.mainWindow.minepage);
        }
    }
}