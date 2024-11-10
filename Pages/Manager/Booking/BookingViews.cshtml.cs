﻿using Bookings_Hotel.DTO;
using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Bookings_Hotel.Pages.Manager.Booking
{
    [Authorize(Policy = "StaffOnly")]
    public class BookingViewsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public BookingViewsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        [ValidateNever]
        public TypeRoomDTO typeRoomDTO { get; set; }
        [BindProperty]
        public int RoomId { get; set; }
        [BindProperty]
        public List<SelectListItem> Floors { get; set; }
        public int SelectedFloor { get; set; }


        public void OnGet()
        {
            Floors = _context.Rooms
                .Select(r => r.Floor)
                .Distinct()
                .OrderBy(floorId => floorId)
                .Select(floorId => new SelectListItem
                {
                    Value = floorId.ToString(),
                    Text = "Tầng " + floorId
                }).ToList();
        }


        public JsonResult OnGetGetRoomsByFloor(int? floorId)
        {
            if (floorId == null)
            {
                return new JsonResult(new List<object>());
            }

            var rooms = _context.Rooms
                .Where(r => r.Floor == floorId.Value)
                .Select(r => new
                {
                    roomId = r.RoomId,
                    roomNumber = r.RoomNumber
                }).ToList();

            return new JsonResult(rooms);
        }


        public JsonResult OnGetGetBookingStatus(int roomId, int? month = null, int? year = null)
        {
            // Set current month and year if not provided
            month ??= DateTime.Now.Month;
            year ??= DateTime.Now.Year;

            // Fetch orders with status SUCCESS or WAITING_PAYMENT
            var orders = _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.SUCCESS || o.OrderStatus == OrderStatus.WAITING_PAYMENT)
                .Select(o => o.OrderId)
                .ToList();

            // Retrieve booking dates including waiting payment orders
            var bookedDates = _context.OrderDetails
                .Where(od => od.RoomId == roomId &&
                             orders.Contains(od.OrderId ?? 0) &&
                             ((od.CheckIn.Year == year && od.CheckIn.Month <= month && od.CheckOut.Month >= month && od.CheckOut.Year == year) ||
                              (od.CheckIn.Year == year && od.CheckIn.Month == month) ||
                              (od.CheckOut.Year == year && od.CheckOut.Month == month)))
                .Select(od => new
                {
                    CheckIn = od.CheckIn,
                    CheckOut = od.CheckOut,
                    OrderId = od.OrderId,
                    OrderStatus = _context.Orders
                        .Where(o => o.OrderId == od.OrderId)
                        .Select(o => o.OrderStatus)
                        .FirstOrDefault()
                })
                .ToList();

            return new JsonResult(bookedDates);
        }


        public JsonResult OnGetGetBookingDetails(int orderId)
        {
            var bookingDetail = _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Select(od => new
                {
                    RoomNumber = od.Room.RoomNumber,
                    RoomType = od.Room.Type.TypeName,
                    OrderId = od.OrderId,
                    UserId = od.Order.AccountId,
                    UserName = od.Order.Account.FullName,
                    CheckIn = od.CheckIn,
                    CheckOut = od.CheckOut,
                    TotalAmount = od.Order.TotalMoney
                })
                .FirstOrDefault();

            return new JsonResult(bookingDetail);
        }
        public async Task<IActionResult> OnGetCreateBookingAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == id);
            if (typeRoom == null)
            {
                return NotFound("Không tìm thấy phòng");
            }

            var typeRoomDTO = new TypeRoomDTO
            {
                TypeId = typeRoom.TypeId,
                TypeName = typeRoom.TypeName,
                NumberOfChild = typeRoom.NumberOfChild,
                NumberOfAdult = typeRoom.NumberOfAdult,
                NumberOfBed = typeRoom.NumberOfBed,
                Price = typeRoom.Price,
                PriceString = typeRoom.Price.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                PriceVATString = (typeRoom.Price * 1.1m).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                MaximumExtraAdult = typeRoom.MaximumExtraAdult,
                MaximumExtraChild = typeRoom.MaximumExtraChild,
                ExtraAdultFee = typeRoom.ExtraAdultFee,
                ExtraChildFee = typeRoom.ExtraChildFee,
                ExtraAdultFeeString = ((decimal)typeRoom.ExtraAdultFee).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
                ExtraChildFeeString = ((decimal)typeRoom.ExtraChildFee).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")),
            };

            typeRoomDTO = typeRoomDTO;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateBooking(string CheckInDate, string CheckOutDate, string? SpecialRequest, int TypeId, int NumberOfAdult, int NumberOfChild)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!DateTime.TryParse(CheckInDate, out DateTime checkinDate))
            {
                return BadRequest("Ngày Check-In không hợp lệ.");
            }

            if (!DateTime.TryParse(CheckOutDate, out DateTime checkout))
            {
                return BadRequest("Ngày Check-Out không hợp lệ.");
            }

            var typeRoom = _context.TypeRooms.FirstOrDefault(tr => tr.TypeId == TypeId);
            if (typeRoom == null)
            {
                return BadRequest("Phòng không tìm thấy");
            }

            var lstRoom = this.getValidLstRoom(typeRoom, checkinDate, checkout);
            if (!lstRoom.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Tất cả các phòng đã được thuê. Vui lòng chọn ngày khác.",
                    data = lstRoom.Count
                });
            }

            var extraAdultNumber = Math.Max(0, (decimal)(NumberOfAdult - typeRoom.NumberOfAdult));
            var extraChildNumber = Math.Max(0, (decimal)(NumberOfChild - typeRoom.NumberOfChild));
            var numberOfNights = (checkout - checkinDate).Days;

            if (numberOfNights <= 0)
            {
                throw new ArgumentException("Ngày check-out phải sau ngày check-in.");
            }

            decimal totalMoney = (decimal)((typeRoom.Price * numberOfNights) + (extraAdultNumber * typeRoom.ExtraAdultFee) + (extraChildNumber * typeRoom.ExtraChildFee));
            totalMoney = totalMoney * 1.1m;

            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalMoney = totalMoney,
                Discount = 0,
                OrderStatus = OrderStatus.WAITING_PAYMENT,
                Note = SpecialRequest,
                PaymentCode = GenerateRandomPaymentCode(),
                NumberExtraAdult = (int?)extraAdultNumber,
                NumberExtraChild = (int?)extraChildNumber,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            var orderDetails = new OrderDetail
            {
                RoomId = lstRoom.First().RoomId,
                CheckIn = checkinDate,
                CheckOut = checkout,
                OrderId = newOrder.OrderId,
            };

            _context.OrderDetails.Add(orderDetails);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "Đặt phòng thành công!",
                data = newOrder.OrderId
            });
        }

        public List<Bookings_Hotel.Models.Room> getValidLstRoom(Bookings_Hotel.Models.TypeRoom typeRoom, DateTime checkinDate, DateTime checkoutDate)
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
    }

}


