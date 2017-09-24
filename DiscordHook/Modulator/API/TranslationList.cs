using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulator.Data;

namespace Modulator.API
{
    public class TranslationList : ICollection
    {
        #region Variables
        private XMLData XML;
        #endregion

        #region Properties
        public string this[string key]
        {
            get => XML[key].InnerXml;
            set => XML[key].InnerXml = value;
        }

        public int Count => XML.Count;
        public object SyncRoot => null;
        public bool IsSynchronized => false;
        #endregion

        public TranslationList(string FileName = null)
        {
            if (!Directory.Exists(ModulatorContants.TranslationPath))
                Directory.CreateDirectory(ModulatorContants.TranslationPath);
            string file = GetType().Assembly.GetName().Name;
            if (!string.IsNullOrEmpty(FileName))
                file += "." + FileName;
            file += ".xml";

            XML = new XMLData(ModulatorContants.TranslationPath + "/" + file);
        }

        #region Collection Functions
        public void Add(string key, string value)
        {
            if (!XML.ContainsKey(key))
                XML.Add(key, value);

            XML.Save();
        }
        public void Add(KeyValuePair<string, string> kvp) => Add(kvp.Key, kvp.Value);

        public void Remove(string key)
        {
            if (XML.ContainsKey(key))
                XML.Remove(key);

            XML.Save();
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> list)
        {
            foreach (KeyValuePair<string, string> x in list)
                Add(x.Key, x.Value);
        }
        public void AddRange(TranslationList list)
        {
            foreach (KeyValuePair<string, string> kvp in list)
                Add(kvp);
        }

        public bool ContainsKey(string key) => XML.ContainsKey(key);

        IEnumerator IEnumerable.GetEnumerator() => XML.GetEnumerator();

        public void CopyTo(Array array, int index)
        {
            int i = 0;
            foreach(KeyValuePair<string, string> kvp in this)
                array.SetValue(kvp, i++);
        }

        public string Translate(string key, params object[] values) => string.Format(this[key], values);
        #endregion
    }
}
