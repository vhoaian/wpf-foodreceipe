using FoodRecipe.DAO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FoodRecipe.Screens
{
    /// <summary>
    /// Interaction logic for RecipeDetail.xaml
    /// </summary>
    public partial class RecipeDetail : Window
    {
        private DTO.Food food = null;
        private int currentStepIndex = -1;
        public void LoadStepList()
        {
            var steps = food.Steps;
            stepStack.Children.Clear();
            
            for (int i = 0; i < steps.Count; i++)
            {
                Button button = new Button() {
                    BorderThickness = (currentStepIndex == i) ? new Thickness(1) : new Thickness(0),
                    Padding = new Thickness(10),
                    FontWeight = FontWeights.Bold,
                    Foreground = (currentStepIndex == i) ? Brushes.Red : Brushes.White,
                    Background = (currentStepIndex == i) ? Brushes.LightGray : Brushes.Gray,
                    Content = steps[i].StepName,
                    Tag = i
                };
                button.Click += stepButton_Click;
                stepStack.Children.Add(button);
                if (currentStepIndex == i)
                {
                    descriptionText.Text = steps[i].DescriptionStep;
                    imageStack.Children.Clear();
                    if (steps[i].ImageStepPath.Count == 0)
                    {
                        imagesBox.Visibility = Visibility.Collapsed;
                        detailBox.SetValue(Grid.RowProperty, 1);
                    } else
                    {
                        imagesBox.Visibility = Visibility.Visible;
                        detailBox.SetValue(Grid.RowProperty, 2);
                        if (steps[i].ImageStepPath.Count == 1)
                            arrowLeft.Visibility = arrowRight.Visibility = rectLeft.Visibility = rectRight.Visibility = Visibility.Hidden;
                        else
                            arrowLeft.Visibility = arrowRight.Visibility = rectLeft.Visibility = rectRight.Visibility = Visibility.Visible;

                    }
                    for (int j = 0; j < steps[i].ImageStepPath.Count; j++)
                    {
                        Image image = new Image() {
                            Margin = new Thickness(20, 5, 20, 5)
                        };
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + steps[i].ImageStepPath[j], UriKind.Absolute);
                        bi.EndInit();
                        image.Source = bi;
                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                        imageStack.Children.Add(image);
                    }
                }
            }
        }

        private void stepButton_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            if (index != currentStepIndex)
            {
                currentStepIndex = index;
                LoadStepList();
            }
        }

        public RecipeDetail(int id)
        {
            InitializeComponent();
            food = FoodDAO.getById(id.ToString());
            this.DataContext = food;
            LoadStepList();
            if (food.VideoLink != null && food.VideoLink.Length > 0)
            {
                videoLink.Visibility = Visibility.Visible;
            } else
            {
                videoLink.Visibility = Visibility.Collapsed;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
            };
            foreach (var element in imageStack.Children)
                ((Image)element).BeginAnimation(OpacityProperty, da);
            var action = ((Image)sender).Tag.ToString();
            if (action.Equals("next"))
            {
                var el = (Image)imageStack.Children[0];
                imageStack.Children.RemoveAt(0);
                imageStack.Children.Add(el);
            }
            else
            {
                var el = (Image)imageStack.Children[imageStack.Children.Count - 1];
                imageStack.Children.RemoveAt(imageStack.Children.Count - 1);
                imageStack.Children.Insert(0, el);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {

        }

        private void videoLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((this.DataContext as DTO.Food).VideoLink);
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
