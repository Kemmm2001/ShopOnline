using Microsoft.AspNetCore.Mvc;
using Iter3.Models;  // Your models namespace
using System.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using Amazon.Extensions.NETCore.Setup;

namespace Iter3.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;  // Assuming you have a DbContext
        private readonly IConfiguration _configuration;

        public OrderDetailController(G3_PerfumeShopDB_Iter3Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                                    Gender = o.User.Gender,  // Lấy Gender từ User
                                    Address = o.User.Address,  // Lấy Address từ User
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

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, bool newStatus)
        {
            // Tìm đơn hàng theo orderId
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái
            order.Status = newStatus;
            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Chuyển hướng trở lại trang chi tiết đơn hàng với orderId
            return RedirectToAction("Index", new { orderId = orderId });
        }
        public async Task<IActionResult> UpdateImage(int orderId, IFormFile newImage)
        {
            if (newImage != null && newImage.Length > 0)
            {
                // Lấy thông tin cấu hình S3 từ appsettings.json
                var accessKey = _configuration["AWS:AccessKey"];
                var secretKey = _configuration["AWS:SecretKey"];
                var bucketName = _configuration["AWS:BucketName"];
                var serviceURL = _configuration["AWS:ServiceURL"];

                // Cấu hình Amazon S3 với chỉ ServiceURL (nếu không dùng Region)
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = serviceURL
                };

                try
                {
                    // Khởi tạo S3 Client
                    var s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);

                    // Đặt breakpoint tại đây để kiểm tra s3Client đã được tạo hay chưa
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newImage.OpenReadStream(),
                        Key = $"images/{newImage.FileName}",
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var transferUtility = new TransferUtility(s3Client);

                    // Đặt breakpoint tại dòng này để kiểm tra khi bắt đầu upload
                    await transferUtility.UploadAsync(uploadRequest);

                    var imageUrl = $"{serviceURL}/{bucketName}/images/{newImage.FileName}";

                    // Cập nhật URL ảnh vào cơ sở dữ liệu
                    var orderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId);
                    if (orderDetail != null)
                    {
                        orderDetail.ProductSizePricing.Product.ImageUrl = imageUrl;
                        _context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Đặt breakpoint tại đây để kiểm tra nếu có lỗi xảy ra
                    Console.WriteLine("Lỗi khi kết nối và upload lên S3: " + ex.Message);
                    // Log thêm chi tiết nếu cần, ví dụ: StackTrace hoặc InnerException
                }
            }

            return RedirectToAction("Index", new { orderId });
        }

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
    public int? Gender { get; set; }  // Thêm thuộc tính Gender
    public string Address { get; set; }
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

