using Next_Level.Classes;
using Next_Level.ContextData;
using Next_Level.Entity;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Next_Level.Pages
{

    public partial class Sign_in : Window
    {
        private readonly DataContext dataContext;
        private DispatcherTimer timer;

        bool FieldsNoEmpty;
        bool PasswordFormatCorrect;
        bool EmailFormatCorrect;
        public Sign_in()
        {
            InitializeComponent();
            dataContext = new();
            BaseOptions();
        }
        void BaseOptions()
        {
            FieldsNoEmpty = false;
            PasswordFormatCorrect=false;
            EmailFormatCorrect=true;
            ErrorText.Visibility = Visibility.Hidden;

            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(CheckFields);
            timer.Start();
        }

        private void CheckFields(object sender, EventArgs args)
        {
            if (LoginTextBox.Text.Trim() == String.Empty &&
                NameTextBox.Text.Trim() == String.Empty &&
                SurnameTextBox.Text.Trim() == String.Empty &&
                PasswordTextBox.Password.Trim() == String.Empty)
            {
                FieldsNoEmpty = false;
                ErrorText.Text = "*Field is empty or contains only spaces";
            }
            else
                FieldsNoEmpty = true;

            if (!EnterControl.CheckPass(PasswordTextBox.Password))
                PasswordFormatCorrect = false;
            else PasswordFormatCorrect = true;

            if (EmailTextBox.Text.Trim() != String.Empty)
            {
                if (!EnterControl.CheckEmail(EmailTextBox.Text))
                    EmailFormatCorrect = false;
                else
                    EmailFormatCorrect = true;
            }
            else
                EmailFormatCorrect = true;

        }
        #region WINDOW_EVENTS
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
        private void Button_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region HINT_TEXT_EVENTS
        private void txtLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(LoginTextBox.Text) && LoginTextBox.Text.Length > 0)
            {
                textLogin.Visibility = Visibility.Collapsed;
                ErrorText.Visibility = Visibility.Hidden;
                LoginBorder.BorderBrush = Brushes.Gray;
            }
            else
                textLogin.Visibility = Visibility.Visible;
        }

        private void textLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginTextBox.Focus();
        }

        private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NameTextBox.Text) && NameTextBox.Text.Length > 0)
                textName.Visibility = Visibility.Collapsed;
            else
                textName.Visibility = Visibility.Visible;
        }

        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NameTextBox.Focus();
        }

        private void txtSurname_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SurnameTextBox.Text) && SurnameTextBox.Text.Length > 0)
                textSurname.Visibility = Visibility.Collapsed;
            else
                textSurname.Visibility = Visibility.Visible;
        }

        private void textSurname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SurnameTextBox.Focus();
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTextBox.Text) && EmailTextBox.Text.Length > 0)
                textEmail.Visibility = Visibility.Collapsed;
            else
                textEmail.Visibility = Visibility.Visible;
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailTextBox.Focus();
        }

        private void userPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordTextBox.Password) && PasswordTextBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
            Regex ch = new Regex("^.{8,}$");
            if (ch.IsMatch(PasswordTextBox.Password))
            {
                Characters.Foreground = new SolidColorBrush(Color.FromRgb(58, 177, 155));
            }
            else
            {
                Characters.Foreground = new SolidColorBrush(Color.FromRgb(198, 0, 0));
            }
            Regex number = new Regex("^(?=.*?[0-9])");
            if (number.IsMatch(PasswordTextBox.Password))
            {
                Number.Foreground = new SolidColorBrush(Color.FromRgb(58, 177, 155));
            }
            else
            {
                Number.Foreground = new SolidColorBrush(Color.FromRgb(198, 0, 0));
            }
            Regex special = new Regex("^(?=.*?[_#?!@$%^&*-])");
            if (special.IsMatch(PasswordTextBox.Password))
            {
                Special.Foreground = new SolidColorBrush(Color.FromRgb(58, 177, 155));
            }
            else
            {
                Special.Foreground = new SolidColorBrush(Color.FromRgb(198, 0, 0));
            }
            Regex capital = new Regex("^(?=.*?[A-Z])");
            if (capital.IsMatch(PasswordTextBox.Password))
            {
                Capital.Foreground = new SolidColorBrush(Color.FromRgb(58, 177, 155));
            }
            else
            {
                Capital.Foreground = new SolidColorBrush(Color.FromRgb(198, 0, 0));
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordTextBox.Focus();
        }

        #endregion

        #region REGISTRATION_BUTTONS
        private void createAccount_Click(object sender, RoutedEventArgs e)
        {
            if (dataContext.Accounts.IsExist(LoginTextBox.Text))
            {
                ErrorText.Text = "*Login is exist";
                ErrorText.Visibility = Visibility.Visible;
                LoginBorder.BorderBrush = Brushes.Red;
                return;
            }
            if (FieldsNoEmpty && EmailFormatCorrect && PasswordFormatCorrect) 
            {
                Account account = new Account();
                account.Login = LoginTextBox.Text;
                if (account.Login == "SuperAdmin")
                    account.IsAdmin = true;
                account.Password = PasswordTextBox.Password;

                Entity.User user = new Entity.User();
                user.Name = NameTextBox.Text;
                user.Surname = SurnameTextBox.Text;
                user.Email = EmailTextBox.Text;
                user.AccountId = account.AccountId;

                dataContext.Accounts.Add(account);
                dataContext.Users.Add(user);
                Login _login = new Login();
                this.Close();
                _login.ShowDialog();
            }
        }

        private void cancelClick(object sender,RoutedEventArgs e)
        {
            Login _login = new Login();
            this.Close();
            _login.ShowDialog();
        }
        #endregion

        private void createAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!FieldsNoEmpty || !EmailFormatCorrect || !PasswordFormatCorrect)
            {
                if (LoginTextBox.Text.Trim() == String.Empty)
                {
                    LoginBorder.BorderBrush = Brushes.Red;
                }
                if (NameTextBox.Text.Trim() == String.Empty)
                {
                    NameBorder.BorderBrush = Brushes.Red;
                }
                if (SurnameTextBox.Text.Trim() == String.Empty)
                {
                    SurnameBorder.BorderBrush = Brushes.Red;
                }
                if (PasswordTextBox.Password.Trim() == String.Empty ||
                    !EnterControl.CheckPass(PasswordTextBox.Password))
                {
                    PasswordBorder.BorderBrush = Brushes.Red;
                }
                if (EmailTextBox.Text.Trim() != String.Empty)
                {
                    if (!EnterControl.CheckEmail(EmailTextBox.Text))
                        EmailBorder.BorderBrush = Brushes.Red;
                }
                else
                    EmailBorder.BorderBrush = Brushes.Gray;
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void createAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            LoginBorder.BorderBrush = Brushes.Gray;
            NameBorder.BorderBrush = Brushes.Gray;
            SurnameBorder.BorderBrush = Brushes.Gray;
            PasswordBorder.BorderBrush = Brushes.Gray;
            EmailBorder.BorderBrush = Brushes.Gray;
            ErrorText.Visibility = Visibility.Hidden;
        }
    }
}
