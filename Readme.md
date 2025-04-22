<h1>
  <img src="https://raw.githubusercontent.com/Hypekingfish/Metar/main/assets/streamerbot-logo-text.svg" width="250" valign="middle">
</h1>

[![Release](https://img.shields.io/github/v/release/hypekingfish/Metar)](https://github.com/hypekingfish/Metar/releases) • ![Discord](https://img.shields.io/discord/858390516223311922?logo=discord&label=Discord&labelColor=RGB%3A88101242)

# ✈️ METAR Fetcher – StreamerBot Edition

Welcome to the **METAR Fetcher**, a high-utility, streamer-ready C# script built specifically for use with [**StreamerBot**](https://streamer.bot)—a free, powerful tool for automating stream interactions. This script lets you fetch and display live METAR aviation weather reports straight from chat commands like `!metar KLAX` or `!metar help`, perfect for flight sim streamers, VATSIM ATC, or aviation nerds who want to bring next-level realism to their Twitch or YouTube streams.

With detailed decoding, automatic weather category parsing, and full chat-ready formatting, this script gives you real-time weather updates directly in your stream chat or OBS overlays—without needing an API key or external account.

---

## 🌟 Key Features

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

## 🧠 Why This Script?

Most METAR fetchers either spit out the raw METAR string (which can confuse viewers) or require external services or subscriptions. This script is:
- 💯 **Completely local**: fetches directly from NOAA without an API key
- 🧩 **Modular**: customizable inside StreamerBot for overlays, alerts, etc.
- ⚡ **Instant**: lightweight and fast, ideal for mid-flight or ATC use
- 🎯 **Streamer-first**: built for engagement, chat clarity, and realism

---

## 💬 Example Usage

### Twitch Chat Command

<pre> <code>!metar ksea</code> </pre>

### Bot Response in Chat

`🟢 KSEA (VFR) | 💨 Wind: 280° at 8kt | 👁️ Vis: 10 SM | 🌡️ Temp/Dew: 18/10°C (64/50°F) | 📟 Alt: 30.12 inHg (1020 hPa) | ☁️ Sky: Broken @ 6,000ft | 🤖 AUTO Report | Obs: Apr 21 @ 13:53Z`


---

## 🔁 Flight Category Emoji Legend

| Category | Emoji | Meaning                      |
|----------|-------|------------------------------|
| VFR      | 🟢    | Visual Flight Rules (clear)  |
| MVFR     | 🔵    | Marginal VFR (light clouds)  |
| IFR      | 🟠    | Instrument rules (low viz)   |
| LIFR     | 🔴    | Very poor flying conditions  |

---

## 🛠️ How to Set Up in StreamerBot

### 🧩 Step 1: Add the C# Action
1. 🖥️ Open **StreamerBot**
2. 🛠️ Go to the **Actions** tab
3. ➕ Click **Add**, choose `Core > C# > Execute C# Code`, name it `Fetch METAR`
4. 📋 Paste the script from this repo into the editor

### 💬 Step 2: Add the Chat Command
1. 🎙️ The command is included in the SB Import for convenience


---

## 📡 Behind the Scenes: How It Works

- 📥 **Source**: Downloads METAR text file directly from: [METAR](https://tgftp.nws.noaa.gov/data/observations/metar/stations/KSEA.TXT)
- 📅 **Timestamp Handling**: Ignores METARs older than 2 hours
- 📚 **Regex Decoding**: Extracts key fields from the raw report
- ⚙️ **Formatter Logic**: Color codes, emoji flags, unit conversions
- 🧠 **Robust Parsing**: Handles AUTO reports, wind gusts, missing fields, etc.

## 💻 Developer Notes

- Language: `C#` (for use within StreamerBot)
- External Libraries: None (uses built-in .NET HTTP/Web classes)

## 📜 License

This script is open-source and released under the **MIT License**.  
Use it, modify it, stream it, remix it—just don't sell it.

## 👋 Final Thoughts

This tool is made by streamers, for streamers—whether you're flying high at FL350 or controlling a busy TRACON sector. Bring weather realism into your stream, engage your audience, and never be caught off-guard by fog again.

Fly safe. Stream strong. And enjoy clear skies! 🛫🌤️
