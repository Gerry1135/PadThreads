using System;
using System.Threading;
using UnityEngine;

namespace PadThreads
{
    public class ThreadObj
    {
        public ThreadObj()
        {
        }

        public bool _running = false;
        public int count = 0;

        public Boolean Run
        {
            get { return _running; }
            set
            {
                if (_running != value)
                {
                    if (_running)
                    {
                        _running = false;   // The thread will stop shortly
                    }
                    else
                    {
                        _running = true;   // The thread will test this and stop when false
                        ThreadPool.QueueUserWorkItem(ThreadFunc, this);
                    }
                }
            }
        }

        private static void ThreadFunc(object obj)
        {
            ThreadObj thread = obj as ThreadObj;
            while (thread.Run)
            {
                int total = 0;
                for (int i = 0; i < 1000000000; i++)
                    total += i;

                thread.count = total;
            }
        }
    }


    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class TestWorkItems : MonoBehaviour
    {
        private const int NumThreads = 4;

        private ThreadObj[] threadarray;

        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUILayoutOption buttonWidth;
        private GUILayoutOption wndWidth;
        private GUILayoutOption wndHeight;

        private Rect windowPos = new Rect(80, 80, 100, 100);
        private bool showUI = false;

        internal void Awake()
        {
            DontDestroyOnLoad(gameObject);

            threadarray = new ThreadObj[NumThreads];
            for (int i = 0; i < NumThreads; i++)
            {
                threadarray[i] = new ThreadObj();
            }
        }

        internal void OnDestroy()
        {
        }

        public void Update()
        {
            //print("Update Start");

            if (GameSettings.MODIFIER_KEY.GetKey() && Input.GetKeyDown(KeyCode.Semicolon))
                showUI = !showUI;

            //print("Update End");
        }

        public void OnGUI()
        {
            if (labelStyle == null)
                labelStyle = new GUIStyle(GUI.skin.label);

            if (buttonStyle == null)
                buttonStyle = new GUIStyle(GUI.skin.button);
            if (buttonWidth == null)
                buttonWidth = GUILayout.Width(80);

            if (wndWidth == null)
                wndWidth = GUILayout.Width(100);
            if (wndHeight == null)
                wndHeight = GUILayout.Height(100);

            if (showUI)
                windowPos = GUILayout.Window(6753464, windowPos, WindowGUI, "Threads", wndWidth, wndHeight);
        }

        public void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < NumThreads; i++)
            {
                ThreadObj thread = threadarray[i];

                GUILayout.BeginVertical();

                thread.Run = GUILayout.Toggle(thread.Run, "Thread", buttonStyle, buttonWidth);

                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

}
