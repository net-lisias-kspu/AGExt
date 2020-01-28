using System.Diagnostics;
using UnityEngine;

using Logger = KSPe.Util.Log.Logger;

namespace ActionGroupsExtended
{
    public static class Log
    {
        private static readonly Logger LOG = Logger.CreateForType<AGXEditor>();

        public static int debuglevel {
            get => (int)LOG.level;
            set => LOG.level = (KSPe.Util.Log.Level)(value % 6);
        }


        internal static void Init()
        {
            LOG.level =
#if DEBUG
                KSPe.Util.Log.Level.TRACE
#else
                KSPe.Util.Log.Level.INFO
#endif
                                ;
        }

        public static void log(string format, params object[] @parms)
        {
            LOG.force(format, parms);
        }

        public static void Detail(string format, params object[] @parms)
        {
            LOG.detail(format, parms);
        }

        public static void Info(string format, params object[] @parms)
        {
            LOG.info(format, parms);
        }

        public static void Warn(string format, params object[] @parms)
        {
            LOG.warn(format, parms);
        }

        public static void Err(string format, params object[] parms)
        {
            LOG.error(format, parms);
        }

        public static void ex(MonoBehaviour offended, System.Exception e)
        {
            LOG.error(offended, e);
        }

        [Conditional("DEBUG")]
        public static void Dbg(string format, params object[] @parms)
        {
            LOG.dbg(format, parms);
        }
    }
}
