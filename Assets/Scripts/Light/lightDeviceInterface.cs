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
    public GameObject targetBay;
    public dial colorDialRed, colorDialGreen, colorDialBlue, maxIntensityDial;
    public dial locationDialX, locationDialY, locationDialZ;
    public dial rotationDialX, rotationDialY, rotationDialZ;
    public float intensityMultiplier = 2.0f;
    public float maxIntensity = 5f;
    public float movementMultiplier = 100f;
    omniJack input;
    signalGenerator externalPulse;
    float[] lastPlaySig;

    bool activated = false;
    // current values
    float colorPercentRed, colorPercentGreen, colorPercentBlue;
    float locationX, locationY, locationZ;
    float rotationX, rotationY, rotationZ;

    Vector3 originalPosition;

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
        originalPosition = targetBay.transform.position;
    }

    void OnDestroy()
    {

    }

    void Update()
    {
        if (input.signal != externalPulse) externalPulse = input.signal;

        //**TODO implement input overrides for dials from sound source

        if (colorPercentRed != colorDialRed.percent) UpdateColor();
        if (colorPercentBlue != colorDialBlue.percent) UpdateColor();
        if (colorPercentGreen != colorDialGreen.percent) UpdateColor();

        if (locationX != locationDialX.percent) UpdateLocation();
        if (locationY != locationDialY.percent) UpdateLocation();
        if (locationZ != locationDialZ.percent) UpdateLocation();

        if (rotationX != rotationDialX.percent) UpdateRotation();
        if (rotationY != rotationDialY.percent) UpdateRotation();
        if (rotationZ != rotationDialZ.percent) UpdateRotation();

        float newIntensity = vol * intensityMultiplier;
        if (newIntensity > maxIntensityDial.percent * maxIntensity) newIntensity = maxIntensityDial.percent * maxIntensity;
        targetLight.intensity = newIntensity;
    }

    void UpdateColor()
    {
        colorPercentRed = colorDialRed.percent;
        colorPercentGreen = colorDialGreen.percent;
        colorPercentBlue = colorDialBlue.percent;
        targetLight.color = new Color(colorPercentRed, colorPercentGreen, colorPercentBlue);
    }

    void UpdateLocation()
    {
        locationX = (locationDialX.percent * 100 * movementMultiplier) - 100 * movementMultiplier / 2;
        locationY = (locationDialY.percent * 100 * movementMultiplier) - 100 * movementMultiplier / 2;
        locationZ = (locationDialZ.percent * 100 * movementMultiplier) - 100 * movementMultiplier / 2;
        Vector3 newPosition = new Vector3(locationX, locationY, locationZ);
        targetBay.transform.localPosition = newPosition;
    }

    void UpdateRotation()
    {
        rotationX = (rotationDialX.percent * 360);
        rotationY = (rotationDialY.percent * 360);
        rotationZ = (rotationDialZ.percent * 360);
        Vector3 newPosition = new Vector3(rotationX, rotationY, rotationZ);
        targetBay.transform.localEulerAngles = newPosition;
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
        data.maxIntensity = maxIntensityDial.percent;
        data.locationX = locationDialX.percent;
        data.locationY = locationDialY.percent;
        data.locationZ = locationDialZ.percent;
        data.rotationX = rotationDialX.percent;
        data.rotationY = rotationDialY.percent;
        data.rotationZ = rotationDialZ.percent;
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
        locationDialX.percent = data.locationX;
        locationDialY.percent = data.locationY;
        locationDialZ.percent = data.locationZ;
        rotationDialX.percent = data.rotationX;
        rotationDialY.percent = data.rotationY;
        rotationDialZ.percent = data.rotationZ;
    }
}

public class LightrigData : InstrumentData
{
    public int inputID;
    public float colorPercentRed, colorPercentGreen, colorPercentBlue;
    public float maxIntensity;
    public float locationX, locationY, locationZ;
    public float rotationX, rotationY, rotationZ;
}
