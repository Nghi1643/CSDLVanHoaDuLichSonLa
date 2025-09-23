using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class TestApiController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;

        public TestApiController(IMediator mediator, IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        //[HttpPost]
        //[Route("Gets")]
        //public async Task<ActionResult<Result<List<Test12>>>> Gets(Test12 request)
        //{
        //    return await Mediator.Send(new DanhSach.Query { Request = request });
        //}
    }
}
