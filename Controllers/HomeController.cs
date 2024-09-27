using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Controllers
{
    public class HomeController : Controller
    {
        private Booking_hotelContext _booking_hotelContext;
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            var genders = _booking_hotelContext.Users
                 .Select(u => u.Gender)
                 .ToList();
            ViewBag.Gender = new SelectList(genders, "UserId", "Gender");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind("UserId, Fullname, Email, Phonenumber, Username, Password, Gender, Address, DateOfBirth")] User user, string ConfirmPassword)
        {
            if (!ModelState.IsValid)
            {
                if (user.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password is incorrect");
                    var genders = _booking_hotelContext.Users.Select(u => u.Gender).ToList();
                    ViewBag.Gender = new SelectList(genders, "UserId", "Gender");
                    return View(user);
                }

                if (!IsValidEmail(user.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email address.");
                }

                if (!IsValidPhoneNumber(user.Phonenumber))
                {
                    ModelState.AddModelError("Phonenumber", "Invalid phonenumber");
                }

                if (ModelState.IsValid)
                {
                    if (_booking_hotelContext.Users.Any(u => u.Username == user.Username.ToLower().Trim()))
                    {
                        ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                        var genders = _booking_hotelContext.Users.Select(u => u.Gender).ToList();
                        ViewBag.Gender = new SelectList(genders, "UserId", "Gender");
                        return View(user);
                    }

                    Random random = new Random();
                    int otp = random.Next(100000, 999999);
                    if (SendOTP(user.Email, otp))
                    {
                        TempData["OTP"] = otp;
                        TempData["User"] = user;
                        return RedirectToAction("VerifyOTP", "Home");
                    }
                }
            }

            
            var gendersList = _booking_hotelContext.Users.Select(u => u.Gender).ToList();
            ViewBag.Gender = new SelectList(gendersList);
            return View(user);
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            
            return Regex.IsMatch(phoneNumber, @"^\d{10,11}$");
        }

        [HttpPost]
        public ActionResult VerifyOTP(int otp)
        {
            try
            {
                int storedOTP = Convert.ToInt32(TempData["OTP"]);
                if (otp == storedOTP)
                {
                    ViewBag.Message = "OTP xác thực thành công";
                    User user = TempData["User"] as User;
                    user.Phonenumber = user.Phonenumber.Trim();
                    user.Username = user.Username.Trim();

                    _booking_hotelContext.Users.Add(user);
                    _booking_hotelContext.SaveChanges();
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ModelState.AddModelError("otp", "Mã OTP không đúng vui lòng thử lại");
                    return View();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("otp", "Mã OTP Không hợp lệ !");
                return View();

            }


        }

        private bool SendOTP(string email, int otp)
        {
            try
            {
                // Configure SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Update with your SMTP server details
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("haizaki505@gmail.com", "dnwhqnezsnhqpgmg"); // Update with your email credentials
                smtpClient.EnableSsl = true;

                // Compose email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("haizaki505@gmail.com"); // Update with your email address
                mailMessage.To.Add(email);
                mailMessage.Subject = "Your OTP for Registration";
                mailMessage.Body = "Your OTP is: " + otp.ToString();

                // Send email
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Failed to send email
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public IActionResult Login()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear(); // Clear all session data

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {

            var user = _booking_hotelContext.Users.FirstOrDefault(u => u.Username == username.Trim() && u.Password == password);
            var manager = _booking_hotelContext.Managers.FirstOrDefault(u => u.Username == username.Trim() && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("user", user.UserId.ToString()); // Store user ID in session
                return RedirectToAction("Index", "Home", new { id = user.UserId });
            }
            else if (manager != null)
            {
                if (manager.Role == 1)
                {
                    HttpContext.Session.SetString("admin", manager.ManagerId.ToString()); // Store admin ID in session
                    return RedirectToAction("Managers", "Admin");
                }
                else if (manager.Role == 2)
                {
                    HttpContext.Session.SetString("staff", manager.ManagerId.ToString()); // Store staff ID in session
                    return RedirectToAction("Managers", "Staff");
                }
            }
            // Authentication failed, return back to login view with error message
            ModelState.AddModelError("Error_Login", "Tên đăng nhập hoặc mật khẩu không đúng !.");
                return View();
            
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = _booking_hotelContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("email", "Email không tồn tại trong hệ thống.");
                return View();
            }

            // Gửi OTP đến email của người dùng
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            if (SendOTPForgotPass(email, otp))
            {
                TempData["OTP"] = otp;
                TempData["UserID"] = user.UserId;
                return RedirectToAction("VerifyOTPForgotPassword");
            }

            return View();
        }
        public ActionResult VerifyOTPForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyOTPForgotPassword(int otp)
        {
            try
            {
                int storedOTP = Convert.ToInt32(TempData["OTP"]);
                if (otp == storedOTP)
                {
                    ViewBag.Message = "OTP xác thực thành công";
                    TempData["UserID"] = TempData["UserID"];

                    return RedirectToAction("Datlaimatkhau");
                }
                else
                {
                    ModelState.AddModelError("otp", "Mã OTP không đúng vui lòng thử lại");
                    return View();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("otp", "Mã OTP Không hợp lệ !");
                return View();

            }
        }

        private bool SendOTPForgotPass(string email, int otp)
        {
            try
            {
                // Configure SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Update with your SMTP server details
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("phamduycuong2k1@gmail.com", "krowpvhmjfsndfum"); // Update with your email credentials
                smtpClient.EnableSsl = true;

                // Compose email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("phamduycuong2k1@gmail.com"); // Update with your email address
                mailMessage.To.Add(email);
                mailMessage.Subject = "Mã OTP để đặt lại mật khẩu";
                mailMessage.Body = "Mã OTP của bạn là: " + otp.ToString();

                // Send email
                smtpClient.Send(mailMessage);
                return true;
            }


            catch (Exception ex)
            {
                // Failed to send email
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string password, string confirmPassword)
        {
            try
            {
                int userId = Convert.ToInt32(TempData["UserID"]);
                var user = _booking_hotelContext.Users.FirstOrDefault(x => x.UserId == userId);

                if (password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View();
                }

                user.Password = password;
                user.Phonenumber = user.Phonenumber.Trim();
                user.Username = user.Username.Trim();
                _booking_hotelContext.SaveChanges();
                ViewBag.Message = "Đặt lại mật khẩu thành công.";
                return RedirectToAction("Dangnhap", "Home");
            }
            catch (DbUpdateException ex)
            {
                // Log the error or show an error message
                Debug.WriteLine("An error occurred while updating the user: " + ex.Message);
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật thông tin. Vui lòng thử lại.");
                return View();
            }
            catch (Exception ex)
            {
                // Log the error or show a general error message
                Debug.WriteLine("An unexpected error occurred: " + ex.Message);
                ModelState.AddModelError("", "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
                return View();
            }
        }

    }
}
