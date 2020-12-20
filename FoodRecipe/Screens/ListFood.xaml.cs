using FoodRecipe.DAO;
using FoodRecipe.Db;
using FoodRecipe.DTO;
using FoodRecipe.Helper;
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
    /// Interaction logic for ListFood.xaml
    /// </summary>
    public partial class ListFood : Window
    {
        private BindingList<Food> _list = new BindingList<Food>();
        private int perPage;
        private int page = 1;
        private string sortBy;
        private string search = "";
        private string modeSearch = "exact";
        Paging paging;
        int pageSelected = 1;

        public ListFood()
        {
            InitializeComponent();
        }

        class PageInfo
        {
            public int Page { get; set; }
            public int TotalPages { get; set; }
        }

        class Paging
        {
            private int _totalPages;
            public int CurrentPage { get; set; }

            public int RowsPerPage { get; set; }

            public int TotalPages
            {
                get => _totalPages; set
                {
                    _totalPages = value;
                    Pages = new List<PageInfo>();
                    for (int i = 1; i <= _totalPages; i++)
                    {
                        Pages.Add(new PageInfo()
                        {
                            Page = i,
                            TotalPages = _totalPages
                        });
                    }
                }
            }

            public List<PageInfo> Pages { get; set; }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            modeSearch = ((ComboBoxItem)modeSearchComboBox.SelectedItem).Tag?.ToString();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            perPage = int.Parse(config.AppSettings.Settings["PerPage"].Value);
            perPageTextbox.Text = perPage.ToString();

            
            calculatePagingInfo(searchTextBox.Text, ((ComboBoxItem)modeSearchComboBox.SelectedItem).Tag?.ToString());
            displayProducts();
        }

        void calculatePagingInfo(string searchKey, string searchMode) 
        {
            var count = FoodDAO.CountProducts(searchKey, searchMode);

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var rowsPerPage = int.Parse(config.AppSettings.Settings["PerPage"].Value);

            paging = new Paging
            {
                CurrentPage = 1,
                RowsPerPage = rowsPerPage,
                TotalPages = count / rowsPerPage +
                    (((count % rowsPerPage) == 0) ? 0 : 1)

            };

            createPagingUI(paging.TotalPages);
        }

        private void createPagingUI(int totalPage)
        {
            pagingStackPanel.Children.Clear();

            var firstCreatedButton = createPagingButton(true, "1");
            pagingStackPanel.Children.Add(firstCreatedButton);

            
            for (int i = 2; i <= totalPage; i++)
            {
                var createdButton = createPagingButton(false, i.ToString());

                pagingStackPanel.Children.Add(createdButton);
            }
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
            result.Background = isFirst? Brushes.LightBlue: Brushes.Transparent;
            result.Click += pagingButton_Click;

            return result;
        }

        private void resetActivePagingButton()
        {
            var pagingButtons = pagingStackPanel.Children;

            foreach(var el in pagingButtons)
            {
                ((Button)el).Background = Brushes.Transparent;
            }
            
        }
       
        private void pagingButton_Click(object sender, RoutedEventArgs e)
        {
            resetActivePagingButton();
            ((Button)sender).Background = Brushes.LightBlue;

            pageSelected = int.Parse((((Button)sender).Content).ToString());

            displayProducts();
        }


        void displayProducts()
        {
            _list = FoodDAO.GetProducts(paging.RowsPerPage, pageSelected, sortBy, search, modeSearch);
            dataListView.ItemsSource = _list;
        }

        private void resetInitialPagingButton()
        {
            resetActivePagingButton();
            var firstButton = pagingStackPanel.Children.OfType<Button>().FirstOrDefault();
            if (firstButton != null) { 
                firstButton.Background = Brushes.LightBlue;
            }
        }

        private void Button_Click_Fav(object sender, RoutedEventArgs e)
        {
            var favScreen = new FavoriteFood();
            favScreen.Show();
            this.Close();
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            var addScreen = new AddRecipe();
            addScreen.Show();
            this.Close();
        }

        private void Button_Click_AddToFav(object sender, RoutedEventArgs e)
        {
            var id = ((Button)sender).Tag;
            bool result = FoodDAO.updateIsFavorite(id.ToString(), true);
            
            if(!result)
            {
                MessageBox.Show("Lỗi id không đúng", "Thông báo");
            }
            
            MessageBox.Show("Đã thêm vào danh sách yêu thích", "Thông báo");
        }

        private void Button_Click_Detail(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(((Button)sender).Tag.ToString());
            var detailScreen = new RecipeDetail(id);
            detailScreen.Show();
            //this.Close();
        }


        private void Button_Click_ChangePerPage(object sender, RoutedEventArgs e)
        {
            int maxValue = 12;
            string enteredPerPage = perPageTextbox.Text;
            if(int.Parse(enteredPerPage) > maxValue)
            {
                MessageBox.Show($"Chương trình chỉ hỗ trợ tối đa {maxValue} sản phẩm trên 1 trang cho màn hình mặc định. Và bé hơn 30 cho full màn hình", "Thông báo");
                
            }
            if (int.Parse(enteredPerPage) > 30)
            {
                MessageBox.Show($"Chương trình chỉ hỗ trợ tối đa {maxValue} sản phẩm trên 1 trang cho màn hình mặc định. Và bé hơn 30 cho full màn hình", "Thông báo");
                return;
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["PerPage"].Value = enteredPerPage;
            config.Save(ConfigurationSaveMode.Minimal);

            calculatePagingInfo(search, modeSearch);
            displayProducts();


            
        }

        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sortBy = ((ComboBoxItem)sortComboBox.SelectedItem).Tag.ToString();

            //_list = FoodDAO.GetAll(perPage, page, sortBy);
            int perPage;
            if(paging == null)
            {
                perPage = 0;
            } else
            {
                perPage = paging.RowsPerPage;
            }
            _list = FoodDAO.GetProducts(perPage, page, sortBy, search, modeSearch);
            dataListView.ItemsSource = _list;

            if (pagingStackPanel != null) {
                resetInitialPagingButton();
            };
        }


        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredSearch = searchTextBox.Text.Trim();
            search = enteredSearch;

            if(modeSearch == "smart")
            {
                search = SearchHelper.ConvertToUnSign(enteredSearch);
            }


            calculatePagingInfo(search, modeSearch);
            displayProducts();
        }

        private void modeSearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            modeSearch = ((ComboBoxItem)modeSearchComboBox.SelectedItem).Tag?.ToString();
            
        }
    }
}
