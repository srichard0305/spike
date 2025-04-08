using System;

namespace spike.Models;

public class Appointment
{
    public Int64 AppointmentId { get; set; }
    public Client? Client { get; set; }
    public Pet? Pet { get; set; }
    public string? PetNotes { get; set; }
    public string? Service { get; set; }
    public string? ServiceNotes { get; set; }
    public string? Cost  { get; set; }
    public string? Comment { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public string? Cancelled { get; set; }
    public string? NoShow { get; set; }
    public string? Day { get; set; }
    public Employee? EmployeeStylists { get; set; }
    public Employee? EmployeeBookedBy { get; set; }
    
}