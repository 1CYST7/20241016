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
using Microsoft.Win32;
using System.IO;

namespace _20241016
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<string, int> drinks = new Dictionary<string, int>(); // 定義飲料名稱與價格的字典
        Dictionary<string, int> orders = new Dictionary<string, int>(); // 定義訂購飲料名稱與數量的字典
        string takeout = ""; // 定義是否外帶的字串變數
        public MainWindow()
        {
            InitializeComponent(); // 初始化視窗元件
            AddNewDrink(drinks); // 呼叫方法以添加飲料項目到 drinks 字典
            DisplayDrinkMenu(drinks); // 呼叫方法以顯示飲料選單
        }
        private void AddNewDrink(Dictionary<string, int> drinks)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // 建立開啟檔案對話框
            openFileDialog.Title = "選擇飲料品項檔案"; // 設定對話框標題
            openFileDialog.Filter = "CSV文件|*.csv|文字檔案|*.txt|所有文件|*.*"; // 設定檔案過濾條件

            if (openFileDialog.ShowDialog() == true) // 檢查是否成功選擇檔案
            {
                string fileName = openFileDialog.FileName; // 獲取選擇的檔案路徑
                string[] lines = File.ReadAllLines(fileName); // 讀取檔案中的每一行文字

                foreach (var line in lines) 
                {
                    string[] tokens = line.Split(','); // 以逗號分割每行文字
                    string drinkName = tokens[0]; // 取得飲料名稱
                    int price = Convert.ToInt32(tokens[1]); // 轉換飲料價格為整數
                    drinks.Add(drinkName, price); // 將飲料名稱與價格添加到 drinks 字典
                }
            }
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
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var drinkName = cb.Content.ToString();
                var sl = sp.Children[2] as Slider;
                var amount = (int)sl.Value;

                if (cb.IsChecked == true && amount > 0) orders.Add(drinkName, amount);
            }
            // 顯示訂購內容，初始化
            string msg = "";
            string discount_msg = "";
            int total = 0;

            DateTime dateTime = DateTime.Now;
            msg += $"訂購時間：{dateTime.ToString("yyyy/MM/dd HH:mm:ss")}，此次訂購為{takeout}，訂購內容如下：\n";

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

            // 將訂單訊息顯示在介面上
            ResultTextBlock.Text = msg; 
            // 建立儲存檔案對話框
            SaveFileDialog saveFileDialog = new SaveFileDialog(); 

            saveFileDialog.Title = "儲存訂購內容"; // 設定標題
            // 設定檔案過濾條件
            saveFileDialog.Filter = "文字檔案|*.txt|所有文件|*.*"; 
            // 檢查是否成功選擇存檔路徑
            if (saveFileDialog.ShowDialog() == true) 
            {
                // 取得選擇的檔案路徑
                string fileName = saveFileDialog.FileName; 
                try
                {
                    // 建立 StreamWriter 物件寫入檔案
                    using (StreamWriter sw = new StreamWriter(fileName)) 
                    {
                        sw.Write(msg); // 寫入訂單訊息
                    }
                    MessageBox.Show("訂單已成功儲存。"); // 顯示成功訊息
                }
                catch (IOException ex)//判斷處理輸入/輸出操作中發生的錯誤
                {
                    MessageBox.Show($"儲存檔案時發生錯誤: {ex.Message}"); // 若發生錯誤，顯示錯誤訊息
                }
            }
        }
    }
}