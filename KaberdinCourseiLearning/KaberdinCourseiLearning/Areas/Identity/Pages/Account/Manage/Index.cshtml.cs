using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;

        public IndexModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name ="Username")]
            public string Username { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(CustomUser user)
        {
            var userName = await userManager.GetUserNameAsync(user);
            var phoneNumber = await userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Username = userName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var userName = await userManager.GetUserNameAsync(user);
            if (Input.Username != userName)
            {
                var setUserName = await userManager.SetUserNameAsync(user, Input.Username);
                if (!setUserName.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set Username.";
                    return RedirectToPage();
                }
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
