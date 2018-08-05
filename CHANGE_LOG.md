# Action Groups Extended :: Change Log

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
