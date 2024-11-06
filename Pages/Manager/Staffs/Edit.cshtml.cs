using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookings_Hotel.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Bookings_Hotel.Pages.Manager.Staffs
{
    public class EditModel : PageModel
    {
        private readonly Bookings_Hotel.Models.HotelBookingSystemContext _context;

        private readonly Cloudinary _cloudinary;

        public EditModel(Bookings_Hotel.Models.HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        [BindProperty]
        public Bookings_Hotel.Models.Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            Account = account;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Các trường thông tin không chính xác";
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
            }

            Account.UpdateDate = DateTime.Now;
            _context.Attach(Account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AccountExists(Account.AccountId))
                {
                    TempData["ErrorMessage"] = "Tài khoản nhân viên không tồn tại";
                }
                else
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            return RedirectToPage("./Edit", new { id = Account.AccountId });
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
