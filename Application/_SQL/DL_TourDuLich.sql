--DL_TourDuLich
-- gets
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_GetFilter
    @TuKhoa NVARCHAR(300) = NULL,
    @ToChucID UNIQUEIDENTIFIER = NULL,
    @TheLoaiID UNIQUEIDENTIFIER = NULL,
    @LoaiHinhID INT = NULL,
    @PhanKhucID INT = NULL,
    @PhuongTienID INT = NULL,
    @MaNgonNgu VARCHAR(4) = 'vi',
    @TrangThai BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
    dl.*,
    dl_nd.TenTour, dl_nd.MoTa, dl_nd.ThoiGian,TraiNghiem,GoiY,
    tc.TenToChuc,
    tl.TenTheLoai,
    lh.Ten AS TenLoaiHinh,
    pk.Ten AS TenPhanKhuc,
    di.TenDiaDiem AS TenDiemDi,
    den.TenDiaDiem AS TenDiemDen,
    nn.TenNgonNgu,
    pt.Ten AS TenPhuongTien
    FROM DL_TourDuLich dl
    JOIN DL_TourDuLich_NoiDung dl_nd ON dl.TourID = dl_nd.TourID 
        AND dl_nd.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_ToChuc_NoiDung tc ON dl.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DL_TourDuLich_TheLoai_NoiDung tl ON dl.TheLoaiID = tl.TheLoaiID
     --AND tl.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_DanhMucChung_NoiDung lh ON dl.LoaiHinhID = lh.DanhMucID 
        AND lh.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung pk ON dl.PhanKhucID = pk.DanhMucID
        AND pk.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_DiaDiem_NoiDung di ON dl.DiemDiID = di.DiaDiemID
        AND di.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DiaDiem_NoiDung den ON dl.DiemDenID = den.DiaDiemID
        AND den.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_NgonNgu nn ON dl.MaNgonNgu = nn.MaNgonNgu
        AND nn.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung pt ON dl.PhuongTienID = pt.DanhMucID
        AND pt.MaNgonNgu = @MaNgonNgu

    WHERE (@TrangThai IS NULL OR dl.TrangThai = @TrangThai)
        AND (@ToChucID IS NULL OR dl.ToChucID = @ToChucID)
        AND (@TheLoaiID IS NULL OR dl.TheLoaiID = @TheLoaiID)
        AND (@LoaiHinhID IS NULL OR dl.LoaiHinhID = @LoaiHinhID)
        AND (@PhanKhucID IS NULL OR dl.PhanKhucID = @PhanKhucID)
        AND (@PhuongTienID IS NULL OR dl.PhuongTienID = @PhuongTienID)
        AND (@TuKhoa IS NULL OR (dl_nd.TenTour LIKE N'%' + @TuKhoa + N'%' 
            OR dl_nd.MoTa LIKE N'%' + @TuKhoa + N'%'))
END

-- get by id
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_Get
    @TourID UNIQUEIDENTIFIER,
    @MaNgonNgu VARCHAR(4) = 'vi'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
    dl.*,
    dl_nd.TenTour, dl_nd.MoTa, dl_nd.ThoiGian,TraiNghiem,GoiY,
    tc.TenToChuc,
    tl.TenTheLoai,
    lh.Ten AS TenLoaiHinh,
    pk.Ten AS TenPhanKhuc,
    di.TenDiaDiem AS TenDiemDi,
    den.TenDiaDiem AS TenDiemDen,
    nn.TenNgonNgu,
    pt.Ten AS TenPhuongTien
    FROM DL_TourDuLich dl
    JOIN DL_TourDuLich_NoiDung dl_nd ON dl.TourID = dl_nd.TourID 
        AND dl_nd.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_ToChuc_NoiDung tc ON dl.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DL_TourDuLich_TheLoai_NoiDung tl ON dl.TheLoaiID = tl.TheLoaiID
     --AND tl.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_DanhMucChung_NoiDung lh ON dl.LoaiHinhID = lh.DanhMucID 
        AND lh.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung pk ON dl.PhanKhucID = pk.DanhMucID
        AND pk.MaNgonNgu = @MaNgonNgu

     LEFT JOIN DM_DiaDiem_NoiDung di ON dl.DiemDiID = di.DiaDiemID
        AND di.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DiaDiem_NoiDung den ON dl.DiemDenID = den.DiaDiemID
        AND den.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_NgonNgu nn ON dl.MaNgonNgu = nn.MaNgonNgu
        AND nn.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung pt ON dl.PhuongTienID = pt.DanhMucID
        AND pt.MaNgonNgu = @MaNgonNgu

    WHERE dl.TourID = @TourID
