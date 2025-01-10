using Monocle;
using FortRise;

namespace MenuVariantsMod;

public class ModSettings : ModuleSettings 
{
    public int MenuVariant;
    public int BezelVariant;
}

public static class CommandList 
{
    [Command("TowerSus")]
    public static void SayHello(string[] args) 
    {
        Engine.Instance.Commands.Log("You were arrowed. 2 impostors remaining.");
    }
}