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

public class realviewDeviceInterface : deviceInterface {

  public override void Awake() {
    base.Awake();
  }

  void Start() {

  }

  void OnDestroy() {

  }

  void Update() {

  }

  void OnDisable() {

  }

  public override void hit(bool on, int ID = -1) {

  }

  public override InstrumentData GetData() {
    RealviewData data = new RealviewData();
    data.deviceType = menuItem.deviceType.Realview;
    GetTransformData(data);
    return data;
  }

  public override void Load(InstrumentData d) {
     RealviewData data = d as RealviewData;
    base.Load(data);
  }
}

public class RealviewData : InstrumentData {

}
