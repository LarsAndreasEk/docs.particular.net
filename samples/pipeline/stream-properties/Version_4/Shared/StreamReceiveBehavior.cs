using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
#pragma warning disable 618

public class StreamReceiveBehavior : IBehavior<ReceiveLogicalMessageContext>
{
    string location;

    public StreamReceiveBehavior(StreamStorageSettings storageSettings)
    {
        location = Path.GetFullPath(storageSettings.Location);
    }
    public void Invoke(ReceiveLogicalMessageContext context, Action next)
    {
        #region write-stream-properties-back
        object message = context.LogicalMessage.Instance;
        List<FileStream> streamsToCleanUp = new List<FileStream>();
        foreach (PropertyInfo property in StreamStorageHelpers.GetStreamProperties(message))
        {
            string headerKey = StreamStorageHelpers.GetHeaderKey(message, property);
            string dataBusKey;
            if (!context.LogicalMessage.Headers.TryGetValue("NServiceBus.PropertyStream." + headerKey, out dataBusKey))
            {
                continue;
            }

            string filePath = Path.Combine(location, dataBusKey);
            if (!File.Exists(filePath))
            {
                string format = string.Format("Expected a file to exist in '{0}'. It is possible the file has been prematurely cleaned up.", filePath);
                throw new Exception(format);
            }
            FileStream fileStream = File.OpenRead(filePath);
            property.SetValue(message,fileStream);
            streamsToCleanUp.Add(fileStream);
        }
        #endregion

        #region cleanup-after-nested-action
        next();
        foreach (FileStream fileStream in streamsToCleanUp)
        {
            fileStream.Dispose();
        }
        #endregion
    }

}