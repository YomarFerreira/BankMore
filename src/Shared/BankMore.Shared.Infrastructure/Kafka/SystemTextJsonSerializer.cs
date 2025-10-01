using System.Text.Json;
using KafkaFlow;

namespace BankMore.Shared.Infrastructure.Kafka
{
    public class SystemTextJsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            await JsonSerializer.SerializeAsync(output, message, _options);
        }
    }

    public class SystemTextJsonDeserializer : IDeserializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonDeserializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            var result = JsonSerializer.Deserialize(input, type, _options);
            return Task.FromResult(result!);
        }
    }
}
