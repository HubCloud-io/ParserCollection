using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HubCloud.ParserCollection
{
    public class SeparateNameValuesJsonParser
    {
        private SeparateNameValuesJsonParserSettings _settings = new SeparateNameValuesJsonParserSettings();

        public SeparateNameValuesJsonParser()
        {

        }

        public SeparateNameValuesJsonParser(SeparateNameValuesJsonParserSettings settings)
        {
            _settings = settings;
        }

        public IList<IDictionary<string, object>> Parse(string jsonString)
        {

            if (string.IsNullOrEmpty(_settings.NamesPath))
            {
                throw new ArgumentException("Names path is empty.", nameof(_settings.NamesPath));
            }

            if (string.IsNullOrEmpty(_settings.ValuesPath))
            {
                throw new ArgumentException("Values path is empty.", nameof(_settings.ValuesPath));
            }

            var result = new List<IDictionary<string, object>>();

            var data = JObject.Parse(jsonString);

            var namesArray = GetArrayFromObject(data, _settings.NamesPath);

            if (namesArray.Count() == 0)
            {
                throw new ArgumentException("Names not found. It is possible NamesPath is wrong.", nameof(_settings.NamesPath));
            }

            // Get data
            var items = GetArrayFromObject(data, _settings.ValuesPath);

            if (items.Count() == 0)
            {
                throw new ArgumentException("Data not found. It is possible ValuesPath is wrong.", nameof(_settings.ValuesPath));
            }

            // Prepare names positions dictionary
            var namesPositions = PrepareNamesPositions(namesArray);


            foreach (JArray item in items)
            {
                var resultItem = new Dictionary<string, object>();

                var j = -1;
                foreach (var jToken in item)
                {
                    j++;

                    string name = "";
                    if (!namesPositions.TryGetValue(j, out name))
                    {
                        continue;
                    }

                    if (jToken is JValue jValue)
                    {
                        if (jValue.HasValues)
                        {
                            continue;
                        }

                        var currentValue = jValue.Value;
                        resultItem.Add(name, currentValue);

                    }
                    else if (jToken is JArray jArray)
                    {
                        var currentValue = BuildArrayPresentation(jArray, ", ");
                        resultItem.Add(name, currentValue);

                    }

                }

                result.Add(resultItem);
            }

            return result;
        }

        private JArray GetArrayFromObject(JObject data, string path)
        {
            var separators = new char[] { '.' };
            var pathParts = path.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries);

            JObject jObject = data;
            JArray jArray = new JArray();

            foreach (var name in pathParts)
            {
                var jToken = jObject[name];
                if (jToken is JObject)
                {
                    jObject = (JObject)jToken;
                }
                else if (jToken is JArray)
                {
                    jArray = (JArray)jToken;
                }
            }

            return jArray;
        }

        private Dictionary<int, string> PrepareNamesPositions(JArray namesArray)
        {
            var namesPositions = new Dictionary<int, string>();
            var namesDict = new Dictionary<string, int>();

            for (var i = 0; i < namesArray.Count; i++)
            {
                var jValue = namesArray[i];

                if (jValue.HasValues)
                {
                    continue;
                }

                var name = jValue.Value<string>();

                if (_settings.IgnoreDoubleNames)
                {
                    // Ignore doubles
                    if (namesDict.ContainsKey(name))
                    {
                        continue;
                    }
                    namesDict.Add(name, 1);
                }
                else
                {
                    // Take doubles
                    if (namesDict.TryGetValue(name, out var j))
                    {
                        namesDict[name] = j++;
                        name = $"{name}{j}";
                    }
                    else
                    {
                        namesDict.Add(name, 1);
                    }

                }

                if (_settings.NamesMapping.Any())
                {
                    // Add only mapped names.
                    if (_settings.NamesMapping.TryGetValue(name, out var resultName))
                    {
                        namesPositions.Add(i, resultName);
                    }
                }
                else
                {
                    // No names mapping. Take all properties.
                    namesPositions.Add(i, name);
                }

            }

            return namesPositions;
        }

        private string BuildArrayPresentation(JArray jarray, string separator)
        {
            var sb = new StringBuilder();

            foreach(var jToken in jarray)
            {
                string currentValue = "";
                if (jToken is JValue jvalue)
                {
                    currentValue = jvalue.Value?.ToString();
                }
                else if (jToken is JArray innerArray)
                {
                    currentValue = BuildArrayPresentation(innerArray, separator);
                }
                else if (jToken is JObject jObject)
                {

                }

                if (!string.IsNullOrEmpty(currentValue))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }
                    sb.Append(currentValue);
                }
            }

            return sb.ToString();
        }
    }
}
