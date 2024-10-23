using Microsoft.AspNetCore.Mvc;
using Iter3.Models;  // Your models namespace
using System.Linq;

namespace Iter3.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter2Context _context;  // Assuming you have a DbContext

        public OrderDetailController(G3_PerfumeShopDB_Iter2Context context)
        {
            _context = context;
        }

        public IActionResult Index(int orderId)
        {
            var order = _context.Orders
                                .Where(o => o.Id == orderId)
                                .Select(o => new OrderDetailViewModel
                                {
                                    OrderId = o.Id,
                                    UserName = o.User.FirstName + " " + o.User.LastName,
                                    Email = o.User.Email,
                                    Phone = o.User.Phone,
                                    CreatedAt = o.CreatedAt,
                                    ShippingAddress = o.ShippingAddress,
                                    Status = o.Status,
                                    Products = o.OrderDetails.Select(od => new ProductViewModel
                                    {
                                        ProductName = od.ProductSizePricing.Product.Name,
                                        Size = od.ProductSizePricing.Size,
                                        Quantity = od.Quantity,
                                        Price = od.ProductSizePricing.Price,
                                        ImageUrl = od.ProductSizePricing.Product.ImageUrl
                                    }).ToList()
                                }).FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ShippingAddress { get; set; }
        public bool Status { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }

    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
