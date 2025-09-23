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
    public class DanhSachPhanQuyen
    {
        public class Query : IRequest<Result<List<CSDL_MenuPermission>>>
        {
            public int MenuId { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<CSDL_MenuPermission>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<CSDL_MenuPermission>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryResult = await _context.CSDL_MenuPermission.Where(o => o.MenuId == request.MenuId && o.Rolename != "Host").ToListAsync();

                return Result<List<CSDL_MenuPermission>>.Success(queryResult);
            }
        }
    }
}
