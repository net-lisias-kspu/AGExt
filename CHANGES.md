# Action Groups Extended /L Unleashed :: Changes

* 2024-1215: 2.4.2.0 (lisias) for KSP >= 1.3.1
	+ Updates save settings code to cope with the refactored `KSPe.IO.Save<T>`.
		- **Needs** KSPe 2.5.5.0 or superior. 
	+ Huge refactoring, removing dead/useless code and optimising the whole GUI code.
		- Now we can change things on the GUI without breaking half of the other widgets. 
	+ Catches up with upstream:
		- 2.4.1.3 (upstream):
			- Updated initialization of the EditShow variable from true to false
			- Added check for exit window being displayed, if so, doesn't show the agext window
		- 2.4.1.2 (upstream):
			- Fixed small memory leak by not removing a callback from a game event
