using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private int _tongSoSach;
        public int TongSoSach { get => _tongSoSach; set { _tongSoSach = value; OnPropertyChanged(); } }

        private int _tongDocGia;
        public int TongDocGia { get => _tongDocGia; set { _tongDocGia = value; OnPropertyChanged(); } }

        private int _sachDangMuon;
        public int SachDangMuon { get => _sachDangMuon; set { _sachDangMuon = value; OnPropertyChanged(); } }

        public ICommand OpenSachCommand { get; set; }
        public ICommand OpenTheLoaiCommand { get; set; }
        public ICommand OpenDocGiaCommand { get; set; }
        public ICommand OpenNhanVienCommand { get; set; }
        public ICommand OpenPhieuMuonCommand { get; set; }
        public ICommand OpenThongKeCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public MainViewModel()
        {
            OpenSachCommand = new RelayCommand<object>(p => {
                MessageBox.Show("Thành viên A code chức năng Quản lý Sách!", "Thông báo");
            });

            OpenTheLoaiCommand = new RelayCommand<object>(p => {
                MessageBox.Show("Thành viên B code chức năng Quản lý Thể loại!", "Thông báo");
            });

            OpenDocGiaCommand = new RelayCommand<object>(p => {
                MessageBox.Show("Thành viên C code chức năng Quản lý Độc giả!", "Thông báo");
            });

            OpenNhanVienCommand = new RelayCommand<object>(p => {
                MessageBox.Show("Thành viên D code chức năng Quản lý Nhân viên!", "Thông báo");
            });

            OpenPhieuMuonCommand = new RelayCommand<object>(p => {
                MessageBox.Show("Thành viên E code chức năng Quản lý Phiếu mượn!", "Thông báo");
            });

            OpenThongKeCommand = new RelayCommand<object>(p => {
                MessageBox.Show("(F) sẽ làm Form Thống kê chi tiết ở đây sau!", "Thông báo");
            });

            LogoutCommand = new RelayCommand<Window>(p => {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            });

            LoadThongKeData();
        }

        private void LoadThongKeData()
        {
            using (var db = new LibraryManagementDBEntities())
            {
                TongSoSach = db.Books.Count();
                TongDocGia = db.Readers.Count();
                SachDangMuon = db.BorrowRecords.Count(x => x.Status == "Đang mượn");
            }
        }
    }
}