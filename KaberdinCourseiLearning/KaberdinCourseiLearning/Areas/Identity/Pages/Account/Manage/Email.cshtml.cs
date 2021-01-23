using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Resources;
using Microsoft.Extensions.Localization;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<EmailModel> localizer;

        public EmailModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IEmailSender emailSender,
            IStringLocalizer<EmailModel> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.localizer = localizer;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.EmailRequired))]
            [EmailAddress(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.EmailInvalid))]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.NewEmailPrompt), Prompt = nameof(ValidationResource.NewEmailPrompt))]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(CustomUser user)
        {
            var email = await userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await emailSender.SendEmailAsync(
                    Input.NewEmail,
                    localizer["EmailSubject"],
                    localizer["EmailMessage", HtmlEncoder.Default.Encode(callbackUrl)]);

                StatusMessage = localizer["Confirmation link to change email sent. Please check your email."];
                return RedirectToPage();
            }

            StatusMessage = localizer["Your email is unchanged."];
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await userManager.GetUserIdAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await emailSender.SendEmailAsync(
                email,
                localizer["EmailSubject"],
                localizer["EmailMessage", HtmlEncoder.Default.Encode(callbackUrl)]);

            StatusMessage = localizer["Verification email sent. Please check your email."];
            return RedirectToPage();
        }
    }
}
