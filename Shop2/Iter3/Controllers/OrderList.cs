using Iter3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Iter3.Controllers
{
    public class OrderList : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private const int DefaultPageSize = 5;

        public OrderList(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = DefaultPageSize, DateTime? fromDate = null, DateTime? toDate = null, string status = null, string searchTerm = null)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSizePricing)
                        .ThenInclude(psp => psp.Product)
                .AsQueryable();

            // Chỉ lọc theo ngày nếu cả fromDate và toDate có giá trị
            if (fromDate.HasValue && toDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= fromDate.Value && o.CreatedAt <= toDate.Value);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                bool isCompleted = status == "completed";
                orders = orders.Where(o => o.Status == isCompleted);
            }

            // Lọc theo tên người dùng
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.User.FirstName.Contains(searchTerm) || o.User.LastName.Contains(searchTerm));
            }

            // Phân trang
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)orders.Count() / pageSize);

            // Lấy danh sách đơn hàng với phân trang
            var paginatedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền các thông tin lọc cho View
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Status = status;
            ViewBag.SearchTerm = searchTerm;

            return View(paginatedOrders);
        }

        [HttpPost]
        public IActionResult ChangePageSize(int change)
        {
            var currentPageSize = int.TryParse(Request.Form["pageSize"], out int size) ? size : DefaultPageSize;
            var newPageSize = currentPageSize + change;

            // Đảm bảo pageSize không nhỏ hơn 1
            if (newPageSize < 1) newPageSize = 1;

            return RedirectToAction("Index", new { page = ViewBag.CurrentPage, pageSize = newPageSize });
        }
    }
}
