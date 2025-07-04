﻿using System;
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
    public class CthoadonsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CthoadonsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Cthoadons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cthoadons.Include(c => c.MaHdNavigation).Include(c => c.MaMhNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cthoadons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons
                .Include(c => c.MaHdNavigation)
                .Include(c => c.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaCthd == id);
            if (cthoadon == null)
            {
                return NotFound();
            }

            return View(cthoadon);
        }

        // GET: Cthoadons/Create
        public IActionResult Create()
        {
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd");
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh");
            return View();
        }

        // POST: Cthoadons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCthd,MaHd,MaMh,DonGia,SoLuong,ThanhTien")] Cthoadon cthoadon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cthoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }

        // GET: Cthoadons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons.FindAsync(id);
            if (cthoadon == null)
            {
                return NotFound();
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }

        // POST: Cthoadons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCthd,MaHd,MaMh,DonGia,SoLuong,ThanhTien")] Cthoadon cthoadon)
        {
            if (id != cthoadon.MaCthd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cthoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CthoadonExists(cthoadon.MaCthd))
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
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }

        // GET: Cthoadons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons
                .Include(c => c.MaHdNavigation)
                .Include(c => c.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaCthd == id);
            if (cthoadon == null)
            {
                return NotFound();
            }

            return View(cthoadon);
        }

        // POST: Cthoadons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cthoadon = await _context.Cthoadons.FindAsync(id);
            if (cthoadon != null)
            {
                _context.Cthoadons.Remove(cthoadon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthoadonExists(int id)
        {
            return _context.Cthoadons.Any(e => e.MaCthd == id);
        }
    }
}
