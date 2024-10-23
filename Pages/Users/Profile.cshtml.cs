using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Users
{
    public class ProfileModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly IHostEnvironment _environment;

        public ProfileModel(HotelBookingSystemContext context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Account Account { get; set; }

        [BindProperty]
        public IFormFile AvatarUpload { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;

            if (accountId == null)
            {
                return RedirectToPage("/Home/Login");
            }

            Account = await _context.Accounts.FindAsync(int.Parse(accountId));

            if (Account == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;

            if (accountId == null)
            {
                return RedirectToPage("/Home/Login");
            }

            var existingAccount = await _context.Accounts.FindAsync(int.Parse(accountId));

            if (existingAccount == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin tài khoản
            existingAccount.FullName = Account.FullName;
            existingAccount.Dob = Account.Dob;
            existingAccount.Email = Account.Email;
            existingAccount.UseName = Account.UseName;
            existingAccount.Password = Account.Password;
            existingAccount.Phonenumber = Account.Phonenumber;
            existingAccount.Gender = Account.Gender;
            existingAccount.Address = Account.Address;

            // Xử lý việc tải ảnh lên
            if (AvatarUpload != null && AvatarUpload.Length > 0)
            {
                // Lưu đường dẫn tới thư mục uploads trong wwwroot
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads");

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file
                var fileName = $"{existingAccount.AccountId}_{Path.GetFileName(AvatarUpload.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file lên server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarUpload.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn ảnh vào DB
                existingAccount.Avatar = $"/uploads/{fileName}";

                // Cập nhật claim Avatar
                var identity = (ClaimsIdentity)User.Identity;
                var avatarClaim = identity.FindFirst("Avatar");

                if (avatarClaim != null)
                {
                    identity.RemoveClaim(avatarClaim);
                }
                identity.AddClaim(new Claim("Avatar", existingAccount.Avatar));

                // Reload lại claims của người dùng
                await HttpContext.SignInAsync(User);
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            return RedirectToPage("/Users/Profile");
        }
    }
}
