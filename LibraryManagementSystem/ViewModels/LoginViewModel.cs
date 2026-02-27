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
            LoginCommand = new RelayCommand<PasswordBox>(p => {
                if (p == null) return;

                string password = p.Password;

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        var acc = db.Accounts.FirstOrDefault(x => x.Username == Username && x.PasswordHash == password);

                        if (acc != null)
                        {
                            MainWindow mainWin = new MainWindow();
                            mainWin.Show();

                            Window loginWin = Window.GetWindow(p);
                            if (loginWin != null)
                            {
                                loginWin.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Không thể kết nối đến Cơ sở dữ liệu. Hãy kiểm tra lại cấu hình SQL!", "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }
}