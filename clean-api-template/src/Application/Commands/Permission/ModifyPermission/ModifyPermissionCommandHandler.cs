namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExecutionStrategyWrapper _executionStrategyWrapper;
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    public ModifyPermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IExecutionStrategyWrapper executionStrategyWrapper,
        IKakfaService kakfaService,
        IElasticSearchService<Permission> elasticSearchService
        )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _executionStrategyWrapper = executionStrategyWrapper ?? throw new ArgumentNullException(nameof(executionStrategyWrapper));
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
    }
    public async Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetById(request.Id) ?? throw new MsCleanException($"Permission with id : {request.Id} not found");

        permission.Modify(request.EmployeeName, request.EmployeeLastName, request.PermissionTypeId);

        await _executionStrategyWrapper.ExecuteAsync(async () =>
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.Repository<Permission>().Update(permission);
                    await _unitOfWork.SaveEntitiesAsync(cancellationToken);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new MsCleanException($"Error: {ex.Message}");
                }
            }
        });

        _ = await _kakfaService.ProduceAsync("test-topic", "modify");

        _ = await _elasticSearchService.IndexAsync(permission, "modify-permissions");

        return true;
    }
}
