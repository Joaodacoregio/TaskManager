using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{


    public class AddTaskForm : Form
    {
        public string TaskName { get; private set; }
        public string TaskDate { get; private set; }
        public string TaskDescription { get; private set; }

        private TextBox txtName;
        private DateTimePicker datePicker;
        private Button btnSave;
        private Button btnCancel;
        private TextBox txtDescription;
        public AddTaskForm()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            // Configuração da Janela
            this.Text = "Adicionar Nova Tarefa";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Cores Bootstrap
            Color primaryColor = Color.FromArgb(13, 110, 253);  // Azul Bootstrap
            Color secondaryColor = Color.FromArgb(33, 37, 41);  // Cinza Escuro
            Color textColor = Color.White;

            // Nome da Tarefa
            Label lblName = new Label() { Text = "Nome:", Location = new Point(20, 30), AutoSize = true };
            txtName = new TextBox()
            {
                Location = new Point(100, 25),
                Width = 250,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Data da Tarefa
            Label lblDate = new Label() { Text = "Data:", Location = new Point(20, 80), AutoSize = true };
            datePicker = new DateTimePicker()
            {
                Location = new Point(100, 75),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MM-yyyy",
                Font = new Font("Segoe UI", 10),
                Width = 250
            };

            // Descrição da Tarefa
            Label lblDescription = new Label() { Text = "Descrição:", Location = new Point(20, 120), AutoSize = true };
            txtDescription = new TextBox()
            {
                Location = new Point(100, 115), // Ajuste aqui para o campo de descrição
                Width = 250,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true, // Faz o campo de texto ser multiline
                Height = 60 // Ajuste de altura para acomodar mais texto
            };

            btnSave = new Button()
            {
                Text = "Salvar",
                Location = new Point(100, 168),
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = primaryColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Botão Cancelar
            btnCancel = new Button()
            {
                Text = "Cancelar",
                Location = new Point(230, 168),
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = secondaryColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (sender, e) => this.DialogResult = DialogResult.Cancel;

            // Adicionando os controles ao formulário
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDate);
            this.Controls.Add(datePicker);
            this.Controls.Add(lblDescription); // Adicionando o label da descrição
            this.Controls.Add(txtDescription); // Adicionando o campo de descrição
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("O nome da tarefa não pode estar vazio!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TaskName = txtName.Text;
            TaskDate = datePicker.Value.ToString("dd-MM-yyyy");
            TaskDescription = txtDescription.Text;

            this.DialogResult = DialogResult.OK;
        }
    }
}
