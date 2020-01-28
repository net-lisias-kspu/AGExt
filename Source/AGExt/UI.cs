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
    }
}
