using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

namespace extOSC.Examples
{
    public class trackView : MonoBehaviour
    {
        #region OVR Initialization
        [SerializeField] public OVRCameraRig rig;

        public static string testerName = "p4_large_3";
        string path = "D:/testResult/" + testerName + ".txt";
        public bool turnOnBeam = false;
        public float timeRemaining = 5;
        public bool timerIsRunning = false;

        private LineRenderer beam;
        #endregion

        #region OSC Receiver Settings
        public string Address1 = "";
        public string Address2 = "";
        public string Address3 = "";
        public string Address4 = "";

        [Header("OSC Settings")]
        public OSCReceiver Receiver;
        public float data_gamma = 0f;
        public float data_beta = 0f;
        public float data_alpha = 0f;
        public float data_theta = 0f;
        #endregion

        void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(myLine, duration);
        }

        [MenuItem("Tools/Write file")]
        static void WriteString(Vector3 p1)
        {
            StreamWriter writer = new StreamWriter("D:/test1.txt");
            writer.WriteLine(p1);
            writer.Close();
        }
        void Start()
        {
            //string path = "D:/testResult/" + testerName + ".txt";

            Debug.Log("Start counting");
            timerIsRunning = true;

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "\n");
            }
            beam = this.gameObject.AddComponent<LineRenderer>();
            beam.startColor = Color.magenta;
            beam.endColor = Color.red;
            beam.startWidth = 0.04f;
            beam.endWidth = 0.01f;

            Receiver.Bind(Address1, ReceivedMessage1);
            Receiver.Bind(Address2, ReceivedMessage2);
            Receiver.Bind(Address3, ReceivedMessage3);
            Receiver.Bind(Address4, ReceivedMessage4);

            InvokeRepeating("TimerWrapper", 0, 1f);
        }

        #region Record position data and EEG data every second
        void TimerWrapper()
        {
            Vector3 start = rig.centerEyeAnchor.transform.position;
            Ray ray = new Ray(start, rig.centerEyeAnchor.transform.forward);
            Vector3 pt = ray.GetPoint(1);
            if (turnOnBeam)
            {
                beam.SetPosition(0, start + rig.centerEyeAnchor.transform.forward * 0.2f);
                beam.SetPosition(1, pt);
                beam.enabled = true;

            }
            else beam.enabled = false;
            TextWriter tw = new StreamWriter(path, true);
            tw.WriteLine("" + pt.x + "," + pt.y + "," + pt.z + "," + start.x + "," + start.y + "," + start.z + "," 
                + data_gamma + "," + data_beta + "," + data_alpha + "," + data_theta);
            tw.Close();

            Debug.Log(pt + ":" + data_gamma + "," + data_beta + "," + data_alpha + "," + data_theta);
            timeRemaining -= 1;
        }
        #endregion

        #region Extract brainwave data
        private void ReceivedMessage1(OSCMessage message)
        {
            var value1 = message.Values[0].FloatValue;
            //Debug.Log(value);
            data_gamma = value1;
        }

        private void ReceivedMessage2(OSCMessage message)
        {
            var value2 = message.Values[0].FloatValue;
            //Debug.Log(value);
            data_beta = value2;
        }
        private void ReceivedMessage3(OSCMessage message)
        {
            var value3 = message.Values[0].FloatValue;
            //Debug.Log(value);
            data_alpha = value3;
        }

        private void ReceivedMessage4(OSCMessage message)
        {
            var value4 = message.Values[0].FloatValue;
            //Debug.Log(value);
            data_theta = value4;
        }
        #endregion


        float intervalTime = 1f;
        float timePassed = 0;
        void Update()
        {
            //if (timerIsRunning)
            //{
            //    if (timeRemaining > 0)
            //    {
            //        timePassed += Time.deltaTime;
            //        if (timePassed > intervalTime)
            //        {
            //            Vector3 start = rig.centerEyeAnchor.transform.position;
            //            Ray ray = new Ray(start, rig.centerEyeAnchor.transform.forward);
            //            Vector3 pt = ray.GetPoint(1);
            //            if (turnOnBeam)
            //            {
            //                beam.SetPosition(0, start + rig.centerEyeAnchor.transform.forward * 0.2f);
            //                beam.SetPosition(1, pt);
            //                beam.enabled = true;

            //            }
            //            else beam.enabled = false;

            //            timePassed = 0f;
            //        }
            //        timeRemaining -= 1;
            //    }
            //    else
            //    {
            //        Debug.Log("Time is out.");
            //        timerIsRunning = false;
            //        timeRemaining = 0;
            //    }
            //}
            //timePassed += Time.deltaTime;
            //if (timePassed >= intervalTime)
            //{
            //    timeRemaining -= 1;
            //    Debug.Log("Yes");
            //    timePassed = 0;
            //}
            if (timeRemaining <= 0)
            {
                CancelInvoke();
            }

        }

        public LineRenderer CreateLine()
        {
            GameObject myLine = new GameObject();
            //myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            //lr.gameObject.SetActive(false);
            return lr;
        }

        void DrawLine2(LineRenderer lr, Vector3 start, Vector3 end)
        {
            if (!lr.gameObject.activeSelf)
            {
                lr.gameObject.SetActive(true);
            }
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }
}
