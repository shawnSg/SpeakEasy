using System.Collections.Generic;
using System.Linq;
using NLog;
using Resticle.Deserializers;

namespace Resticle
{
    public class Transmission : ITransmission
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISerializer serializer;

        private readonly IEnumerable<IDeserializer> deserializers;

        public Transmission(ISerializer serializer, IEnumerable<IDeserializer> deserializers)
        {
            this.serializer = serializer;
            this.deserializers = deserializers;
        }

        public ISerializer DefaultSerializer
        {
            get { return serializer; }
        }

        public string ContentType
        {
            get { return DefaultSerializer.ContentType; }
        }

        public IEnumerable<string> DeserializableMediaTypes
        {
            get { return deserializers.SelectMany(d => d.SupportedMediaTypes).Distinct(); }
        }

        public IDeserializer FindDeserializer(string contentType)
        {
            Logger.Debug("Finding deserializer for {0}", contentType);

            var deserializer = deserializers.FirstOrDefault(d => d.SupportedMediaTypes.Any(contentType.StartsWith));

            return deserializer ?? new NullDeserializer(contentType);
        }
    }
}