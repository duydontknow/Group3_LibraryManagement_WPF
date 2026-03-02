using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Views;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<object>(p => {
                if (p == null) return;

                var passwordBox = p as System.Windows.Controls.PasswordBox;
                string password = passwordBox.Password;
                string username = Username; 

                using (var db = new LibraryManagementDBEntities())
                {
                    var acc = db.Accounts.FirstOrDefault(x => x.Username == username && x.PasswordHash == password);

                    if (acc != null)
                    {
                        UserSession.AccountId = acc.AccountId;
                        UserSession.RoleId = acc.RoleId ?? 2; 

                        MainWindow main = new MainWindow();
                        main.Show();

                        var loginWindow = System.Windows.Window.GetWindow(passwordBox);
                        if (loginWindow != null)
                        {
                            loginWindow.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
        }
    }
}