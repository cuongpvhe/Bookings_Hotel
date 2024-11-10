using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using CloudinaryDotNet.Actions;
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
        public TypeRoomDTO typeRoomDTOGet { get; set; }



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
                return RedirectToPage("/Home/Login", new { returnUrl = "/Booking?id=" + id });
            }

            var account = await _context.Accounts.FindAsync(int.Parse(accountId));
            if (account == null)
            {
                return RedirectToPage("/Home/Login", new { returnUrl = "/Booking?id=" + id });
            }

            //Process
            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == id);

            if (typeRoom == null)
            {
                return NotFound("Not Found ID");
            }

            //Get Service
            var lstServiceName = _context.TypeRoomServices
                    .Where(trs => trs.TypeId == typeRoom.TypeId)
                    .Join(_context.Services,
                        trs => trs.ServiceId,
                        s => s.ServiceId,
                        (trs, s) => s.ServiceName) 
            .ToList();

            //Get Image
            var lstImage = _context.TypeRoomImages
                .Where(tri => tri.TypeId == typeRoom.TypeId)
                .OrderBy(tri => tri.ImageIndex)
                .Select(tri => tri.ImageUrl)
                .ToList();

            typeRoomDTOGet = new TypeRoomDTO {
                TypeId = typeRoom.TypeId,
                TypeName = typeRoom.TypeName,
                NumberOfChild = typeRoom.NumberOfChild,
                NumberOfAdult = typeRoom.NumberOfAdult,
                NumberOfBed = typeRoom.NumberOfBed,
                Price = typeRoom.Price,
                PriceString = typeRoom.Price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                PriceVATString = (typeRoom.Price * 1.1m).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                LstService = lstServiceName,
                MaximumExtraAdult = typeRoom.MaximumExtraAdult,
                MaximumExtraChild = typeRoom.MaximumExtraChild,
                ExtraAdultFee = typeRoom.ExtraAdultFee,
                ExtraChildFee = typeRoom.ExtraChildFee,
                ExtraAdultFeeString = ((decimal)typeRoom.ExtraAdultFee).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                ExtraChildFeeString = ((decimal)typeRoom.ExtraChildFee).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                LstImage = lstImage,
            };



            return Page();
        }


        public async Task<IActionResult> OnPostSubmitOrder(string CheckInDate, string CheckOutDate, string? SpecialRequest,int TypeId,int NumberOfAdult, int NumberOfChild)
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

            //Convert Date
            if (!DateTime.TryParse(CheckInDate, out DateTime checkinDate))
            {
                return BadRequest("Invalid Check-In Date format.");
            }

            if (!DateTime.TryParse(CheckOutDate, out DateTime checkoutDate))
            {
                return BadRequest("Invalid Check-Out Date format.");
            }

            //Get Type by TypeID(from header request)
            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == TypeId);
            if (typeRoom == null)
            {
                return BadRequest("Room not found");
            }

            //Get {quantity} Valid Room
            var lstRoom = this.getValidLstRoom(typeRoom,checkinDate,checkoutDate);

            if (!lstRoom.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Tất Cả Các Phòng Đã Được Thuê. Vui Lòng Chọn Ngày Khác",
                    data = lstRoom.Count
                });
            }

            //Caculate Total Money
            var extraAdultNumber = NumberOfAdult - typeRoom.NumberOfAdult < 0 ? 0 : NumberOfAdult - typeRoom.NumberOfAdult;
            var extraChildNumber = NumberOfChild - typeRoom.NumberOfChild < 0 ? 0 : NumberOfChild - typeRoom.NumberOfChild;

            var numberOfNights = (checkoutDate - checkinDate).Days;
            if (numberOfNights <= 0)
            {
                throw new ArgumentException("Check-out date must be later than check-in date.");
            }

            decimal? totalMoney = (typeRoom.Price * numberOfNights + extraAdultNumber * typeRoom.ExtraAdultFee + extraChildNumber * typeRoom.ExtraChildFee) * 1.1m; //1.1 VAT
            if(totalMoney == null)
            {
                throw new ArgumentException("Can't caculate Total Money.");
            }

            //Create Order
            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalMoney = (decimal)totalMoney,
                Discount = 0,
                OrderStatus = OrderStatus.WAITING_PAYMENT,
                AccountId = int.Parse(accountId), 
                Note = SpecialRequest,
                PaymentCode = GenerateRandomPaymentCode(),
                NumberExtraAdult = extraAdultNumber,
                NumberExtraChild = extraChildNumber,
            };

            // Add Order to the context
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Create the OrderDetails and link it to the order
            var orderDetails = new OrderDetail
            {
                RoomId = lstRoom.First().RoomId,
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

        public List<Room> getValidLstRoom(TypeRoom typeRoom, DateTime checkinDate, DateTime checkoutDate)
        {
            //Get Valid Room By TypeID
            return _context.Rooms
                .Where(r => r.TypeId == typeRoom.TypeId)
                .Where(r => r.RoomStatus.Equals(RoomStatus.ACTIVE))
                .Where(room => !_context.OrderDetails.Any(
                    od => od.RoomId == room.RoomId &&
                   ((checkinDate >= od.CheckIn && checkinDate < od.CheckOut) ||
                    (checkoutDate > od.CheckIn && checkoutDate <= od.CheckOut) ||
                    (checkinDate <= od.CheckIn && checkoutDate >= od.CheckOut))))
                .OrderBy(room => room.RoomNumber)
                .ToList();
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

        public async Task<IActionResult> OnPostBatchUpdateOrderStatus()
        {
            List<Order> lstOrder = _context.Orders
                .Where(o => o.OrderStatus.Equals(OrderStatus.WAITING_PAYMENT))
            .ToList();

            lstOrder.ForEach(order =>
            {
                var timeSinceOrderPlaced = DateTime.Now - order.OrderDate;
                if (timeSinceOrderPlaced.TotalMinutes > 1)
                {
                    // Update the order status to "Canceled"
                    order.OrderStatus = OrderStatus.CANCEL;

                    // Save the changes to the database
                    _context.Orders.Update(order);
                    _context.SaveChangesAsync();
                }
            });
            
            

            return new JsonResult(new
            {
                success = true,
                message = "Udate successfully!"
            });
        }
    }
}
