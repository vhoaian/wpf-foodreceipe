using FoodRecipe.DAO;
using FoodRecipe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for FavoriteFood.xaml
    /// </summary>
    public partial class FavoriteFood : Window
    {
        private BindingList<Food> _list = new BindingList<Food>();
        private int perPage = 6;
        private int page = 1;
        private int totalPage = 1;
        public FavoriteFood()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int totalFood = FoodDAO.GetLengthFavs();
            totalPage = (totalFood / perPage) + 1;

            _list = FoodDAO.GetFavorites(perPage, page);
            dataListView.ItemsSource = _list;

            var firstCreatedButton = createPagingButton(true, "1");
            pagingStackPanel.Children.Add(firstCreatedButton);

            for (int i = 2; i <= totalPage; i++)
            {
                var createdButton = createPagingButton(false, i.ToString());

                pagingStackPanel.Children.Add(createdButton);
            }
        }

        private void pagingButton_Click(object sender, RoutedEventArgs e)
        {
            resetActivePagingButton();
            ((Button)sender).Background = Brushes.LightBlue;

            var pageSelected = ((Button)sender).Content;

            _list = FoodDAO.GetFavorites(perPage, int.Parse(pageSelected.ToString()));
            dataListView.ItemsSource = _list;
        }

        private Button createPagingButton(bool isFirst, string content)
        {
            var result = new Button();

            result.Content = content;
            Thickness margin = result.Margin;
            margin.Left = 10;
            margin.Right = 10;
            result.Margin = margin;
            Thickness padding = result.Padding;
            padding.Left = 5;
            padding.Right = 5;
            padding.Top = 5;
            padding.Bottom = 5;
            result.Padding = padding;
            result.Cursor = Cursors.Hand;
            result.Background = isFirst ? Brushes.LightBlue : Brushes.Transparent;
            result.Click += pagingButton_Click;

            return result;
        }
        private void resetActivePagingButton()
        {
            var pagingButtons = pagingStackPanel.Children;

            foreach (var el in pagingButtons)
            {
                ((Button)el).Background = Brushes.Transparent;
            }

        }

        private void Button_Click_Detail(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(((Button)sender).Tag.ToString());
            var detailScreen = new RecipeDetail(id);
            detailScreen.Show();
        }

        private void Button_Click_RemoveFav(object sender, RoutedEventArgs e)
        {
            var id = ((Button)sender).Tag;
            bool result = FoodDAO.updateIsFavorite(id.ToString(), false);

            if (!result)
            {
                MessageBox.Show("Lỗi id không đúng", "Thông báo");
            }

            MessageBox.Show("Đã xóa khỏi danh sách yêu thích", "Thông báo");
            var updatedScreen = new FavoriteFood();
            updatedScreen.Show();
            this.Close();
            
        }

        private void homeMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var listfoodScreen = new ListFood();
            listfoodScreen.Show();
            this.Close();
        }

        private void addMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var addfoodScreen = new AddRecipe();
            addfoodScreen.Show();
            this.Close();
        }
    }
}
