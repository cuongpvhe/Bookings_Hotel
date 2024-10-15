Create database HotelBookingSystem
use HotelBookingSystem

USE master;
ALTER DATABASE HotelBookingSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
drop DATABASE HotelBookingSystem

-- Tạo bảng Role
CREATE TABLE Role (
    Role_ID INT PRIMARY KEY IDENTITY(1,1),
    Role_Name NVARCHAR(100) NOT NULL
);

-- Tạo bảng Account
CREATE TABLE Account (
    [Account_ID] INT PRIMARY KEY IDENTITY(1,1),
    [ROLE_ID] INT,
    [FullName] NVARCHAR(100) NOT NULL,
    [DOB] DATE,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
	[UseName] NVARCHAR(100) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
	[Avatar] NVARCHAR(MAX) NULL,
	[Phonenumber] NVARCHAR(MAX) NULL,
	[Gender] NVARCHAR(MAX) NULL, 
	[Address] NVARCHAR(MAX) NULL,
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [UpdateDate] DATETIME DEFAULT GETDATE(),
    [Status] NVARCHAR(50) DEFAULT 'Active'
    FOREIGN KEY (ROLE_ID) REFERENCES Role(Role_ID)
);

-- Tạo bảng Service
CREATE TABLE Service (
    Service_ID INT PRIMARY KEY IDENTITY(1,1),
    Service_Name NVARCHAR(100) NOT NULL,
    Created_Date DATE NOT NULL,
    Update_Date DATE,
    Price DECIMAL(18,2),
    Description NVARCHAR(MAX)
);

-- Tạo bảng Service_Image
CREATE TABLE Service_Image (
    Service_Image_ID INT PRIMARY KEY IDENTITY(1,1),
    Image_Url NVARCHAR(MAX),
    Service_ID INT,
    FOREIGN KEY (Service_ID) REFERENCES Service(Service_ID)
);

-- Tạo bảng Type
CREATE TABLE Type_Room (
    Type_ID INT PRIMARY KEY IDENTITY(1,1),
    Type_Name NVARCHAR(100) NOT NULL
);

-- Tạo bảng Room
CREATE TABLE Room (
    Room_ID INT PRIMARY KEY IDENTITY(1,1),
    Price DECIMAL(18,2) NOT NULL,
    Type_ID INT,
    Room_Number INT NOT NULL,
    Number_Of_Child INT,
    Number_Of_Adult INT,
    Number_Of_Bed INT,
    Room_Status NVARCHAR(50),
    Description NVARCHAR(MAX),
    Created_Date DATE NOT NULL,
    Update_Date DATE,
    FOREIGN KEY (Type_ID) REFERENCES Type_Room(Type_ID)
);

-- Tạo bảng Room_Image
CREATE TABLE Room_Image (
    Room_Image_Id INT PRIMARY KEY IDENTITY(1,1),
    Image_Url NVARCHAR(MAX),
    Room_ID INT,
	Image_Index INT,
    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID)
);

-- Tạo bảng Room_Service
CREATE TABLE Room_Service (
    Room_Service_ID INT PRIMARY KEY IDENTITY(1,1),
    Room_ID INT,
    Service_ID INT,
    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID),
    FOREIGN KEY (Service_ID) REFERENCES Service(Service_ID)
);

-- Tạo bảng Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    Order_Date DATE NOT NULL,
    Total_Money DECIMAL(18,2) NOT NULL,
    Discount DECIMAL(5,2),
    Order_Status NVARCHAR(50),
    Account_ID INT,
    FOREIGN KEY (Account_ID) REFERENCES Account(Account_ID)
);

-- Tạo bảng Order_Details
CREATE TABLE Order_Details (
    OD_ID INT PRIMARY KEY IDENTITY(1,1),
    Room_ID INT,
    Check_In DATE NOT NULL,
    Check_Out DATE NOT NULL,
    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID)
);

-- Tạo bảng Review
CREATE TABLE Review (
    Review_ID INT PRIMARY KEY IDENTITY(1,1),
    Room_ID INT,
    Order_ID INT,
    Account_ID INT,
	[Rating] DECIMAL(2, 1), 
	[ReviewDate] DATETIME DEFAULT GETDATE(),
    [Comment] NVARCHAR(MAX),
    FOREIGN KEY (Room_ID) REFERENCES Room(Room_ID),
    FOREIGN KEY (Order_ID) REFERENCES [Order](OrderID),
    FOREIGN KEY (Account_ID) REFERENCES Account(Account_ID)
);

