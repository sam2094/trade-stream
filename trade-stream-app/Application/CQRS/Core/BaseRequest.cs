using Application.Interfaces;
using MediatR;

namespace Application.CQRS.Core;
public class BaseRequest<TResponse> : IRequest<TResponse>
{
    public Error Error { get; set; }
    protected ILogger Logger { get; set; }
}
