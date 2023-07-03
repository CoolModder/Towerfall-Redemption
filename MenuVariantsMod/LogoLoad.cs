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
    public class LogoLoad
    {
        public static List<CustomLogo> LogoList;
        public static void Load() {
            MenuVariantModModule.Vanilla = new List<bool>();
            MenuVariantModModule.Vanilla.Add(true);
            MenuVariantModModule.Vanilla.Add(true);
            var _separator = Path.DirectorySeparatorChar.ToString();
            var _customLogos = "Content" + _separator + "Mod" + _separator + "CustomLogos" + _separator;
            string[] directories = Directory.GetDirectories(_customLogos);
            string[] array = directories;
            LogoList = new List<CustomLogo>();
            foreach (string text in array)
            {
                Console.WriteLine(text);
                var text2 = text.Replace("Content" + _separator, "");
                Atlas atlas = new Atlas(text2 + _separator + "atlas.xml", text2 + _separator + "atlas.png", true);
                var LogoData = Calc.LoadXML(text + _separator + "LogoData.xml");
                var TrueLogoData = LogoData["LogoData"];
                Console.WriteLine(atlas);
                if (atlas == null)
                {
                    Debugger.Break();
                }
                //if (TrueLogoData != null)
                //{
                    CustomLogo logoCustomData = new CustomLogo(TrueLogoData, atlas);
                   //if (logoCustomData != null)
                   //{
                        LogoList.Add(logoCustomData);
                        MenuVariantModModule.MenuVariantNames.Add(TrueLogoData.ChildText("Name"));
                        MenuVariantModModule.Vanilla.Add(false);
                  // }
                    
                //}
            }
        }
    }
}
