namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MsClean.Infrastructure;
using MsClean.Domain;

public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExecutionStrategyWrapper _executionStrategyWrapper;
    private readonly IMediator _mediator;
    public RequestPermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IExecutionStrategyWrapper executionStrategyWrapper,
        IMediator mediator
        )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _executionStrategyWrapper = executionStrategyWrapper ?? throw new ArgumentNullException(nameof(executionStrategyWrapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    public async Task<int> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = new Permission();
        permission.Register(request.EmployeeName, request.EmployeeLastName, request.PermissionTypeId, DateTime.UtcNow);

        var existPermissionType = await _unitOfWork.Repository<PermissionType>().AnyAsync(m => m.Id == request.PermissionTypeId, cancellationToken);

        if (!existPermissionType)
            throw new MsCleanException($"Permission Type with id : {request.PermissionTypeId} not found");

        await _executionStrategyWrapper.ExecuteAsync(async () =>
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.Repository<Permission>().Add(permission);
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

        await _mediator.Publish(new RequestPermissionNotification(permission), cancellationToken);

        return permission.Id;
    }
}
