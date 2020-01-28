# Action Groups Extended :: Change Log

* 2018-0806: 2.4.0.2 (lisias) for KSP >= 1.4
	+ Moving the whole shebang into `net-lisias-ksp` to prevent clashes with the upstream
	+ Decluttered the GameData by moving Textures into `PluginData`
		- Fixed double loading Textures
	+ Moved patches to `Patches` folder
	+ Updated to the latest KSPe API
		+ Added instalment check facilities 
	+ Merging fixes from upstream:
		- 2.4.0.1
			- Fixed nullref on an edge case:
			- Have the action editor open before going on EVA.
			- Left click on EVA Kerbal to select in editor (blue cross)
			- Board vessel
		- 2.3.4.1
			- fixed missing kOSVoidAction
		- 2.3.4
			- Fixed panel selection for KSP 1.7, now required to wait until panel is displayed, the internal flags in KSP are now being set slightly differently
		- Changes on upstream not mentioned here were already fixed on this fork.
* 2018-0806: 2.3.3.7 (lisias) for KSP 1.4.x
	+ Moving settings/configuration files into <KSP_ROOT>/PluginData
* 2018-0503: 2.3.3.6 (linuxgurugamer) for KSP 1.4.2
	+ Updated to registering with Toolbar Controller
	+ Removed Blizzy option from settings
	+ Updated version file for all 1.4 versions
* 2018-0413: 2.3.3.5 (linuxgurugamer) for KSP 1.4.2
	+ Revert last change of the MM code
* 2018-0412: 2.3.3.4 (linuxgurugamer) for KSP 1.4.2
	+ Commented out MM code which was adding depreciated ModuleAGX to all parts
	+ Commented out unused variables
* 2018-0408: 2.3.3.3 (linuxgurugamer) for KSP 1.4.2
	+ Added German Translation
* 2018-0408: 2.3.3.2 (linuxgurugamer) for KSP 1.4.2
	+ Fixed missing localization function for Group2 in Flight
* 2018-0407: 2.3.3.1 (linuxgurugamer) for KSP 1.4.2
	+ Fixed localization typo for Group2 in Editor
* 2018-0316: 2.3.3 (linuxgurugamer) for KSP 1.4.1
	+ Updated for 1.4.1
	+ Added support for the Toolbar Controller
	+ Added simple settings page to toggle use of the Blizzy toolbar
	+ Added additional button to settings window to toggle Blizzy toolbar
* 2018-0302: 2.3.2.3 (linuxgurugamer) for KSP 1.3.1
	+ Thanks to forum user @Arivald Ha'gel for the following:
		- Invalid conversion from uint to int fixed
	+ Added support for the Click Through Blocker.  ClickThroughBlocker is now a requirement
* 2018-0201: 2.3.2.2 (linuxgurugamer) for KSP 1.3.1
	+ Fixed localization strings in the editor
* 2018-0127: 2.3.2.1 (linuxgurugamer) for KSP 1.3.1
	+ Moved Localization folder into AGExt (was being missed by Jenkins)
* 2018-0122: 2.3.2 (linuxgurugamer) for KSP 1.3.1
	+ Added Log module
	+ Changed all Debug.Log to Log.Info
* 2017-1106: 2.3.1 (linuxgurugamer) for KSP 1.3.1
	+ Simplification and optimization of Flight.CheckActionsActiveActualCode
* 2017-1104: 2.3.0 (linuxgurugamer) for KSP 1.3.1
	+ This is just a straight recompile, with my normal updates for my build process
* 2016-1211: 2.2 (Diazo) for KSP 1.2
	+ Fix SmartParts radio compatibiliy (OtherVessel class)
	+ Fix DefaultActionGroups compatibility (Now look for assigned action groups when part placed.)
	+ Revise Kerbal Reboarding method so hopefully it isn't considered a sub-vessel.
* 2016-1031: 2.1.2 (Diazo) for KSP 1.2
	+ Fix solar panels.
	+ Change "Toggle" button to "StateVis" (State Visibility) to make clear what it actually does.
