# CVR-CC: ChilloutVR Closed Captions for movie worlds

----

## 📺 What is CVR-CC?

CVR-CC hooks into CVRVideoPlayers in [ChilloutVR](https://abinteractive.net) to provide closed captions for movies.
When a movie is loaded it reads the URL, attempts to extract the movie name, and queries the CVR-CC backend AWS service
to request an SRT for that movie. In most cases, resonably named movies will result in the SRT being returned.
The SRT is then parsed and synced to the CVRVideoPlayer, displaying the subtitles on the HUD.

## 🔧 Requirements

- [MelonLoader](https://github.com/LavaGang/MelonLoader.Installer) or [CVRMelonAssistant](https://github.com/knah/CVRMelonAssistant/) (automated install) for ChilloutVR 
    - Refer to the [CVR Modding Discord](https://discord.gg/xE7AwSrn) for installation help

## 💾 Installation

1. Copy the `CVR_CC.dll` into your ChilloutVR Mods folder (e.g., `C:\Steam\steamapps\common\ChilloutVR\Mods\CVR_CC.dll`).
2. Join a world with a video player in desktop or VR mode
3. Load a movie (paste a URL or have someone select one)

If the movie is found, you should see subtitles appear in yellow text on the bottom of your HUD.

## ❔ Troubleshooting

If subtitles fail to load, refer to your MelonLoader logs/window to see error messages. Most likely, the URL doesn't contain the movie name.
If necessary, submit an issue on Github and include all relevant log output. 

MelonLoader logs are located in your `ChilloutVR\MelonLoader\Logs` folder.

## 📈 Future Improvements

- Add support for multiple languages
- Implement UI for changing the offset for subtitles that are running fast/slow
- Implement UI for searching for a movie when the search fails
- Implement UI for toggling CC on/off and/or querying for a movie only when requested

## 🛡️ External Data

For the mod to function, it sends the URLs of CVRVideoPlayers in worlds that you join to a caching server on AWS to grab the SRTs.
The mod's request contains your IP address and the URL you're attempting to access, which go into a normal HTTP log and are periodically purged.
 
## 🦊🐕 Authors

- Foxipso
- BenacleJames

## 🧾 License

```
Copyright (c) 2022 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```