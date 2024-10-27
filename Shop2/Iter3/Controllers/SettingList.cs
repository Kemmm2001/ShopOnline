using Iter3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Iter3.Controllers
{
    public class SettingList : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private const int DefaultPageSize = 10;

        public SettingList(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = DefaultPageSize, string searchTerm = null)
        {
            var users = _context.Users
                .Include(u => u.Status)
                .Include(u => u.Role)
                .AsQueryable();

            // Tìm kiếm theo tên người dùng
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm));
            }

            // Phân trang
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize);

            var paginatedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền thông tin tìm kiếm vào View
            ViewBag.SearchTerm = searchTerm;

            return View(paginatedUsers);
        }

        [HttpPost]
        public IActionResult ChangePageSize(int change)
        {
            var currentPageSize = int.TryParse(Request.Form["pageSize"], out int size) ? size : DefaultPageSize;
            var newPageSize = currentPageSize + change;

            if (newPageSize < 1) newPageSize = 1;

            return RedirectToAction("Index", new { page = ViewBag.CurrentPage, pageSize = newPageSize });
        }
    }
}
