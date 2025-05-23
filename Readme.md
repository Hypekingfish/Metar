<!-- Keywords: METAR, StreamerBot, C# Script, Twitch Weather Bot, VATSIM Tools, Aviation Weather, OBS Overlay, Real-time METAR -->

<h1>
  <img src="https://raw.githubusercontent.com/Hypekingfish/Metar/main/assets/streamerbot-logo-text.svg" width="250" valign="middle" alt="StreamerBot Logo">
</h1>

![Built with C#](https://img.shields.io/badge/Built%20with-C%23-blue) • [![Release](https://img.shields.io/github/v/release/hypekingfish/Metar)](https://github.com/hypekingfish/Metar/releases) • [![Discord](https://img.shields.io/discord/858390516223311922?logo=discord&label=Discord&labelColor=RGB%3A88101242)](https://discord.gg/YOUR-SERVER-ID) • ![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/Hypekingfish/Metar?utm_source=oss&utm_medium=github&utm_campaign=Hypekingfish%2FMetar&labelColor=171717&color=FF570A&link=https%3A%2F%2Fcoderabbit.ai&label=CodeRabbit+Reviews) • ![GitHub License](https://img.shields.io/github/license/Hypekingfish/Metar) • ![StreamerBot Compatible](https://img.shields.io/badge/StreamerBot-Compatible-brightgreen)
![No API Key Needed](https://img.shields.io/badge/No%20API%20Key-Required-blueviolet)

## Table of Contents

- [Overview](#metar-fetcher--streamerbot-edition)
- [Key Features](#key-features)
- [Example Usage](#example-usage)
- [Flight Category Emoji Legend](#flight-category-emoji-legend)
- [Setup Instructions](#how-to-set-up-in-streamerbot)
- [How It Works](#behind-the-scenes-how-it-works)
- [Developer Notes](#developer-notes)
- [License](#license)
- [Credits](#Credits)
- [Final Thoughts](#final-thoughts)

---

## Quick Start

1. Open StreamerBot > Actions tab > Add new C# action
2. Paste the script from this repo
3. Link it to a Twitch command (e.g., `!metar`)
4. Try it out in chat: `!metar KLAX`

---

# METAR Fetcher – StreamerBot Edition

Welcome to the **METAR Fetcher**, a high-utility, streamer-ready C# script built specifically for use with [**StreamerBot**](https://streamer.bot)—a free, powerful tool for automating stream interactions. This script lets you fetch and display live METAR aviation weather reports straight from chat commands like `!metar KLAX` or `!metar help`, perfect for flight sim streamers, VATSIM ATC, or aviation nerds who want to bring next-level realism to their Twitch or YouTube streams.

With detailed decoding, automatic weather category parsing, and full chat-ready formatting, this script gives you real-time weather updates directly in your stream chat or OBS overlays—without needing an API key or external account.

---

## Key Features

- **Real-time METAR Fetching**  
  Retrieve the most recent METAR weather report from any ICAO-coded airport (e.g., `KSEA`, `EGLL`, `RJTT`, etc.).

- **Human-Readable Weather Summaries**  
  Transforms raw METAR data into a clean, informative summary:

  - Wind (direction, speed, gusts)
  - Visibility in SM
  - Temperature & Dew Point (°C and °F)
  - Altimeter pressure (inHg and hPa)
  - Sky conditions (broken, scattered, overcast clouds with altitudes)
  - Weather symbols (rain, fog, snow, etc.)
  - Flight Category: VFR, MVFR, IFR, LIFR with emoji status indicators
  - Observation source (human vs. AUTO)

- **Severe Weather Detection**  
  Alerts for intense conditions such as:

  - Thunderstorms (`+TSRA`)
  - Heavy snow/rain (`+SN`, `+RA`)
  - Fog (`FG`, `BR`)
  - Low ceilings and poor visibility

- **Fully StreamerBot-Compatible**
  - Trigger via Twitch Chat Commands (`!metar KLAX`)
  - Fully offline-capable—no extra installs or libraries required

---

## Why This Script?

Most METAR fetchers either spit out the raw METAR string (which can confuse viewers) or require external services or subscriptions. This script is:

- **Completely local**: fetches directly from NOAA without an API key
- **Modular**: customizable inside StreamerBot for overlays, alerts, etc.
- **Instant**: lightweight and fast, ideal for mid-flight or ATC use
- **Streamer-first**: built for engagement, chat clarity, and realism

---

## Example Usage

![Example METAR output showing weather details for KSEA](https://raw.githubusercontent.com/Hypekingfish/Metar/main/assets/Metar-Example.png)

### Twitch Chat Command

```bash
!metar ksea
```

### Bot Response in Chat

` 🟢 KSEA (VFR) | 💨 Wind: 010° at 5kt | 👁️ Vis: 10 SM | 🌡️ Temp/Dew: 10/04°C (50/39°F) | 📟 Alt: 30.24 inHg (1024 hPa) | ☁️ Sky: Scattered @ 2,000ft | 👨‍✈️ Human Observer | Obs: Apr 22 @ 16:53Z | METAR: KSEA 221653Z 01005KT 10SM SCT020 10/04 A3024 RMK AO2 SLP249 T01000039`

---

## Flight Category Emoji Legend

| Category | Emoji | Meaning                     |
| -------- | ----- | --------------------------- |
| VFR      | 🟢    | Visual Flight Rules (clear) |
| MVFR     | 🔵    | Marginal VFR (light clouds) |
| IFR      | 🟠    | Instrument rules (low viz)  |
| LIFR     | 🔴    | Very poor flying conditions |

---

## How to Set Up in StreamerBot

### Step 1: Add the C# Action

1. Open **StreamerBot**
2. Go to the **Actions** tab
3. Click **Add**, choose `Core > C# > Execute C# Code`, name it `Fetch METAR`
4. Paste the script from this repo into the editor

### Command

1. Use the provided `SB Import` to add the command automatically
2. Or manually link the action to a chat command like `!metar`

## Troubleshooting tips

| Issue              | Solution                                                |
| ------------------ | ------------------------------------------------------- |
| No chat response   | Ensure the action is correctly linked to a command      |
| Invalid ICAO code  | Confirm it's a real ICAO airport (e.g., `KSEA`, `EGLL`) |
| Script not running | Check for syntax errors or StreamerBot action settings  |

---

## Command list

| Command         | Description                    |
| --------------- | ------------------------------ |
| `!metar <ICAO>` | Fetch METAR for given airport  |
| `!metar help`   | Show help and legend info      |
| `!metar random` | Pull a random ICAO from a list |

---

## Frequently Asked Questions

| **Question**                                            | **Answer**                                                                                                                                                                                                                      |
| ------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Does this script work for all airports?                 | Yes, as long as the airport has a valid METAR report, the script can fetch weather data for any ICAO airport code (e.g., `KSEA`, `EGLL`, `RJTT`). If the airport does not have a METAR report, the script will not return data. |
| How do I update the script or fetch a new version?      | To update the script, you can visit the [GitHub repository](https://github.com/Hypekingfish/Metar) for the latest release and simply replace the script in StreamerBot with the updated version.                                |
| Can I use this script outside of Twitch?                | Currently, the script is designed specifically for Twitch using StreamerBot. However, it may be adaptable for other platforms that support custom bots or commands with some modifications.                                     |
| How do I get METAR data for airports outside of the US? | METAR data is available globally, so the script will work for airports worldwide as long as they have active METAR reports (using ICAO codes like `EGLL`, `RJTT`, etc.).                                                        |

---

## Links to Related Resources

- [VATSIM](https://www.vatsim.net/): Join a network of live air traffic controllers and pilots to enhance your flight sim experience.
- [Aviation Weather Center](https://www.weather.gov/aviation/): For more detailed weather information and forecasts.
- [NOAA METAR Feed](https://www.weather.gov/): Official METAR data source for US-based airports.

---

## Contributing

Want to contribute to the project? Here’s how you can help:

- **Bug Reports:** Open issues for bugs or any problems you encounter.
- **Feature Requests:** Suggest new features or improvements for future releases.
- **Code Contributions:** Fork the repo and submit a pull request with your changes. Contributions are always welcome!

---

## Behind the Scenes: How It Works

- **Source**: Downloads METAR text file directly from: [METAR](https://tgftp.nws.noaa.gov/data/observations/metar/stations/KSEA.TXT)
- **Timestamp Handling**: Ignores METARs older than 2 hours
- **Regex Decoding**: Extracts key fields from the raw report
- **Formatter Logic**: Color codes, emoji flags, unit conversions
- **Robust Parsing**: Handles AUTO reports, wind gusts, missing fields, etc.

## Developer Notes

- Language: `C#` (for use within StreamerBot)
- External Libraries: None (uses only built-in .NET classes like HttpClient and Regex)

## License

This script is open-source and released under the **GPL License**.  
Use it, modify it, stream it, remix it—just don't sell it.

## Credits

- Script developed by [@Hypekingfish](https://github.com/Hypekingfish)

## Final Thoughts

This tool is made by streamers, for streamers—whether you're flying high at FL350 or controlling a busy TRACON sector. Bring weather realism into your stream, engage your audience, and never be caught off-guard by fog again.

Fly safe. Stream strong. And enjoy clear skies! 🛫🌤️
