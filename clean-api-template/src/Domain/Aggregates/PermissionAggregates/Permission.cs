namespace MsClean.Domain;
public class Permission : Entity
{
    public string EmployeeForename { get; private set; }
    public string EmployeeLastName { get; private set; }
    public int PermissionTypeId { get; private set; }
    public DateTime PermissionDate { get; private set; }
    public PermissionType PermissionType { get; private set; }

    public void Register(
        string employeeForename,
        string employeeLastName,
        int permissionTypeId,
        DateTime permissionDate
        )
    {
        EmployeeForename = employeeForename;
        EmployeeLastName = employeeLastName;
        PermissionTypeId = permissionTypeId;
        PermissionDate = permissionDate;
    }

    public void Modify(
        string employeeForename,
        string employeeLastName,
        int permissionTypeId)
    {
        EmployeeForename = employeeForename;
        EmployeeLastName = employeeLastName;
        PermissionTypeId = permissionTypeId;
    }
}
