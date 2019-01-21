using GDPR.Common.Core;
using GDPR.Common.Enums;
using GDPR.Common.Messages;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class GDPREventProcessor : IEventProcessor
    {

        Stopwatch checkpointStopWatch;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            GDPRCore.Current.Log(string.Format("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            GDPRCore.Current.Log(string.Format("SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            long preOffset = 0;
            DateTime checkPoint = GDPRCore.Current.GetOffset(context.ConsumerGroupName, context.Lease.PartitionId);
            DateTime lastMessageDate = checkPoint;

            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());

                try
                {
                    Type t = typeof(GDPRMessageWrapper);
                    GDPRMessageWrapper w = (GDPRMessageWrapper)Newtonsoft.Json.JsonConvert.DeserializeObject(data, t);
                    w.OffSet = eventData.Offset;

                    long msgOffSet = long.Parse(eventData.Offset);

                    if (w.MessageDate >= checkPoint)
                    {
                        if (w.MessageDate > lastMessageDate)
                        {
                            lastMessageDate = w.MessageDate;
                        }

                        GDPRCore.Current.Log(string.Format("Message received.  Partition: '{0}', Data: '{1}'", context.Lease.PartitionId, data));

                        //start a new thread to process the request...
                        await Task.Run(() => GDPRCore.Current.ProcessRequest(w));

                        //hopefully the are serialized...
                        checkPoint = lastMessageDate;

                        //save the position...
                        GDPRCore.Current.SetOffSet(context.ConsumerGroupName, context.Lease.PartitionId, lastMessageDate, eventData.Offset);
                    }
                    else
                    {
                        GDPRCore.Current.Log(string.Format("Skiping message {0}", eventData.Offset));
                    }
                }
                catch(Exception ex)
                {
                    GDPRCore.Current.Log(data);
                    GDPRCore.Current.Log(ex, LogLevel.Error);
                }                                
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }
    }
}
