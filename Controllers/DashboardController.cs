using bolonotoproxy.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Protocol;
using bolonotoproxy.Models;

namespace bolonotoproxy.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult MyDashboard()
        {
            if(!TempData.ContainsKey("AuthToken"))
            {
                return RedirectToAction("signin", "Account");

            }
            string tokenFromUser = TempData["AuthToken"].ToString();
            var storedToken = _db.UserTokens
                                .Include(t => t.User)
                                .FirstOrDefault(testc => testc.TokenValue == tokenFromUser);
            if (storedToken == null || storedToken.ExpiryDate <= DateTime.UtcNow) 
            {
                return RedirectToAction("Index", "Home");
            }
            TempData.Keep("AuthToken");
            return View(storedToken.User);
        }
          
        public IActionResult AddStudent()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddStudent(Student obj)
        {
            var token = TempData["AuthToken"] as string;
            TempData.Keep("AuthToken");

            if (token != null)
            {
                var tokenRecord = _db.UserTokens.FirstOrDefault(t => t.TokenValue == token);
                if (tokenRecord != null)
                {
                    obj.signupID = tokenRecord.UserId; // <--- LINKING THE USER
                }
            }
            ModelState.Remove("signupID");
            ModelState.Remove("teacherid");
            ModelState.Remove("imageAddress");
            ModelState.Remove("ImageFile");


            if (ModelState.IsValid)
            {
                if (obj.ImageFile != null)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(obj.ImageFile.FileName);
                    string filepath = Path.Combine(webRootPath, "images", filename);
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    // 2. Check if the FOLDER exists (not the file)
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    using (var filestream=new FileStream(filepath,FileMode.Create))
                    {
                        obj.ImageFile.CopyTo(filestream);
                    }
                    obj.imageAddress = "/images/" + filename;
                    _db.Students.Add(obj);
                    _db.SaveChanges();

                }
                return RedirectToAction("MyDashboard");

            }
            return View(obj);
            
        }
        public IActionResult MarkAttendence()
        {
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> MarkAttendence(int studentid, IFormFile liveImage)
        {
            var student = _db.Students.FirstOrDefault(u => u.RollNo == studentid);
            if (student == null) return Json(new { success = false, message = "Student ID not found." });

            // B. DATE CHECK
            var existing = _db.Mark_Attendence.FirstOrDefault(a => a.Studentid == student.RollNo && a.DateTime.Date == DateTime.Now.Date);
            if (existing != null) return Json(new { success = false, message = "Already marked Present today!" });

            var id =_db.Students.FirstOrDefault(t => t.RollNo == studentid);
            if (id == null)
                return Json(new { success = false, message = "Student ID not found." });
            string webroot=_webHostEnvironment.WebRootPath;
            string liveimagetemp = Path.Combine(webroot, "tempLive");
            if (!Directory.Exists(liveimagetemp))
            {
                Directory.CreateDirectory(liveimagetemp);
            }
            string liveimageadd = Guid.NewGuid().ToString() + ".jpg";
            string livepath=Path.Combine(liveimagetemp, liveimageadd);
            using (var fs = new FileStream(livepath, FileMode.Create))
            {
                await liveImage.CopyToAsync(fs); // Async copy
            }
            string storedPath = Path.Combine(_webHostEnvironment.WebRootPath, id.imageAddress.TrimStart('/').Replace("/", "\\"));
            bool isMatch=false;
            string apiMessage = "";
            try
            {
                var payload = new
                {
                    stored_path = storedPath,
                    live_path = livepath
                };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

            
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://127.0.0.1:5000/verify", jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        // 4. Read the Reply
                        var responseString = await response.Content.ReadAsStringAsync();

                        // Parse JSON: {"match": true, "error": null}
                        using (JsonDocument doc = JsonDocument.Parse(responseString))
                        {
                            isMatch = doc.RootElement.GetProperty("match").GetBoolean();

                            // Check for python errors
                            var errorElement = doc.RootElement.GetProperty("error");
                            if (errorElement.ValueKind != JsonValueKind.Null)
                            {
                                apiMessage = errorElement.GetString();
                            }
                        }
                    }
                    else
                    {
                        apiMessage = "API Connection Failed";
                    }

                }
            }
            catch (Exception ex)
            {
                apiMessage = "Server Error: " + ex.Message;
            }

            // F. CLEANUP
            System.IO.File.Delete(livepath);
            // G. FINAL DECISION
            if (isMatch)
            {
                Attendence att = new Attendence
                {
                    Studentid = student.RollNo,
                    DateTime = DateTime.Now,
                    status = "Present"
                };
                _db.Mark_Attendence.Add(att);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Attendance Marked Successfully!" });
            }
            else
            {
                return Json(new { success = false, message = string.IsNullOrEmpty(apiMessage) ? "Face Mismatch." : apiMessage });
            }


            return View();
        }
    }


}
