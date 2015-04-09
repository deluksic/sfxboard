using UnityEngine;
using System.Collections;
using System;

public class BoardTextureManager : MonoBehaviour
{
    public MeshRenderer BoardTextureRenderer;

    public void Start()
    {
        transform.localScale = new Vector3(2 * Camera.main.aspect, 2, 1) * Camera.main.orthographicSize;
    }

    public void PaintTexture(Func<double, double, double> colorFunc, Color[] color)
    {
        Texture2D texture = new Texture2D
        (
            Screen.width,
            Screen.height,
            TextureFormat.ARGB32,
            false
        );

        var min = float.NaN;
        var max = float.NaN;
        var colorvar = 0f;
        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            {
                colorvar = (float)colorFunc((float)x / texture.width, (float)y / texture.height);
                if (colorvar > max || float.IsNaN(max)) max = colorvar;
                if (colorvar < min || float.IsNaN(min)) min = colorvar;
            }

        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
            {
                colorvar = (float)colorFunc((float)x / texture.width, (float)y / texture.height);
                texture.SetPixel(x, y, getColorGradient(color, colorvar, min, max));
            }

        texture.Apply(false);

        BoardTextureRenderer.material.SetTexture(0, texture);
    }

    Color getColorGradient(Color[] colors, double x, double from, double to)
    {
        x = (x - from) / (to - from);
        int baseid = Mathf.Clamp((int)(x * (colors.Length - 1)), 0, colors.Length - 1);
        int nextid = Mathf.Clamp(baseid + 1, 0, colors.Length - 1);
        return Color.Lerp(colors[baseid], colors[nextid], (float)x * (colors.Length - 1) % 1);
    }
}
