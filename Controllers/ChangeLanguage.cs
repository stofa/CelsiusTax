using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CelsiusTax.Controllers
{
    public class ChangeLanguage : Controller
    {
        public IActionResult Index(string language)
        {
            Response.Cookies.Append(
    CookieRequestCultureProvider.DefaultCookieName,
    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
);
            return Redirect(Request.GetTypedHeaders().Referer?.ToString());
        }
    }
}
