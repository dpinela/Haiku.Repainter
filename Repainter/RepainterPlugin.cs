global using System;
using Bep = BepInEx;
using Reflection = System.Reflection;
using MM = MonoMod.RuntimeDetour;
using UE = UnityEngine;
using USM = UnityEngine.SceneManagement;

namespace Haiku.Repainter
{
    [Bep.BepInPlugin("haiku.repainter", "Haiku Repainter", "1.0.0.0")]
    [Bep.BepInDependency("haiku.mapi", "1.0")]
    public class RepainterPlugin : Bep.BaseUnityPlugin
    {
        public void Start()
        {
            modSettings = new(Config);

            UE.Camera.onPostRender += Repaint;
            // MAPI doesn't have a predefined hook for StartNewGame yet.
            new MM.Hook(
                typeof(MainMenuManager).GetMethod("StartNewGame", 
                    Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public),
                ApplyPaletteOnStart);
            On.PCSaveManager.Load += LoadSaveData;
            On.PCSaveManager.Save += StoreSaveData;
        }

        private Settings? modSettings;
        private SaveData? saveData;

        private void ApplyPaletteOnStart(Action<MainMenuManager, string> orig, MainMenuManager self, string mode)
        {
            orig(self, mode);
            try
            {
                if (modSettings!.ApplyOnStart.Value)
                {
                    var seed = modSettings!.Seed.Value;
                    if (seed == "")
                    {
                        // insert a random seed here
                    }
                    saveData = new(seed);
                    areaPalettes = Palette.GenerateN(NumPaletteAreas, new(seed));
                }
                else
                {
                    saveData = null;
                    areaPalettes = null;
                }
            }
            catch (Exception err)
            {
                Logger.LogError(err.ToString());
            }
        }

        private void LoadSaveData(On.PCSaveManager.orig_Load orig, PCSaveManager self, string filePath)
        {
            orig(self, filePath);
            try
            {
                saveData = SaveData.Load(self.es3SaveFile);
                if (saveData != null)
                {
                    if (saveData.Algorithm == 1)
                    {
                        areaPalettes = Palette.GenerateN(NumPaletteAreas, new(saveData.Seed));
                    }
                    else
                    {
                        Logger.LogError($"save file contains unknown palette algorithm {saveData.Algorithm}; ignoring palette data");
                    }
                }
            }
            catch (Exception err)
            {
                Logger.LogError(err.ToString());
            }
        }

        private void StoreSaveData(On.PCSaveManager.orig_Save orig, PCSaveManager self, string filePath)
        {
            orig(self, filePath);
            if (saveData == null)
            {
                return;
            }
            try
            {
                saveData.SaveTo(self.es3SaveFile);
            }
            catch (Exception err)
            {
                Logger.LogError(err.ToString());
            }
        }

        // Palette areas:
        //
        // 0: Abandoned Wastes
        // 1: Sunken Wastes
        // 2: Research Lab
        // 3: Last Bunker
        // 4: Central Core
        // 5: Incinerator Burner
        // 6: Pinion's Expanse
        // 7: Factory Facility
        // 8: Blazing Furnace
        // 9: Ruined Surface
        // 10: Water Ducts
        // 11: Forgotten Ruins
        // 12: Traveling Town
        // 13: Mainframe Vault
        private const int NumPaletteAreas = 14;
        private Palette[]? areaPalettes;

        // Scene 160 is the Sunken Wastes/Research Lab transition
        // Scene 153 is the Sunken Wastes/Forgotten Ruins transition
        // These could be reasonably assigned to either area

