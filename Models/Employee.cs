using System;

namespace spike.Models;

public class Employee
{
    Int64 EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Address? Address { get; set; }
    public ContactInfo? ContactInfo { get; set; }

    public Employee(string employeeId)
    {
        FirstName = employeeId;
    }
   
}