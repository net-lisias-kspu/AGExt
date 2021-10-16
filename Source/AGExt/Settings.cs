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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace ActionGroupsExtended
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    //   HighLogic.CurrentGame.Parameters.CustomParams<AGExt>()

    #if false
    public class AGExt : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Action Groups Extended"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "AGExt"; } }
        public override string DisplaySection { get { return "Action Groups Extended"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Action Groups Lockout always availabe", toolTip = "To override Career mode and always have all action groups available")]
        public bool OverrideCareer = false;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            base.SetDifficultyPreset(preset);
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
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                {
                    Type t = ((FieldInfo)member).FieldType;
                    Log.dbg("ValidValues {0} {1}", member, t);
                    if (t == typeof(bool)) return new List<bool>(new bool[]{false, true });
                }  break;
                case MemberTypes.Event:
                case MemberTypes.Method:
                case MemberTypes.Property:
                default:
                    break;
            }
            Log.dbg("base.ValidValues {0}", member);
            return base.ValidValues(member);
        }
    }
    #endif
}
