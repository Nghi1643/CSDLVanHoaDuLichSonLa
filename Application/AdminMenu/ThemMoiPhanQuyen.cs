using Domain;
using Domain.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminMenu
{
    public class ThemMoiPhanQuyen
    {
        public class Command : IRequest<Result<int>>
        {
            public CSDL_MenuPermission Entity { get; set; }
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
                    var dkm = await _context.CSDL_MenuPermission.Where(o => o.MenuId == request.Entity.MenuId && o.Rolename == request.Entity.Rolename).FirstOrDefaultAsync();

                    if (dkm != null)
                    {
                        return Result<int>.Failure("Đã tồn tại phân quyền!");
                    }

                    await _context.CSDL_MenuPermission.AddAsync(request.Entity);
                    int insertedRow = await _context.SaveChangesAsync();
                    if (insertedRow > 0)
                    {
                        return Result<int>.Success(insertedRow);
                    }

                    return Result<int>.Failure("Thêm mới thất bại");

                }
                catch(Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
