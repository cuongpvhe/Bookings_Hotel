﻿using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookings_Hotel.Pages.Manager.Booking
{
    public class BookingViewsModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;

        public BookingViewsModel(HotelBookingSystemContext context)
        {
            _context = context;
        }

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

            var orders = _context.Orders
                .Where(o => o.OrderStatus == "Confirmed")
                .Select(o => o.OrderId)
                .ToList();

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
                    OrderId = od.OrderId
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


    }
}