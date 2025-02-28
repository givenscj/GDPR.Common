﻿using GDPR.Common;
using GDPR.Common.Core;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public class GDPRErrorEventProcessor : IEventProcessor
    {
        public static List<EventData> Events = new List<EventData>();

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
            DateTime checkPoint;
            string offSet;
            GDPRCore.Current.GetOffset(Configuration.EventHubNamespace, Configuration.EventErrorHubName, context.ConsumerGroupName, context.Lease.PartitionId, out checkPoint, out offSet);

            DateTime lastMessageDate = checkPoint;

            foreach (EventData eventData in messages)
            {
                Events.Add(eventData);                                
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
