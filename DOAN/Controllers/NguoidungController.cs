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

namespace DOAN.Controllers
{
    public class NguoidungsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Nguoidung> _passwordHasher;

        public NguoidungsController(ApplicationDbContext context, IPasswordHasher<Nguoidung> passwordHasher) : base(context)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Nguoidungs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Nguoidungs.ToListAsync());
        }

        // GET: Nguoidungs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoidung = await _context.Nguoidungs
                .FirstOrDefaultAsync(m => m.MaNd == id);
            if (nguoidung == null)
            {
                return NotFound();
            }

            return View(nguoidung);
        }

        // GET: Nguoidungs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nguoidungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNd,Ten,DienThoai,Email,MatKhau,LoaiNd")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {

                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                if (!string.IsNullOrEmpty(nguoidung.MatKhau))
                {
                    nguoidung.MatKhau = _passwordHasher.HashPassword(nguoidung, nguoidung.MatKhau);
                }

                _context.Add(nguoidung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoidung);
        }

        // GET: Nguoidungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound();
            }
            return View(nguoidung);
        }

        // POST: Nguoidungs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNd,Ten,DienThoai,Email,MatKhau,LoaiNd")] Nguoidung nguoidung)
        {
            if (id != nguoidung.MaNd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem thực thể có đang được track không
                    var existingEntity = await _context.Nguoidungs.FirstOrDefaultAsync(x => x.MaNd == nguoidung.MaNd);
                    if (existingEntity != null)
                    {
                        // Nếu mật khẩu mới được nhập, mã hóa mật khẩu và cập nhật
                        if (!string.IsNullOrEmpty(nguoidung.MatKhau))
                        {
                            existingEntity.MatKhau = _passwordHasher.HashPassword(existingEntity, nguoidung.MatKhau);
                        }

                        // Cập nhật các giá trị khác (trừ mật khẩu đã xử lý riêng)
                        existingEntity.Ten = nguoidung.Ten;
                        existingEntity.DienThoai = nguoidung.DienThoai;
                        existingEntity.Email = nguoidung.Email;
                        existingEntity.LoaiNd = nguoidung.LoaiNd;

                        // Lưu thay đổi

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound(); // Trường hợp thực thể không tồn tại
                    }

                    // Lưu thay đổi
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoidungExists(nguoidung.MaNd))
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
            return View(nguoidung);
        }


        // GET: Nguoidungs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoidung = await _context.Nguoidungs
                .FirstOrDefaultAsync(m => m.MaNd == id);
            if (nguoidung == null)
            {
                return NotFound();
            }

            return View(nguoidung);
        }

        // POST: Nguoidungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung != null)
            {
                _context.Nguoidungs.Remove(nguoidung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoidungExists(int id)
        {
            return _context.Nguoidungs.Any(e => e.MaNd == id);
        }
    }
}
