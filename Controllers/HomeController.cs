using Business.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Supermarket.ModelViews;
using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Supermarket.Models;
using Domain;

namespace Supermarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbMarketsContext _context;
        private readonly LanguageService _localizer;

        public HomeController(ILogger<HomeController> logger, DbMarketsContext context, LanguageService localizer)
        {
            _logger = logger;
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return Index(item);
        }

        public IActionResult Index(Category item)
        {
         //   ViewBag.deneme = _localizer.Getkey("bestSelling").Value;
            HomeViewVM model = new HomeViewVM();

            var lsProducts = _context.Products.AsNoTracking()
               .Where(x => x.Active == true && x.HomeFlag == true)
               .OrderByDescending(x => x.DateCreated)
               .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();
            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x => x.CatId == item.CatId).ToList();
                lsProductViews.Add(productHome);

                var advertise = _context.Advertises
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Active == true);

                var New = _context.TinDangs
                    .AsNoTracking()
                    .Where(x => x.Published == true && x.IsNewfeed == true)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(3)
                    .ToList();
                model.Products = lsProductViews;
                model.Advertise = advertise;
                model.News = New;
                ViewBag.AllProducts = lsProducts;
            }
            return View(model);
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1)});

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Route("contact.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("about.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}