using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings_Hotel.Pages.Users
{
    public class ProfileModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public ProfileModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account account { get; set; }

        public IActionResult OnGet(int id) 
        {
            account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }
            return Page();
        }


        // OnPost method to handle updates
        public IActionResult OnPost()
        {
            // Kiểm tra tính hợp lệ của ModelState
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu không hợp lệ, quay lại trang với thông tin hiện tại
            }

            // Tìm tài khoản hiện tại trong cơ sở dữ liệu
            var existingAccount = _context.Accounts.Find(account.AccountId);
            if (existingAccount == null)
            {
                return NotFound(); // Trả về 404 nếu tài khoản không tồn tại
            }

            // Cập nhật thông tin tài khoản
            existingAccount.FullName = account.FullName;
            existingAccount.Dob = account.Dob;
            existingAccount.Email = account.Email;
            existingAccount.Phonenumber = account.Phonenumber;
            existingAccount.Gender = account.Gender;
            existingAccount.Address = account.Address;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            return RedirectToPage("/Users/Profile", new { id = existingAccount.AccountId }); // Chuyển hướng đến trang hồ sơ
        }

    }
}
