namespace MsClean.Application;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MsClean.Infrastructure;

public class PermissionRepository : IPermissionRepository
{

    private readonly ApplicationDbContext _context;
    public PermissionRepository(ApplicationDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<PermissionViewModel>> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<PermissionViewModel> result;

        var query = @"SELECT   [p].[Id], 
                               [p].[EmployeeForename],
                               [p].[EmployeeLastName],
                               [p].[PermissionDate],
                               [p].[PermissionTypeId],
                               [p].[UserRegister],
                               [p].[DateTimeRegister],
                               [p].[IsActive],
                               [pt].[Description] AS PermissionType
                        FROM [Permission] [p]
                        INNER JOIN [PermissionType] [pt] ON [p].[PermissionTypeId] = [pt].[Id]";

        var connection = _context.Database.GetDbConnection();

        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync(cancellationToken); 

        result = await connection.QueryAsync<PermissionViewModel>(query, commandType: CommandType.Text);

        return result;
    }
    public async Task<PermissionViewModel> GetById(int permissionId,CancellationToken cancellationToken)
    {
       PermissionViewModel result;

        var query = @"SELECT   [p].[Id], 
                               [p].[EmployeeForename],
                               [p].[EmployeeLastName],
                               [p].[PermissionDate],
                               [p].[PermissionTypeId],
                               [p].[UserRegister],
                               [p].[DateTimeRegister],
                               [p].[IsActive],
                               [pt].[Description] AS PermissionType
                        FROM [Permission] [p]
                        INNER JOIN [PermissionType] [pt] ON [p].[PermissionTypeId] = [pt].[Id]
                        WHERE [p].[Id] = @PermissionId";

        var parameters = new { PermissionId = permissionId };

        var connection = _context.Database.GetDbConnection();

        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync(cancellationToken);

        result = await connection.QueryFirstOrDefaultAsync<PermissionViewModel>(query, parameters, commandType: CommandType.Text);

        return result;
    }
}
