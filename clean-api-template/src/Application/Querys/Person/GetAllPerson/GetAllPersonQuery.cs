namespace MsClean.Application;
using System.Collections.Generic;
using MediatR;

public sealed record GetAllPersonQuery() : IRequest<IEnumerable<PersonViewModel>>;
