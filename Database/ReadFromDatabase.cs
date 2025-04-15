using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Data.Sqlite;
using spike.Models;
using spike.ViewModels;

namespace spike.Database;

public static class ReadFromDatabase
{
    public static ObservableCollection<Client> GetAllClients()
    {
        ObservableCollection<Client> clients = new ObservableCollection<Client>();

        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var sqlStatment = @"
                SELECT * FROM Clients
                WHERE IsActive = True;
            ";
            
            // grab all clients from database
            using var clientInfo = new SqliteCommand(sqlStatment, connection);

            using var clientReader = clientInfo.ExecuteReader();
            if (clientReader.HasRows)
            {
                while (clientReader.Read())
                {
                    Client client = new()
                    {
                        ClientId = (Int64)clientReader["Client_Id"],
                        FirstName = (string)clientReader["First_Name"],
                        LastName = (string)clientReader["Last_Name"]
                    };
                    
                    // grab the address for each client
                    sqlStatment = @"
                        SELECT Address, Etc, City, Country, Province, Postal_Code
                        FROM ClientAddress
                        WHERE Client_Id = @ClientId;
                    ";
                    
                    using var addressInfo = new SqliteCommand(sqlStatment, connection);
                    addressInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                    using var addressReader = addressInfo.ExecuteReader();
                    
                    client.Address = new Address();
                    
                    if (addressReader.HasRows)
                    {
                        while (addressReader.Read()){
                            client.Address.AddressLine = (string)addressReader["Address"];
                            client.Address.Etc =  addressReader["Etc"] == DBNull.Value ? "" : (string)addressReader["Etc"];
                            client.Address.City = (string)addressReader["City"];
                            client.Address.Country = (string)addressReader["Country"];
                            client.Address.Province = (string)addressReader["Province"];
                            client.Address.PostalCode = (string)addressReader["Postal_Code"];
                        }
                    }
                    
                    //grab contact info
                    sqlStatment = @"
                        SELECT PrimaryPhone, SecondaryPhone, EmergencyPhone, EmergencyName, Email
                        FROM ClientContact
                        WHERE Client_Id = @ClientId;
                    ";
                    
                    using var contactInfo = new SqliteCommand(sqlStatment, connection);
                    contactInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                    using var contactReader = contactInfo.ExecuteReader();
                    
                     client.ContactInfo = new ContactInfo();

                    if (contactReader.HasRows)
                    {
                        while (contactReader.Read())
                        {
                            client.ContactInfo.PrimaryPhone = (string)contactReader["PrimaryPhone"];
                            client.ContactInfo.SecondaryPhone = contactReader["SecondaryPhone"] == DBNull.Value ? "" : (string)contactReader["SecondaryPhone"];
                            client.ContactInfo.EmergencyPhone = contactReader["EmergencyPhone"] == DBNull.Value ? "" : (string)contactReader["EmergencyPhone"];
                            client.ContactInfo.EmergencyName = contactReader["EmergencyName"] == DBNull.Value ? "" : (string)contactReader["EmergencyName"]; 
                            client.ContactInfo.Email = contactReader["Email"] == DBNull.Value ? "" : (string)contactReader["Email"];
                        }
                    }
                    
                    // grab all pets
                    sqlStatment = @"
                        SELECT * FROM Pets
                        WHERE Client_Id = @ClientId AND
                        IsActive = True;
                    ";
                    
                    using var petsInfo = new SqliteCommand(sqlStatment, connection);
                    petsInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                    using var petsReader = petsInfo.ExecuteReader();

                    if (petsReader.HasRows)
                    {
                        client.Pets = new ObservableCollection<Pet>();
                        while (petsReader.Read())
                        {
                            Pet pet = new()
                            {
                                PetId = (Int64)petsReader["Pet_Id"],
                                Name = (string)petsReader["Name"],
                                Breed = (string)petsReader["Breed"],
                                Age = (string)petsReader["Age"],
                                Birthday = petsReader["Birthday"] == DBNull.Value ? "" : (string)petsReader["Birthday"],
                                Gender = (string)petsReader["Gender"],
                                Health = petsReader["Health"] == DBNull.Value ? "" : (string)petsReader["Health"],
                                SpayedNeutered = (string)petsReader["Spayed_Neutered"], 
                                VetName = petsReader["Vet_Name"] == DBNull.Value ? "" : (string)petsReader["Vet_Name"],
                                VetPhone = petsReader["Vet_Phone"] == DBNull.Value ? "" : (string)petsReader["Vet_Phone"],
                                Vaccines = petsReader["Vaccines"] == DBNull.Value ? "" : (string)petsReader["Vaccines"],
                                
                            };
                            
                            client.Pets.Add(pet);
                        }
                    }
                    
                    clients.Add(client);
                }
            }
            
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
        }

