# Action Groups Extended :: Changes

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
