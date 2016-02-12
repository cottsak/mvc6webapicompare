using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;
using Xunit.Abstractions;

public static class LogHelper
{
    private readonly static ConcurrentDictionary<Guid, ITestOutputHelper> _loggerLookup = new ConcurrentDictionary<Guid, ITestOutputHelper>();

    public static void Log(string log)
    {
        var currentCorrelationId = (Guid?)CallContext.LogicalGetData("TestCorrelationId");
        if (currentCorrelationId == null)
            return;

        _loggerLookup[currentCorrelationId.Value].WriteLine(log);
    }

    static void AddOutputHelper(Guid correlationId, ITestOutputHelper outputHelper)
    {
        if (outputHelper == null)
            throw new ArgumentNullException(nameof(outputHelper));
        _loggerLookup.TryAdd(correlationId, outputHelper);
    }

    static void RemoveOutputHelper(Guid correlationId)
    {
        ITestOutputHelper removedHelper;
        _loggerLookup.TryRemove(correlationId, out removedHelper);
    }
    
    public static IDisposable Capture(ITestOutputHelper outputHelper)
    {
        if (outputHelper == null)
            throw new ArgumentNullException(nameof(outputHelper));
        var correlationId = Guid.NewGuid();
        AddOutputHelper(correlationId, outputHelper);
        CallContext.LogicalSetData("TestCorrelationId", correlationId);

        return new DelegateDisposable(() =>
        {
            RemoveOutputHelper(correlationId);
            CallContext.LogicalSetData("TestCorrelationId", null);
        });
    }

    class DelegateDisposable : IDisposable
    {
        private readonly Action _action;

        public DelegateDisposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}