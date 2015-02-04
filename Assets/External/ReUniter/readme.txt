ReUniter is a Unity Editor extension inspired by ReSharper, the C# plugin for Visual Studio from JetBrains.

Features:
 - Navigate to Game Object (Ctrl-G or Command-G) - search as you type through entities in the Hierarchy window
 - Navigate to Asset (Ctrl-T or Command-T) - search as you type through assets in the Project window
 - Recent Items (Ctrl-E or Command-E) - quick navigation to recently selected items (regardless of whether they were selected via ReUniter or not - this keeps track of the Unity current selection as it appears in the Inspector window)
 - using the Shift key when selecting something will add it to the current selection
 - using the Control/Command key when selecting something will "execute" it (the equivalent of double clicking it or pressing Enter when it is selected) - for example: a scene will be opened, a script file will bring up the external editor
 - pressing Ctrl-A or Command-A will select all visible search results (only for navigate to asset or game object), then press Enter or Tab to select them

Popup window behavior:
 - search as you type, searched text will be highlighted in all search results (search is case insensitive)
 - results are ordered alphabetically based on the folder they're in, then by name
 - use the up/down arrow keys or the mouse to navigate up and down in the search results
 - press Enter, Tab or left click to select a search result
 - press Escape or click anywhere ouside of the search dialog to dismiss it
 - if the recent items window is open, press Ctrl-E or Command-E again to go down that list (to allow for quick one-handed switching between two different items using the Tab key)
 
The extension supports and remembers multiple selection items. Recent item selections are persistent across editor runs - they are stored in Assets/.reuniter_data so don't forget to add it to the ignore list (.gitignore, svn:ignore) if you're using version control.
 
This extension comes with source code if you find the default keyboard shorcuts conflict with other existing assets.


