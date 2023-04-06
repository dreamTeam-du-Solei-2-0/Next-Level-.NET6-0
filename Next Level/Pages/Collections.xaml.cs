using Next_Level.Classes;
using Next_Level.ContextData;
using Next_Level.Entity;
using Next_Level.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Next_Level.Pages
{
    public partial class Collections : Page
    {
        List<Entity.Category> categories;
        private readonly DataContext dataContext;
        //ProductList products;
        public Collections()
        {
            InitializeComponent();
            dataContext = new();
            //products = new ProductList();
            LoadCategories();
        }

        #region EVENTS

        private void showCategory(object sender, RoutedEventArgs e)
        {
            Button but = (Button)sender;
            productsPanel.Children.Clear();
            Category temp = dataContext.Categories.GetCategory(but.Name);
            var products = temp.Products;
            foreach(var prod in products)
            {
                productsPanel.Children.Add(CreateProduct(prod, (SolidColorBrush)FindResource("TertiaryBackgroundColor"), (SolidColorBrush)FindResource("PrimaryTextColor")));
            }
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

        private void LoadCategories()
        {
            IFile file = new XmlFormat(NextLevelPath.CATEGORIES_PATH);
            categories = dataContext.Categories.GetCategories();
            if(categories.Count!=0)
            {
                foreach(var category in categories)
                {
                        categoriesPanel.Children.Add(createCategory(category)); 
                }
            }
            else
            {
                TextBlock text = new TextBlock();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.FontSize = 100;
                text.Text = "No products";
                text.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");

                productsPanel.Children.Add(text);
            }
        }

        #region CREATE_ELEMENTS
        private Border createCategory(Entity.Category category)
        {
            Border categoryBorder = new Border();
            categoryBorder.Background = (SolidColorBrush)FindResource("PrimaryBackgroundColor");
            categoryBorder.CornerRadius = new CornerRadius(8);
            categoryBorder.BorderThickness = new Thickness(1);
            categoryBorder.Margin = new Thickness(10);

            //Кнопка купить
            Button categoryBut = new Button();
            categoryBut.BorderThickness = new Thickness(0);
            categoryBut.Content = category.Name;
            categoryBut.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
            categoryBut.Background = (SolidColorBrush)FindResource("PrimaryBackgroundColor");
            categoryBut.FontSize = 20;
            categoryBut.Margin = new Thickness(2);
            categoryBut.Name = category.Name;
            categoryBut.Click += new RoutedEventHandler(showCategory);

            categoryBorder.Child = categoryBut;
            return categoryBorder;
        }

        BitmapImage loadPhoto(byte[]photo)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(photo);
            bitmapImage.DecodePixelWidth = 120;
            bitmapImage.DecodePixelHeight = 120;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        Border CreateProduct(Entity.Product product, SolidColorBrush gridColor, SolidColorBrush textColor)
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

            TextBlock category = new TextBlock();
            category.Text = product.Category.Name;
            category.Margin = new Thickness(3);
            category.FontSize = 12;
            category.TextWrapping = TextWrapping.Wrap;
            category.VerticalAlignment = VerticalAlignment.Center;
            category.TextAlignment = TextAlignment.Center;
            category.Foreground = textColor;

            categoryBorder.Child = category;
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

            Border buyBorder = new Border();
            buyBorder.Background = SetColor("#15531C");
            buyBorder.CornerRadius = new CornerRadius(8);
            buyBorder.BorderThickness = new Thickness(1);
            buyBorder.Margin = new Thickness(10);

            //Кнопка купить
            Button buyBut = new Button();
            buyBut.BorderThickness = new Thickness(0);
            buyBut.Content = "Buy";
            buyBut.Foreground = Brushes.White;
            buyBut.Background = SetColor("#15531C");
            buyBut.Margin = new Thickness(2);
            buyBut.Foreground = Brushes.White;
            //buyBut.Click += new RoutedEventHandler(button_BuyProduct);
            buyBorder.Child = buyBut;
            Grid.SetRow(buyBorder, 5);
            myGrid.Children.Add(buyBorder);

            Border infoBorder = new Border();
            infoBorder.Background = SetColor("#d32f2f");
            infoBorder.CornerRadius = new CornerRadius(8);
            infoBorder.BorderThickness = new Thickness(1);
            infoBorder.Margin = new Thickness(10);

            //Кнопка информация о товаре
            Button infoBut = new Button();
            infoBut.BorderThickness = new Thickness(0);
            infoBut.Content = "About";
            infoBut.Foreground = Brushes.White;
            infoBut.Background = SetColor("#d32f2f");
            infoBut.Margin = new Thickness(2);
            infoBut.Foreground = Brushes.White;
            //infoBut.Click += new RoutedEventHandler(button_InfoProduct);
            infoBorder.Child = infoBut;
            Grid.SetRow(infoBorder, 5);
            Grid.SetColumn(infoBorder, 1);
            myGrid.Children.Add(infoBorder);

            //добавляю в рамку сетку
            border.Child = myGrid;
            return border;
        }
        #endregion
    }
}
