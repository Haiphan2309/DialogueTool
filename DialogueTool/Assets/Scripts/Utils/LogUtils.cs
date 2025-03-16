using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif
/*
 * Usage: 

 * In instance method: 
 * - this.Log("message", Color.green) -> [ClassName] message
 * - this.Log("message", Color.green, "Player") -> [Player] message
 * - this.LogError("message") -> [ClassName] message
 *
 * In static method: 
 * - Logger.Log("message", Color.green) -> [ClassName] message
 * - Logger.Log("message", Color.green, "Player") -> [Player] message
 * - Logger.LogError("message") -> [ClassName] message
 *
 */
namespace GDC
{
    public static class LogExt
    {
        /// <summary>
        /// LogError with default header is the caller class name in string format
        /// </summary>
        /// <param name="o">the caller</param>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void LogError(this object o, string message, string customHeader = "")
        => Logger.LogError(message, customHeader: string.IsNullOrEmpty(customHeader) ? o.GetType().ToString() : string.Empty);
        /// <summary>
        /// Log with default header is the caller class name in string format
        /// </summary>
        /// <param name="o">the caller</param>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(this object o, string message, string customHeader = "")
        => Logger.Log(message, customHeader: string.IsNullOrEmpty(customHeader) ? o.GetType().ToString() : string.Empty);
        /// <summary>
        /// Log with default header is the caller class name in string format and coloring the content
        /// </summary>
        /// <param name="o">the caller</param>
        /// <param name="message">message to be logged out</param>
        /// <param name="color">color in Color type (default is white)</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(this object o, string message, Color color, string customHeader = "")
        => Logger.Log(message, color, customHeader: string.IsNullOrEmpty(customHeader) ? o.GetType().ToString() : string.Empty);
        /// <summary>
        /// Log with default header is the caller class name in string format and coloring the content
        /// </summary>
        /// <param name="o">the caller</param>
        /// <param name="message">message to be logged out</param>
        /// <param name="color">color string in hex format (with or without "#")</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(this object o, string message, string colorHex, string customHeader = "")
        => Logger.Log(message, colorHex, customHeader: string.IsNullOrEmpty(customHeader) ? o.GetType().ToString() : string.Empty);
        /// <summary>
        /// LogWarning with default header is the caller class name in string format
        /// </summary>
        /// <param name="o">the caller</param>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void LogWarning(this object o, string message, string customHeader = "")
        => Logger.LogWarning(message, customHeader: string.IsNullOrEmpty(customHeader) ? o.GetType().ToString() : string.Empty);
        /// <summary>
        /// From IEnumerator type (List, HashSet, Queue, etc...), grab all its elements into "[]" seperated by comma ","
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ListToString<T>(this IEnumerable<T> list)
        {
            string result = "[";
            int count = list.Count();
            if (count <= 0)
            {
                return "[]";
            }
            int i = 0;
            string lastItemString = string.Empty;
            foreach (var item in list)
            {
                if (i >= count - 1)
                {
                    lastItemString = item.ToString();
                    break;
                }
                result += item.ToString() + ", ";
                i++;
            }
            result += lastItemString + "]";
            return result;
        }
    }
    public static class Logger
    {
        /// <summary>
        /// LogError with default header is "Logger"
        /// </summary>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void LogError(string message, string customHeader = "")
        {
            Debug.LogErrorFormat("<b>[{0}] {1}</b>", string.IsNullOrEmpty(customHeader) ? "Logger" : customHeader, message);
        }
        /// <summary>
        /// Log with default header is "Logger"
        /// </summary>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(string message, string customHeader = "")
        {
            Debug.LogFormat("<b>[{0}] {1}</b>", string.IsNullOrEmpty(customHeader) ? "Logger" : customHeader, message);
        }
        /// <summary>
        /// Log with default header is "Logger" and coloring the content
        /// </summary>
        /// <param name="message">message to be logged out</param>
        /// <param name="colorHex">color in Color type (default is white)</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(string message, Color color, string customHeader = "")
        {
            string colorString = "#" + ColorUtility.ToHtmlStringRGBA(color);
            Debug.LogFormat("<b>[{0}] <color={1}>{2}</color></b>", string.IsNullOrEmpty(customHeader) ? "Logger" : customHeader, colorString, message);
        }
        /// <summary>
        /// Log with default header is "Logger" and coloring the content
        /// </summary>
        /// <param name="message">message to be logged out</param>
        /// <param name="colorHex">color string in hex format (with or without "#")</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void Log(string message, string colorHex, string customHeader = "")
        {
            string defaultColor = "#" + ColorUtility.ToHtmlStringRGBA(Color.white);
            if (!colorHex.StartsWith("#"))
                colorHex = "#" + colorHex;
            bool isValidHexString = Regex.IsMatch(colorHex, @"[#][0-9A-Fa-f]{6}\b");
            if (!isValidHexString)
                colorHex = defaultColor;
            Debug.LogFormat("<b>[{0}] <color={1}>{2}</color></b>", string.IsNullOrEmpty(customHeader) ? "Logger" : customHeader, colorHex, message);
        }
        /// <summary>
        /// LogWarning with default header is "Logger"
        /// </summary>
        /// <param name="message">message to be logged out</param>
        /// <param name="customHeader">custom header instead of default header</param>
        public static void LogWarning(string message, string customHeader = "")
        {
            Debug.LogWarningFormat("<b>[{0}] {1}</b>", string.IsNullOrEmpty(customHeader) ? "Logger" : customHeader, message);
        }
    }
    public class DebugUtils
    {
#if UNITY_EDITOR
        public static void PingObjectAtPath(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
#endif
    }
}