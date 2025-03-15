namespace MsClean.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using MsClean.Infrastructure;

public class GetAllPersonQueryHandler : IRequestHandler<GetAllPersonQuery, IEnumerable<PersonViewModel>>
{
    private readonly IPersonQueryRepository _personQueryRepository;
    private readonly IMemoryCacheService _memoryCacheService;
    public GetAllPersonQueryHandler(
        IPersonQueryRepository personQueryRepository,
        IMemoryCacheService memoryCacheService
        )
    {
        _personQueryRepository = personQueryRepository ?? throw new ArgumentNullException(nameof(personQueryRepository));
        _memoryCacheService = memoryCacheService ?? throw new ArgumentNullException(nameof(memoryCacheService));
    }

    public async Task<IEnumerable<PersonViewModel>> Handle(GetAllPersonQuery request, CancellationToken cancellationToken)
    {
        if (!_memoryCacheService.TryGetValue("GetPerson", out IEnumerable<PersonViewModel> result))
        {
            result = await Get(cancellationToken);
            _memoryCacheService.SetValue("GetPerson", result);
        }

        return result;
    }

    private async Task<IEnumerable<PersonViewModel>> Get(CancellationToken cancellationToken)
    {
        var result = await _personQueryRepository.GetAll(cancellationToken);

        return result;
    }
}
