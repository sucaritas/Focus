namespace Focus
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Container
    {
        internal IDictionary<string, Property> RootObjects { get; set; } = new Dictionary<string, Property>();
        internal IDictionary<string, Property> ContainerObjects { get; set; } = new Dictionary<string, Property>();
        private ConcurrentDictionary<string, ConcurrentDictionary<Type, object>> TypeCache { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<Type, object>>();
        private ConcurrentDictionary<string, JToken> JTokenCache { get; } = new ConcurrentDictionary<string, JToken>();
        private readonly string JSON;
        private JToken jsonObject;
        public bool IsArray { get; private set; }
        public bool CacheResults { get; set; } = true;
        public ConcurrentBag<string> Constants { get; private set; } = new ConcurrentBag<string>();

        #region Constructors
        public Container(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException(nameof(json) + " cannot be null or empty");
            }

            this.JSON = json;
            var jToken = JToken.Parse(this.JSON);
            if (jToken.Type != JTokenType.Object && jToken.Type != JTokenType.Array)
            {
                throw new ArgumentException(nameof(json) + " must be an object or array");
            }

            this.RootObjects = Property.GetRootObjects(jToken);
            init(jToken);
        }

        public Container(JToken jobject)
        {
            this.RootObjects = Property.GetRootObjects(jobject);
            init(jobject);
        }

        internal Container(JToken jobject, IDictionary<string, Property> rootObjects)
        {
            this.RootObjects = rootObjects;
            init(jobject);
        }

        private void init(JToken jobject)
        {
            if (jobject == null)
            {
                throw new ArgumentException(nameof(jobject) + " cannot be null or empty");
            }

            this.IsArray = jobject is JArray;
            this.jsonObject = jobject;
            this.ContainerObjects = Property.GetRootObjects(this.jsonObject);
        }

        #endregion

        internal JToken Focus(IDictionary<string, string> lenses, IDictionary<string, Property> rootObjects)
        {
            return this.IsArray ? FocusArray(lenses, rootObjects) : FocusObject(lenses, rootObjects);
        }

        public JToken Focus()
        {
            return this.Focus(new Dictionary<string, string>());
        }

        public JToken Focus(IDictionary<string, string> lenses)
        {
            lenses = lenses ?? new Dictionary<string, string>();
            JToken result;
            string key = GetCacheKey(lenses);
            if (this.CacheResults && this.JTokenCache.TryGetValue(key, out result))
            {
                return result;
            }

            result = this.IsArray ? FocusArray(lenses, this.RootObjects) : FocusObject(lenses, this.RootObjects);

            if (this.CacheResults)
            {
                this.JTokenCache.AddOrUpdate(key, result, (o, obj) => result);
            }

            return result;
        }

        private JToken FocusArray(IDictionary<string, string> lenses, IDictionary<string, Property> rootObjects)
        {
            if ((this.jsonObject as JArray) == null)
            {
                return null;
            }

            var array = new JArray();
            if (this.ContainerObjects.Count == 0)
            {
                return array;
            }

            lenses = InjectConstants(lenses);
            foreach (var obj in this.ContainerObjects)
            {
                array.Add(obj.Value.Focus(lenses, rootObjects));
            }

            return array;
        }

        private JToken FocusObject(IDictionary<string, string> lenses, IDictionary<string, Property> rootObjects)
        {
            if (this.jsonObject == null)
            {
                return null;
            }

            var jObject = new JObject();
            if (this.ContainerObjects.Count == 0)
            {
                return jObject;
            }

            lenses = InjectConstants(lenses);
            foreach (var obj in this.ContainerObjects)
            {
                jObject.Add(obj.Key, obj.Value.Focus(lenses, rootObjects));
            }

            return jObject;
        }

        public T Focus<T>()
        {
            return this.Focus<T>(new Dictionary<string, string>());
        }

        public T Focus<T>(IDictionary<string, string> lenses)
        {
            if (lenses == null)
            {
                throw new ArgumentNullException(nameof(lenses));
            }

            if (this.CacheResults)
            {
                var key = GetCacheKey(lenses);
                object value;
                ConcurrentDictionary<Type, object> innerCache;

                innerCache = this.TypeCache.GetOrAdd(key, new ConcurrentDictionary<Type, object>());
                if (innerCache.TryGetValue(typeof(T), out value))
                {
                    return (T)value;
                }
            }

            var jobject = this.Focus(lenses);
            var convertedObject = jobject.ToObject<T>();

            if (this.CacheResults)
            {
                var key = GetCacheKey(lenses);
                ConcurrentDictionary<Type, object> innerCache;
                innerCache = this.TypeCache.GetOrAdd(key, new ConcurrentDictionary<Type, object>());
                this.TypeCache.AddOrUpdate(key, innerCache, (o, obj) => innerCache);
            }

            return convertedObject;
        }

        private static string GetCacheKey(IDictionary<string, string> lenses)
        {
            return string.Join("|", lenses.OrderBy(x => x.Key).Select(x => x.Key + ":" + x.Value));
        }

        private IDictionary<string, string> InjectConstants(IDictionary<string, string> original)
        {
            var dic = new Dictionary<string, string>(original);
            foreach (var constant in Constants)
            {
                if (!dic.ContainsKey(constant))
                {
                    dic.Add(constant, string.Empty);
                }
            }
            return dic;
        }

        public void AddConstantRange(IEnumerable<string> constants)
        {
            if (constants == null)
            {
                return;
            }

            foreach (var constant in constants)
            {
                this.Constants.Add(constant);
            }
        }

        public void ClearConstants()
        {
            this.Constants = new ConcurrentBag<string>();
        }
    }
}