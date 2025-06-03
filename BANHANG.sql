USE Master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name='SHOPQUANAO')
    DROP DATABASE SHOPQUANAO
GO
CREATE DATABASE SHOPQUANAO
GO

USE SHOPQUANAO;

-- 1. Bảng Cửa hàng
CREATE TABLE CUAHANG (
    MaCH INT PRIMARY KEY IDENTITY(1,1),
    Ten NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(20),
	Email VARCHAR(100),
    HinhAnh VARCHAR(255),
    DiaChi NVARCHAR(100)
);
GO

-- 2. Bảng Danh mục
CREATE TABLE DANHMUC (
    MaDM INT PRIMARY KEY IDENTITY(1,1),
    Ten NVARCHAR(100) NOT NULL UNIQUE, -- Tên danh mục phải là duy nhất
    MoTa NVARCHAR(255)                   -- Thêm mô tả cho danh mục
);
GO

-- 3. Bảng Mặt hàng
CREATE TABLE MATHANG (
    MaMH INT PRIMARY KEY IDENTITY(1,1),
    Ten NVARCHAR(100) NOT NULL,
    GiaGoc INT DEFAULT 0,
    GiaBan INT DEFAULT 0,
    SoLuong INT DEFAULT 0,
    MoTa NVARCHAR(1000),
    HinhAnh VARCHAR(255),
    MaDM INT NOT NULL FOREIGN KEY REFERENCES DANHMUC(MaDM),
    LuotXem INT DEFAULT 0,
    LuotMua INT DEFAULT 0
);
GO

-- 4. Bảng Người dùng
CREATE TABLE NGUOIDUNG (
    MaND INT PRIMARY KEY IDENTITY(1,1), -- ID người dùng
    Ten NVARCHAR(100) NOT NULL,         -- Tên người dùng
    DienThoai VARCHAR(20),              -- Số điện thoại
    Email NVARCHAR(50),                 -- Email
    MatKhau NVARCHAR(255),              -- Mật khẩu
    LoaiND NVARCHAR(50) CHECK (LoaiND IN (N'Quản lý', N'Nhân viên', N'Khách hàng')) DEFAULT N'Khách hàng' -- Vai trò
);
GO

-- 5. Bảng Địa chỉ
CREATE TABLE DIACHI (
    MaDC INT PRIMARY KEY IDENTITY(1,1),
    MaND INT NOT NULL FOREIGN KEY REFERENCES NGUOIDUNG(MaND),
    ChiTiet NVARCHAR(100) NOT NULL,
    PhuongXa NVARCHAR(50) DEFAULT N'Đông Xuyên',
    QuanHuyen NVARCHAR(50) DEFAULT N'TP. Long Xuyên',
    TinhThanh NVARCHAR(50) DEFAULT N'An Giang',
    MacDinh INT DEFAULT 0
);
GO

-- 6. Bảng Hóa đơn
CREATE TABLE HOADON (
    MaHD INT PRIMARY KEY IDENTITY(1,1),
    Ngay DATETIME DEFAULT GETDATE(),
    TongTien INT DEFAULT 0,
    MaND INT NOT NULL FOREIGN KEY REFERENCES NGUOIDUNG(MaND) ON DELETE CASCADE,
	MaPTTT INT NULL FOREIGN KEY REFERENCES PHUONGTHUCTHANHTOAN(MaPTTT) ON DELETE SET NULL;
    TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN (N'Chờ xác nhận', N'Đang giao', N'Hoàn thành', N'Đã hủy')) DEFAULT N'Chờ xác nhận'
);
GO




-- 7. Bảng Chi tiết hóa đơn
CREATE TABLE CTHOADON (
    MaCTHD INT PRIMARY KEY IDENTITY(1,1),
    MaHD INT NOT NULL FOREIGN KEY REFERENCES HOADON(MaHD),
    MaMH INT NOT NULL FOREIGN KEY REFERENCES MATHANG(MaMH),
    DonGia INT DEFAULT 0,
    SoLuong INT DEFAULT 1,
    ThanhTien AS (DonGia * SoLuong)
);
GO

