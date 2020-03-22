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
using System.Runtime.InteropServices;

public class timelineTrackSignalGenerator : signalGenerator {

  bool newSignal = false;
  bool signalOn = false;
    public List<splitterNodeSignalGenerator> nodes;
    timelineTrackComponentInterface _interface;
  public signalGenerator incoming;
  const int MAX_COUNT = 16;
  float[][] mergeBuffers;

    [DllImport("SoundStageNative")]
  public static extern void SetArrayToSingleValue(float[] a, int length, float val);

  [DllImport("SoundStageNative")]
  public static extern bool GetBinaryState(float[] buffer, int length, int channels, ref float lastBuf);

  [DllImport("SoundStageNative")]
  public static extern bool IsPulse(float[] buffer, int length);

  [DllImport("SoundStageNative")]
  public static extern void AddArrays(float[] a, float[] b, int length);

 [DllImport("SoundStageNative")]
  public static extern void CopyArray(float[] a, float[] b, int length);

    public override void Awake() {
    base.Awake();
        //Added from Splitter   
        mergeBuffers = new float[MAX_COUNT][];
        for (int i = 0; i < MAX_COUNT; ++i)
        {
            mergeBuffers[i] = new float[MAX_BUFFER_LENGTH];
        }
        _interface = GetComponent<timelineTrackComponentInterface>();
  }

  public void setSignal(bool on) //from outside
  {
    newSignal = true;
    signalOn = on;
  }

  float lastBuffer = 0;
  public override void processBuffer(float[] buffer, double dspTime, int channels) {

    if (incoming != null) {
      incoming.processBuffer(buffer, dspTime, channels);

      if (IsPulse(buffer, buffer.Length)) {
        _interface.updateSignal(false, timelineTrackComponentInterface.sigSource.signal);
        _interface.updateSignal(true, timelineTrackComponentInterface.sigSource.signal);
      } else {
        bool on = GetBinaryState(buffer, buffer.Length, channels, ref lastBuffer);
        _interface.updateSignal(on, timelineTrackComponentInterface.sigSource.signal);
      }
    }
        // added from splitter
        int count = nodes.Count;
        for (int i = 0; i < count; i++)
        {
            if (buffer.Length != mergeBuffers[i].Length)
                System.Array.Resize(ref mergeBuffers[i], buffer.Length);

            SetArrayToSingleValue(mergeBuffers[i], buffer.Length, 0.0f);

            if (i < nodes.Count)
            {
                if (nodes[i] != null) nodes[i].processBuffer(mergeBuffers[i], dspTime, channels);
            }
        }
        for (int i = 0; i < count; i++) AddArrays(buffer, mergeBuffers[i], buffer.Length);
        
       float val = signalOn ? 1.0f : -1.0f;
    SetArrayToSingleValue(buffer, buffer.Length, val);

    if (newSignal) {
      buffer[0] = buffer[1] = -1;
      newSignal = false;
    }
  }
}

