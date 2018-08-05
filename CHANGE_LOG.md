# Action Groups Extended :: Change Log

* 2014-1129: 1.24.1 (Diazo) for KSP 0.24.2
	+ NOT KSP 0.90 COMPATIBLE
	+ Force activate the engine part so that when the Activate Engine or Toggle Engine is activated, the Gimbal attaced to that engine also activates.
	+ Full explanation in forum thread.
* 2014-1129: 1.24 (Diazo) for KSP 0.24.2
	+ Add cooldown to action group activation to work around KSP passing a key press twice.
	+ Move toggle state monitoring from Update() to FixedUpdate()
* 2014-xxyy: 1.23.4 (Diazo) for KSP 0.24.2
	+ Add expanded logging to track down an error.
	+ (No binary found)
* 2014-1128: 1.23.3 (Diazo) for KSP 0.24.2
	+ Fix AGX Ambiguous and Toggle State monitoring issues
* 2014-1122: 1.23.1 (Diazo) for KSP 0.24.2
	+ Fix AGX Ambiguous and Toggle State monitoring issues
* 2014-1122: 1.23 (Diazo) for KSP 0.24.2
	+ Add modifier key support
* 2014-1120: 1.22.2 (Diazo) for KSP 0.24.2
	+ Add SCANsat (Karbonite) fix for AGX Ambiguous action.
	+ Initial support for modifier keys (Alt-1) to activate actions. Not yet working in flight, do not use.
	+ (I rushed this release out with the modifier keys half finished to get karbonite working.)
* 2014-1117: 1.22.1 (Diazo) for KSP 0.24.2
	+ Add in-game button to right-click button on toolbar to reset window locations when they teleport off screen.
