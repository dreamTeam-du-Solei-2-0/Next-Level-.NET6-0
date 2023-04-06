using Next_Level.ContextData;
using Next_Level.Entity;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Next_Level.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для EditCategories.xaml
    /// </summary>
    public partial class EditCategories : Page
    {
        private DataContext dataContext;
        List<Category> categories;
        public EditCategories()
        {
            InitializeComponent();
            dataContext = new();
            loadCategories();
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
                productCategory.Text = comboCategory.SelectedItem.ToString();
            }
            else
            {
                EditWindow.Visibility = Visibility.Hidden;
                TextBlock text = new TextBlock();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.FontSize = 100;
                text.Text = "No categories";
                text.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                MainWindow.Child = text;
            }
        }
        bool Exist(string name)
        {
            categories = dataContext.Categories.GetCategories();
            if (categories.Count != 0)
            {
                foreach (var category in categories)
                {
                    if (category.Name == name)
                        return true;
                }
            }
            return false;
        }

        private void editCategory_Click(object sender, RoutedEventArgs e)
        {
            var category = dataContext.Categories.GetCategory(comboCategory.SelectedItem.ToString());
            if (productCategory.Text.Trim() != String.Empty||!Exist(productCategory.Text))
            {
                category.Name = productCategory.Text;
                dataContext.Categories.Update(category);
                comboCategory.Items.Clear();
                loadCategories();
            }
        }

    }
}
