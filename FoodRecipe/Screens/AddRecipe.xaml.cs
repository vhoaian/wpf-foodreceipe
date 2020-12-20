using FoodRecipe.Db;
using FoodRecipe.DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace FoodRecipe.Screens
{
    /// <summary>
    /// Interaction logic for AddRecipe.xaml
    /// </summary>
    public partial class AddRecipe : Window
    {
        public AddRecipe()
        {
            InitializeComponent();
        }

        private Food myFood = new Food();

        private ObservableCollection<string> stepImages = new ObservableCollection<string>();
        private ObservableCollection<FoodStep> steps = new ObservableCollection<FoodStep>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.stepImagesList.ItemsSource = stepImages;
            this.stepsList.ItemsSource = steps;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myFood.Name = txtName.Text;
            myFood.VideoLink = txtVideoLink.Text;
            myFood.Description = txtDescription.Text;

            if (myFood.Name.Equals("") || myFood.Description.Equals("") || myFood.ThumbnailPath.Equals("") || steps.Count == 0)
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường bắt buộc");
                return;
            }

            myFood.steps = steps.ToList();

            string imageRoot = "Images\\";
            string newThumbPath = imageRoot + Guid.NewGuid().ToString() + Path.GetExtension(myFood.ThumbnailPath);
            // Copy the food's thumb
            File.Copy(myFood.ThumbnailPath, newThumbPath);
            myFood.ThumbnailPath = newThumbPath;

            foreach (var step in myFood.steps)
            {
                for (int i = 0; i < step.ImageStepPath.Count; i++) {
                    var path = step.ImageStepPath[i];
                    var newImagePath = imageRoot + Guid.NewGuid().ToString() + Path.GetExtension(path);
                    File.Copy(path, newImagePath);
                    step.ImageStepPath[i] = newImagePath;
                }
            }
            DBUtils.write(myFood);
            MessageBox.Show("Lưu thành công!");
        }

        private void Button_Add_Step(object sender, RoutedEventArgs e)
        {
            // Check
            if (StepName.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập Tên bước", "Thiếu dữ liệu cần thiết");
                return;
            }
            if (DescriptionStep.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập Cách thực hiện", "Thiếu dữ liệu cần thiết");
                return;
            }

            if (currentSelectedStep != -1)
            {
                steps.Insert(currentSelectedStep, new FoodStep
                {
                    DescriptionStep = DescriptionStep.Text,
                    StepName = StepName.Text,
                    ImageStepPath = stepImages.ToList()
                });
                steps.RemoveAt(currentSelectedStep + 1);
                currentSelectedStep = -1;
                addStepButton.Content = "Thêm";
            } else
            {
                steps.Add(new FoodStep
                {
                    DescriptionStep = DescriptionStep.Text,
                    StepName = StepName.Text,
                    ImageStepPath = stepImages.ToList()
                });
            }
            // Reset the form
            StepName.Text = "";
            DescriptionStep.Text = "";
            stepImages.Clear();
        }

        private void Upload_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh đại diện cho món ăn";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            var o = op.ShowDialog();
            if (o == true)
            {
                Image.Source = new BitmapImage(new Uri(op.FileName));
                Image.Opacity = 1;
                Image.MaxWidth = 200;
            }

            myFood.ThumbnailPath = op.FileName;
        }

        private void StepImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn các ảnh minh hoạ cho bước";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            op.Multiselect = true;
            var o = op.ShowDialog();
            if (o == true)
            {
                foreach (var name in op.FileNames)
                stepImages.Add(name);
            }
        }

        private void Button_Click_Fav(object sender, RoutedEventArgs e)
        {
            var favScreen = new FavoriteFood();
            favScreen.Show();
            this.Close();
        }

        private void Button_Click_List(object sender, RoutedEventArgs e)
        {
            var listScreen = new ListFood();
            listScreen.Show();
            this.Close();
        }

        private void Button_Reset_Step(object sender, RoutedEventArgs e)
        {
            currentSelectedStep = -1;
            addStepButton.Content = "Thêm";
            // Reset the form
            StepName.Text = "";
            DescriptionStep.Text = "";
            stepImages.Clear();
        }

        private int currentSelectedStep = -1;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                addStepButton.Content = "Lưu";
                var step = (((TextBlock)sender).Tag as FoodStep);
                currentSelectedStep = steps.IndexOf(step);

                StepName.Text = step.StepName;
                DescriptionStep.Text = step.DescriptionStep;
                stepImages.Clear();
                foreach (var stepImg in step.ImageStepPath)
                {
                    stepImages.Add(stepImg);
                }
            }
        }
    }
}

