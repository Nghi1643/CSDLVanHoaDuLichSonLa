using Domain.Core;
using Domain.ResponseDtos;
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
    public class DanhSachMenu
    {
        public class Query : IRequest<Result<List<MenuItem>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<MenuItem>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<MenuItem>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var lstResult = await _context.CSDL_AdminMenu.Select(s => new MenuItem
                    {
                        Id = s.Id,
                        ParentId = s.ParentId,
                        AreaName = s.AreaName,
                        ControllerName = s.ControllerName,
                        ActionName = s.ActionName,
                        Title = s.Title,
                        IsLeaf = s.IsLeaf,
                        IsShow = s.IsShow,
                        DisplayOrder = s.DisplayOrder,
                        Icon = s.Icon
                    })
                        .OrderBy(x => x.DisplayOrder)
                        .ThenBy(x => x.Title)
                        .ToListAsync();

                    for(var i = 0; i < lstResult.Count; i++)
                    {
                        var item = lstResult[i];
                        item.HasChildren = lstResult.Any(x => x.ParentId == item.Id);
                    }

                    return Result<List<MenuItem>>.Success(lstResult);
                }
                catch (Exception ex)
                {
                    return Result<List<MenuItem>>.Failure(ex.Message);
                }
            }
        }
    }
}
