using Application.CQRS.Core;
using Application.Interfaces;
using Application.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Partners.Queries;

public class GetCurrencyListQuery : BaseRequest<ApiResult<IEnumerable<string>>>
{
    //public GetPartnerRequest Model { get; set; }
    //public GetCurrencyListQuery(GetPartnerRequest model) => Model = model;

    public class GetCurrencyListQueryHandler : IRequestHandler<GetCurrencyListQuery, ApiResult<IEnumerable<string>>>
    {
        private readonly ILogger _logger;
        private readonly CurrencyCollection _currencyCollection;

        public GetCurrencyListQueryHandler(ILogger logger, CurrencyCollection currencyCollection)
        {
            _logger = logger;
            _currencyCollection = currencyCollection;
        }

        public async Task<ApiResult<IEnumerable<string>>> Handle(GetCurrencyListQuery request, CancellationToken cancellationToken)
        {
            //if (request.Error != null)
            //{
            //    request.Error = new Error
            //    {
            //        ErrorCode = CustomExceptionCodes.UnHandledException,
            //        ErrorMessage = CustomExceptionCodes.UnHandledException.GetEnumDescription(),
            //        HttpStatus = System.Net.HttpStatusCode.BadRequest
            //    };

            //    string errorMessage = ExceptionMessageBuilder.Build(CustomExceptionCodes.UnHandledException, null, null, request.Model);
            //    await _logger.LogToConsoleAsync(errorMessage);

            //    return ApiResult<string>.ERROR(request.Error);
            //}

            var result = _currencyCollection.GetCurrenciesUppercaseTrimmed();
            return ApiResult<IEnumerable<string>>.OK(result);
        }
    }
}
