# Action Groups Extended :: Change Log

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
