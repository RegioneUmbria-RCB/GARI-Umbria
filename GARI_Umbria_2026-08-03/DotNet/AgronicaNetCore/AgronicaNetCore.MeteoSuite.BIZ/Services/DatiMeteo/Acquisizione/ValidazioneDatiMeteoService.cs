using AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Implementation of the meteorological data validation service.
    /// Validates meteorological data values against acceptable ranges and completeness.
    /// </summary>
    public class ValidazioneDatiMeteoService : IValidazioneDatiMeteoService
    {
        // Validation range constants from DS02-BL_
        private const decimal MinTemperature = -50m;
        private const decimal MaxTemperature = 60m;
        private const decimal MinRelativeHumidity = 0m;
        private const decimal MaxRelativeHumidity = 100m;
        private const decimal MinPrecipitation = 0m;
        private const decimal MinLeafWetness = 0m;
        private const decimal MaxLeafWetness = 1m;
        private const double DataCompletionThreshold = 0.95; // 95% minimum for valid data

        public ValidazioneDatiMeteoResult ValidateMeteoRainDataH(MeteoRainDataH meteoData)
        {
            var result = new ValidazioneDatiMeteoResult
            {
                IsValid = true,
                IsComplete = true
            };

            // Check completeness - all parameters should be present
            if (!meteoData.Temp.HasValue)
            {
                result.IsComplete = false;
                result.ValidationMessages.Add("Temperature (Temp) is missing");
            }

            if (!meteoData.Prec.HasValue)
            {
                result.IsComplete = false;
                result.ValidationMessages.Add("Precipitation (Prec) is missing");
            }

            if (!meteoData.RelHum.HasValue)
            {
                result.IsComplete = false;
                result.ValidationMessages.Add("Relative Humidity (RelHum) is missing");
            }

            if (!meteoData.Lw.HasValue)
            {
                result.IsComplete = false;
                result.ValidationMessages.Add("Leaf Wetness (Lw) is missing");
            }

            // Validate ranges for values that are present
            if (meteoData.Temp.HasValue)
            {
                if (meteoData.Temp < MinTemperature || meteoData.Temp > MaxTemperature)
                {
                    result.IsValid = false;
                    result.ValidationMessages.Add(
                        $"Temperature value {meteoData.Temp}°C is outside acceptable range [{MinTemperature}, {MaxTemperature}]°C");
                }
            }

            if (meteoData.Prec.HasValue)
            {
                if (meteoData.Prec < MinPrecipitation)
                {
                    result.IsValid = false;
                    result.ValidationMessages.Add(
                        $"Precipitation value {meteoData.Prec}mm is negative, must be >= 0 mm");
                }
            }

            if (meteoData.RelHum.HasValue)
            {
                if (meteoData.RelHum < MinRelativeHumidity || meteoData.RelHum > MaxRelativeHumidity)
                {
                    result.IsValid = false;
                    result.ValidationMessages.Add(
                        $"Relative Humidity value {meteoData.RelHum}% is outside acceptable range [{MinRelativeHumidity}, {MaxRelativeHumidity}]%");
                }
            }

            if (meteoData.Lw.HasValue)
            {
                if (meteoData.Lw < MinLeafWetness || meteoData.Lw > MaxLeafWetness)
                {
                    result.IsValid = false;
                    result.ValidationMessages.Add(
                        $"Leaf Wetness value {meteoData.Lw} is outside acceptable range [{MinLeafWetness}, {MaxLeafWetness}]");
                }
            }

            return result;
        }

        public string DetermineOverallRainDataStatus(List<MeteoRainDataH> dataPoints, List<ValidazioneDatiMeteoResult> validationResults)
        {
            if (dataPoints == null || dataPoints.Count == 0 || validationResults == null || validationResults.Count == 0)
            {
                return "INCOMPLETO";
            }

            // Check for any anomalies (data out of range)
            bool hasAnomalies = validationResults.Any(r => !r.IsValid);
            if (hasAnomalies)
            {
                return "ANOMALO";
            }

            // Check for incomplete data or insufficient data coverage
            bool hasIncompleteData = validationResults.Any(r => !r.IsComplete);
            if (hasIncompleteData)
            {
                return "INCOMPLETO";
            }

            // Check data completion threshold: at least 95% of expected data in 24h windows
            if (!IsSufficientDataCoverage(dataPoints))
            {
                return "INCOMPLETO";
            }

            // All validations passed
            return "VALIDO";
        }

        /// <summary>
        /// Verifies that there is at least 95% data coverage in each 24-hour window.
        /// </summary>
        private bool IsSufficientDataCoverage(List<MeteoRainDataH> dataPoints)
        {
            if (dataPoints.Count == 0)
                return false;

            // Group data by date (24-hour periods)
            var groupedByDate = dataPoints.GroupBy(d => d.DataOra.Date).ToList();

            foreach (var dateGroup in groupedByDate)
            {
                // Each 24-hour period should have up to 24 hourly data points
                int expectedHours = 24;
                int actualHours = dateGroup.Count();
                double coverage = (double)actualHours / expectedHours;

                if (coverage < DataCompletionThreshold)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
