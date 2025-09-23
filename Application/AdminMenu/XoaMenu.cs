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
    public class XoaMenu
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
                    int parentId = 0;
                    var entity = await _context.CSDL_AdminMenu.FindAsync(request.Id);

                    if (entity == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }

                    if (entity.ParentId.HasValue)
                        parentId = entity.ParentId.Value;

                    if (!entity.IsLeaf)
                    {
                        var lstChildren = await _context.CSDL_AdminMenu.Where(o => o.ParentId == entity.Id).ToListAsync();

                        if (lstChildren.Count > 0)
                        {
                            _context.CSDL_AdminMenu.RemoveRange(lstChildren);

                            int affectRow = await _context.SaveChangesAsync();

                            if (affectRow <= 0)
                            {
                                throw new Exception("Không xóa được node con");
                            }
                        }                        
                    }

                    _context.CSDL_AdminMenu.Remove(entity);
                    int removedRow = await _context.SaveChangesAsync();
                    if (removedRow <= 0)
                    {
                        throw new Exception("Xóa node không thành công");
                    }

                    if (parentId != 0) {
                        var parentNode = await _context.CSDL_AdminMenu.FindAsync(parentId);
                        if (parentNode != null)
                        {
                            var lstSiblingNode = await _context.CSDL_AdminMenu.Where(o => o.ParentId == parentId).ToListAsync();
                            if (!lstSiblingNode.Any())
                            {
                                parentNode.IsLeaf = true;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    return Result<int>.Success(removedRow);
                }
                catch (Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
