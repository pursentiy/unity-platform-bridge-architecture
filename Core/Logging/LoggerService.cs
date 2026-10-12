using System;
using UnityEngine;

namespace Core.Logging
{
    public static class LoggerService
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogDebug(object @object, string message)
        {
            LogDebug(@object.GetType().Name + ": " + message);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogDebug(string message)
        {
            Debug.Log(message);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarningEditor(string message)
        {
            LogWarning(message);
        }
        
        public static void LogWarning(object @object, string message)
        {
            LogWarning(@object.GetType().Name +  ": " + message);
        }
        
        public static void LogWarning(Exception exception)
        {
            Debug.LogWarning(exception);
        }
        
        public static void LogError(object @object, string message)
        {
            Debug.LogError(@object.GetType().Name +  ": " + message);
        }
        
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }
        
        public static void LogError(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void LogError(object @object, string context, Exception exception)
        {
            LogError(@object.GetType().Name + ": " + context, exception);
        }

        public static void LogError(string context, Exception exception)
        {
            // Logged right before the exception so Sentry attaches it as a breadcrumb
            Debug.Log(context);
            Debug.LogException(exception);
        }
        
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
    }
}