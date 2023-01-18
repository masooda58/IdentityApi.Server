using Microsoft.AspNetCore.Mvc;

namespace Jwt.Identity.BoursYarServer.Controllers
{
    [ApiController]
    [Route("AccountManage/[controller]")]
    public class AccountManagerController : Controller
    {
        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }
    }
}