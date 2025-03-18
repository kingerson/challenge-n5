namespace MsClean.Application;
using MediatR;
using MsClean.Domain;

public record RequestPermissionNotification(Permission permission) : INotification;
