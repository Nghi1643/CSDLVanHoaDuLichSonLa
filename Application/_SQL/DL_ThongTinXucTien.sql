-- gets
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_GetFilter
    @TuKhoa NVARCHAR(300) = NULL,
    --@ToChucID UNIQUEIDENTIFIER = NULL,
    @HinhThucID INT = NULL,
    @TruyenThongID INT = NULL,
    @MaNgonNgu VARCHAR(4) = 'vi',
    @TrangThaiID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        xt.*,
        xt_nd.TieuDe,
        xt_nd.MoTa,
        ht_nd.Ten AS TenHinhThuc,
        tt_nd.Ten AS TenTruyenThong,
        ttID_nd.Ten AS TenTrangThai,
        dt_nd.Ten AS TenDoiTuong,
        tc.TenToChuc,
        lv.Ten AS TenLinhVuc
    FROM DL_ThongTinXucTien xt
    JOIN DL_ThongTinXucTien_NoiDung xt_nd ON xt.XucTienID = xt_nd.XucTienID 
        AND xt_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung ht_nd ON xt.HinhThucID = ht_nd.DanhMucID 
        AND ht_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung tt_nd ON xt.TruyenThongID = tt_nd.DanhMucID 
        AND tt_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung ttID_nd ON xt.TrangThaiID = ttID_nd.DanhMucID 
        AND ttID_nd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung dt_nd ON xt.DoiTuongID = dt_nd.DanhMucID 
        AND dt_nd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_ToChuc_NoiDung tc ON xt.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_LinhVuc_NoiDUng lv ON xt.LinhVucID = lv.LinhVucID
        AND lv.MaNgonNgu = @MaNgonNgu


    WHERE (@TrangThaiID IS NULL OR xt.TrangThaiID = @TrangThaiID)
        AND (@HinhThucID IS NULL OR xt.HinhThucID = @HinhThucID)
        AND (@TruyenThongID IS NULL OR xt.TruyenThongID = @TruyenThongID)
        AND (@TuKhoa IS NULL OR (xt_nd.TieuDe LIKE N'%' + @TuKhoa + N'%' 
            OR xt_nd.MoTa LIKE N'%' + @TuKhoa + N'%'))
END

-- get by id
GO
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_Get
    @MaNgonNgu VARCHAR(4) = 'vi',
    @XucTienID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        xt.*, 
        xt_nd.TieuDe, 
        xt_nd.MoTa,
        ht_nd.Ten AS TenHinhThuc,
        tt_nd.Ten AS TenTruyenThong,
        ttID_nd.Ten AS TenTrangThai,
        dt_nd.Ten AS TenDoiTuong,
        tc.TenToChuc,
        lv.Ten AS TenLinhVuc
    FROM DL_ThongTinXucTien xt
    JOIN DL_ThongTinXucTien_NoiDung xt_nd ON xt.XucTienID = xt_nd.XucTienID 
        AND xt_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung ht_nd ON xt.HinhThucID = ht_nd.DanhMucID 
        AND ht_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung tt_nd ON xt.TruyenThongID = tt_nd.DanhMucID 
        AND tt_nd.MaNgonNgu = @MaNgonNgu
    
    LEFT JOIN DM_DanhMucChung_NoiDung ttID_nd ON xt.TrangThaiID = ttID_nd.DanhMucID 
        AND ttID_nd.MaNgonNgu = @MaNgonNgu
    
     LEFT JOIN DM_DanhMucChung_NoiDung dt_nd ON xt.DoiTuongID = dt_nd.DanhMucID 
        AND dt_nd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_ToChuc_NoiDung tc ON xt.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_LinhVuc_NoiDUng lv ON xt.LinhVucID = lv.LinhVucID
        AND lv.MaNgonNgu = @MaNgonNgu
    WHERE xt.XucTienID = @XucTienID
END
-- add + edit DL_ThongTinXucTien
GO
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_AddEdit
	@XucTienID uniqueidentifier,
	@LinhVucID SMALLINT,
	@ToChucID uniqueidentifier,
	@DoiTuongID int,
	@HinhThucID int,
	@TruyenThongID int,
	@NgayBatDau date,
	@NgayKetThuc date,
	@TrangThaiID int
	AS
	BEGIN
			SET NOCOUNT ON;
			IF EXISTS (
				SELECT 1 
				FROM DL_ThongTinXucTien 
				WHERE XucTienID = @XucTienID
			)
			BEGIN
				UPDATE DL_ThongTinXucTien
						SET LinhVucID = @LinhVucID,
							ToChucID = @ToChucID,
							DoiTuongID = @DoiTuongID,
							HinhThucID = @HinhThucID,
							TruyenThongID = @TruyenThongID,
							NgayBatDau = @NgayBatDau,
							NgayKetThuc = @NgayKetThuc,
							TrangThaiID = @TrangThaiID
                        OUTPUT inserted.*
						WHERE XucTienID = @XucTienID
			END
			ELSE
			BEGIN
				INSERT INTO DL_ThongTinXucTien ( LinhVucID, ToChucID, DoiTuongID, HinhThucID, TruyenThongID, NgayBatDau, NgayKetThuc, TrangThaiID)
                    OUTPUT inserted.*
					VALUES ( @LinhVucID, @ToChucID, @DoiTuongID, @HinhThucID, @TruyenThongID, @NgayBatDau, @NgayKetThuc, @TrangThaiID)
			END
	END
-- add + edit DL_ThongTinXucTien_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_NoiDung_AddEdit
    @XucTienID uniqueidentifier,
    @MaNgonNgu varchar(4),
    @TieuDe nvarchar(300),
    @MoTa nvarchar(max)
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DL_ThongTinXucTien_NoiDung 
                WHERE XucTienID = @XucTienID AND MaNgonNgu = @MaNgonNgu
            )
            BEGIN
                UPDATE DL_ThongTinXucTien_NoiDung
                        SET TieuDe = @TieuDe,
                            MoTa = @MoTa
                        OUTPUT inserted.*
                        WHERE XucTienID = @XucTienID AND MaNgonNgu = @MaNgonNgu
            END
            ELSE
            BEGIN
                INSERT INTO DL_ThongTinXucTien_NoiDung ( XucTienID, MaNgonNgu, TieuDe, MoTa)
                    OUTPUT inserted.*
                    VALUES ( @XucTienID, @MaNgonNgu, @TieuDe, @MoTa)
            END
    END
-- delete DL_ThongTinXucTien
GO
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_Delete
    @XucTienID uniqueidentifier
    AS
    BEGIN
    -- SET NOCOUNT ON;
        DELETE FROM DL_ThongTinXucTien_NoiDung WHERE XucTienID = @XucTienID
        DELETE FROM DL_ThongTinXucTien WHERE XucTienID = @XucTienID 
    END
-- Lấy danh sách bản dịch của ThongTinXucTien
GO
CREATE OR ALTER PROCEDURE spu_DL_ThongTinXucTien_NoiDung_GetFilter
    @XucTienID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM DL_ThongTinXucTien_NoiDung
    WHERE (@XucTienID IS NULL OR XucTienID = @XucTienID)
        AND (@MaNgonNgu IS NULL OR MaNgonNgu = @MaNgonNgu)
END