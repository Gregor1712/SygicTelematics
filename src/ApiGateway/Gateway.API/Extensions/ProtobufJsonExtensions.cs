using Google.Protobuf;
using Google.Protobuf.Collections;
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

    public static string FormatMessage(IMessage message) => Formatter.Format(message);

    public static string FormatList<T>(RepeatedField<T> items) where T : IMessage
    {
        return "[" + string.Join(",", items.Select(m => FormatMessage(m))) + "]";
    }
}
