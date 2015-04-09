using UnityEngine;
using System.Collections;
using System;

public class BoardController : MonoBehaviour
{

    BoardTextureManager textureManager;
    SoundGenerator[] soundGenerators;

    void Start()
    {
        Func<double, double, double> function = keyboard;

        if ((textureManager = GetComponentInChildren<BoardTextureManager>()) == null)
            Debug.LogError("No BoardTextureManager found on Board object.");

        soundGenerators = GetComponentsInChildren<SoundGenerator>();
        foreach (var generator in soundGenerators)
        {
            generator.FreqFunction = function;
            generator.WaveFunction = cutewave;
            generator.Period = 2 * Mathf.PI;
        }

        textureManager.PaintTexture(function, new Color[] 
        {
            Color.black,
            Color.yellow
        });
    }

    double test(double x, double y)
    {
        return toKeySpace(x * 100 + 10 * x * Math.Sin(500 * y * y * y));
    }

    double radsquare(double x, double y)
    {
        x -= 0.5;
        y -= 0.5;
        return toKeySpace(20 * (x * x + y * y) + 1);
    }

    double keyboard(double x, double y)
    {
        x = 26 * x;
        y = Math.Max(10 * y - 1, 0);
        var xx = x % 2 - 1;
        return toKeySpace(2 * Math.Ceiling(x / 2) + Math.Sign(xx) * Math.Pow(Math.Abs(xx), (2 * y * y + 1)));
    }

    double twlwrooth2 = 1.059463094359;
    double toKeySpace(double x)
    {
        return Math.Pow(twlwrooth2, x * 0.5f);
    }

    double cutewave(double x)
    {
        x = Math.Sin(x);
        return Math.Sign(x) * Math.Pow(x * x, 0.2);
    }
}
