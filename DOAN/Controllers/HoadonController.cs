using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOAN.Data;
using DOAN.Models;

namespace DOAN.Controllers
{
    public class HoadonsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HoadonsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Hoadons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hoadons.Include(h => h.MaNdNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Hoadons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.MaNdNavigation)  // Lấy thông tin người dùng
                .Include(h => h.Cthoadons)       // Lấy các mặt hàng trong hóa đơn
                .ThenInclude(ct => ct.MaMhNavigation) // Lấy thông tin mặt hàng
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        // GET: Hoadons/Create
        public IActionResult Create()
        {
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd");
            return View();
        }

        // POST: Hoadons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,Ngay,TongTien,MaNd,TrangThai")] Hoadon hoadon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", hoadon.MaNd);
            return View(hoadon);
        }

        // GET: Hoadons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái hiện tại và chỉ gán các giá trị hợp lệ
            if (hoadon.TrangThai == "Chờ xác nhận")
            {
                hoadon.TrangThai = "Đang giao"; // hoặc trạng thái khác tuỳ theo yêu cầu
            }
            else if (hoadon.TrangThai == "Đang giao")
            {
                hoadon.TrangThai = "Hoàn thành";
            }
            // Có thể thêm các điều kiện khác nếu cần

            _context.Update(hoadon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: Hoadons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,Ngay,TongTien,MaNd,TrangThai")] Hoadon hoadon)
        {
            if (id != hoadon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoadonExists(hoadon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", hoadon.MaNd);
            return View(hoadon);
        }

        private bool HoadonExists(int id)
        {
            return _context.Hoadons.Any(e => e.MaHd == id);
        }
    }
}
