namespace MsClean.Application;
using System.Collections.Generic;
using MediatR;
using MsClean.Domain;

public record GetAllPermissionNotification(IEnumerable<Permission> permissions) : INotification;
