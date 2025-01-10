using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class BezelLoad
    {
        public static List<CustomBezel> BezelList = new();
        public static List<string> BezelNames = new();
        
        private static readonly string CUSTOM_BEZELS_DIR = Path.Combine("Mods", "MenuVariantsMod", "Content", "CustomBezels");
        
        public static void Load(FortContent content) {
            string[] directories = Directory.GetDirectories(CUSTOM_BEZELS_DIR);
            BezelNames.Add("DEFAULT");
            BezelList = new List<CustomBezel>();
            foreach (string customBezelPath in directories)
            {
                Atlas atlas = AtlasExt.CreateAtlas(content, Path.Combine(customBezelPath, "atlas.xml"), Path.Combine(customBezelPath,"atlas.png"));
                var BezelData = Calc.LoadXML(Path.Combine(customBezelPath, "BezelData.xml"));
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
