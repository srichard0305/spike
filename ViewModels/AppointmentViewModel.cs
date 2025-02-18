using spike.Models;

namespace spike.ViewModels;

public class AppointmentViewModel : ViewModelBase
{
    public Appointment Appointment { get; }

    public double TopPosition => (Appointment.StartTime.Hour * 100 + (Appointment.StartTime.Minute * 1.67)) - 300;
    
    public double Duration => ((Appointment.EndTime - Appointment.StartTime).TotalMinutes * 1.67) - 4;
    
    public double Column { get; set; }
    public double Width { get; set;  }

    public string Client => Appointment.client;
    
    public AppointmentViewModel(Appointment appointment, double column, double width)
    {
        this.Appointment = appointment;
        this.Column = column;
        this.Width = width;
    }

   
    
    
    
}