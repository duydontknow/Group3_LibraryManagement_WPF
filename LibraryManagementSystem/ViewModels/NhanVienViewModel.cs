using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class NhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _listNhanVien;
        public ObservableCollection<Staff> ListNhanVien
        {
            get => _listNhanVien;
            set { _listNhanVien = value; OnPropertyChanged(); }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set { _isAdmin = value; OnPropertyChanged(); }
        }

        private Staff _selectedItem;
        public Staff SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    FullName = _selectedItem.FullName;
                    Phone = _selectedItem.Phone;

                    if (_selectedItem.Account != null)
                    {
                        Username = _selectedItem.Account.Username;
                        Password = _selectedItem.Account.PasswordHash;
                        IsAdmin = _selectedItem.Account.RoleId == 1;
                    }
                    else
                    {
                        Username = "";
                        Password = "";
                        IsAdmin = false;
                    }
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public NhanVienViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>(p =>
            {
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng nhập đủ Họ tên, Tên đăng nhập và Mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var checkAcc = db.Accounts.FirstOrDefault(x => x.Username == Username);
                    if (checkAcc != null)
                    {
                        MessageBox.Show("Tên đăng nhập này đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newAccount = new Account()
                    {
                        Username = Username,
                        PasswordHash = Password,
                        RoleId = IsAdmin ? 1 : 2
                    };
                    db.Accounts.Add(newAccount);
                    db.SaveChanges();

                    var nv = new Staff { FullName = FullName, Phone = Phone, AccountId = newAccount.AccountId };
                    db.Staffs.Add(nv);
                    db.SaveChanges();

                    LoadData();
                    ResetForm();
                    MessageBox.Show("Thêm nhân viên và cấp tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 nhân viên bên danh sách để sửa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng không để trống Họ tên, Tên đăng nhập và Mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var nv = db.Staffs.Find(SelectedItem.StaffId);
                    if (nv != null)
                    {
                        nv.FullName = FullName;
                        nv.Phone = Phone;

                        if (nv.AccountId != null)
                        {
                            var acc = db.Accounts.Find(nv.AccountId);
                            if (acc != null)
                            {
                                var checkAcc = db.Accounts.FirstOrDefault(x => x.Username == Username && x.AccountId != acc.AccountId);
                                if (checkAcc != null)
                                {
                                    MessageBox.Show("Tên đăng nhập đã có người sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                acc.Username = Username;
                                acc.PasswordHash = Password;
                                acc.RoleId = IsAdmin ? 1 : 2;
                            }
                        }
                        
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 nhân viên để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {SelectedItem.FullName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        bool hasBorrowed = db.BorrowRecords.Any(x => x.StaffId == SelectedItem.StaffId);
                        if (hasBorrowed)
                        {
                            MessageBox.Show("Không thể xóa! Nhân viên này đã từng lập phiếu mượn.", "Lỗi Ràng Buộc", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var nv = db.Staffs.Find(SelectedItem.StaffId);
                        if (nv != null)
                        {
                            var accId = nv.AccountId;
                            db.Staffs.Remove(nv);

                            if (accId != null)
                            {
                                var acc = db.Accounts.Find(accId);
                                if (acc != null) db.Accounts.Remove(acc);
                            }

                            db.SaveChanges();
                            LoadData();
                            ResetForm();
                            MessageBox.Show("Đã xóa nhân viên và tài khoản liên quan!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListNhanVien = new ObservableCollection<Staff>(db.Staffs.Include("Account").ToList());
            }
        }

        private void ResetForm()
        {
            FullName = "";
            Phone = "";
            Username = "";
            Password = "";
            IsAdmin = false;
        }
    }
}