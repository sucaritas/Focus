namespace Focus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json.Linq;

    public class Property
    {
        private struct PropertyTuple
        {
            public List<string> lenses;
            public JToken jToken;
            public Container Container;
        }

        private List<PropertyTuple> items = new List<PropertyTuple>();

        public void Add(Dictionary<string, string> lenses, JToken value)
        {
            if (lenses == null) { throw new ArgumentException(nameof(lenses) + " cannot be null"); }

            // Ignore null values. because we will not be able to interpret them.
            if (value == null){ return; }

            items.Add(new PropertyTuple()
            {
                lenses = lenses.Select(x=> x.Key.Trim() + ":" + x.Value.Trim()).ToList(),
                Container = new Container(value),
                jToken = value,
            });
        }

        /// <summary>
        /// Extracts the conditions in the property name to be used later on focus.
        /// Format is:
        /// {
        ///     "SimpleProperty" : {/*Some value*/},
        ///     "SimpleProperty|Constatnt" : {/*Some value*/},
        ///     "SimpleProperty|SomeLens:WithValue|SecondLens:SecondValue" : {/*Some value*/},
        ///     "SimpleProperty|Constatnt|SomeLens:WithValue" : {/*Some value*/},
        /// }
        /// </summary>
        internal static bool TryParsePropertyName(JProperty property, out string propertyName, out Dictionary<string, string> lenses)
        {
            var propertyParts = property.Name.Split('|');
            propertyName = propertyParts[0];
            lenses = new Dictionary<string, string>();
            foreach (var lens in propertyParts.Skip(1).OrderBy(x => x))
            {
                var lensParts = lens.Split(':');
                lenses.Add(lensParts[0], lensParts.Length > 1 ? (string.Join(":", lensParts.Skip(1))) : string.Empty);
            }

            return true;
        }

        public static Dictionary<string, Property> GetRootObjects(JToken container)
        {
            var ret = new Dictionary<string, Property>();
            Property prop;
            var array = container as JArray;
            if (array != null)
            {
                for (int i=0; i < array.Count; i++)
                {
                    prop = new Property();
                    prop.Add(new Dictionary<string, string>(), array[i]);
                    ret.Add(i.ToString(), prop);
                }

                return ret;
            }

            var obj = container as JObject;
            if (obj == null)
            {
                return new Dictionary<string, Property>();
            }

            foreach (var property in obj.Properties())
            {
                string propertyName;
                Dictionary<string, string> lenses;
                if (!Property.TryParsePropertyName(property, out propertyName, out lenses))
                {
                    throw new FormatException("Could not parse property " + property.Name + " on path " + property.Path);
                }

                if (!ret.TryGetValue(propertyName, out prop))
                {
                    prop = new Property();
                    ret.Add(propertyName, prop);
                }

                prop.Add(lenses, property.Value);
            }

            return ret;
        }

        public JToken Focus(Dictionary<string, string> lenses, Dictionary<string, Property> rootObjects)
        {
            var lensesSet = lenses.Select(x => x.Key.Trim() + ":" + x.Value.Trim()).ToList();
            PropertyTuple tuple = this.items.FirstOrDefault(x=> !x.lenses.Except(lensesSet).Any());

            if (tuple.jToken == null)
            {
                return null;
            }

            if (tuple.jToken.Type != JTokenType.Object && tuple.jToken.Type != JTokenType.Array)
            {
                if (tuple.jToken.Type == JTokenType.String
                    && ((JValue)tuple.jToken).Value.ToString().StartsWith("##")
                    && ((JValue)tuple.jToken).Value.ToString().EndsWith("##"))
                {
                    Property rootObj;
                    if (!rootObjects.TryGetValue(((JValue)tuple.jToken).Value.ToString().Trim('#'), out rootObj))
                    {
                        return tuple.jToken;
                    }

                    return rootObj.Focus(lenses, rootObjects);
                }

                return tuple.jToken;
            }

            return tuple.Container.Focus(lenses, rootObjects);
        }
    }
}
