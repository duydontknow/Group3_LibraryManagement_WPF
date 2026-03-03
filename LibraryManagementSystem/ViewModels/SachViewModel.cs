using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class SachViewModel : BaseViewModel
    {
        private ObservableCollection<Book> _listSach;
        public ObservableCollection<Book> ListSach
        {
            get => _listSach;
            set { _listSach = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Category> _listTheLoai;
        public ObservableCollection<Category> ListTheLoai
        {
            get => _listTheLoai;
            set { _listTheLoai = value; OnPropertyChanged(); }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _author;
        public string Author
        {
            get => _author;
            set { _author = value; OnPropertyChanged(); }
        }

        private int? _quantity;
        public int? Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        private int? _selectedCategoryId;
        public int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set { _selectedCategoryId = value; OnPropertyChanged(); }
        }

        private Book _selectedItem;
        public Book SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    Title = _selectedItem.Title;
                    Author = _selectedItem.Author;
                    Quantity = _selectedItem.Quantity;
                    SelectedCategoryId = _selectedItem.CategoryId;
                }
                else
                {
                    ResetForm();
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public SachViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>(p =>
            {
                if (string.IsNullOrEmpty(Title) || SelectedCategoryId == null)
                {
                    MessageBox.Show("Vui lòng nhập tên sách và chọn thể loại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var db = new LibraryManagementDBEntities())
                {
                    var sachMoi = new Book()
                    {
                        Title = Title,
                        Author = Author,
                        Quantity = Quantity,
                        CategoryId = SelectedCategoryId
                    };
                    db.Books.Add(sachMoi);
                    db.SaveChanges();
                    LoadData();
                    ResetForm();
                    MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 cuốn sách bên phải để sửa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(Title) || SelectedCategoryId == null)
                {
                    MessageBox.Show("Vui lòng nhập tên sách và chọn thể loại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var db = new LibraryManagementDBEntities())
                {
                    var sach = db.Books.Find(SelectedItem.BookId);
                    if (sach != null)
                    {
                        sach.Title = Title;
                        sach.Author = Author;
                        sach.Quantity = Quantity;
                        sach.CategoryId = SelectedCategoryId;
                        db.SaveChanges();
                        LoadData();
                        ResetForm();
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p => {
                if (SelectedItem == null) { MessageBox.Show("Vui lòng chọn 1 cuốn sách để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

                if (MessageBox.Show("Chắc chắn xóa cuốn sách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        bool isBorrowed = db.BorrowDetails.Any(x => x.BookId == SelectedItem.BookId);

                        if (isBorrowed)
                        {
                            MessageBox.Show("Không thể xóa! Cuốn sách này đã có lịch sử mượn/trả.\nNếu sách đã hỏng hoặc mất, vui lòng Bấm Sửa và cập nhật Số lượng về 0.",
                                            "Lỗi Ràng Buộc Dữ Liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var sach = db.Books.Find(SelectedItem.BookId);
                        db.Books.Remove(sach);
                        db.SaveChanges();

                        LoadData();
                        ResetForm();
                        MessageBox.Show("Đã xóa sách khỏi thư viện!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListTheLoai = new ObservableCollection<Category>(db.Categories.ToList());
                ListSach = new ObservableCollection<Book>(db.Books.Include("Category").ToList());
            }
        }

        private void ResetForm()
        {
            Title = "";
            Author = "";
            Quantity = null;
            SelectedCategoryId = null;
        }
    }
}