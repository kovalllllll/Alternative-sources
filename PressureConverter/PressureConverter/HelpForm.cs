using System;
using System.Drawing;
using System.Windows.Forms;

namespace PressureConverter
{
    
    public class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Довідка - Конвертер тиску";
            this.Size = new Size(850, 870);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);

            Panel headerPanel = new Panel
            {
                BackColor = Color.FromArgb(41, 128, 185),
                Dock = DockStyle.Top,
                Height = 60
            };

            Label titleLabel = new Label
            {
                Text = "📖 Довідкова інформація",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(titleLabel);

            Panel contentPanel = new Panel
            {
                Location = new Point(15, 75),
                Size = new Size(800, 650),
                BackColor = Color.White,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            int yPosition = 15;

            AddSectionTitle(contentPanel, "🔵 Що таке Бар (bar)?", ref yPosition);
            AddInfoText(contentPanel, 
                "Бар — це одиниця вимірювання тиску в метричній системі одиниць.\n" +
                "1 бар приблизно дорівнює атмосферному тиску на рівні моря.\n\n" +
                "📊 Співвідношення:\n" +
                "• 1 бар = 100,000 Па (Паскалів)\n" +
                "• 1 бар = 0.1 МПа (Мегапаскалів)\n" +
                "• 1 бар ≈ 0.987 атм (Атмосфер)\n" +
                "• 1 бар = 1.01972 кгс/см²",
                ref yPosition);

            AddSectionTitle(contentPanel, "💼 Застосування Бару:", ref yPosition);
            AddInfoText(contentPanel, 
                "✓ Метеорологія (атмосферний тиск)\n" +
                "✓ Промислове обладнання\n" +
                "✓ Гідравлічні та пневматичні системи\n" +
                "✓ Автомобільні шини\n" +
                "✓ Компресори та насоси\n" +
                "✓ Системи водопостачання",
                ref yPosition);

            AddDivider(contentPanel, ref yPosition);

            AddSectionTitle(contentPanel, "🔴 Що таке кілограм-сила на квадратний сантиметр (кгс/см²)?", ref yPosition);
            AddInfoText(contentPanel, 
                "кгс/см² (кілограм-сила на квадратний сантиметр) — технічна одиниця\n" +
                "вимірювання тиску, яка широко використовується в пострадянських країнах.\n\n" +
                "Також відома як:\n" +
                "• ат (технічна атмосфера)\n" +
                "• kg/cm² (в міжнародному позначенні)\n" +
                "• kgf/cm² (kilogram-force per square centimeter)\n\n" +
                "📊 Співвідношення:\n" +
                "• 1 кгс/см² = 98,066.5 Па\n" +
                "• 1 кгс/см² = 0.980665 бар\n" +
                "• 1 кгс/см² ≈ 0.968 атм",
                ref yPosition);

            AddSectionTitle(contentPanel, "💼 Застосування кгс/см²:", ref yPosition);
            AddInfoText(contentPanel, 
                "✓ Опалювальні системи\n" +
                "✓ Газові балони\n" +
                "✓ Водопровідні мережі\n" +
                "✓ Котли та бойлери\n" +
                "✓ Манометри та датчики тиску\n" +
                "✓ Радянське та пострадянське обладнання",
                ref yPosition);

            AddDivider(contentPanel, ref yPosition);

            AddSectionTitle(contentPanel, "📐 Формули конвертації:", ref yPosition);
            
            GroupBox formulaBox = new GroupBox
            {
                Text = "Математичні формули",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(15, yPosition),
                Size = new Size(760, 120),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Label formulaLabel = new Label
            {
                Text = 
                    "Конвертація з Барів в кгс/см²:\n" +
                    "   P(кгс/см²) = P(бар) × 1.01972\n\n" +
                    "Конвертація з кгс/см² в Бари:\n" +
                    "   P(бар) = P(кгс/см²) / 1.01972\n",
                Font = new Font("Consolas", 10),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(15, 25),
                AutoSize = true
            };
            formulaBox.Controls.Add(formulaLabel);
            contentPanel.Controls.Add(formulaBox);
            yPosition += 135;

            AddSectionTitle(contentPanel, "💡 Приклади конвертації:", ref yPosition);
            
            GroupBox exampleBox = new GroupBox
            {
                Text = "Практичні приклади",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(15, yPosition),
                Size = new Size(760, 140),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Label exampleLabel = new Label
            {
                Text = 
                    "Приклад 1:\n" +
                    "   5 бар = 5 × 1.01972 = 5.0986 кгс/см²\n\n" +
                    "Приклад 2:\n" +
                    "   10 кгс/см² = 10 / 1.01972 = 9.8067 бар\n\n" +
                    "Приклад 3 (типовий тиск у водопроводі):\n" +
                    "   3 бар = 3 × 1.01972 = 3.0592 кгс/см²\n\n" +
                    "Приклад 4 (тиск у автомобільній шині):\n" +
                    "   2.2 бар = 2.2 × 1.01972 = 2.2434 кгс/см²",
                Font = new Font("Consolas", 9),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(15, 25),
                AutoSize = true
            };
            exampleBox.Controls.Add(exampleLabel);
            contentPanel.Controls.Add(exampleBox);
            yPosition += 155;

            AddSectionTitle(contentPanel, "ℹ️ Корисна інформація:", ref yPosition);
            AddInfoText(contentPanel, 
                "• Обидві одиниці широко використовуються для вимірювання тиску\n" +
                "• Бар є більш сучасною та міжнародною одиницею\n" +
                "• кгс/см² часто зустрічається на старому обладнанні\n" +
                "• При виборі обладнання важливо знати, в яких одиницях\n" +
                "  вимірюється робочий тиск\n" +
                "• Для точних розрахунків використовуйте всі знаки після коми",
                ref yPosition);

            Button btnClose = new Button
            {
                Text = "Закрити",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(360, 735),
                Size = new Size(120, 40),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
            this.Controls.Add(btnClose);
        }

        private void AddSectionTitle(Panel panel, string text, ref int yPosition)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(15, yPosition),
                AutoSize = true
            };
            panel.Controls.Add(label);
            yPosition += 35;
        }

        private void AddInfoText(Panel panel, string text, ref int yPosition)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(15, yPosition),
                MaximumSize = new Size(760, 0),
                AutoSize = true
            };
            panel.Controls.Add(label);
            yPosition += label.Height + 15;
        }

        private void AddDivider(Panel panel, ref int yPosition)
        {
            Panel divider = new Panel
            {
                BackColor = Color.FromArgb(189, 195, 199),
                Location = new Point(15, yPosition),
                Size = new Size(760, 2)
            };
            panel.Controls.Add(divider);
            yPosition += 20;
        }
    }
}