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
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<ReviewImage> ReviewImages { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomImage> RoomImages { get; set; } = null!;
        public virtual DbSet<RoomService> RoomServices { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<ServiceImage> ServiceImages { get; set; } = null!;
        public virtual DbSet<TypeRoom> TypeRooms { get; set; } = null!;

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

                entity.HasIndex(e => e.Email, "UQ__Account__A9D10534007C6C71")
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
                    .HasConstraintName("FK__Account__Status__3D5E1FD2");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.Discount).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(50)
                    .HasColumnName("Order_Status");

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
                    .HasName("PK__Order_De__3D0CE883AAA3FA0A");

                entity.ToTable("Order_Details");

                entity.Property(e => e.OdId).HasColumnName("OD_ID");

                entity.Property(e => e.CheckIn)
                    .HasColumnType("date")
                    .HasColumnName("Check_In");

                entity.Property(e => e.CheckOut)
                    .HasColumnType("date")
                    .HasColumnName("Check_Out");

                entity.Property(e => e.OrderId).HasColumnName("Order_ID");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Order_Det__Order__534D60F1");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Order_Det__Room___5441852A");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");

                entity.Property(e => e.ReviewId).HasColumnName("Review_ID");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.OrderId).HasColumnName("Order_ID");

                entity.Property(e => e.Rating).HasColumnType("decimal(2, 1)");

                entity.Property(e => e.ReviewDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Review__Account___59FA5E80");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Review__Order_ID__59063A47");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Review__Room_ID__5812160E");
            });

            modelBuilder.Entity<ReviewImage>(entity =>
            {
                entity.ToTable("Review_Image");

                entity.Property(e => e.ReviewImageId).HasColumnName("Review_Image_ID");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.ReviewId).HasColumnName("Review_ID");

                entity.HasOne(d => d.Review)
                    .WithMany(p => p.ReviewImages)
                    .HasForeignKey(d => d.ReviewId)
                    .HasConstraintName("FK__Review_Im__Revie__5CD6CB2B");
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

                entity.Property(e => e.NumberOfAdult).HasColumnName("Number_Of_Adult");

                entity.Property(e => e.NumberOfBed).HasColumnName("Number_Of_Bed");

                entity.Property(e => e.NumberOfChild).HasColumnName("Number_Of_Child");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

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
                    .HasConstraintName("FK__Room__Type_ID__46E78A0C");
            });

            modelBuilder.Entity<RoomImage>(entity =>
            {
                entity.ToTable("Room_Image");

                entity.Property(e => e.RoomImageId).HasColumnName("Room_Image_Id");

                entity.Property(e => e.ImageIndex).HasColumnName("Image_Index");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomImages)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Room_Imag__Room___49C3F6B7");
            });

            modelBuilder.Entity<RoomService>(entity =>
            {
                entity.ToTable("Room_Service");

                entity.Property(e => e.RoomServiceId).HasColumnName("Room_Service_ID");

                entity.Property(e => e.RoomId).HasColumnName("Room_ID");

                entity.Property(e => e.ServiceId).HasColumnName("Service_ID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomServices)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Room_Serv__Room___4CA06362");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.RoomServices)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Room_Serv__Servi__4D94879B");
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

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("Update_Date");
            });

            modelBuilder.Entity<ServiceImage>(entity =>
            {
                entity.ToTable("Service_Image");

                entity.Property(e => e.ServiceImageId).HasColumnName("Service_Image_ID");

                entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");

                entity.Property(e => e.ServiceId).HasColumnName("Service_ID");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceImages)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Service_I__Servi__4222D4EF");
            });

            modelBuilder.Entity<TypeRoom>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__Type_Roo__FE90DDFE3740FB01");

                entity.ToTable("Type_Room");

                entity.Property(e => e.TypeId).HasColumnName("Type_ID");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(100)
                    .HasColumnName("Type_Name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
