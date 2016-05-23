using System.IO;
using System.Threading.Tasks;

namespace SpeakEasy
{
    public class SerializableByteArray : ISerializableBody
    {
        public SerializableByteArray(string contentType, byte[] bytes)
        {
            ContentType = contentType;
            Content = bytes;
        }

        public int ContentLength => Content.Length;

        public byte[] Content { get; }

        public bool HasContent => ContentLength > 0;

        public string ContentType { get; }

        public Task WriteTo(Stream stream)
        {
            return stream.WriteAsync(Content, 0, Content.Length);
        }
    }
}