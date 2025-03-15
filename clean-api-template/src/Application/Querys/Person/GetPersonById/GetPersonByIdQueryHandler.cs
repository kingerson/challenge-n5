namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonViewModel>
{
    private readonly IPersonQueryRepository _personQueryRepository;
    public GetPersonByIdQueryHandler(IPersonQueryRepository personQueryRepository)
    {
        _personQueryRepository = personQueryRepository ?? throw new ArgumentNullException(nameof(personQueryRepository));
    }

    public async Task<PersonViewModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _personQueryRepository.GetById(request, cancellationToken);

        return result;
    }
}
