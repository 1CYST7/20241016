using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
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

namespace _20241016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 定義飲料及其價格的字典
        Dictionary<string, int> drinks = new Dictionary<string, int>
        {
            { "紅茶大杯", 60 },
            { "紅茶小杯", 40 },
            { "綠茶大杯", 50 },
            { "綠茶小杯", 30 },
            { "可樂大杯", 50 },
            { "可樂小杯", 30 },
        };
        // 儲存訂單的字典
        Dictionary<string, int> orders = new Dictionary<string, int>();
        // 外帶或內用選擇
        string takeout = "";
        public MainWindow()
        {
            InitializeComponent();
            // 顯示飲料菜單
            DisplayDrinkMenu(drinks);
        }
        // 顯示飲料菜單的方法
        private void DisplayDrinkMenu(Dictionary<string, int> drinks)
        {
            // 根據飲料項目調整菜單高度
            stackpanel_DrinkMenu.Height = 42 * drinks.Count;
            foreach (var drink in drinks)
            {
                // 建立 StackPanel 作為每種飲料的顯示範圍
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(3),
                    Background = Brushes.LightBlue,
                    Height = 35,
                };
                // 為飲料建立CheckBox勾選框
                var cb = new CheckBox
                {
                    Content = drink.Key,
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Blue,
                    Width = 150,
                    Margin = new Thickness(5),
                    VerticalContentAlignment = VerticalAlignment.Center,
                };
                // 建立顯示飲料價格的Label標籤
                var lb_price = new Label
                {
                    Content = $"{drink.Value}元",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Green,
                    Width = 60,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };
                // 建立數量Slider滑桿
                var sl = new Slider
                {
                    Width = 150,
                    Minimum = 0,
                    Maximum = 10,
                    Value = 0,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true,
                };
                // 顯示數量的Label標籤
                var lb_amount = new Label
                {
                    Content = "0",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 50,
                };
                // 建立綁定物件
                Binding myBinding = new Binding("Value");
                // 綁定來源設為滑桿
                myBinding.Source = sl;
                // 綁定滑桿值到顯示標籤
                lb_amount.SetBinding(ContentProperty, myBinding);
                // 將勾選框加入 StackPanel
                sp.Children.Add(cb);
                // 將價格標籤加入 StackPanel
                sp.Children.Add(lb_price);
                // 將滑桿加入 StackPanel
                sp.Children.Add(sl);
                // 將數量標籤加入 StackPanel
                sp.Children.Add(lb_amount);
                // 將 StackPanel 加入菜單面板
                stackpanel_DrinkMenu.Children.Add(sp);

            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                //MessageBox.Show(rb.Content.ToString());
                takeout = rb.Content.ToString();
            }
        }
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // 確認訂購內容
            orders.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                // 取得每個飲料的 StackPanel
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                // 取得勾選框、飲料名稱、數量滑桿，再將數量滑桿轉成int型態
                var cb = sp.Children[0] as CheckBox;
                var drinkName = cb.Content.ToString();
                var sl = sp.Children[2] as Slider;
                var amount = (int)sl.Value;

                // 若勾選框被選中且數量大於 0
                if (cb.IsChecked == true && amount > 0)
                {
                    // 將飲料名稱及數量加入訂單
                    orders.Add(drinkName, amount);
                }
            }

            // 顯示訂購內容，初始化
            string msg = "";
            string discount_msg = "";
            int total = 0;
            msg += $"此次訂購為{takeout}，訂購內容如下：\n";
            // 訂單項目序號
            int num = 1;
            foreach (var order in orders)
            {
                // 計算每項小計，顯示每項訂購內容，並加總
                int subtotal = drinks[order.Key] * order.Value;
                msg += $"{num}. {order.Key} x {order.Value}杯，小計{subtotal}元\n";
                total += subtotal;
                num++;
            }
            msg += $"總金額為{total}元";

            int sellPrice = total;
            if (total >= 500)
            {
                sellPrice = (int)(total * 0.8);
                discount_msg = $"恭喜您獲得滿500元打8折優惠";
            }
            else if (total >= 300)
            {
                sellPrice = (int)(total * 0.9);
                discount_msg = $"恭喜您獲得滿300元打9折優惠";
            }
            else
            {
                discount_msg = $"未達到任何折扣條件";
            }
            msg += $"\n{discount_msg}，原價為{total}元，售價為 {sellPrice}元。";
            // 將最終訂單訊息顯示在界面上
            ResultTextBlock.Text = msg;
        }
    }
}