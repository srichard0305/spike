using System;

namespace spike.Models;

public class Employee
{
    public Int64 EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Address? Address { get; set; }
    public ContactInfo? ContactInfo { get; set; }
    
    public string? Cardinality { get; set; }
    
    //TODO Grooming shit to add?

}