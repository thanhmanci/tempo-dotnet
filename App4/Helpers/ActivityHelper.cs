using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace App4.Helpers
{
    public static class ActivityHelper
    {
        public static IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            try
            {
                if (props.Headers != null && props.Headers.ContainsKey(key))
                {
                    props.Headers.TryGetValue(key, out var value);
                    var bytes = value as byte[];
                    return new[] { Encoding.UTF8.GetString(bytes ?? Array.Empty<byte>()) };
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to extract trace context: {ex}");
            }

            return Enumerable.Empty<string>();
        }

        public static void AddActivityTags(Activity activity)
        {
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", "sample_2");
        }

    }
}