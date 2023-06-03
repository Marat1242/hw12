using AspNetCoreHero.ToastNotification.Abstractions;
using Business;
using Business.Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Supermarket.Controllers
{
    public class OderController : Controller
    {
        private readonly DbMarketsContext _context;
        public INotyfService _notyfService { get; }
        public OderController(DbMarketsContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang == null) return NotFound();
                var donhang = await _context.Orders
                    .Include(x => x.TransactStatus)
                    .FirstOrDefaultAsync(m => m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerId);
                if (donhang == null) return NotFound();

                var chitietdonhang = _context.OrderDetails
                    .Include(x => x.Product)
                    .AsNoTracking()
                    .Where(x => x.OrderId == id)
                    .OrderBy(x => x.OrderDetailId)
                    .ToList();
                ViewOders donHang = new ViewOders();
                donHang.Order = donhang;
                donHang.OrderDetail = chitietdonhang;
                return PartialView("Details", donHang);

            }
            catch
            {
                return NotFound();
            }
        }
    }
}
