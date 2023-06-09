﻿using Microsoft.Win32;
using Next_Level.Classes;
using Next_Level.ContextData;
using Next_Level.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Next_Level
{
    
    public partial class ProductInfo : Page
    {
        IFile file;
        private readonly DataContext dataContext;
        Entity.Account current_user;
        Entity.Product product = null;

        SolidColorBrush gridColor;
        SolidColorBrush textColor;
        List<Entity.Product> productList;
        List<Entity.Feedback> feedbacks;
        public ProductInfo(string id)
        {
            InitializeComponent();
            dataContext = new();
            LoadCurrentUser();
            LoadProduct(id);
            LoadColors();
            LoadComments();
           
        }

        #region LOAD_ALL_DATA

        //загрузка текущего пользователя
        void LoadCurrentUser()
        {
            string path_currentUser = NextLevelPath.CURRENT_USER;
            file = new BinnaryFile(path_currentUser);
            current_user = dataContext.Accounts.GetAccount(file.Load<string>());
        }

        //подгружает продукты из бд
        //void LoadProductList() => _products = new ProductList();

        //находит нужный продукт
        void LoadProduct(string id)
        {
            productList = dataContext.Products.GetProducts();
            foreach (var prod in productList)
            {
                if (prod.ProductId.ToString().Contains(id))
                {
                    product = prod;
                    break;
                }
            }
            LoadPriceDescription();
            LoadImage();
        }

        //загрузка цветов
        void LoadColors()
        {
            gridColor = (SolidColorBrush)FindResource("SecundaryBackgroundColor");
            textColor = (SolidColorBrush)FindResource("PrimaryTextColor");
        }

        //подгружает комментарии из бд
        void LoadComments()
        {
            Coments.Children.Clear();
            feedbacks = product.Feedbacks;
            if (feedbacks.Count != 0)
            {
                foreach (var feedback in feedbacks)
                {
                    Coments.Children.Add(CreateGrid(feedback, gridColor, textColor));
                }
            }
        }

        //подгружает описание и цену из бд
        void LoadPriceDescription()
        {
            if (product.Description == string.Empty)
                Description.Text = "No description";
            else
                Description.Text = product.Description;

            Price.Text = "Price: " + product.Price.ToString("0.00");
            Count.Text = "Count: " + product.Count.ToString();
        }

        //подгружает фото из бд
        void LoadImage()
        {
            productImage.Source = loadPhoto(product.Photo);
        }

        #endregion

        #region EVENTS      
        //оставить отзыв нажатием на кнопку
        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ComW.Text))
            {
                Entity.Feedback feedback = new();
                feedback.AccountId = current_user.AccountId;
                feedback.ProductId = product.ProductId;
                feedback.Text = ComW.Text;
                dataContext.Feedbacks.Add(feedback);
                feedbacks.Add(feedback);
                Coments.Children.Add(CreateGrid(feedback, current_user,gridColor, textColor));
               
                ComW.Clear();
            }
        }
        //оставить отзыв нажатием на Enter
        private void products_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if (!string.IsNullOrEmpty(ComW.Text))
                {
                    Entity.Feedback feedback = new();
                    feedback.AccountId = current_user.AccountId;
                    feedback.ProductId = product.ProductId;
                    feedback.Text = ComW.Text;
                    dataContext.Feedbacks.Add(feedback);
                    feedbacks.Add(feedback);
                    Coments.Children.Add(CreateGrid(feedback, current_user, gridColor, textColor));

                    ComW.Clear();
                }
            }
        }

        private void deleteFeedback(object sender, RoutedEventArgs e) 
        {
            Button but = (Button)sender;
            Entity.Feedback temp=new();
            foreach(var feed in feedbacks)
            {
                if(feed.FeedbackId.ToString().Contains(but.Name.Remove(0, 1)))
                {
                    temp = feed;
                    break;
                }
            }
            dataContext.Feedbacks.Delete(temp);
            LoadComments();
        }
        
        #endregion

        #region CREATE_ELEMENTS

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

        //Установка 16-ричного цвета
        Brush SetColor(string hex)
        {
            return (Brush)(new BrushConverter().ConvertFrom(hex));
        }
        Border CreateGrid(Entity.Feedback feedback,Entity.Account account, SolidColorBrush gridColor, SolidColorBrush textColor)
        {
            //тело отзыва
            Border border = new Border();
            border.Margin = new Thickness(5);
            border.CornerRadius = new CornerRadius(8);
            border.MaxHeight = 350;
            border.MinHeight = 150;
            border.Background = gridColor;
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.BlurRadius = 8;
            shadowEffect.Opacity = 0.5;
            border.Effect = shadowEffect;

            Grid myGrid = new Grid();
            myGrid.Margin = new Thickness(2);
            myGrid.MinHeight = 150;
            myGrid.MaxHeight = 350;
            //myGrid.ShowGridLines = true;

            RowDefinition[] rows = new RowDefinition[3];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = new RowDefinition();

            ColumnDefinition[] columns = new ColumnDefinition[4];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = new ColumnDefinition();

            rows[0].Height = new GridLength(30);
            //rows[1].Height = new GridLength(100);
            //rows[3].Height = new GridLength(25);

            columns[0].Width = new GridLength(110);
            columns[1].Width = new GridLength(100);
            columns[3].Width = new GridLength(120);

            myGrid.ColumnDefinitions.Add(columns[0]);
            myGrid.ColumnDefinitions.Add(columns[1]);
            myGrid.ColumnDefinitions.Add(columns[2]);
            myGrid.ColumnDefinitions.Add(columns[3]);

            myGrid.RowDefinitions.Add(rows[0]);
            myGrid.RowDefinitions.Add(rows[1]);
            myGrid.RowDefinitions.Add(rows[2]);

            //имя пользователя
            TextBlock textName = new TextBlock();
            textName.FontWeight = FontWeights.Bold;
            textName.Text = account.Login;
            textName.Foreground = textColor;
            textName.Margin = new Thickness(10, 0, 0, 0);
            textName.FontSize = 25;

            //текст отзыва
            TextBox feed = new TextBox();
            feed.Margin = new Thickness(10);
            feed.IsReadOnly = true;
            feed.BorderThickness = new Thickness(0);
            feed.Background = gridColor;
            feed.Text = feedback.Text;
            feed.Foreground = textColor;
            feed.TextWrapping = TextWrapping.Wrap;
            feed.Margin = new Thickness(10, 0, 0, 0);
            feed.FontSize = 15;

            //время
            TextBlock currentDate = new TextBlock();
            currentDate.Margin = new Thickness(10);
            currentDate.Text = feedback.Date.ToString();
            currentDate.Foreground = textColor;
            currentDate.TextWrapping = TextWrapping.Wrap;
            currentDate.Margin = new Thickness(10, 0, 0, 0);
            currentDate.FontSize = 12;
            currentDate.VerticalAlignment = VerticalAlignment.Center;

            //delete feedback
            Button deleteFeed = new Button();
            deleteFeed.BorderThickness = new Thickness(0);
            deleteFeed.Background = Brushes.Transparent;
            deleteFeed.HorizontalAlignment = HorizontalAlignment.Right;
            deleteFeed.Margin = new Thickness(0, 0, 5, 0);
            deleteFeed.Click += new RoutedEventHandler(deleteFeedback);
            var feedId = feedback.FeedbackId.ToString();
            var index = feedId.IndexOf('-');
            deleteFeed.Name = "_" + feedId.Substring(0, index);
            TextBlock deleteText = new TextBlock();
            deleteText.TextDecorations = TextDecorations.Underline;
            deleteText.FontSize = 15;
            deleteText.Text = "Delete";
            deleteText.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
            deleteFeed.Content = deleteText;

            ///ДОБАВЛЕНИЕ В ГРИД

            //фото
            Grid grid1 = new Grid();
            grid1.Height = 100;
            grid1.Width = 100;
            grid1.Margin = new Thickness(5);
            grid1.Background = SetColor("#B4B4B4");
            Grid.SetRowSpan(grid1, 2);
            myGrid.Children.Add(grid1);

            //имя пользователя
            Grid.SetRow(textName, 0);
            Grid.SetColumn(textName, 1);
            Grid.SetColumnSpan(textName, 2);
            myGrid.Children.Add(textName);

            //отзыв
            Grid.SetRow(feed, 1);
            Grid.SetRowSpan(feed, 2);
            Grid.SetColumn(feed, 1);
            Grid.SetColumnSpan(feed, 3);
            myGrid.Children.Add(feed);


            //текущая дата
            Grid.SetColumn(currentDate, 3);
            myGrid.Children.Add(currentDate);
            sc.MinHeight = 150;
            sc.MaxHeight = 1000;
            Grid.SetColumn(deleteFeed, 2);
            Grid.SetRow(deleteFeed, 0);
            myGrid.Children.Add(deleteFeed);
  

            border.Child = myGrid;
            return border;
        }
        //Создаёт комментарий
        Border CreateGrid(Entity.Feedback feedback, SolidColorBrush gridColor, SolidColorBrush textColor)
        {
            //тело отзыва
            Border border = new Border();
            border.Margin = new Thickness(5);
            border.CornerRadius = new CornerRadius(8);
            border.MaxHeight = 350;
            border.MinHeight = 150;
            border.Background = gridColor;
            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.BlurRadius = 8;
            shadowEffect.Opacity = 0.5;
            border.Effect = shadowEffect;

            Grid myGrid = new Grid();
            myGrid.Margin = new Thickness(2);
            myGrid.MinHeight = 150;
            myGrid.MaxHeight = 350;
            //myGrid.ShowGridLines = true;

            RowDefinition[] rows = new RowDefinition[3];
            for (int i = 0; i < rows.Length; i++)
                rows[i] = new RowDefinition();

            ColumnDefinition[] columns = new ColumnDefinition[4];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = new ColumnDefinition();

            rows[0].Height = new GridLength(30);
            //rows[1].Height = new GridLength(100);
            //rows[3].Height = new GridLength(25);

            columns[0].Width = new GridLength(110);
            columns[1].Width = new GridLength(100);
            columns[3].Width = new GridLength(120);

            myGrid.ColumnDefinitions.Add(columns[0]);
            myGrid.ColumnDefinitions.Add(columns[1]);
            myGrid.ColumnDefinitions.Add(columns[2]);
            myGrid.ColumnDefinitions.Add(columns[3]);

            myGrid.RowDefinitions.Add(rows[0]);
            myGrid.RowDefinitions.Add(rows[1]);
            myGrid.RowDefinitions.Add(rows[2]);

            //имя пользователя
            TextBlock textName = new TextBlock();
            textName.FontWeight = FontWeights.Bold;
            textName.Text = feedback.Account.Login;
            textName.Foreground = textColor;
            textName.Margin = new Thickness(10, 0, 0, 0);
            textName.FontSize = 25;

            //текст отзыва
            TextBox feed = new TextBox();
            feed.Margin = new Thickness(10);
            feed.IsReadOnly = true;
            feed.BorderThickness = new Thickness(0);
            feed.Background = gridColor;
            feed.Text = feedback.Text;
            feed.Foreground = textColor;
            feed.TextWrapping = TextWrapping.Wrap;
            feed.Margin = new Thickness(10, 0, 0, 0);
            feed.FontSize = 15;

            //время
            TextBlock currentDate = new TextBlock();
            currentDate.Margin = new Thickness(10);
            currentDate.Text = feedback.Date.ToString();
            currentDate.Foreground = textColor;
            currentDate.TextWrapping = TextWrapping.Wrap;
            currentDate.Margin = new Thickness(10, 0, 0, 0);
            currentDate.FontSize = 12;
            currentDate.VerticalAlignment = VerticalAlignment.Center;

            //delete feedback
            Button deleteFeed = new Button();
            deleteFeed.BorderThickness = new Thickness(0);
            deleteFeed.Background = Brushes.Transparent;
            deleteFeed.HorizontalAlignment = HorizontalAlignment.Right;
            deleteFeed.Margin=new Thickness(0,0, 5, 0);
            deleteFeed.Click += new RoutedEventHandler(deleteFeedback);
            var feedId = feedback.FeedbackId.ToString();
            var index = feedId.IndexOf('-');
            deleteFeed.Name ="_"+ feedId.Substring(0,index);
            TextBlock deleteText = new TextBlock();
            deleteText.TextDecorations = TextDecorations.Underline;
            deleteText.FontSize = 15;
            deleteText.Text = "Delete";
            deleteText.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
            deleteFeed.Content = deleteText;

            ///ДОБАВЛЕНИЕ В ГРИД

            //фото
            Grid grid1 = new Grid();
            grid1.Height = 100;
            grid1.Width = 100;
            grid1.Margin = new Thickness(5);
            grid1.Background = SetColor("#B4B4B4");
            Grid.SetRowSpan(grid1, 2);
            myGrid.Children.Add(grid1);

            //имя пользователя
            Grid.SetRow(textName, 0);
            Grid.SetColumn(textName, 1);
            Grid.SetColumnSpan(textName, 2);
            myGrid.Children.Add(textName);

            //отзыв
            Grid.SetRow(feed, 1);
            Grid.SetRowSpan(feed, 2);
            Grid.SetColumn(feed, 1);
            Grid.SetColumnSpan(feed, 3);
            myGrid.Children.Add(feed);


            //текущая дата
            Grid.SetColumn(currentDate, 3);
            myGrid.Children.Add(currentDate);
            sc.MinHeight = 150;
            sc.MaxHeight = 1000;


            //delete feedback
            if (feedback.Account.Login == current_user.Login)
            {
                Grid.SetColumn(deleteFeed, 2);
                Grid.SetRow(deleteFeed, 0);
                myGrid.Children.Add(deleteFeed);
            }

            border.Child = myGrid;
            return border;
        }

        #endregion

        #region GALLERY_EVENTS

        //private void button4_Click(object sender, RoutedEventArgs e)
        //{
        //    //BitmapImage b = new BitmapImage();
        //    //b.UriSource = new Uri("Assets\\Images\\google.png");
        //    //im.Source = b;
        //}
        //private void First_Click(object sender, RoutedEventArgs e)
        //{
        //    MyImage.Source = new BitmapImage(new Uri(files[0]));
        //    counter = 0;

        //}

        //private void Prev_Click(object sender, RoutedEventArgs e)
        //{
        //    if (counter - 1 >= 0)
        //        counter--;
        //    MyImage.Source = new BitmapImage(new Uri(files[counter]));

        //}

        //private void Next_Click(object sender, RoutedEventArgs e)
        //{
        //    if (counter + 1 < files.Count)
        //        counter++;
        //    MyImage.Source = new BitmapImage(new Uri(files[counter]));

        //}

        //private void Last_Click(object sender, RoutedEventArgs e)
        //{
        //    counter = files.Count - 1;
        //    MyImage.Source = new BitmapImage(new Uri(files[counter]));

        //}
        #endregion

    }
}


