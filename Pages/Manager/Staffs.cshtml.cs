using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookings_Hotel.Pages.Manager
{
    public class StaffsModel : PageModel
    {
        public List<string>? TableHeaders { get; set; }
        public List<Staff>? Staffs { get; set; }

        public void OnGet()
        {
            // Kh?i t?o tiêu ?? và danh sách phòng
            TableHeaders = new List<string> { "#", "Username", "Full name", "Gender", "D.O.B", "Phone Number", "Address", "Status", "Actions" };
            Staffs = new List<Staff>
        {
            new Staff
            {
                Id = 1, Username = "john_doe", FullName = "John Doe", Gender = "Male",
                DOB = new DateTime(1990, 1, 15), PhoneNumber = "123456789",
                Address = "123 Elm St", Status = "Active"
            },
            new Staff
            {
                Id = 2, Username = "jane_smith", FullName = "Jane Smith", Gender = "Female",
                DOB = new DateTime(1985, 5, 23), PhoneNumber = "987654321",
                Address = "456 Oak St", Status = "Inactive"
            },
            new Staff
            {
                Id = 3, Username = "mike_jones", FullName = "Mike Jones", Gender = "Male",
                DOB = new DateTime(1992, 10, 10), PhoneNumber = "555666777",
                Address = "789 Pine St", Status = "Active"
            }
        };
            {

        };
        }


        public class Staff
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public DateTime DOB { get; set; } // Date of Birth
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }
        }
    }
}
