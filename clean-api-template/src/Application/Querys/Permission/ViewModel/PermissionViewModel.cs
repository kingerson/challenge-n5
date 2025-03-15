namespace MsClean.Application;
using System;

public class PermissionViewModel
{
    public int Id { get; set; }
    public string EmployeeForename { get; set; }
    public string EmployeeLastName { get; set; }
    public DateTime PermissionDate { get; set; }
    public int PermissionTypeId { get; set; }
    public string UserRegister { get; set; }
    public DateTime DateTimeRegister { get; set; }
    public bool IsActive { get; set; }
}
