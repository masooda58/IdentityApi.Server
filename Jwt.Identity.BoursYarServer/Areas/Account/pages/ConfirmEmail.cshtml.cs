using System.Text;
using System.Threading.Tasks;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Jwt.Identity.BoursYarServer.Areas.Account.pages
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData] public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string code)
        {
            if (email == null || code == null) return RedirectToPage("/Index");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                // return NotFound();
                return NotFound($"Unable to load user with ID '{email}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "ایمیل شما تایید گردید" : "در تایید ایمیل خطای بوجود آمده است";
            return Page();
        }
    }
}