using Bookings_Hotel.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        private readonly Cloudinary _cloudinary;
        public ProfileModel(HotelBookingSystemContext context, IHostEnvironment environment, Cloudinary cloudinary)
        {
            _context = context;
            _environment = environment;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public Models.Account Account { get; set; }

        [BindProperty]
        public IFormFile? AvatarUpload { get; set; }

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

            // Kiểm tra trùng lặp dữ liệu
            if (await _context.Accounts.AnyAsync(a => a.Email == Account.Email && a.AccountId != existingAccount.AccountId))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            if (await _context.Accounts.AnyAsync(a => a.UseName == Account.UseName && a.AccountId != existingAccount.AccountId))
            {
                ModelState.AddModelError("UseName", "Tên tài khoản đã tồn tại.");
            }

            if (await _context.Accounts.AnyAsync(a => a.FullName == Account.FullName && a.AccountId != existingAccount.AccountId))
            {
                ModelState.AddModelError("FullName", "Họ và tên đã tồn tại.");
            }

            if (await _context.Accounts.AnyAsync(a => a.Phonenumber == Account.Phonenumber && a.AccountId != existingAccount.AccountId))
            {
                ModelState.AddModelError("Phonenumber", "Số điện thoại đã tồn tại.");
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

            // Chỉ xử lý việc tải ảnh lên nếu có
            /*            if (AvatarUpload != null && AvatarUpload.Length > 0)
                        {
                            // Kiểm tra định dạng tệp
                            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var extension = Path.GetExtension(AvatarUpload.FileName).ToLowerInvariant();

                            if (!permittedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("AvatarUpload", "Vui lòng tải lên tệp hình ảnh hợp lệ (jpg, jpeg, png, gif).");
                                return Page();
                            }



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
                        }*/
            if (AvatarUpload != null && AvatarUpload.Length > 0)
            {

                if (!string.IsNullOrEmpty(existingAccount.Avatar))
                {
                    // Extract public ID from the existing URL
                    var publicId = Path.GetFileNameWithoutExtension(new Uri(existingAccount.Avatar).AbsolutePath);
                    var deleteParams = new DeletionParams(publicId);
                    await _cloudinary.DestroyAsync(deleteParams);
                }


                using (var stream = AvatarUpload.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(AvatarUpload.FileName, stream),
                        Folder = "hotel_avatars", // Your Cloudinary folder
                        PublicId = $"{existingAccount.AccountId}_{Path.GetFileNameWithoutExtension(AvatarUpload.FileName)}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        existingAccount.Avatar = uploadResult.SecureUrl.AbsoluteUri;

                        // Update claim
                        var identity = (ClaimsIdentity)User.Identity;
                        var avatarClaim = identity.FindFirst("Avatar");

                        if (avatarClaim != null)
                        {
                            identity.RemoveClaim(avatarClaim);
                        }
                        identity.AddClaim(new Claim("Avatar", existingAccount.Avatar));
                        await HttpContext.SignInAsync(User);
                    }
                    else
                    {
                        ModelState.AddModelError("AvatarUpload", "Lỗi tải ảnh lên Cloudinary.");
                        return Page();
                    }
                }
            }




            existingAccount.UpdateDate = DateTime.Now;
            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công";

            return RedirectToPage("/Users/Profile");
        }

    }
}
