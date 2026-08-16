# Preference Arena — Human Preference Collection Tool

A small ASP.NET Core web app for collecting human preference labels on pairs of
Pendulum trajectory clips. This is the human-labeling frontend for the
[preference-based RL project](https://github.com/fatimalikova/preference-based-rl),
which uses these labels to train a Bradley-Terry reward model — a simplified,
self-contained version of the preference-learning loop behind **PEBBLE** (Lee et al.,
2021) and RLHF more broadly.

## What it does

- Loads trajectory clips (angle sequences) exported from the Python RL project
- Shows two animated pendulum clips side by side, each with a live sparkline of its
  angle trajectory
- Lets a human pick which one looks better, via button click or keyboard shortcut (A/B)
- Writes each choice to `Data/human_preferences.json`, in a format the Python project
  reads directly to train a reward model

Deliberately, the frontend does **not** show the clip's true reward or source
(trained/random policy) — the human judgment has to be made blind, the same way real
RLHF labeling works.

## Why a separate web app

Preference labeling needs to happen many times, quickly, so a simple always-available
web UI is more practical than re-running a Python script for every comparison. Keeping
it as a separate ASP.NET Core project also mirrors how these systems are built in
practice: a lightweight labeling frontend, decoupled from the ML training code.

## Running it

Requires the [.NET SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

Then open the URL shown in the terminal (e.g. `http://localhost:5000`). Make sure
`Data/clips.json` exists first — it's exported by `extract_clips.py` in the
[Python project](https://github.com/fatimalikova/preference-based-rl).

## Result

Using this tool, human labels were collected and compared against the environment's
true reward. The comparison surfaced a systematic labeling bias — see the
[**Finding: a systematic human labeling bias**](https://github.com/fatimalikova/preference-based-rl#finding-a-systematic-human-labeling-bias)
section of the main project for details.

## Structure

```
PreferenceWebApp/
├── Program.cs          # API endpoints: /api/pair, /api/preference
├── Data/
│   ├── clips.json               # input, exported from the Python project
│   └── human_preferences.json   # output, generated as labels are collected
└── wwwroot/
    └── index.html       # labeling UI
```
