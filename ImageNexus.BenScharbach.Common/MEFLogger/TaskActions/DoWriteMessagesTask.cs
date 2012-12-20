using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ImageNexus.BenScharbach.Common.LogWinEvents;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;

namespace ImageNexus.BenScharbach.Common.MEFLogger.TaskActions
{
    // 10/2/2011
    /// <summary>
    /// Adds messages to an internal concurrent queue, which are then processed and transferred to the <see cref="Trace"/>.
    /// </summary>
    public class DoWriteMessagesTask : IDisposable
    {
        // holds messages to be processed for the Tracer.
        private readonly ConcurrentQueue<TaskObjectState>  _concurrentQueueMessages = new ConcurrentQueue<TaskObjectState>();
        private readonly CancellationTokenSource _tokenSource;

        private static readonly Stopwatch TimerToSleep1 = new Stopwatch();
        private static readonly TimeSpan TimerSleepMax = new TimeSpan(0, 0, 0, 0, 3000);

        // task
        private readonly Task _concurrentQueueTask;

        private class TaskObjectState
        {
            public string Message;
            public MessageTypeEnum MessageType;
            public string SourceName;
            public int TraceListenerInstanceId;
            public DoWriteMessagesTask ThisTask; // 1/23/2012
            public CancellationToken CancellationToken; // 1/23/2012
        }

        /// <summary>
        /// Initializes an instance of the <see cref="DoWriteMessagesTask"/>.
        /// </summary>
        public DoWriteMessagesTask()
        {
            // Create new instance of cancellation token.
            _tokenSource = new CancellationTokenSource();
            CancellationToken token = _tokenSource.Token;

            // create the parallel task which will process messages.
            _concurrentQueueTask = Task.Factory.StartNew(DoTaskActionCallback, new TaskObjectState
                                                                                   {
                                                                                       ThisTask = this,
                                                                                       CancellationToken = token
                                                                                   }, token);
        }

        /// <summary>
        /// Adds a message to the internal concurrent queue, to be processed and transferred to the <see cref="Trace"/>.
        /// </summary>
        /// <param name="traceListenerInstanceId"><see cref="TraceListener"/> array index to use.</param>
        /// <param name="message">Message to log</param>
        /// <param name="messageType">The <see cref="MessageTypeEnum"/> in use.</param>
        /// <param name="sourceName">Source name</param>
        public void DoWriteMessage(int traceListenerInstanceId, string message, MessageTypeEnum messageType, string sourceName)
        {
            // Create message
            var messageObject = new TaskObjectState
                                    {
                                        TraceListenerInstanceId = traceListenerInstanceId,
                                        Message = message,
                                        MessageType = messageType,
                                        SourceName = sourceName,
                                    };

            // add message to concurrent queue
            _concurrentQueueMessages.Enqueue(messageObject);
        }

        /// <summary>
        /// Task's main action, which checks for items to process within the concurrent queue, and
        /// transferred them to the <see cref="Trace"/>.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private void DoTaskActionCallback(object arg)
        {
            // Retrieve Task's objectState
            var objectState = (TaskObjectState)arg;
            var thisTask = objectState.ThisTask;

            try
            {
                while (true)
                {
                    // check for cancellation
                    var cancellationToken = objectState.CancellationToken;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    // do while not empty
                    while (!thisTask._concurrentQueueMessages.IsEmpty)
                    {
                        // dequeue message
                        TaskObjectState messageObject;
                        if (thisTask._concurrentQueueMessages.TryDequeue(out messageObject))
                        {
                            // Add to Trace
                            thisTask.AddTraceMessage(messageObject);
                        }

                        // check if time to sleep
                        if (TimerToSleep1.Elapsed.TotalMilliseconds <= TimerSleepMax.TotalMilliseconds)
                        {
                            continue;
                        }

                        // Start StopWatch timer
                        TimerToSleep1.Reset();
                        TimerToSleep1.Start();

                        Thread.Sleep(10);
                    }

                    // Start StopWatch timer
                    TimerToSleep1.Reset();
                    TimerToSleep1.Start();

                    // sleep while queue is empty
                    while (TimerToSleep1.Elapsed.TotalMilliseconds < TimerSleepMax.TotalMilliseconds)
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogWinEventsHelper.LogWarningMessage("Task 'DoPollLoggerMessages' method received the cancel request.  Operation Aborted.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Adds the given message to the <see cref="Trace"/>.
        /// </summary>
        private void AddTraceMessage(TaskObjectState messageObject)
        {
            if (messageObject == null) return;
           
            // cache data
            var messageTypeEnum = messageObject.MessageType;
            var sourceName = messageObject.SourceName;
            var traceListenerInstanceId = messageObject.TraceListenerInstanceId;
            var message = messageObject.Message;

            var listeners = Trace.Listeners;
            if (traceListenerInstanceId >= listeners.Count)
                return;

            var traceListener = listeners[traceListenerInstanceId];

            // 7/17/2011 - Use TraceEvent for the specific message type.
            switch (messageTypeEnum)
            {
                case MessageTypeEnum.Information:
                case MessageTypeEnum.Result:
                    // Write Message
                    traceListener.TraceEvent(new TraceEventCache(), sourceName, TraceEventType.Information, traceListenerInstanceId, message);
                    break;
                case MessageTypeEnum.Warning:
                    // Write Message
                    traceListener.TraceEvent(new TraceEventCache(), sourceName, TraceEventType.Warning, traceListenerInstanceId, message);
                    break;
                case MessageTypeEnum.Error:
                    // Write Message
                    traceListener.TraceEvent(new TraceEventCache(), sourceName, TraceEventType.Error, traceListenerInstanceId, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("messageTypeEnum");
            }

            // Write Message
            traceListener.WriteLine(message);

            // Flush tracer
            traceListener.Flush();

        }

        /// <summary>
        /// Cancels internal task.
        /// </summary>
        public void Dispose()
        {
            // Cancel task
            if (_tokenSource == null)
            {
                return;
            }

            _tokenSource.Cancel();

            if (!_concurrentQueueTask.Wait(25))
            {
                Thread.Sleep(10);
            }
        }
    }
}