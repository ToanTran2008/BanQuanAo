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
    public class ThongkeDoanhthusController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ThongkeDoanhthusController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: ThongkeDoanhthus
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu doanh thu theo phương thức, tháng và năm
            var thongkeDoanhthus = await _context.ThongkeDoanhthus
                .Include(t => t.PhuongThucNavigation)
                .Where(t => t.Thang.HasValue && t.Nam.HasValue)  // Lọc dữ liệu có thời gian (tháng và năm)
                .ToListAsync();

            // Nhóm theo phương thức thanh toán, tháng và năm
            var thongKeTheoThangVaPttt = thongkeDoanhthus
                .GroupBy(t => new { t.PhuongThucNavigation.TenPttt, Thang = t.Thang.Value, Nam = t.Nam.Value })
                .Select(g => new {
                    PhuongThuc = g.Key.TenPttt,
                    Thang = g.Key.Thang,
                    Nam = g.Key.Nam,
                    DoanhThu = g.Sum(x => x.DoanhThu)
                })
                .ToList();

            var thangNamLabels = thongKeTheoThangVaPttt.Select(t => "Tháng " + t.Thang + " - Năm " + t.Nam).Distinct().ToList();
            var doanhThuData = thongKeTheoThangVaPttt
                .GroupBy(t => t.PhuongThuc)
                .Select(g => new { PhuongThuc = g.Key, Data = g.Select(t => t.DoanhThu).ToList() })
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.ThangNamLabels = thangNamLabels;
            ViewBag.DanhThuData = doanhThuData;

            // Trả về danh sách đã nhóm cho view
            return View(thongKeTheoThangVaPttt);  // Trả về danh sách đã nhóm
        }


        // GET: ThongkeDoanhthus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeDoanhthu = await _context.ThongkeDoanhthus
                .Include(t => t.PhuongThucNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongkeDoanhthu == null)
            {
                return NotFound();
            }

            return View(thongkeDoanhthu);
        }

        // GET: ThongkeDoanhthus/Create
        public IActionResult Create()
        {
            ViewData["PhuongThuc"] = new SelectList(_context.Phuongthucthanhtoans, "MaPttt", "MaPttt");
            return View();
        }

        // POST: ThongkeDoanhthus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nam,Thang,DoanhThu,PhuongThuc")] ThongkeDoanhthu thongkeDoanhthu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongkeDoanhthu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhuongThuc"] = new SelectList(_context.Phuongthucthanhtoans, "MaPttt", "MaPttt", thongkeDoanhthu.PhuongThuc);
            return View(thongkeDoanhthu);
        }

        // GET: ThongkeDoanhthus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeDoanhthu = await _context.ThongkeDoanhthus.FindAsync(id);
            if (thongkeDoanhthu == null)
            {
                return NotFound();
            }
            ViewData["PhuongThuc"] = new SelectList(_context.Phuongthucthanhtoans, "MaPttt", "MaPttt", thongkeDoanhthu.PhuongThuc);
            return View(thongkeDoanhthu);
        }

        // POST: ThongkeDoanhthus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nam,Thang,DoanhThu,PhuongThuc")] ThongkeDoanhthu thongkeDoanhthu)
        {
            if (id != thongkeDoanhthu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongkeDoanhthu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongkeDoanhthuExists(thongkeDoanhthu.Id))
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
            ViewData["PhuongThuc"] = new SelectList(_context.Phuongthucthanhtoans, "MaPttt", "MaPttt", thongkeDoanhthu.PhuongThuc);
            return View(thongkeDoanhthu);
        }

        // GET: ThongkeDoanhthus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeDoanhthu = await _context.ThongkeDoanhthus
                .Include(t => t.PhuongThucNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thongkeDoanhthu == null)
            {
                return NotFound();
            }

            return View(thongkeDoanhthu);
        }

        // POST: ThongkeDoanhthus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thongkeDoanhthu = await _context.ThongkeDoanhthus.FindAsync(id);
            if (thongkeDoanhthu != null)
            {
                _context.ThongkeDoanhthus.Remove(thongkeDoanhthu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongkeDoanhthuExists(int id)
        {
            return _context.ThongkeDoanhthus.Any(e => e.Id == id);
        }
    }
}
