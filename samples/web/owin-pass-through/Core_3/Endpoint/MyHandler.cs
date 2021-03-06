﻿using log4net;
using Messages;
using NServiceBus;

public class MyHandler : IHandleMessages<MyMessage>
{
    static ILog log = LogManager.GetLogger(typeof(MyHandler));

    public void Handle(MyMessage message)
    {
        log.InfoFormat("Received MyMessage. Property1:'{0}'. Property2:'{1}'", message.Property1, message.Property2);
    }
}