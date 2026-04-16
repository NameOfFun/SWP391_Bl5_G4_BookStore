-- ============================================================
--  SEED DATA — BookStoreDB  (Test Shipper Dashboard)
--  Dùng OUTPUT clause để lấy ID — không dùng TOP+OFFSET
-- ============================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
USE [BookStoreDB];
GO

-- ── 0. Xóa dữ liệu cũ (bỏ comment nếu muốn chạy lại) ────────
-- DELETE FROM OrderDetail;
-- DELETE FROM [Order];
-- DELETE FROM Book;
-- DELETE FROM Category;
-- DELETE FROM Author;
-- DELETE FROM AspNetUserRoles WHERE UserId IN (
--     SELECT Id FROM AspNetUsers WHERE UserName IN ('shipper01','customer01','customer02'));
-- DELETE FROM AspNetUsers WHERE UserName IN ('shipper01','customer01','customer02');
-- GO

-- ============================================================
-- BƯỚC 1: Tạo Users (Shipper + 2 Customer)
-- ============================================================
-- Password hash bên dưới là hash Identity v3 cho chuỗi "Test@123"
-- Được generate bằng app .NET — hợp lệ với ASP.NET Core Identity
DECLARE @PwHash NVARCHAR(MAX) =
    N'AQAAAAIAAYagAAAAEE5bujVaKUb+JJe/5L9skZVEbA8kRs+nqVbCX7klkjH4+4MYuBDqFT8Y7R5pIkFNcA==';

DECLARE @ShipperRoleId  NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE [Name] = 'Shipper');
DECLARE @CustomerRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE [Name] = 'Customer');

-- Bảng tạm chứa ID
DECLARE @UserIds TABLE (Tag NVARCHAR(20), Id NVARCHAR(450));

-- Chỉ insert nếu chưa tồn tại
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = 'shipper01')
BEGIN
    DECLARE @sid NVARCHAR(450) = NEWID();
    INSERT INTO AspNetUsers
        (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
         EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
         PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (@sid, 'shipper01','SHIPPER01','shipper01@bookstore.vn','SHIPPER01@BOOKSTORE.VN',
            1, @PwHash, NEWID(), NEWID(), 0, 0, 1, 0);
    INSERT INTO @UserIds VALUES ('shipper', @sid);
    INSERT INTO AspNetUserRoles VALUES (@sid, @ShipperRoleId);
END
ELSE
    INSERT INTO @UserIds SELECT 'shipper', Id FROM AspNetUsers WHERE UserName='shipper01';

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = 'customer01')
BEGIN
    DECLARE @cid1 NVARCHAR(450) = NEWID();
    INSERT INTO AspNetUsers
        (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
         EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
         PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (@cid1,'customer01','CUSTOMER01','customer01@bookstore.vn','CUSTOMER01@BOOKSTORE.VN',
            1, @PwHash, NEWID(), NEWID(), 0, 0, 1, 0);
    INSERT INTO @UserIds VALUES ('cust1', @cid1);
    INSERT INTO AspNetUserRoles VALUES (@cid1, @CustomerRoleId);
END
ELSE
    INSERT INTO @UserIds SELECT 'cust1', Id FROM AspNetUsers WHERE UserName='customer01';

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = 'customer02')
BEGIN
    DECLARE @cid2 NVARCHAR(450) = NEWID();
    INSERT INTO AspNetUsers
        (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
         EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
         PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (@cid2,'customer02','CUSTOMER02','customer02@bookstore.vn','CUSTOMER02@BOOKSTORE.VN',
            1, @PwHash, NEWID(), NEWID(), 0, 0, 1, 0);
    INSERT INTO @UserIds VALUES ('cust2', @cid2);
    INSERT INTO AspNetUserRoles VALUES (@cid2, @CustomerRoleId);
END
ELSE
    INSERT INTO @UserIds SELECT 'cust2', Id FROM AspNetUsers WHERE UserName='customer02';

DECLARE @ShipperId  NVARCHAR(450) = (SELECT Id FROM @UserIds WHERE Tag='shipper');
DECLARE @Cust1Id    NVARCHAR(450) = (SELECT Id FROM @UserIds WHERE Tag='cust1');
DECLARE @Cust2Id    NVARCHAR(450) = (SELECT Id FROM @UserIds WHERE Tag='cust2');

PRINT 'Shipper ID : ' + ISNULL(@ShipperId,'NULL');
PRINT 'Customer1  : ' + ISNULL(@Cust1Id,'NULL');
PRINT 'Customer2  : ' + ISNULL(@Cust2Id,'NULL');

-- ============================================================
-- BƯỚC 2: Author → Category → Book
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Author WHERE [Name]=N'Nguyễn Nhật Ánh')
    INSERT INTO Author ([Name],Bio,IsActive) VALUES (N'Nguyễn Nhật Ánh',N'Nhà văn nổi tiếng với nhiều tác phẩm thiếu nhi.',1);
