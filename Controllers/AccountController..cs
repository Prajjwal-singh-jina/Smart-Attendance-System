using bolonotoproxy.Data;
using bolonotoproxy.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace bolonotoproxy.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db)
        {
            _db=db;
            
        }
        
        public IActionResult signin()
        {

            return View();
            
        }
        [HttpPost]
        [HttpPost]
        public IActionResult signin(Sign_up loginData)
        {
            // 1. Debugging: Check if data arrived
            if (loginData == null)
            {
                ViewBag.Error = "Data not received from the form.";
                return View();
            }

            // 2. Fetch user matching BOTH Email and Password
            // Notice we use loginData.Email because we accepted the object
            var existingUser = _db.register.FirstOrDefault(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (existingUser != null)
            {
                // --- SUCCESS ---
                var tokenValue = GenerateSecureToken();
                var userToken = new UserToken
                {
                    UserId = existingUser.Id,
                    TokenValue = tokenValue,
                    ExpiryDate = DateTime.UtcNow.AddDays(7)
                };
                _db.UserTokens.Add(userToken);
                _db.SaveChanges();

                // Pass the token
                TempData["AuthToken"] = tokenValue;
                TempData.Keep("AuthToken"); // Keep it so it doesn't disappear on refresh

                return RedirectToAction("MyDashboard", "Dashboard");
            }
            else
            {
                // --- FAILURE ---
                ViewBag.Error = "Invalid Email or Password";
                return View(loginData); // Return the data so they don't have to re-type everything
            }
        }
        public IActionResult signup()
        {
            return View();

        }
        [HttpPost]
        public IActionResult signup(string email, string password )

        {
           Sign_up sign_Up = new Sign_up
           {
               // 2. Set the properties of the new object
               Email = email,
               Password = password // See security note below!
           };
            _db.register.Add(sign_Up);
            _db.SaveChanges();
            var tokenValue = GenerateSecureToken();
            var userToken = new UserToken
            {
                UserId = sign_Up.Id,
                TokenValue = tokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7)

            };
            _db.UserTokens.Add(userToken);
            _db.SaveChanges();
            TempData["AuthToken"] = tokenValue;
            return RedirectToAction("MyDashboard","Dashboard");
            
            
        }
        private string GenerateSecureToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }








    }
}
