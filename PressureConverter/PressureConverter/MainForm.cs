using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PressureConverter
{
    public partial class MainForm : Form
    {
        private const double BarToKgfcm2 = 1.01972;

        public MainForm()
        {
            InitializeComponent();
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            this.Text = "Конвертер тиску - Бар ↔ кгс/см²";
            this.Size = new Size(750, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);
        }


        private void InitializeComponent()
        {
            Panel headerPanel = new Panel
            {
                BackColor = Color.FromArgb(44, 62, 80),
                Dock = DockStyle.Top,
                Height = 70
            };

            Label titleLabel = new Label
            {
                Text = "Конвертер тиску",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Бар ↔ кгс/см²",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(189, 195, 199),
                Location = new Point(0, 45),
                AutoSize = false,
                Width = 750,
                TextAlign = ContentAlignment.TopCenter
            };
            headerPanel.Controls.Add(subtitleLabel);

            Panel mainPanel = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(690, 540),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblBarTitle = new Label
            {
                Text = "Конвертація: Бар → кгс/см²",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblBar = new Label
            {
                Text = "Введіть значення в Барах:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 55),
                AutoSize = true
            };

            TextBox txtBar = new TextBox
            {
                Name = "txtBar",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 80),
                Size = new Size(200, 30)
            };

            Button btnBarToKgf = new Button
            {
                Text = "Конвертувати →",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(230, 77),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnBarToKgf.FlatAppearance.BorderSize = 0;

            Label lblResultKgf = new Label
            {
                Name = "lblResultKgf",
                Text = "Результат: -",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(380, 82),
                AutoSize = true
            };

            Panel separator1 = new Panel
            {
                BackColor = Color.FromArgb(189, 195, 199),
                Location = new Point(20, 130),
                Size = new Size(650, 2)
            };

            Label lblKgfTitle = new Label
            {
                Text = "Конвертація: кгс/см² → Бар",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 150),
                AutoSize = true
            };

            Label lblKgf = new Label
            {
                Text = "Введіть значення в кгс/см²:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 185),
                AutoSize = true
            };

            TextBox txtKgf = new TextBox
            {
                Name = "txtKgf",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 210),
                Size = new Size(200, 30)
            };

            Button btnKgfToBar = new Button
            {
                Text = "Конвертувати →",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(230, 207),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnKgfToBar.FlatAppearance.BorderSize = 0;

            Label lblResultBar = new Label
            {
                Name = "lblResultBar",
                Text = "Результат: -",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(380, 212),
                AutoSize = true
            };

            Panel separator2 = new Panel
            {
                BackColor = Color.FromArgb(189, 195, 199),
                Location = new Point(20, 260),
                Size = new Size(650, 2)
            };

            Button btnClear = new Button
            {
                Text = "Очистити",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 280),
                Size = new Size(120, 35),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            Button btnHelp = new Button
            {
                Text = "Довідка (Help)",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(150, 280),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            btnHelp.FlatAppearance.BorderSize = 0;

            GroupBox formulaBox = new GroupBox
            {
                Text = "Формула конвертації",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 330),
                Size = new Size(650, 55)
            };

            Label lblFormula = new Label
            {
                Text = "1 бар = 1.01972 кгс/см²\n1 кгс/см² = 0.980665 бар",
                Font = new Font("Consolas", 9),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(10, 20),
                AutoSize = true
            };
            formulaBox.Controls.Add(lblFormula);

            btnBarToKgf.Click += (s, e) => ConvertBarToKgf(txtBar, lblResultKgf);
            btnKgfToBar.Click += (s, e) => ConvertKgfToBar(txtKgf, lblResultBar);
            btnClear.Click += (s, e) => ClearAll(txtBar, txtKgf, lblResultKgf, lblResultBar);
            btnHelp.Click += (s, e) => ShowHelp();

            txtBar.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    ConvertBarToKgf(txtBar, lblResultKgf);
                    e.Handled = true;
                }
            };

            txtKgf.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    ConvertKgfToBar(txtKgf, lblResultBar);
                    e.Handled = true;
                }
            };

            mainPanel.Controls.Add(lblBarTitle);
            mainPanel.Controls.Add(lblBar);
            mainPanel.Controls.Add(txtBar);
            mainPanel.Controls.Add(btnBarToKgf);
            mainPanel.Controls.Add(lblResultKgf);
            mainPanel.Controls.Add(separator1);
            mainPanel.Controls.Add(lblKgfTitle);
            mainPanel.Controls.Add(lblKgf);
            mainPanel.Controls.Add(txtKgf);
            mainPanel.Controls.Add(btnKgfToBar);
            mainPanel.Controls.Add(lblResultBar);
            mainPanel.Controls.Add(separator2);
            mainPanel.Controls.Add(btnClear);
            mainPanel.Controls.Add(btnHelp);
            mainPanel.Controls.Add(formulaBox);

            this.Controls.Add(headerPanel);
            this.Controls.Add(mainPanel);
        }


        private void ConvertBarToKgf(TextBox input, Label output)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Text))
                {
                    MessageBox.Show("Будь ласка, введіть значення!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double barValue = double.Parse(input.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

                if (barValue < 0)
                {
                    MessageBox.Show("Значення не може бути від'ємним!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double kgfResult = barValue * BarToKgfcm2;
                output.Text = $"Результат: {kgfResult:F4} кгс/см²";
                output.ForeColor = Color.FromArgb(39, 174, 96);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неправильний формат числа! Використовуйте цифри та крапку/кому.",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ConvertKgfToBar(TextBox input, Label output)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Text))
                {
                    MessageBox.Show("Будь ласка, введіть значення!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double kgfValue = double.Parse(input.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

                if (kgfValue < 0)
                {
                    MessageBox.Show("Значення не може бути від'ємним!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double barResult = kgfValue / BarToKgfcm2;
                output.Text = $"Результат: {barResult:F4} бар";
                output.ForeColor = Color.FromArgb(39, 174, 96);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неправильний формат числа! Використовуйте цифри та крапку/кому.",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearAll(TextBox txt1, TextBox txt2, Label lbl1, Label lbl2)
        {
            txt1.Clear();
            txt2.Clear();
            lbl1.Text = "Результат: -";
            lbl2.Text = "Результат: -";
            lbl1.ForeColor = Color.FromArgb(41, 128, 185);
            lbl2.ForeColor = Color.FromArgb(41, 128, 185);
        }

        private void ShowHelp()
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();
        }
    }
}