* 2016-1023: 2.1.1 (Diazo) for KSP 1.2
	+ [blizzy78/ksp_toolbar#39](https://github.com/blizzy78/ksp_toolbar/pull/39)
	+ No changes to AGX itself, will run with Contract Configurator installed now
* 2016-1016: 2.1 (Diazo) for KSP 1.2
	+ KSP 1.2 Update
	+ Recognize new stock Always Show Actiongroups option.
* 2016-0429: 2.0.2 (Diazo) for KSP 1.1
	+ Fix bug where some parts (notably the new wheels) could not be selected in flight mode to edit their actions.
* 2016-0423: 2.0.1 (Diazo) for KSP 1.1
	+ Show/Hide mod windows on F2 key.
	+ Fix settings file so it doesn't void ModuleManager's cache every load.
	+ Fix nullref error on Flags that have been planted.
* 2016-0401: 2.0 (Diazo) for KSP 1.1
	+ KSP 1.1 Update. 
* 2016-0321: 1.34.2 (Diazo) for KSP 1.0.5
	+ Fix an error in checking for Kerbal on EVA vs. a vessel that would cause log spam while on EVA.
* 2016-0318: 1.35 (Diazo) for KSP 1.0.5
	+ Refactor how saving of non-actions data (group names, keybinds, etc.) work to fix docking errors. 
* 2015-1225: 1.34.4 (Diazo) for KSP 1.0.5
	+ Add new External interface methods. No player visible changes. 
* 2015-1217: 1.34.3 (Diazo) for KSP 1.0.5
	+ Fix action save/load for new format Squad is using.
	+ Lock action groups out correctly when a vessel is out of control. (Was being done previously in the wrong way.)
* 2015-1118: 1.34.2 (Diazo) for KSP 1.0.5
	+ Fix Editor panels.
	+ Fix error on Kerbal EVA
 * 2015-1115: 1.34.1 (Diazo) for KSP 1.0.5
	+ Fix Editor windows going screwy under several circumstances.
	+ Add workaround for Toggle action on controls surfaces to account for wierdness.
* 2015-1112: 1.34 (Diazo) for KSP 1.0.5
	+ KSP 1.0.5
	+ Move data storage to PartModule, no more external files.
* 2015-0710: 1.33.1 (Diazo) for KSP 1.0.4
	+ Fix symmetry loading bug where a set of symmetry placement parts would always load all actions symmetrically even if the action was only present of one part.
* 2015-0624: 1.33 (Diazo) for KSP 1.0.4
	+ KSP 1.0.4 Recompile
	+ Minor tweaks and bugfixes
	+ ModActions compatibility
* 2015-0607: 1.32.4 (Diazo) for KSP 1.0
	+ Compatibility for the new mod ModActions.
* 2015-0516: 1.32.2 (Diazo) for KSP 1.0
	+ Version fix for CKAN.
	+ Previous version 1.32a still had the version 1.32 .dll file, need to push a new version so CKAN updates.
* 2015-0515: 1.32.1 (Diazo) for KSP 1.0
	+ Edit: Release file is version 1.32 (oops). Leaving it in place so CKAN does not throw a fit.
		- Will not be able to upload the correct release until Saturday morning.
	+ External Interface tweaks (previous commit)
	+ Monitor FAR spoiler actions correctly
	+ Add KIS config so parts will stack in inventory
	+ Fix MERGE option in editor when loading a second craft
* 2015-0428: 1.32 (Diazo) for KSP 1.0
	+ KSP 1.0 Update.
* 2015-0419: 1.31.8 (Diazo) for KSP 0.90
	+ Memory leak fix.
* 2015-0416: 1.31.7 (Diazo) for KSP 0.90
	+ Disable toggle checking on hiding window, trying to track down a memory leak.
* 2015-0411: 1.31.6 (Diazo) for KSP 0.90
	+ Fix duplicating actions
* 2015-0411: 1.31.5 (Diazo) for KSP 0.90
	+ Fix some stupid typos.
* 2015-0410: 1.31.4 (Diazo) for KSP 0.90
	+ more kOS tweaks.
* 2015-0407: 1.31.3 (Diazo) for KSP 0.90
	+ GUI Fixes
		- Lock down the font
		- Fix when Scroll Windows show.
* 2015-0404: 1.31.2 (Diazo) for KSP 0.90
	+ Sub-Assemblies now save Toggle and Hold states.
* 2015-0403: 1.31.1 (Diazo) for KSP 0.90
	+ kOS tweaks
	+ MonitorDefaultActions error loggin
* 2015-0326: 1.31 (Diazo) for KSP 0.90
	+ Add option to have a "hold-to-activate" group, as per the Brakes group.
	+ Change how options file saves so it will no longer reset when updating in future versions. (This version will still reset your options however.) This also allows for a modulemanager patch to set options for this mod.
* 2015-0208: 1.30.3 (Diazo) for KSP 0.90
	+ Fix Smartparts so local activations are not RemoteTech delayed.
	+ Tweak RemoteTech bypass so it is a true bypass (both connection and delay)
	+ Fix issue where disabled actions were not being hidden
* 2015-0205: 1.30.1 (Diazo) for KSP 0.90
	+ Fix ModuleRCS
	+ Add RemoteTech Bypass
* 2015-0201: 1.30 (Diazo) for KSP 0.90
	+ Add remotetech support
	+ Run cleanup once per session
	+ add support for COntrol Lock
* 2015-0108: 1.29.4 (Diazo) for KSP 0.90
	+ Move Auto-Hide button from the top-left of the Groups window to the Right-Click menu on the toolbar button.
	+ Fix FSCopter action monitoring code
* 2015-0105: 1.29.3 (Diazo) for KSP 0.90
	+ Fix ModuleEnginesFX so they load correctly
	+ More External Interface tweaks.
* 2015-0104: 1.29.2 (Diazo) for KSP 0.90
	+ Further external support tweaks.
* 2015-0104: 1.29.1 (Diazo) for KSP 0.90
	+ Tweak key lockout
* 2015-0104: 1.29 (Diazo) for KSP 0.90
	+ Overhaul external mod support for integration
	+ No changes to AGX itself this version
* 2015-0101: 1.28.2 (Diazo) for KSP 0.90
	+ Add more error traps around the AGXEditorNode null error currently being seen.
	+ Tweak external support for other mods. 
* 2014-1231: 1.28.1 (Diazo) for KSP 0.90
	+ Fix bug where deleted actions could reappear under some circumstances. 
* 2014-1230: 1.28 (Diazo) for KSP 0.90
	+ Add sub-assembly support
* 2014-1228: 1.27 (Diazo) for KSP 0.90
	+ Fix SPH made vessels not saving their data across game sessions
	+ ModuleManager is now required for upcoming changes to support sub-assemblies. This is not functional in this version but enough of the code is present that ModuleManager is required in the background for the code to execute.
* 2014-1224: 1.26 (Diazo) for KSP 0.90
	+ Add career mode compatibility so that only action groups you have unlocked show up.
	+ In flight, the highest level building between the SPH and VAB will be used.
	+ Add override capability for this feature so that you can always use all action groups if you wish.
* 2014-1222: 1.25.4 (Diazo) for KSP 0.90
	+ Fix Keysets so they actually work.
	+ Reduce log spam clutter including removal of AGX Ambiguous message from main screen, it is still logged.
* 2014-1219: 1.25.1 (Diazo) for KSP 0.90
	+ Various bugfixes
	+ Add stock toolbar support
	+ This version does not fix the known issue of only KeySet1 working in flight mode.
* 2014-1216: 1.25 (Diazo) for KSP 0.90
	+ KSP 0.90 fix
	+ This version runs without error and basic functionality is present.
	+ While most features should be working, there is a known issue that only Keyset1 works in flight mode.
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
