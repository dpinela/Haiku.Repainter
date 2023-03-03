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
            var rng = new Random();
            kA = rng.NextDouble();
            kB = rng.NextDouble();
            Logger.LogInfo($"kA={kA}, kB={kB}");
        }

        private double kA, kB;

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

        private void Recolor(UE.RenderTexture target)
        {
            var buf = GetBuffer(target.width, target.height);
            var act = UE.RenderTexture.active;
            UE.RenderTexture.active = target;
            buf.ReadPixels(new(0, 0, target.width, target.height), 0, 0, false);
            UE.RenderTexture.active = act;
            var pixels = buf.GetRawTextureData<UE.Color32>();
            for (var i = 0; i < buf.width * buf.height; i++)
            {
                var p = pixels[i];
                var y = .299 * p.r + .587 * p.g + .114 * p.b;
                var pb = -.1687 * p.r - .3313 * p.g + .5 * p.b;
                var pr = .5 * p.r - .4187 * p.g - .0813 * p.b;
                var pb2 = kA * pb + (1 - kA) * pr;
                var pr2 = kB * pb + (1 - kB) * pr;
                var r2 = (byte)(y + 1.402 * pr2);
                var g2 = (byte)(y - .34414 * pb2 - .71414 * pr2);
                var b2 = (byte)(y + 1.772 * pb2);
                pixels[i] = new(r2, g2, b2, p.a);
            }
            buf.Apply();
            UE.Graphics.CopyTexture(buf, target);
        }
    }
}
