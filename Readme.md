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
  - Return output directly to Twitch, OBS overlays, TTS, or Discord
  - Flexible variable return for chaining with other StreamerBot actions
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

`!metar ksea`

### Bot Response in Chat

`🟢 KSEA (VFR) | 💨 Wind: 280° at 8kt | 👁️ Vis: 10 SM | 🌡️ Temp/Dew: 18/10°C (64/50°F) | 📟 Alt: 30.12 inHg (1020 hPa) | ☁️ Sky: Broken @ 6,000ft | 🤖 AUTO Report | Obs: Apr 21 @ 13:53Z`