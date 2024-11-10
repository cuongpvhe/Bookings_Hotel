using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bookings_Hotel.Models
{
    public partial class HotelBookingSystemContext : DbContext
    {
        public HotelBookingSystemContext()
        {
        }

        public HotelBookingSystemContext(DbContextOptions<HotelBookingSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FeedbackImage> FeedbackImages { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceImage> ServiceImages { get; set; } = null!;
        public virtual DbSet<TypeRoom> TypeRooms { get; set; } = null!;
        public virtual DbSet<TypeRoomImage> TypeRoomImages { get; set; } = null!;
        public virtual DbSet<TypeRoomService> TypeRoomServices { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDB"));

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "UQ__Account__A9D10534DCDD567B")
                    .IsUnique();

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Active')");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UseName).HasMaxLength(100);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__ROLE_ID__5165187F");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.ReviewId)
                    .HasName("PK__Feedback__F85DA7EBCD0BD752");

                entity.ToTable("Feedback");

                entity.Property(e => e.ReviewId).HasColumnName("Review_ID");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.OrderId).HasColumnName("Order_ID");

                entity.Property(e => e.Rating).HasColumnType("decimal(2, 1)");

                entity.Property(e => e.ReviewDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Feedback__Accoun__52593CB8");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Review__Order_ID__5812160E");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Feedback__Room_I__534D60F1");
            });

            modelBuilder.Entity<FeedbackImage>(entity =>
            {
                entity.HasKey(e => e.ReviewImageId)
                    .HasName("PK__Feedback__E961B89CE2BCDCD7");

                entity.ToTable("Feedback_Image");

                entity.Property(e => e.ReviewImageId).HasColumnName("Review_Image_ID");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.ReviewId).HasColumnName("Review_ID");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.FeedbackImages)
                    .HasForeignKey(d => d.ReviewId)
                    .HasConstraintName("FK__Feedback___Revie__5535A963");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.Property(e => e.NumberExtraAdult).HasColumnName("Number_Extra_Adult");

                entity.Property(e => e.NumberExtraChild).HasColumnName("Number_Extra_Child");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(50)
                    .HasColumnName("Order_Status");

                entity.Property(e => e.PaymentCode).HasMaxLength(50);

                entity.Property(e => e.TotalMoney)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Total_Money");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Order__Account_I__5070F446");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OdId)
                    .HasName("PK__Order_De__3D0CE883B86F41ED");

                entity.ToTable("Order_Details");

                entity.Property(e => e.OdId).HasColumnName("OD_ID");

                entity.Property(e => e.CheckIn)
                    .HasColumnType("date")
                    .HasColumnName("Check_In");

                entity.Property(e => e.CheckOut)
                    .HasColumnType("date")
                    .HasColumnName("Check_Out");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Order_Details_Order");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Order_Det__Room___534D60F1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("Role_ID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .HasColumnName("Role_Name");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.RoomNumber).HasColumnName("Room_Number");

                entity.Property(e => e.RoomStatus)
                    .HasMaxLength(50)
                    .HasColumnName("Room_Status");

                entity.Property(e => e.TypeId).HasColumnName("Type_ID");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("Update_Date");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__Room__Type_ID__59063A47");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.ServiceId).HasColumnName("Service_ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ServiceName)
                    .HasMaxLength(100)
                    .HasColumnName("Service_Name");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("Update_Date");
            });

            modelBuilder.Entity<ServiceImage>(entity =>
            {
                entity.ToTable("Service_Image");

                entity.Property(e => e.ServiceImageId).HasColumnName("Service_Image_ID");

                entity.Property(e => e.ImageIndex).HasColumnName("Image_Index");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.ServiceId).HasColumnName("Service_ID");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceImages)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Service_I__Servi__59FA5E80");
            });

            modelBuilder.Entity<TypeRoom>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__Type_Roo__FE90DDFEF21F6595");

                entity.ToTable("Type_Room");

                entity.Property(e => e.TypeId).HasColumnName("Type_ID");

                entity.Property(e => e.ExtraAdultFee)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Extra_Adult_Fee");

                entity.Property(e => e.ExtraChildFee)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Extra_Child_Fee");

                entity.Property(e => e.MaximumExtraAdult).HasColumnName("Maximum_Extra_Adult");

                entity.Property(e => e.MaximumExtraChild).HasColumnName("Maximum_Extra_Child");

                entity.Property(e => e.NumberOfAdult).HasColumnName("Number_Of_Adult");

                entity.Property(e => e.NumberOfBed).HasColumnName("Number_Of_Bed");

                entity.Property(e => e.NumberOfChild).HasColumnName("Number_Of_Child");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(100)
                    .HasColumnName("Type_Name");
            });

            modelBuilder.Entity<TypeRoomImage>(entity =>
            {
                entity.ToTable("Type_Room_Image");

                entity.Property(e => e.TypeRoomImageId).HasColumnName("Type_Room_Image_Id");

                entity.Property(e => e.ImageIndex).HasColumnName("Image_Index");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.TypeId).HasColumnName("Type_ID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TypeRoomImages)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Type_Room_Image_Type_Room");
            });

            modelBuilder.Entity<TypeRoomService>(entity =>
            {
                entity.HasKey(e => e.TypeServiceId)
                    .HasName("PK__Type_Roo__B51908651B9FA161");

                entity.ToTable("Type_Room_Service");

                entity.Property(e => e.TypeServiceId).HasColumnName("Type_Service_ID");

                entity.Property(e => e.ServiceId).HasColumnName("Service_ID");

                entity.Property(e => e.TypeId).HasColumnName("Type_ID");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.TypeRoomServices)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Type_Room__Servi__5BE2A6F2");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TypeRoomServices)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Type_Room_Service_Type_Room");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
