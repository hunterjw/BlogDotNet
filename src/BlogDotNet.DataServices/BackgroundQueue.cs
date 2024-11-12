using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;

namespace BlogDotNet.DataServices;

/// <summary>
/// Background Queue.
/// Run tasks in the background, first in first out
/// </summary>
/// <typeparam name="T">Item to process</typeparam>
/// <param name="logger">Logger</param>
/// <param name="processFunc">Item process function</param>
internal class BackgroundQueue<T>(ILogger logger, Func<T, Task> processFunc)
{
    private readonly ILogger _logger = logger;
    private readonly Func<T, Task> _processFunc = processFunc;

    private readonly ConcurrentQueue<T> _queue = new();
    private readonly SemaphoreSlim _queueLock = new(1, 1);
    private readonly ResiliencePipeline _resiliencePipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,  
            MaxRetryAttempts = 4,
            Delay = TimeSpan.FromSeconds(3),
        })
        .Build(); 

    private bool _processing = false;

    private async Task ProcessQueue()
    {
        if (!_processing && !_queue.IsEmpty)
        {
            await _queueLock.WaitAsync();
            if (!_queue.IsEmpty)
            {
                _processing = true;

                while (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out T? queueItem))
                    {
                        try
                        {
                            await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
                            {
                                await _processFunc(queueItem);
                            });
                        }
                        catch (Exception ex) 
                        {
                            _logger.LogError(ex, "Background queue error");
                        }
                    }
                }

            }
            _processing = false;
            _queueLock.Release();
        }
    }

    /// <summary>
    /// Enqueue a <see cref="T"/>
    /// </summary>
    /// <param name="item">Item to enqueue</param>
    public void Enqueue(T item)
    {
        _queue.Enqueue(item);

        _ = Task.Run(ProcessQueue);
    }
}