-- Tạo bảng Review_Image
CREATE TABLE Review_Image (
    Review_Image_ID INT PRIMARY KEY IDENTITY(1,1),
    Image_Url NVARCHAR(MAX),
    Review_ID INT,
    FOREIGN KEY (Review_ID) REFERENCES Review(Review_ID)
);

-- Thêm dữ liệu vào bảng Role
INSERT INTO Role (Role_Name)
VALUES 
    ('Admin'),
    ('Customer'),
    ('Staff');

-- Thêm dữ liệu vào bảng Account
INSERT INTO Account (ROLE_ID, FullName, DOB, Email, UseName, [Password], Avatar,[Phonenumber], Gender, [Address])
VALUES
    (1, 'Admin', '1985-05-20', 'phamduycuong2k1@gmail.com', 'Admin', '123456', NULL,NULL, 'Male', '123 Main St, New York, NY'),
    (2, 'Customer', '1990-10-15', 'customer@gmail.com', 'Customer', '123456', NULL,NULL, 'Female', '456 Broadway, Los Angeles, CA'),
    (3, 'Staff', '1978-07-12', 'staff@gmail.com', 'Staff', '123456', NULL,NULL, 'Male', '789 Market St, San Francisco, CA');

-- Thêm dữ liệu vào bảng Service
INSERT INTO Service (Service_Name, Created_Date, Update_Date, Price, Description)
VALUES
    ('Spa', GETDATE(), NULL, 50.00, 'Luxury spa treatment'),
    ('Gym', GETDATE(), NULL, 0.00, 'Access to the hotel gym'),
    ('Room Cleaning', GETDATE(), NULL, 20.00, 'Daily room cleaning service');

-- Thêm dữ liệu vào bảng Service_Image
INSERT INTO Service_Image (Image_Url, Service_ID)
VALUES
    ('/images/spa1.jpg', 1),
    ('/images/gym1.jpg', 2),
    ('/images/room_cleaning.jpg', 3);

-- Thêm dữ liệu vào bảng Type_Room
INSERT INTO Type_Room (Type_Name)
VALUES
    ('Single'),
    ('Double'),
    ('Suite');

-- Thêm dữ liệu vào bảng Room
INSERT INTO Room (Price, Type_ID, Room_Number, Number_Of_Child, Number_Of_Adult, Number_Of_Bed, Room_Status, Description, Created_Date, Update_Date)
VALUES
    (100.00, 1, 101, 0, 1, 1, 'Available', 'Single room with one bed', GETDATE(), NULL),
    (150.00, 2, 202, 1, 2, 2, 'Occupied', 'Double room with two beds', GETDATE(), NULL),
    (300.00, 3, 303, 2, 4, 3, 'Available', 'Luxury suite with three beds', GETDATE(), NULL);

-- Thêm dữ liệu vào bảng Room_Image
INSERT INTO Room_Image (Image_Url, Room_ID, Image_Index)
VALUES
    ('/images/room101.jpg', 1, 1),
    ('/images/room202.jpg', 2, 1),
    ('/images/room303.jpg', 3, 1);

-- Thêm dữ liệu vào bảng Room_Service
INSERT INTO Room_Service (Room_ID, Service_ID)
VALUES
    (1, 1), -- Room 101 có dịch vụ Spa
    (2, 2), -- Room 202 có dịch vụ Gym
    (3, 3); -- Room 303 có dịch vụ dọn phòng

-- Thêm dữ liệu vào bảng Order
INSERT INTO [Order] (Order_Date, Total_Money, Discount, Order_Status, Account_ID)
VALUES
    (GETDATE(), 120.00, 10.00, 'Confirmed', 2), -- Khách hàng Jane Smith đặt phòng
    (GETDATE(), 320.00, 20.00, 'Pending', 3);  -- Khách hàng Mark Johnson đặt phòng

-- Thêm dữ liệu vào bảng Order_Details
INSERT INTO Order_Details (Room_ID, Check_In, Check_Out)
VALUES
    (1, '2024-10-01', '2024-10-05'),
    (3, '2024-10-10', '2024-10-15');

-- Thêm dữ liệu vào bảng Review
INSERT INTO Review (Room_ID, Order_ID, Account_ID, Rating, Comment)
VALUES
    (1, 1, 2, 4.5, 'Very comfortable stay!'),
    (3, 2, 3, 5.0, 'Amazing suite, highly recommend!');

-- Thêm dữ liệu vào bảng Review_Image
INSERT INTO Review_Image (Image_Url, Review_ID)
VALUES
    ('/images/review101.jpg', 1),
    ('/images/review303.jpg', 2);
