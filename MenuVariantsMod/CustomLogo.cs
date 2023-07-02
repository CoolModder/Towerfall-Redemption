using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;

namespace MenuVariantsMod
{
    public class CustomLogo //Just the class for logos.... 
    {
        public bool VanillaBg;
        public bool VanillaArrow;
        public bool VanillaAscension;
        public bool VanillaTitle;
        public bool VanillaAscensionCore;
        public string Name;
        public string Ascension;
        public string Title;
        public string TitleLight;
        public string Arrow;
        public string Bg;
        public string AscensionCore;

        public Atlas Atlas;
        public Vector2 BgOrigin;
        public CustomLogo(XmlElement xml, Atlas atlas)
        {
            Name = xml.ChildText("name", defaultValue: "Name Not Defined");
            VanillaArrow = xml.ChildBool("VanillaArrow", defaultValue: true);
            VanillaBg = xml.ChildBool("VanillaBg", defaultValue: true);
            VanillaAscension = xml.ChildBool("VanillaAscension", defaultValue: true);
            VanillaAscensionCore = xml.ChildBool("VanillaAscensionCore", defaultValue: true);
            VanillaTitle = xml.ChildBool("VanillaTitle", defaultValue: true);
            Ascension = xml.ChildText("Ascension", defaultValue: "title/ascension");
            AscensionCore = xml.ChildText("AscensionCore", defaultValue: "title/ascension");
            Arrow = xml.ChildText("Arrow", defaultValue: "title/bigArrow");
            Bg = xml.ChildText("Bg", defaultValue: "title/bg");
            Title = xml.ChildText("Title", defaultValue: "title/titleLetters");
            TitleLight = xml.ChildText("TitleLight", defaultValue: "title/titleLettersLight");
            Atlas = atlas;
            BgOrigin = xml.ChildPosition("BgOrigin", defaultValue: new Vector2(148f, 74f));
        }

    }
}
