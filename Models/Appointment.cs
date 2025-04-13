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
    public string? CommentFromLastApponitmnet { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string? Cancelled { get; set; }
    public string? NoShow { get; set; }
    public DateTimeOffset? Date { get; set; }
    public Employee? EmployeeStylists { get; set; }
    public Employee? EmployeeBookedBy { get; set; }
    public string FormattedStartTime => DateTime.Today.Add(StartTime.GetValueOrDefault()).ToString("hh:mm tt").ToLower();
    public string FormattedEndTime => DateTime.Today.Add(EndTime.GetValueOrDefault()).ToString("hh:mm tt").ToLower();
    public string FormattedDate => Date?.ToString("dddd, MMMM dd yyyy");

}
    
