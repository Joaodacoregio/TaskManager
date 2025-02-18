using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace TaskManager
{
    public abstract class Database
    {
        private string connectionString;

        public abstract void InitializeDatabase();
        public abstract void InsertTask(string name, string date);
        public abstract SQLiteDataReader GetAllTasks();



        public void setConnectionString(string connectionString_)
        {
            connectionString = connectionString_;
        }

        public string getConnectionString()
        {
            return connectionString;
        }

    }

    public class SQLite : Database
    {
        public SQLite()
        {
            InitializeDatabase(); // Inicializa o banco de dados
        }

        public override void InitializeDatabase()
        {
            //Setar a string de conexão
            setConnectionString("Data Source=tasks.db;Version=3;");

            if (!File.Exists("tasks.db"))
            {
                SQLiteConnection.CreateFile("tasks.db");
            }

            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Date TEXT NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Pendente'
                );";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public override void InsertTask(string name, string date)
        {
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
            {
                conn.Open();

                string insertQuery = "INSERT INTO Tasks (Name, Date, Status) VALUES (@name, @date, 'Pendente')";
                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override SQLiteDataReader GetAllTasks()
        {
            SQLiteConnection conn = new SQLiteConnection(getConnectionString());
            conn.Open();

            string selectQuery = "SELECT * FROM Tasks";
            SQLiteCommand cmd = new SQLiteCommand(selectQuery, conn);
            return cmd.ExecuteReader();
        }
    }


    //Fabrica de base de dados caso eu queira mudar no futuro
    public class DatabaseFactory
    {
        public Database CreateDatabase(string targetDatabase)
        {
            switch (targetDatabase.ToLower()) // Para garantir compatibilidade  
            {
                case "":
                    {
                        throw new ArgumentNullException("Banco de dados deve ser definido!");
                    }

                case "sqlite":
                    return new SQLite();

                default:
                    throw new ArgumentException("Banco de dados não suportado: " + targetDatabase);
            }
        }
    }




}
