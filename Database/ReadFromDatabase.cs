using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using spike.Models;

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
                        LastName = (string)employeeReader["Last_Name"]
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
}