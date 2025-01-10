using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class BezelLoad
    {
        public static List<CustomBezel> BezelList = new();
        public static List<string> BezelNames = new();
        
        private static readonly string CUSTOM_BEZELS_DIR ="Content/CustomBezels";
        
        public static void Load(FortContent content) 
        {
            BezelNames.Add("DEFAULT");
            BezelList = new List<CustomBezel>();
            
            foreach (RiseCore.Resource resource in content[CUSTOM_BEZELS_DIR].Childrens)
            {
                Atlas atlas = AtlasExt.CreateAtlas(content, $"{resource.Path}/atlas.xml", $"{resource.Path}/atlas.png", ContentAccess.ModContent);
                
                RiseCore.Resource bezelDataResource = content[$"{resource.Path}/BezelData.xml"];
                var BezelData = Calc.LoadXML(bezelDataResource.Stream);
                var TrueBezelData = BezelData["BezelData"];
                if (atlas == null)
                {
                    Debugger.Break();
                }
                CustomBezel BezelCustomData = new CustomBezel(TrueBezelData, atlas);
                BezelList.Add(BezelCustomData);
                BezelNames.Add(TrueBezelData.ChildText("Name"));
            }
        }
    }
}
