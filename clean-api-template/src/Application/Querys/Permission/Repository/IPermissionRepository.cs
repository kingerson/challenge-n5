namespace MsClean.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPermissionRepository
{
    Task<IEnumerable<PermissionViewModel>> GetAll(CancellationToken cancellationToken);
    Task<PermissionViewModel> GetById(int permissionId, CancellationToken cancellationToken);
}
