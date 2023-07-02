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


public class MyLogo : Logo
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


    public static void ctor(On.TowerFall.Logo.orig_ctor orig, Logo self) 
    {
        var LogoData = DynamicData.For(self);
        orig(self);
        self.RemoveAll();
        var towerTarget = LogoData.Get<Vector2>("towerTarget");
        var coreTarget = LogoData.Get<Vector2>("towerTarget");
        if (MenuVariantsMod.MenuVariantModModule.Vanilla[MenuVariantModModule.Settings.MenuVariant] == true)
        {
            if (MenuVariantModModule.Settings.MenuVariant == 0)
            {
                title = new Image(TFGame.MenuAtlas["title/titleLetters"]);
                title.Origin = new Vector2(126f, 37f);
                titleLight = new Image(TFGame.MenuAtlas["title/titleLettersLight"]);
                titleLight.Origin = new Vector2(126f, 37f);
                arrow = new Image(TFGame.MenuAtlas["title/bigArrow"]);
                arrow.Origin = new Vector2(156f, 24f);
                bg = new Image(TFGame.MenuAtlas["title/titleBg"]);
                bg.Origin = new Vector2(141f, 60f);
                ascension = new Image(TFGame.MenuAtlas["title/ascension"]);
                ascension.Position = towerTarget;
                ascension.Origin = new Vector2(60f, 15f);
                ascensionCore = new Sprite<int>(TFGame.MenuAtlas["title/ascensionCoreAnimation"], 59, 55);
                ascensionCore.Add(0, 0.1f, 0, 1, 2, 3, 4, 5, 6, 7);
                ascensionCore.Play(0);
                ascensionCore.Position = coreTarget;
                ascensionCore.CenterOrigin();
                letters = new Image[9];
                lettersY = new float[9];
                for (int i = 0; i < letters.Length; i++)
                {
                    letters[i] = new Image(TFGame.MenuAtlas["title/ascensionLetters/" + i]);
                    letters[i].CenterOrigin();
                    letters[i].Color = Color.Transparent;
                }
                LogoData.Set("title", title);
                LogoData.Set("titleLight", titleLight);
                LogoData.Set("arrow", arrow);
                LogoData.Set("bg", bg);
                LogoData.Set("ascension", ascension);
                LogoData.Set("ascensionCore", ascensionCore);
                LogoData.Set("letters", letters);
                LogoData.Set("lettersY", lettersY);
                self.Add(bg, ascension, ascensionCore, arrow, title, titleLight);
                self.Add(letters);
                LogoData.Set("darkWorldMode", false);
            }
            if (MenuVariantModModule.Settings.MenuVariant == 1)
            {
                title = new Image(TFGame.MenuAtlas["title/darkWorld/title"]);
                title.Origin = new Vector2(126f, 37f);
                titleLight = new Image(TFGame.MenuAtlas["title/darkWorld/titleLight"]);
                titleLight.Origin = new Vector2(126f, 37f);
                arrow = new Image(TFGame.MenuAtlas["title/darkWorld/arrow"]);
                arrow.Origin = new Vector2(156f, 24f);
                bg = new Image(TFGame.MenuAtlas["title/darkWorld/bg"]);
                bg.Origin = new Vector2(148f, 74f);
                ascension = new Image(TFGame.MenuAtlas["title/darkWorld/towerBase"]);
                ascension.Position = towerTarget;
                ascension.Origin = new Vector2(58f, 2f);
                ascensionCore = new Sprite<int>(TFGame.MenuAtlas["title/darkWorld/towerCore"], 55, 83);
                ascensionCore.Add(0, 0.1f, 0, 1, 2, 3, 4, 5, 6, 7, 8);
                ascensionCore.Play(0);
                ascensionCore.Position = coreTarget;
                ascensionCore.CenterOrigin();
                ascensionCore.Origin.X = (float)Math.Round(ascensionCore.Origin.X);
                var darkGlow = new Image(TFGame.MenuAtlas["title/darkWorld/darkGlow"]);
                darkGlow.Color = Color.Transparent;
                darkGlow.Origin = new Vector2(76f, -27f);
                var worldGlow = new Image(TFGame.MenuAtlas["title/darkWorld/worldGlow"]);
                worldGlow.Color = Color.Transparent;
                worldGlow.Origin = new Vector2(-21f, -27f);
                var dark = new Image(TFGame.MenuAtlas["title/darkWorld/dark"]);
                dark.Color = Color.Transparent;
                dark.Origin = new Vector2(76f, -28f);
                var world = new Image(TFGame.MenuAtlas["title/darkWorld/world"]);
                world.Color = Color.Transparent;
                world.Origin = new Vector2(-22f, -28f);
                LogoData.Set("title", title);
                LogoData.Set("titleLight", titleLight);
                LogoData.Set("arrow", arrow);
                LogoData.Set("bg", bg);
                LogoData.Set("ascension", ascension);
                LogoData.Set("ascensionCore", ascensionCore);
                LogoData.Set("dark", dark);
                LogoData.Set("world", world);
                self.Add(bg, ascension, ascensionCore, arrow, title, titleLight, dark, world, darkGlow, worldGlow);
                LogoData.Set("darkWorldMode", true);
            }
            var lettersSine = new SineWave(120);
            self.Add(lettersSine);
            lightSine = new SineWave(240);
            self.Add(lightSine);
            var isIn = true;
            var isLettersIn = false;
            var tweenMult = 1f;
            LogoData.Set("lettersSine", lettersSine);
            LogoData.Set("lightSine", lightSine);
            LogoData.Set("isIn", isIn);
            LogoData.Set("isLettersIn", isLettersIn);
            LogoData.Set("tweenMult", tweenMult);
            return;
        }
        else
        {
            var LoadedLogo = LogoLoad.LogoList[MenuVariantModModule.Settings.MenuVariant-2];
            var atlas = LoadedLogo.Atlas;
            if (LoadedLogo.VanillaTitle == true)
            {
                title = new Image(TFGame.MenuAtlas[LoadedLogo.Title]);
                titleLight = new Image(TFGame.MenuAtlas[LoadedLogo.TitleLight]);
            }
            else
            {
                Console.WriteLine(LoadedLogo.Title);
                title = new Image(atlas[LoadedLogo.Title]);
                titleLight = new Image(atlas[LoadedLogo.TitleLight]);
            }
            title.Origin = new Vector2(126f, 37f);
            titleLight.Origin = new Vector2(126f, 37f);
            if (LoadedLogo.VanillaArrow == true)
            {
                arrow = new Image(TFGame.MenuAtlas[LoadedLogo.Arrow]);
            }
            else
            {
                arrow = new Image(atlas[LoadedLogo.Arrow]);
            }
            arrow.Origin = new Vector2(156f, 24f);
            if (LoadedLogo.VanillaBg == true)
            {
               bg = new Image(TFGame.MenuAtlas[LoadedLogo.Bg]);
               bg.Origin = new Vector2(148f, 74f);
            }
            else
            {
                bg = new Image(atlas[LoadedLogo.Bg]);
                bg.Origin = new Vector2(LoadedLogo.BgOrigin.X, LoadedLogo.BgOrigin.Y);
            }
            if (LoadedLogo.VanillaAscension == true)
            {
                ascension = new Image(TFGame.MenuAtlas[LoadedLogo.Ascension]);
            }
            else
            {
                ascension = new Image(atlas[LoadedLogo.Ascension]);
            }
            ascension.Position = towerTarget;
            ascension.Origin = new Vector2(58f, 2f);
            ascensionCore = new Sprite<int>(TFGame.MenuAtlas["title/ascensionCoreAnimation"], 59, 55);
            ascensionCore.Add(0, 0.1f, 0, 1, 2, 3, 4, 5, 6, 7);
            ascensionCore.Play(0);
            ascensionCore.Position = coreTarget;
            ascensionCore.CenterOrigin();
            ascensionCore.Origin.X = (float)Math.Round(ascensionCore.Origin.X);
            letters = new Image[0];
            lettersY = new float[0];
            LogoData.Set("title", title);
            LogoData.Set("titleLight", titleLight);
            LogoData.Set("arrow", arrow);
            LogoData.Set("bg", bg);
            LogoData.Set("ascension", ascension);
            LogoData.Set("ascensionCore", ascensionCore);
            LogoData.Set("darkWorldMode", false);
            LogoData.Set("letters", letters);
            LogoData.Set("lettersY", lettersY);
            self.Add(bg, ascension, ascensionCore, arrow, title, titleLight);
            self.Add(letters);
            lightSine = new SineWave(240);
            self.Add(lightSine);
            var isIn = true;
            var isLettersIn = false;
            var tweenMult = 1f;
            var lettersSine = new SineWave(120);
            LogoData.Set("lettersSine", lettersSine);
            LogoData.Set("lightSine", lightSine);
            LogoData.Set("isIn", isIn);
            LogoData.Set("isLettersIn", isLettersIn);
            LogoData.Set("tweenMult", tweenMult);
        }
    }
}
