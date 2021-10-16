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
#if false
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace AGExt
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    //   HighLogic.CurrentGame.Parameters.CustomParams<AGExt>()

    public class AG_Ext : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Action Groups Extended"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "AGExt"; } }
        public override string DisplaySection { get { return "AGExt 2"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Use Blizzy Toolbar", toolTip = "If available")]
        public bool useBlizzy = false;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {

            return true;
        }


        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
#endif