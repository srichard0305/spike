using spike.Models;

namespace spike.ViewModels;

public class AppointmentViewModel : ViewModelBase
{
    public Appointment Appointment { get; set; }

   public double TopPosition => (Appointment.StartTime.Value.Hours * 100 + (Appointment.StartTime.Value.Minutes * 1.67)) - 500;
   
   public double Duration => (((Appointment.EndTime - Appointment.StartTime).Value.TotalMinutes * 1.67) - 4) < 20
       ? (((Appointment.EndTime - Appointment.StartTime).Value.TotalMinutes * 1.67) - 4) + 20
       : ((Appointment.EndTime - Appointment.StartTime).Value.TotalMinutes * 1.67) - 4;
   
    public double Column { get; set; }
    public double Width { get; set;  }
    
    
    public AppointmentViewModel(Appointment appointment, double column, double width)
    {
        Appointment = appointment;
        Column = column;
        Width = width;
    }

   
    
    
    
}