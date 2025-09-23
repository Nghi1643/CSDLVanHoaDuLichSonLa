using CSDLVanHoaDuLichSonLa.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace CSDLVanHoaDuLichSonLa.Areas.AdminTool.Controllers
{
    [Area("AdminTool")]
    public class AdminControllerBase : Controller
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected async Task<ViewPermissionViewModel> getPermission()
        {
            ViewPermissionViewModel vm = new ViewPermissionViewModel
            {
                PermitedView = 0,
                PermitedEdit = 0,
                PermitedDelete = 0,
                PermitedApprove = 0,
                PermitedCreate = 0
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            string areaName = ControllerContext.RouteData.Values["area"].ToString();
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            string actionName = ControllerContext.RouteData.Values["action"].ToString();

            if (claimsIdentity != null && claimsIdentity.Name != null)
            {
                string strRoles = claimsIdentity.FindFirst("RolesList").Value;
                ViewPermissionViewModel vma = new ViewPermissionViewModel();
                if (!string.IsNullOrEmpty(strRoles) && (strRoles.Contains("Host") || strRoles.Contains("SystemAdmin")))
                {
                    vma = new ViewPermissionViewModel
                    {
                        PermitedView = 1,
                        PermitedEdit = 1,
                        PermitedDelete = 1,
                        PermitedApprove = 1,
                        PermitedCreate = 1
                    };
                }
                else
                {
                    var permissionResult = await Mediator.Send(new Application.Users.GetPermission.Query { AreaName = areaName, ControllerName = controllerName, ActionName = actionName, RolesName = strRoles });

                    vma = new ViewPermissionViewModel
                    {
                        PermitedView = permissionResult.Value.PermitedView.Value ? 1 : 0,
                        PermitedEdit = permissionResult.Value.PermitedEdit.Value ? 1 : 0,
                        PermitedDelete = permissionResult.Value.PermitedDelete.Value ? 1 : 0,
                        PermitedApprove = permissionResult.Value.PermitedApprove.Value ? 1 : 0,
                        PermitedCreate = permissionResult.Value.PermitedCreate.Value ? 1 : 0
                    };
                }
                return vma;
            }

            return vm;
        }
    }
}
