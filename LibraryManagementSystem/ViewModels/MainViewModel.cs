using LibraryManagementSystem.Core;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Views;
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

        private Visibility _adminVisibility;
        public Visibility AdminVisibility
        {
            get => _adminVisibility;
            set { _adminVisibility = value; OnPropertyChanged(); }
        }

        public ICommand OpenSachCommand { get; set; }
        public ICommand OpenTheLoaiCommand { get; set; }
        public ICommand OpenDocGiaCommand { get; set; }
        public ICommand OpenNhanVienCommand { get; set; }
        public ICommand OpenPhieuMuonCommand { get; set; }
        public ICommand OpenThongKeCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public MainViewModel()
        {

            AdminVisibility = (UserSession.RoleId == 1) ? Visibility.Visible : Visibility.Collapsed;

            OpenSachCommand = new RelayCommand<object>(p => {
                SachWindow win = new SachWindow();
                win.ShowDialog();
            });

            OpenTheLoaiCommand = new RelayCommand<object>(p => {
                TheLoaiWindow win = new TheLoaiWindow();
                win.ShowDialog();
            });

            OpenDocGiaCommand = new RelayCommand<object>(p => {
                DocGiaWindow win = new DocGiaWindow();
                win.ShowDialog();
            });

            OpenNhanVienCommand = new RelayCommand<object>(p => {
                NhanVienWindow win = new NhanVienWindow();
                win.ShowDialog();
            });

            OpenPhieuMuonCommand = new RelayCommand<object>(p => {
                PhieuMuonWindow win = new PhieuMuonWindow();
                win.ShowDialog();
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