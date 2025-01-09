using Monocle;
using System.Xml;

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