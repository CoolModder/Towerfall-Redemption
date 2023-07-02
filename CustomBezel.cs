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
    public class CustomBezel //Just the class for BEZELS.... 
    {
       
        public string Name;
        public string Left;
        public string Right;
        public Atlas Atlas;

        public CustomBezel(XmlElement xml, Atlas atlas)
        {
            Name = xml.ChildText("Name", defaultValue: "Name Not Defined");
            Left = xml.ChildText("Left", defaultValue: "Name Not Defined");
            Right = xml.ChildText("Right", defaultValue: "Name Not Defined");
            Atlas = atlas;
        }

    }
}