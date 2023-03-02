global using System;
using Bep = BepInEx;
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
            Logger.LogInfo("Repainter - Under Construction");
            UE.Camera.onPostRender += Repaint;
        }

        private void Repaint(UE.Camera cam)
        {
            if (cam == UE.Camera.main)
            {
                try
                {
                    Recolor(cam.activeTexture);
                }
                catch (Exception err)
                {
                    Logger.LogError(err.ToString());
                }
            }
        }

        private static UE.Color Pinkify(UE.Color c)
        {
            UE.Color.RGBToHSV(c, out var h, out var s, out var v);
            var p = UE.Color.HSVToRGB(.83f, s, v);
            p.a = c.a;
            return p;
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

        private bool loggedTargetMismatch;

        private void Recolor(UE.RenderTexture target)
        {
            var buf = GetBuffer(target.width, target.height);
            var act = UE.RenderTexture.active;
            if (act != target)
            {
                if (!loggedTargetMismatch)
                {
                    Logger.LogInfo("active texture was not target");
                    loggedTargetMismatch = true;
                }
                UE.RenderTexture.active = target;
            }
            buf.ReadPixels(new(0, 0, target.width, target.height), 0, 0, false);
            UE.RenderTexture.active = act;
            var pixels = buf.GetRawTextureData<UE.Color32>();
            for (var y = 0; y < buf.height; y++)
            {
                for (var x = 0; x < buf.width; x++)
                {
                    var i = y * buf.width + x;
                    pixels[i] = Pinkify(pixels[i]);
                }
            }
            buf.Apply();
            UE.Graphics.CopyTexture(buf, target);
        }
    }
}
