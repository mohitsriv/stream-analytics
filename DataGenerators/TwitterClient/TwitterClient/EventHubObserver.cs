﻿//********************************************************* 
// 
//    Copyright (c) Microsoft. All rights reserved. 
//    This code is licensed under the Microsoft Public License. 
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF 
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY 
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR 
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT. 
// 
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;

namespace TwitterClient
{
    class EventHubObserver : IObserver<Payload>
    {
        private EventHubConfig _config;
        private EventHubClient _eventHubClient;
       
                
        public EventHubObserver(EventHubConfig config)
        {
            try
            {
                _config = config;
                _eventHubClient = EventHubClient.CreateFromConnectionString(_config.ConnectionString, config.EventHubName);
                
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not create EventHub: " + e.ToString());
            }

        }
        public void OnNext(Payload TwitterPayloadData)
        {
            try
            {

                var serialisedString = JsonConvert.SerializeObject(TwitterPayloadData);
                EventData data = new EventData(Encoding.UTF8.GetBytes(serialisedString)) { PartitionKey = TwitterPayloadData.UserName };
                _eventHubClient.Send(data);
               
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sending" + serialisedString + " at: " + TwitterPayloadData.CreatedAt.ToString() );
                                
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception sending EventHub data: " + e.ToString());
            }

        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            Trace.TraceError("EventHubObserver.OnError: ", error.ToString());
        }

    }
}
