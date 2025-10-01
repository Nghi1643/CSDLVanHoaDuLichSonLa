--cơ sở ăn uống
-- gets
CREATE OR ALTER PROCEDURE spu_DL_CoSoAnUong_GetFilter
    @TuKhoa NVARCHAR(300) = NULL,
    @LoaiDichVuAnUongID INT = NULL,
    @MaNgonNgu VARCHAR(4) = 'vi',
	@TrangThai BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SELECT 
    au.*,
    au_nd.TenCoSo,au_nd.DiaChi,
    dmc_nd.Ten AS TenLoaiDichVuAnUong
    FROM DL_CoSoAnUong au
    JOIN DL_CoSoAnUong_NoiDung au_nd ON au.CoSoAnUongID = au_nd.CoSoAnUongID AND au_nd.MaNgonNgu = @MaNgonNgu
    LEFT JOIN DM_DanhMucChung dmc ON dmc.DanhMucID = au.LoaiDichVuAnUongID
    LEFT JOIN DM_DanhMucChung_NoiDung dmc_nd ON dmc.DanhMucID = dmc_nd.DanhMucID AND dmc_nd.MaNgonNgu = @MaNgonNgu
    WHERE (@TrangThai IS NULL OR au.TrangThai = @TrangThai)
    AND (@LoaiDichVuAnUongID IS NULL OR au.LoaiDichVuAnUongID = @LoaiDichVuAnUongID)
    AND (@TuKhoa IS NULL OR (au_nd.TenCoSo LIKE N'%' + @TuKhoa + N'%' OR au_nd.DiaChi LIKE N'%' + @TuKhoa + N'%'))
END
--get by id
GO
CREATE OR ALTER PROCEDURE spu_DL_CoSoAnUong_Get
	@MaNgonNgu varchar(4)="vi",
	@CoSoAnUongID uniqueidentifier
		AS
			BEGIN
				SELECT au.*, au_nd.TenCoSo, au_nd.MoTa, au_nd.DiaChi, au_nd.NguoiDaiDien, dd.TenDiaDiem, dcm_nd.Ten as tenLoaiDichVuAnUong
				FROM DL_CoSoAnUong AS au
				INNER JOIN DL_CoSoAnUong_NoiDung AS au_nd ON au.CoSoAnUongID = au_nd.CoSoAnUongID AND au_nd.MaNgonNgu = @MaNgonNgu
				LEFT JOIN DM_DiaDiem_NoiDung AS dd ON au.DiaDiemID  = dd.DiaDiemID AND dd.MaNgonNgu = @MaNgonNgu
				LEFT JOIN DM_DanhMucChung_NoiDung AS dcm_nd ON au.LoaiDichVuAnUongID = dcm_nd.DanhMucID AND dcm_nd.MaNgonNgu = @MaNgonNgu
				WHERE au.CoSoAnUongID = @CoSoAnUongID
			END
