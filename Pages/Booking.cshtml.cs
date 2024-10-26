using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;
using static Bookings_Hotel.Pages.Manager.RoomsModel;

namespace Bookings_Hotel.Pages
{
    public class BookingModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        public BookingModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public RoomDTO roomDTOGet { get; set; }

        

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //Get parameter
            if (id == null)
            {
                return NotFound();
            }

            //Checklogin
            var accountId = User.FindFirstValue("AccountId"); // Assumes "AccountId" is stored in the claims

            if (string.IsNullOrEmpty(accountId))
            {
                return Redirect("/Login");
            }

            var account = await _context.Accounts.FindAsync(int.Parse(accountId));
            if (account == null)
            {
                return Redirect("/Login");
            }

            //Process
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound("Not found room");
            }

            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == room.TypeId);
            if (typeRoom == null)
            {
                return NotFound("Not found type room");
            }

            roomDTOGet = new RoomDTO { 
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                NumberOfChild = typeRoom.NumberOfChild,
                NumberOfAdult = typeRoom.NumberOfAdult,
                NumberOfBed = typeRoom.NumberOfBed,
                Price = typeRoom.Price,
                PriceString = typeRoom.Price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
            };



            return Page();
        }

        
        public async Task<IActionResult> OnPostSubmitOrder(string CheckInDate, string CheckOutDate, string? SpecialRequest,int RoomId)
        {
            // Checklogin
            var accountId = User.FindFirstValue("AccountId"); // Assumes "AccountId" is stored in the claims

            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized();
            }

            var account = await _context.Accounts.FindAsync(int.Parse(accountId));
            if (account == null)
            {
                return Unauthorized();
            }

            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            //Get Room
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            if (room == null)
            {
                return BadRequest("Room not found");
            }
            //Get Type
            var type = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == room.TypeId);
            if (type == null)
            {
                return BadRequest("Room not found");
            }
            //Convert Date
            if (!DateTime.TryParse(CheckInDate, out DateTime checkinDate))
            {
                return BadRequest("Invalid Check-In Date format.");
            }

            if (!DateTime.TryParse(CheckOutDate, out DateTime checkoutDate))
            {
                return BadRequest("Invalid Check-Out Date format.");
            }

            //Check if the room is booked 
            var conflictingOrderDetails = _context.OrderDetails
                .Where(od => od.RoomId == room.RoomId &&
                             ((checkinDate >= od.CheckIn && checkinDate < od.CheckOut) || // Checkin mới trùng với khoảng thời gian đã đặt
                              (checkoutDate > od.CheckIn && checkoutDate <= od.CheckOut) || // Checkout mới trùng với khoảng thời gian đã đặt
                              (checkinDate <= od.CheckIn && checkoutDate >= od.CheckOut))) // Khoảng thời gian mới bao phủ toàn bộ thời gian đã đặt
                .ToList();
            if (conflictingOrderDetails.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "The room is already booked for the selected dates."
                });
            }

            //Caculate Total Money
            var numberOfNights = (checkoutDate - checkinDate).Days;
            if (numberOfNights <= 0)
            {
                throw new ArgumentException("Check-out date must be later than check-in date.");
            }
            decimal totalMoney = type.Price* numberOfNights;

            //Create Order
            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalMoney = totalMoney,
                Discount = 0,
                OrderStatus = OrderStatus.WAITING_PAYMENT,
                AccountId = int.Parse(accountId), 
                Note = SpecialRequest,
                PaymentCode = GenerateRandomPaymentCode()
            };

            // Add Order to the context
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync(); 

            // Create the OrderDetails and link it to the order
            var orderDetails = new OrderDetail
            {
                RoomId = roomDTOGet.RoomId,
                CheckIn = checkinDate,
                CheckOut = checkoutDate,
                OrderId = newOrder.OrderId, 
            };

            // Add OrderDetail to the context
            _context.OrderDetails.Add(orderDetails);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Trả về kết quả
            return new JsonResult(new
            {
                success = true,
                message = "Received successfully!",
                data = newOrder.OrderId
            });
        }


        public static string GenerateRandomPaymentCode()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] stringChars = new char[8];

            for (int i = 0; i < 8; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
