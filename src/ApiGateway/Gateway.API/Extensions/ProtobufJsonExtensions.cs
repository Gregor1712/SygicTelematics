using System.Collections.Generic;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Extensions;

public static class ProtobufJsonExtensions
{
    private static readonly JsonFormatter Formatter = new(JsonFormatter.Settings.Default.WithFormatDefaultValues(false));

    public static ContentResult ProtoJson(this ControllerBase _, IMessage message, int statusCode = 200)
    {
        return new ContentResult
        {
            Content = Formatter.Format(message),
            ContentType = "application/json",
            StatusCode = statusCode
        };
    }

    public static ContentResult ProtoJsonCreated(this ControllerBase _, IMessage message)
        => ProtoJson(_, message, 201);

    public static ContentResult ProtoJsonComposite(this ControllerBase _, params (string key, object value)[] parts)
    {
        var json = "{" + string.Join(",", parts.Select(p => p.value switch
        {
            IMessage msg => $"\"{p.key}\":{Formatter.Format(msg)}",
            IEnumerable<IMessage> list => $"\"{p.key}\":[{string.Join(",", list.Select(m => Formatter.Format(m)))}]",
            _ => $"\"{p.key}\":null"
        })) + "}";

        return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };
    }
}