IF NOT EXISTS (SELECT 1 FROM Author WHERE [Name]=N'Tô Hoài')
    INSERT INTO Author ([Name],Bio,IsActive) VALUES (N'Tô Hoài',N'Tác giả của Dế Mèn Phiêu Lưu Ký.',1);
IF NOT EXISTS (SELECT 1 FROM Author WHERE [Name]=N'Nam Quốc Chánh')
    INSERT INTO Author ([Name],Bio,IsActive) VALUES (N'Nam Quốc Chánh',N'Tác giả trẻ viết về kinh doanh.',1);

IF NOT EXISTS (SELECT 1 FROM Category WHERE [Name]=N'Văn học')
    INSERT INTO Category ([Name],Description,IsActive) VALUES (N'Văn học',N'Tiểu thuyết, truyện ngắn, thơ ca.',1);
IF NOT EXISTS (SELECT 1 FROM Category WHERE [Name]=N'Thiếu nhi')
    INSERT INTO Category ([Name],Description,IsActive) VALUES (N'Thiếu nhi',N'Sách dành cho trẻ em.',1);
IF NOT EXISTS (SELECT 1 FROM Category WHERE [Name]=N'Kinh doanh')
    INSERT INTO Category ([Name],Description,IsActive) VALUES (N'Kinh doanh',N'Sách về kinh tế, quản trị.',1);

DECLARE @A1 INT = (SELECT AuthorId FROM Author WHERE [Name]=N'Nguyễn Nhật Ánh');
DECLARE @A2 INT = (SELECT AuthorId FROM Author WHERE [Name]=N'Tô Hoài');
DECLARE @A3 INT = (SELECT AuthorId FROM Author WHERE [Name]=N'Nam Quốc Chánh');
DECLARE @CL INT = (SELECT CategoryId FROM Category WHERE [Name]=N'Văn học');
DECLARE @CK INT = (SELECT CategoryId FROM Category WHERE [Name]=N'Thiếu nhi');
DECLARE @CB INT = (SELECT CategoryId FROM Category WHERE [Name]=N'Kinh doanh');

IF NOT EXISTS (SELECT 1 FROM Book WHERE Title=N'Tôi thấy hoa vàng trên cỏ xanh')
    INSERT INTO Book (Title,Price,Stock,CategoryId,AuthorId,IsActive,CreatedAt)
    VALUES (N'Tôi thấy hoa vàng trên cỏ xanh',85000,50,@CL,@A1,1,GETDATE());
IF NOT EXISTS (SELECT 1 FROM Book WHERE Title=N'Mắt biếc')
    INSERT INTO Book (Title,Price,Stock,CategoryId,AuthorId,IsActive,CreatedAt)
    VALUES (N'Mắt biếc',79000,40,@CL,@A1,1,GETDATE());
IF NOT EXISTS (SELECT 1 FROM Book WHERE Title=N'Dế Mèn Phiêu Lưu Ký')
    INSERT INTO Book (Title,Price,Stock,CategoryId,AuthorId,IsActive,CreatedAt)
    VALUES (N'Dế Mèn Phiêu Lưu Ký',55000,60,@CK,@A2,1,GETDATE());
IF NOT EXISTS (SELECT 1 FROM Book WHERE Title=N'Khởi nghiệp tinh gọn')
    INSERT INTO Book (Title,Price,Stock,CategoryId,AuthorId,IsActive,CreatedAt)
    VALUES (N'Khởi nghiệp tinh gọn',120000,30,@CB,@A3,1,GETDATE());

