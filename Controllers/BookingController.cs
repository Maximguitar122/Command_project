using Luftreise_Command_project_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Luftreise_Command_project_.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Details(int id)
        {
            return View();
        }

    }
}
