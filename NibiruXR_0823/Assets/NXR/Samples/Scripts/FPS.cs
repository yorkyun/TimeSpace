// Copyright 2016 Nibiru. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Nxr.Internal;
using UnityEngine;

namespace NXR.Samples
{
    [RequireComponent(typeof(TextMesh))]
    public class FPS : MonoBehaviour
    {
        private TextMesh textField;
        private float fps = 60;

        void Start()
        {
            textField = GetComponent<TextMesh>();

            //// change keyboard postion or rotation
            // NibiruKeyBoard.Instance.keyBoardTransform.Rotate(new Vector3(30, 0, 0));
            // // show keyboard
            // NibiruKeyBoard.Instance.Show();

            NxrViewer.serviceReadyUpdatedDelegate += OnServiceReady;
        }

        void OnServiceReady(SERVICE_TYPE serviceType, bool isConnectedSucc)
        {
            Debug.Log("OnServiceReady---------------------" + serviceType + "," + isConnectedSucc);
            if (serviceType == SERVICE_TYPE.GESTURE)
            {
                bool supportGesture = NxrViewer.Instance.GetNibiruService().IsSupportGesture();
                Debug.Log("Gesture Support Status : " + (supportGesture ? 1 : 0));
            }
            else if (serviceType == SERVICE_TYPE.SIX_DOF)
            {
                bool support6DOF = NxrViewer.Instance.GetNibiruService().IsSupport6DOF();
                Debug.Log("Six Dof Support Status : " + (support6DOF ? 1 : 0));
            }
            else if (serviceType == SERVICE_TYPE.VOICE)
            {
                bool supportVoice = NxrViewer.Instance.GetNibiruService().IsSupportVoice();
                Debug.Log("Voice Support Status : " + (supportVoice ? 1 : 0));
            }
        }



        private int lastFPS = -1;
        void Update()
        {
            int fps = calculateFPS();
            if (fps != lastFPS)
            {
                string text = " FPS: " + fps + " fps";
                if (textField != null)
                {
                    textField.text = text;
                }
            }
        }

        private int calculateFPS()
        {
            float interp = Time.deltaTime / (0.5f + Time.deltaTime);
            float currentFPS = 1.0f / Time.deltaTime;
            fps = Mathf.Lerp(fps, currentFPS, interp);
            return Mathf.RoundToInt(fps);
        }

        private void OnDestroy()
        {
            NxrViewer.serviceReadyUpdatedDelegate -= OnServiceReady;
            Debug.Log("FPS.OnDestroy");
        }
    }
}