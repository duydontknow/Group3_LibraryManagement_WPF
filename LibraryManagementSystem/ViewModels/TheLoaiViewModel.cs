using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class TheLoaiViewModel : BaseViewModel
    {
        private ObservableCollection<Category> _listTheLoai;
        public ObservableCollection<Category> ListTheLoai
        {
            get => _listTheLoai;
            set { _listTheLoai = value; OnPropertyChanged(); }
        }

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(); }
        }

        private Category _selectedItem;
        public Category SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    CategoryName = _selectedItem.CategoryName;
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public TheLoaiViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>(p =>
            {
                if (string.IsNullOrEmpty(CategoryName))
                {
                    MessageBox.Show("Vui lòng nhập Tên thể loại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    if (db.Categories.Any(x => x.CategoryName == CategoryName))
                    {
                        MessageBox.Show("Thể loại này đã tồn tại!", "Lỗi");
                        return;
                    }

                    var tl = new Category { CategoryName = CategoryName };
                    db.Categories.Add(tl);
                    db.SaveChanges();

                    LoadData();
                    MessageBox.Show("Thêm thể loại thành công!");
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 thể loại bên phải để sửa!");
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var tl = db.Categories.Find(SelectedItem.CategoryId);
                    if (tl != null)
                    {
                        tl.CategoryName = CategoryName;
                        db.SaveChanges();

                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p => {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 thể loại để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa Thể loại '{SelectedItem.CategoryName}' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        bool isUsed = db.Books.Any(x => x.CategoryId == SelectedItem.CategoryId);

                        if (isUsed)
                        {
                            MessageBox.Show("Không thể xóa! Đang có sách thuộc Thể loại này trong thư viện.\nĐể bảo toàn dữ liệu, vui lòng xóa (hoặc đổi thể loại) cho các sách đó trước khi xóa Thể loại này.",
                                            "Lỗi Ràng Buộc Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        var tl = db.Categories.Find(SelectedItem.CategoryId);
                        if (tl != null)
                        {
                            db.Categories.Remove(tl);
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Đã xóa Thể loại thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListTheLoai = new ObservableCollection<Category>(db.Categories.ToList());
            }
        }
    }
}
