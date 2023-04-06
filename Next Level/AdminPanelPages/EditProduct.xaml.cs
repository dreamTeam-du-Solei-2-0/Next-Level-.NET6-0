using Microsoft.Win32;
using Next_Level.ContextData;
using Next_Level.Entity;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Next_Level.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Page
    {
        WrapPanel wrap = null;

        private DataContext dataContext;
        List<Category> categories;

        public EditProduct()
        {
            InitializeComponent();
            dataContext = new();
            LoadProducts();
        }
        
        public void LoadProducts()
        {

            ScrollViewer scroll = createScroll();
            StackPanel myStack = createStackPanel();
            scroll.Content = myStack;
            homeView.Child = scroll;
            categories = dataContext.Categories.GetCategories();
            int idProduct = 0;
            if (categories.Count != 0)
            {
                foreach (var category in categories)
                {
                    if (category.Products.Count != 0)
                    {
                        myStack.Children.Add(createCategory(category.Name));
                        wrap = new WrapPanel();
                        myStack.Children.Add(wrap);
                        foreach (var product in category.Products)
                        {
                            wrap.Children.Add(CreateProduct(product, category, (SolidColorBrush)FindResource("TertiaryBackgroundColor"), (SolidColorBrush)FindResource("PrimaryTextColor")));
                            idProduct++;
                        }
                    }
                }
            }
            else
            {
                TextBlock text = new TextBlock();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.FontSize = 100;
                text.Text = "No categories";
                text.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                homeView.Child = text;
            }
        }
        #region EVENTS

        private void button_InfoProduct(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var index = button.Name.IndexOf("_");
            var category_name = button.Name.Substring(0, index);
            var category = dataContext.Categories.GetCategory(category_name);
            Entity.Product Product = null;
            foreach (var product in category.Products)
            {
                if (product.ProductId.ToString().Contains(button.Name.Substring(index + 1)))
                {
                    Product = product;
                    break;
                }
            }
            homeView.Visibility = Visibility.Hidden;
            basicSettings();
            loadCategories();
            fillFields(Product); 
        }



        #endregion

        #region CREATE COLORS
        SolidColorBrush makeRandomColor()
        {
            Random random = new Random();
            int red = random.Next(0, 255);
            int green = random.Next(0, 255);
            int blue = random.Next(0, 255);
            var color = Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
            return new SolidColorBrush(color);
        }
        SolidColorBrush makeColorRgb(int red, int green, int blue)
        {
            var color = Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
            return new SolidColorBrush(color);
        }
        SolidColorBrush SetColor(string hex)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
        }
        #endregion

        #region CREATE ELEMENTS


        StackPanel createStackPanel()
        {
            StackPanel stackPanel = new StackPanel();
            return stackPanel;
        }

        ScrollViewer createScroll()
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Background = (SolidColorBrush)FindResource("SecundaryBackgroundColor");
            scrollViewer.Margin = new Thickness(10, 0, 10, 0);
            return scrollViewer;
        }
        TextBlock createCategory(string _category)
        {
            TextBlock category = new TextBlock();
            category.Text = _category;
            category.FontSize = 20;
            category.FontWeight = FontWeights.Bold;
            category.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
            category.HorizontalAlignment = HorizontalAlignment.Center;
            category.Margin = new Thickness(10);
            return category;
        }

        BitmapImage loadPhoto(byte[] photo)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(photo);
            bitmapImage.DecodePixelWidth = 120;
            bitmapImage.DecodePixelHeight = 120;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        Border CreateProduct(Entity.Product product, Category category, SolidColorBrush gridColor, SolidColorBrush textColor)
        {
            int ROWS_COUNT = 6;
            int COLUMNS_COUNT = 2;

            //Создание рамки. Для скругления углов
            Border border = new Border();
            border.CornerRadius = new CornerRadius(8);
            border.Background = gridColor;
            border.Height = 265;
            border.Width = 180;
            border.Margin = new Thickness(8);

            //Эффект тени для рамки
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.BlurRadius = 8;
            shadowEffect.Opacity = 0.5;
            border.Effect = shadowEffect;

            //Создание сетки
            Grid myGrid = new Grid();
            myGrid.Height = 265;
            myGrid.Width = 180;
            //показать линии сетки
            //myGrid.ShowGridLines = true;

            //Создание строк для сетки
            RowDefinition[] rows = new RowDefinition[ROWS_COUNT];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = new RowDefinition();

            //Создание столбцов для сетки
            ColumnDefinition[] columns = new ColumnDefinition[COLUMNS_COUNT];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = new ColumnDefinition();

            //добавялем столбцы в сетку
            myGrid.ColumnDefinitions.Add(columns[0]);
            myGrid.ColumnDefinitions.Add(columns[1]);

            //меняем высоту первой строки
            rows[1].Height = new GridLength(120);
            rows[5].Height = new GridLength(50);
            //добавляем строки в сетку
            myGrid.RowDefinitions.Add(rows[0]);
            myGrid.RowDefinitions.Add(rows[1]);
            myGrid.RowDefinitions.Add(rows[2]);
            myGrid.RowDefinitions.Add(rows[3]);
            myGrid.RowDefinitions.Add(rows[4]);
            myGrid.RowDefinitions.Add(rows[5]);

            //Фото товара
            Grid photo = new Grid();
            photo.Height = 120;
            photo.Width = 120;

            //Категория
            Border categoryBorder = new Border();
            categoryBorder.Background = (SolidColorBrush)FindResource("PrimaryBackgroundColor");
            categoryBorder.HorizontalAlignment = HorizontalAlignment.Center;
            categoryBorder.CornerRadius = new CornerRadius(8);
            categoryBorder.Margin = new Thickness(2);

            TextBlock Block = new TextBlock();
            Block.Text = category.Name;
            Block.Margin = new Thickness(3);
            Block.FontSize = 12;
            Block.TextWrapping = TextWrapping.Wrap;
            Block.VerticalAlignment = VerticalAlignment.Center;
            Block.TextAlignment = TextAlignment.Center;
            Block.Foreground = textColor;

            categoryBorder.Child = Block;
            //Добавляю в строку
            Grid.SetRow(categoryBorder, 0);
            //Растягиваю на два столбца
            Grid.SetColumnSpan(categoryBorder, 2);
            //Добавляю текст в сетку
            myGrid.Children.Add(categoryBorder);

            //Загрузка фото
            var productPhoto = loadPhoto(product.Photo);
            if (productPhoto != null)
            {
                Image imageBox = new Image();
                imageBox.Source = productPhoto;
                photo.Children.Add(imageBox);
            }
            else
            {
                TextBlock photoInfo = new TextBlock();
                photoInfo.Text = "#PHOTO#";
                photo.Background = SetColor("#1F1F1F");
                photo.Children.Add(photoInfo);
            }


            Grid.SetRow(photo, 1);
            Grid.SetColumnSpan(photo, 2);
            myGrid.Children.Add(photo);

            //Items count
            TextBlock itemsCount = new TextBlock();
            itemsCount.Text = $"Items count: {product.Count}";
            itemsCount.FontSize = 12;
            itemsCount.TextWrapping = TextWrapping.Wrap;
            itemsCount.VerticalAlignment = VerticalAlignment.Center;
            itemsCount.TextAlignment = TextAlignment.Center;
            itemsCount.Foreground = textColor;
            //Добавляю в строку
            Grid.SetRow(itemsCount, 2);
            //Растягиваю на два столбца
            Grid.SetColumnSpan(itemsCount, 2);
            //Добавляю текст в сетку
            myGrid.Children.Add(itemsCount);

            //Название товара
            TextBlock productName = new TextBlock();
            productName.Text = product.Name;
            productName.FontSize = 15;
            productName.TextWrapping = TextWrapping.Wrap;
            productName.VerticalAlignment = VerticalAlignment.Center;
            productName.TextAlignment = TextAlignment.Center;
            productName.Foreground = textColor;
            //Добавляю в строку
            Grid.SetRow(productName, 3);
            //Растягиваю на два столбца
            Grid.SetColumnSpan(productName, 2);
            //Добавляю текст в сетку
            myGrid.Children.Add(productName);

            //Цена товара
            TextBlock price = new TextBlock();
            price.Text = $"{product.Price.ToString("0.00")} grn";
            price.TextAlignment = TextAlignment.Center;
            price.VerticalAlignment = VerticalAlignment.Center;
            price.FontSize = 15;
            price.Foreground = textColor;
            Grid.SetRow(price, 4);
            Grid.SetColumnSpan(price, 2);
            myGrid.Children.Add(price);

            Border infoBorder = new Border();
            infoBorder.Background = SetColor("#15531C");
            infoBorder.CornerRadius = new CornerRadius(8);
            infoBorder.BorderThickness = new Thickness(1);
            infoBorder.Margin = new Thickness(10);

            //Кнопка информация о товаре
            Button infoBut = new Button();
            infoBut.BorderThickness = new Thickness(0);
            var prodId = product.ProductId.ToString();
            var index = prodId.IndexOf('-');
            infoBut.Name = category.Name + "_" + prodId.Substring(0, index);
            infoBut.Content = "Edit";
            infoBut.Foreground = Brushes.White;
            infoBut.Background = SetColor("#15531C");
            infoBut.Margin = new Thickness(2);
            infoBut.Foreground = Brushes.White;
            infoBut.Click += new RoutedEventHandler(button_InfoProduct);
            infoBorder.Child = infoBut;
            Grid.SetRow(infoBorder, 5);
            Grid.SetColumnSpan(infoBorder, 2);
            myGrid.Children.Add(infoBorder);
            //добавляю в рамку сетку
            border.Child = myGrid;
            return border;
        }
        #endregion

        #region EDIT_PRODUCT
        private DispatcherTimer timer;
        bool FieldsNoEmpty;
        bool CategoryNoEmpty;
        string sourcePhoto=String.Empty;
        Product product1;
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

            productPrice.Text = string.Empty;
            productCount.Text = string.Empty;
            productDescription.Text = string.Empty;

            gridPhoto.Children.Clear();
        }
        private void fillFields(Product product)
        {
            productName.Text = product.Name;

            productPrice.Text = product.Price.ToString("0.00");
            productCount.Text = product.Count.ToString();
            productDescription.Text = product.Description;
            gridPhoto.Children.Add(createImageBox(product.Photo));

            product1 = product;
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

            if (createNew.IsChecked == true)
            {
                if (productCategory.Text.Trim() == String.Empty)
                    CategoryNoEmpty = false;
                else
                    CategoryNoEmpty = true;
            }
            else
                CategoryNoEmpty = true;
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
                if (dataContext.Categories.GetCategory(productCategory.Text) != null)
                {
                    ErrorText.Text = "This category exist";
                    ErrorText.Visibility = Visibility.Visible;
                    return;
                }
                Entity.Product product = product1;
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
                dataContext.Products.Update(product);
                clearFields();
                homeView.Visibility = Visibility.Visible;
                LoadProducts();
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
        Image createImageBox(byte[] photo)
        {
            Image img = new Image();
            img.Source = loadPhoto(photo);
            return img;
        }
        #endregion

        #region CATEGORIES_COMBO_TEXTBOX_EVENTS
        private void createNew_Checked(object sender, RoutedEventArgs e)
        {
            if (createNew.IsChecked == true)
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
            if (createNew.IsChecked == false)
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
        #endregion
    }
}
