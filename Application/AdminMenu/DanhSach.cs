using Domain;
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
    public class DanhSach
    {
        public class Query : IRequest<Result<List<MenuItem>>>
        {
            public int? ParentId { get; set; }
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

                    var lstResult = await _context.CSDL_AdminMenu.Where(o => o.ParentId == (request.ParentId.HasValue ? request.ParentId.Value : null)).Select(s => new MenuItem
                    {
                        Id = s.Id,
                        ParentId = s.ParentId,
                        AreaName = s.AreaName,
                        ControllerName = s.ControllerName,
                        ActionName = s.ActionName,
                        Title = s.Title,
                        IsLeaf = s.IsLeaf,
                        HasChildren = !s.IsLeaf
                    }).ToListAsync();
                    //var lstResult = await _context.OD_AdminMenu.Where(o => o.ParentId == (request.ParentId.HasValue ? request.ParentId.Value : null)).ToListAsync();

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
