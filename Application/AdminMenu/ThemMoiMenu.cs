using Domain;
using Domain.Core;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Application.AdminMenu
{
    public class ThemMoiMenu
    {
        public class Command : IRequest<Result<int>>
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
                    if (request.Entity.ParentId.HasValue)
                    {
                        var parentEntity = await _context.CSDL_AdminMenu.FindAsync(request.Entity.ParentId.Value);

                        if (parentEntity == null)
                        {
                            throw new Exception("Không tìm thấy thông tin");
                        }

                        //if (parentEntity.IsLeaf)
                        //{
                        //    parentEntity.AreaName = null;
                        //    parentEntity.ControllerName = null;
                        //    parentEntity.ActionName = null;
                        //    parentEntity.IsLeaf = false;
                        //    parentEntity.ParentId = null;
                        //    parentEntity.IsShow = true;

                        //    int updatedRow = await _context.SaveChangesAsync();

                        //    if (updatedRow <= 0)
                        //    {
                        //        throw new Exception("Proccess Error");
                        //    }
                        //}
                    }
                    
                    request.Entity.IsShow = true;

                    await _context.CSDL_AdminMenu.AddAsync(request.Entity);
                    int insertedRow = await _context.SaveChangesAsync();
                    if (insertedRow <= 0)
                    {
                        throw new Exception("Thêm mới không thành công");
                    }

                    return Result<int>.Success(insertedRow);
                }
                catch(Exception ex)
                {
                    return Result<int>.Failure(ex.Message);
                }
            }
        }
    }
}
