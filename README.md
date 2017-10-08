# DTLocalization

Localization framework that comes with adaptor for GDatabase backend, bundled caching, runtime caching, and more.

![Cycling through Localized Languages](./Images/LocalizationGif.gif)
(Cycling through some localized languages)

## Features
#### Caching
DTLocalization supports caching your localization data as part of the build (bundled). You can cache it manually in the editor or integrate it as a step in your build pipeline (recommended).

It also supports downloading the localization data during runtime and also caching that data to prevent repeated requests within a period of time.

#### Google Translate Integration
Add Google service account credentials to your project and DTLocalization will automatically populate new keys with a placeholder translations through the Google Translate API!

Placeholder translations are marked as needsUpdating in the localization table and can be sent to external translators for final translation.

#### TextMeshPro Integration
DTLocalization comes with support for TextMeshPro, including a workflow for downloading all used characters to be built into the font atlas and also automatically downgrading the TextMeshPro text into Unity text if characters are missing from the atlas.

#### Editor Workflow
In combination with DTCommandPalette, DTLocalization comes with handy commands for searching through localization keys, adding new localization keys, and more. All these commands can be run right inside the Unity editor!

#### Database Support
Currently DTLocalization supports storing localization data inside Google spreadsheets through the GDataDB API, but can be easily extended to support other types of backends.


## To Install & Use:
1. Download the DTLocalization project from this repository by pressing [this link](https://github.com/DarrenTsung/DTLocalization/archive/master.zip). It should automatically download the latest state of the master branch.
2. Place the downloaded folder in your project. I recommend placing it in the Assets/Plugins directory so [it doesn’t add to your compile time](https://medium.com/@darrentsung/the-clocks-ticking-how-to-optimize-compile-time-in-unity-45d1f200572b). 

#### Connecting Google Spreadsheet
1. Follow the steps to setup a Google developer account and create a service account address and p12 key [here](https://github.com/DarrenTsung/GDataDB).
2. Create a spreadsheet named 'Localization Database' in Google Drive with two sheets: we'll name them 'Localization' and 'Localization Master'.
3. 'Localization' should have the columns 'Key | Language Code | LocalizedText | NeedsUpdating'. See example [here](TKlink).
4. 'Localization Master' should have the columns 'Key | Context'. See example [here](TKlink).
5. Share the spreadsheet with read-only access to the service account address (looks like xxxx.gserviceaccount.com).

(Back in Unity)
6. Create a GDatabaseSource asset in your project by right-clicking in the project window and going to Create -> DTLocalization -> GDatabaseSource.
7. Setup your newly created GDatabaseSource with a table key, a unique key used to identify this localization table.
8. Set the Service Account Address\_ under the OAuth2 properties to the service account with read-only access. Link the p12 file renamed with .bytes extension as the Private Key P12 Asset\_.
9. Create another service account, this time set it up with edit access (read-write). Set this under the Editor Service Account Address_ so editor tools can write to the localization table.

#### Setting Language Localization Preferences
1. (In Unity) Create an EditorLocalizationConfiguration asset in your project by right-clicking, Create -> DTLocalization -> EditorLocalizationConfiguration.
2. Set your master language using the language codes from [this page](https://msdn.microsoft.com/en-us/library/ee825488%28v=cs.20%29.aspx).
3. Set which languages to localize to using the language codes from [this page](https://msdn.microsoft.com/en-us/library/ee825488%28v=cs.20%29.aspx).
4. These values are just used for the editor tools and have no restriction on what types of languages can be localized in the database.

#### Setup Google Translate API (optional)
1. (In Unity) Create an GoogleTranslateSource asset in your project by right-clicking, Create -> DTLocalization -> GoogleTranslateSource.
2. Setup the Google Translate Source with a service account + enable Google Translate API access for your account.

#### Create First Localization Key
1. [Add DTCommandPalette to your project](https://github.com/DarrenTsung/DTCommandPalette) (along with DT_COMMAND_PALETTE compile define) to access editor commands.
2. Press Cmd-shift-M to open command palette.
3. Type in "Add Localization Key".
4. Fill out localization key along with initial translation.
5. Now you should have the localized key and translated values in the spreadsheet!

#### Localize Your Text
1. Add a LocalizedText component to your text component and link a UI Text component to it.
2. Set the localization key.
3. Press play!
4. Check out the different languages localized by pressing cmd-shift-M and running the "EnableLocalizationAutoSwitch" command.
