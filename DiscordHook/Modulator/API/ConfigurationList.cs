using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordHook.Modulator.API
{
    public class ConfigurationList : ICollection
    {
        #region Properties
        public Dictionary<string, object> Configurations { get; protected set; }

        public object this[string configurationName]
        {
            get => !Configurations.ContainsKey(configurationName) ? null : Configurations[configurationName];
            set
            {
                if (Configurations.ContainsKey(configurationName))
                    Configurations[configurationName] = value;
                else
                    Configurations.Add(configurationName, value);
            }
        }
        public KeyValuePair<string, object> this[int index]
        {
            get => Configurations.ElementAt(index);
            set => Configurations[Configurations.ElementAt(index).Key] = value.Value;
        }

        public int Count => Configurations.Count;
        public object SyncRoot => throw new NotImplementedException();
        public bool IsSynchronized => throw new NotImplementedException();
        #endregion

        public ConfigurationList() =>
            Configurations = new Dictionary<string, object>();

        #region Collection Functions
        public void Add(string configurationName, object configuration)
        {
            if (!Configurations.ContainsKey(configurationName))
                Configurations.Add(configurationName, configuration);
        }
        public void Add(KeyValuePair<string, object> kvp) => this.Add(kvp.Key, kvp.Value);

        public void Remove(string configurationName)
        {
            if (Configurations.ContainsKey(configurationName))
                Configurations.Remove(configurationName);
        }
        public void RemoveAt(int index) => this.Remove(this[index].Key);

        public void AddRange(IEnumerable<KeyValuePair<string, object>> list)
        {
            foreach(KeyValuePair<string, object> x in list)
                this.Add(x.Key, x.Value);
        }
        public void AddRange(ConfigurationList list)
        {
            for (int i = 0; i < list.Count; i++)
                this.Add(list[i]);
        }

        public bool ContainsKey(string key) => Configurations.ContainsKey(key);

        IEnumerator IEnumerable.GetEnumerator() => Configurations.GetEnumerator();

        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < (this.Count - index); i++)
                array.SetValue(this[index + i], i);
        }

        public void Clear() => Configurations.Clear();
        #endregion
    }
}
