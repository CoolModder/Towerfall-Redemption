using Monocle;
using MenuVariantsMod;

public class MyBezel 
{
    public static void MyLoad() 
    {
        if (MenuVariantModModule.Settings.BezelVariant > 0)
        {
            var LoadedBezel = BezelLoad.BezelList[MenuVariantModModule.Settings.BezelVariant - 1];
            var atlas = LoadedBezel.Atlas;
            Screen.LeftImage = atlas[LoadedBezel.Left];
            Screen.RightImage = atlas[LoadedBezel.Right];
        }
    }
}
