using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System;
using System.Collections.ObjectModel; // Chỉ giữ lại dòng này thôi
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class PhieuMuonViewModel : BaseViewModel
    {
        private ObservableCollection<BorrowDetail> _listPhieuMuon;
        public ObservableCollection<BorrowDetail> ListPhieuMuon
        {
            get => _listPhieuMuon;
            set { _listPhieuMuon = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Reader> _listDocGia;
        public ObservableCollection<Reader> ListDocGia
        {
            get => _listDocGia;
            set { _listDocGia = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Book> _listSach;
        public ObservableCollection<Book> ListSach
        {
            get => _listSach;
            set { _listSach = value; OnPropertyChanged(); }
        }

        private Reader _selectedReader;
        public Reader SelectedReader
        {
            get => _selectedReader;
            set { _selectedReader = value; OnPropertyChanged(); }
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        private DateTime _dueDate = DateTime.Now.AddDays(7);
        public DateTime DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(); }
        }

        private BorrowDetail _selectedItem;
        public BorrowDetail SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ICommand BorrowCommand { get; set; }
        public ICommand ReturnCommand { get; set; }

        public PhieuMuonViewModel()
        {
            LoadData();

            BorrowCommand = new RelayCommand<object>(p =>
            {
                if (SelectedReader == null || SelectedBook == null)
                {
                    MessageBox.Show("Vui lòng chọn Độc giả và Sách muốn mượn!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new LibraryManagementDBEntities())
                {
                    var bookInDb = db.Books.Find(SelectedBook.BookId);
                    if (bookInDb == null || bookInDb.Quantity <= 0)
                    {
                        MessageBox.Show("Sách này đã hết trong kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    bookInDb.Quantity -= 1;

                    var record = new BorrowRecord
                    {
                        ReaderId = SelectedReader.ReaderId,
                        BorrowDate = DateTime.Now,
                        Status = "Đang mượn"
                    };
                    db.BorrowRecords.Add(record);

                    var detail = new BorrowDetail
                    {
                        BorrowRecord = record,
                        BookId = bookInDb.BookId,
                        DueDate = DueDate,
                        ReturnDate = null
                    };
                    db.BorrowDetails.Add(detail);

                    db.SaveChanges();

                    LoadData();
                    MessageBox.Show("Đã lập phiếu mượn và xuất kho thành công!", "Thông báo");
                }
            });

            ReturnCommand = new RelayCommand<object>(p =>
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 dòng phiếu mượn bên phải để trả sách!");
                    return;
                }

                if (SelectedItem.ReturnDate != null)
                {
                    MessageBox.Show("Sách này đã được trả rồi, không thể trả lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show($"Xác nhận độc giả trả cuốn: {SelectedItem.Book.Title}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new LibraryManagementDBEntities())
                    {
                        var detailInDb = db.BorrowDetails.Find(SelectedItem.RecordId, SelectedItem.BookId);

                        detailInDb.ReturnDate = DateTime.Now;

                        var recordInDb = db.BorrowRecords.Find(SelectedItem.RecordId);
                        recordInDb.Status = "Đã trả";

                        var bookInDb = db.Books.Find(SelectedItem.BookId);
                        if (bookInDb != null)
                        {
                            bookInDb.Quantity += 1;
                        }

                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Đã nhận lại sách và nhập kho!", "Thông báo");
                    }
                }
            });
        }

        private void LoadData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                ListDocGia = new ObservableCollection<Reader>(db.Readers.ToList());
                ListSach = new ObservableCollection<Book>(db.Books.ToList());

                ListPhieuMuon = new ObservableCollection<BorrowDetail>(
                    db.BorrowDetails
                        .Include("BorrowRecord.Reader")
                        .Include("Book")
                        .OrderByDescending(x => x.BorrowRecord.BorrowDate)
                        .ToList()
                );
            }
        }
    }
}