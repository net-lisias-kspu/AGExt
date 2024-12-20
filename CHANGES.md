# Action Groups Extended /L Unleashed :: Changes

* 2024-1219: 2.4.2.1 (lisias) for KSP >= 1.3.1
	+ Rolling back some of the refactorings, as I did some stupidity on doing it and don't have time right now to hunt it down.
		- I will try again next year.
	+ Updates save settings code to cope with the refactored `KSPe.IO.Save<T>`.
		- **Needs** KSPe 2.5.5.0 or superior. 
	+ Catches up with upstream:
		- 2.4.1.3 (upstream):
			- Updated initialization of the EditShow variable from true to false
			- Added check for exit window being displayed, if so, doesn't show the agext window
		- 2.4.1.2 (upstream):
			- Fixed small memory leak by not removing a callback from a game event
* 2024-1215: 2.4.2.0 (lisias) for KSP >= 1.3.1
	+ ***WITHDRAWN*** due major borkage on some refactorings.
