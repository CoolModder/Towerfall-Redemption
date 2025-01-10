using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class LogoLoad
    {
        public static List<CustomLogo> LogoList;
        private static readonly string CUSTOM_LOGOS_DIR = "Content/CustomLogos";
        
        public static void Load(FortContent content) 
        {
            MenuVariantModModule.Vanilla = new List<bool>();
            MenuVariantModModule.Vanilla.Add(true);
            MenuVariantModModule.Vanilla.Add(true);
            
            LogoList = new List<CustomLogo>();
            foreach (RiseCore.Resource resource in content[CUSTOM_LOGOS_DIR].Childrens)
            {
                if (resource.Childrens == null)
                    continue;
                
                Atlas atlas = AtlasExt.CreateAtlas(content, $"{resource.Path}/atlas.xml" , $"{resource.Path}/atlas.png", ContentAccess.ModContent);
                RiseCore.Resource logoDataResource = content[$"{resource.Path}/LogoData.xml"];
                var LogoData = Calc.LoadXML(logoDataResource.Stream);
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
