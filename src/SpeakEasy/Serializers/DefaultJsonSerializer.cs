﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpeakEasy.Serializers
{
    public class DefaultJsonSerializer : Serializer
    {
        public override IEnumerable<string> SupportedMediaTypes => new[]
        {
            "application/json",
            "text/json",
            "text/x-json",
            "text/javascript"
        };

        public override Task SerializeAsync<T>(Stream stream, T body)
        {
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(stream, new System.Text.UTF8Encoding(false), 1024, true))
            {
                using (var jsonTextWriter = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jsonTextWriter, body);
                }
            }

            return Task.FromResult(true);
        }

        public override T Deserialize<T>(Stream body)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(body))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public override object Deserialize(Stream body, Type type)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(body))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }
    }
}
