using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using MenuVariantsMod;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System.Diagnostics;


public class MyBezel : TFGame
{
    private static ConstructorInfo base_Logo;

    public static Image ascension;
    public static Image title;
    public static Image titleLight;
    public static Image arrow;
    public static Image bg;
    public static Sprite<int> ascensionCore;
    public static Image[] letters;
    public static float[] lettersY;

    public static float lightMult = 1f;
    public static SineWave lightSine;

    public MyBezel(bool noIntro) : base(noIntro)
    {
    }

    public static void MyLoad(On.TowerFall.TFGame.orig_LoadContent orig, TFGame self) 
    {
        orig(self);
        if (MenuVariantModModule.Settings.BezelVariant > 0)
        {
            var LoadedBezel = BezelLoad.BezelList[MenuVariantModModule.Settings.BezelVariant - 1];
            var atlas = LoadedBezel.Atlas;
            Screen.LeftImage = atlas[LoadedBezel.Left];
            Screen.RightImage = atlas[LoadedBezel.Right];
        }
    }
}
