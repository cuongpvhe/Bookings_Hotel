using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

            var currentAccount = await _context.Accounts.AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == Account.AccountId);

            if (currentAccount == null)
            {
                TempData["ErrorMessage"] = "Tài khoản nhân viên không tồn tại";
                return RedirectToPage("./Edit", new { id = Account.AccountId });
            }

            if (await _context.Accounts.AnyAsync(a => a.Email == Account.Email && a.AccountId != currentAccount.AccountId))
            {
                TempData["ErrorMessage"] = "Email đã tồn tại";
                return RedirectToPage("./Edit", new { id = Account.AccountId });
            }

            if (await _context.Accounts.AnyAsync(a => a.UseName == Account.UseName && a.AccountId != currentAccount.AccountId))
            {
                TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại";
                return RedirectToPage("./Edit", new { id = Account.AccountId });
            }
            if (await _context.Accounts.AnyAsync(a => a.Phonenumber == Account.Phonenumber && a.AccountId != currentAccount.AccountId))
            {
                TempData["ErrorMessage"] = "Số điện thoại đã tồn tại";
                return RedirectToPage("./Edit", new { id = Account.AccountId });
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
            else
            {
                Account.Avatar = currentAccount.Avatar;
            }

            try
            {
                Account.UpdateDate = DateTime.Now;
                _context.Accounts.Update(Account);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage("./Edit", new { id = Account.AccountId });
        }

    }
}
