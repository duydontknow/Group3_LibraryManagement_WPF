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
                if (string.IsNullOrEmpty(Title))
                {
                    MessageBox.Show("Vui lòng nhập tên sách!", "Cảnh báo");
                    return;
                }
                using (var db = new LibraryManagementDBEntities())
                {
                    var sachMoi = new Book()
                    {
                        Title = Title,
                        Author = Author,
                        Quantity = Quantity
                    };
                    db.Books.Add(sachMoi);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Thêm sách thành công!");
                }
            });

            EditCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 cuốn sách bên phải để sửa!");
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
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                    }
                }
            });

            DeleteCommand = new RelayCommand<object>(p => {
                if (SelectedItem == null) { MessageBox.Show("Vui lòng chọn 1 cuốn sách để xóa!"); return; }

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
                        MessageBox.Show("Đã xóa sách khỏi thư viện!");
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListSach = new ObservableCollection<Book>(db.Books.ToList());
            }
        }
    }
}

