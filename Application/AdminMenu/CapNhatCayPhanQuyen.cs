using Dapper;
using Domain.Core;
using Domain.ResponseDtos;
using MediatR;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminMenu
{
    public class CapNhatCayPhanQuyen
    {
        public class Command : IRequest<Result<int>>
        {
            public string DanhSachPhanQuyen { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<int>>
        {
            private readonly IConfiguration _configuration;
            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@pUpdateList", request.DanhSachPhanQuyen);
                        var updateResult = await connettion.ExecuteScalarAsync<int>("spu_MenuPermission_BatchUpdate", parameters, commandType: System.Data.CommandType.StoredProcedure);
                        return Result<int>.Success(updateResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<int>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connettion.CloseAsync();
                    }
                }
            }
        }
    }
}
