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
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Phone))
                {
                    MessageBox.Show("Vui lòng nhập đủ Họ tên và Số điện thoại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var nv = new Staff { FullName = FullName, Phone = Phone };
                    db.Staffs.Add(nv);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo");
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 nhân viên bên danh sách để sửa!");
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var nv = db.Staffs.Find(SelectedItem.StaffId);
                    if (nv != null)
                    {
                        nv.FullName = FullName;
                        nv.Phone = Phone;
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!", "Thông báo");
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 nhân viên để xóa!");
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {SelectedItem.FullName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        var nv = db.Staffs.Find(SelectedItem.StaffId);
                        db.Staffs.Remove(nv);
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Đã xóa nhân viên!", "Thông báo");
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListNhanVien = new ObservableCollection<Staff>(db.Staffs.ToList());
            }
        }
    }
}