using CSDLVanHoaDuLichSonLa.Controllers.API;
using Domain;
using Domain.RequestDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain.Enums;

namespace CSDLVanHoaDuLichSonLa.Controllers
{
    public class AdminMenuApiController : BaseApiController
    {
        private readonly IOptions<AdminNavModel> _adminNav;
        private readonly IJsonService _jsonService;

        public AdminMenuApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config, IOptions<AdminNavModel> adminNav, IJsonService jsonService) : base(hostingEnvironment, config)
        {
            _adminNav = adminNav;
            _jsonService = jsonService;
        }

        [HttpGet]
        [Route("getmenu/{parentid?}")]
        public async Task<IActionResult> GetDanhSachMenu(int? parentid)
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSach.Query { ParentId = parentid });
            return HandleResult(result);
        }

        [HttpGet]
        [Route("danhsachmenu")]
        public async Task<IActionResult> DanhSachMenu()
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSachMenu.Query());
            return HandleResult(result);
        }

        [HttpGet]
        [Route("danhsachmenutheorole/{rolename}")]
        public async Task<IActionResult> DanhSachMenuTheoRole(string rolename)
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSachMenuTheoRole.Query { RoleName = rolename });
            return HandleResult(result);
        }

        [HttpGet]
        [Route("getmenupermission/{id}")]
        public async Task<IActionResult> GetDanhSachMenu(int id)
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSachPhanQuyen.Query { MenuId = id });
            return HandleResult(result);
        }

        [HttpPost]
        [Route("addmenupermission")]
        public async Task<IActionResult> AddMenuPermission([FromBody] CSDL_MenuPermission _request)
        {
            var result = await Mediator.Send(new Application.AdminMenu.ThemMoiPhanQuyen.Command { Entity = _request });
            //var abc = await Mediator.Send(new Application.AdminMenu.ChiTietThanhPhanMenu.Query { MenuId = _request.MenuId });
            //AdminNavModel vm = (AdminNavModel)_adminNav.Value.Clone();
            //_jsonService.UpdateData(vm, abc.Value, _request.Rolename);

            if (!result.IsSuccess)
                return HandleResult(result);

            _jsonService.RecreateData();
            return HandleResult(result);
        }

        [HttpPost]
        [Route("updatemenupermission")]
        public async Task<IActionResult> UpdateMenuPermission([FromBody] CSDL_MenuPermission _request)
        {
            var result = await Mediator.Send(new Application.AdminMenu.CapNhatPhanQuyen.Command { permission = _request });
            return HandleResult(result);
        }

        [HttpDelete]
        [Route("deletemenupermission/{id}")]
        public async Task<IActionResult> DeleteMenuPermission(int id)
        {
            var result = await Mediator.Send(new Application.AdminMenu.XoaPhanQuyen.Command { Id = id });
            if (!result.IsSuccess)
                return HandleResult(result);
            _jsonService.RecreateData();
            return HandleResult(result);
        }

        [HttpGet]
        [Route("danhsachnodecha")]
        public async Task<IActionResult> DanhSachNodeCha()
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSachMenuParent.Query());
            return HandleResult(result);
        }

        [HttpGet]
        [Route("danhsachnodela")]
        public async Task<IActionResult> DanhSachNodeLa()
        {
            var result = await Mediator.Send(new Application.AdminMenu.DanhSachMenuChild.Query());
            return HandleResult(result);
        }

        [HttpPost]
        [Route("themmoimenu")]
        public async Task<IActionResult> ThemMoiMenu([FromBody] CSDL_AdminMenu _request)
        {
            var result = await Mediator.Send(new Application.AdminMenu.ThemMoiMenu.Command { Entity = _request });
            if (!result.IsSuccess)
                return HandleResult(result);
            _jsonService.RecreateData();
            return HandleResult(result);
        }
        [HttpPut]
        [Route("chinhsuamenu")]
        public async Task<IActionResult> ChinhSuaMenu([FromBody] CSDL_AdminMenu _request)
        {
            var result = await Mediator.Send(new Application.AdminMenu.CapNhatMenu.Command { Entity = _request });
            if (!result.IsSuccess)
                return HandleResult(result);
            _jsonService.RecreateData();
            return HandleResult(result);
        }
        [HttpDelete]
        [Route("xoamenu/{id}")]
        public async Task<IActionResult> XoaMenu(int id)
        {
            var result = await Mediator.Send(new Application.AdminMenu.XoaMenu.Command { Id = id });
            if (!result.IsSuccess)
                return HandleResult(result);
            _jsonService.RecreateData();
            return HandleResult(result);
        }
        [HttpGet]
        [Route("chitietmenu/{id}")]
        public async Task<IActionResult> ChiTietMenu(int id)
        {
            var result = await Mediator.Send(new Application.AdminMenu.ChiTiet.Query { Id = id });
            return HandleResult(result);
        }

        [HttpPost]
        [Route("updatepermissionbatch")]
        public async Task<IActionResult> UpdatePermissionBatch([FromBody] UpdatePermissionRequest _request)
        {
            List<string> lstUpdate = new List<string>();
            foreach (var item in _request.Permission)
            {
                lstUpdate.Add($"{item.PermissionId}|{(item.permitedEdit ? 1 : 0)}|{(item.permitedDelete ? 1 : 0)}|{(item.permitedApprove ? 1 : 0)}|{(item.permitedCreate ? 1 : 0)}");
            }
            string strBatchUpdate = string.Join(",", lstUpdate);

            var result = await Mediator.Send(new Application.AdminMenu.CapNhatCayPhanQuyen.Command { DanhSachPhanQuyen = strBatchUpdate });
            return HandleResult(result);
        }

        [HttpDelete]
        [Route("deletepermissiontree/{menuid}/{rolename}")]
        public async Task<IActionResult> XoaCayPhanQuyen(int menuid, string rolename)
        {
            var result = await Mediator.Send(new Application.AdminMenu.XoaPhanQuyenTheoMenu.Command { MenuId = menuid, RoleName = rolename });
            if (!result.IsSuccess)
                return HandleResult(result);
            _jsonService.RecreateData();
            return HandleResult(result);
        }

        [HttpGet]
        [Route("NhomMenu")]
        public async Task<IActionResult> NhomMenu()
        {
            var result = await Mediator.Send(new Application.EnumHepler.DanhSach.Query());
            return HandleResult(result);
        }
    }
}