-- 9. Bảng Đánh giá
CREATE TABLE DANHGIA (
    MaDG INT PRIMARY KEY IDENTITY(1,1),
    MaMH INT NOT NULL FOREIGN KEY REFERENCES MATHANG(MaMH),
    MaND INT NOT NULL FOREIGN KEY REFERENCES NGUOIDUNG(MaND),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    BinhLuan NVARCHAR(500),
    NgayDanhGia DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE KHUYENMAI (
    MaKM INT PRIMARY KEY IDENTITY(1,1),   
    TenKM NVARCHAR(255) NOT NULL,         
    MoTa NVARCHAR(500),                   
    LoaiGiamGia NVARCHAR(10) NOT NULL CHECK (LoaiGiamGia IN (N'PhanTram', N'TienMat')), 
    GiaTriGiam INT NOT NULL,              
    DieuKienApDung INT DEFAULT 0,         
    NgayBatDau DATETIME NOT NULL,         
    NgayKetThuc DATETIME NOT NULL,        
    TrangThai NVARCHAR(10) DEFAULT N'HoatDong' CHECK (TrangThai IN (N'HoatDong', N'HetHan', N'TamDung'))  
);
GO

-- Bảng trung gian để liên kết khuyến mãi với mặt hàng
CREATE TABLE MATHANG_KHUYENMAI (
    MaMH INT NOT NULL,
    MaKM INT NOT NULL,
    CONSTRAINT PK_MATHANG_KM PRIMARY KEY (MaMH, MaKM),
    CONSTRAINT FK_MATHANG_KM FOREIGN KEY (MaMH) REFERENCES MATHANG(MaMH) ON DELETE CASCADE,
    CONSTRAINT FK_KHUYENMAI_KM FOREIGN KEY (MaKM) REFERENCES KHUYENMAI(MaKM) ON DELETE CASCADE
);
GO

CREATE TABLE PHUONGTHUCTHANHTOAN (
    MaPTTT INT PRIMARY KEY IDENTITY(1,1),
    TenPTTT NVARCHAR(50) NOT NULL UNIQUE CHECK (TenPTTT IN (N'Tiền mặt', N'Chuyển khoản', N'Ví điện tử'))
);
GO

ALTER TABLE HOADON
ADD MaPTTT INT NULL FOREIGN KEY REFERENCES PHUONGTHUCTHANHTOAN(MaPTTT) ON DELETE SET NULL;
GO


CREATE TABLE LICHSUMUAHANG (
    MaLS INT PRIMARY KEY IDENTITY(1,1),  
    MaND INT NULL FOREIGN KEY REFERENCES NGUOIDUNG(MaND) ON DELETE SET NULL,  
    MaHD INT NOT NULL FOREIGN KEY REFERENCES HOADON(MaHD) ON DELETE NO ACTION,  
    NgayMua DATETIME DEFAULT GETDATE(),  
    TongTien INT DEFAULT 0,              
    MaPTTT INT NULL FOREIGN KEY REFERENCES PHUONGTHUCTHANHTOAN(MaPTTT) ON DELETE SET NULL,  
    TinhTrang NVARCHAR(50) NOT NULL CHECK (TinhTrang IN (N'Chờ xác nhận', N'Đang giao', N'Hoàn thành', N'Đã hủy')) DEFAULT N'Chờ xác nhận',  
    GhiChu NVARCHAR(255)                 
);
GO

CREATE TABLE THONGKE_MATHANG_BAN (
    MaTK INT PRIMARY KEY IDENTITY(1,1),
    MaMH INT NOT NULL FOREIGN KEY REFERENCES MATHANG(MaMH),
    SoLuongBan INT NOT NULL,
    ThoiGian DATETIME NOT NULL,
    CONSTRAINT CK_ThoiGian CHECK (ThoiGian <= GETDATE()) -- đảm bảo thời gian không quá hiện tại
);
GO
SELECT * FROM THONGKE_MATHANG_BAN WHERE ThoiGian BETWEEN '2024-04-01' AND '2024-04-07';

CREATE TABLE THONGKE_DOANHTHU (
    ID INT PRIMARY KEY IDENTITY(1,1),  -- ID cho bản ghi thống kê
    Nam INT,
    Thang INT,
    PhuongThuc INT,  -- Thêm cột PhuongThuc để liên kết với phương thức thanh toán
    DoanhThu DECIMAL(15,2),  -- Tổng doanh thu cho tháng/năm
    FOREIGN KEY (PhuongThuc) REFERENCES PHUONGTHUCTHANHTOAN(MaPTTT)  -- Khóa ngoại liên kết với bảng phương thức thanh toán
);
GO

--------Thống kê phương thức
INSERT INTO THONGKE_DOANHTHU (Nam, Thang, PhuongThuc, DoanhThu)
SELECT 
    YEAR(hoadon.Ngay) AS Nam,
    MONTH(hoadon.Ngay) AS Thang,
    hoadon.MaPTTT AS PhuongThuc,  -- Thêm MaPTTT vào đây để chèn vào bảng
    SUM(hoadon.TongTien) AS DoanhThu
FROM 
    HOADON hoadon
JOIN 
    PHUONGTHUCTHANHTOAN phuongthucthanhtoan ON hoadon.MaPTTT = phuongthucthanhtoan.MaPTTT
GROUP BY 
    YEAR(hoadon.Ngay),
    MONTH(hoadon.Ngay),
    hoadon.MaPTTT;  -- Sử dụng MaPTTT trong GROUP BY để tính toán chính xác

	




--============================INSERT NỘI DUNG =============================================
--INSERT TABLE CUAHANG
INSERT INTO CUAHANG (Ten, DienThoai, Email, HinhAnh,DiaChi) VALUES (N'TBT', '0384294495', 'TBT@gmail.com', 'Logo.png',N'Long Xuyên, An Giang')

--INSERT TABLE DANHMUC
INSERT INTO DANHMUC (Ten, MoTa) 
VALUES 
    (N'Áo thun', N'Các loại áo thun phong cách và thoải mái'),
    (N'Áo polo', N'Áo polo lịch sự, phù hợp cho nhiều dịp'),
    (N'Áo sơ mi', N'Áo sơ mi công sở và dạo phố'),
    (N'Áo khoác', N'Áo khoác thời trang, giữ ấm mùa đông'),
    (N'Quần jeans', N'Quần jeans bền đẹp, phong cách trẻ trung'),
    (N'Quần kaki', N'Quần kaki lịch sự, phù hợp công sở'),
    (N'Quần short', N'Quần short mát mẻ, thoải mái mùa hè'),
    (N'Quần tây', N'Quần tây lịch sự, phù hợp cho các dịp trang trọng');


INSERT INTO MATHANG (Ten, GiaGoc, GiaBan, SoLuong, MoTa, HinhAnh, MaDM) VALUES  
    (N'Áo thun nam nữ unisex tay lỡ A.R.0.1', 120000, 200000, 100, N'Áo thun nam, áo thun nữ tay lỡ unisex, áo phông cotton form rộng oversize streetwear Phong cách Hàn Quốc', 'Aothun1.jpg', 1),  
    (N'Áo thun nam nữ tay lỡ Angel Devil', 130000, 220000, 120, N'Áo thun nam nữ unisex tay lỡ Angel Devil, áo phông cotton freesize AD-Trend', 'Aothun2.jpg', 1),  
    (N'Áo Polo Unisex Teelab Essential', 150000, 250000, 90, N'Shop chuyên thời trang unisex nam nữ với các sản phẩm nổi bật, áo phông cotton freesize AD-Trend', 'Polo1.jpg', 2),  
    (N'Áo Thun Polo POLOMAN', 180000, 280000, 110, N'Vải được nhuộm màu theo công nghệ cao giúp hạn chế bị phai màu khi giặt giũ hay thường xuyên đi ra ngoài trời.', 'Polo2.jpg', 2),  
    (N'Áo sơ mi nam tay dài JBAGY Pastel', 200000, 320000, 80, N'JBAGY Pastel sơ mi trắng form rộng, vải lụa tăm cao cấp phong cách Hàn Quốc', 'Somi1.jpg', 3),  
    (N'Áo sơ mi ngắn tay form rộng', 190000, 310000, 95, N'Chất liệu: Lụa chéo Hàn Quốc. Công dụng: Chống nhăn, giãn nhẹ, êm ái, mềm mịn và mát da.', 'Somi2.jpg', 3),  
    (N'Áo khoác JULIDO vải nỉ 2 da form boxy', 180000, 300000, 95, N'Cơ bản, Nhiệt đới, Thể thao', 'Aokhoac1.jpg', 4),  
    (N'Áo khoác Nam Nữ, Áo khoác thể thao', 270000, 420000, 85, N'Chất liệu cotton dày mịn, thoáng mát được lựa chọn kỹ lưỡng để phù hợp với mọi hoàn cảnh.', 'Aokhoac2.jpg', 4),  
    (N'Quần Jean Nam Ống Suông Wash Retro Tellme Fashion', 280000, 450000, 60, N'Quần jean nam được làm chất liệu jean mềm cao cấp, co giãn nhẹ và thấm hút mồ hôi giúp cho người dùng thực sự thoải mái, năng động trong quá trình sử dụng', 'jean1.jpg', 5),  
    (N'Quần Jean ống rộng Cạp Chun', 290000, 470000, 75, N'Quần Jean ống rộng Cạp Chun Form Unisex, có dây rút bản to sành điệu phiên bản đặc biệt đã lên kệ nhà MT Group', 'jean2.jpg', 5),  
    (N'Quần Dài Kaki Dáng Baggy Ống Suông Cạp', 220000, 380000, 85, N'Quần Dài Kaki Dáng Baggy Ống Suông Cạp Chun Unisex Nam Nữ, Quần Tây Âu Vải Kaki Mềm Mại QBK', 'Kaki1.jpg', 6),  
    (N'Quần Kaki Double Knee Dinoman & Yumi', 240000, 390000, 70, N'Quần Kaki Double Knee Dinoman & Yumi Chất Liệu Kaki Mềm 300gsm Phong Cách Trẻ Trung - QTH03', 'Kaki2.jpg', 6),  
    (N'Quần Short Sờ tu sy Unisex', 180000, 300000, 95, N'Quần Short Sờ tu sy unisex nam nữ chất cotton cao cấp, phong cách basic, thể thao, mặc thoáng mát', 'Short1.jpg', 7),  
    (N'Kaki Dù PN.Store1993', 190000, 310000, 90, N'Chất liệu vải kaki cao cấp mang lại cảm giác mềm mại khi tiếp xúc với da.', 'Short2.jpg', 7),  
    (N'Quần Tây baggy nam ống côn vải hàn', 260000, 420000, 65, N'Mẫu Quần baggy nam ống côn vải hàn, quần tây âu nam co giãn thời trang OKU thiết kế dáng trơn đơn giản và toát lên vẻ lịch lãm tinh tế, mang đến phong cách thời trang trẻ trung, năng động', 'Qtay1.jpg', 8),  
    (N'Quần Tây Nam Hàn Quốc Ống Côn QTN Fashion', 270000, 430000, 60, N'Quần âu kiểu dáng Slim fit ống côn. Thiết kế cổ điển, lịch lãm với nếp ly vĩnh viễn giúp quần đứng dáng, dễ là ủi. Chất liệu vải chéo mỏng nhẹ thoáng mát, dễ giặt, nhanh khô, dễ bảo quản.', 'Qtay2.jpg', 8);  



--INSERT TABLE NGUOIDUNG
INSERT INTO NGUOIDUNG (Ten, DienThoai, Email, MatKhau, LoaiND)
VALUES 
	(N'Admin', '0123456789', 'admin@123', 'AQAAAAIAAYagAAAAEHV915CVKW9BqZlu3Iy+MVKquwwAOGGnCKwWFVUewL/FPy5jduy7WyHkUTGANhP3wg==', N'Quản lý'),
	(N'Trần Bảo Toàn', '0384294495', 'TBT@gmail.com', 'AQAAAAIAAYagAAAAEHV915CVKW9BqZlu3Iy+MVKquwwAOGGnCKwWFVUewL/FPy5jduy7WyHkUTGANhP3wg==', N'Nhân viên'),
	(N'Khách hàng', '0987654321', 'kh@gmail.com', 'AQAAAAIAAYagAAAAEHV915CVKW9BqZlu3Iy+MVKquwwAOGGnCKwWFVUewL/FPy5jduy7WyHkUTGANhP3wg==', N'Khách hàng');

--INSERT TABLE DIACHI
INSERT INTO DIACHI (MaND, ChiTiet, PhuongXa, QuanHuyen, TinhThanh, MacDinh)
VALUES 
	(2, N'123 Đường ABC', N'Phường 1', N'Quận 1', N'TP.HCM', 1),
	(2, N'456 Đường XYZ', N'Phường 2', N'Quận 2', N'TP.HCM', 0);

-- INSERT TABLE HOADON (Sửa trạng thái hợp lệ)
INSERT INTO HOADON (Ngay, TongTien, MaND, TrangThai)
VALUES 
    (GETDATE(), 100000, 3, N'Chờ xác nhận'),
    (GETDATE(), 50000, 3, N'Đang giao');


--INSERT TABLE CTHOADON
INSERT INTO CTHOADON (MaHD, MaMH, DonGia, SoLuong)
VALUES 
	(1, 1, 10000, 5),
	(1, 2, 30000, 1);

--INSERT TABLE DANHGIA
INSERT INTO DANHGIA (MaMH, MaND, Rating, BinhLuan, NgayDanhGia)
VALUES 
		(1, 2, 5, N'Đồ giống như mô tả', GETDATE()),
		(2, 3, 4, N'Tuyệt với 10 điểm cho shop', GETDATE());


INSERT INTO KHUYENMAI (TenKM, MoTa, LoaiGiamGia, GiaTriGiam, DieuKienApDung, NgayBatDau, NgayKetThuc, TrangThai)
VALUES 
(N'Giảm 10% cho đơn trên 500K', N'Áp dụng cho đơn hàng từ 500.000 VNĐ trở lên.', 'PhanTram', 10, 500000, '2025-03-01', '2025-03-10', 'HoatDong'),
(N'Giảm 50K cho đơn trên 300K', N'Giảm trực tiếp 50.000 VNĐ khi đơn hàng trên 300.000 VNĐ.', 'TienMat', 50000, 300000, '2025-03-05', '2025-03-15', 'HoatDong'),
(N'Flash Sale 20% ngày 8/3', N'Chương trình giảm giá nhân ngày Quốc tế Phụ nữ.', 'PhanTram', 20, 0, '2025-03-08', '2025-03-08', 'HoatDong')

INSERT INTO MATHANG_KHUYENMAI (MaMH, MaKM) VALUES (1, 1), (2, 2); -- Ví dụ



-- Thêm dữ liệu vào bảng PHUONGTHUCTHANHTOAN
INSERT INTO PHUONGTHUCTHANHTOAN (TenPTTT) VALUES 
(N'Tiền mặt'),
(N'Chuyển khoản'),
(N'Ví điện tử');
GO

INSERT INTO LICHSUMUAHANG (MaND, MaHD, NgayMua, TongTien, MaPTTT, TinhTrang, GhiChu)
VALUES 
    (2, 1, GETDATE(), 150000, 2, N'Chờ xác nhận', N'Giao hàng trong ngày'),
	(3, 1, GETDATE(), 250000, 3, N'Đang giao', N'Đã thanh toán đầy đủ');

	SELECT * FROM LICHSUMUAHANG WHERE TinhTrang = N'Chờ xác nhận';


-- Dữ liệu cho mặt hàng bán
INSERT INTO THONGKE_MATHANG_BAN (MaMH, SoLuongBan, ThoiGian)
VALUES
(1, 50, '2025-03-01'), -- Mặt hàng 1 bán 50 cái vào ngày 1 tháng 3
(2, 30, '2025-03-01'); -- Mặt hàng 2 bán 30 cái vào ngày 1 tháng 3





USE SHOPQUANAO
SELECT * FROM CUAHANG
SELECT * FROM DANHMUC
SELECT * FROM MATHANG
SELECT * FROM NGUOIDUNG
SELECT * FROM DIACHI
SELECT * FROM HOADON
SELECT * FROM CTHOADON
SELECT * FROM DANHGIA
SELECT * FROM KHUYENMAI
SELECT * FROM MATHANG_KHUYENMAI
SELECT * FROM LICHSUMUAHANG
SELECT * FROM PHUONGTHUCTHANHTOAN
SELECT * FROM THONGKE_MATHANG_BAN
SELECT * FROM THONGKE_DOANHTHU






-- Thông tin tài khoản gồm có
/*
	Quản lý: admin@123; MK: 123
	Nhân viên : TBT@gmail.com, MK 123
	Khách hàng: kh@@gmail.com; MK 123
*/
CREATE TABLE CHUYENMUC(
    CHUYENMUCID INT PRIMARY KEY IDENTITY(1,1),
    TENCHUYENMUC NVARCHAR(MAX) NOT NULL,
    THUTU INT NOT NULL
);
GO

CREATE TABLE CHUDE (
    CHUDEID INT PRIMARY KEY IDENTITY(1,1),
    TenChuDe NVARCHAR(255) NOT NULL,
    CHUYENMUCID INT NOT NULL,
    THUTU INT NOT NULL,
    KICHHOAT BIT NOT NULL,
    CONSTRAINT FK_CHUDE_CHUYENMUC FOREIGN KEY (CHUYENMUCID) REFERENCES CHUYENMUC(CHUYENMUCID)
);
GO

CREATE TABLE BanTin(
    BanTinID INT PRIMARY KEY IDENTITY(1,1),
    CHUDEID INT NOT NULL,
    TieuDe NVARCHAR(MAX) NOT NULL,
    TOMTAT NVARCHAR(MAX) NOT NULL,
    NOIDUNG NVARCHAR(MAX) NOT NULL,
    MaND INT NOT NULL,
    NgayDang DATETIME NOT NULL DEFAULT GETDATE(),
    HINHANH NVARCHAR(MAX) NULL,
    TIEUDEHINH NVARCHAR(MAX) NULL,
    LUOTXEM INT NOT NULL,
    NOIBAT BIT NOT NULL,
    KICHHOAT BIT NOT NULL,
    CONSTRAINT FK_BanTin_NGUOIDUNG FOREIGN KEY (MaND) REFERENCES NGUOIDUNG(MaND),
    CONSTRAINT FK_BanTin_CHUDE FOREIGN KEY (CHUDEID) REFERENCES CHUDE(CHUDEID)
);
GO




INSERT INTO CHUYENMUC (TENCHUYENMUC, THUTU)
VALUES
(N'Áo Nam', 1),
(N'Áo Nữ', 2);

INSERT INTO CHUDE (TenChuDe, CHUYENMUCID, THUTU, KICHHOAT)
VALUES
(N'Áo Thun Nam', 1, 1, 1),
(N'Áo Thun Nữ', 2, 1, 1);

INSERT INTO BanTin (CHUDEID, TieuDe, TOMTAT, NOIDUNG, MaND, HINHANH, TIEUDEHINH, LUOTXEM, NOIBAT, KICHHOAT)
VALUES
(1, N'Áo Thun Nam Unisex A.R.0.1', N'Áo thun nam unisex tay lỡ, phong cách Hàn Quốc', N'Chi tiết về áo thun nam unisex tay lỡ A.R.0.1...', 1, N'Aothun1.jpg', N'Áo Thun Nam Unisex A.R.0.1', 100, 1, 1),
(2, N'Áo Thun Nữ Angel Devil', N'Áo thun nữ tay lỡ, thiết kế Angel Devil', N'Chi tiết về áo thun nữ Angel Devil...', 2, N'Aothun2.jpg', N'Áo Thun Nữ Angel Devil', 150, 1, 1);








-- Kiểm tra cấu trúc bảng
EXEC sp_help 'ChuyenMuc';
EXEC sp_help 'ChuDe';
EXEC sp_help 'BanTin';

-- Kiểm tra dữ liệu trong bảng
SELECT * FROM ChuyenMuc;
SELECT * FROM ChuDe;
SELECT * FROM BanTin;
