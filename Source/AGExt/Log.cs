/*
	This file is part of Action Groups Extended (AGExt) /L
	© 2018-21 Lisias T : http://lisias.net <support@lisias.net>

	THIS FILE is licensed to you under:

	* WTFPL - http://www.wtfpl.net
		* Everyone is permitted to copy and distribute verbatim or modified
 			copies of this license document, and changing it is allowed as long
			as the name is changed.

	THIS FILE is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
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
