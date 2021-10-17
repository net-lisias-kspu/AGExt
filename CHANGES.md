# Action Groups Extended /L Unleashed :: Changes

* 2021-1028: 2.4.1.3 (lisias) for KSP >= 1.3.1
	+ Raising the bar to 1.3.1. 
		- from KSP 1.3.0 to 1.3.1, Squad broke the interface for `GameParameters.CustomParameterUI`: `tittle` and `toolTip` were fields, and became properties. This obviously screwed up thing at runtime, leading to variables that was never null to be null on 1.3.1 and newer, and this is the reason something on the C++ land blew up on a Null Pointer when using DLLs compiled against 1.3.0 (and older) on 1.3.1 and newer that use `GameParameters.CustomParameterUI`
		- So until I cook a way to overcome this, AGExt will be compatible to 1.3.1 and newer only.
	+ Some pretty lame mistakes fixed...
* 2021-1016: 2.4.1.2 (lisias) for KSP >= 1.3
	+ Updating the code to use the new KSPe 2.4 facilities
	+ Compatibility down to KSP 1.3.0 verified.
	+ Some log on the (huge) logging
	+ Bumping up version to catch up with the upstream
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