END
-- add + edit DL_TourDuLich
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_AddEdit
	@TourID uniqueidentifier,
	@ToChucID uniqueidentifier,
    @MaDinhDanh varchar(50),
    @TheLoaiID uniqueidentifier,
    @LoaiHinhID int,
    @PhanKhucID int,
    @DiemDiID uniqueidentifier,
    @DiemDenID uniqueidentifier,
    @NgayKhoiHanh datetime2,
    @NgayKetThuc datetime2,
    @SoKhachToiThieu smallint,
    @SoKhachToiDa smallint,
    @GiaTour bigint,
    @GiaNguoi bigint,
    @MaNgonNgu varchar(4),
    @PhuongTienID int,
    @SoLuotDanhGia int,
    @DiemDanhGia int,
    @TrangThai bit
	AS
	BEGIN
			SET NOCOUNT ON;
			IF EXISTS (
				SELECT 1 
				FROM DL_TourDuLich
				WHERE TourID = @TourID
			)
			BEGIN
				UPDATE DL_TourDuLich
						SET ToChucID = @ToChucID,
                            MaDinhDanh = @MaDinhDanh,   
							TheLoaiID = @TheLoaiID,
							LoaiHinhID = @LoaiHinhID,
							PhanKhucID = @PhanKhucID,
							DiemDiID = @DiemDiID,
							DiemDenID = @DiemDenID,
							NgayKhoiHanh = @NgayKhoiHanh,
							NgayKetThuc = @NgayKetThuc,
							SoKhachToiThieu = @SoKhachToiThieu,
							SoKhachToiDa = @SoKhachToiDa,
							GiaTour = @GiaTour,
							GiaNguoi = @GiaNguoi,
							MaNgonNgu = @MaNgonNgu,
							PhuongTienID = @PhuongTienID,
							SoLuotDanhGia = @SoLuotDanhGia,
							DiemDanhGia = @DiemDanhGia,
							TrangThai = @TrangThai
                        OUTPUT inserted.*
						WHERE TourID = @TourID
			END
			ELSE
			BEGIN
				INSERT INTO DL_TourDuLich ( ToChucID,MaDinhDanh, TheLoaiID, LoaiHinhID, PhanKhucID, DiemDiID, DiemDenID, NgayKhoiHanh, NgayKetThuc, SoKhachToiThieu, SoKhachToiDa, GiaTour, GiaNguoi, MaNgonNgu, PhuongTienID, SoLuotDanhGia, DiemDanhGia, TrangThai)
                    OUTPUT inserted.*
					VALUES ( @ToChucID,@MaDinhDanh, @TheLoaiID, @LoaiHinhID, @PhanKhucID, @DiemDiID, @DiemDenID, @NgayKhoiHanh, @NgayKetThuc, @SoKhachToiThieu, @SoKhachToiDa, @GiaTour, @GiaNguoi, @MaNgonNgu, @PhuongTienID, @SoLuotDanhGia, @DiemDanhGia, @TrangThai)
			END
	END
