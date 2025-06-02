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
    [Route("Danhgiums")]
    public class DanhgiumController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DanhgiumController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Danhgium
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Danhgia
            .Include(d => d.MaMhNavigation)
            .Include(d => d.MaNdNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Danhgium/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhgium = await _context.Danhgia
                .Include(d => d.MaMhNavigation)
                .Include(d => d.MaNdNavigation)
                .FirstOrDefaultAsync(m => m.MaDg == id);
            if (danhgium == null)
            {
                return NotFound();
            }

            return View(danhgium);
        }

        // GET: Danhgium/Create
        public IActionResult Create()
        {
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh");
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd");
            return View();
        }

        // POST: Danhgium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDg,MaMh,MaNd,Rating,BinhLuan,NgayDanhGia")] Danhgium danhgium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhgium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", danhgium.MaMh);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", danhgium.MaNd);
            return View(danhgium);
        }

        // GET: Danhgium/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhgium = await _context.Danhgia.FindAsync(id);
            if (danhgium == null)
            {
                return NotFound();
            }
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", danhgium.MaMh);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", danhgium.MaNd);
            return View(danhgium);
        }

        // POST: Danhgium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDg,MaMh,MaNd,Rating,BinhLuan,NgayDanhGia")] Danhgium danhgium)
        {
            if (id != danhgium.MaDg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhgium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhgiumExists(danhgium.MaDg))
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
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", danhgium.MaMh);
            ViewData["MaNd"] = new SelectList(_context.Nguoidungs, "MaNd", "MaNd", danhgium.MaNd);
            return View(danhgium);
        }

        // GET: Danhgium/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhgium = await _context.Danhgia
                .Include(d => d.MaMhNavigation)
                .Include(d => d.MaNdNavigation)
                .FirstOrDefaultAsync(m => m.MaDg == id);
            if (danhgium == null)
            {
                return NotFound();
            }

            return View(danhgium);
        }

        // POST: Danhgium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhgia = await _context.Danhgia.FindAsync(id);
            if (danhgia != null)
            {
                _context.Danhgia.Remove(danhgia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool DanhgiumExists(int id)
        {
            return _context.Danhgia.Any(e => e.MaDg == id);
        }
    }
}
