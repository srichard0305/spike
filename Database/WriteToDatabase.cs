using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Data.Sqlite;
using spike.Models;

namespace spike.Database;

public static class WriteToDatabase
{
    public static bool AddAppointment(Appointment appointment)
    {
         try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
           
            // first insert appon into table
            var sqlStatement = @"
                   INSERT INTO Appointments (Client_Id, Pet_Id, Employee_Id, Pet_Notes, Service, Service_Notes, Cost, StartTime, EndTime,
                                              CheckIn, CheckOut, Cancelled, NoShow, BookedBy) 
                   VALUES (@ClientId, @PetId, @EmployeeId, @Pet_Notes, @Service, @Service_Notes, @Cost, @StartTime, @EndTime,
                           @CheckIn, @CheckOut, @Cancelled, @NoShow, @BookedBy)
              ";
            using var insertAppointment = new SqliteCommand(sqlStatement, connection);
            insertAppointment.Parameters.AddWithValue("@ClientId", appointment.Client.ClientId);
            insertAppointment.Parameters.AddWithValue("@PetId", appointment.Pet.PetId); 
            insertAppointment.Parameters.AddWithValue("@EmployeeId", appointment.EmployeeStylists.EmployeeId);
            insertAppointment.Parameters.AddWithValue("@Pet_Notes", (object?)appointment.PetNotes ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@Service", appointment.Service);
            insertAppointment.Parameters.AddWithValue("@Service_Notes", (object?)appointment.ServiceNotes ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@Cost", double.TryParse(appointment.Cost, NumberStyles.Any, CultureInfo.InvariantCulture, out var cost));
            insertAppointment.Parameters.AddWithValue("@StartTime", appointment.StartTime);
            insertAppointment.Parameters.AddWithValue("@EndTime", appointment.EndTime);
            insertAppointment.Parameters.AddWithValue("@CheckIn", (object?)appointment.CheckIn ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@CheckOut", (object?)appointment.CheckOut ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@Cancelled", (object?)appointment.Cancelled ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@NoShow", (object?)appointment.NoShow ?? DBNull.Value);
            insertAppointment.Parameters.AddWithValue("@BookedBy", appointment.EmployeeBookedBy.EmployeeId);
            insertAppointment.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
        return true;
    }

    public static bool AddClient(Client client)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
        
            using var transaction = connection.BeginTransaction();

            try
            {
                // first insert client into table
                var sqlStatement = @"
                    INSERT INTO Clients (First_Name, Last_Name, IsActive) 
                    VALUES (@First_Name, @Last_Name, True)
                 ";
                using var insertClient = new SqliteCommand(sqlStatement, connection, transaction);
                insertClient.Parameters.AddWithValue("@First_Name", client.FirstName);
                insertClient.Parameters.AddWithValue("@Last_Name", client.LastName);
                insertClient.ExecuteNonQuery();

                //grab last inserted row for the auto incremented id
                sqlStatement = @"SELECT last_insert_rowid()";
                using var lastRow = new SqliteCommand(sqlStatement, connection, transaction);
                var clientId = lastRow.ExecuteScalar();

                //insert address
                sqlStatement = @"
                    INSERT INTO ClientAddress (Address, Etc, City, Country, Province, Postal_Code, Client_Id)
                    VALUES (@Address, @Etc, @City, @Country, @Province, @Postal_Code, @Client_Id)
                    ";
                using var insertAddress = new SqliteCommand(sqlStatement, connection, transaction);
                insertAddress.Parameters.AddWithValue("@Address", client.Address.AddressLine);
                insertAddress.Parameters.AddWithValue("@Etc", (object?)client.Address.Etc ?? DBNull.Value);
                insertAddress.Parameters.AddWithValue("@City", client.Address.City);
                insertAddress.Parameters.AddWithValue("@Country", client.Address.Country);
                insertAddress.Parameters.AddWithValue("@Province", client.Address.Province);
                insertAddress.Parameters.AddWithValue("@Postal_Code", client.Address.PostalCode);
                insertAddress.Parameters.AddWithValue("@Client_Id", clientId);
                insertAddress.ExecuteNonQuery();
                
                //insert contact info
                sqlStatement = @"
                    INSERT INTO ClientContact (PrimaryPhone, SecondaryPhone, EmergencyPhone, EmergencyName, Email, Client_Id)
                    VALUES (@PrimaryPhone, @SecondaryPhone, @EmergencyPhone, @EmergencyName, @Email, @Client_Id)
                    ";
                using var insertContactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                insertContactInfo.Parameters.AddWithValue("@PrimaryPhone", client.ContactInfo.PrimaryPhone);
                insertContactInfo.Parameters.AddWithValue("@SecondaryPhone", (object?)client.ContactInfo.SecondaryPhone ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@EmergencyPhone", (object?)client.ContactInfo.EmergencyPhone ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@EmergencyName", (object?)client.ContactInfo.EmergencyName ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@Email", (object?)client.ContactInfo.Email ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@Client_Id", clientId);
                insertContactInfo.ExecuteNonQuery();
                
                // insert each pet needs to be done with this transaction 
                foreach (var pet in client.Pets)
                {
                    sqlStatement = @"
                    INSERT INTO Pets (Name, Breed, Age, Birthday, Gender, Health, Spayed_Neutered, Vet_Name, Vet_Phone, Vaccines, Client_Id, IsActive) 
                    VALUES (@Name, @Breed, @Age, @Birthday, @Gender, @Health, @Spayed_Neutered, @Vet_Name, @Vet_Phone, @Vaccines, @Client_Id, True)
                    ";
                    using var insertPet = new SqliteCommand(sqlStatement, connection, transaction);
                    insertPet.Parameters.AddWithValue("@Name", pet.Name);
                    insertPet.Parameters.AddWithValue("@Breed", pet.Breed);
                    insertPet.Parameters.AddWithValue("@Age", pet.Age);
                    insertPet.Parameters.AddWithValue("@Birthday", (object?)pet.Birthday ?? DBNull.Value);
                    insertPet.Parameters.AddWithValue("@Gender", pet.Gender);
                    insertPet.Parameters.AddWithValue("@Health", (object?)pet.Health ?? DBNull.Value);
                    insertPet.Parameters.AddWithValue("@Spayed_Neutered", pet.SpayedNeutered);
                    insertPet.Parameters.AddWithValue("@Vet_Name", (object?)pet.VetName ?? DBNull.Value);
                    insertPet.Parameters.AddWithValue("@Vet_Phone", (object?)pet.VetPhone ?? DBNull.Value);
                    insertPet.Parameters.AddWithValue("@Vaccines", (object?)pet.Vaccines ?? DBNull.Value);
                    insertPet.Parameters.AddWithValue("@Client_Id", clientId);
                    insertPet.ExecuteNonQuery();
                }

                transaction.Commit();
                
            }
            catch (SqliteException e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
        return true;
    }

    public static bool AddPet(Pet pet,  object clientId)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
        
            using var transaction = connection.BeginTransaction();

            try
            {
                
                var sqlStatement = @"
                    INSERT INTO Pets (Name, Breed, Age, Birthday, Gender, Health, Spayed_Neutered, Vet_Name, Vet_Phone, Vaccines, Client_Id, IsActive) 
                    VALUES (@Name, @Breed, @Age, @Birthday, @Gender, @Health, @Spayed_Neutered, @Vet_Name, @Vet_Phone, @Vaccines, @Client_Id, True)
                    ";
                using var insertPet = new SqliteCommand(sqlStatement, connection, transaction);
                insertPet.Parameters.AddWithValue("@Name", pet.Name);
                insertPet.Parameters.AddWithValue("@Breed", pet.Breed);
                insertPet.Parameters.AddWithValue("@Age", pet.Age);
                insertPet.Parameters.AddWithValue("@Birthday", (object?)pet.Birthday ?? DBNull.Value);
                insertPet.Parameters.AddWithValue("@Gender", pet.Gender);
                insertPet.Parameters.AddWithValue("@Health", (object?)pet.Health ?? DBNull.Value);
                insertPet.Parameters.AddWithValue("@Spayed_Neutered", pet.SpayedNeutered);
                insertPet.Parameters.AddWithValue("@Vet_Name", (object?)pet.VetName ?? DBNull.Value);
                insertPet.Parameters.AddWithValue("@Vet_Phone", (object?)pet.VetPhone ?? DBNull.Value);
                insertPet.Parameters.AddWithValue("@Vaccines", (object?)pet.Vaccines ?? DBNull.Value);
                insertPet.Parameters.AddWithValue("@Client_Id", clientId);
                insertPet.ExecuteNonQuery();
                
                transaction.Commit();
                
            }
            catch (SqliteException e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
        
        return true;
    }

    public static bool AddEmployee(Employee employee)
    {
        
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
        
            using var transaction = connection.BeginTransaction();

            try
            {
                // first insert employee into table
                var sqlStatement = @"
                    INSERT INTO Employees (First_Name, Last_Name, IsActive, Cardinality, Commission, Base_Pay) 
                    VALUES (@First_Name, @Last_Name, True, @Cardinality, @Commission, @BasePay)
                 ";
                using var insertEmployee = new SqliteCommand(sqlStatement, connection, transaction);
                insertEmployee.Parameters.AddWithValue("@First_Name", employee.FirstName);
                insertEmployee.Parameters.AddWithValue("@Last_Name", employee.LastName);
                insertEmployee.Parameters.AddWithValue("@Cardinality", (object?)employee.Cardinality ?? DBNull.Value);
                insertEmployee.Parameters.AddWithValue("@Commission", (object?)employee.Commission ?? DBNull.Value);
                insertEmployee.Parameters.AddWithValue("@BasePay", (object?)employee.BasePay ?? DBNull.Value);
                insertEmployee.ExecuteNonQuery();

                //grab last inserted row for the auto incremented id
                sqlStatement = @"SELECT last_insert_rowid()";
                using var lastRow = new SqliteCommand(sqlStatement, connection, transaction);
                var employeeId = lastRow.ExecuteScalar();

                //insert address
                sqlStatement = @"
                    INSERT INTO EmployeeAddress (Address, Etc, City, Country, Province, PostalCode, Employee_Id)
                    VALUES (@Address, @Etc, @City, @Country, @Province, @Postal_Code, @Employee_Id)
                    ";
                using var insertAddress = new SqliteCommand(sqlStatement, connection, transaction);
                insertAddress.Parameters.AddWithValue("@Address", employee.Address.AddressLine);
                insertAddress.Parameters.AddWithValue("@Etc", (object?)employee.Address.Etc ?? DBNull.Value);
                insertAddress.Parameters.AddWithValue("@City", employee.Address.City);
                insertAddress.Parameters.AddWithValue("@Country", employee.Address.Country);
                insertAddress.Parameters.AddWithValue("@Province", employee.Address.Province);
                insertAddress.Parameters.AddWithValue("@Postal_Code", employee.Address.PostalCode);
                insertAddress.Parameters.AddWithValue("@Employee_Id", employeeId);
                insertAddress.ExecuteNonQuery();
                
                //insert contact info
                sqlStatement = @"
                    INSERT INTO EmployeeContact (PrimaryPhone, SecondaryPhone, EmergencyPhone, EmergencyName, Email, Employee_Id)
                    VALUES (@PrimaryPhone, @SecondaryPhone, @EmergencyPhone, @EmergencyName, @Email, @Employee_Id)
                    ";
                using var insertContactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                insertContactInfo.Parameters.AddWithValue("@PrimaryPhone", employee.ContactInfo.PrimaryPhone);
                insertContactInfo.Parameters.AddWithValue("@SecondaryPhone", (object?)employee.ContactInfo.SecondaryPhone ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@EmergencyPhone", (object?)employee.ContactInfo.EmergencyPhone ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@EmergencyName", (object?)employee.ContactInfo.EmergencyName ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@Email", (object?)employee.ContactInfo.Email ?? DBNull.Value);
                insertContactInfo.Parameters.AddWithValue("@Employee_Id", employeeId);
                insertContactInfo.ExecuteNonQuery();
                
                transaction.Commit();
                
            }
            catch (SqliteException e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
        return true;
    }
}