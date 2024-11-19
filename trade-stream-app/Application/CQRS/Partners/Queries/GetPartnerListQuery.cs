using Application.CQRS.Core;
using Application.CustomExceptions;
using Application.Extensions;
using Application.Interfaces;
using AutoMapper;
using Domain.DTOs;
using Domain.Enums;
using Domain.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Partners.Queries;

public class GetPartnerListQuery : BaseRequest<ApiResult<PartnerDto>>
{
    public GetPartnerRequest Model { get; set; }
    public GetPartnerListQuery(GetPartnerRequest model) => Model = model;
    public class GetPartnerListQueryHandler : IRequestHandler<GetPartnerListQuery, ApiResult<PartnerDto>>
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetPartnerListQueryHandler(IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResult<PartnerDto>> Handle(GetPartnerListQuery request, CancellationToken cancellationToken)
        {
            if (request.Model.Age < 18)
            {
                request.Error = new Error
                {
                    ErrorCode = CustomExceptionCodes.UnHandledException,
                    ErrorMessage = CustomExceptionCodes.UnHandledException.GetEnumDescription(),
                    HttpStatus = System.Net.HttpStatusCode.BadRequest
                };

                string errorMessage = ExceptionMessageBuilder.Build(CustomExceptionCodes.UnHandledException, null, null, request.Model);
                await _logger.LogToConsoleAsync(errorMessage);

                return ApiResult<PartnerDto>.ERROR(request.Error);
            }

            var result = new PartnerDto
            {
                Age = request.Model.Age,
                Name = Guid.NewGuid().ToString(),
            };

            await _logger.LogToConsoleAsync(result.Name);

            return ApiResult<PartnerDto>.OK(result);
        }
    }
}
