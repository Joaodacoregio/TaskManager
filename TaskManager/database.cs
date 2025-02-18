using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TaskManager
{
    public abstract class Database
    {
        private string connectionString;

        public abstract void InitializeDatabase();
        public abstract void InsertTask(string name, string date, string description);
        public abstract void setStatus(int Id);

        public abstract string getTaskDescription(int ID);
        public abstract void DeleteTask(int ID);
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
                Status TEXT NOT NULL DEFAULT 'Pendente',
                Description TEXT  
                );";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public override void InsertTask(string name, string date, string description)
        {
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
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
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
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


        public override string getTaskDescription(int ID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT Description FROM Tasks WHERE Id = @taskId";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@taskId", DbType.Int32).Value = ID;
                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            return "Descrição não encontrada."; // Evita exceção desnecessária
                        }

                        return result.ToString();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // Log do erro pode ser útil
                Console.WriteLine("Erro no banco de dados: " + ex.Message);
                return "Erro ao acessar o banco de dados.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro inesperado: " + ex.Message);
                return "Ocorreu um erro inesperado.";
            }
        }


        public override void setStatus(int ID)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
                {
                    conn.Open();

                    // Query de atualização
                    string query = "UPDATE Tasks SET Status = 'Concluido' WHERE Id = @taskId";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.Add("@taskId", DbType.Int32).Value = ID;

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