-- add + edit DL_CoSoAnUong
GO
CREATE OR ALTER PROCEDURE spu_DL_CoSoAnUong_AddEdit
	@CoSoAnUongID uniqueidentifier,
	@DiaDiemID uniqueidentifier,
	@LoaiDichVuAnUongID int,
	@SoLuotDanhGia int,
	@DiemDanhGia int,
	@SucChua int,
	@TiecGiaDinh bit,
	@TiecDongNguoi bit,
	@CoPhongRieng bit,
	@CoVuiChoiTreEm bit,
	@ChuanDuLich bit,
	@DienThoai varchar(30),
	@HopThu varchar(100),
	@LienKet varchar(200),
	@NgayNghiHangTuan varchar(20),
	@GioMoCua time(7),
	@GioDongCua time(7),
	@TrangThai bit,
	@NguoiTao int
	--@NguoiCapNhat int
	AS
	BEGIN
			SET NOCOUNT ON;
			IF EXISTS (
				SELECT 1 
				FROM DL_CoSoAnUong 
				WHERE CoSoAnUongID = @CoSoAnUongID
			)
			BEGIN
				UPDATE DL_CoSoAnUong
						SET DiaDiemID = @DiaDiemID,
							LoaiDichVuAnUongID = @LoaiDichVuAnUongID,
							SoLuotDanhGia = @SoLuotDanhGia,
							DiemDanhGia = @DiemDanhGia,
							SucChua = @SucChua,
							TiecGiaDinh = @TiecGiaDinh,
							TiecDongNguoi = @TiecDongNguoi,
							CoPhongRieng = @CoPhongRieng,
							CoVuiChoiTreEm = @CoVuiChoiTreEm,
							ChuanDuLich = @ChuanDuLich,
							DienThoai = @DienThoai,
							HopThu = @HopThu,
							LienKet = @LienKet,
							NgayNghiHangTuan = @NgayNghiHangTuan,
							GioMoCua = @GioMoCua,
							GioDongCua = @GioDongCua,
							TrangThai = @TrangThai,
							NguoiCapNhat = @NguoiTao,
							NgayCapNhat = GETDATE()
							OUTPUT inserted.* 
						WHERE CoSoAnUongID = @CoSoAnUongID
			END
			ELSE
			BEGIN
				INSERT INTO DL_CoSoAnUong (DiaDiemID, LoaiDichVuAnUongID, SoLuotDanhGia, DiemDanhGia, SucChua, TiecGiaDinh, TiecDongNguoi, CoPhongRieng, CoVuiChoiTreEm,
					ChuanDuLich, DienThoai, HopThu, LienKet, NgayNghiHangTuan, GioMoCua, GioDongCua, TrangThai, NguoiTao, NgayTao)
					OUTPUT inserted.*
					VALUES (@DiaDiemID, @LoaiDichVuAnUongID, @SoLuotDanhGia, @DiemDanhGia, @SucChua, @TiecGiaDinh, @TiecDongNguoi, @CoPhongRieng, @CoVuiChoiTreEm,
					@ChuanDuLich, @DienThoai, @HopThu, @LienKet, @NgayNghiHangTuan, @GioMoCua, @GioDongCua, @TrangThai, @NguoiTao, GETDATE())
			END
	END
-- add + edit DL_CoSoAnUong_NoiDung
GO
CREATE OR ALTER PROCEDURE spu_DL_CoSoAnUong_NoiDung_AddEdit
	@CoSoAnUongID uniqueidentifier,
	@MaNgonNgu varchar(20),
	@TenCoSo nvarchar(300),
	@MoTa nvarchar(4000),
	@DiaChi nvarchar(300),
	@NguoiDaiDien nvarchar(100)
		AS
			BEGIN
				IF EXISTS (SELECT 1 FROM DL_CoSoAnUong_NoiDung WHERE CoSoAnUongID = @CoSoAnUongID AND MaNgonNgu = @MaNgonNgu)
					BEGIN
						UPDATE DL_CoSoAnUong_NoiDung
							SET TenCoSo = @TenCoSo,
								MoTa = @MoTa,
								DiaChi = @DiaChi,
								NguoiDaiDien = @NguoiDaiDien
							OUTPUT inserted.*
						WHERE CoSoAnUongID = @CoSoAnUongID AND MaNgonNgu = @MaNgonNgu
					END
				ELSE 
					BEGIN
						INSERT INTO DL_CoSoAnUong_NoiDung (CoSoAnUongID, MaNgonNgu, TenCoSo, MoTa, DiaChi, NguoiDaiDien)
						OUTPUT inserted.*
						VALUES(@CoSoAnUongID, @MaNgonNgu, @TenCoSo, @MoTa, @DiaChi, @NguoiDaiDien)
					END
			END
-- delete
GO
CREATE OR ALTER PROCEDURE spu_DL_CoSoAnUong_Delete
	@CoSoAnUongID uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @DeletedRows INT = 0;
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- xóa bảng con trước
		DELETE FROM DL_CoSoAnUong_NoiDung 
		WHERE CoSoAnUongID = @CoSoAnUongID;
		
		DELETE FROM DL_CoSoAnUong 
		WHERE CoSoAnUongID = @CoSoAnUongID;
		
		SET @DeletedRows = @@ROWCOUNT;
		COMMIT TRANSACTION;
	
		SELECT @DeletedRows as RowsAffected;
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH
END