* 2014-1116: 1.22 (Diazo) for KSP 0.24.2
	+ ExtraPlanatary Launchpads compatibility.
	+ Tweak Docking Code for weaknesses reveale''''d
* 2014-1109: 1.21.1 (Diazo) for KSP 0.24.2
	+ Fix ModuleAnimateGeneric to remove some Action Ambiguous messages.
	+ Fix showing of Part Cross when not in Action Editing mode
	+ Move Edit In Flight button to the toolbar due to space issues (Right-Click Toolbar, Edit button).
	+ Add Keyset selection button to Flight window the same as the Group Visibility button.
	+ Overhaul externally exposed code for other mods to interface (this change will not be visible to the average player.)
* 2014-1031: 1.20.1 (Diazo) for KSP 0.24.2
	+ kOS inter-op test version, not for public release
* 2014-1030: 1.20 (Diazo) for KSP 0.24.2
	+ Add Part Location crosses (blue plus, green cross, red large cross)
	+ Add editing of non-numeric actions to editor (Brakes/Lights/etc)
	+ Add RCS/SAS/Stage action groups to non-numeric editing.
* 2014-1018: 1.19 (Diazo) for KSP 0.24.2
	+ Change from Vessel.ID to Part.flightID
	+ Add ability to override KSP default action groups lockout
	+ note: This is the correct version 1.19. The previous version 1.19 is a typo, it should be version 1.9 
* 2014-1004: 1.18 (Diazo) for KSP 0.24.2
	+ Add USI Survivability Pack Support
	+ Compile 1.17 fixes for official release
	+ Notable changes:
		- Add undo functionality
		- Fix deletion/creating of same named vessels compatibiliy
* 2014-xxyy: 1.17.1 (Diazo) for KSP 0.24.2
	+ Version 1.17a through 1.17f
	+ Add Near Future and RemoteTech support (remove Action Ambiguous message).
	+ Tweak ModuleScience support.
	+ Fix handling of "Untitled Space Craft" in Editor
	+ Add support for Undo function in editor
	+ Toggle state of Action groups on vessel object for other mods to monitor (kOS requested this.)
	+ Add Better Then Starting Manned support
	+ Reduce aggressiveness of Editor auto-save so it no longer saves invalid data
	+ Add ability to skip cleanup of old save files.
* 2014-0923: 1.17 (Diazo) for KSP 0.24.2
	+ Expand saving in editor, now when mods offer a non-standard way to leave the editor AGX will save correctly.
	+ Note that this means there is no way to leave the editor without saving, even an Alt-F4 quit will save now, although it saves the AGExtEditor.cfg file to a non-standard location.
	+ Add BTSM mod support.
	+ Add on-screen warning when an action loads incorrectly. See note above.
* 2014-0920: 1.16.2 (Diazo) for KSP 0.24.2
	+ Fix show/hide of AGExt in flight mode
	+ Rework action-part saving to improve launching of new vessels
* 2014-0919: 1.16 (Diazo) for KSP 0.24.2
	+ Refactor Save/Load code in Flight Scene so it will save/load correctly.
	+ This fixes the Timewarp issue as well.
	+ Include CCraigen's tweak to make how the mod handles old files better.
* 2014-0914: 1.15.1 (Diazo) for KSP 0.24.2
	+ Timewarp fix so data save/loads correctly.
* 2014-0912: 1.15 (Diazo) for KSP 0.24.2
	+ Fix loading of actions for Spaceplanes when a Spaceplane and a Rocket have the same name.
	+ Add ability to edit actions on Abort/Brake/Gear/Light action groups in flight mode. (Editor still uses KSP default editor for these groups.)
	+ Start adding code to support vessels remembering settings after dock/undock. This does not work yet however.
* 2014-0906: 1.14 (Diazo) for KSP 0.24.2
	+ Upgrading from Version 1.12 will lose all data, except for the Action assignments themselves on groups 1-10
	+ Vessels do not save KeySet or Group visibility or names during dock/undock (see comment below)
	+ Add ability to select parts from list rather then clicking on the vessel.
	+ Mod now cleans up after itself and deletes old AGExt00000.cfg files automatically.
* 2014-0902: 1.13.6 (Diazo) for KSP 0.24.2
	+ Add icon showing related part when mouse-over a part in the selected parts window or the current actions window.
	+ Is a black and yellow square and it shows through other parts.
* 2014-0901: 1.13.5 (Diazo) for KSP 0.24.2
	+ Fix group name behavior on docking/undocking
	+ download pulled as 1.13f superceeds is with bugfixes
* 2014-0901: 1.13.4 (Diazo) for KSP 0.24.2
	+ Bugfixing of 1.13 continues
		- Fix loading of base KSP actions so existing actions import when mod is installed
		- Fix spaceplane coordinate issues so the mod works in the SPH
	+ edit: Download pulled as 1.13e has important bugfixes
* 2014-0831: 1.13.2 (Diazo) for KSP 0.24.2
	+ Fix Key Binds so they load correctly and the keyboard can be used to activate action groups.
	+ Read in action groups from the base 10 KSP groups correctly.
	+ edit: Pulled file, had some major bugs
* 2014-0831: 1.13.1 (Diazo) for KSP 0.24.2
	+ Hotfix to delete the clicked action in flight mode rather then a random action.
	+ edit: download pulled, key bindings do not load correctly in this version
* 2014-0830: 1.13 (Diazo) for KSP 0.24.2
	+ Beta test of new save/load routines that no longer use partModules
	+ remove all references to partModules, so ModuleManager is no longer needed, included deletion of part.cfg file from the mod directly
		- Therefore ModuleManager is no longer required and the part.cfg file is no longer part of the download. (See note in download post before deleting this file however.)
	+ no UI changes this version, should work the same as Version 1.12'
	+ edit: Download pulled, critical bug present.
* 2014-0728: 1.12 (Diazo) for KSP 0.24.2
	+ KSP 0.24.2 recompile.
	+ Add ability to show/hide the Keycodes in Flight. To do so, right-click on the AGX button on the toolbar and use the button that shows to change this setting.
* 2014-0724: 1.11 (Diazo) for KSP 0.24.2
	+ KSP 0.24.1 Compatibility confirmation.
	+ DMagic Orbital Science fix
	+ Roll up fixes from several test releases including several minor bugfixes.
	+ Add more detailed logging to try and track down a case where the actions do not save. (Hopefully fixed by KSP 0.24.1, but left in just in case)
* 2014-0721: 1.10 (Diazo) for KSP 0.24.2
	+ Further KSP 0.24 tweaks
	+ Lock down GUI to prevent bleed over from other mods
	+ Add detailed logging to try and track down a case of the data not saving
	+ Tighten up UI so bleed over from other mods can not mess up my GUI window layouts
	+ Add detailed logging to try and track down a case where the actions do not save.
* 2014-0718: 1.9 (Diazo) for KSP 0.24.2
	+ Compile for KSP 0.24 64-bit
	+ Update download to include ModuleManager 2.2.0
	+ Reduce checking of toggle states from 60 to 3 times a second.
	+ Add ability to override custom action groups lockout
	+ Add Interstellar, TAC Life Support to toggle state monitoring.
	+ Procedural Fairings and Kerbal Attachment system checked and have no actions to monitor.
	+ Throttle toggle state checking from 60 times a second to 3 times a second.
* 2014-xxyy: 1.8.2 (Diazo) for KSP 0.23.5
	+ Full support for Firespitter and add FAR, and Infernal Robotics toggle state monitor.
	+ Tweak ReactionWheel to only show off when disabled by player, not lack of resources
* 2014-0713: 1.8.1 (Diazo) for KSP 0.23.5
	+ Add Kethane, ScanSAT and started on Firespitter Toggle state monitoring.
* 2014-xxyy: 1.8 (Diazo) for KSP 0.23.5
	+ Rewrite Toggle state logic from scratch. Actions are now monitored in real time and when changed via any means (such as a part's right-click menu), the Actions list updates correctly.
	+ Note that as a part of this, actions that previously could not be monitored (such as if a wheel had its steering inverted) can now be monitored via this feature.
	+ The toggle monitoring still defaults to off, please see release post at the end of thread.
	+ Fix how the ModuleAnimateGeneric is supported so parts with more then one animation should now behave correctly.
* 2014-xxyy: 1.7.2 (Diazo) for KSP 0.23.5
	+ Update to ModuleManager Version 2. Update part.cfg to match. 	+ Note that older versions of Module Manager should still work. No .dll changes.
* 2014-xxyy: 1.7.1 (Diazo) for KSP 0.23.5
	+ Fix Toggle state so it saves correctly.
* 2014-0629: 1.7 (Diazo) for KSP 0.23.5
	+ Flight window now changes its anchor to the bottom of the window when at the bottom of the screen. (Within 20 pixels.)
	+ Optional toggle state monitoring can be added on a per group basis.
	+ Actiongroups can now be grouped into 5 views so you can show/hide individual action groups without having to show/hide all of them.
	+ Added method for other mods to get an actiongroup's activated status. (See dev thread here.)
	+ This update changes the UI, overview of the changes only here. This first post is in the process of being updated and does not show the changes yet.
* 2014-0615: 1.6 (Diazo) for KSP 0.23.5
	+ Fully integrate with KSP's default action groups.
	+ Hide KSP's default action group editor when AGX is showing.
* 2014-0614: 1.5 (Diazo) for KSP 0.23.5
	+ Add support for multiple partModules with the same name on the same part.
	+ Add support for multiple actions with the same name on a part.
	+ Note that for a part with the same partModule and the same action name I will have to manually tweak the code, but the structure is in place and it will take about 30 seconds to add.
	+ So far the Module EnviroSensor from base KSP is the only place this has been necessary.
	+ Revise significant portions of the save/load code to fix weaknesses revealed by the above changes. (Should result in significantly fewer errors.)
	+ Simplify error handling significantly. Now instead of trying to actually handling errors, it simply stops loading that part and writes to the log ("AGX Fail...." messages). This means if an error happens, configured actions will vanish, but everything will keep working so no more KSP lockup issues.
* 2014-0608: 1.4.1 (Diazo) for KSP 0.23.5
	+ Hotfix to allow launching more then one rocket per game load.
	+ Also includes the start of my work to incorporate the base game actiongroups. The only thing in this version is that Keyset1, Groups 1 to 10 have the same keybind as KSP's default action groups.
		- IE: Changing the key in the KSP settings window will change the keybind in the AGX keyset window and changing the keybind in the AGX keyset window will change the key in KSP's setting window.
		- Note that only the Primary key is linked, the Secondary key is not linked and setting a key to this in default KSP will not be affected by AGX.
* 2014-0607: 1.4 (Diazo) for KSP 0.23.5
	+ Bugfix and stability release.
	+ Error trap VAB/SPH so you can't get stuck with a piece picked up you can't drop.
	+ When the Flight Scene loads with AGX hidden via the toolbar from a previous game session, ActionGroups will now work without having to show and then hide AGX first.
	+ Changes made in flight mode now save correctly. Note that I use the exposed KSP save methods, not my own so if you unexpectedly exit (crash, Alt-F4), you may still lose any recent changes you have made.
	+ Can now select small parts, sort of. Workaround: No change to left click but if you right click a part to show it's menu, that now counts as selecting the part for my mod and will add it to the list.
* 2014-0492: 1.3 (Diazo) for KSP 0.23.5
	+ KSP 23.5 Compatibility. No other changes of note.
* 2014-0401: 1.2.1 (Diazo)
	+ Fix to work with RemoteTech. New users download the file from Spaceport as normal, existing users see this post before updating.
* 2014-0331: 1.2 (Diazo)
	+ Bugfix to symmetry placement. You can now assign actions then go back and multiply the part with symmetry.
	+ Visibility of Flight Window now saved in Flight Scene. At present the only way to hide it is though the Toolbar.
	+ Spelling fix so Linux systems can now use this mod.
* 2014-0330: 1.1 (Diazo)
	+ Hotfix to make the built in Gear/Light/etc. action groups work again. This required removing the compatibility with the default 10 action groups. Action groups in groups Custom 1 through 10 will no longer load when this mod is installed. This will be reinstated in a future release.
	+ Keyset and KeyBinding changes are saved in the AGExt.cfg file. Do not overwrite this file when updating to save your keysets.
* 2014-0330: 1.0 (Diazo)
	+ Initial Release