-- add + edit DL_TourDuLich_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_NoiDung_AddEdit
    @TourID uniqueidentifier,
    @MaNgonNgu varchar(4),
    @TenTour nvarchar(200),
    @MoTa nvarchar(4000),
    @ThoiGian nvarchar(50),
    @TraiNghiem nvarchar(4000),
    @GoiY nvarchar(1000)
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DL_TourDuLich_NoiDung
                WHERE TourID = @TourID AND MaNgonNgu = @MaNgonNgu
            )
            BEGIN
                UPDATE DL_TourDuLich_NoiDung
                        SET TenTour = @TenTour,
                            MoTa = @MoTa,
                            ThoiGian = @ThoiGian,
                            TraiNghiem = @TraiNghiem,
                            GoiY = @GoiY
                        OUTPUT inserted.*
                        WHERE TourID = @TourID AND MaNgonNgu = @MaNgonNgu
            END
            ELSE
            BEGIN
                INSERT INTO DL_TourDuLich_NoiDung ( TourID, MaNgonNgu, TenTour, MoTa, ThoiGian, TraiNghiem, GoiY)
                    OUTPUT inserted.*
                    VALUES ( @TourID, @MaNgonNgu, @TenTour, @MoTa, @ThoiGian, @TraiNghiem, @GoiY)
            END
    END
-- delete 
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_Delete
    @TourID uniqueidentifier
    AS
    BEGIN
    -- SET NOCOUNT ON;
        DELETE FROM DL_TourDuLich_NoiDung WHERE TourID = @TourID
        DELETE FROM DL_TourDuLich WHERE TourID = @TourID
    END
-- Lấy danh sách bản dịch của 
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_NoiDung_GetFilter
    @TourID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM DL_TourDuLich_NoiDung
    WHERE (@TourID IS NULL OR TourID = @TourID)
        AND (@MaNgonNgu IS NULL OR MaNgonNgu = @MaNgonNgu)
END

--DL_TourDuLich_LichTrinh
-- gets lich trinh by tour id
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_LichTrinh_GetsByTour
    @TourID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4) = 'vi'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
    lt.*,
    lt_nd.HoatDong,SoGio,
    dd.TenDiaDiem,dd.DiaChi
    FROM DL_TourDuLich_LichTrinh lt
    LEFT JOIN DL_TourDuLich_LichTrinh_NoiDung lt_nd ON lt.LichTrinhID = lt_nd.LichTrinhID
    
    LEFT JOIN DM_DiaDiem_NoiDung dd ON lt.DiaDiemID = dd.DiaDiemID
        AND dd.MaNgonNgu = @MaNgonNgu
    WHERE (@TourID IS NULL OR lt.TourID = @TourID)
    ORDER BY lt.ThuTu
END
--get
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_LichTrinh_Get
    @LichTrinhID int
    AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
    lt.*,
    lt_nd.HoatDong,SoGio,
    dd.TenDiaDiem,dd.DiaChi
    FROM DL_TourDuLich_LichTrinh lt
    LEFT JOIN DL_TourDuLich_LichTrinh_NoiDung lt_nd ON lt.LichTrinhID = lt_nd.LichTrinhID
    
    LEFT JOIN DM_DiaDiem_NoiDung dd ON lt.DiaDiemID = dd.DiaDiemID
    WHERE (lt.LichTrinhID = @LichTrinhID)
END
-- add + edit DL_TourDuLich_LichTrinh
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_LichTrinh_AddEdit
    @LichTrinhID int,
    @TourID uniqueidentifier,
    @DiaDiemID uniqueidentifier,
    @ThoiDiem datetime2,
    @ThuTu int
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DL_TourDuLich_LichTrinh
                WHERE LichTrinhID = @LichTrinhID
            )
            BEGIN
                DELETE FROM DL_TourDuLich_LichTrinh_NoiDung WHERE LichTrinhID = @LichTrinhID
                UPDATE DL_TourDuLich_LichTrinh
                        SET TourID = @TourID,
                            DiaDiemID = @DiaDiemID,
                            ThoiDiem = @ThoiDiem,
                            ThuTu = @ThuTu
                        OUTPUT inserted.*
                        WHERE LichTrinhID = @LichTrinhID
            END
            ELSE
            BEGIN
                INSERT INTO DL_TourDuLich_LichTrinh ( TourID,  DiaDiemID,ThoiDiem, ThuTu)
                    OUTPUT inserted.*
                    VALUES ( @TourID,  @DiaDiemID,@ThoiDiem, @ThuTu)
            END
    END
