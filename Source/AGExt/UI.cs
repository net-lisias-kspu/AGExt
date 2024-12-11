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
using UnityEngine;

namespace ActionGroupsExtended
{
    using Asset = KSPe.IO.Asset<AGXEditor>;

    public static class UI
    {
        internal static readonly Texture2D ButtonTexture = Asset.Texture2D.LoadFromFile("Textures", "ButtonTexture");
        internal static readonly Texture2D ButtonTextureRed = Asset.Texture2D.LoadFromFile("Textures", "ButtonTextureRed");
        internal static readonly Texture2D ButtonTextureGreen = Asset.Texture2D.LoadFromFile("Textures", "ButtonTextureGreen");
        internal static readonly Texture2D PartCenter = Asset.Texture2D.LoadFromFile("Textures", "PartLocationCross");
        internal static readonly Texture2D PartCross = Asset.Texture2D.LoadFromFile("Textures", "PartLocCross");
        internal static readonly Texture2D PartPlus = Asset.Texture2D.LoadFromFile("Textures", "PartLocPlus");
        internal static readonly Texture2D icon_button_38 = Asset.Texture2D.LoadFromFile("Textures", "icon_button_38");
        internal static readonly Texture2D icon_button_24 = Asset.Texture2D.LoadFromFile("Textures", "icon_button_24");
    }
}
