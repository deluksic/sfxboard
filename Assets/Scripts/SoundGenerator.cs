using UnityEngine;
using System.Collections;
using System;

public class SoundGenerator : MonoBehaviour
{

    public bool Active;
    public int TouchID;
    public float BaseFreq;
    public float Attack;
    public float Decay;

    public Func<double, double> WaveFunction;
    public float Period;

    private int sampleRate;
    private double t, freq, phase, inc, amplitude;

    public Func<double, double, double> FreqFunction;
    private Vector2 currentFreqLocation;
    private Vector2 nextFreqLocation;

    void Start()
    {
        t = 0;
        phase = 0;
        sampleRate = AudioSettings.outputSampleRate;
        inc = 1f / sampleRate;
    }

    void Update()
    {
        Active =
            ((TouchID == -1 && Input.GetAxis("Fire1") > 0) ||
            (TouchID < Input.touchCount && TouchID != -1));

        if (TouchID == -1 && Active)
        {
            // this soundgenerator should be controlled by mouse
            var mousex = Input.mousePosition.x;
            var mousey = Input.mousePosition.y;
            mousex /= Screen.width;
            mousey /= Screen.height;
            nextFreqLocation = new Vector2(mousex, mousey);
            if (Input.GetMouseButtonDown(0))
                currentFreqLocation = nextFreqLocation;
        }
        else if (Active)
        {
            // this generator is controlled by finger
            var touchx = Input.touches[TouchID].position.x;
            var touchy = Input.touches[TouchID].position.y;
            touchx /= Screen.width;
            touchy /= Screen.height;
            nextFreqLocation = new Vector2(touchx, touchy);
            if (Input.touches[TouchID].phase == TouchPhase.Began)
                currentFreqLocation = nextFreqLocation;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (FreqFunction == null || WaveFunction == null || Period == 0) return;

        var vectorinc = (nextFreqLocation - currentFreqLocation) / data.Length;
        var ampinc = (Active ? Attack : Decay) / data.Length;
        var amptarget = Active ? 0.2f : 0;
        for (int i = 0; i < data.Length; i += 1)
        {
            calculateFreqAndPhase();
            amplitude = Mathf.Lerp((float)amplitude, amptarget, ampinc);
            data[i] += (float)(WaveFunction(t * freq + phase) * amplitude);
            t += inc;
            currentFreqLocation += vectorinc;
        }

        t %= Period;
    }

    private double getFreq(Vector2 location)
    {
        return FreqFunction(location.x, location.y) * BaseFreq * Period;
    }

    private void calculateFreqAndPhase()
    {
        double nextFreq = getFreq(currentFreqLocation);
        double curr = (t * freq + phase);
        double next = (t * nextFreq);
        phase = (curr - next) % Period;
        freq = nextFreq;
    }
}
