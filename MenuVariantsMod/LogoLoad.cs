using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class LogoLoad
    {
        public static List<CustomLogo> LogoList;
        public static void Load(FortContent content) {
            MenuVariantModModule.Vanilla = new List<bool>();
            MenuVariantModModule.Vanilla.Add(true);
            MenuVariantModModule.Vanilla.Add(true);
            
            var _separator = Path.DirectorySeparatorChar.ToString();
            string _customLogos = "Mods" + _separator + "MenuVariantsMod" + _separator + "Content" + _separator + "CustomLogos" + _separator; 
            
            string[] directories = Directory.GetDirectories(_customLogos);
            string[] array = directories;
            LogoList = new List<CustomLogo>();
            foreach (string customLogoPath in array)
            {
                Console.WriteLine(customLogoPath);
                Atlas atlas = AtlasExt.CreateAtlas(content, customLogoPath + _separator + "atlas.xml", customLogoPath + _separator + "atlas.png", true, ContentAccess.Root);
                var LogoData = Calc.LoadXML(customLogoPath + _separator + "LogoData.xml");
                var TrueLogoData = LogoData["LogoData"];
                Console.WriteLine(atlas);
                
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
