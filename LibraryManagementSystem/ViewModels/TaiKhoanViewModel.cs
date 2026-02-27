using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class TaiKhoanViewModel : BaseViewModel
    {
        private ObservableCollection<Account> _listTaiKhoan;
        public ObservableCollection<Account> ListTaiKhoan { get => _listTaiKhoan; set { _listTaiKhoan = value; OnPropertyChanged(); } }

        private ObservableCollection<Role> _listRoles;
        public ObservableCollection<Role> ListRoles { get => _listRoles; set { _listRoles = value; OnPropertyChanged(); } }

        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private Role _selectedRole;
        public Role SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); } }

        private Account _selectedItem;
        public Account SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    Username = _selectedItem.Username;
                    Password = _selectedItem.PasswordHash;
                    SelectedRole = ListRoles.FirstOrDefault(r => r.RoleId == _selectedItem.RoleId);
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public TaiKhoanViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>(p => {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || SelectedRole == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin!", "Cảnh báo"); return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    if (db.Accounts.Any(x => x.Username == Username))
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi"); return;
                    }

                    var acc = new Account()
                    {
                        Username = Username,
                        PasswordHash = Password,
                        RoleId = SelectedRole.RoleId
                    };
                    db.Accounts.Add(acc);
                    db.SaveChanges();

                    ListTaiKhoan.Add(acc);
                    MessageBox.Show("Thêm thành công!");
                }
            });

            EditCommand = new RelayCommand<object>(p => {
                if (SelectedItem == null) { MessageBox.Show("Vui lòng chọn 1 tài khoản để sửa!"); return; }

                using (var db = new LibraryManagementDBEntities())
                {
                    var acc = db.Accounts.Find(SelectedItem.AccountId);
                    if (acc != null)
                    {
                        acc.Username = Username;
                        acc.PasswordHash = Password;
                        acc.RoleId = SelectedRole.RoleId;
                        db.SaveChanges();

                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p => {
                if (SelectedItem == null) { MessageBox.Show("Vui lòng chọn 1 tài khoản để xóa!"); return; }

                if (MessageBox.Show("Chắc chắn xóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        var acc = db.Accounts.Find(SelectedItem.AccountId);
                        db.Accounts.Remove(acc);
                        db.SaveChanges();

                        ListTaiKhoan.Remove(SelectedItem);
                        MessageBox.Show("Đã xóa!");
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListRoles = new ObservableCollection<Role>(db.Roles.ToList());
                ListTaiKhoan = new ObservableCollection<Account>(db.Accounts.Include("Role").ToList());
            }
        }
    }
}