using System.Globalization;
using System.Windows;
using SharpProp;
using UnitsNet.NumberExtensions.NumberToPressure;
using UnitsNet.NumberExtensions.NumberToDensity;

namespace WaterSteamCalculator;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        InfoText.Text = "Готово до розрахунку. SharpProp wrapper для CoolProp.";
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        var pressureMPa = ParseDouble(PressureInput.Text);
        var specificVolume = ParseDouble(SpecificVolumeInput.Text);
        var densityValue = 1.0 / specificVolume;
        try
        {
            var water = new Fluid(FluidsList.Water).WithState(
                Input.Pressure(pressureMPa.Megapascals()),
                Input.Density(densityValue.KilogramsPerCubicMeter())
            );

            TemperatureResult.Text =
                $"Температура (T): {water.Temperature.Kelvins:F2} K ({water.Temperature.DegreesCelsius:F2} °C)";
            DensityResult.Text = $"Густина (ρ): {water.Density.KilogramsPerCubicMeter:F4} кг/м³";
            EnthalpyResult.Text = $"Ентальпія (h): {water.Enthalpy.KilojoulesPerKilogram:F2} кДж/кг";
            EntropyResult.Text = $"Ентропія (s): {water.Entropy.KilojoulesPerKilogramKelvin:F4} кДж/(кг·К)";

            InfoText.Text = "Розрахунок виконано успішно!";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка розрахунку: {ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            InfoText.Text = $"Помилка: {ex.Message}";
        }
    }

    private static double ParseDouble(string input)
    {
        var normalized = input.Replace(',', '.');

        return double.Parse(normalized, CultureInfo.InvariantCulture);
    }
}