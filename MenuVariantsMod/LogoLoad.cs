using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class LogoLoad
    {
        public static List<CustomLogo> LogoList;
        
        private static readonly string CUSTOM_LOGOS_DIR =  Path.Combine("Mods", "MenuVariantsMod", "Content", "CustomLogos" );
        
        public static void Load(FortContent content) {
            MenuVariantModModule.Vanilla = new List<bool>();
            MenuVariantModModule.Vanilla.Add(true);
            MenuVariantModModule.Vanilla.Add(true);
            
            string[] directories = Directory.GetDirectories(CUSTOM_LOGOS_DIR);
            LogoList = new List<CustomLogo>();
            foreach (string customLogoPath in directories)
            {
                Atlas atlas = AtlasExt.CreateAtlas(content, Path.Combine(customLogoPath,"atlas.xml" ), Path.Combine(customLogoPath,"atlas.png"));
                var LogoData = Calc.LoadXML(Path.Combine(customLogoPath, "LogoData.xml") );
                var TrueLogoData = LogoData["LogoData"];
                
                if (atlas == null)
                {
                    Debugger.Break();
                }
                CustomLogo logoCustomData = new CustomLogo(TrueLogoData, atlas);
                LogoList.Add(logoCustomData);
                MenuVariantModModule.MenuVariantNames.Add(TrueLogoData.ChildText("Name"));
                MenuVariantModModule.Vanilla.Add(false);
            }
        }
    }
}
