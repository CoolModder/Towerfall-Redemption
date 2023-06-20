using Monocle;
using FortRise;
using TowerFall;

namespace MenuVariantsMod;

public class ModSettings : ModuleSettings 
{

    [SettingsNumber(0, 1, 1)]
    public int MenuVariant;
    [SettingsName("For custom menus, go to TowerFall/Saves/")]
    public bool Useless;
    [SettingsName("com.CoolModder.MenuVariantsMod! Modify the file inside!")]
    public bool Notice;
}

public static class CommandList 
{
    [Command("TowerSus")]
    public static void SayHello(string[] args) 
    {
        Engine.Instance.Commands.Log("You were arrowed. 2 impostors remaining.");
    }
}