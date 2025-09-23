using Domain;
using Domain.Core;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminMenu
{
    public class CapNhatMenu
    {
        public class Command: IRequest<Result<int>>
        {
            public CSDL_AdminMenu Entity { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var currentEntity = await _context.CSDL_AdminMenu.FindAsync(request.Entity.Id);
                    if(currentEntity == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu!");
                    }
                    currentEntity.Title = request.Entity.Title;
                    currentEntity.DisplayOrder = request.Entity.DisplayOrder;
                    currentEntity.Icon = request.Entity.Icon;

                    currentEntity.AreaName = request.Entity.AreaName;
                    currentEntity.ControllerName = request.Entity.ControllerName;
                    currentEntity.ActionName = request.Entity.ActionName;
                    currentEntity.IsShow = request.Entity.IsShow;

                    // Không ràng buộc chỉ menu cấp con vì có cả loại menu chỉ có một mình
                    //if (currentEntity.IsLeaf)
                    //{
                    //    currentEntity.AreaName = request.Entity.AreaName;
                    //    currentEntity.ControllerName = request.Entity.ControllerName;
                    //    currentEntity.ActionName = request.Entity.ActionName;
                    //}

                    var updatedRow = await _context.SaveChangesAsync();

                    if (updatedRow <= 0)
                    {
                        throw new Exception("Cập nhật không thành công");
                    }
                    return Result<int>.Success(updatedRow);
                }
                catch (Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
