# Wiki workspace
A Visual Studio Code workspace made for more easily editing wiki pages

## How to setup
Step 1. Install [Python 3](https://www.python.org/downloads). The version I use specifically is 3.10, but the specific version shouldn't matter all that much.
Step 2. Install [Visual Studio Code](https://code.visualstudio.com)
Step 3. Install the [Terminals Manager](https://marketplace.visualstudio.com/items?itemName=fabiospampinato.vscode-terminals) extension  
Step 4. Clone this repository and delete the README.md file  
Step 5. Open up the Command Palette (Ctrl+P) and run the command `>Terminals: Run` (this will have to be repeated the terminal closes for whatever reason)

## Notes
This workspace is made specifically for my use case and is here purely in case others may find it interesting or helpful. For this reason, certain functionality some people may want may not be here and possibly never will be. I made and am expanding this tool according to my needs and mine alone, but you are free to try and add functionality of your own.

This workspace only works on Windows, due to relying on several executables I only compiled for it.

This workspace is designed to work on FANDOM wikis specifically and will require editing of the python scripts to work on wikis on other domains (so long as said wikis are using MediaWiki and allow for bot editing, this should be possible).

This tool also comes with another program called WikiWatcher, which will periodically scan the pages you have locally saved for changes and redownload them. There is no way to disable this functionality, though you can close it from the system tray once it starts.

## Changelog
1.0: Initial version
1.2: Preprocessor update
- Preprocessor spec now passes pagedata in addition to wikitext to the proprocessor
- Added support for multiple preprocessors per-page
- Added tptw command to preview the wikitext that will get sent to the wiki after running through the preprocessors
- Added pdedit command to edit pagedata for a page
- WikiTerminal now remembers the last open directory and will start in that directory when starting
- Fixed a potential rash with the get command