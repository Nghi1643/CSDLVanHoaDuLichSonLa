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
    public class XoaPhanQuyen
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
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
                    var entity = await _context.CSDL_MenuPermission.FindAsync(request.Id);
                    if (entity == null)
                    {
                        return Result<int>.Failure("Không tìm thấy dữ liệu");
                    }
                    _context.CSDL_MenuPermission.Remove(entity);

                    var deletedRow = await _context.SaveChangesAsync();

                    if (deletedRow <= 0)
                    {
                        return Result<int>.Failure("Xóa dữ liệu không thành công");
                    }

                    return Result<int>.Success(deletedRow);
                }
                catch (Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
