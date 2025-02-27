using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace spike.Database;

public class DatabaseConnection
{
    private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spikedb.db");
    private static readonly string ConnectionString = $"Data Source={DbPath}";

    public static void InitializeDatabase()
    {
        if (!File.Exists(DbPath))
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Clients (
                    Client_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    First_Name TEXT NOT NULL,
                    Last_Name TEXT NOT NULL,
                    Address TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Employees (
                    Employee_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    First_Name TEXT NOT NULL,
                    Last_Name TEXT NOT NULL,
                    Cardinality INTEGER
                );
                   
                CREATE TABLE IF NOT EXISTS Pets (
                    Pet_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Breed TEXT NOT NULL,
                    Age INTEGER NOT NULL,
                    Birthday TEXT NOT NULL,
                    Sex TEXT NOT NULL,
                    Health TEXT NOT NULL,
                    Spayed_Neutred TEXT NOT NULL,
                    Vet_Name TEXT,
                    Vet_Phone TEXT,
                    Vaccines TEXT,
                    Client_Id INTEGER,
                    FOREIGN KEY (Client_Id) REFERENCES Clients (Client_Id)
                );
                
                CREATE TABLE IF NOT EXISTS Appointments (
                    Appointment_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Client_Id INTEGER NOT NULL,
                    Pet_Id INTEGER NOT NULL,
                    Employee_Id INTEGER NOT NULL,
                    Pet_Notes TEXT,
                    Service TEXT NOT NULL,
                    Service_Notes TEXT,
                    Cost REAL NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    CheckIn TEXT NOT NULL,
                    CheckOut TEXT NOT NULL,
                    Cancelled TEXT,
                    NoShow TEXT,
                    BookedBy INTEGER NOT NULL,
                    FOREIGN KEY(Client_Id) REFERENCES Clients(Client_Id),
                    FOREIGN KEY(Pet_Id) REFERENCES Pets(Pet_Id),
                    FOREIGN KEY(Employee_Id) REFERENCES Employees(Employee_Id)
                    FOREIGN KEY(BookedBy) REFERENCES Employees(Employee_Id)
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    public static SqliteConnection GetConnection() => new SqliteConnection(ConnectionString);
    
}