
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOAN.Data;
using DOAN.Models;
using System.Globalization;

namespace DOAN.Controllers
{
    public class ThongkeMathangBansController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ThongkeMathangBansController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: ThongkeMathangBans
        public IActionResult Index()
        {
            var list = _context.ThongkeMathangBans
                .Include(t => t.MaMhNavigation)
                .Select(t => new {
                    Ten = t.MaMhNavigation.Ten,
                    SoLuongDaBan = t.SoLuongBan,
                    ThoiGian = t.ThoiGian
                })
                .ToList();

            // Lọc các bản ghi có ThoiGian không null
            var thongKeTheoThang = list
                .Where(t => t.ThoiGian.HasValue) // Chỉ lấy những bản ghi có ThoiGian khác null
                .GroupBy(t => t.ThoiGian.Value.Month)
                .Select(g => new {
                    Thang = g.Key,
                    TongSoLuongBan = g.Sum(t => t.SoLuongDaBan)
                })
                .ToList();

            // Chuyển đổi dữ liệu thành danh sách (string cho labels, int cho data)
            var thangLabels = thongKeTheoThang.Select(t => "Tháng " + t.Thang).ToList();
            var thangData = thongKeTheoThang.Select(t => t.TongSoLuongBan).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.ThangLabels = thangLabels;
            ViewBag.ThangData = thangData;

            return View(list);
        }



        // GET: ThongkeMathangBans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeMathangBan = await _context.ThongkeMathangBans
                .Include(t => t.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (thongkeMathangBan == null)
            {
                return NotFound();
            }

            return View(thongkeMathangBan);
        }

        // GET: ThongkeMathangBans/Create
        public IActionResult Create()
        {
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh");
            return View();
        }

        // POST: ThongkeMathangBans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTk,MaMh,SoLuongBan,ThoiGian")] ThongkeMathangBan thongkeMathangBan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongkeMathangBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", thongkeMathangBan.MaMh);
            return View(thongkeMathangBan);
        }

        // GET: ThongkeMathangBans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeMathangBan = await _context.ThongkeMathangBans.FindAsync(id);
            if (thongkeMathangBan == null)
            {
                return NotFound();
            }
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", thongkeMathangBan.MaMh);
            return View(thongkeMathangBan);
        }

        // POST: ThongkeMathangBans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTk,MaMh,SoLuongBan,ThoiGian")] ThongkeMathangBan thongkeMathangBan)
        {
            if (id != thongkeMathangBan.MaTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongkeMathangBan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongkeMathangBanExists(thongkeMathangBan.MaTk))
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
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", thongkeMathangBan.MaMh);
            return View(thongkeMathangBan);
        }

        // GET: ThongkeMathangBans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongkeMathangBan = await _context.ThongkeMathangBans
                .Include(t => t.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (thongkeMathangBan == null)
            {
                return NotFound();
            }

            return View(thongkeMathangBan);
        }

        // POST: ThongkeMathangBans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thongkeMathangBan = await _context.ThongkeMathangBans.FindAsync(id);
            if (thongkeMathangBan != null)
            {
                _context.ThongkeMathangBans.Remove(thongkeMathangBan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongkeMathangBanExists(int id)
        {
            return _context.ThongkeMathangBans.Any(e => e.MaTk == id);
        }
    }
}
