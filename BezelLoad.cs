using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TowerFall;
using FortRise;
using MonoMod.ModInterop;
using System.Diagnostics;

namespace MenuVariantsMod
{
    public class BezelLoad
    {
        public static List<CustomBezel> BezelList;
        public static List<string> BezelNames = new List<string>();
        public static void Load() {
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
                Atlas atlas = new Atlas(text2 + _separator + "atlas.xml", text2 + _separator + "atlas.png", true);
                var BezelData = Calc.LoadXML(text + _separator + "BezelData.xml");
                var TrueBezelData = BezelData["BezelData"];
                Console.WriteLine(atlas);
                if (atlas == null)
                {
                    Debugger.Break();
                }
                //if (TrueLogoData != null)
                //{
                    CustomBezel BezelCustomData = new CustomBezel(TrueBezelData, atlas);
                   //if (logoCustomData != null)
                   //{
                        BezelList.Add(BezelCustomData);
                        BezelNames.Add(TrueBezelData.ChildText("Name"));
                  // }
                    
                //}
            }
        }
    }
}