-- add + edit DL_TourDuLich_LichTrinh_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_LichTrinh_NoiDung_AddEdit
    @LichTrinhID int,
    @HoatDong nvarchar(1000),
    @SoGio nvarchar(50)
    AS
    BEGIN
            SET NOCOUNT ON;
            --IF EXISTS (
            --    SELECT 1 
            --    FROM DL_TourDuLich_LichTrinh_NoiDung
            --    WHERE LichTrinhID = @LichTrinhID
          --  )
           -- BEGIN
             --   UPDATE DL_TourDuLich_LichTrinh_NoiDung
             --           SET HoatDong = @HoatDong,
            --                SoGio = @SoGio
            --            OUTPUT inserted.*
             --           WHERE LichTrinhID = @LichTrinhID
          --  END
--ELSE
           -- BEGIN
               
                INSERT INTO DL_TourDuLich_LichTrinh_NoiDung ( LichTrinhID, HoatDong, SoGio)
                    OUTPUT inserted.*
                    VALUES ( @LichTrinhID, @HoatDong, @SoGio)
           -- END
    END
-- delete
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_LichTrinh_Delete
    @LichTrinhID int
    AS
    BEGIN
    -- SET NOCOUNT ON;
        DELETE FROM DL_TourDuLich_LichTrinh_NoiDung WHERE LichTrinhID = @LichTrinhID
        DELETE FROM DL_TourDuLich_LichTrinh WHERE LichTrinhID = @LichTrinhID
    END

-- DL_TourDuLich_TheLoai
-- gets
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_Gets
     @MaNgonNgu VARCHAR(4) 
    AS
    BEGIN
    SET NOCOUNT ON;
        SELECT 
        tl.*,
        tl_nd.TenTheLoai, tl_nd.MoTa,
        lv_nd.Ten as TenLinhVuc
        FROM DL_TourDuLich_TheLoai tl
        LEFT JOIN DL_TourDuLich_TheLoai_NoiDung tl_nd ON tl.TheLoaiID = tl_nd.TheLoaiID
        
        LEFT JOIN DL_TourDuLich_TheLoai_LinhVuc tl_lv ON tl.TheLoaiID = tl_lv.TheLoaiID

        LEFT JOIN DM_LinhVuc lv ON tl_lv.LinhVucID = lv.LinhVucID
        AND lv.TrangThai = 1

        LEFT JOIN DM_LinhVuc_NoiDung lv_nd ON lv.LinhVucID = lv_nd.LinhVucID
        AND LTRIM(RTRIM(lv_nd.MaNgonNgu)) = LTRIM(RTRIM(@MaNgonNgu));
    END
-- get
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_Get
    @TheLoaiID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4)
    AS
    BEGIN
    SET NOCOUNT ON;
    SELECT 
        tl.*,
        tl_nd.TenTheLoai, tl_nd.MoTa,
        lv_nd.Ten as TenLinhVuc
        FROM DL_TourDuLich_TheLoai tl
        LEFT JOIN DL_TourDuLich_TheLoai_NoiDung tl_nd ON tl.TheLoaiID = tl_nd.TheLoaiID
        
        LEFT JOIN DL_TourDuLich_TheLoai_LinhVuc tl_lv ON tl.TheLoaiID = tl_lv.TheLoaiID

        LEFT JOIN DM_LinhVuc lv ON tl_lv.LinhVucID = lv.LinhVucID
        AND lv.TrangThai = 1

        LEFT JOIN DM_LinhVuc_NoiDung lv_nd ON lv.LinhVucID = lv_nd.LinhVucID
        AND lv_nd.MaNgonNgu =@MaNgonNgu

        WHERE tl.TheLoaiID = @TheLoaiID
    END

