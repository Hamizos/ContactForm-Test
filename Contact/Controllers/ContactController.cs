using Contact.Context;
using Contact.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Contact.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const int Limit = 5;
        private const int TimeWindow = 60;

        public ContactController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public ActionResult Index()
        {
            return View(new ContactModel());
        }
        public ActionResult Create()
        {
            return View(new ContactModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ContactModel model)
        {
            string userIp = HttpContext.Connection.RemoteIpAddress.ToString();
            string cacheKey = $"RequestCount_{userIp}";

            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            int requestCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TimeWindow);
                return 0;
            });

            if (requestCount >= Limit)
            {
                ModelState.AddModelError("", "Vous avez atteint la limite de messages par heure.");
                return View("Create", model);

            }

            _cache.Set(cacheKey, requestCount + 1);

            var contact = new ContactModel
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message
            };

            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
