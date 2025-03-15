namespace MsClean.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPersonQueryRepository
{
    Task<IEnumerable<PersonViewModel>> GetAll(CancellationToken cancellationToken);
    Task<PersonViewModel> GetById(GetPersonByIdQuery request, CancellationToken cancellationToken);
}
