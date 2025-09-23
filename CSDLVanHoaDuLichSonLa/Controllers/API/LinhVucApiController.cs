using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class LinhVucApiController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly DataContext _dataContext;

        public LinhVucApiController(IMediator mediator, DataContext dataContext, IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
            _dataContext = dataContext;
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<ActionResult<Result<List<DM_LinhVuc>>>> Gets()
        {
            var list =  await _dataContext.DM_LinhVuc.ToListAsync();
            return Result<List<DM_LinhVuc>>.Success(list);
        }
    }
}