DECLARE @B1 INT = (SELECT BookId FROM Book WHERE Title=N'Tôi thấy hoa vàng trên cỏ xanh');
DECLARE @B2 INT = (SELECT BookId FROM Book WHERE Title=N'Mắt biếc');
DECLARE @B3 INT = (SELECT BookId FROM Book WHERE Title=N'Dế Mèn Phiêu Lưu Ký');
DECLARE @B4 INT = (SELECT BookId FROM Book WHERE Title=N'Khởi nghiệp tinh gọn');

PRINT 'Book IDs: ' + CAST(@B1 AS VARCHAR) + ', ' + CAST(@B2 AS VARCHAR) + ', ' + CAST(@B3 AS VARCHAR) + ', ' + CAST(@B4 AS VARCHAR);

-- ============================================================
-- BƯỚC 3: Orders — dùng OUTPUT để lấy OrderId ngay lập tức
--   Status:  Shipped=3 | Delivered=4 | Cancelled=5
-- ============================================================
-- Bảng tạm lưu OrderId vừa insert
DECLARE @Ids TABLE (Tag NVARCHAR(10), OrderId INT);

-- ── Chỉ seed nếu shipper chưa có đơn ────────────────────────
IF EXISTS (SELECT 1 FROM [Order] WHERE ShipperId = @ShipperId)
BEGIN
    PRINT 'Shipper đã có đơn hàng — bỏ qua bước insert Order.';
    GOTO Done;
END

-- ── SHIPPED (4 đơn đang giao) ────────────────────────────────
INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust1Id,@ShipperId,DATEADD(HOUR,-26,GETDATE()),3,
     85000,0,85000,N'Nguyễn Văn An','0901234567',
     N'123 Nguyễn Huệ, Quận 1, TP.HCM','COD','Pending');
UPDATE @Ids SET Tag='S1' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust2Id,@ShipperId,DATEADD(HOUR,-14,GETDATE()),3,
     164000,6000,158000,N'Trần Thị Bình','0912345678',
     N'456 Lê Lợi, Quận 3, TP.HCM','COD','Pending');
UPDATE @Ids SET Tag='S2' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust1Id,@ShipperId,DATEADD(HOUR,-6,GETDATE()),3,
     120000,10000,110000,N'Lê Minh Cường','0923456789',
     N'789 Trần Hưng Đạo, Quận 5, TP.HCM','VNPay','Paid');
UPDATE @Ids SET Tag='S3' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust2Id,@ShipperId,DATEADD(HOUR,-2,GETDATE()),3,
     55000,0,55000,N'Phạm Thu Hà','0934567890',
     N'321 Bùi Viện, Quận 1, TP.HCM','COD','Pending');
UPDATE @Ids SET Tag='S4' WHERE Tag IS NULL OR Tag='';

-- ── DELIVERED (3 đơn thành công) ─────────────────────────────
INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust1Id,@ShipperId,DATEADD(DAY,-3,GETDATE()),4,
     205000,21000,184000,N'Hoàng Văn Đức','0945678901',
     N'654 Phan Xích Long, Phú Nhuận, TP.HCM','COD','Paid');
UPDATE @Ids SET Tag='D1' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust2Id,@ShipperId,DATEADD(DAY,-2,GETDATE()),4,
     79000,0,79000,N'Vũ Ngọc Lan','0956789012',
     N'987 Nguyễn Đình Chiểu, Quận 3, TP.HCM','VNPay','Paid');
UPDATE @Ids SET Tag='D2' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust1Id,@ShipperId,DATEADD(DAY,-1,GETDATE()),4,
     175000,15000,160000,N'Đặng Bảo Ngọc','0967890123',
     N'147 Đinh Tiên Hoàng, Bình Thạnh, TP.HCM','COD','Paid');
UPDATE @Ids SET Tag='D3' WHERE Tag IS NULL OR Tag='';

-- ── CANCELLED (2 đơn thất bại) ───────────────────────────────
INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust2Id,@ShipperId,DATEADD(DAY,-5,GETDATE()),5,
     85000,0,85000,N'Ngô Thị Mai','0978901234',
     N'258 Cách Mạng Tháng 8, Quận 10, TP.HCM','COD','Pending');
UPDATE @Ids SET Tag='C1' WHERE Tag IS NULL OR Tag='';

