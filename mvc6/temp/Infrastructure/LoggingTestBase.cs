using System;
using Xunit.Abstractions;

public class LoggingTestBase : IDisposable
{
    private readonly IDisposable _logger;

    protected LoggingTestBase(ITestOutputHelper outputHelper)
    {
        _logger = LogHelper.Capture(outputHelper);
    }

    public virtual void Dispose()
    {
        _logger.Dispose();
    }
}