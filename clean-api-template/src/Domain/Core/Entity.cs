namespace MsClean.Domain;
using System;

public abstract class Entity
{
    public int Id { get; set; }
    public string UserRegister { get; set; }
    public string? UserUpdated { get; set; }
    public DateTime? DateTimeUpdated { get; set; }
    public DateTime DateTimeRegister { get; set; }
    public bool IsActive { get; set; }
}
