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
    omniJack input;
    signalGenerator externalPulse;
    float[] lastPlaySig;

    bool activated = false;

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
        if (hits > 0)
        {
            if (hits % 2 != 0) togglelightPower(!lightPower);
            hits = 0;
        }
    }

    void OnDisable()
    {
        if (lightPower) togglelightPower(false);
    }

    bool lightPower = false;
    public void togglelightPower(bool on)
    {
        if (lightPower == on) return;
        lightPower = on;
        if (on)
        {
            targetLight.enabled = true;
        } else
        {
            targetLight.enabled = false;
        }
    }

    int hits = 0;
    private void OnAudioFilterRead(float[] buffer, int channels)
    {
        if (externalPulse == null) return;
        double dspTime = AudioSettings.dspTime;

        float[] playBuffer = new float[buffer.Length];
        externalPulse.processBuffer(playBuffer, dspTime, channels);

        hits += CountPulses(playBuffer, buffer.Length, channels, lastPlaySig);
    }


    public override InstrumentData GetData()
    {
        CameraData data = new CameraData();
        data.deviceType = menuItem.deviceType.Camera;
        GetTransformData(data);
        data.inputID = input.transform.GetInstanceID();
        return data;
    }

    public override void Load(InstrumentData d)
    {
        LightData data = d as LightData;
        base.Load(data);
        input.ID = data.inputID;
    }
}

public class LightData : InstrumentData
{
    public int inputID;
}
