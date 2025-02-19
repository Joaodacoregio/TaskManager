﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager
{
    public class MainForm : Form
    {
        private Panel sideMenu;
        private Panel mainPanel;
        private ListView taskList;
        private ContextMenuStrip contextMenu;
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
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  

            AddSideMenu();
            AddMainPanel();
            AddTaskList();
            InitializeContextMenu();
        }

        //Menu quando apertar o right click do mousi
        private void InitializeContextMenu()
        {
            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem readDescriptionMenuItem = new ToolStripMenuItem("👁️ Ler Descrição");
            ToolStripMenuItem checkMenuItem = new ToolStripMenuItem("✔️ Concluir");

            //Fontes
            checkMenuItem.Font = new Font("Segoe UI", 12);
            readDescriptionMenuItem.Font = new Font("Segoe UI", 12);

            //Clicks
            readDescriptionMenuItem.Click += ReadDescriptionMenuItem_Click;
            checkMenuItem.Click += checkMenuItem_Click;

            //Adiciona
            contextMenu.Items.Add(checkMenuItem);
            contextMenu.Items.Add(readDescriptionMenuItem);

            //Acopla ao task item
            taskList.ContextMenuStrip = contextMenu;
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
            btnUpdateTask.Click += btnUpdateTaskClick;

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
            taskList.Columns.Add("Data limite", 200);
            taskList.Columns.Add("Status", 240);

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

        private void ReadDescriptionMenuItem_Click(object sender, EventArgs e)
        {
            try 
            {
                ListViewItem selectedItem = taskList.SelectedItems[0];
                int taskId = Convert.ToInt32(selectedItem.SubItems[0].Text);

                string description = database.getTask(taskId).Description;
                using (DescriptionModalForm descriptionForm = new DescriptionModalForm(description))
                {
                    descriptionForm.ShowDialog();  
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro ao exibir descrição " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        //click botão direito na task
        private void checkMenuItem_Click(object sender , EventArgs e)
        {
            try
            {
                ListViewItem selectedItem = taskList.SelectedItems[0];
                int taskId = Convert.ToInt32(selectedItem.SubItems[0].Text);

                if (database.getTask(taskId).Status == "Expirado")
                {
                    MessageBox.Show("Tarefas expiradas não podem ser concluidas!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //Atualiza o status da task
                Task task = database.getTask(taskId);
                task.Status = "Concluido";
                database.updateTask(task, taskId);
                UpdateTaskList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exibir descrição " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddTaskClick(object sender, EventArgs e)
        {
            using (addOrUpdateTaskModal addTaskForm = new addOrUpdateTaskModal())
            {
                if (addTaskForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        database.InsertTask(addTaskForm.TaskName, addTaskForm.TaskDate, addTaskForm.TaskDescription);
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

        private void btnUpdateTaskClick(object sender, EventArgs e)
        {
            if (taskList.SelectedItems.Count > 0)
            {
                try
                {
                    int taskId = Convert.ToInt32(taskList.SelectedItems[0].SubItems[0].Text);
                    Task taskToUpdate = database.getTask(taskId);

                    if(taskToUpdate.Status == "Expirado")
                    {
                        MessageBox.Show("Tarefa expirada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Vou usar o modal de adicionar   
                    using (addOrUpdateTaskModal addForm = new addOrUpdateTaskModal(true,taskToUpdate))
                    {
                        if (addForm.ShowDialog() == DialogResult.OK)
                        {
                            // Atualiza a tarefa com os novos valores do formulário
                            taskToUpdate.Name = addForm.TaskName;
                            taskToUpdate.LimitDate = addForm.TaskDate;
                            taskToUpdate.Description = addForm.TaskDescription;
                            taskToUpdate.Status = "Pendente";

                            // Atualiza a tarefa no banco de dados
                            database.updateTask(taskToUpdate, taskId);

                            // Atualiza a lista de tarefas na interface
                            UpdateTaskList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar tarefa: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma tarefa para atualizar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);

                    using (var boldFont = new Font("Segoe UI", 12, FontStyle.Bold))
                    {
                        e.Graphics.DrawString(status, boldFont, Brushes.Black, e.Bounds);
                    }
                }

                else if (status == "Concluido")
                {
                    e.Graphics.FillRectangle(Brushes.Green, e.Bounds);

                    using (var boldFont = new Font("Segoe UI", 12, FontStyle.Bold))
                    {
                        e.Graphics.DrawString(status, boldFont, Brushes.Black, e.Bounds);
                    }
                }


                else if (status == "Expirado")
                {
                    e.Graphics.FillRectangle(Brushes.Red, e.Bounds);

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
            expireTasks();
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


                        DateTime taskDate = DateTime.Parse(data);
                        string formattedDate = taskDate.ToString("dd-MM-yyyy");

                        // Adiciona o item ao ListView
                        ListViewItem item = new ListViewItem(taskId); // Primeiro item é o ID (invisível)
                        item.SubItems.Add(nome);
                        item.SubItems.Add(formattedDate);  
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

        private void expireTasks()
        {
            List<Task> tasks = database.getAllTasksInObjectFormat();

            foreach (var task in tasks)
            {
                    if (task.LimitDate < DateTime.Today && task.Status != "Concluido")
                    {
                        //Atualiza o status da task
                        task.Status = "Expirado";
                        database.updateTask(task, task.Id);
                    }
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
