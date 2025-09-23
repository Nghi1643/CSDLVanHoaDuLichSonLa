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
    public class ChiTiet
    {
        public class Query : IRequest<Result<CSDL_AdminMenu>>
        {
            public int Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<CSDL_AdminMenu>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<CSDL_AdminMenu>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = await _context.CSDL_AdminMenu.FindAsync(request.Id);

                    if (entity == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }

                    return Result<CSDL_AdminMenu>.Success(entity);
                }
                catch (Exception ex)
                {
                    return Result<CSDL_AdminMenu>.Failure(ex.Message);
                }
            }
        }
    }
}
