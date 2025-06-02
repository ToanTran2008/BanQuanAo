using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOAN.Data;
using DOAN.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DOAN.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);


            // Lấy thông tin cửa hàng
            var cuahang = _context.Cuahangs.FirstOrDefault(k => k.MaCh == 1);
            if (cuahang != null)
            {
                ViewBag.cuahang = cuahang;
            }
            else
            {
                ViewBag.cuahang = new { Ten = "Cửa hàng chưa được cấu hình" };
            }


            // Lấy thông tin người dùng từ session
            var email = HttpContext.Session.GetString("nguoidung");
            if (email != null)
            {
                var nguoidung = _context.Nguoidungs.FirstOrDefault(k => k.Email == email);
                ViewBag.nguoidung = nguoidung;
            }

            var mand = HttpContext.Session.GetInt32("MaND");
            if (mand.HasValue)  // Kiểm tra nếu mand != null
            {
                var diachi = _context.Diachis.Where(dc => dc.MaNd == mand.Value).ToList();
                ViewBag.diachi = diachi;
            }

            ViewBag.danhmuc = _context.Danhmucs.ToList();

            ViewBag.danhgia = _context.Danhgia.ToList();

            ViewBag.Mathangs = _context.Mathangs.ToList();

            ViewBag.MathangKhuyenMai = _context.Khuyenmais.ToList();
            

           
            ViewBag.LoaiNd = new SelectList(new List<SelectListItem>
            {
              new SelectListItem { Text = "Khách hàng", Value = "Khách hàng" },
              new SelectListItem { Text = "Nhân viên", Value = "Nhân viên" },
              new SelectListItem { Text = "Quản lý", Value = "Quản lý" }
            }, "Value", "Text");
        }
    }
}
