/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Extensions {

    using UnityEngine;
    using System.Collections;

    public static class Utilities {

        #region --Logging--

        private static StackTraceLogType currentTrace;
        public static bool verbose;

        public static void Log (string log) {
            EnterLog();
            Debug.Log("NatCorder: "+log);
            ExitLog();
        }

        public static void LogVerbose (string log) {
            if (!verbose) return;
            EnterLog();
            Debug.Log("NatCorder Logging: "+log);
            ExitLog();
        }
        
        public static void LogError (string warning) {
            EnterLog(LogType.Warning, StackTraceLogType.ScriptOnly);
            Debug.LogWarning("NatCorder Error: "+warning);
            ExitLog(LogType.Warning);
        }

        private static void EnterLog (LogType type = LogType.Log, StackTraceLogType stack = StackTraceLogType.None) {
            #if UNITY_5_4_OR_NEWER
            currentTrace = Application.GetStackTraceLogType(type);
            Application.SetStackTraceLogType(type, stack);
            #else
            currentTrace = Application.stackTraceLogType;
            Application.stackTraceLogType = stack;
            #endif
        }

        private static void ExitLog (LogType type = LogType.Log) {
            #if UNITY_5_4_OR_NEWER
            Application.SetStackTraceLogType(type, currentTrace);
            #else
            Application.stackTraceLogType = currentTrace;
            #endif
        }
        #endregion


        #region --Extensions--

        public static Coroutine Invoke (this IEnumerator routine, MonoBehaviour mono) {
            return mono.StartCoroutine(routine);
        }
        
        public static void Terminate (this Coroutine routine, MonoBehaviour mono) {
            mono.StopCoroutine(routine);
        }

        public static void ForEach<T> (this T[] array, System.Action<T> action) {
            if (array == null) return;
            for (int i = 0, len = array.Length; i < len; i++) action(array[i]);
        }
        #endregion
    }
}