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
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class lightController : signalGenerator
{

    public signalGenerator incoming;
    public Light targetLight;
    public bool lightSwitch = false;
    float[] lastPlaySig;

    [DllImport("SoundStageNative")]
    public static extern int CountPulses(float[] buffer, int length, int channels, float[] lastSig);

    public override void Awake()
    {
        base.Awake();
        lastPlaySig = new float[] { 0, 0 };
    }

    void Start()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (lightSwitch) { targetLight.enabled = true; } else { targetLight.enabled = false; }
    }

    int hits = 0;
    private void OnAudioFilterRead(float[] buffer, int channels)
    {
        if (incoming == null) { lightSwitch = false; return; }
        double dspTime = AudioSettings.dspTime;
        incoming.processBuffer(buffer, dspTime, channels);
        float[] playBuffer = new float[buffer.Length];
        hits += CountPulses(playBuffer, buffer.Length, channels, lastPlaySig);
        Debug.Log(hits);
        lightSwitch = true;
    }
}