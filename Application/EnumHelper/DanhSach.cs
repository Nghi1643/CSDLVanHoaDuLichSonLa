using Domain;
using Domain.Core;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Application.EnumHepler
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<EnumDto>>>
        {

        }

        public class Handler : IRequestHandler<Query, Result<List<EnumDto>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<EnumDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var groups = EnumHelper.GetEnumList<EnumMenuGroup>();
               
                var enumDtoList = groups.Select(group => new EnumDto
                {
                    Value = group.Value,
                    Key = group.Name,
                    Description = group.Description
                }).ToList();

                return Result<List<EnumDto>>.Success(enumDtoList);
            }
        }
    }
}
