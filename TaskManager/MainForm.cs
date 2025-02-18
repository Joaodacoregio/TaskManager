using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;


 
namespace TaskManager
{
    public class MainForm : Form
    {
        private Panel sideMenu;
        private Panel mainPanel;
        private ListView taskList;
        private Database database;


        public MainForm()
        {
            InitializeComponent();
            CustomizeUI();
            initializeDatabase("SQLite"); // Vou usar SQLite para esse projeto por ser simples
            UpdateTaskList();  
        }

        private void CustomizeUI()
        {
            this.Text = "Task Manager";
            this.Size = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;

            AddSideMenu();
            AddMainPanel();
            AddTaskList();
        }

        private void AddSideMenu()
        {
            sideMenu = new Panel()
            {
                Size = new Size(200, this.Height),
                BackColor = Color.FromArgb(33, 37, 41), // Bootstrap dark
                Dock = DockStyle.Left
            };
            this.Controls.Add(sideMenu);

            AddButtons();
        }

        private void AddMainPanel()
        {
            mainPanel = new Panel()
            {
                Size = new Size(780, this.Height),
                Location = new Point(200, 0),
                BackColor = Color.White
            };
            this.Controls.Add(mainPanel);
        }

        private void AddButtons()
        {
            Button btnAddTask = CreateButton("+ Nova Tarefa", new Point(10, 20));
            Button btnRemoveTask = CreateButton("- Remover Tarefa", new Point(10, 70));
            Button btnUpdateTask = CreateButton("⟳ Atualizar Tarefa", new Point(10, 120));

            btnAddTask.Click += btnAddTaskClick; // Conectando o evento de clique

            sideMenu.Controls.Add(btnAddTask);
            sideMenu.Controls.Add(btnRemoveTask);
            sideMenu.Controls.Add(btnUpdateTask);
        }

        private Button CreateButton(string text, Point location)
        {
            Button button = new Button()
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(180, 40),
                Location = location,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(13, 110, 253), // Bootstrap primary
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void AddTaskList()
        {
            taskList = new ListView()
            {
                View = View.Details,
                Size = new Size(750, 600), // Defini o tamanho statico
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10),
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Clickable
            };
            taskList.Columns.Add("Nome", 300);
            taskList.Columns.Add("Data", 200);
            taskList.Columns.Add("Status", 150);
            mainPanel.Controls.Add(taskList);
        }

        private void initializeDatabase(string target_database)
        {
            //Factory 
            DatabaseFactory factory = new DatabaseFactory();

            try
            {
                database = factory.CreateDatabase(target_database);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o banco de dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

 
        private void btnAddTaskClick(object sender, EventArgs e)
        {
            try
            {
                //Insere e atualiza
                database.InsertTask("Test", "01/01/3000", "Pendente");
                UpdateTaskList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adicionar a tarefa: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTaskList()
        {
            //Limpa para não ter problemas de exibição aqui
            taskList.Items.Clear();  

            try
            {
                SQLiteDataReader reader = database.GetAllTasks();

                // Verifica se o reader possui dados
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Certifica-se de que as colunas existem
                        string nome = reader["Name"].ToString();
                        string data = reader["Date"].ToString();
                        string status = reader["Status"].ToString();

                        // Verificação básica: se algum dos campos estiver vazio, ignora a linha
                        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(data) || string.IsNullOrEmpty(status))
                            continue;

                        // Adiciona o item ao ListView
                        ListViewItem item = new ListViewItem(nome);
                        item.SubItems.Add(data);
                        item.SubItems.Add(status);

                        taskList.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhuma tarefa encontrada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar as tarefas: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(401, 314);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}
