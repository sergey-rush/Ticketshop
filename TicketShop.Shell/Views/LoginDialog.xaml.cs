using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TicketShop.Core;
using TicketShop.Data;
using TicketShop.Shell.Commands;

namespace TicketShop.Shell.Views
{
    public partial class LoginDialog
    {
        public LoginDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoginDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            StringCollection logins = Properties.Settings.Default.Logins;
            LoginBox.ItemsSource = logins;
            if (logins != null && logins.Count > 0)
            {
                LoginBox.SelectedItem = logins[logins.Count - 1];
            }

            //TODO: Remove before release
            PasswordBox.Password = Properties.Settings.Default.Password;
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            SignIn();
        }

        private void SignIn()
        {
            Logger.Default.Append("User clicked login button");
            string login = LoginBox.Text;
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль, нажмите \"Войти\".", "Введите логин и пароль", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Member member = new Member()
            {
                Email = login,
                Pass = pass
            };
            member = Access.Login(member);

            if (member != null && !string.IsNullOrEmpty(member.Token))
            {
                if (member.TokenDate > DateTime.Now)
                {
                    Logger.Default.Append(String.Format("Member {0} successfully logged in with token {1}", member.Id, member.Token));
                    Hide();
                    member.StartDate = DateTime.Now;
                    MainWindow mainWindow = new MainWindow(member);
                    mainWindow.Show();

                    StringCollection logins = Properties.Settings.Default.Logins ?? new StringCollection();

                    if (!logins.Contains(login))
                    {
                        logins.Add(login);
                    }
                    else
                    {
                        logins.Remove(login);
                        logins.Add(login);
                    }

                    Properties.Settings.Default.Logins = logins;
                    //TODO: Remove before release
                    Properties.Settings.Default.Password = pass;
                    Properties.Settings.Default.Save();

                    Close();
                }
                else
                {
                    Logger.Default.Append("Authentication error");
                    MessageBox.Show("Ваша учетная запись более не предоставляет доступ к удаленному подключению. Обратитесь к администратору.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    Properties.Settings.Default.Password = String.Empty;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                Logger.Default.Append("Error. Wrong credentials. No remote access. No internet connection.");
                MessageBox.Show("Неверный логин или пароль. Проверьте интернет соединение. Обратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Properties.Settings.Default.Password = String.Empty;
                Properties.Settings.Default.Save();
            }
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private DelegateCommand _login;

        public ICommand Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new DelegateCommand(OnLogin);
                }
                return _login;
            }
        }

        private void OnLogin()
        {
            SignIn();
        }
    }
}