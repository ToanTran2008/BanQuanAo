using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOAN.Data;
using DOAN.Models;
using NuGet.Packaging;

namespace DOAN.Controllers
{
    public class KhuyenmaisController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public KhuyenmaisController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public DbSet<Khuyenmai> Khuyenmais { get; set; }
        public DbSet<Mathang> MatHangs { get; set; }  

        // GET: Khuyenmais
        public async Task<IActionResult> Index()
        {
            var danhSachKhuyenMai = await _context.Khuyenmais
        .Include(km => km.MaMhs) // Load danh sách mặt hàng áp dụng
        .ToListAsync();

            return View(danhSachKhuyenMai);
        }

        // GET: Khuyenmais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenmai = await _context.Khuyenmais
                .FirstOrDefaultAsync(m => m.MaKm == id);
            if (khuyenmai == null)
            {
                return NotFound();
            }

            return View(khuyenmai);
        }

        // GET: Khuyenmais/Create
        // Hiển thị form tạo khuyến mãi
      
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Mathangs = _context.Mathangs.ToList();
            return View();
        }
        // Xử lý dữ liệu khi gửi form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("MaKm,TenKm,MoTa,LoaiGiamGia,GiaTriGiam,DieuKienApDung,NgayBatDau,NgayKetThuc,TrangThai")] Khuyenmai khuyenmai, List<int> MatHangIds)
        {
            if (ModelState.IsValid)
            {
                if (khuyenmai.LoaiGiamGia != "PhanTram" && khuyenmai.LoaiGiamGia != "TienMat")
                {
                    ModelState.AddModelError("LoaiGiamGia", "Loại giảm giá không hợp lệ!");
                    return View(khuyenmai);
                }

                // Ràng buộc logic dựa trên loại giảm giá
                if (khuyenmai.LoaiGiamGia == "PhanTram")
                {
                    if (khuyenmai.GiaTriGiam < 1 || khuyenmai.GiaTriGiam > 100)
                    {
                        ModelState.AddModelError("GiaTriGiam", "Giá trị giảm theo phần trăm phải từ 1% đến 100%!");
                        return View(khuyenmai);
                    }
                }
                else if (khuyenmai.LoaiGiamGia == "TienMat")
                {
                    if (khuyenmai.DieuKienApDung > 0 && khuyenmai.GiaTriGiam > khuyenmai.DieuKienApDung)
                    {
                        ModelState.AddModelError("DieuKienApDung", "Điều kiện áp dụng phải lớn hơn hoặc bằng giá trị giảm!");
                        return View(khuyenmai);
                    }
                }

                _context.Khuyenmais.Add(khuyenmai);
                await _context.SaveChangesAsync(); // Lưu trước để có `MaKm`

                if (MatHangIds != null && MatHangIds.Any())
                {
                    if (khuyenmai.MaMhs == null)
                    {
                        khuyenmai.MaMhs = new List<Mathang>(); // Đảm bảo danh sách không bị null
                    }

                    foreach (var maMh in MatHangIds)
                    {
                        var mathang = await _context.Mathangs.FindAsync(maMh);
                        if (mathang != null)
                        {
                            khuyenmai.MaMhs.Add(mathang);
                            Console.WriteLine($"Thêm mặt hàng: {mathang.Ten} vào khuyến mãi {khuyenmai.TenKm}");
                        }
                    }
                    await _context.SaveChangesAsync(); // Lưu lại quan hệ Many-to-Many
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Mathangs = await _context.Mathangs.ToListAsync();
            return View(khuyenmai);
        }



        // GET: Khuyenmais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var khuyenMai = _context.Khuyenmais
        .Include(km => km.MaMhs) // Giả sử có quan hệ với mặt hàng
        .FirstOrDefault(km => km.MaKm == id);

            if (id == null)
            {
                return NotFound();
            }

            var khuyenmai = await _context.Khuyenmais.FindAsync(id);
            if (khuyenmai == null)
            {
                return NotFound();
            }

            ViewBag.Mathangs = _context.Mathangs.ToList(); // Lấy toàn bộ mặt hàng
            ViewBag.SelectedMatHangs = khuyenMai.MaMhs.Select(mh => mh.MaMh).ToList(); // Lấy danh sách ID mặt hàng đã chọn

            return View(khuyenmai);
        }

        // POST: Khuyenmais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
       
        public IActionResult Edit(Khuyenmai model, List<int> MatHangIds)
        {
            var khuyenMai = _context.Khuyenmais
                .Include(km => km.MaMhs) // Load danh sách mặt hàng
                .FirstOrDefault(km => km.MaKm == model.MaKm);

            if (khuyenMai == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin khuyến mãi
            khuyenMai.TenKm = model.TenKm;
            khuyenMai.MoTa = model.MoTa;
            khuyenMai.LoaiGiamGia = model.LoaiGiamGia;
            khuyenMai.GiaTriGiam = model.GiaTriGiam;
            khuyenMai.DieuKienApDung = model.DieuKienApDung;
            khuyenMai.NgayBatDau = model.NgayBatDau;
            khuyenMai.NgayKetThuc = model.NgayKetThuc;
            khuyenMai.TrangThai = model.TrangThai;

            // Cập nhật danh sách mặt hàng
            if (MatHangIds != null)
            {
                var selectedMatHangs = _context.Mathangs
                    .Where(mh => MatHangIds.Contains(mh.MaMh))
                    .ToList();

                khuyenMai.MaMhs.Clear(); // Xóa danh sách cũ
                khuyenMai.MaMhs.AddRange(selectedMatHangs); // Thêm mới
            }

            try
            {
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return BadRequest("Lỗi khi lưu dữ liệu: " + ex.InnerException?.Message);
            }
        }


        // GET: Khuyenmais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenmai = await _context.Khuyenmais
                .FirstOrDefaultAsync(m => m.MaKm == id);
            if (khuyenmai == null)
            {
                return NotFound();
            }

            return View(khuyenmai);
        }

        // POST: Khuyenmais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khuyenmai = await _context.Khuyenmais.FindAsync(id);
            if (khuyenmai != null)
            {
                _context.Khuyenmais.Remove(khuyenmai);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhuyenmaiExists(int id)
        {
            return _context.Khuyenmais.Any(e => e.MaKm == id);
        }
    }
}
