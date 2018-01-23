// Copyright 2017 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class lightDeviceInterface : deviceInterface
{
    public Light targetLight;
    public dial colorDialRed, colorDialGreen, colorDialBlue, maxIntensityDial;
    public float intensityMultiplier = 2.0f;
    public float maxIntensity = 5f;
    public float movementMultiplier = 0.1f;
    omniJack input;
    signalGenerator externalPulse;
    float[] lastPlaySig;

    bool activated = false;
    // current values
    float colorPercentRed, colorPercentGreen, colorPercentBlue;

    private Vector3 startPos;
    [DllImport("SoundStageNative")]
    public static extern int CountPulses(float[] buffer, int length, int channels, float[] lastSig);

    public override void Awake()
    {
        base.Awake();
        lastPlaySig = new float[] { 0, 0 };
        input = GetComponentInChildren<omniJack>();
    }

    void Start()
    {

    }

    void OnDestroy()
    {

    }

    void Update()
    {
        if (input.signal != externalPulse) externalPulse = input.signal;
        if (colorPercentRed != colorDialRed.percent) UpdateColor();
        if (colorPercentBlue != colorDialBlue.percent) UpdateColor();
        if (colorPercentGreen != colorDialGreen.percent) UpdateColor();

        float newIntensity = vol * intensityMultiplier;
        if (newIntensity > maxIntensityDial.percent * maxIntensity) newIntensity = maxIntensityDial.percent * maxIntensity;
        targetLight.intensity = newIntensity;

        Vector3 rotation = new Vector3(vol * movementMultiplier, 0, 0);
        //targetLight.transform.Rotate(rotation);
    }

    void UpdateColor()
    {
        colorPercentRed = colorDialRed.percent;
        colorPercentGreen = colorDialGreen.percent;
        colorPercentBlue = colorDialBlue.percent;
        targetLight.color = new Color(colorPercentRed, colorPercentGreen, colorPercentBlue);
    }

    void OnDisable()
    {

    }

    int hits = 0;
    float vol = 0;
    private void OnAudioFilterRead(float[] buffer, int channels)
    {
        if (externalPulse == null) return;
        double dspTime = AudioSettings.dspTime;


        float[] playBuffer = new float[buffer.Length];
        externalPulse.processBuffer(playBuffer, dspTime, channels);

        int i = 0;
        int samples = 0;
        float total = 0;
        vol = 0;
        while (i < playBuffer.Length / channels)
        {
            float temp = playBuffer[i];
            if (temp !=0)
            {
                samples++;
                total += playBuffer[i];

            }
            i += channels;
        }
        if (samples!=0) vol = total;
    }


    public override InstrumentData GetData()
    {
        LightrigData data = new LightrigData();
        data.deviceType = menuItem.deviceType.Lightrig;
        GetTransformData(data);
        data.inputID = input.transform.GetInstanceID();
        data.colorPercentRed = colorDialRed.percent;
        data.colorPercentGreen = colorDialGreen.percent;
        data.colorPercentBlue = colorDialBlue.percent;
        return data;
    }

    public override void Load(InstrumentData d)
    {
        LightrigData data = d as LightrigData;
        base.Load(data);
        input.ID = data.inputID;

        colorDialRed.percent = data.colorPercentRed;
        colorDialGreen.percent = data.colorPercentGreen;
        colorDialBlue.percent = data.colorPercentBlue;
        maxIntensityDial.percent = data.maxIntensity;
    }
}

public class LightrigData : InstrumentData
{
    public int inputID;
    public float colorPercentRed, colorPercentGreen, colorPercentBlue;
    public float maxIntensity;
}
