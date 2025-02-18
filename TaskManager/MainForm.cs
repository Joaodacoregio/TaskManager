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
            initializeDatabase("SQLite"); // da para implementar outros DB usando a factory
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

            //Clicks
            btnAddTask.Click += btnAddTaskClick;  
            btnRemoveTask.Click += btnRemoveTaskClick;

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
                OwnerDraw = true, //Isso é usado para fazer desenhos personalizados
                Size = new Size(750, 600), // Defini o tamanho estático
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 12),
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Clickable
            };

            taskList.Columns.Add("Id", 0); // 0 para não exibir
            taskList.Columns.Add("Nome", 300);
            taskList.Columns.Add("Data", 200);
            taskList.Columns.Add("Status", 150);

            taskList.DrawSubItem += taskList_DrawSubItem; // Vinculando o evento
            taskList.DrawColumnHeader += taskList_DrawColumnHeader; // Para desenhar o cabeçalho

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
            using (AddTaskForm addTaskForm = new AddTaskForm())
            {
                if (addTaskForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        database.InsertTask(addTaskForm.TaskName, addTaskForm.TaskDate);
                        UpdateTaskList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao adicionar tarefa " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRemoveTaskClick(object sender, EventArgs e)
        {
            if (taskList.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Tem certeza que deseja excluir esta tarefa?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Pega o ID da tarefa selecionada (primeira coluna invisível)
                        int taskId = Convert.ToInt32(taskList.SelectedItems[0].SubItems[0].Text);
                        // Chama o método de exclusão passando o ID
                        database.DeleteTask(taskId);

                        // Atualiza a lista de tarefas
                        UpdateTaskList();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao excluir tarefa " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tarefa para excluir.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void taskList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Desenha o cabeçalho normalmente
            e.DrawDefault = true;
        }

        private void taskList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

            if (e.ColumnIndex == 3)
            {
                var status = e.SubItem.Text;

                if (status == "Pendente")
                {
                    // Cor de fundo amarela para a célula
                    e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);

                    // Texto em negrito e preto
                    using (var boldFont = new Font("Segoe UI", 12, FontStyle.Bold))
                    {
                        e.Graphics.DrawString(status, boldFont, Brushes.Black, e.Bounds);
                    }
                }
                else
                {
                    e.DrawDefault = true;
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void UpdateTaskList()
        {
            // Limpa para não ter problemas de exibição aqui
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
                        string taskId = reader["Id"].ToString(); // Pegue o ID da tarefa
                        string nome = reader["Name"].ToString();
                        string data = reader["Date"].ToString();
                        string status = reader["Status"].ToString();

                        // Verificação básica: se algum dos campos estiver vazio, ignora a linha
                        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(data) || string.IsNullOrEmpty(status))
                            continue;

                        // Adiciona o item ao ListView
                        ListViewItem item = new ListViewItem(taskId); // Primeiro item é o ID (invisível)
                        item.SubItems.Add(nome);
                        item.SubItems.Add(data);
                        item.SubItems.Add(status);

                        item.Font = new Font("Segoe UI", 12);

                        taskList.Items.Add(item);
                    }
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
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "MainForm";
            this.ResumeLayout(false);
        }
    }
}
