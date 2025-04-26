using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class CPHInline
{
	   public bool Execute()
	{
		string rawInput = args["rawInput"]?.ToString().Trim();
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            CPH.SendMessage("Usage: !metar ICAO or !metar random");
            return false;
        }

        rawInput = rawInput.ToUpper();

        if (rawInput != "RANDOM" && !Regex.IsMatch(rawInput, "^[A-Z]{4}$"))
        {
            CPH.SendMessage("❌ Invalid ICAO code. Please use a 4-letter airport code (e.g., KSEA, EGLL), or type !metar random.");
            return false;
        }


		if (rawInput.Equals("help", StringComparison.OrdinalIgnoreCase) ||
			rawInput.Equals("!help metar", StringComparison.OrdinalIgnoreCase))
		{
			CPH.SendMessage("🛫 **METAR Help** 🛬\n" +
				"Usage: `!metar ICAO` or `!randommetar`\n" +
				"Example: `!metar KSEA` or `!randommetar`\n" +
				"Returns the latest METAR including wind, visibility, clouds, altimeter, and flight rules.\n" +
				"Color Codes: 🟢 VFR | 🔵 MVFR | 🟠 IFR | 🔴 LIFR\n" +
				"Bonus: `!randommetar` adds chaos by fetching METAR from a random airport.");
			return false;
		}

		if (rawInput.Equals("!randommetar", StringComparison.OrdinalIgnoreCase) ||
			rawInput.Equals("random", StringComparison.OrdinalIgnoreCase))
		{
			var icaoList = new List<string>
			{
				// North America
				"KSEA", "KLAX", "JFK", "KSFO", "KORD", "KATL", "KDEN", "KDFW", "KMIA", "PHNL", "CYYZ", "CYVR", "CYUL", "CYWG",
				
				// Europe
				"EGLL", "EHAM", "EDDF", "LFPG", "LEMD", "LIRF", "LOWW", "LTBA", "LEBL", "EKCH", "LHRH", "UUEE", "EBBR",

				// Asia
				"RJTT", "RJAA", "VHHH", "ZBAA", "WSSS", "VTBS", "RKSI", "VIDP", "VECC", "RPLL", "ZSPD", "ZGGG", "RJCC",

				// Oceania
				"YSSY", "NZAA", "YMML", "YPPH", "NZCH", "NFFN", "NTAA",

				// South America
				"SBGR", "SCEL", "SEQM", "SKBO", "SLLP", "SPIM",

				// Africa
				"FAOR", "DNMM", "HAAB", "GMMN", "FACT", "HKJK", "DTTA",

				// Middle East
				"OMDB", "OTHH", "OKBK", "OEJN", "OIIE", "LLBG"
			};
			
			var random = new Random();
			rawInput = icaoList[random.Next(icaoList.Count)];
			CPH.SendMessage($"🎲 Weather Roulette! Pulling random METAR for **{rawInput}**...");
		}

		string station = rawInput.ToUpper();
   

        string url = $"https://tgftp.nws.noaa.gov/data/observations/metar/stations/{station}.TXT";
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string result = client.GetStringAsync(url).GetAwaiter().GetResult();
                string[] lines = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2)
                {
                    CPH.SendMessage($"No valid METAR data found for {station}.");
                    return false;
                }

                string obsTime = ParseObservationTime(lines[0].Trim());
                string metar = lines[1].Trim();
                string flightRules = GetFlightCategory(metar);
                string color = flightRules switch
                {
                    "VFR" => "🟢",
                    "MVFR" => "🔵",
                    "IFR" => "🟠",
                    "LIFR" => "🔴",
                    _ => "⚪"
                };
                string summary = BuildWeatherSummary(metar);
                string automationNote = IsAutomatedStation(metar);
                string severeWx = DetectSevereWeather(metar);

                CPH.SendMessage($"{color} **{station} ({flightRules})** | {summary}" +
                                (string.IsNullOrEmpty(severeWx) ? "" : $" | {severeWx}") +
                                $" | {automationNote} | Obs: {obsTime} | METAR: {metar}");
            }
            catch (HttpRequestException ex)
            {
                CPH.SendMessage($"HTTP error retrieving METAR for {station}: {ex.Message}");
            }
            catch (Exception ex)
            {
                CPH.SendMessage($"Unexpected error fetching METAR for {station}: {ex.Message}");
            }
        }

        return true;
    }

    private string GetFlightCategory(string metar)
    {
        var visMatch = Regex.Match(metar, @"(\d{1,2})?\s?(\d{1,2}/\d{1,2})?SM");
        if (!visMatch.Success)
            return "UNKNOWN";
        double visibilityMiles = ParseVisibility(visMatch.Value);
        var ceilingMatch = Regex.Match(metar, @"(BKN|OVC)(\d{3})");
        int ceiling = ceilingMatch.Success ? int.Parse(ceilingMatch.Groups[2].Value) * 100 : 99999;
        if (visibilityMiles >= 5 && ceiling >= 3000)
            return "VFR";
        if (visibilityMiles >= 3 && ceiling >= 1000)
            return "MVFR";
        if (visibilityMiles >= 1 && ceiling >= 500)
            return "IFR";
        return "LIFR";
    }

    private double ParseVisibility(string visStr)
    {
        visStr = visStr.Replace("SM", "").Trim();

        // Handle mixed number like "1 1/2"
        if (visStr.Contains(" "))
        {
            var parts = visStr.Split(' ');
            if (parts.Length == 2 &&
                double.TryParse(parts[0], out double whole) &&
                parts[1].Contains("/") &&
                TryParseFraction(parts[1], out double fraction))
            {
                return whole + fraction;
            }
        }

        // Handle pure fraction like "1/2"
        if (visStr.Contains("/") && TryParseFraction(visStr, out double fracOnly))
        {
            return fracOnly;
        }

        // Handle whole number only
        if (double.TryParse(visStr, out double miles))
        {
            return miles;
        }

        return 10; // fallback
    }

    private bool TryParseFraction(string input, out double result)
    {
        result = 0;
        var parts = input.Split('/');
        if (parts.Length == 2 &&
            double.TryParse(parts[0], out double num) &&
            double.TryParse(parts[1], out double den) &&
            den != 0)
        {
            result = num / den;
            return true;
        }
        return false;
    }

    private string BuildWeatherSummary(string metar)
    {
        string wind = GetWindInfo(metar);
        string temp = "🌡️ Temp: unknown";
        string alt = "📟 Altimeter: unknown";
        string sky = "☁️ Sky: unknown";
        string visibility = "👁️ Visibility: unknown";
        string wx = "";

        // Visibility
        var visMatch = Regex.Match(metar, @"(\d{1,2}(?:/\d)?|\d{1,2})SM");
        if (visMatch.Success)
        {
            double visMiles = ParseVisibility(visMatch.Value);
            visibility = $"👁️ Visibility: {visMiles:0} SM";
        }

        // Temperature and Dew Point
        var tempMatch = Regex.Match(metar, @" (\d{2}|M\d{2})/(M?\d{2})");
        if (tempMatch.Success)
        {
            string t = tempMatch.Groups[1].Value.Replace("M", "-");
            string d = tempMatch.Groups[2].Value.Replace("M", "-");
            temp = $"🌡️ Temp/Dew: {t}/{d}°C ({ConvertCtoF(t)}/{ConvertCtoF(d)})";
        }

        // Altimeter
        var altMatch = Regex.Match(metar, @"A(\d{4})");
		if (altMatch.Success && int.TryParse(altMatch.Groups[1].Value, out int altRaw))
		{
			double altInHg = altRaw / 100.0;
			double qnhHpa = altInHg * 33.8639;
			alt = $"📟 Altimeter: {altInHg:0.00} inHg (QNH {Math.Round(qnhHpa)})";
		}

        // Sky conditions
        var skyMatches = Regex.Matches(metar, @"(FEW|SCT|BKN|OVC)(\d{3})");
        if (skyMatches.Count > 0)
        {
            var conditions = new List<string>();
            foreach (Match match in skyMatches)
            {
                string cover = match.Groups[1].Value;
                string level = (int.Parse(match.Groups[2].Value) * 100).ToString();
                conditions.Add($"{cover} at {level}ft");
            }

            sky = $"☁️ Sky: {string.Join(", ", conditions)}";
        }

        // Weather conditions
        if (!metar.Contains("VISNO"))
        {
            var wxMatch = Regex.Match(metar, @"\s(-|\+)?(RA|SN|TS|BR|FG|HZ|DZ|SH|VC)?(RA|SN|TS|BR|FG|HZ|DZ|SH|VC)?\s");
            if (wxMatch.Success)
            {
                wx = $"🌧️ Wx: {wxMatch.Value.Trim()}";
            }
        }
        else
        {
            wx = "🌧️ Wx: None (VISNO present)";
        }

        return $"{wind} | {visibility} | {temp} | {alt} | {sky}" + (string.IsNullOrEmpty(wx) ? "" : $" | {wx}");
    }

		private string GetWindInfo(string metar)
    {
        var windMatch = Regex.Match(metar, @"\b(\d{3}|VRB)(\d{2,3})(G\d{2,3})?KT\b");
        if (windMatch.Success)
        {
            string dir = windMatch.Groups[1].Value;
            string spd = windMatch.Groups[2].Value;
            string gust = windMatch.Groups[3].Value;

            if (dir == "000" && spd == "00")
                return "💨 Winds calm";

            if (dir == "VRB")
                return $"💨 Variable wind at {spd}kt" + (string.IsNullOrEmpty(gust) ? "" : $" gusting {gust.Substring(1)}kt");

            return $"💨 Wind: {dir}° at {spd}kt" + (string.IsNullOrEmpty(gust) ? "" : $" gusting {gust.Substring(1)}kt");
        }

        return "💨 Wind: Not reported";
    }
    

    private string ConvertCtoF(string celsiusStr)
    {
        if (double.TryParse(celsiusStr, out double c))
        {
            double f = c * 9 / 5 + 32;
            return $"{f:0}°F";
        }
        return "";
    }

    private string ParseObservationTime(string line)
    {
        if (DateTime.TryParse(line, out var obsTime))
        {
            return obsTime.ToString("MMM dd, HH:mm 'UTC'");
        }
        return line;
    }

    private string IsAutomatedStation(string metar)
    {
        string upperMetar = metar.ToUpperInvariant();

        if (upperMetar.Contains("AO2"))
            return upperMetar.Contains("RMK") ? "Automated (AO2) with remarks" : "Automated (AO2)";
        else if (upperMetar.Contains("AO1"))
            return upperMetar.Contains("RMK") ? "Automated (AO1) with remarks" : "Automated (AO1)";
        else if (upperMetar.Contains("AUTO"))
            return "Fully automated (no human observer)";
        else if (upperMetar.Contains("COR"))
            return "Corrected observation (may be manual)";
        else
            return "Manual or Unknown";
    }



    private string DetectSevereWeather(string metar)
    {
        var alerts = new List<string>();

        // Thunderstorms & convective weather
        if (metar.Contains("+TSRA")) alerts.Add("Heavy thunderstorm with rain");
        else if (metar.Contains("TSRA")) alerts.Add("Thunderstorm with rain");
        else if (metar.Contains("TS")) alerts.Add("Thunderstorm");

        // Rain
        if (metar.Contains("+RA")) alerts.Add("Heavy rain");
        else if (metar.Contains("-RA")) alerts.Add("Light rain");
        else if (metar.Contains("RA")) alerts.Add("Rain");

        // Drizzle
        if (metar.Contains("+DZ")) alerts.Add("Heavy drizzle");
        else if (metar.Contains("-DZ")) alerts.Add("Light drizzle");
        else if (metar.Contains("DZ")) alerts.Add("Drizzle");

        // Freezing rain/drizzle
        if (metar.Contains("FZRA")) alerts.Add("Freezing rain");
        if (metar.Contains("FZDZ")) alerts.Add("Freezing drizzle");

        // Snow & wintry mix
        if (metar.Contains("+SN")) alerts.Add("Heavy snow");
        else if (metar.Contains("-SN")) alerts.Add("Light snow");
        else if (metar.Contains("SN") && metar.Contains("GS")) alerts.Add("Snow");

        if (metar.Contains("SG")) alerts.Add("Snow grains");
        if (metar.Contains("GS")) alerts.Add("small hail");
        if (metar.Contains("GR")) alerts.Add("Hail");

        // Ice crystals or ice pellets
        if (metar.Contains("IC")) alerts.Add("Ice crystals");
        if (metar.Contains("PL")) alerts.Add("Ice pellets");

        // Fog, mist, haze
        if (metar.Contains("FG")) alerts.Add("Fog");
        if (metar.Contains("BR")) alerts.Add("Mist");
        if (metar.Contains("HZ")) alerts.Add("Haze");

        // Obscurations and airborne particles
        if (metar.Contains("FU") && metar.Contains("DSNT")) alerts.Add("Smoke in the distance");
        else if (metar.Contains("FU")) alerts.Add("Smoke");

        if (metar.Contains("DU")) alerts.Add("Widespread dust");
        if (metar.Contains("SA")) alerts.Add("Sand");
        if (metar.Contains("VA")) alerts.Add("Volcanic ash");

        // Blowing phenomena
        if (metar.Contains("BLSN")) alerts.Add("Blowing snow");
        if (metar.Contains("BLDU")) alerts.Add("Blowing dust");
        if (metar.Contains("BLSA")) alerts.Add("Blowing sand");

        // Vicinity phenomena
        if (metar.Contains("VCSH")) alerts.Add("Showers in the vicinity");
        if (metar.Contains("VCFG")) alerts.Add("Fog in the vicinity");
        if (metar.Contains("VCTS")) alerts.Add("Thunderstorms in the vicinity");

        // Severe phenomena
        if (metar.Contains("+FC")) alerts.Add("Tornado or waterspout");
        else if (metar.Contains("FC")) alerts.Add("Funnel cloud");

        // Wind & visibility
        if (metar.Contains("G") && metar.Contains("KT")) alerts.Add("Gusty winds");
        if (metar.Contains("SQ")) alerts.Add("Squalls");
        if (metar.Contains("VIS") && metar.Contains("NO")) alerts.Add("Visibility data missing");

        // Maintenance & pressure
        if (metar.Contains("$")) alerts.Add("Maintenance indicator active");
        if (metar.Contains("SLPNO")) alerts.Add("Sea-level pressure not available");

        return alerts.Count > 0 ? $"⚠️ {string.Join(", ", alerts)}" : "";
    }
}
