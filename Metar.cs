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
        if (visStr.Contains("/"))
        {
            var parts = visStr.Replace("SM", "").Split('/');
            if (parts.Length == 2 && double.TryParse(parts[0], out double numerator) && double.TryParse(parts[1], out double denominator))
            {
                return numerator / denominator;
            }
        }
        else if (double.TryParse(visStr.Replace("SM", ""), out double miles))
        {
            return miles;
        }

        return 10;
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

			// Special wind conditions
			if (dir == "000" && spd == "00")
				return "💨 Calm winds";

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
        return metar.Contains("AUTO") ? "🤖 Automated Report" : "👨‍✈️ Human Observer";
    }

    private string DetectSevereWeather(string metar)
    {
        var alerts = new List<string>();
        if (metar.Contains("+TSRA")) alerts.Add("⛈️ Heavy Thunderstorm");
        if (metar.Contains("FG")) alerts.Add("🌫️ Fog");
        // if (metar.Contains("SN")) alerts.Add("❄️ Snow");
        if (metar.Contains("+RA")) alerts.Add("🌧️ Heavy Rain");
        if (metar.Contains("-RA")) alerts.Add("☔ Light Rain");
        if (metar.Contains("$")) alerts.Add("Maintenance Required");
        if (metar.Contains("VCSH")) alerts.Add("Showers in Area");
        return alerts.Count > 0 ? $"⚠️ {string.Join(", ", alerts)}" : "";
    }
}
