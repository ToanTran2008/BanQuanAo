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
    public class CuahangsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CuahangsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Cuahangs
        public async Task<IActionResult> Index()
        {
            var cuahangs = await _context.Cuahangs.ToListAsync(); // Lấy danh sách cửa hàng
            return View(cuahangs);
        }

        // GET: Cuahangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuahang = await _context.Cuahangs
                .FirstOrDefaultAsync(m => m.MaCh == id);
            if (cuahang == null)
            {
                return NotFound();
            }

            return View(cuahang);
        }

        // GET: Cuahangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuahang = await _context.Cuahangs.FindAsync(id);
            if (cuahang == null)
            {
                return NotFound();
            }
            return View(cuahang);
        }

        // POST: Cuahangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile HinhAnh, [Bind("MaCh,Ten,DienThoai,Email,HinhAnh,DiaChi")] Cuahang cuahang)
        {
            if (id != cuahang.MaCh)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra xem đã có thực thể cùng MaCh đang được EF track chưa
                var trackedEntity = _context.ChangeTracker.Entries<Cuahang>()
                                            .FirstOrDefault(e => e.Entity.MaCh == cuahang.MaCh);

                if (trackedEntity != null)
                {
                    // Nếu có thì detach nó trước
                    trackedEntity.State = EntityState.Detached;
                }

                // Lấy ảnh cũ nếu không chọn ảnh mới
                var existing = await _context.Cuahangs.AsNoTracking().FirstOrDefaultAsync(x => x.MaCh == id);
                if (existing == null)
                {
                    return NotFound();
                }

                if (HinhAnh != null)
                {
                    cuahang.HinhAnh = Upload(HinhAnh);
                }
                else
                {
                    cuahang.HinhAnh = existing.HinhAnh;
                }

                // Gắn cuahang và đánh dấu là đang sửa
                _context.Attach(cuahang);
                _context.Entry(cuahang).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuahangExists(cuahang.MaCh))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CuahangExists(int id)
        {
            return _context.Cuahangs.Any(e => e.MaCh == id);
        }

        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\images\\Logo\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }
    }
}
