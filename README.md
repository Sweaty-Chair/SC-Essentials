# SC Essentials
[![Build status](https://ci.appveyor.com/api/projects/status/github/Sweaty-Chair/SC-Essentials?branch=main&svg=true)](https://ci.appveyor.com/project/Sweaty-Chair/Unity-Essentials/branch/main)

> The common and essentials scripts used across our projects providing code reusability and fast prototyping. This is also required by other SC modules.

## Table of Contents
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [License](#license)

## Installation
Simply import this package into Unity. You can also set this repository as a submodule and always keep this up to date. Add it in `Assets/SweatyChair/Essentials/`. Your `.gitmodules` should have these lines:
```
[submodule "Assets/SweatyChair/Essentials"]
	path = Assets/SweatyChair/Essentials
	url = https://github.com/sweatyc/Essentials.git
```

## Configuration
All settings are located at menu toolbar > Sweaty Chair > Settings.

## Usage

### External Plugins
Essentials uses the follow plugins:
- [Anti-Cheat Toolkit](https://assetstore.unity.com/packages/tools/utilities/anti-cheat-toolkit-10395)
- [Easy Save 3](https://assetstore.unity.com/packages/tools/input-management/easy-save-the-complete-save-load-asset-768)
- [Easy Mobile Pro](https://assetstore.unity.com/packages/tools/input-management/easy-save-the-complete-save-load-asset-768)
- [I2 Localization](https://assetstore.unity.com/packages/tools/localization/i2-localization-14884)
- [LeanTween](https://assetstore.unity.com/packages/tools/animation/leantween-3595)
- [Pool Manager](https://assetstore.unity.com/packages/tools/utilities/poolmanager-1010)
- [Time-cheat prevention](https://assetstore.unity.com/packages/tools/integration/time-cheat-prevention-18984)
- [Unity-UI-Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/master/)

### Modules Details
### [Analytics](Analytics)
Game analytics and tracking.
### [Currencies](Currencies)
Virtual currency system, also works with IAP and use anti-cheat.
### [Databases](Databases)
Database system that reads CSV and converts to accessible class in-game.
### [Editor](Editor)
Editor only tools, such as build tool and reference finder.
### [Gameplay](Gameplay)
Tools and utilites for common gameplay, such as bullet spawning.
### [GameSave](GameSave)
A system that saves players' progress and sync with Game Center and Play Games.
### [Helpers](Helpers)
All in-game helpers not categoized elsewhere.
### [Infos](Infos)
Player info helpers.
### [Inputs](Inputs)
Player inputs managers and utilities, such as keyboard and mobild touches.
### [Items](Items)
The common item data classes that can be used in game.
### [Localize](Localize)
Translation and localization utility.
### [Managers](Managers)
The master managers that not categorized elsewhere.
### [Server](Server)
The utility that posts and retrieves data from the backend server.
### [Settings](Settings)
Game settings helpers, such as graphics quality and sound volume.
### [Shops](Shops)
Common shop functionalities, for items selling with virual currency or real money.
### [Singleton](Singleton)
Singleton base classes.
### [States](State)
Game state functionalities. Set and fire events of current game state.
### [TweenOnEnable](TweenOnEnable)
Tweens objects using [LeanTween](https://assetstore.unity.com/packages/tools/animation/leantween-3595).
### [UI](UI)
All commonly used UI scripts and prefabs.
  - #### [BetterUI](UI/BetterUI)
    Replacing some stock Unity's UI.
  - #### [Buttons](UI/Buttons)
    Button effects and extended functionalities.
  - #### [Effects](UI/Effects)
    Common UI effets.
  - #### [InputFields](UI/InputFields)
    A in-game input field box.
  - #### [Loading](UI/Loading)
    A loading screen between each game loading.
  - #### [Messages](UI/Messages)
    A message box shown in game.
  - #### [Panels](UI/Panels)
    All commonly used panels.
  - #### [PasswordField](UI/PasswordField)
    An extended InputField for passwords.
  - #### [ProgressBars](UI/ProgressBars)
    A progress bar shows player's progress, HP, etc.
  - #### [Tabs](UI/Tabs)
    Sub panel tabs that can switch around in a panel.
  - #### [Texts](UI/Texts)
    In-game texts such as in-game notices and announcements.
  - #### [Transition](UI/Transition)
    Transition between menus, game levels, etc.
  - #### [Utilites](UI/Utilites)
    All UI tools that not categorized elsewhere.
### [Utilities](Utilities)
All tools that not categorized elsewhere.

## License
SC Essentials is licensed under a [MIT License](https://github.com/Sweaty-Chair/Unity-Essentials/blob/main/LICENSE).
