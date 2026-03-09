using System;
using System.Collections.Generic;
using System.Windows;

namespace HeatExchangerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ── Обробник кнопки ────────────────────────────────────────────
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double F    = ParseInput(txtF.Text,   "F");
                double K    = ParseInput(txtK.Text,   "K");
                double ThIn = ParseInput(txtThIn.Text,"Th_вх");
                double TcIn = ParseInput(txtTcIn.Text,"Tc_вх");
                double Gh   = ParseInput(txtGh.Text,  "Gh") * 1000; // т/год → кг/год
                double Ch   = ParseInput(txtCh.Text,  "Ch");
                double Gc   = ParseInput(txtGc.Text,  "Gc") * 1000;
                double Cc   = ParseInput(txtCc.Text,  "Cc");

                // ── Основний розрахунок ──────────────────────────────
                var p = Solve(F, K, ThIn, TcIn, Gh, Ch, Gc, Cc, false);
                var c = Solve(F, K, ThIn, TcIn, Gh, Ch, Gc, Cc, true);

                // ── Вивід результатів прямотоку ──────────────────────
                lblP_ThOut.Text = $"Th_вих: {p.ThOut:F2} °C";
                lblP_TcOut.Text = $"Tc_вих: {p.TcOut:F2} °C";
                lblP_Q.Text     = $"Q: {p.Q:N0} ккал/год";
                lblP_DT.Text    = $"ΔTлог: {p.Lmtd:F2} °C";

                // ── Вивід результатів протитоку ──────────────────────
                lblC_ThOut.Text = $"Th_вих: {c.ThOut:F2} °C";
                lblC_TcOut.Text = $"Tc_вих: {c.TcOut:F2} °C";
                lblC_Q.Text     = $"Q: {c.Q:N0} ккал/год";
                lblC_DT.Text    = $"ΔTлог: {c.Lmtd:F2} °C";

                // ── Порівняння: яка F потрібна протитоку для Q прямотоку ──
                double Fc   = RequiredCounterFlowArea(p.Q, K, ThIn, TcIn, Gh, Ch, Gc, Cc);
                double pct  = (F - Fc) / F * 100.0;
                lblCmpFCounter.Text   = $"F протитоку (той самий Q): {Fc:F1} м²";
                lblCmpReduction.Text  = $"Зменшення площі: {pct:F1}%";

                // ── Таблиця дослідження ──────────────────────────────
                dgResults.ItemsSource = BuildTable(K, ThIn, TcIn, Gh, Ch, Gc, Cc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Метод послідовних наближень (бісекція) ─────────────────────
        private Result Solve(double F, double K,
                             double ThIn, double TcIn,
                             double Gh, double Ch, double Gc, double Cc,
                             bool counter)
        {
            double ChGh = Ch * Gh;
            double CcGc = Cc * Gc;

            // Визначення меж пошуку для ThOut
            double lo, hi;
            if (counter)
            {
                // ThOut_min: холодний вихід не може перевищити гарячий вхід
                double ThOutMin = ThIn - (ThIn - TcIn) * CcGc / ChGh;
                lo = Math.Max(TcIn, ThOutMin) + 0.5;
            }
            else
            {
                // Точка защемлення (ThOut = TcOut)
                double Qmax = (ThIn - TcIn) / (1.0 / CcGc + 1.0 / ChGh);
                lo = ThIn - Qmax / ChGh + 0.5;
            }
            hi = ThIn - 0.5;

            double fLo = CalcArea(lo, K, ThIn, TcIn, ChGh, CcGc, counter);
            double fHi = CalcArea(hi, K, ThIn, TcIn, ChGh, CcGc, counter);

            if (fLo < F)
                throw new InvalidOperationException(
                    $"При F={F} м² ({(counter?"протитік":"прямотік")}): задана площа перевищує максимально досяжну ({fLo:F0} м²).");

            // Бісекція
            double ThOut = (lo + hi) / 2.0;
            for (int i = 0; i < 3000; i++)
            {
                ThOut = (lo + hi) / 2.0;
                double fMid = CalcArea(ThOut, K, ThIn, TcIn, ChGh, CcGc, counter);

                if (Math.Abs(fMid - F) < 0.01) break;

                if (fMid > F) lo = ThOut;  // великa F → збільшуємо ThOut (менше тепло)
                else          hi = ThOut;  // мала F → зменшуємо ThOut (більше тепло)
            }

            double Q    = ChGh * (ThIn - ThOut);
            double TcOut = TcIn + Q / CcGc;
            double lmtd  = Lmtd(ThIn, ThOut, TcIn, TcOut, counter);
            return new Result(ThOut, TcOut, Q, lmtd);
        }

        // ── Розрахунок площі для заданого ThOut ───────────────────────
        private double CalcArea(double ThOut, double K,
                                double ThIn, double TcIn,
                                double ChGh, double CcGc, bool counter)
        {
            double Q    = ChGh * (ThIn - ThOut);
            if (Q <= 0) return 0;
            double TcOut = TcIn + Q / CcGc;
            double lmtd  = Lmtd(ThIn, ThOut, TcIn, TcOut, counter);
            if (double.IsNaN(lmtd) || lmtd <= 0) return double.MaxValue;
            return Q / (K * lmtd);
        }

        // ── Середня логарифмічна різниця температур ───────────────────
        private double Lmtd(double ThIn, double ThOut,
                            double TcIn, double TcOut, bool counter)
        {
            double dT1 = counter ? (ThIn - TcOut) : (ThIn - TcIn);
            double dT2 = counter ? (ThOut - TcIn) : (ThOut - TcOut);

            if (dT1 <= 0 || dT2 <= 0) return double.NaN;
            if (Math.Abs(dT1 - dT2) < 1e-9) return dT1;
            return (dT1 - dT2) / Math.Log(dT1 / dT2);
        }

        // ── F протитоку для заданого Q (прямий розрахунок) ────────────
        private double RequiredCounterFlowArea(double Q, double K,
                                               double ThIn, double TcIn,
                                               double Gh, double Ch,
                                               double Gc, double Cc)
        {
            double ChGh = Ch * Gh, CcGc = Cc * Gc;
            double ThOut = ThIn - Q / ChGh;
            double TcOut = TcIn + Q / CcGc;
            double lmtd  = Lmtd(ThIn, ThOut, TcIn, TcOut, true);
            return Q / (K * lmtd);
        }

        // ── Таблиця дослідження ────────────────────────────────────────
        private List<TableRow> BuildTable(double K,
                                          double ThIn, double TcIn,
                                          double Gh, double Ch,
                                          double Gc, double Cc)
        {
            var rows = new List<TableRow>();
            int[] areas = { 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

            foreach (int f in areas)
            {
                try
                {
                    var p = Solve(f, K, ThIn, TcIn, Gh, Ch, Gc, Cc, false);
                    var c = Solve(f, K, ThIn, TcIn, Gh, Ch, Gc, Cc, true);
                    double dq = (c.Q - p.Q) / p.Q * 100.0;
                    rows.Add(new TableRow
                    {
                        F       = f,
                        P_ThOut = $"{p.ThOut:F1}",
                        P_TcOut = $"{p.TcOut:F1}",
                        C_ThOut = $"{c.ThOut:F1}",
                        C_TcOut = $"{c.TcOut:F1}",
                        DeltaQ  = $"{dq:+0.0;-0.0;0.0}%"
                    });
                }
                catch { /* пропускаємо нефізичні значення */ }
            }
            return rows;
        }

        private double ParseInput(string text, string name)
        {
            if (!double.TryParse(text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double val))
                throw new FormatException($"Некоректне значення для «{name}».");
            return val;
        }
    }

    // ── Допоміжні класи ────────────────────────────────────────────────
    public record Result(double ThOut, double TcOut, double Q, double Lmtd);

    public class TableRow
    {
        public int    F       { get; set; }
        public string P_ThOut { get; set; } = "";
        public string P_TcOut { get; set; } = "";
        public string C_ThOut { get; set; } = "";
        public string C_TcOut { get; set; } = "";
        public string DeltaQ  { get; set; } = "";
    }
}
