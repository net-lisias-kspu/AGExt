using UnityEngine;
using ToolbarControl_NS;
using KSP_Log;

namespace ActionGroupsExtended
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        internal static Log Log = null;

        void Awake()
        {
            if (Log == null)
#if DEBUG
                Log = new Log("AGExt", Log.LEVEL.INFO);
#else
                Log = new Log("AGExt", Log.LEVEL.ERROR);
#endif

        }

        void Start()
        {
            ToolbarControl.RegisterMod(AGXFlight.MODID, AGXFlight.MODNAME);
        }
    }
}