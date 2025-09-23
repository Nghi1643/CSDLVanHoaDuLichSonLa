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

namespace Application.TaiKhoan
{
    public class ChiTiet
    {
        public class Query : IRequest<Result<ChiTietTaiKhoan>>
        {
            public Int64 UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ChiTietTaiKhoan>>
        {
            private readonly IConfiguration _configuration;
            private readonly DataContext _context;
            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _configuration = configuration;
                _context = dataContext;
            }
            public async Task<Result<ChiTietTaiKhoan>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@pUserId", request.UserId);
                        var queryResult = await connettion.QueryFirstOrDefaultAsync<ChiTietTaiKhoan>("spu_User_Detail", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        if (queryResult == null)
                        {
                            throw new Exception("Không tìm thấy tài khoản");
                        }

                        return Result<ChiTietTaiKhoan>.Success(queryResult);
                    }
                    catch (Exception ex)
                    {
                        return Result<ChiTietTaiKhoan>.Failure(ex.Message);
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
