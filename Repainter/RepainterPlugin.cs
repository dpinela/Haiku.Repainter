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
                var yb = (byte)y;
                pixels[i] = new(yb, yb, yb, p.a);
            }
            buf.Apply();
            UE.Graphics.CopyTexture(buf, target);
        }
    }
}
