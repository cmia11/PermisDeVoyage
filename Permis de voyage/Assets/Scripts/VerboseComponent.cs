using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Enables filtering of the logs in components which may produce a lot of it.
/// </summary>
public class VerboseComponent : MonoBehaviour
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        None,
    }

    public LogLevel LoggingLevel
    {
        get => loggingLevel;
        set {
            if (loggingLevel != value)
            {
                LogLevel oldValue = loggingLevel;
                loggingLevel = value;
                _loggingLevel = value;
                OnLoggingLevelChanged(oldValue, loggingLevel);
            }
        }
    }
    private LogLevel loggingLevel = LogLevel.Warning;

    // Unity inspector field
    [SerializeField]
    private LogLevel _loggingLevel = LogLevel.Warning;

    protected virtual void Update()
    {
        if (_loggingLevel != loggingLevel)
        {
            // The value has been set in the inspector.
            LogDebug($"Copying new logging level from inspector: {_loggingLevel}");
            LoggingLevel = _loggingLevel;
        }
    }

    protected void LogDebug(object message)
    {
        LogAtLevel(LogLevel.Debug, message, null);
    }

    protected void LogDebug(object message, UnityEngine.Object context)
    {
        LogAtLevel(LogLevel.Debug, message, context);
    }

    protected void Log(object message)
    {
        LogAtLevel(LogLevel.Info, message, null);
    }

    protected void Log(object message, UnityEngine.Object context)
    {
        LogAtLevel(LogLevel.Info, message, context);
    }

    protected void LogWarning(object message)
    {
        LogAtLevel(LogLevel.Warning, message, null);
    }

    protected void LogWarning(object message, UnityEngine.Object context)
    {
        LogAtLevel(LogLevel.Warning, message, context);
    }

    protected void LogError(object message)
    {
        LogAtLevel(LogLevel.Error, message, null);
    }

    protected void LogError(object message, UnityEngine.Object context)
    {
        LogAtLevel(LogLevel.Error, message, context);
    }

    protected void LogAtLevel(LogLevel level, object message)
    {
        LogAtLevel(level, message, null);
    }

    protected void LogAtLevel(LogLevel level, object message, UnityEngine.Object context)
    {
        if (level >= LoggingLevel)
            Debug.Log(message, context);
    }

    protected virtual void OnLoggingLevelChanged(LogLevel oldValue, LogLevel loggingLevel) { }
}
