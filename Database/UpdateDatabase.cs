using System;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using spike.Models;

namespace spike.Database;

public static class UpdateDatabase
{
    public static bool UpdateClient(Client client)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                var sqlStatement = @"";
                
                // add or update pets
                foreach (var pet in client.Pets)
                {
                    if (pet.PetId != null)
                    {
                        sqlStatement = @"
                            UPDATE Pets
                            SET Name = @Name, Breed = @Breed, Age = @Age, Birthday = @Birthday, Gender = @Gender, Health = @Health, 
                                Spayed_Neutered = @Spayed_Neutered, Vet_Name = @Vet_Name, Vet_Phone = @Vet_Phone, 
                                Vaccines = @Vaccines, IsActive = True    
                                WHERE Pet_Id = @PetId;
                        ";
                        using var petUpdate = new SqliteCommand(sqlStatement, connection, transaction);
                        petUpdate.Parameters.AddWithValue("@Name", pet.Name);
                        petUpdate.Parameters.AddWithValue("@Breed", pet.Breed);
                        petUpdate.Parameters.AddWithValue("@Age", pet.Age);
                        petUpdate.Parameters.AddWithValue("@Birthday", (object?)pet.Birthday ?? DBNull.Value);
                        petUpdate.Parameters.AddWithValue("@Gender", pet.Gender);
                        petUpdate.Parameters.AddWithValue("@Health", (object?)pet.Health ?? DBNull.Value);
                        petUpdate.Parameters.AddWithValue("@Spayed_Neutered", pet.SpayedNeutered);
                        petUpdate.Parameters.AddWithValue("@Vet_Name", (object?)pet.VetName ?? DBNull.Value);
                        petUpdate.Parameters.AddWithValue("@Vet_Phone", (object?)pet.VetPhone ?? DBNull.Value);
                        petUpdate.Parameters.AddWithValue("@Vaccines", (object?)pet.Vaccines ?? DBNull.Value);
                        petUpdate.Parameters.AddWithValue("@PetId", pet.PetId);
                        petUpdate.ExecuteNonQuery();
                    }
                    else
                    {
                        sqlStatement = @"
                            INSERT INTO Pets (Name, Breed, Age, Birthday, Gender, Health, 
                                              Spayed_Neutered, Vet_Name, Vet_Phone, Vaccines, Client_Id, IsActive)
                            VALUES (@Name, @Breed, @Age, @Birthday, @Gender, @Health, 
                                    @Spayed_Neutered, @Vet_Name, @Vet_Phone, @Vaccines, @ClientId, True);
                        ";
                        using var petInsert = new SqliteCommand(sqlStatement, connection, transaction);
                        petInsert.Parameters.AddWithValue("@Name", pet.Name);
                        petInsert.Parameters.AddWithValue("@Breed", pet.Breed);
                        petInsert.Parameters.AddWithValue("@Age", pet.Age);
                        petInsert.Parameters.AddWithValue("@Birthday", (object?)pet.Birthday ?? DBNull.Value);
                        petInsert.Parameters.AddWithValue("@Gender", pet.Gender);
                        petInsert.Parameters.AddWithValue("@Health", (object?)pet.Health ?? DBNull.Value);
                        petInsert.Parameters.AddWithValue("@Spayed_Neutered", pet.SpayedNeutered);
                        petInsert.Parameters.AddWithValue("@Vet_Name", (object?)pet.VetName ?? DBNull.Value);
                        petInsert.Parameters.AddWithValue("@Vet_Phone", (object?)pet.VetPhone ?? DBNull.Value);
                        petInsert.Parameters.AddWithValue("@Vaccines", (object?)pet.Vaccines ?? DBNull.Value);
                        petInsert.Parameters.AddWithValue("@ClientId", client.ClientId);
                        petInsert.ExecuteNonQuery();
                    }
                }
               
