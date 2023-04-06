using Microsoft.Win32;
using Next_Level.Classes;
using Next_Level.ContextData;
using Next_Level.Entity;
using Next_Level.Interfaces;
using Next_Level.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace Next_Level.AdminPanelPages
{
    public partial class AddProduct : Page
    {
        //исходная ссылка на фото которое загружаешь
        string sourcePhoto = String.Empty;

        private readonly DataContext dataContext;

        //список категорий
        List<Category> categories;

        private DispatcherTimer timer;

        bool FieldsNoEmpty;
        bool CategoryNoEmpty;
        public AddProduct()
        {
            InitializeComponent();
            dataContext = new();
            basicSettings();
            loadCategories();
        }
        private void basicSettings()
        {
            ErrorText.Visibility = Visibility.Hidden;
            FieldsNoEmpty = false;
            CategoryNoEmpty = false;
            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(CheckFields);
            timer.Start();
        }
        private void loadCategories()
        {

            categories = dataContext.Categories.GetCategories();
            if (categories.Count != 0)
            {
                foreach (var category in categories)
                {
                    comboCategory.Items.Add(category.Name);
                }
                comboCategory.SelectedIndex = 0;
            }
            else
            {
                createNew.IsChecked = true;
                createNew.IsEnabled = false;
            }
        }
        //очистка полей после того как нажал добавить продукт
        private void clearFields()
        {
            productName.Text = string.Empty;
            productCategory.Text = string.Empty;
            createNew.IsChecked = false;
            createNew.IsEnabled = true;
            productPrice.Text = string.Empty;
            productCount.Text = string.Empty;
            productDescription.Text = string.Empty;
            comboCategory.SelectedIndex = 0;
            gridPhoto.Children.Clear();
        }

        private void CheckFields(object sender, EventArgs args)
        {
           
            if (productName.Text.Trim() == String.Empty ||
                productPrice.Text.Trim() == String.Empty ||
                productCount.Text.Trim() == String.Empty)
            {
                FieldsNoEmpty = false;
            }
            else
                FieldsNoEmpty = true;
            
            if(createNew.IsChecked==true)
            {
                if (productCategory.Text.Trim() == String.Empty)
                    CategoryNoEmpty = false;
                else
                    CategoryNoEmpty = true;
            }
            else
                CategoryNoEmpty = true;
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        #region BUTTONS_EVENTS
        //ДОБАВЛЯЕТ ТОВАР В БАЗУ
        private void addProduct_but(object sender, RoutedEventArgs e)
        { 
            if (FieldsNoEmpty && CategoryNoEmpty)
            {
                bool isValid = Regex.IsMatch(productPrice.Text, @"^[0-9]*\.?[0-9]+$");
                if (!isValid)
                {
                    productPrice.Text = String.Empty;
                    return;
                }
                if(dataContext.Categories.GetCategory(productCategory.Text)!=null)
                {
                    ErrorText.Text = "This category exist";
                    ErrorText.Visibility = Visibility.Visible;
                    return;
                }
                Entity.Product product = new();
                product.Name = productName.Text;
                product.Description = productDescription.Text;
                product.Count = int.Parse(productCount.Text);
                product.Price = double.Parse(productPrice.Text, CultureInfo.InvariantCulture);
                if (sourcePhoto != String.Empty)
                {
                    product.Photo = File.ReadAllBytes(sourcePhoto);
                }
                if (createNew.IsChecked == true)
                {
                    Category category = new();
                    category.Name = productCategory.Text;
                    dataContext.Categories.Add(category);
                    product.CategoryId = category.CategoryId;
                }
                else
                {
                    var category = dataContext.Categories.GetCategory(comboCategory.SelectedItem.ToString());
                    product.CategoryId = category.CategoryId;
                }
                dataContext.Products.Add(product);
                clearFields();
            }
        }
      
        //Достаёт полный путь к картинке которую загружаем для продкута

        private void addProductPhoto_but(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            if (openFile.ShowDialog() == true)
            {
                sourcePhoto = openFile.FileName;
            }
            gridPhoto.Children.Clear();
            gridPhoto.Children.Add(createImageBox(sourcePhoto));
        }

        private void addProduct_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!FieldsNoEmpty)
            {
                ErrorText.Visibility = Visibility.Visible;
                productName.BorderBrush = Brushes.Red;
                productPrice.BorderBrush = Brushes.Red;
                productCount.BorderBrush = Brushes.Red;
                ErrorText.Text = "Fields is empty or conatins only spaces";
            }
            if (createNew.IsChecked == true && !CategoryNoEmpty)
            {
                ErrorText.Visibility = Visibility.Visible;
                productCategory.BorderBrush = Brushes.Red;
                ErrorText.Text = "Fields is empty or conatins only spaces";
            }
        }
        private void addProduct_MouseLeave(object sender, MouseEventArgs e)
        {
            ErrorText.Visibility = Visibility.Hidden;
            productName.BorderBrush = Brushes.Gray;
            productPrice.BorderBrush = Brushes.Gray;
            productCount.BorderBrush = Brushes.Gray;
            productCategory.BorderBrush = Brushes.Gray;
        }
        #endregion

        #region PRODUCT_PHOTO_VIEW
        BitmapImage loadPhoto(string path)
        {
            BitmapImage img = new BitmapImage();
            if (File.Exists(path))
            {
                img.BeginInit();
                img.UriSource = new Uri(path);
                img.DecodePixelWidth = 120;
                img.DecodePixelHeight = 120;
                img.EndInit();
                return img;
            }
            return null;
        }

        //создание имаджбокса
        Image createImageBox(string photoPath)
        {
            Image img = new Image();
            img.Source = loadPhoto(photoPath);
            return img;
        }
        #endregion

        #region CATEGORIES_COMBO_TEXTBOX_EVENTS
        private void createNew_Checked(object sender, RoutedEventArgs e)
        {
            if(createNew.IsChecked==true)
            {
                comboCategory.Visibility = Visibility.Collapsed;
                productCategory.Visibility = Visibility.Visible;
            }
            
        }

        private void createNew_Unchecked(object sender, RoutedEventArgs e)
        {
            if (createNew.IsChecked == false)
            {
                comboCategory.Visibility = Visibility.Visible;
                productCategory.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region VIEW_BLOCK_EVENTS
        private void comboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(createNew.IsChecked==false)
                categoryBlock.Text = comboCategory.SelectedItem.ToString();
        }

        private void productName_TextChanged(object sender, TextChangedEventArgs e)
        {
           if (productName.Text.Trim() != string.Empty)
               productBlock.Text = productName.Text;
           else
               productBlock.Text = "ProductName";
        }
        private void productCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (createNew.IsChecked == true)
            {
                if (productCategory.Text.Trim() != string.Empty)
                    categoryBlock.Text = productCategory.Text;
                else
                    categoryBlock.Text = "Category";
            }
        }

        private void productPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productPrice.Text.Contains(","))
                productPrice.Text = productPrice.Text.Replace(",", ".");
           
            if (productPrice.Text.Trim() != string.Empty)
                priceBlock.Text = productPrice.Text;
            else
                priceBlock.Text = "Price";
        }

        private void productCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            bool isMatch = regex.IsMatch(productCount.Text);
            if (!isMatch)
                productCount.Text = String.Empty;
            if (productCount.Text.Trim() != string.Empty)
                itemBlock.Text = productCount.Text;
            else
                itemBlock.Text = "Items count";
        }
        #endregion

        
    }
}
