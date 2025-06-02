using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOAN.Data;
using DOAN.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;


namespace DOAN.Controllers
{

    public class CustomerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Nguoidung> _passwordHasher;
        private readonly MailSettings _mailSettings;

        public CustomerController(ApplicationDbContext context, IPasswordHasher<Nguoidung> passwordHasher, IOptions<MailSettings> mailSettings) : base(context)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            // Lấy thông tin cấu hình từ appsettings.json
            _mailSettings = mailSettings.Value; // Đảm bảo mailSettings được truyền vào đúng cách
        }

        /*
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        } */

        // GET: Customer
        public IActionResult Index1()
        {
            var mathangs = _context.Mathangs.ToList(); // Lấy danh sách sản phẩm
            return View(mathangs);
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mathangs
                .Include(m => m.MaDmNavigation) // Load danh mục
                .Include(m => m.MaKms) // Load danh sách khuyến mãi
                .ThenInclude(mk => mk.MaMhs); // Load thông tin khuyến mãi


            // Tạo dictionary lưu điểm trung bình cho từng sản phẩm
            var danhgiaDict = await _context.Danhgia
                .GroupBy(d => d.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    AverageRating = g.Average(d => d.Rating)
                })
                .ToDictionaryAsync(d => d.MaMh, d => d.AverageRating);

            // Truyền thông tin đánh giá trung bình xuống ViewBag
            ViewBag.AverageRatings = danhgiaDict;

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Tạo dictionary lưu điểm trung bình cho từng sản phẩm
            var danhgiaDict = await _context.Danhgia
                .GroupBy(d => d.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    AverageRating = g.Average(d => d.Rating)
                })
                .ToDictionaryAsync(d => d.MaMh, d => d.AverageRating);

            // Truyền thông tin đánh giá trung bình xuống ViewBag
            ViewBag.AverageRatings = danhgiaDict;

            var mathang = await _context.Mathangs
                .Include(m => m.MaDmNavigation)
                .Include(m => m.Danhgia).ThenInclude(d => d.MaNdNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            //Cập nhật lượt xem
            mathang.LuotXem += 1;

            _context.Mathangs.Update(mathang);
            await _context.SaveChangesAsync();

            GetData();
            return View(mathang);
        }


        private bool MathangExists(int id)
        {
            return _context.Mathangs.Any(e => e.MaMh == id);
        }

        // Các phương thức xử lý giỏ hàng
        // Đọc danh sách CartItem từ session
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString("shopcart");
            if (jsoncart != null)
            {
                //return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                return cartItems ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        // Lưu danh sách CartItem trong giỏ hàng vào session
        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Bỏ qua vòng tham chiếu
            };
            string jsoncart = JsonConvert.SerializeObject(list, settings);
            session.SetString("shopcart", jsoncart);

            // Kiểm tra dữ liệu sau khi lưu
            Console.WriteLine($"Giỏ hàng lưu vào Session: {jsoncart}");
        }


        //xóa session giỏ hàng
        public void ClearCart()
        {
            HttpContext.Session.Remove("shopcart");
            Console.WriteLine("Giỏ hàng đã được xóa khỏi Session");
        }


        // Cho hàng vào giỏ 
        public async Task<IActionResult> AddToCart(int id)
        {

            var mathang = await _context.Mathangs
                  .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                cart.Add(new CartItem() { MatHang = mathang, SoLuong = 1 });
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        // Chuyển đến view xem giỏ hàng 
        public IActionResult ViewCart()
        {
            GetData();
            return View(GetCartItems());
        }


        //Xóa một mặt hàng khỏi giỏ
        public IActionResult RemoveItem(int id)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }
        // Cật nhật số lượng một mặt hàng trong giỏ
        public IActionResult UpdateItem(int id, int quantity)
        {
            var mathang = _context.Mathangs.FirstOrDefault(p => p.MaMh == id);
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                // Kiểm tra số lượng yêu cầu với số lượng tồn kho
                if (quantity > mathang.SoLuong)
                {
                    // Thông báo lỗi nếu số lượng yêu cầu vượt quá số lượng tồn kho
                    TempData["ErrorMessage"] = $"Số lượng không đủ! Chỉ còn {mathang.SoLuong} sản phẩm trong kho.";
                }
                else
                {
                    item.SoLuong = quantity;
                    SaveCartSession(cart);
                }
            }

            return RedirectToAction(nameof(ViewCart));
        }

        //Chuyển đến view thanh thanh toán
        public IActionResult CheckOut()
        {
            GetData();

            return View(GetCartItems());
        }

        [HttpPost, ActionName("CreateBill")]
        public async Task<IActionResult> CreateBill(string email, string ten, string dienthoai, int MaPttt)
        {
            // Kiểm tra xem người dùng đã có trong hệ thống chưa
            var nd = await _context.Nguoidungs.FirstOrDefaultAsync(n => n.Email == email);

            if (nd == null)
            {
                // Nếu người dùng không tồn tại, tạo mới người dùng
                nd = new Nguoidung()
                {
                    Email = email,
                    Ten = ten,
                    DienThoai = dienthoai
                };
                _context.Add(nd);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Nếu người dùng đã tồn tại, cập nhật thông tin nếu cần thiết
                nd.Ten = ten ?? nd.Ten;
                nd.DienThoai = dienthoai ?? nd.DienThoai;
                _context.Update(nd);
                await _context.SaveChangesAsync();
            }

            // Tạo hóa đơn mới
            var hd = new Hoadon();
            hd.Ngay = DateTime.Now;
            hd.MaNd = nd.MaNd;
            hd.MaPttt = MaPttt;  // Lưu phương thức thanh toán vào hóa đơn

            _context.Add(hd);
            await _context.SaveChangesAsync();

            // Thêm chi tiết hóa đơn
            var cart = GetCartItems();
            int thanhtien = 0;
            int tongtien = 0;  // Biến lưu tổng tiền

            foreach (var i in cart)
            {
                var ct = new Cthoadon();
                ct.MaHd = hd.MaHd;
                ct.MaMh = i.MatHang.MaMh;
                 thanhtien = i.MatHang.GiaBan * i.SoLuong ?? 1;
                tongtien += thanhtien;
                ct.DonGia = i.MatHang.GiaBan;
                ct.SoLuong = (short)i.SoLuong;
                ct.ThanhTien = thanhtien;
                _context.Add(ct);

                // Cập nhật thông tin mặt hàng trong kho
                var mh = await _context.Mathangs.FirstOrDefaultAsync(m => m.MaMh == i.MatHang.MaMh);
                mh.SoLuong -= i.SoLuong; // Giảm số lượng trong kho
                mh.LuotMua += 1; // Tăng số lượt mua
                _context.Update(mh);
                await _context.SaveChangesAsync();
               

                var BH = new ThongkeMathangBan
                {
                    MaMh = i.MatHang.MaMh,
                    SoLuongBan = (short)(i.SoLuong),
                    ThoiGian = DateTime.Now // Gán giá trị thời gian

                };

                try
                {
                    _context.ThongkeMathangBans.Add(BH);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log hoặc hiển thị lỗi nếu có
                    Console.WriteLine(ex.Message);
                }


            }

            // Cập nhật tổng tiền hóa đơn
            hd.TongTien = tongtien;
            _context.Update(hd);
            await _context.SaveChangesAsync();
            // Gửi email xác nhận
            try
            {
                await SendConfirmationEmail(email, ten, hd.MaHd, tongtien);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
                // Optionally: hiển thị thông báo nhẹ nhàng cho người dùng
            }

            //Thống kê doanh thu
            var doanhthu = new ThongkeDoanhthu
            {
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                DoanhThu = tongtien,
                PhuongThuc = MaPttt //  
            };
            try
            {
                _context.ThongkeDoanhthus.Add(doanhthu);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi thống kê doanh thu: " + ex.Message);
            }



            // Xử lý phương thức thanh toán
            var phuongThucThanhToan = await _context.Phuongthucthanhtoans
                .FirstOrDefaultAsync(pttt => pttt.MaPttt == MaPttt);  // Tìm phương thức thanh toán theo MaPttt

            if (phuongThucThanhToan != null)
            {
                // Lưu lịch sử mua hàng với phương thức thanh toán
                var lichSuMuaHang = new Lichsumuahang
                {
                    MaNd = nd.MaNd,
                    MaHd = hd.MaHd,
                    NgayMua = DateTime.Now,
                    TongTien = tongtien,
                    MaPttt = phuongThucThanhToan.MaPttt,  // Lưu phương thức thanh toán vào lịch sử mua hàng
                    TinhTrang = "Chờ xác nhận"
                };

                _context.Add(lichSuMuaHang);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Phương thức thanh toán không hợp lệ
                Console.WriteLine("Không tìm thấy phương thức thanh toán!");
            }
            // Gọi blockchain để lưu
            try
            {
                var blockchain = new EthereumService();
                string txHash = await blockchain.AddOrderToBlockchain(email, tongtien, phuongThucThanhToan.TenPttt);
                Console.WriteLine("Giao dịch blockchain hash: " + txHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu đơn hàng lên blockchain: " + ex.Message);
            }


            // Xóa giỏ hàng và lấy dữ liệu lại
            ClearCart();
            GetData();

            return View(hd);
        }


        //gọi hàm thống báo email
        private async Task SendConfirmationEmail(string toEmail, string ten, int mahd, int tongtien)
        {
            // Kiểm tra nếu tên khách hàng là null hoặc rỗng
            ten = string.IsNullOrEmpty(ten) ? "Khách hàng" : ten;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mailSettings.Address)); // Địa chỉ gửi
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Xác nhận đơn hàng";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
        <h3>Xin chào {ten},</h3>
        <p>Bạn đã đặt hàng thành công đơn hàng có mã số <strong>{mahd}</strong>.</p>
        <p>Tổng giá trị: <strong>{tongtien:N0}đ</strong>.</p>
        <p>Chúng tôi sẽ sớm liên hệ bạn để xác nhận và giao hàng.</p>
        <br/>
        <p>Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi.</p>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Address, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }



        void GetData()
        {
            ViewData["soluong"] = GetCartItems().Count;
            ViewBag.danhmuc = _context.Danhmucs.ToList();
            var cartItems = GetCartItems();
            ViewData["solg"] = cartItems != null ? cartItems.Count : 0;
            var cuahang = _context.Cuahangs.FirstOrDefault(k => k.MaCh == 1);
            ViewBag.cuahang = cuahang;

            // Lấy thông tin người dùng từ session
            var email = HttpContext.Session.GetString("nguoidung");
            if (!string.IsNullOrEmpty(email))
            {
                var nguoidung = _context.Nguoidungs.FirstOrDefault(k => k.Email == email);
                ViewBag.nguoidung = nguoidung;
            }
            else
            {
                ViewBag.nguoidung = null;
            }
        }

          

        //Danh sách sản phẩm theo danh mục
        public async Task<IActionResult> List(int id)
        {
            // Tạo dictionary lưu điểm trung bình cho từng sản phẩm
            var danhgiaDict = await _context.Danhgia
                .GroupBy(d => d.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    AverageRating = g.Average(d => d.Rating)
                })
                .ToDictionaryAsync(d => d.MaMh, d => d.AverageRating);

            // Truyền thông tin đánh giá trung bình xuống ViewBag
            ViewBag.AverageRatings = danhgiaDict;

            var applicationDBContext = _context.Mathangs.Where(m => m.MaDm == id).Include(m => m.MaDmNavigation);
            GetData();
            ViewData["tendanhmuc"] = _context.Danhmucs.FirstOrDefault(d => d.MaDm == id).Ten;
            return View(await applicationDBContext.ToListAsync());
        }


        //Phương thức form đăng ký
        public IActionResult Register(int id)
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string hoten, string dienthoai, string email, string matkhau, string xnmatkhau)
        {
            // Kiểm tra xác nhận mật khẩu
            if (matkhau != xnmatkhau)
            {
                ViewBag.ErrorMessage = "Xác nhận mật khẩu không đúng.";
                ViewBag.Hoten = hoten;
                ViewBag.DienThoai = dienthoai;
                ViewBag.Email = email;
                return View();
            }

            // Kiểm tra nếu email đã tồn tại
            if (_context.Nguoidungs.Any(u => u.Email == email))
            {
                ViewBag.ErrorMessage = "Email này đã được sử dụng.";
                ViewBag.Hoten = hoten;
                ViewBag.DienThoai = dienthoai;
                ViewBag.Email = email;
                return View();
            }

            // Tạo người dùng mới
            Nguoidung nd = new Nguoidung
            {
                Ten = hoten,
                DienThoai = dienthoai,
                Email = email,
                MatKhau = _passwordHasher.HashPassword(null, matkhau)
            };

            if (ModelState.IsValid)
            {
                _context.Add(nd);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            // Nếu có lỗi khác, trả lại view
            ViewBag.ErrorMessage = "Đã xảy ra lỗi. Vui lòng thử lại.";
            return View();
        }

        //Phương thức form đăng nhập

        public IActionResult Login()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string matkhau)
        {
            var nd = await _context.Nguoidungs.FirstOrDefaultAsync(m => m.Email == email);
            if (nd != null && _passwordHasher.VerifyHashedPassword(nd, nd.MatKhau, matkhau) == PasswordVerificationResult.Success)
            {
                // Đăng nhập thành công, thực hiện các hành động cần thiết
                // Ví dụ: Ghi thông tin người dùng vào Session
                HttpContext.Session.SetString("nguoidung", nd.Email);
                HttpContext.Session.SetInt32("MaND", nd.MaNd);
                return RedirectToAction(nameof(CustomerInfo));
                /*//Liên kết giỏ hàng với người dùng
                var sessionCart = GetCartItems();
                if (sessionCart.Any())*/



            }
            ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng.";
            GetData();
            return View();
        }

        public IActionResult CustomerInfo()
        {
            GetData();
            return View();
        }

        //Đăng xuất
        public IActionResult Signout()
        {
            HttpContext.Session.SetString("nguoidung", "");
            GetData();
            return RedirectToAction("Index");
        }

         //Tìm kiếm
        public async Task<IActionResult> Search(string searchTerm)
        {
            // Tạo dictionary lưu điểm trung bình cho từng sản phẩm
            var danhgiaDict = await _context.Danhgia
                .GroupBy(d => d.MaMh)
                .Select(g => new
                {
                    MaMh = g.Key,
                    AverageRating = g.Average(d => d.Rating)
                })
                .ToDictionaryAsync(d => d.MaMh, d => d.AverageRating);

            // Truyền thông tin đánh giá trung bình xuống ViewBag
            ViewBag.AverageRatings = danhgiaDict;

            GetData();
            var applicationDBContext = _context.Mathangs.Where(mh => mh.Ten.Contains(searchTerm) || mh.MoTa.Contains(searchTerm)).Include(mh => mh.MaDmNavigation);

            ViewBag.timkiem = searchTerm;

            return View(await applicationDBContext.ToListAsync());
        }

        public IActionResult EditUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string ten, string sdt, string matkhau)
        {

            var email = HttpContext.Session.GetString("nguoidung");
            var nguoidung = await _context.Nguoidungs.FirstOrDefaultAsync(nd => nd.Email == email);
            if (nguoidung != null)
            {
                nguoidung.Ten = ten;
                nguoidung.DienThoai = sdt;

                if (matkhau != null)
                {
                    var mhmakh = _passwordHasher.HashPassword(null, matkhau);
                    nguoidung.MatKhau = mhmakh;
                }

                _context.Update(nguoidung);

            }

            await _context.SaveChangesAsync();

            GetData();
            return RedirectToAction("CustomerInfo");
        }

        //Chỉnh sửa 1 địa chỉ
        public IActionResult EditAddressItem(int id)
        {
            var diaChi = _context.Diachis.FirstOrDefault(d => d.MaDc == id);
            if (diaChi == null)
            {
                return NotFound();
            }

            GetData();
            return View(diaChi);
        }
        [HttpPost]
        public async Task<IActionResult> EditAddressItem(int id, string chitiet, string phuongxa, string quanhuyen, string tinhthanh)
        {
            // Lấy địa chỉ dựa trên ID
            var diaChi = await _context.Diachis.FirstOrDefaultAsync(dc => dc.MaDc == id);
            if (diaChi != null)
            {
                // Cập nhật thông tin
                diaChi.ChiTiet = chitiet;
                diaChi.PhuongXa = phuongxa;
                diaChi.QuanHuyen = quanhuyen;
                diaChi.TinhThanh = tinhthanh;

                _context.Update(diaChi);
            }

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Trả về trang thông tin khách hàng
            return RedirectToAction("CustomerInfo");
        }

        //Xóa địa chỉ
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var diachi = await _context.Diachis.FindAsync(id);
            if (diachi == null)
            {
                return NotFound();
            }

            _context.Diachis.Remove(diachi);
            await _context.SaveChangesAsync();
            return RedirectToAction("CustomerInfo");
        }

        public IActionResult AddAddress()
        {
            GetData();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddAddress(string chitiet, string phuongxa, string quanhuyen, string tinhthanh)
        {
            var nguoidungId = HttpContext.Session.GetInt32("MaND");

            var dc = new Diachi()
            {
                MaNd = nguoidungId.Value,
                ChiTiet = chitiet,
                PhuongXa = phuongxa,
                QuanHuyen = quanhuyen,
                TinhThanh = tinhthanh
            };

            _context.Add(dc);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View();
            }

            return RedirectToAction("CustomerInfo");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitReview(int maMh, int maND, int sao, string binhluan)
        {
            if (maMh <= 0 || sao < 1 || sao > 5 || string.IsNullOrWhiteSpace(binhluan))
            {
                TempData["ReviewError"] = "Thông tin đánh giá không hợp lệ.";
                return RedirectToAction("Details", new { id = maMh });
            }

            try
            {
                // Tạo đánh giá mới
                var danhgia = new Danhgium
                {
                    MaMh = maMh,
                    MaNd = maND,
                    Rating = sao,
                    BinhLuan = binhluan,
                    NgayDanhGia = DateTime.Now
                };

                // Thêm vào cơ sở dữ liệu
                _context.Danhgia.Add(danhgia);
                await _context.SaveChangesAsync();

                TempData["ReviewSuccess"] = "Đánh giá thành công!";
            }
            catch (Exception ex)
            {
                TempData["ReviewError"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Details", new { id = maMh });
        }

        public IActionResult LHIndex()
        {
            var nguoidungId = HttpContext.Session.GetInt32("MaND");
            GetData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LHIndex(string ten, string email, string dienthoai, string NoiDung)
        {

            // Xử lý dữ liệu nhận từ form
            if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(dienthoai) || string.IsNullOrEmpty(NoiDung))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
                return View();
            }

            // Ví dụ: Lưu vào database (giả lập)
            await Task.Run(() =>
            {
                Console.WriteLine($"Tên: {ten}, Email: {email}, Điện thoại: {dienthoai}");
            });

            TempData["Success"] = "Thông tin đã được gửi thành công!";
            return RedirectToAction("LHIndex");
        }


    }
}

