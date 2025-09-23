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
    public class CapNhatPhanQuyen
    {
        public class Command : IRequest<Result<int>>
        {
            public CSDL_MenuPermission permission { get; set; }
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
                    var entity = await _context.CSDL_MenuPermission.FindAsync(request.permission.Id);
                    if (entity == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }

                    entity.PermitedApprove = request.permission.PermitedApprove;
                    entity.PermitedDelete = request.permission.PermitedDelete;
                    entity.PermitedEdit = request.permission.PermitedEdit;
                    entity.PermitedCreate = request.permission.PermitedCreate;

                    int affectedRow = await _context.SaveChangesAsync();

                    if (affectedRow > 0)
                    {
                        return Result<int>.Success(affectedRow);
                    }

                    return Result<int>.Failure("Cập nhật không thành công");
                }
                catch (Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