        private static readonly sbyte[] paletteAreasByScene = new sbyte[]
        {
            /* 000 */ -1, -1, -1, -1, -1, -1, -1, -1, -1, 12,
            /* 010 */ 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            /* 020 */ 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            /* 030 */ 3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
            /* 040 */ 3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
            /* 050 */ 3,  3,  3,  3,  3,  3,  3,  3,  3,  3,
            /* 060 */ 3,  4,  4,  4,  4,  4,  4,  4,  4,  4,
            /* 070 */ 4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
            /* 080 */ 4,  4,  4,  4,  4,  4,  6,  6,  6,  6,
            /* 090 */ 6,  6,  6,  6,  6,  6,  6,  6,  6,  6,
            /* 100 */ 6,  5,  5,  5,  5,  5,  5,  5,  5,  5,
            /* 110 */ 5,  10, 10, 10, 10, 10, 10, 10, 10, 10,
            /* 120 */ 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
            /* 130 */ 10, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            /* 140 */ 11, 11, -1, 11, 11, 11, 1,  11, -1, -1,
            /* 150 */ -1, 1,  1,  1,  1,  1,  1,  1,  1,  2,
            /* 160 */ 1,  1,  1,  2,  2,  1,  0,  4,  4,  4,
            /* 170 */ 4,  7,  7,  7,  7,  7,  7,  7,  7,  7,
            /* 180 */ 7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
            /* 190 */ 7,  7,  7,  7,  7,  7,  9,  9,  9,  7,
            /* 200 */ 8,  3,  4,  3,  3,  13, 10, 10, 10, 10,
            /* 210 */ 10, 11, 7,  8,  8,  8,  8,  8,  8,  -1,
            /* 220 */ 4,  7,  3,  10, 9,  11, -1, 11, 4,  7,
            /* 230 */ 7,  7,  8,  7
        };

        private static int CurrentPaletteArea()
        {
            var s = USM.SceneManager.GetActiveScene();
            if (s == null)
            {
                return -1;
            }
            var n = s.buildIndex;
            return n < paletteAreasByScene.Length ? paletteAreasByScene[n] : -1;
        }

        private void Repaint(UE.Camera cam)
        {
            if (cam == UE.Camera.main && areaPalettes != null)
            {
                try
                {
                    var area = CurrentPaletteArea();
                    if (area != -1)
                    {
                        Recolor(cam.activeTexture, areaPalettes[area]);
                    }
                }
                catch (Exception err)
                {
                    Logger.LogError(err.ToString());
                }
            }
        }

        private UE.Texture2D? buffer;

        private UE.Texture2D GetBuffer(int width, int height)
        {
            if (buffer != null && buffer.width == width && buffer.height == height)
            {
                return buffer;
            }
            buffer = new(width, height, UE.TextureFormat.RGBA32, false);
            return buffer;
        }

        private void Recolor(UE.RenderTexture target, Palette pal)
        {
            var buf = GetBuffer(target.width, target.height);
            var act = UE.RenderTexture.active;
            UE.RenderTexture.active = target;
            buf.ReadPixels(new(0, 0, target.width, target.height), 0, 0, false);
            UE.RenderTexture.active = act;
            var pixels = buf.GetRawTextureData<UE.Color32>();
            for (var i = 0; i < buf.width * buf.height; i++)
            {
                // Convert to YCbCr space, change the colour parameters, then convert back to RGB.
                // For more efficiency, we could precompute the following matrix
                // and multiply it by (r, g, b):
                // 
                // .299-.237Yb+.701Yr ;  .587-.464Yb-.587Yr ;  .114+.701Yb-.114Yr
                // .299+.058Xb-.17207Xr+.120Yb-.35707Yr ;  .587+.114Xb+.144Xr+.237Yb+.299Yr ;  .114-.17207Xb+.028Xr-.35707Yb+.058Yr
                // .299-.299Xb+.886Xr ;  .587-.587Xb-.742Xr ;  .114+.886Xb-.144Yr
                var p = pixels[i];
                var y = .299 * p.r + .587 * p.g + .114 * p.b;
                var pb = -.1687 * p.r - .3313 * p.g + .5 * p.b;
                var pr = .5 * p.r - .4187 * p.g - .0813 * p.b;
                var pb2 = pal.Xb * pb + pal.Xr * pr;
                var pr2 = pal.Yb * pb + pal.Yr * pr;
                var r2 = Clamp(y + 1.402 * pr2);
                var g2 = Clamp(y - .34414 * pb2 - .71414 * pr2);
                var b2 = Clamp(y + 1.772 * pb2);
                pixels[i] = new(r2, g2, b2, p.a);
            }
            buf.Apply();
            UE.Graphics.CopyTexture(buf, target);
        }

        private static byte Clamp(double x) => x switch
        {
            < 0 => 0,
            > 255 => 255,
            _ => (byte)x
        };
    }
}
