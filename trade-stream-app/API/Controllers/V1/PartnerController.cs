using Application.CQRS.Core;
using Application.CQRS.Partners.Queries;
using Application.Extension;
using Application.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.V1;

[Route("api/v{version:apiVersion}/partner")]
[ApiController]
[ApiVersion("1.0")]
public class PartnerController : BaseController
{
    private readonly ILogger _logger;

    public PartnerController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("list")]
    public async Task<ActionResult<ApiResult<PartnerDto>>> PartnerList(GetPartnerRequest input)
    {
        var query = new GetPartnerListQuery(input);
        return await Mediator.HandleRequest(query, HttpContext);
    }
}
