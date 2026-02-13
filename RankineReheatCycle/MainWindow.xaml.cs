using System.Globalization;
using System.Windows;
using SharpProp;
using UnitsNet;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace RankineReheatCycle;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private bool TryParseDouble(string value, out double result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return false;
        }

        string normalized = value.Trim().Replace(',', '.');

        if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
        {
            return !double.IsNaN(result) && !double.IsInfinity(result);
        }

        return false;
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            txtStatus.Text = "Виконується розрахунок...";

            if (!TryParseDouble(txtP1.Text, out double P1_MPa) || P1_MPa <= 0)
            {
                MessageBox.Show("P₁ має бути додатнім числом", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDouble(txtT1.Text, out double T1) || T1 < 0 || T1 > 800)
            {
                MessageBox.Show("t₁ має бути в діапазоні 0-800°C", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDouble(txtP2.Text, out double P2_MPa) || P2_MPa <= 0)
            {
                MessageBox.Show("P₂ має бути додатнім числом", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDouble(txtPReheat.Text, out double PReheat_MPa) || PReheat_MPa <= 0)
            {
                MessageBox.Show("P' має бути додатнім числом", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDouble(txtTReheatBase.Text, out double TReheatBase) || TReheatBase < 0 || TReheatBase > 800)
            {
                MessageBox.Show("t' має бути в діапазоні 0-800°C", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDouble(txtVariation.Text, out double variation) || variation < 0 || variation > 100)
            {
                MessageBox.Show("Зміна має бути в діапазоні 0-100%", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtPoints.Text, out int points) || points < 2 || points > 100)
            {
                MessageBox.Show("Кількість точок має бути в діапазоні 2-100", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Перетворення в Па
            double P1 = P1_MPa * 1e6;
            double P2 = P2_MPa * 1e6;
            double PReheat = PReheat_MPa * 1e6;
            double variationDecimal = variation / 100.0;

            // Перевірка логіки тисків
            if (P2 >= P1)
            {
                MessageBox.Show("P₂ має бути менше P₁", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PReheat <= P2 || PReheat >= P1)
            {
                MessageBox.Show("P' має бути між P₂ та P₁", "Помилка вводу",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Діапазон зміни температури вторинного перегріву
            double TReheatMin = TReheatBase * (1 - variationDecimal);
            double TReheatMax = TReheatBase * (1 + variationDecimal);

            // Debug інформація
            txtStatus.Text = $"Розрахунок для t' = {TReheatMin:F1}...{TReheatMax:F1}°C";

            // Результати
            var results = new List<CalculationResult>();

            for (int i = 0; i < points; i++)
            {
                double TReheat = TReheatMin + (TReheatMax - TReheatMin) * i / (points - 1);

                // Перевірка на NaN
                if (double.IsNaN(TReheat) || double.IsInfinity(TReheat))
                {
                    MessageBox.Show($"Помилка обчислення температури на кроці {i}",
                        "Помилка розрахунку", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Розрахунок ККД циклу з вторинним перегрівом
                double efficiency = CalculateReheatCycleEfficiency(P1, T1, P2, PReheat, TReheat);

                // Перевірка результату
                if (double.IsNaN(efficiency) || double.IsInfinity(efficiency))
                {
                    MessageBox.Show($"Помилка розрахунку ККД при t'={TReheat:F2}°C",
                        "Помилка розрахунку", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                results.Add(new CalculationResult
                {
                    TReheat = Math.Round(TReheat, 2),
                    Efficiency = Math.Round(efficiency * 100, 3)
                });
            }

            // Відображення результатів
            dgResults.ItemsSource = results;

            // Побудова графіка
            PlotChart(results);

            // Розрахунок ККД Карно
            double T1K = T1 + 273.15;
            double TCarnotCold = GetSaturationTemperature(P2) - 273.15;
            double efficiencyCarno = (T1 - TCarnotCold) / T1K * 100;

            var maxResult = results.OrderByDescending(r => r.Efficiency).First();

            txtCarnotComparison.Text =
                $"ККД циклу Карно: {efficiencyCarno:F3}%\n" +
                $"Максимальний ККД Ренкіна: {maxResult.Efficiency:F3}%\n" +
                $"Різниця: {(efficiencyCarno - maxResult.Efficiency):F3}%";

            // Оптимальне значення
            txtOptimal.Text = $"t' = {maxResult.TReheat}°C\nККД = {maxResult.Efficiency}%";

            txtStatus.Text = "Розрахунок завершено успішно!";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка: {ex.Message}\n\n{ex.StackTrace}",
                "Помилка розрахунку", MessageBoxButton.OK, MessageBoxImage.Error);
            txtStatus.Text = "Помилка розрахунку";
        }
    }

    private double CalculateReheatCycleEfficiency(double P1, double T1Celsius, double P2,
        double PReheat, double TReheatCelsius)
    {
        try
        {
            if (double.IsNaN(P1) || double.IsNaN(T1Celsius) || double.IsNaN(P2) ||
                double.IsNaN(PReheat) || double.IsNaN(TReheatCelsius))
            {
                throw new ArgumentException("Один з параметрів має значення NaN");
            }

            var water = new Fluid(FluidsList.Water);

            // Точка 1: Вхід у турбіну ВТ (перегріта пара)
            water.Update(
                Input.Pressure(Pressure.FromPascals(P1)),
                Input.Temperature(Temperature.FromDegreesCelsius(T1Celsius))
            );
            double h1 = water.Enthalpy.JoulesPerKilogram;
            double s1 = water.Entropy.JoulesPerKilogramKelvin;

            // Точка 2: Вихід з турбіни ВТ (ізоентропійне розширення до P')
            water.Update(
                Input.Pressure(Pressure.FromPascals(PReheat)),
                Input.Entropy(SpecificEntropy.FromJoulesPerKilogramKelvin(s1))
            );
            double h2 = water.Enthalpy.JoulesPerKilogram;

            // Точка 3: Після вторинного перегріву (при P' і t')
            water.Update(
                Input.Pressure(Pressure.FromPascals(PReheat)),
                Input.Temperature(Temperature.FromDegreesCelsius(TReheatCelsius))
            );
            double h3 = water.Enthalpy.JoulesPerKilogram;
            double s3 = water.Entropy.JoulesPerKilogramKelvin;

            // Точка 4: Вихід з турбіни НТ (ізоентропійне розширення до P2)
            water.Update(
                Input.Pressure(Pressure.FromPascals(P2)),
                Input.Entropy(SpecificEntropy.FromJoulesPerKilogramKelvin(s3))
            );
            double h4 = water.Enthalpy.JoulesPerKilogram;

            // Точка 5: Після конденсатора (насичена рідина при P2)
            water.Update(
                Input.Pressure(Pressure.FromPascals(P2)),
                Input.Quality(Ratio.FromDecimalFractions(0))
            );
            double h5 = water.Enthalpy.JoulesPerKilogram;
            double v5 = water.Density.KilogramsPerCubicMeter;

            // Точка 6: Після насоса (приблизно)
            double h6 = h5 + (P1 - P2) / v5;

            // Робота турбін
            double WTurbineHP = h1 - h2; // Турбіна ВТ
            double WTurbineLP = h3 - h4; // Турбіна НТ
            double WTurbineTotal = WTurbineHP + WTurbineLP;

            // Робота насоса
            double WPump = h6 - h5;

            // Підведене тепло
            double QBoiler = h1 - h6; // В котлі
            double QReheat = h3 - h2; // Вторинний перегрів
            double QTotal = QBoiler + QReheat;

            // Перевірка на ділення на нуль
            if (Math.Abs(QTotal) < 1e-6)
            {
                throw new InvalidOperationException("Підведене тепло дорівнює нулю");
            }

            // ККД циклу
            double efficiency = (WTurbineTotal - WPump) / QTotal;

            return efficiency;
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка при розрахунку для TReheat={TReheatCelsius}°C: {ex.Message}", ex);
        }
    }

    private double GetSaturationTemperature(double pressure)
    {
        try
        {
            var water = new Fluid(FluidsList.Water);
            water.Update(
                Input.Pressure(Pressure.FromPascals(pressure)),
                Input.Quality(Ratio.FromDecimalFractions(0))
            );
            return water.Temperature.Kelvins;
        }
        catch
        {
            return 273.15 + 28.96; // Приблизна для 0.004 МПа
        }
    }

    private void PlotChart(List<CalculationResult> results)
    {
        var plotModel = new PlotModel
        {
            Title = "Залежність ККД від температури вторинного перегріву",
            TitleFontSize = 16
        };

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Температура вторинного перегріву t' (°C)",
            TitleFontSize = 14,
            FontSize = 12,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "ККД (%)",
            TitleFontSize = 14,
            FontSize = 12,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        var lineSeries = new LineSeries
        {
            Title = "ККД циклу Ренкіна з вторинним перегрівом",
            Color = OxyColors.Blue,
            StrokeThickness = 2,
            MarkerType = MarkerType.Circle,
            MarkerSize = 4,
            MarkerFill = OxyColors.Blue
        };

        foreach (var result in results)
        {
            lineSeries.Points.Add(new DataPoint(result.TReheat, result.Efficiency));
        }

        plotModel.Series.Add(lineSeries);
        plotView.Model = plotModel;
    }
}

public class CalculationResult
{
    public double TReheat { get; set; }
    public double Efficiency { get; set; }
}