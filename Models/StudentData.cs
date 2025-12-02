using Microsoft.AspNetCore.Mvc;

namespace bolonotoproxy.Models
{
    public class StudentData : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
