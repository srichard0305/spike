using System;
using System.Globalization;
using Microsoft.Data.Sqlite;
using spike.Models;

namespace spike.Database;

public class WriteToDatabase
{
    public static void AddAppointment(Appointment appointment)
    {
        
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
                    INSERT INTO Clients (First_Name, Last_Name) 
                    VALUES (@First_Name, @Last_Name)
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
                insertAddress.Parameters.AddWithValue("@Etc", client.Address.Etc);
                insertAddress.Parameters.AddWithValue("@City", client.Address.City);
                insertAddress.Parameters.AddWithValue("@Country", client.Address.Country);
                insertAddress.Parameters.AddWithValue("@Province", client.Address.Province);
                insertAddress.Parameters.AddWithValue("@Postal_Code", client.Address.PostalCode);
                insertAddress.Parameters.AddWithValue("@Client_Id", clientId);
                insertAddress.ExecuteNonQuery();

                //insert email
                sqlStatement = @"
                    INSERT INTO ClientEmail (Email, Client_Id)
                    VALUES (@Email, @Client_Id)
                    ";
                using var insertEmail = new SqliteCommand(sqlStatement, connection, transaction);
                insertEmail.Parameters.AddWithValue("@Email", client.Email);
                insertEmail.Parameters.AddWithValue("@Client_Id", clientId);
                insertEmail.ExecuteNonQuery();

                //insert phone number 
                sqlStatement = @"
                    INSERT INTO ClientPhone (Phone, Client_Id)
                    VALUES (@Phone, @Client_Id)
                    ";
                using var insertPhone = new SqliteCommand(sqlStatement, connection, transaction);
                insertPhone.Parameters.AddWithValue("@Phone", client.Phone);
                insertPhone.Parameters.AddWithValue("@Client_Id", clientId);
                insertPhone.ExecuteNonQuery();


                transaction.Commit();

                return true;

            }
            catch (SqliteException e)
            {
                transaction.Rollback();
                Console.WriteLine(e.Message);
            }
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
        }
        
        return false;
    }

    public static void AddPet(Pet pet)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Clients (Name, Breed, Age, Birthday, Gender, Health, Spayed_Neutered, Vet_Name, Vet_Phone, Vaccines) 
            VALUES (@Name, @Breed, @Age, @Birthday, @Gender, @Health, @Spayed_Neutred, @Vet_Name, @Vet_Phone, @Vaccines)
         ";
        command.Parameters.AddWithValue("@Name", pet.Name);
        command.Parameters.AddWithValue("@Breed", pet.Breed);
        command.Parameters.AddWithValue("@Age", pet.Age);
        command.Parameters.AddWithValue("@Birthday", pet.Birthday.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("@Gender", pet.Gender);
        command.Parameters.AddWithValue("@Health", pet.Health);
        command.Parameters.AddWithValue("@Spayed_Neutered", pet.Spayed_Neutered);
        command.Parameters.AddWithValue("@Vet_Name", pet.Vet_Name);
        command.Parameters.AddWithValue("@Vet_Phone", pet.Vet_Phone);
        command.Parameters.AddWithValue("@Vaccines", pet.Vaccines);
        command.ExecuteNonQuery();
    }

    public static void AddEmployee(Employee employee)
    {
        
    }
}