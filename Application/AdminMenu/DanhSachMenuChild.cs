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
    public class DanhSachMenuChild
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
                    var lstResult = await _context.CSDL_AdminMenu.Where(o => o.IsLeaf).OrderBy(e => e.DisplayOrder).ToListAsync();

                    return Result<List<CSDL_AdminMenu>>.Success(lstResult);
                }
                catch (Exception ex)
                {
                    return Result<List<CSDL_AdminMenu>>.Failure(ex.Message);
                }
            }
        }
    }
}
