using Next_Level.AdminPanelPages;
using Next_Level.Classes;
using Next_Level.ContextData;
using Next_Level.Entity;
using Next_Level.Interfaces;
using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Next_Level.Pages
{
    
    public partial class Login : Window
    {
        string path_saveData = NextLevelPath.SAVE_LOGIN;
        string path_currentUser = NextLevelPath.CURRENT_USER;
        IFile file;
        SaveLogin saveLogin;

        private DispatcherTimer timer;
        private DataContext dataContext;

        bool isSaved;

        bool FieldsNoEmpty;
        public Login()
        {
            InitializeComponent();
           
            //account = new Accounts();
            dataContext = new();
            BaseOptions();
        }

        void BaseOptions()
        {
            LoginPassworError.Visibility = Visibility.Hidden;
            isSaved = false;
            FieldsNoEmpty = false;

            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(CheckFields);
            timer.Start();

            //
            if (File.Exists(path_saveData))
            {
                file = new BinnaryFile(path_saveData);
                saveLogin = file.Load<SaveLogin>();
                SaveLoginCheckBox.IsChecked = true;
                LoginTextBox.Text = saveLogin.login;
                PasswordTextBox.Password = saveLogin.password;
            }
            else
                saveLogin = new SaveLogin();
        }

        #region FIELDS_EVENTS
        private void CheckFields(object sender, EventArgs args)
        {
            if (LoginTextBox.Text.Trim() == String.Empty ||
                 PasswordTextBox.Password.Trim() == String.Empty)
            {
                FieldsNoEmpty = false;
                LoginPassworError.Text = "Field is empty or contains only spaces";
            }
            else
                FieldsNoEmpty = true;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordTextBox.Password) && PasswordTextBox.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
                
                passwordBorder.BorderBrush = Brushes.Gray;
                LoginPassworError.Visibility = Visibility.Hidden;
            }
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordTextBox.Focus();
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LoginTextBox.Text) && LoginTextBox.Text.Length > 0)
            {
                textEmail.Visibility = Visibility.Collapsed;
                loginBorder.BorderBrush = Brushes.Gray;
                LoginPassworError.Visibility = Visibility.Hidden;
            }
            else
                textEmail.Visibility = Visibility.Visible;
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginTextBox.Focus();
        }
        #endregion

        #region WINDOW_EVENTS
        private void Button_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion

        #region BUTTONS_EVENTS
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            if(FieldsNoEmpty)
            {
                Account account = dataContext.Accounts.GetAccount(LoginTextBox.Text);
                if(account!=null)
                {
                    if(account.Password!=PasswordTextBox.Password)
                    {
                        LoginPassworError.Text = "Invalid login or password";
                        loginBorder.BorderBrush = Brushes.Red;
                        passwordBorder.BorderBrush = Brushes.Red;
                        LoginPassworError.Visibility = Visibility.Visible;
                        return;
                    }
                }
                else
                {
                    LoginPassworError.Text = "Invalid login or password";
                    loginBorder.BorderBrush = Brushes.Red;
                    passwordBorder.BorderBrush = Brushes.Red;
                    LoginPassworError.Visibility = Visibility.Visible;
                    return;
                }
                if (SaveLoginCheckBox.IsChecked == true)
                {
                    file = new BinnaryFile(path_saveData);
                    saveLogin = new SaveLogin();
                    saveLogin.save_Data = isSaved;
                    saveLogin.login = LoginTextBox.Text;
                    saveLogin.password = PasswordTextBox.Password;
                    file.Save(saveLogin);
                }
                else
                {
                    if (File.Exists(path_saveData))
                        File.Delete(path_saveData);
                }
                if (LoginTextBox.Text == "SuperAdmin")
                {
                    AdminPanel admin = new AdminPanel();
                    this.Close();
                    admin.Show();
                    return;
                }

                MainWindow main = new MainWindow();
                file = new BinnaryFile(path_currentUser);
                file.Save(LoginTextBox.Text);
                this.Close();
                main.Show();
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Sign_in sign_In = new Sign_in();
            this.Close();
            sign_In.ShowDialog();
        }

        private void LogInButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!FieldsNoEmpty)
            {
                if (LoginTextBox.Text.Trim() == String.Empty)
                {
                    loginBorder.BorderBrush = Brushes.Red;
                }
                if (PasswordTextBox.Password.Trim() == String.Empty)
                {
                    passwordBorder.BorderBrush = Brushes.Red;
                }
                LoginPassworError.Visibility = Visibility.Visible;
            }
        }

        private void LogInButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!FieldsNoEmpty)
            {
                loginBorder.BorderBrush = Brushes.Gray;
                passwordBorder.BorderBrush = Brushes.Gray;
                LoginPassworError.Visibility = Visibility.Hidden;
            }
        }
        #endregion
    }
}