-- add + edit DL_TourDuLich_TheLoai
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_AddEdit
    @TheLoaiID uniqueidentifier,
    @TrangThai bit
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DL_TourDuLich_TheLoai
                WHERE TheLoaiID = @TheLoaiID
            )
            BEGIN
                UPDATE DL_TourDuLich_TheLoai
                        SET TrangThai = @TrangThai
                        OUTPUT inserted.*
                        WHERE TheLoaiID = @TheLoaiID
            END
            ELSE
            BEGIN
                INSERT INTO DL_TourDuLich_TheLoai (TrangThai)
                    OUTPUT inserted.*
                    VALUES (@TrangThai)
            END
    END
-- add + edit DL_TourDuLich_TheLoai_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_NoiDung_AddEdit
    @TheLoaiID uniqueidentifier,
    @TenTheLoai nvarchar(200),
    @MoTa nvarchar(4000)
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DL_TourDuLich_TheLoai_NoiDung
                WHERE TheLoaiID = @TheLoaiID
            )
            BEGIN
                UPDATE DL_TourDuLich_TheLoai_NoiDung
                        SET TenTheLoai = @TenTheLoai,
                            MoTa = @MoTa
                        OUTPUT inserted.*
                        WHERE TheLoaiID = @TheLoaiID 
            END
            ELSE
            BEGIN
                INSERT INTO DL_TourDuLich_TheLoai_NoiDung ( TheLoaiID, TenTheLoai, MoTa)
                    OUTPUT inserted.*
                    VALUES ( @TheLoaiID, @TenTheLoai, @MoTa)
            END
    END
-- delete
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_Delete
    @TheLoaiID uniqueidentifier
    AS
    BEGIN
    -- SET NOCOUNT ON;
        DELETE FROM DL_TourDuLich_TheLoai_NoiDung WHERE TheLoaiID = @TheLoaiID
        DELETE FROM DL_TourDuLich_TheLoai WHERE TheLoaiID = @TheLoaiID
    END
-- add relation the loai - linh vuc DL_TourDuLich_TheLoai_LinhVuc
GO
CREATE OR ALTER PROCEDURE spu_DL_TourDuLich_TheLoai_LinhVuc_Add
    @TheLoaiID uniqueidentifier,
    @ListLinhVucID nvarchar(4000) 
    AS
    BEGIN
            SET NOCOUNT ON;
            --IF EXISTS (
                --SELECT 1 
                --FROM DL_TourDuLich_TheLoai_LinhVuc
               -- WHERE TheLoaiID = @TheLoaiID
           -- )
               -- BEGIN
               --luôn xóa trước khi thêm 
                    DELETE FROM DL_TourDuLich_TheLoai_LinhVuc WHERE TheLoaiID = @TheLoaiID
                    INSERT INTO DL_TourDuLich_TheLoai_LinhVuc ( TheLoaiID, LinhVucID)
                        OUTPUT inserted.*
                    SELECT @TheLoaiID, TRY_CAST(value AS smallint)
                    FROM STRING_SPLIT(@ListLinhVucID, ',')
                    WHERE 
						TRY_CAST(value AS smallint) IS NOT NULL 
              --  END
          --  ELSE
             --   BEGIN
              --      INSERT INTO DL_TourDuLich_TheLoai_LinhVuc ( TheLoaiID, LinhVucID)
               --         OUTPUT inserted.*
              --      SELECT @TheLoaiID, TRY_CAST(value AS smallint)
               --     FROM STRING_SPLIT(@ListLinhVuc, ',')
               --     WHERE 
					--	TRY_CAST(value AS smallint) IS NOT NULL 
            --    END
    END