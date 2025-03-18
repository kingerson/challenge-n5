namespace MsClean.Application;
using MediatR;
using MsClean.Domain;

public record ModifyPermissionNotification(Permission permission) : INotification;

