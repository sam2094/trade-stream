using Application.CQRS.Core;
using Application.CQRS.Partners.Queries;
using Application.Extension;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.V1;

[Route("api/v{version:apiVersion}/сurrency")]
[ApiController]
[ApiVersion("1.0")]
public class CurrencyController : BaseController
{
    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<ApiResult<IEnumerable<string>>>> GetCurrencies()
    {
        var query = new GetCurrencyListQuery();
        return await Mediator.HandleRequest(query, HttpContext);
    }
}
