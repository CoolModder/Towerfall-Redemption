using Monocle;
using FortRise;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class BezelLoad
    {
        public static List<CustomBezel> BezelList = new();
        public static List<string> BezelNames = new List<string>();
        public static void Load(FortContent content) {
            var _separator = Path.DirectorySeparatorChar.ToString();
            var _customLogos = "Content" + _separator + "Mod" + _separator + "CustomBezels" + _separator;
            string[] directories = Directory.GetDirectories(_customLogos);
            string[] array = directories;
            BezelNames.Add("DEFAULT");
            BezelList = new List<CustomBezel>();
            foreach (string text in array)
            {
                Console.WriteLine(text);
                var text2 = text.Replace("Content" + _separator, "");
                Atlas atlas = AtlasExt.CreateAtlas(content, text2 + _separator + "atlas.xml", text2 + _separator + "atlas.png", true, ContentAccess.Content);
                var BezelData = Calc.LoadXML(text + _separator + "BezelData.xml");
                var TrueBezelData = BezelData["BezelData"];
                Console.WriteLine(atlas);
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
