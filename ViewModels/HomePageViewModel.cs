using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using spike.Data;
using spike.Models;

namespace spike.ViewModels;

public partial class HomePageViewModel : PageViewModel
{
    private readonly int _slotDuration= 5;  
    private readonly int _startHour = 0;      
    private readonly int _endHour = 24;
    private readonly int _employeeColumnWidth = 300;
    
    [ObservableProperty]
    private double _canvasWidth;
    
    public ObservableCollection<Times> Times { get; } = new();
    public ObservableCollection<Employee> Employees { get; set; } = new();
    public ObservableCollection<AppointmentViewModel> Appointments { get; set; }

    public HomePageViewModel()
    {
        PageTitle = AppPageNames.Home;
        
        InitTimes();
        
        Employees.Add(new Employee("Employee1"));
        Employees.Add(new Employee("Employee2"));
        Employees.Add(new Employee("Employee3"));
        Employees.Add(new Employee("Employee4"));
        Employees.Add(new Employee("Employee5"));
        
        CanvasWidth = _employeeColumnWidth * Employees.Count;
        
        var appointments = new List<Appointment>
        {
            new Appointment { client = "Rover", Employee = "Employee1", StartTime = DateTime.Today.AddHours(9), EndTime = DateTime.Today.AddHours(10) },
            new Appointment { client = "Frank", Employee = "Employee1", StartTime = DateTime.Today.AddHours(9.5), EndTime = DateTime.Today.AddHours(11) },
            new Appointment { client = "Rover", Employee = "Employee2", StartTime = DateTime.Today.AddHours(10), EndTime = DateTime.Today.AddHours(12) },
            new Appointment { client = "Rover", Employee = "Employee3", StartTime = DateTime.Today.AddHours(8), EndTime = DateTime.Today.AddHours(11) },
            new Appointment { client = "Rover", Employee = "Employee4", StartTime = DateTime.Today.AddHours(11), EndTime = DateTime.Today.AddHours(13) },
            new Appointment { client = "Rover", Employee = "Employee5", StartTime = DateTime.Today.AddHours(7), EndTime = DateTime.Today.AddHours(9.5) },
        };
        
        Appointments = new ObservableCollection<AppointmentViewModel>(CalculatePositions(appointments));
    }

    private void InitTimes()
    {
        
        Times.Add(new Times("3:00 am"));
        Times.Add(new Times("4:00 am"));
        Times.Add(new Times("5:00 am"));
        Times.Add(new Times("6:00 am"));
        Times.Add(new Times("7:00 am"));
        Times.Add(new Times("8:00 am"));
        Times.Add(new Times("9:00 am"));
        Times.Add(new Times("10:00 am"));
        Times.Add(new Times("11:00 am"));
        Times.Add(new Times("12:00 pm"));
        Times.Add(new Times("1:00 pm"));
        Times.Add(new Times("2:00 pm"));
        Times.Add(new Times("3:00 pm"));
        Times.Add(new Times("4:00 pm"));
        Times.Add(new Times("5:00 pm"));
        Times.Add(new Times("6:00 pm"));
        Times.Add(new Times("7:00 pm"));
        Times.Add(new Times("8:00 pm"));
        Times.Add(new Times("9:00 pm"));
        Times.Add(new Times("10:00 pm"));
        Times.Add(new Times("11:00 pm"));
        Times.Add(new Times("12:00 am"));
        Times.Add(new Times("1:00 am"));
        Times.Add(new Times("2:00 am"));
    }

    // calculates the position and width of each appointment block
    private List<AppointmentViewModel> CalculatePositions(List<Appointment> appointments)
    {
        // group all appointments by employee
        var groupedAppointments = appointments.GroupBy(a => a.Employee);
        var result = new List<AppointmentViewModel>();
        int employeeColumn = 0;
        
        foreach(var group in groupedAppointments)
        {
            // sort appointments by start time 
            var sortedAppointments = group.OrderBy(a => a.StartTime);
            var columns = new List<List<Appointment>>();

            foreach (var appointment in sortedAppointments)
            {
                bool placed = false;
                foreach (var column in columns)
                {
                    // get all appointments that are within half and hour of starting and a add to the same column
                    if (column.Any(a => a.StartTime <= appointment.StartTime && a.StartTime.Minute + 30 <= appointment.StartTime.Minute))
                    {
                        column.Add(appointment);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    columns.Add(new List<Appointment>{appointment});
                }
            }

            for (int i = 0; i < columns.Count; i++)
            {
                int appointmentIndex = 0;
                foreach (var appointment in columns[i])
                {
                    double column = (employeeColumn * _employeeColumnWidth) +
                                    (appointmentIndex * _employeeColumnWidth / columns[i].Count);
                    double width = _employeeColumnWidth/columns[i].Count - 4;
                    result.Add(new AppointmentViewModel(appointment, column, width));
                    appointmentIndex++;
                }
            }

            employeeColumn++;
        }
        
        return result;
    }
    
    
}

