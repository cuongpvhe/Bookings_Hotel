using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Linq;

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
        public Account Account { get; set; } = new Account(); // Bind account thông tin

        // Nạp thông tin người dùng khi truy cập trang Profile
        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy accountId từ claim của người dùng đã đăng nhập
            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;

            if (accountId == null)
            {
                return RedirectToPage("/Home/Login"); // Nếu chưa đăng nhập, quay lại trang login
            }

            // Tìm tài khoản người dùng từ cơ sở dữ liệu
            Account = await _context.Accounts.FindAsync(int.Parse(accountId));

            if (Account == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy tài khoản
            }

            return Page(); // Trả về trang với dữ liệu tài khoản được nạp
        }

        // Xử lý khi người dùng submit thông tin
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu thông tin không hợp lệ, quay lại trang với lỗi
            }

            // Lấy accountId từ claim của người dùng đã đăng nhập
            var accountId = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;

            if (accountId == null)
            {
                return RedirectToPage("/Home/Login"); // Nếu chưa đăng nhập, quay lại trang login
            }

            // Tìm tài khoản từ cơ sở dữ liệu để cập nhật
            var existingAccount = await _context.Accounts.FindAsync(int.Parse(accountId));

            if (existingAccount == null)
            {
                return NotFound(); // Nếu không tìm thấy tài khoản
            }

            // Cập nhật thông tin tài khoản từ form
            existingAccount.FullName = Account.FullName;
            existingAccount.Dob = Account.Dob;
            existingAccount.Email = Account.Email;
            existingAccount.UseName = Account.UseName;
            existingAccount.Password = Account.Password;
            existingAccount.Phonenumber = Account.Phonenumber;
            existingAccount.Gender = Account.Gender;
            existingAccount.Address = Account.Address;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return RedirectToPage("/Users/Profile"); // Quay lại trang profile sau khi cập nhật
        }
    }
}
