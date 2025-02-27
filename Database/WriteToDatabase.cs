using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace spike.Database;

public class WriteToDatabase
{
    public static void SaveAppointment()
    {
        using (var connection = DatabaseConnection.GetConnection())
        {
            connection.Open();
            var command = connection.CreateCommand();
            
        }
    }
}