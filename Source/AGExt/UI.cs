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
        private static readonly Texture2D ButtonTexture = Asset.Texture2D.LoadFromFile("Textures", "ButtonTexture");
        private static readonly Texture2D ButtonTextureRed = Asset.Texture2D.LoadFromFile("Textures", "ButtonTextureRed");
        private static readonly Texture2D ButtonTextureGreen = Asset.Texture2D.LoadFromFile("Textures", "ButtonTextureGreen");

        internal static readonly Texture2D PartCenter = Asset.Texture2D.LoadFromFile("Textures", "PartLocationCross");
        internal static readonly Texture2D PartCross = Asset.Texture2D.LoadFromFile("Textures", "PartLocCross");
        internal static readonly Texture2D PartPlus = Asset.Texture2D.LoadFromFile("Textures", "PartLocPlus");
        internal static readonly Texture2D icon_button_38 = Asset.Texture2D.LoadFromFile("Textures", "icon_button_38");
        internal static readonly Texture2D icon_button_24 = Asset.Texture2D.LoadFromFile("Textures", "icon_button_24");
		internal static readonly Texture2D BtnTexRed = new Texture2D(1, 1);
		internal static readonly Texture2D BtnTexGrn = new Texture2D(1, 1);

		internal static readonly GUISkin AGXSkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
		internal static readonly GUIStyle AGXWinStyle = new GUIStyle(AGXSkin.window);

		internal static readonly GUIStyle AGXFldStyle = new GUIStyle(AGXSkin.textField);
		internal static readonly GUIStyle AGXFldStyleMiddleRight;
		internal static readonly GUIStyle AGXFldStyleMiddleCenter;

		internal static readonly GUIStyle AGXLblStyle = new GUIStyle(AGXSkin.label);
		internal static readonly GUIStyle AGXLblStyleMiddleCenter;
		internal static readonly GUIStyle AGXLblStyleMiddleCenterBold;
		internal static readonly GUIStyle AGXLblStyleMiddleCenterActive;
		internal static readonly GUIStyle AGXLblStyleMiddleCenterSelected;

		internal static readonly GUIStyle AGXBtnStyle = new GUIStyle(AGXSkin.button);
		internal static readonly GUIStyle AGXBtnStyleTextYellow;
		internal static readonly GUIStyle AGXBtnStyleAutoHide;
		internal static readonly GUIStyle AGXBtnStyleEnabled;
		internal static readonly GUIStyle AGXBtnStyleDisabled;
		internal static readonly GUIStyle AGXBtnStyleMiddleLeft;
		internal static readonly GUIStyle AGXBtnStyleMiddleLeftActive;
		internal static readonly GUIStyle AGXBtnStyleMiddleLeftSelected;
		internal static readonly GUIStyle AGXBtnStyleMiddleRight;
		internal static readonly GUIStyle AGXBtnStyleMiddleRightActive;
		internal static readonly GUIStyle AGXBtnStyleMiddleRightSelected;

		static UI()
		{
			BtnTexRed.SetPixel(0, 0, new Color(1, 0, 0, .5f));
			BtnTexRed.Apply();
			BtnTexGrn.SetPixel(0, 0, new Color(0, 1, 0, .5f));
			BtnTexGrn.Apply();

			Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

			{
				AGXFldStyle.fontStyle = FontStyle.Normal;
				AGXFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
				AGXFldStyle.font = font;
				{
					AGXFldStyleMiddleRight = new GUIStyle(AGXFldStyle);
					AGXFldStyleMiddleRight.alignment = TextAnchor.MiddleRight;

				}
				{
					AGXFldStyleMiddleCenter = new GUIStyle(AGXFldStyle);
					AGXFldStyleMiddleCenter.alignment = TextAnchor.MiddleCenter;
				}
			}
			{
				AGXLblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
				AGXLblStyle.wordWrap = false;
				AGXLblStyle.font = font;
				{ 
					AGXLblStyleMiddleCenter = new GUIStyle(AGXLblStyle);
					AGXLblStyleMiddleCenter.alignment = TextAnchor.MiddleCenter;
					{
						AGXLblStyleMiddleCenterActive = new GUIStyle(AGXLblStyleMiddleCenter);
						AGXLblStyleMiddleCenterActive.normal.background =  UI.ButtonTextureGreen;
						AGXLblStyleMiddleCenterActive.hover.background =  UI.ButtonTextureGreen;
					}
					{
						AGXLblStyleMiddleCenterSelected = new GUIStyle(AGXLblStyleMiddleCenter);
						AGXLblStyleMiddleCenterSelected.normal.background =  UI.ButtonTextureRed;
						AGXLblStyleMiddleCenterSelected.hover.background =  UI.ButtonTextureRed;
					}
				}
				{
					AGXLblStyleMiddleCenterBold = new GUIStyle(AGXLblStyleMiddleCenter);
					AGXLblStyleMiddleCenterBold.fontStyle = FontStyle.Bold;
					AGXLblStyleMiddleCenterBold.normal.textColor = new Color(0.5f, 1f, 0f, 1f);
				}
			}

			{ 
				AGXBtnStyle.fontStyle = FontStyle.Normal;
				AGXBtnStyle.alignment = TextAnchor.MiddleCenter;
				AGXBtnStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
				AGXBtnStyle.normal.background = UI.ButtonTexture;
				AGXBtnStyle.onNormal.background = UI.ButtonTexture;
				AGXBtnStyle.onActive.background = UI.ButtonTexture;
				AGXBtnStyle.onFocused.background = UI.ButtonTexture;
				AGXBtnStyle.onHover.background = UI.ButtonTexture;
				AGXBtnStyle.active.background = UI.ButtonTexture;
				AGXBtnStyle.focused.background = UI.ButtonTexture;
				AGXBtnStyle.hover.background = UI.ButtonTexture;
				AGXBtnStyle.font = font;
				{
					AGXBtnStyleTextYellow = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleTextYellow.normal.textColor = Color.yellow;
				}
				{
					AGXBtnStyleAutoHide = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleAutoHide.normal.background = UI.ButtonTextureRed;
					AGXBtnStyleAutoHide.hover.background = UI.ButtonTextureRed;
				}
				{
					AGXBtnStyleEnabled = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleEnabled.normal.background =  UI.ButtonTextureGreen;
					AGXBtnStyleEnabled.hover.background =  UI.ButtonTextureGreen;
				}
				{
					AGXBtnStyleDisabled = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleDisabled.normal.background =  UI.ButtonTextureRed;
					AGXBtnStyleDisabled.hover.background =  UI.ButtonTextureRed;
				}
				{
					AGXBtnStyleMiddleLeft = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleMiddleLeft.alignment = TextAnchor.MiddleLeft;
					{
						AGXBtnStyleMiddleLeftActive = new GUIStyle(AGXBtnStyleMiddleLeft);
						AGXBtnStyleMiddleLeftActive.normal.textColor = Color.green;
					}
					{
						AGXBtnStyleMiddleLeftSelected = new GUIStyle(AGXBtnStyleMiddleLeft);
						AGXBtnStyleMiddleLeftSelected.normal.textColor = Color.red;
					}
				}
				{
					AGXBtnStyleMiddleRight = new GUIStyle(AGXBtnStyle);
					AGXBtnStyleMiddleRight.alignment = TextAnchor.MiddleRight;
					{
						AGXBtnStyleMiddleRightActive = new GUIStyle(AGXBtnStyleMiddleRight);
						AGXBtnStyleMiddleRightActive.normal.textColor = Color.green;
					}
					{
						AGXBtnStyleMiddleRightSelected = new GUIStyle(AGXBtnStyleMiddleRight);
						AGXBtnStyleMiddleRightSelected.normal.textColor = Color.red;
					}
				}
			}
		}
	}
}
