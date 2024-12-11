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
using System.Collections.Generic;

namespace ActionGroupsExtended
{
    public class AGXPart
    {
        public Part AGPart;
        public List<BaseAction> AGba;

        public AGXPart()
        {
            AGba = new List<BaseAction>();
        }

        public AGXPart(Part p)
        {
            AGba = p.Actions;
            AGPart = p;
        }
        
    }

    public class AGXActionsState
    {
        public int group;
        public bool actionOff;
        public bool actionOn;
    }

    public class AGXPartVesselCheck
    {
        public Part prt;
        public Vessel pVsl;
    }

}