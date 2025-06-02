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
    public class LichsumuahangsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LichsumuahangsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Lichsumuahangs
        [Route("Lichsumuahang")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lichsumuahangs
                .Include(l => l.MaHdNavigation)
                .Include(l => l.MaNdNavigation)
                .Include(l => l.MaPtttNavigation);  // **Đúng tên navigation property**

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Lichsumuahangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsumuahang = await _context.Lichsumuahangs
                .Include(l => l.MaHdNavigation)
                .Include(l => l.MaNdNavigation)
                .Include(l => l.MaPtttNavigation)
                .FirstOrDefaultAsync(m => m.MaLs == id);
            if (lichsumuahang == null)
            {
                return NotFound();
            }

            return View(lichsumuahang);
        }

        // GET: Lichsumuahangs/Create
        public IActionResult Create()
        {
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd");
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd");
            return View();
        }

        // POST: Lichsumuahangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLs,MaNd,MaHd,NgayMua,TongTien,PhuongThucThanhToan,TinhTrang,GhiChu")] Lichsumuahang lichsumuahang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichsumuahang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", lichsumuahang.MaHd);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", lichsumuahang.MaNd);
            return View(lichsumuahang);
        }

        // GET: Lichsumuahangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsumuahang = await _context.Lichsumuahangs.FindAsync(id);
            if (lichsumuahang == null)
            {
                return NotFound();
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", lichsumuahang.MaHd);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", lichsumuahang.MaNd);
            return View(lichsumuahang);
        }

        // POST: Lichsumuahangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLs,MaNd,MaHd,NgayMua,TongTien,PhuongThucThanhToan,TinhTrang,GhiChu")] Lichsumuahang lichsumuahang)
        {
            if (id != lichsumuahang.MaLs)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichsumuahang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichsumuahangExists(lichsumuahang.MaLs))
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
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", lichsumuahang.MaHd);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", lichsumuahang.MaNd);
            return View(lichsumuahang);
        }

        // GET: Lichsumuahangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichsumuahang = await _context.Lichsumuahangs
                .Include(l => l.MaHdNavigation)
                .Include(l => l.MaNdNavigation)
                .FirstOrDefaultAsync(m => m.MaLs == id);
            if (lichsumuahang == null)
            {
                return NotFound();
            }

            return View(lichsumuahang);
        }

        // POST: Lichsumuahangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichsumuahang = await _context.Lichsumuahangs.FindAsync(id);
            if (lichsumuahang != null)
            {
                _context.Lichsumuahangs.Remove(lichsumuahang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichsumuahangExists(int id)
        {
            return _context.Lichsumuahangs.Any(e => e.MaLs == id);
        }
    }
}
