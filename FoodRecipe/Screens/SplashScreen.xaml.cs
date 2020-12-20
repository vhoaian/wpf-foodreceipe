using FoodRecipe.DAO;
using FoodRecipe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Shapes;

namespace FoodRecipe.Screens
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private System.Timers.Timer timer;
        private int count = 0;
        private int target = 5;
        private BindingList<FoodPreview> _list = new BindingList<FoodPreview>();
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _list = FoodPreviewDAO.GetAll();
            Random _rng = new Random();
            int indexRandom = _rng.Next(_list.Count);
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var imagePath = $"{folder}Images\\{_list[indexRandom].ImageNameFile}";
            previewImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            IntroAccessText.Text = _list[indexRandom].Intro;

            var isShowSplash = bool.Parse(ConfigurationManager.AppSettings["ShowSplashScreen"]);

            if (isShowSplash == false)
            {
                var listFoodScreen = new ListFood();
                listFoodScreen.Show();

                this.Close();
            }
            else
            {
                timer = new System.Timers.Timer();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 1000;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            count++;
            if (count == target)
            {
                timer.Stop();
                Dispatcher.Invoke(() =>
                {
                    var listFoodScreen = new ListFood();
                    listFoodScreen.Show();

                    this.Close();
                });
            }

            Dispatcher.Invoke(() =>
            {
                splashProgress.Value = count;
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["ShowSplashScreen"].Value = "false";
            config.Save(ConfigurationSaveMode.Minimal);
        }

        private void saveShowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["ShowSplashScreen"].Value = "true";
            config.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
