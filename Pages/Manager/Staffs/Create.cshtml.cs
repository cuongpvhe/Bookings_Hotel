using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bookings_Hotel.Models;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Util;
using CloudinaryDotNet.Actions;

namespace Bookings_Hotel.Pages.Manager.Staffs
{
    public class CreateModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        private readonly Cloudinary _cloudinary;
        private readonly String DefaultImage = "https://res.cloudinary.com/dt9hjydap/image/upload/v1731182417/hrrx6nnwuvrxbaqlacne.jpg";

        public CreateModel(Bookings_Hotel.Models.HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        [BindProperty]
        public Bookings_Hotel.Models.Account Account { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid || _context.Accounts == null || Account == null)
                {
                    TempData["ErrorMessage"] = "Các trường thông tin không chính xác";
                    return Page();
                }

                if (await _context.Accounts.AnyAsync(a => a.Email == Account.Email))
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại";
                    return Page();
                }

                if (await _context.Accounts.AnyAsync(a => a.UseName == Account.UseName))
                {
                    TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại";
                    return Page();
                }
                if (await _context.Accounts.AnyAsync(a => a.Phonenumber == Account.Phonenumber))
                {
                    TempData["ErrorMessage"] = "Số điện thoại đã tồn tại";
                    return Page();
                }

                if (AvatarFile != null)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(AvatarFile.FileName, AvatarFile.OpenReadStream()),
                        Folder = "hotel_images"
                    });
                    Account.Avatar = uploadResult.SecureUrl.ToString();
                } else
                {
                    Account.Avatar = DefaultImage;
                }
                Account.RoleId = _context.Roles.FirstOrDefault(a => a.RoleName == RoleName.STAFF).RoleId;
                Account.Dob = (Account.Dob != null ? Account.Dob : DateTime.Now);
                Account.CreatedDate = DateTime.Now;
                Account.UpdateDate = DateTime.Now;

                _context.Accounts.Add(Account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới nhân viên thành công";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return Page();
        }
    }
}
