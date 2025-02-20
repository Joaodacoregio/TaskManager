using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TaskManager
{

    public class Task
    {
        //Em c# a principio é uma boa pratica deixar os metodos publicos e usar get e set apenas se precisar de validação

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime LimitDate { get; set; }
        public string Description { get; set; }
    }



    public abstract class Database
    {
        public string connectionString { get; set; }

        public abstract void InitializeDatabase();
        public abstract void InsertTask(string name, DateTime date, string description);
        public abstract void setStatus(int id , string status);

        public abstract Task getTaskUsingId(int id);
        public abstract List<Task> getAllTasksInObjectFormat();

        public abstract void DeleteTask(int ID);
        public abstract SQLiteDataReader GetAllTasks();



 

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
            connectionString = "Data Source=tasks.db;Version=3;";
 

            if (!File.Exists("tasks.db"))
            {
                SQLiteConnection.CreateFile("tasks.db");
            }

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string drop_sql = @"DROP TABLE IF EXISTS Tasks;";

                string sql = @"CREATE TABLE IF NOT EXISTS Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Date DATETIME NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Pendente',
                Description TEXT  
                );";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public override void InsertTask(string name, DateTime date, string description)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string insertQuery = "INSERT INTO Tasks (Name, Date, Status, Description) VALUES (@name, @date, 'Pendente',@description)";
                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override void DeleteTask(int ID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM Tasks WHERE Id = @taskId";
                    using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", ID);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir a tarefa: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

 

        public override void setStatus(int ID, string status)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Query de atualização
                    string query = "UPDATE Tasks SET Status = @status WHERE Id = @taskId";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@taskId", DbType.Int32).Value = ID;
                        cmd.Parameters.AddWithValue("@status", status);

                        // Executa a atualização
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Erro no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro inesperado: " + ex.Message);
            }
        }


        public override SQLiteDataReader GetAllTasks()
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();

            string selectQuery = "SELECT * FROM Tasks";
            SQLiteCommand cmd = new SQLiteCommand(selectQuery, conn);
            return cmd.ExecuteReader();
        }

        public override List<Task> getAllTasksInObjectFormat()
        {
            List<Task> tasks = new List<Task>();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Id, Name, Status, Date,Description FROM Tasks";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new Task
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Status = reader.GetString(2),
                                    LimitDate = reader.GetDateTime(3),
                                    Description = reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Erro no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro inesperado: " + ex.Message);
            }

            return tasks;
        }


        public override Task getTaskUsingId(int id)
        {
            Task task = null;

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Id, Name, Status, Date,Description FROM Tasks WHERE Id = @taskId";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", id);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                task = new Task
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Status = reader.GetString(2),
                                    LimitDate = reader.GetDateTime(3),
                                    Description = reader.GetString(4)
                                }; 
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Erro no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro inesperado: " + ex.Message);
            }

            return task; // Retorna null se a tarefa não for encontrada
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
