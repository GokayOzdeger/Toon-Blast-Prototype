# Save System
Saving GameData in Json File
The variables that need to be saved must be defined in the GameData class.

## Data Persistence Manager
A singleton. When the game is opened, it loads the saved data and when the game is closed, it saves the data.

## IDataPersistence
    SaveGame(), LoadGame()
A interface for saving and loading data.


