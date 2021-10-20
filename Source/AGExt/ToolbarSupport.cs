/*
	This file is part of Action Groups Extended (AGExt) /L
	© 2018-21 Lisias T : http://lisias.net <support@lisias.net>
	© 2017-2018 LinuxGuruGamer
	© 2014-2016 Sir Diazo

	Action Groups Extended (AGExt) /L is licensed as follows:

	* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Action Groups Extended (AGExt) /L is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	Action Groups Extended (AGExt) /L. If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections.Generic;

using UnityEngine;
using KSP.UI.Screens;

using KSPe.Annotations;
using Toolbar = KSPe.UI.Toolbar;
using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;

namespace ActionGroupsExtended
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class ToolbarController : MonoBehaviour
	{
		internal static KSPe.UI.Toolbar.Toolbar Instance => KSPe.UI.Toolbar.Controller.Instance.Get<ToolbarController>();

		[UsedImplicitly]
		private void Start()
		{
			KSPe.UI.Toolbar.Controller.Instance.Register<ToolbarController>(Version.FriendlyName);
		}
	}
}
