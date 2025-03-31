using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace spike.Database;

public static class DatabaseConnection
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
                CREATE TABLE Clients (
                    Client_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    First_Name TEXT NOT NULL,
                    Last_Name TEXT NOT NULL
                );

                CREATE TABLE ClientAddress (
                    ClientAddress_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Address TEXT NOT NULL,
                    Etc TEXT NULL,
                    City TEXT NOT NULL,
                    Country TEXT NOT NULL,
                    Province TEXT NOT NULL,
                    Postal_Code TEXT NOT NULL,
                    Client_Id INTEGER NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Clients (Client_Id) 
                );

                CREATE TABLE ClientContact (
                    ClientEmail_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PrimaryPhone TEXT NOT NULL,
                    SecondaryPhone TEXT NULL,
                    EmergencyPhone TEXT NULL,
                    EmergencyName TEXT NULL,
                    Email TEXT NULL,
                    Client_Id INTEGER NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Clients (Client_Id) 
                );
               
              
                CREATE TABLE Pets (
                    Pet_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Breed TEXT NOT NULL,
                    Age TEXT NOT NULL,
                    Birthday TEXT NULL,
                    Gender TEXT NOT NULL,
                    Health TEXT NULL,
                    Spayed_Neutered TEXT NOT NULL,
                    Vet_Name TEXT NULL,
                    Vet_Phone TEXT NULL,
                    Vaccines TEXT NULL,
                    Client_Id INTEGER NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Clients (Client_Id)
                );

                CREATE TABLE Employees (
                    Employee_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    First_Name TEXT NOT NULL,
                    Last_Name TEXT NOT NULL,
                    Cardinality INTEGER
                );

                CREATE TABLE EmployeeAddress (
                    EmployeeAddress_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Address TEXT NOT NULL,
                    Etc TEXT NULL,
                    City TEXT NOT NULL,
                    Country TEXT NOT NULL,
                    Province TEXT NOT NULL,
                    PostalCode TEXT NOT NULL,
                    Employee_Id INTEGER NOT NULL,
                    FOREIGN KEY (Employee_Id) REFERENCES Employees (Employee_Id) 
                );

                CREATE TABLE EmployeeContact (
                    EmployeePhone_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PrimaryPhone TEXT NOT NULL,
                    SecondaryPhone TEXT NULL,
                    EmergencyPhone TEXT NULL,
                    EmergencyName TEXT NULL,
                    Email TEXT NOT NULL,
                    Employee_Id INTEGER NOT NULL,
                    FOREIGN KEY (Employee_Id) REFERENCES Employees (Employee_Id) 
                );

                CREATE TABLE Appointments (
                    Appointment_Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Client_Id INTEGER NOT NULL,
                    Pet_Id INTEGER NOT NULL,
                    Employee_Id INTEGER NOT NULL,
                    Pet_Notes TEXT NULL,
                    Service TEXT NOT NULL,
                    Service_Notes TEXT,
                    Cost REAL NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    CheckIn TEXT NULL,
                    CheckOut TEXT NULL,
                    Cancelled TEXT NULL,
                    NoShow TEXT NULL,
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