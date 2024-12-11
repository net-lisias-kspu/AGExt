/*
	This file is part of Action Groups Extended (AGExt) /L Unleashed
		© 2018-2024 LisiasT : https://lisias.net : http://lisias.net <support@lisias.net>
		© 2017-2018 LinuxGuruGamer
		© 2014-2016 Sir Diazo

	Action Groups Extended (AGExt) /L Unleashed is licensed as follows:

		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Action Groups Extended (AGExt) /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Action Groups Extended (AGExt) /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections;
using System.Reflection;

namespace ActionGroupsExtended
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

	public class AGExt : GameParameters.CustomParameterNode
    {
        public override string Title => "Action Groups Extended";
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;
        public override string Section => "AGExt"; 
        public override string DisplaySection => "Action Groups Extended";
        public override int SectionOrder => 1;
        public override bool HasPresets => false;

        [GameParameters.CustomParameterUI("Action Groups Lockout always availabe", toolTip = "To override Career mode and always have all action groups available")]
        public Boolean OverrideCareer = false;

        public override void OnLoad(ConfigNode node)
        {
            Log.dbg("OnLoad {0}", node);
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            Log.dbg("OnSave {0}", node);
            base.OnSave(node);
        }

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Log.dbg("SetDifficultyPreset {0}", preset);
            base.SetDifficultyPreset(preset);
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            Log.dbg("Enabled {0} {1}", member, parameters);
            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            Log.dbg("Interactibles {0} {1}", member, parameters);
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            Log.dbg("ValidValues {0}", member);
            return base.ValidValues(member);
        }
    }
}
