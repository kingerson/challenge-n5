namespace MsClean.Application;
using MediatR;
using MsClean.Domain;

public record GetPermissionNotification(Permission permission) : INotification;
