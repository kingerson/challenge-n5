namespace MsClean.Domain;

public class PermissionType : Entity
{
    public PermissionType(string description) => Description = description;
    public string Description { get; private set; }

    public ICollection<Permission> Permissions { get; private set; } = [];
}
