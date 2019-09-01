using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace XModPackager.Config
{
    public class ConfigCatsModelConverter : JsonConverter<IDictionary<string, IEnumerable<Regex>>>
    {
        public override IDictionary<string, IEnumerable<Regex>> ReadJson(JsonReader reader, Type objectType, IDictionary<string, IEnumerable<Regex>> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var rawDictionary = serializer.Deserialize<Dictionary<string, IEnumerable<string>>>(reader);

            IDictionary<string, IEnumerable<Regex>> result;
            if (hasExistingValue)
            {
                result = existingValue;
            }
            else
            {
                result = new Dictionary<string, IEnumerable<Regex>>();
            }

            foreach (var catInfo in rawDictionary)
            {
                var regexesFromJson = catInfo.Value.Select(pattern => new Regex(pattern));

                var hasExistingEntry = result.TryGetValue(catInfo.Key, out var existingEntry);
                if (hasExistingEntry)
                {
                    result[catInfo.Key] = existingEntry.Concat(regexesFromJson).ToList();
                }
                else
                {
                    result[catInfo.Key] = regexesFromJson.ToList();
                }

            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, IDictionary<string, IEnumerable<Regex>> value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var marshalledDict = value.Keys.ToDictionary(
                key => key,
                key => value[key].Select(pattern => pattern.ToString()).ToList()
            );

            serializer.Serialize(writer, marshalledDict);
        }
    }
}