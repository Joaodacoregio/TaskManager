using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    public partial class MainForm : Form
    {
        private Panel sideMenu;
        private Panel mainPanel;
        private ListView taskList;

        public MainForm()
        {
            InitializeComponent();
            CustomizeUI();
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
                Size = new Size(750, 600), // Tamanho estático
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
    }
}
