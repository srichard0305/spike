using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using spike.Data;
using spike.Database;
using spike.Models;

namespace spike.ViewModels;

public partial class HomePageViewModel : PageViewModel
{  
    private readonly int _employeeColumnWidth = 300;
    
    [ObservableProperty]
    private double _canvasWidth;
    
    [ObservableProperty]
    private DateTimeOffset? _selectedDate;
    
    [ObservableProperty]
    private ObservableCollection<Times> _times;
    
    [ObservableProperty]
    private ObservableCollection<Employee> _employees;
    
    [ObservableProperty]
    private ObservableCollection<Appointment> _appointments;
    
    [ObservableProperty]
    private ObservableCollection<AppointmentViewModel> _appointmentViewModels;

    public HomePageViewModel()
    {
        PageTitle = AppPageNames.Home;
        
        // set date to today
        SelectedDate = DateTimeOffset.Now;
        
        Times = new ObservableCollection<Times>();
        InitTimes();
        
        Employees = new ObservableCollection<Employee>();
        InitEmployees();
        
        Appointments = new ObservableCollection<Appointment>();
        InitAppointments(SelectedDate);
        
        CanvasWidth = _employeeColumnWidth * Employees.Count;
        
    }

    private void InitTimes()
    {
        
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
        Times.Add(new Times("3:00 am"));
        Times.Add(new Times("4:00 am"));
        
    }
    
    private void InitEmployees()
    {
        Employees = ReadFromDatabase.GetAllEmployees();
    }

    private void InitAppointments(DateTimeOffset? date)
    {
        Appointments = ReadFromDatabase.GetAppointments(date);
        AppointmentViewModels = CalculatePositions(Appointments);
    }
    
    partial void OnSelectedDateChanged(DateTimeOffset? value)
    {
        if (value != null)
        {
            InitAppointments(value);
        }
    }

    // calculates the position and width of each appointment block
    private ObservableCollection<AppointmentViewModel> CalculatePositions(ObservableCollection<Appointment> appointments)
    {
        // group all appointments by employee
        var groupedAppointments = appointments.GroupBy(a => a.EmployeeStylists);
        var result = new ObservableCollection<AppointmentViewModel>();
        int employeeColumn = 0;
        
        foreach(var group in groupedAppointments)
        {
            // sort appointments by start time 
            var sortedAppointments = group.OrderBy(a => a.StartTime);
            var columns = new ObservableCollection<ObservableCollection<Appointment>>();

            foreach (var appointment in sortedAppointments)
            {
                bool placed = false;
                foreach (var column in columns)
                {
                    // get all appointments that are within half an hour of starting and add to the same column
                    if (column.Any(a => a.StartTime <= appointment.StartTime 
                                        && a.StartTime.Value.Minutes + 30 <= appointment.StartTime.Value.Minutes))
                    {
                        column.Add(appointment);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    columns.Add(new ObservableCollection<Appointment>{appointment});
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