        return clients;
    }
    
    public static ObservableCollection<Employee> GetAllEmployees()
    {
        ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var sqlStatment = @"
                SELECT * FROM Employees
                WHERE IsActive = True;
            ";
            
            // grab all clients from database
            using var employeeInfo = new SqliteCommand(sqlStatment, connection);

            using var employeeReader = employeeInfo.ExecuteReader();
            if (employeeReader.HasRows)
            {
                while (employeeReader.Read())
                {
                    Employee employee = new()
                    {
                        EmployeeId = (Int64)employeeReader["Employee_Id"],
                        FirstName = (string)employeeReader["First_Name"],
                        LastName = (string)employeeReader["Last_Name"],
                        Cardinality =  employeeReader["Cardinality"] == DBNull.Value ? "" : (string)employeeReader["Cardinality"],
                        Commission =  employeeReader["Commission"] == DBNull.Value ? "" : (string)employeeReader["Commission"],
                        BasePay = employeeReader["Base_Pay"] == DBNull.Value ? "" : (string)employeeReader["Base_Pay"]
                    };
                    
                    // grab the address for each employee
                    sqlStatment = @"
                        SELECT Address, Etc, City, Country, Province, PostalCode
                        FROM EmployeeAddress
                        WHERE Employee_Id = @EmployeeId;
                    ";
                    
                    using var addressInfo = new SqliteCommand(sqlStatment, connection);
                    addressInfo.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    using var addressReader = addressInfo.ExecuteReader();
                    
                    employee.Address = new Address();
                    
                    if (addressReader.HasRows)
                    {
                        while (addressReader.Read()){
                            employee.Address.AddressLine = (string)addressReader["Address"];
                            employee.Address.Etc =  addressReader["Etc"] == DBNull.Value ? "" : (string)addressReader["Etc"];
                            employee.Address.City = (string)addressReader["City"];
                            employee.Address.Country = (string)addressReader["Country"];
                            employee.Address.Province = (string)addressReader["Province"];
                            employee.Address.PostalCode = (string)addressReader["PostalCode"];
                        }
                    }
                    
                    //grab contact info
                    sqlStatment = @"
                        SELECT PrimaryPhone, SecondaryPhone, EmergencyPhone, EmergencyName, Email
                        FROM EmployeeContact
                        WHERE Employee_Id = @EmployeeId;
                    ";
                    
                    using var contactInfo = new SqliteCommand(sqlStatment, connection);
                    contactInfo.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    using var contactReader = contactInfo.ExecuteReader();
                    
                    employee.ContactInfo = new ContactInfo();

                    if (contactReader.HasRows)
                    {
                        while (contactReader.Read())
                        {
                            employee.ContactInfo.PrimaryPhone = (string)contactReader["PrimaryPhone"];
                            employee.ContactInfo.SecondaryPhone = contactReader["SecondaryPhone"] == DBNull.Value ? "" : (string)contactReader["SecondaryPhone"];
                            employee.ContactInfo.EmergencyPhone = contactReader["EmergencyPhone"] == DBNull.Value ? "" : (string)contactReader["EmergencyPhone"];
                            employee.ContactInfo.EmergencyName = contactReader["EmergencyName"] == DBNull.Value ? "" : (string)contactReader["EmergencyName"]; 
                            employee.ContactInfo.Email = contactReader["Email"] == DBNull.Value ? "" : (string)contactReader["Email"];
                        }
                    }
                    employees.Add(employee);
                }
            }
            
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
        }

        return employees;
    }

    public static ObservableCollection<Appointment> GetAppointments(DateTimeOffset? currentDate)
    {
        ObservableCollection<Appointment> appointments = new ObservableCollection<Appointment>();

        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();

            var sqlStatement = @"
                SELECT * FROM Appointments
                WHERE Date = @Date;
            ";
            
            string date = currentDate?.ToString("yyyy/MM/dd");
            
            // grab all clients from database
            using var appointmentInfo = new SqliteCommand(sqlStatement, connection);
            appointmentInfo.Parameters.AddWithValue("@Date", date);
            using var appointmentReader = appointmentInfo.ExecuteReader();
            
            if (appointmentReader.HasRows)
            {
                while (appointmentReader.Read())
                {
                    Appointment appointment = new Appointment();
                    appointment.Client = new Client();
                    appointment.Client.ContactInfo = new ContactInfo();
                    appointment.Pet = new Pet();
                    appointment.EmployeeStylists = new Employee();
                    appointment.EmployeeBookedBy = new Employee();
                    
                    appointment.AppointmentId = (Int64)appointmentReader["Appointment_Id"];
                    appointment.PetNotes = appointmentReader["Pet_Notes"] == DBNull.Value ? "" : (string)appointmentReader["Pet_Notes"];
                    appointment.Service  = (string)appointmentReader["Service"];
                    appointment.ServiceNotes = appointmentReader["Service_Notes"] == DBNull.Value ? "" : (string)appointmentReader["Service_Notes"];
                    appointment.Cost = (string)appointmentReader["Cost"];
                    // TODO grab last appointment for comments
                    appointment.CommentFromLastApponitmnet = appointmentReader["Service_Notes"] == DBNull.Value ? "" : (string)appointmentReader["Service_Notes"];
                    appointment.StartTime = TimeSpan.Parse(appointmentReader["StartTime"].ToString());
                    appointment.EndTime = TimeSpan.Parse(appointmentReader["EndTime"].ToString());
                    appointment.CheckIn = (string)appointmentReader["CheckIn"];
                    appointment.CheckOut = (string)appointmentReader["CheckOut"];
                    appointment.Cancelled = appointmentReader["Cancelled"] == DBNull.Value ? "" : (string)appointmentReader["Cancelled"];
                    appointment.NoShow = appointmentReader["NoShow"] == DBNull.Value ? "" : (string)appointmentReader["NoShow"];
                    appointment.Date = DateTimeOffset.Parse(appointmentReader["Date"].ToString());
                    
                    // grab client info 
                    sqlStatement = @"
                        SELECT Client_Id, First_Name, Last_Name 
                        FROM Clients
                        WHERE Client_Id = @ClientId;
                    ";
                    using var clientsInfo = new SqliteCommand(sqlStatement, connection);
                    clientsInfo.Parameters.AddWithValue("@ClientId", (Int64)appointmentReader["Client_Id"]);
                    using var clientReader = clientsInfo.ExecuteReader();
                    if (clientReader.HasRows)
                    {
                        while (clientReader.Read())
                        {
                            appointment.Client.ClientId = (Int64)clientReader["Client_Id"];
                            appointment.Client.FirstName = (string)clientReader["First_Name"];
                            appointment.Client.LastName = (string)clientReader["Last_Name"];
                        }
                    }
                    
                    sqlStatement = @"
                        SELECT * FROM ClientContact               
                        WHERE Client_Id = @ClientId;
                    ";
                    using var clientContactInfo = new SqliteCommand(sqlStatement, connection);
                    clientContactInfo.Parameters.AddWithValue("@ClientId", (Int64)appointmentReader["Client_Id"]);
                    using var clientContactReader = clientContactInfo.ExecuteReader();
                    if (clientContactReader.HasRows)
                    {
                        while (clientContactReader.Read())
                        {
                            appointment.Client.ContactInfo.PrimaryPhone = (string)clientContactReader["PrimaryPhone"];
                            appointment.Client.ContactInfo.SecondaryPhone = clientContactReader["SecondaryPhone"] == DBNull.Value ? "" : (string)clientContactReader["SecondaryPhone"];
                            appointment.Client.ContactInfo.EmergencyPhone = clientContactReader["EmergencyPhone"] == DBNull.Value ? "" : (string)clientContactReader["EmergencyPhone"];
                            appointment.Client.ContactInfo.EmergencyName = clientContactReader["EmergencyName"] == DBNull.Value ? "" : (string)clientContactReader["EmergencyName"]; 
                            appointment.Client.ContactInfo.Email = clientContactReader["Email"] == DBNull.Value ? "" : (string)clientContactReader["Email"];
                            
                        }
                    }
                    
                    //get employee stylist
                    sqlStatement = @"
                        SELECT Employee_Id, First_Name, Last_Name 
                        FROM Employees               
                        WHERE Employee_Id = @EmployeeId;
                    ";
                    using var employeeInfo = new SqliteCommand(sqlStatement, connection);
                    employeeInfo.Parameters.AddWithValue("@EmployeeId", (Int64)appointmentReader["Employee_Id"]);
                    using var employeeReader = employeeInfo.ExecuteReader();
                    if (employeeReader.HasRows)
                    {
                        while (employeeReader.Read())
                        {
                            appointment.EmployeeStylists.EmployeeId = (Int64)employeeReader["Employee_Id"];
                            appointment.EmployeeStylists.FirstName = (string)employeeReader["First_Name"];
                            appointment.EmployeeStylists.LastName = (string)employeeReader["Last_Name"];
                        }
                    }
                    
                    //get employee booked by
                    sqlStatement = @"
                        SELECT Employee_Id, First_Name, Last_Name 
                        FROM Employees               
                        WHERE Employee_Id = @BookedBy;
                    ";
                    using var employeeBookedByInfo = new SqliteCommand(sqlStatement, connection);
                    employeeBookedByInfo.Parameters.AddWithValue("@BookedBy", (Int64)appointmentReader["BookedBy"]);
                    using var employeeBookedByReader = employeeBookedByInfo.ExecuteReader();
                    if (employeeBookedByReader.HasRows)
                    {
                        while (employeeBookedByReader.Read())
                        {
                            appointment.EmployeeBookedBy.EmployeeId = (Int64)employeeBookedByReader["Employee_Id"];
                            appointment.EmployeeBookedBy.FirstName = (string)employeeBookedByReader["First_Name"];
                            appointment.EmployeeBookedBy.LastName = (string)employeeBookedByReader["Last_Name"];
                        }
                    }
                    
                    //get pet
                    sqlStatement = @"
                        SELECT * FROM Pets               
                        WHERE Pet_Id = @PetId;
                    ";
                    using var petInfo = new SqliteCommand(sqlStatement, connection);
                    petInfo.Parameters.AddWithValue("@PetId", (Int64)appointmentReader["Pet_Id"]);
                    using var petReader = petInfo.ExecuteReader();
                    if (petReader.HasRows)
                    {
                        while (petReader.Read())
                        {
                            appointment.Pet.PetId = (Int64)petReader["Pet_Id"];
                            appointment.Pet.Name = (string)petReader["Name"];
                            appointment.Pet.Breed = (string)petReader["Breed"];
                            appointment.Pet.Age = (string)petReader["Age"];
                            appointment.Pet.Birthday = petReader["Birthday"] == DBNull.Value ? "" : (string)petReader["Birthday"];
                            appointment.Pet.Gender = (string)petReader["Gender"];
                            appointment.Pet.Health = petReader["Health"] == DBNull.Value ? "" : (string)petReader["Health"];
                            appointment.Pet.SpayedNeutered = (string)petReader["Spayed_Neutered"];
                            appointment.Pet.VetName = petReader["Vet_Name"] == DBNull.Value ? "" : (string)petReader["Vet_Name"];
                            appointment.Pet.VetPhone = petReader["Vet_Phone"] == DBNull.Value ? "" : (string)petReader["Vet_Phone"];
                            appointment.Pet.Vaccines = petReader["Vaccines"] == DBNull.Value ? "" : (string)petReader["Vaccines"];
                        }
                    }
                    
                    appointments.Add(appointment);
                    
                }
            }
        }
        
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
        }

        return appointments;
    }
}