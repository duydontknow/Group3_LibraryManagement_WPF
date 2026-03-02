using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class DocGiaViewModel : BaseViewModel
    {
        private ObservableCollection<Reader> _listDocGia;
        public ObservableCollection<Reader> ListDocGia
        {
            get => _listDocGia;
            set { _listDocGia = value; OnPropertyChanged(); }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _identifyCard;
        public string IdentifyCard
        {
            get => _identifyCard;
            set { _identifyCard = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private Reader _selectedItem;
        public Reader SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    FullName = _selectedItem.FullName;
                    IdentifyCard = _selectedItem.IdentifyCard;
                    Address = _selectedItem.Address;
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DocGiaViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>(p =>
            {
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(IdentifyCard))
                {
                    MessageBox.Show("Vui lòng nhập tối thiểu Họ tên và CMND/CCCD!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var dg = new Reader { FullName = FullName, IdentifyCard = IdentifyCard, Address = Address };
                    db.Readers.Add(dg);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Đăng ký thẻ độc giả thành công!", "Thông báo");
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 độc giả bên phải để sửa!");
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var dg = db.Readers.Find(SelectedItem.ReaderId);
                    if (dg != null)
                    {
                        dg.FullName = FullName;
                        dg.IdentifyCard = IdentifyCard;
                        dg.Address = Address;
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 độc giả để xóa!");
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn hủy thẻ thư viện của {SelectedItem.FullName}?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        var dg = db.Readers.Find(SelectedItem.ReaderId);
                        db.Readers.Remove(dg);
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Đã hủy thẻ độc giả!", "Thông báo");
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListDocGia = new ObservableCollection<Reader>(db.Readers.ToList());
            }
        }
    }
}