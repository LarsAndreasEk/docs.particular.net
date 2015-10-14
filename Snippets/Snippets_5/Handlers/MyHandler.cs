﻿namespace Snippets5.Handlers
{
    using NServiceBus;

    #region CreatingMessageHandler

    public class MyHandler : IHandleMessages<MyMessage>
    {
        public void Handle(MyMessage message)
        {
            //do something with the message data
            SomeLibrary.SomeMethod(message.Data);
        }
    }

    #endregion
}


