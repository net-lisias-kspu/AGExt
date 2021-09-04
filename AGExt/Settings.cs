using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace ActionGroupsExtended
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    //   HighLogic.CurrentGame.Parameters.CustomParams<AGExt>()

    public class AGExt : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "AGExt"; } }
        public override string DisplaySection { get { return "Action Groups Extended"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Action Groups Lockout always availabe", toolTip = "To override Career mode and always have all action groups available")]
        public bool OverrideCareer = false;



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
