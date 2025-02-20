using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskManager
{
    public abstract class ModalForm : Form {

        public abstract void setupUIForm();

    }

    public class AddTaskModalForm : ModalForm
    {
        public string TaskName { get; private set; }
        public DateTime TaskDate { get; private set; }
        public string TaskDescription { get; private set; }

        private TextBox txtName;
        private DateTimePicker datePicker;
        private Button btnSave;
        private Button btnCancel;
        private TextBox txtDescription;

        public AddTaskModalForm()
        {
            setupUIForm();
        }

        public override void setupUIForm()
        {
            // Configuração da Janela
            this.Text = "Adicionar Nova Tarefa";
            this.Size = new Size(425, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Cores Bootstrap
            Color primaryColor = Color.FromArgb(13, 110, 253);  // Azul Bootstrap
            Color secondaryColor = Color.FromArgb(33, 37, 41);  // Cinza Escuro
            Color textColor = Color.White;

            // Nome da Tarefa
            Label lblName = new Label()
            {
                Text = "Nome",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            txtName = new TextBox()
            {
                Location = new Point(30, 60),
                Width = 350,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Data da Tarefa
            Label lblDate = new Label()
            {
                Text = "Data limite",
                Location = new Point(20, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            datePicker = new DateTimePicker()
            {
                Location = new Point(30, 115),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MM-yyyy",
                Font = new Font("Segoe UI", 12),
                Width = 350
            };

            // Descrição da Tarefa
            Label lblDescription = new Label()
            {
                Text = "Descrição",
                Location = new Point(20, 150),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            txtDescription = new TextBox()
            {
                Location = new Point(30, 180),
                Width = 350,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true, // Faz o campo de texto ser multiline
                Height = 200 // Ajuste de altura para acomodar mais texto
            };

            // Botão Salvar
            btnSave = new Button()
            {
                Text = "Salvar",
                Location = new Point(80, 400),
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = primaryColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Botão Cancelar (Fechar)
            btnCancel = new Button()
            {
                Text = "Fechar",
                Location = new Point(210, 400),
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
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
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
            TaskDate = datePicker.Value;
            TaskDescription = txtDescription.Text;

            this.DialogResult = DialogResult.OK;
        }
    }


    public class DescriptionModalForm : ModalForm
    {
        private Button btnClose;
        private TextBox txtDescription;

        public string Description { get; private set; }

        public DescriptionModalForm(string description)
        {
            Description = description;
            setupUIForm();
        }

        public override void setupUIForm()
        {
            // Configuração da Janela
            this.Text = "Descrição da Tarefa";
            this.Size = new Size(400, 340);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Cores Bootstrap
            Color primaryColor = Color.FromArgb(13, 110, 253);  // Azul Bootstrap
            Color textColor = Color.White;

            // Descrição
            Label lblDescription = new Label()
            {
                Text = "Descrição da Tarefa:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true
            };

            txtDescription = new TextBox()
            {
                Text = Description,
                Location = new Point(20, 40),
                Width = 340,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                Height = 200,
                ReadOnly = true,
                Cursor = Cursors.Default
            };
            txtDescription.Enabled = false;  

            btnClose = new Button()
            {
                Text = "Fechar",
                Location = new Point(140, 250),
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = primaryColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (sender, e) => this.DialogResult = DialogResult.OK;

            // Adicionando os controles ao formulário
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnClose);
        }
    }

}

