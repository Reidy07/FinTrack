// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FinTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack.Web.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // 1. Si el usuario NO existe, fingimos que todo salió bien (Seguridad OWASP contra Enumeración)
                if (user == null)
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // 2. Si el usuario SÍ existe pero NO ha confirmado su correo
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Le reenviamos el correo de confirmación de cuenta.
                    var confirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    confirmCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmCode));

                    var confirmCallbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = confirmCode },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Acción Requerida: Confirma tu cuenta de FinTrack",
                        $"Hola. Notamos que intentaste recuperar tu contraseña, pero tu cuenta aún no está confirmada. Por favor, confirma tu correo haciendo clic <a href='{HtmlEncoder.Default.Encode(confirmCallbackUrl)}'>aquí</a> para poder continuar.");

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));

                var resetCallbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code = resetCode },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Restablece tu contraseña de FinTrack",
                    $"Para restablecer tu contraseña, haz clic <a href='{HtmlEncoder.Default.Encode(resetCallbackUrl)}'>aquí</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