INSERT INTO [Order]
    (UserId,ShipperId,OrderDate,Status,SubTotal,DiscountAmount,GrandTotal,
     ShippingName,ShippingPhone,ShippingAddress,PaymentMethod,PaymentStatus)
OUTPUT INSERTED.OrderId INTO @Ids(OrderId)
VALUES
    (@Cust1Id,@ShipperId,DATEADD(DAY,-4,GETDATE()),5,
     120000,0,120000,N'Lý Thành Nam','0989012345',
     N'369 Tô Hiến Thành, Quận 10, TP.HCM','COD','Pending');
UPDATE @Ids SET Tag='C2' WHERE Tag IS NULL OR Tag='';

-- ===========================================================
-- BƯỚC 4: OrderDetail — dùng ID từ @Ids
-- ============================================================
DECLARE @S1 INT=(SELECT OrderId FROM @Ids WHERE Tag='S1');
DECLARE @S2 INT=(SELECT OrderId FROM @Ids WHERE Tag='S2');
DECLARE @S3 INT=(SELECT OrderId FROM @Ids WHERE Tag='S3');
DECLARE @S4 INT=(SELECT OrderId FROM @Ids WHERE Tag='S4');
DECLARE @D1 INT=(SELECT OrderId FROM @Ids WHERE Tag='D1');
DECLARE @D2 INT=(SELECT OrderId FROM @Ids WHERE Tag='D2');
DECLARE @D3 INT=(SELECT OrderId FROM @Ids WHERE Tag='D3');
DECLARE @C1 INT=(SELECT OrderId FROM @Ids WHERE Tag='C1');
DECLARE @C2 INT=(SELECT OrderId FROM @Ids WHERE Tag='C2');

INSERT INTO OrderDetail (OrderId,BookId,Quantity,UnitPrice) VALUES
-- Shipped
(@S1, @B1, 1, 85000),
(@S2, @B1, 1, 85000),
(@S2, @B2, 1, 79000),
(@S3, @B4, 1, 120000),
(@S4, @B3, 1, 55000),
-- Delivered
(@D1, @B1, 1, 85000),
(@D1, @B4, 1, 120000),
(@D2, @B2, 1, 79000),
(@D3, @B1, 1, 85000),
(@D3, @B3, 2, 55000),
-- Cancelled
(@C1, @B1, 1, 85000),
(@C2, @B4, 1, 120000);

Done:
-- ============================================================
-- BƯỚC 5: Xác nhận kết quả
-- ============================================================
PRINT '';
PRINT '===== KẾT QUẢ =====';

SELECT u.UserName, r.[Name] AS [Role], u.Email
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id=ur.UserId
JOIN AspNetRoles r ON ur.RoleId=r.Id
WHERE u.UserName IN ('shipper01','customer01','customer02')
ORDER BY r.[Name];

SELECT
    o.OrderId,
    CASE o.Status
        WHEN 3 THEN N'Shipped  → Đang giao ⏳'
        WHEN 4 THEN N'Delivered → Thành công ✅'
        WHEN 5 THEN N'Cancelled → Thất bại  ❌'
        ELSE CAST(o.Status AS NVARCHAR)
    END AS [Trạng thái],
    o.ShippingName  AS [Khách hàng],
    o.GrandTotal    AS [Tổng tiền],
    DATEDIFF(HOUR,o.OrderDate,GETDATE()) AS [Giờ trước]
FROM [Order] o
WHERE o.ShipperId = (SELECT Id FROM AspNetUsers WHERE UserName='shipper01')
ORDER BY o.Status, o.OrderDate;

PRINT '';
PRINT '===== DASHBOARD SUMMARY =====';
SELECT
    SUM(CASE WHEN o.Status=3 THEN 1 ELSE 0 END) AS [Assigned_Shipping],
    SUM(CASE WHEN o.Status=4 THEN 1 ELSE 0 END) AS [Delivered_OK],
    SUM(CASE WHEN o.Status=5 THEN 1 ELSE 0 END) AS [Cancelled_Failed]
FROM [Order] o
WHERE o.ShipperId=(SELECT Id FROM AspNetUsers WHERE UserName='shipper01');

PRINT 'Đăng nhập: shipper01 / Test@123';
PRINT 'URL: /Shipper/Dashboard';
GO
