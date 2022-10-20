# Simple Beatmap Play Count

Simple mod to keep track of how many times you've played a song.

![Preview](assets/menu-leveldetail-playcount.png)

The count is incremented after playing 70% through a song by default, but this can be configured along other values through an in-game UI:

![Mod settings](assets/mod-settings.png)

### TODO

- Enable during multiplayer
- Add "Unplayed" and "Played" filters to song select
- Add sorting by play count to song select

## Installation

1. Make sure your Beat Saber installation [has mod support](https://bsmg.wiki/pc-modding.html) (e.g. setup via ModAssistant)
1. Download the latest release for your Beat Saber version from [the Releases link on the right](releases/).
1. Drop the downloaded .dll into the `(Game directory)\Plugins` directory

## Data location

Play counts are stored in `(Game directory)\UserData\PlayCounts\(Level ID).count`.

Mod configuration is stored in `(Game directory)\UserData\Simple Beatmap Play Count.json`.

## About mod assets

The [Resources/Bundle.bundle](./BeatmapPlayCount/Resources/Bundle.bundle) file is generated via [a separate Unity project](https://github.com/netux/BeatSaber-BeatmapPlayCount-AssetBundler).

This file is embedded into each release .dll, so there is no need to download it separately.
