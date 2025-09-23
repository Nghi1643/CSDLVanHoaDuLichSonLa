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
    public class DanhSachMenuParent
    {
        public class Query : IRequest<Result<List<CSDL_AdminMenu>>>
        {

        }
        public class Handler : IRequestHandler<Query, Result<List<CSDL_AdminMenu>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<CSDL_AdminMenu>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = new List<CSDL_AdminMenu>();
                    var nodeRoot = await _context.CSDL_AdminMenu.Where(o => !o.ParentId.HasValue).OrderBy(e => e.DisplayOrder).ToListAsync();
                    if (nodeRoot?.Any() == true)
                    {
                        result.AddRange(nodeRoot);
                        foreach (var item in nodeRoot)
                        {
                            var nodeCap1 = await _context.CSDL_AdminMenu.Where(e => e.ParentId.HasValue && e.ParentId == item.Id).ToListAsync();
                            if (nodeCap1?.Any() == true)
                            {
                                result.AddRange(nodeCap1);
                            }
                        }
                    }
                    else
                    {
                        return Result<List<CSDL_AdminMenu>>.Failure("Không có dữ liệu");
                    }
                   
                    return Result<List<CSDL_AdminMenu>>.Success(result);
                }
                catch(Exception ex)
                {
                    return Result<List<CSDL_AdminMenu>>.Failure(ex.Message);
                }
            }
        }
    }
}
