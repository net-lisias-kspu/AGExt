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
using System.Reflection;

namespace ActionGroupsExtended
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    //   HighLogic.CurrentGame.Parameters.CustomParams<AGExt>()

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

	//[KSPAddon(KSPAddon.Startup.Instantly, true)]
	//internal class Test:MonoBehaviour
	//{
	//	[UsedImplicitly]
	//	private void Start()
	//	{
	//		Log.force("Test.Startup");
	//		try
	//		{
	//			GameParameters gp = GameParameters.GetDefaultParameters(Game.Modes.SANDBOX, GameParameters.Preset.Easy);
	//			Log.force("{0}", gp.CustomParams<AGExt>().OverrideCareer);
	//		}
	//		catch (Exception e)
	//		{
	//			Log.ex(this, e);
	//		}
	//	}
	//}
}
