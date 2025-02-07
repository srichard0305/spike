using System;

namespace spike.Models;

public class Appointment
{
    public string client { get; set; }
    public string pet { get; set; }
    public string petNotes { get; set; }
    public string service { get; set; }
    public string serviceNotes { get; set; }
    public string cost  { get; set; }
    public string comment { get; set; }
    public string clientPhoneNumber { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public string Employee { get; set; }
    
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    
    
}