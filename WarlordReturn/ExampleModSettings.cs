using Monocle;
using FortRise;
using TowerFall;

namespace Warlord;

public class ExampleModSettings : ModuleSettings 
{
    [SettingsName("Time to score!")]
    [SettingsNumber(5, 30, 1)]
    public int TimeToScore = 15;
}

public static class CommandList 
{
   
}