using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Available starting with C# 9.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModuleInitializerAttribute : Attribute { }
}

public static class TestAssemblyInitializer
{
    [ModuleInitializer]
    public static void ModuleInitializer()
    {
        lock (Debug.Listeners)
        {
            var listeners = Debug.Listeners;

            var defaultListener = listeners.OfType<DefaultTraceListener>().FirstOrDefault();
            if (defaultListener != null)
            {
                defaultListener.AssertUiEnabled = false;
            }

            if (!listeners.OfType<ThrowingTraceListener>().Any())
            {
                listeners.Add(new ThrowingTraceListener());
            }
        }
    }
}

public class AssertFailedException : Exception
{
    public AssertFailedException()
    {
    }

    public AssertFailedException(string message) : base(message)
    {
    }

    public AssertFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ThrowingTraceListener : TraceListener
{
    public override void Fail(string message, string detailMessage)
    {
        throw new AssertFailedException($"{message}\n{detailMessage}");
    }

    public override void Write(string message)
    {
    }

    public override void WriteLine(string message)
    {
    }
}