                //update rest of client info
                sqlStatement = @"
                    UPDATE ClientContact 
                    SET PrimaryPhone = @PrimaryPhone, SecondaryPhone = @SecondaryPhone, 
                        EmergencyPhone = @EmergencyPhone, EmergencyName = @EmergencyName, Email = @Email
                    WHERE Client_Id = @ClientId;
                ";
                using var updateContactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                updateContactInfo.Parameters.AddWithValue("@PrimaryPhone", client.ContactInfo.PrimaryPhone);
                updateContactInfo.Parameters.AddWithValue("@SecondaryPhone", (object?)client.ContactInfo.SecondaryPhone ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@EmergencyPhone", (object?)client.ContactInfo.EmergencyPhone ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@EmergencyName", (object?)client.ContactInfo.EmergencyName ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@Email", (object?)client.ContactInfo.Email ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                updateContactInfo.ExecuteNonQuery();
                
                sqlStatement = @"
                    UPDATE ClientAddress
                    SET Address = @Address, Etc = @Etc, City = @City, Country = @Country, 
                        Province = @Province,  Postal_Code = @Postal_Code
                    WHERE Client_Id = @ClientId;
                    ";
                using var updateAddress = new SqliteCommand(sqlStatement, connection, transaction);
                updateAddress.Parameters.AddWithValue("@Address", client.Address.AddressLine);
                updateAddress.Parameters.AddWithValue("@Etc", (object?)client.Address.Etc ?? DBNull.Value);
                updateAddress.Parameters.AddWithValue("@City", client.Address.City);
                updateAddress.Parameters.AddWithValue("@Country", client.Address.Country);
                updateAddress.Parameters.AddWithValue("@Province", client.Address.Province);
                updateAddress.Parameters.AddWithValue("@Postal_Code", client.Address.PostalCode);
                updateAddress.Parameters.AddWithValue("@ClientId", client.ClientId);
                updateAddress.ExecuteNonQuery();

                sqlStatement = @"
                    UPDATE Clients
                    SET First_Name = @First_Name, Last_Name = @Last_Name, IsActive = True
                    WHERE Client_Id = @ClientId;
                 ";
                using var updateClient = new SqliteCommand(sqlStatement, connection, transaction);
                updateClient.Parameters.AddWithValue("@First_Name", client.FirstName);
                updateClient.Parameters.AddWithValue("@Last_Name", client.LastName);
                updateClient.Parameters.AddWithValue("@ClientId", client.ClientId);
                updateClient.ExecuteNonQuery();
                
                transaction.Commit();

            }catch (SqliteException e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                return false;
            }
        }catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
                
        }
        
        return true;
    }
    
    public static bool DeleteClient(Client client)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var sqlStatement = @"";
                foreach (Pet pet in client.Pets)
                {
                    //set pet to inactive but don't delete can still be used by appointment table
                    sqlStatement = @"
                        UPDATE Pets
                        SET IsActive = False
                        WHERE Pet_Id = @PetId;
                    ";
                    using var pets = new SqliteCommand(sqlStatement, connection, transaction);
                    pets.Parameters.AddWithValue("@PetId", pet.PetId);
                    pets.ExecuteNonQuery();
                }
                
                sqlStatement = @"
                    DELETE FROM ClientContact
                    WHERE Client_Id = @ClientId;
                ";
                using var contactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                contactInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                contactInfo.ExecuteNonQuery();
                
                sqlStatement = @"
                    DELETE FROM ClientAddress
                    WHERE Client_Id = @ClientId;
                ";
                using var contactAddress = new SqliteCommand(sqlStatement, connection, transaction);
                contactAddress.Parameters.AddWithValue("@ClientId", client.ClientId);
                contactAddress.ExecuteNonQuery();
                
                
                sqlStatement = @"
                    UPDATE Clients
                    SET IsActive = False 
                    WHERE Client_Id = @ClientId;
                ";
            
                using var clientInfo = new SqliteCommand(sqlStatement, connection, transaction);
                clientInfo.Parameters.AddWithValue("@ClientId", client.ClientId);
                clientInfo.ExecuteNonQuery();
                
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

    public static bool DeletePet(Pet pet)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            var sqlStatement = @"
                SELECT COUNT(*)
                FROM Pets
                WHERE Pet_Id = @PetId;
            ";
            using var petRowExists = new SqliteCommand(sqlStatement, connection);
            petRowExists.Parameters.AddWithValue("@PetId", pet.PetId);
            using var petReader = petRowExists.ExecuteReader();

            if (petReader.HasRows)
            {
                sqlStatement = @"
                UPDATE Pets
                SET IsActive = False
                WHERE Pet_Id = @PetId;
                ";
                using var petInfo = new SqliteCommand(sqlStatement, connection);
                petInfo.Parameters.AddWithValue("@PetId", pet.PetId);
                petInfo.ExecuteNonQuery();
            }
            else
            {
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

    public static bool UpdateEmployee(Employee employee)
    {
         try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                var sqlStatement = @"";
                
                //update rest of client info
                sqlStatement = @"
                    UPDATE EmployeeContact 
                    SET PrimaryPhone = @PrimaryPhone, SecondaryPhone = @SecondaryPhone, 
                        EmergencyPhone = @EmergencyPhone, EmergencyName = @EmergencyName, Email = @Email
                    WHERE Employee_Id = @EmployeeId;
                ";
                using var updateContactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                updateContactInfo.Parameters.AddWithValue("@PrimaryPhone", employee.ContactInfo.PrimaryPhone);
                updateContactInfo.Parameters.AddWithValue("@SecondaryPhone", (object?)employee.ContactInfo.SecondaryPhone ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@EmergencyPhone", (object?)employee.ContactInfo.EmergencyPhone ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@EmergencyName", (object?)employee.ContactInfo.EmergencyName ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@Email", (object?)employee.ContactInfo.Email ?? DBNull.Value);
                updateContactInfo.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                updateContactInfo.ExecuteNonQuery();
                
                sqlStatement = @"
                    UPDATE EmployeeAddress
                    SET Address = @Address, Etc = @Etc, City = @City, Country = @Country, 
                        Province = @Province,  PostalCode = @Postal_Code
                    WHERE Employee_Id = @EmployeeId;
                    ";
                using var updateAddress = new SqliteCommand(sqlStatement, connection, transaction);
                updateAddress.Parameters.AddWithValue("@Address", employee.Address.AddressLine);
                updateAddress.Parameters.AddWithValue("@Etc", (object?)employee.Address.Etc ?? DBNull.Value);
                updateAddress.Parameters.AddWithValue("@City", employee.Address.City);
                updateAddress.Parameters.AddWithValue("@Country", employee.Address.Country);
                updateAddress.Parameters.AddWithValue("@Province", employee.Address.Province);
                updateAddress.Parameters.AddWithValue("@Postal_Code", employee.Address.PostalCode);
                updateAddress.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                updateAddress.ExecuteNonQuery();

                sqlStatement = @"
                    UPDATE Employees
                    SET First_Name = @First_Name, Last_Name = @Last_Name, IsActive = True, Cardinality = @Cardinality, Commission = @Commission, Base_Pay = @BasePay
                    WHERE Employee_Id = @EmployeeId;
                 ";
                using var updateEmployee = new SqliteCommand(sqlStatement, connection, transaction);
                updateEmployee.Parameters.AddWithValue("@First_Name", employee.FirstName);
                updateEmployee.Parameters.AddWithValue("@Last_Name", employee.LastName);
                updateEmployee.Parameters.AddWithValue("@Cardinality", (object?)employee.Cardinality ?? DBNull.Value);
                updateEmployee.Parameters.AddWithValue("@Commission", (object?)employee.Commission ?? DBNull.Value);
                updateEmployee.Parameters.AddWithValue("@BasePay", (object?)employee.BasePay ?? DBNull.Value);
                updateEmployee.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                updateEmployee.ExecuteNonQuery();
                
                transaction.Commit();

            }catch (SqliteException e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                return false;
            }
        }catch (SqliteException e)
        {
            Debug.WriteLine(e.Message);
            return false;
                
        }
        
        return true;
    }
    
    public static bool DeleteEmployee(Employee employee)
    {
        try
        {
            using var connection = DatabaseConnection.GetConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var sqlStatement = @"
                    DELETE FROM EmployeeContact
                    WHERE Employee_Id = @EmployeeId;
                ";
                using var contactInfo = new SqliteCommand(sqlStatement, connection, transaction);
                contactInfo.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                contactInfo.ExecuteNonQuery();
                
                sqlStatement = @"
                    DELETE FROM EmployeeAddress
                    WHERE Employee_Id = @EmployeeId;
                ";
                using var contactAddress = new SqliteCommand(sqlStatement, connection, transaction);
                contactAddress.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                contactAddress.ExecuteNonQuery();
                
                
                sqlStatement = @"
                    UPDATE Employees
                    SET IsActive = False 
                    WHERE Employee_Id = @EmployeeId;
                ";
            
                using var employeeInfo = new SqliteCommand(sqlStatement, connection, transaction);
                employeeInfo.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                employeeInfo.ExecuteNonQuery();
                
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