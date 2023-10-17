using FortRise;
using Monocle;
using SeasonalChestMod;
using System;
using System.Collections.Generic;
namespace SeasonChestMod;


[Fort("com.CoolModder.SeasonChest", "SeasonalChestMod")]
public class SeasonChestModModule : FortModule
{
    public static Atlas ChestAtlas;

    public static SeasonChestModModule Instance;

    public List<SeasonalChestFormat> ChestVariants = new List<SeasonalChestFormat>();

    public static SeasonalChestFormat seasonalChest;

    public SeasonChestModModule() 
    {
        Instance = this;
    }
    public override Type SettingsType => typeof(SeasonChestModSettings);
    public static SeasonChestModSettings Settings => (SeasonChestModSettings)Instance.InternalSettings;

    public override void LoadContent()
    {
        ChestAtlas = Atlas.Create("Atlas/atlas.xml", "Atlas/atlas.png", true, ContentAccess.ModContent);
        seasonalChest = null;
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 12, 18), new DateTime(DateTime.Now.Year, 12, 26), ChestAtlas["christmasNormal"], ChestAtlas["christmasSpecial"], ChestAtlas["christmasBig"], ChestAtlas["christmasBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 10, 24), new DateTime(DateTime.Now.Year, 11, 1), ChestAtlas["halloweenNormal"], ChestAtlas["halloweenSpecial"], ChestAtlas["halloweenBig"], ChestAtlas["halloweenBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 5, 23), new DateTime(DateTime.Now.Year, 5, 26), ChestAtlas["birthdayNormal"], ChestAtlas["birthdaySpecial"], ChestAtlas["birthdayBig"], ChestAtlas["birthdayBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 3, 15), new DateTime(DateTime.Now.Year, 3, 20), ChestAtlas["patrickNormal"], ChestAtlas["patrickSpecial"], ChestAtlas["patrickBig"], ChestAtlas["patrickBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 3, 22), new DateTime(DateTime.Now.Year, 4, 25), ChestAtlas["easterNormal"], ChestAtlas["easterSpecial"], ChestAtlas["easterBig"], ChestAtlas["easterBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 11, 21), new DateTime(DateTime.Now.Year, 11, 25), ChestAtlas["thankNormal"], ChestAtlas["thankSpecial"], ChestAtlas["thankBig"], ChestAtlas["thankBottomless"]));
        ChestVariants.Add(new SeasonalChestFormat(new DateTime(DateTime.Now.Year, 8, 6), new DateTime(DateTime.Now.Year, 8, 10), ChestAtlas["catNormal"], ChestAtlas["catSpecial"], ChestAtlas["catBig"], ChestAtlas["catBottomless"]));
        foreach (var item in ChestVariants)
        {
            if (DateTime.Now >= item.StartDate && DateTime.Now <= item.EndDate)
            {
                seasonalChest = item; break;
            }
        }
    }

    public override void Load()
    {
        MyTreasueChest.Load();

    
    }

    public override void Unload()
    {
        MyTreasueChest.Unload();
    }
}

