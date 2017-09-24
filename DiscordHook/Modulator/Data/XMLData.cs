using System;
using System.Xml;
using System.Collections;
using System.Linq;

using XFile = System.IO.File;

namespace Modulator.Data
{
    public class XMLData : ICollection
    {
        #region Properties
        public string File { get; private set; }
        public bool CreatedNew { get; private set; }

        public XmlDocument Document { get; private set; }
        public XmlNode RootNode { get; private set; }

        public int Count => RootNode.ChildNodes.Count;
        public object SyncRoot => RootNode;
        public bool IsSynchronized => true;

        public XmlNode this[string key]
        {
            get => RootNode.SelectSingleNode(key);
            set
            {
                XmlNode node = RootNode.SelectSingleNode(key);

                if(node == null)
                {
                    node = MakeXPath(Document, key);
                    node.InnerXml = value.InnerXml;
                }
                else
                {
                    node.InnerXml = value.InnerXml;
                }
            }
        }
        #endregion

        public XMLData(string path)
        {
            File = path;
            CreatedNew = !XFile.Exists(File);

            Reload();
        }

        #region Static Functions
        public static XmlNode MakeXPath(XmlDocument doc, string xpath) =>
            MakeXPath(doc, doc as XmlNode, xpath);

        public static XmlNode MakeXPath(XmlDocument doc, XmlNode parent, string xpath)
        {
            string[] partsOfXPath = xpath.Trim('/').Split('/');
            string nextNodeInXPath = partsOfXPath.First();
            if (string.IsNullOrEmpty(nextNodeInXPath))
                return parent;
            XmlNode node = parent.SelectSingleNode(nextNodeInXPath);

            if (node == null)
                node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));
            string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());

            return MakeXPath(doc, node, rest);
        }
        #endregion

        #region Functions
        public void Reload()
        {
            Document = new XmlDocument();

            if (CreatedNew)
            {
                RootNode = Document.CreateElement("Data");
                Document.AppendChild(RootNode);
            }
            else
            {
                Document.Load(File);
                RootNode = Document.DocumentElement;
            }
        }
        public void Save() => Document.Save(File);

        public void Add(string key, object value) => this[key].InnerXml = value.ToString();
        public bool ContainsKey(string key) => Document.SelectSingleNode(key) != null;
        public void Remove(string key) => Document.RemoveChild(this[key]);

        public void CopyTo(Array array, int index)
        {
            for(int i = 0; i < Count; i++)
                array.SetValue(RootNode.ChildNodes[i], index + i);
        }
        public IEnumerator GetEnumerator() => RootNode.ChildNodes.GetEnumerator();
        #endregion
    }
}
