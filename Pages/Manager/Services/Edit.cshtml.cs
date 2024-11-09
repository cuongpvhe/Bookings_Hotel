﻿using Bookings_Hotel.Models;
using Bookings_Hotel.Util;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookings_Hotel.Pages.Manager.Services
{
    [Authorize(Policy = "StaffOnly")]
    public class EditModel : PageModel
    {
        private readonly HotelBookingSystemContext _context;
        private readonly Cloudinary _cloudinary;
        public bool IsUpdate { get; set; }
        public Models.Service service { get; set; }
        public List<ServiceImageDTO> ImageDTOs { get; set; }
        public EditModel(HotelBookingSystemContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

   
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                IsUpdate = false;
                return Page();
            }

            IsUpdate = true;
            service = await _context.Services
            .Include(s => s.ServiceImages)
            .FirstOrDefaultAsync(s => s.ServiceId == id);

            if (service == null)
            {
                return NotFound();
            }

            ImageDTOs = service.ServiceImages.Select(img => new ServiceImageDTO
            {
                ImageId = img.ServiceImageId,
                ImageUrl = img.ImageUrl,
                Index = img.ImageIndex

            }).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] List<ServiceImageDTO> imageDTOs)
        {
            var serviceId = int.Parse(Request.Form["ServiceId"]);

            var service = await _context.Services
               .Include(s => s.ServiceImages)
               .FirstOrDefaultAsync(t => t.ServiceId == serviceId);

            if (service == null)
            {
                return NotFound();
            }

            service.ServiceName = Request.Form["ServiceName"];
            service.Price = decimal.Parse(Request.Form["Price"]);
            service.Description = Request.Form["Description"];
            service.Status = Request.Form["Status"];
            service.UpdateDate = System.DateTime.Now;

            _context.Services.Update(service);

            foreach (var imageDTO in imageDTOs)
            {
                if (imageDTO.ImageId != 0)
                {
                    // Existing image
                    var existingImage = await _context.ServiceImages.FirstOrDefaultAsync(img => img.ServiceImageId == imageDTO.ImageId);
                    if (existingImage != null && imageDTO.ImageFile != null && imageDTO.ImageFile.Length > 0)
                    {
                        // Delete the old image from Cloudinary
                        var publicId = existingImage.ImageUrl.Split('/').Last().Split('.').First();
                        await _cloudinary.DestroyAsync(new DeletionParams(publicId));

                        // Upload the new image
                        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(imageDTO.ImageFile.FileName, imageDTO.ImageFile.OpenReadStream()),
                            Folder = "hotel_images"
                        });

                        existingImage.ImageUrl = uploadResult.Url.ToString();
                        existingImage.ImageIndex = imageDTO.Index;
                    }
                }
                else if (imageDTO.ImageFile != null && imageDTO.ImageFile.Length > 0)
                {
                    // New image
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(imageDTO.ImageFile.FileName, imageDTO.ImageFile.OpenReadStream()),
                        Folder = "hotel_images"
                    });

                    var serviceImage = new ServiceImage
                    {
                        ServiceId = service.ServiceId,
                        ImageUrl = uploadResult.Url.ToString(),
                        ImageIndex = imageDTO.Index
                    };

                    _context.ServiceImages.Add(serviceImage);
                }
            }
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
        public async Task<IActionResult> OnPostCheckServiceNameAsync(string serviceName)
        {
            var serviceExited = await _context.Services.AnyAsync(s => s.ServiceName == serviceName && s.Status != ServiceStatus.DELETED);
            return new JsonResult(new { exists = serviceExited });
        }

        public async Task<IActionResult> OnPostDeleteLastImageAsync(int id)
        {
            var image = await _context.ServiceImages.FirstOrDefaultAsync(img => img.ServiceImageId == id);
            if (image == null)
            {
                return NotFound();
            }

            var publicId = image.ImageUrl.Split('/').Last().Split('.').First();

            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            /*            if (deletionResult.Result != "ok")
                        {
                            return BadRequest("Xảy ra lỗi trong quá trình xóa file trên Cloudinary");
                        }*/

            _context.ServiceImages.Remove(image);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Xóa ảnh thành công" });
        }

    }
    public class ServiceImageDTO
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public int Index { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
