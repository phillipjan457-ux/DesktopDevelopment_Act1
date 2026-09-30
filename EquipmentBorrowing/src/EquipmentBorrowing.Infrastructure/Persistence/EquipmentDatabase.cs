using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class EquipmentDatabase
{
    public static string GetConnectionString()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "EquipmentBorrowing");

        Directory.CreateDirectory(folder);

        return new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(folder, "equipment-borrowing.db"),
            ForeignKeys = true
        }.ToString();
    }
}