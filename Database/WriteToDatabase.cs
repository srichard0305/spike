using System.Globalization;
using spike.Models;

namespace spike.Database;

public class WriteToDatabase
{
    public static void AddAppointment(Appointment appointment)
    {
        
    }

    public static void AddClient(Client client)
    {
        using var connection = DatabaseConnection.GetConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Clients (First_Name, Last_Name, Address) 
            VALUES (@First_Name, @Last_Name, @Address)
               
            INSERT INTO ClientEmail (Email, Client_Id)
            Values (@Email, @Client_Id)

            INSERT INTO ClientPhone (Phone, Client_Id)
            Values (@Phone, @Client_Id)    
    
         ";
        command.Parameters.AddWithValue("@First_Name", client.First_Name);
        command.Parameters.AddWithValue("@Last_Name", client.Last_Name);
        command.Parameters.AddWithValue("@Address", client.Address);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Phone", client.Phone);
        command.ExecuteNonQuery();
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