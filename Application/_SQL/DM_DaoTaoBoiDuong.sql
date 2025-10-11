-- gets
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_GetFilter
    @TuKhoa NVARCHAR(300) = NULL,
    @LinhVucID SMALLINT = NULL,
    @ToChucID UNIQUEIDENTIFIER = NULL,
    @DiaDiemID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4) = 'vi',
    @TrangThaiID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        dt.*,
        dt_nd.NoiDungDaoTao,dt_nd.DonVi,
        lv.Ten AS TenLinhVuc,
        tc.TenToChuc,
        dd.TenDiaDiem,
        ttID_nd.Ten AS TenTrangThai

    FROM DM_DaoTaoBoiDuong dt
    JOIN DM_DaoTaoBoiDuong_NoiDung dt_nd ON dt.DaoTaoID = dt_nd.DaoTaoID 
        AND dt_nd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_LinhVuc_NoiDUng lv ON dt.LinhVucID = lv.LinhVucID
        AND lv.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_ToChuc_NoiDung tc ON dt.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DiaDiem_NoiDung dd ON dt.DiaDiemID = dd.DiaDiemID 
        AND dd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung ttID_nd ON dt.TrangThaiID = ttID_nd.DanhMucID 
        AND ttID_nd.MaNgonNgu = @MaNgonNgu

    WHERE (@TrangThaiID IS NULL OR dt.TrangThaiID = @TrangThaiID)
        AND (@LinhVucID IS NULL OR dt.LinhVucID = @LinhVucID)
        AND (@ToChucID IS NULL OR dt.ToChucID = @ToChucID)
        AND (@DiaDiemID IS NULL OR dt.DiaDiemID = @DiaDiemID)
        AND (@TuKhoa IS NULL OR (dt_nd.NoiDungDaoTao LIKE N'%' + @TuKhoa + N'%' 
            OR dt_nd.DonVi LIKE N'%' + @TuKhoa + N'%'))
END

-- get by id
GO
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_Get
    @DaoTaoID UNIQUEIDENTIFIER,
    @MaNgonNgu VARCHAR(4) = 'vi'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        dt.*,
        dt_nd.NoiDungDaoTao,dt_nd.DonVi,
        lv.Ten AS TenLinhVuc,
        tc.TenToChuc,
        dd.TenDiaDiem,
        ttID_nd.Ten AS TenTrangThai

    FROM DM_DaoTaoBoiDuong dt
    JOIN DM_DaoTaoBoiDuong_NoiDung dt_nd ON dt.DaoTaoID = dt_nd.DaoTaoID 
        AND dt_nd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_LinhVuc_NoiDUng lv ON dt.LinhVucID = lv.LinhVucID
        AND lv.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_ToChuc_NoiDung tc ON dt.ToChucID = tc.ToChucID
        AND tc.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DiaDiem_NoiDung dd ON dt.DiaDiemID = dd.DiaDiemID 
        AND dd.MaNgonNgu = @MaNgonNgu

    LEFT JOIN DM_DanhMucChung_NoiDung ttID_nd ON dt.TrangThaiID = ttID_nd.DanhMucID 
        AND ttID_nd.MaNgonNgu = @MaNgonNgu

    WHERE dt.DaoTaoID=@DaoTaoID
END
-- add + edit DL_ThongTinXucTien
GO
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_AddEdit
	@DaoTaoID uniqueidentifier,
	@LinhVucID SMALLINT,
	@ToChucID uniqueidentifier,
	@DiaDiemID uniqueidentifier,
	@BatDau datetime,
	@KetThuc datetime,
	@TrangThaiID int
	AS
	BEGIN
			SET NOCOUNT ON;
			IF EXISTS (
				SELECT 1 
				FROM DM_DaoTaoBoiDuong
				WHERE DaoTaoID = @DaoTaoID
			)
			BEGIN
				UPDATE DM_DaoTaoBoiDuong
						SET LinhVucID = @LinhVucID,
							ToChucID = @ToChucID,
							DiaDiemID = @DiaDiemID,
							BatDau = @BatDau,
							KetThuc = @KetThuc,
							TrangThaiID = @TrangThaiID
                        OUTPUT inserted.*
						WHERE DaoTaoID = @DaoTaoID
			END
			ELSE
			BEGIN
				INSERT INTO DM_DaoTaoBoiDuong ( LinhVucID, ToChucID, DiaDiemID, BatDau, KetThuc, TrangThaiID)
                    OUTPUT inserted.*
					VALUES ( @LinhVucID, @ToChucID, @DiaDiemID, @BatDau, @KetThuc, @TrangThaiID)
			END
	END
-- add + edit DL_ThongTinXucTien_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_NoiDung_AddEdit
    @DaoTaoID uniqueidentifier,
    @MaNgonNgu varchar(4),
    @NoiDungDaoTao nvarchar(1000),
    @DonVi nvarchar(100)
    AS
    BEGIN
            SET NOCOUNT ON;
            IF EXISTS (
                SELECT 1 
                FROM DM_DaoTaoBoiDuong_NoiDung 
                WHERE DaoTaoID = @DaoTaoID AND MaNgonNgu = @MaNgonNgu
            )
            BEGIN
                UPDATE DM_DaoTaoBoiDuong_NoiDung
                        SET NoiDungDaoTao = @NoiDungDaoTao,
                            DonVi = @DonVi
                        OUTPUT inserted.*
                        WHERE DaoTaoID = @DaoTaoID AND MaNgonNgu = @MaNgonNgu
            END
            ELSE
            BEGIN
                INSERT INTO DM_DaoTaoBoiDuong_NoiDung ( DaoTaoID, MaNgonNgu, NoiDungDaoTao, DonVi)
                    OUTPUT inserted.*
                    VALUES ( @DaoTaoID, @MaNgonNgu, @NoiDungDaoTao, @DonVi)
            END
    END
-- delete 
GO
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_Delete
    @DaoTaoID uniqueidentifier
    AS
    BEGIN
    -- SET NOCOUNT ON;
        DELETE FROM DM_DaoTaoBoiDuong_NoiDung WHERE DaoTaoID = @DaoTaoID
        DELETE FROM DM_DaoTaoBoiDuong WHERE DaoTaoID = @DaoTaoID
    END
-- Lấy danh sách bản dịch của 
GO
CREATE OR ALTER PROCEDURE spu_DM_DaoTaoBoiDuong_NoiDung_GetFilter
    @DaoTaoID UNIQUEIDENTIFIER = NULL,
    @MaNgonNgu VARCHAR(4) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM DM_DaoTaoBoiDuong_NoiDung
    WHERE (@DaoTaoID IS NULL OR DaoTaoID = @DaoTaoID)
        AND (@MaNgonNgu IS NULL OR MaNgonNgu = @MaNgonNgu)
END