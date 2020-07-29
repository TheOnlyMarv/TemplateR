using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateR.Core.Config;
using TemplateR.Core.Config.DataConverter;

namespace TemplateR.Core
{
    public class TemplateBuilder
    {
        private Configuration configuration;

        public TemplateBuilder() : this(new Configuration()) { }

        public TemplateBuilder(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Template> FromFileAsync(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return await FromStreamAsync(fs);
            }
        }

        public async Task<Template> FromStreamAsync(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return FromString(await sr.ReadToEndAsync());
            }
        }

        public Template FromString(string templateString)
        {
            return new Template(configuration, templateString);
        }

    }

    public class Template
    {
        private string orgTemplateString;
        private Configuration configuration;
        private IList<Placeholder> placeholders;

        internal Template(Configuration configuration, string templateString)
        {
            this.configuration = configuration;
            orgTemplateString = templateString;
            placeholders = new List<Placeholder>();
            AnalyseTemplate();
        }

        private void AnalyseTemplate()
        {
            var searchRegex = new Regex(@"{{(?<path>[\w\.\:\s\[\]]*)}}");
            foreach (Match match in searchRegex.Matches(orgTemplateString))
            {
                placeholders.Add(new Placeholder(configuration, match.Value, match.Groups["path"].Value));
            }
        }

        public string FillTemplate(object templateValueWrapper)
        {
            StringBuilder sb = new StringBuilder(orgTemplateString);
            foreach (var placeholder in placeholders)
            {
                sb.Replace(placeholder.GetOrgPlaceholder(), placeholder.LoadValueString(templateValueWrapper));
            }
            return sb.ToString();
        }
    }

    class Placeholder
    {
        private string _placeholder;
        private string _formatString;
        private string _orgPlaceholder;
        private Configuration _configuration;

        public Placeholder(Configuration configuration, string orgPlaceholder, string placeholderPath)
        {
            _configuration = configuration;
            _orgPlaceholder = orgPlaceholder;
            LoadPlaceholder(placeholderPath.Trim());
        }

        private void LoadPlaceholder(string placeholderPath)
        {
            var splittedPlaceholder = placeholderPath.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedPlaceholder.Length == 1)
            {
                _placeholder = splittedPlaceholder[0];
            }
            else if (splittedPlaceholder.Length == 2)
            {
                _placeholder = splittedPlaceholder[0];
                _formatString = splittedPlaceholder[1];
            }
            else
            {
                throw new FormatException("Wrong placeholder format.");
            }
        }

        public string GetOrgPlaceholder()
        {
            return _orgPlaceholder;
        }

        public string LoadValueString(object templateValueWrapper)
        {
            var objectPaths = _placeholder.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return LoadValueString(templateValueWrapper, objectPaths, 0);
        }

        private string LoadValueString(object valueWrapper, IEnumerable<string> objectValuePath, int objectDepth)
        {
            if (objectDepth > _configuration.MaxObjectDepth)
            {
                throw new IndexOutOfRangeException("Max object path depth reached.");
            }
            else if (objectValuePath.Count() > 1)
            {
                var propertyName = objectValuePath.First();
                bool isEnumeration = TryParseEnumeration(propertyName, ref propertyName, out int idx);
                var property = valueWrapper.GetType().GetProperties().FirstOrDefault(x => x.Name == propertyName);
                if (property != null)
                {
                    var obj = property.GetValue(valueWrapper);
                    if (isEnumeration)
                    {
                        var dataPicker = _configuration.GetDataPicker(property.PropertyType);
                        return LoadValueString(dataPicker.GetObjectInCollection(obj, idx), objectValuePath.Skip(1), ++objectDepth);
                    }
                    else
                    {
                        return LoadValueString(obj, objectValuePath.Skip(1), ++objectDepth);
                    }
                }
            }
            else if (objectValuePath.Count() == 1)
            {
                var propertyName = objectValuePath.First();
                bool isEnumeration = TryParseEnumeration(propertyName, ref propertyName, out int idx);
                var property = valueWrapper.GetType().GetProperties().FirstOrDefault(x => x.Name == propertyName);
                if (property != null)
                {
                    var obj = property.GetValue(valueWrapper);
                    if (isEnumeration)
                    {
                        var dataPicker = _configuration.GetDataPicker(property.PropertyType);
                        obj = dataPicker.GetObjectInCollection(obj, idx);
                    }
                    var converter = _configuration.GetDataConverter(property.PropertyType);

                    return converter.Convert(obj, _formatString, _configuration.FormatProvider);
                }
            }
            return null;
        }

        private bool TryParseEnumeration(string pathPart, ref string propertyNameWithoutIndex, out int enumerationIndex)
        {
            enumerationIndex = -1;
            if (pathPart.Contains("[") && pathPart.Contains("]"))
            {
                var match = Regex.Match(pathPart, @"\[([0-9]+)\]");
                if (match.Success && int.TryParse(match.Value.Replace("[", "").Replace("]", ""), out enumerationIndex))
                {
                    propertyNameWithoutIndex = propertyNameWithoutIndex.Replace(match.Value, "");
                    return true;
                }
            }
            return false;
        }
    }
}
