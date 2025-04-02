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
                                WHERE Pet_Id = @PetsId;
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
                using var insertAddress = new SqliteCommand(sqlStatement, connection, transaction);
                insertAddress.Parameters.AddWithValue("@Address", client.Address.AddressLine);
                insertAddress.Parameters.AddWithValue("@Etc", (object?)client.Address.Etc ?? DBNull.Value);
                insertAddress.Parameters.AddWithValue("@City", client.Address.City);
                insertAddress.Parameters.AddWithValue("@Country", client.Address.Country);
                insertAddress.Parameters.AddWithValue("@Province", client.Address.Province);
                insertAddress.Parameters.AddWithValue("@Postal_Code", client.Address.PostalCode);
                insertAddress.Parameters.AddWithValue("@ClientId", client.ClientId);
                insertAddress.ExecuteNonQuery();

                sqlStatement = @"
                    UPDATE Clients
                    SET First_Name = @First_Name, Last_Name = @Last_Name, IsActive = True
                    WHERE Client_Id = @ClientId;
                 ";
                using var insertClient = new SqliteCommand(sqlStatement, connection, transaction);
                insertClient.Parameters.AddWithValue("@First_Name", client.FirstName);
                insertClient.Parameters.AddWithValue("@Last_Name", client.LastName);
                insertClient.Parameters.AddWithValue("@ClientId", client.ClientId);
                insertClient.ExecuteNonQuery();
                
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
}