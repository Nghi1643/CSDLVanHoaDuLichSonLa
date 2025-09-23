using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class ChiTiet
    {
        public class Query : IRequest<AppUser>
        {
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Query, AppUser>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<AppUser> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.AppUser.SingleOrDefaultAsync(x => x.UserName == request.UserName);
            }
        }
    }
}
