using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;

namespace Contal.IwQuick.Data
{
    [Serializable()]
    [XmlRoot("hashmap")]
    public class XmlSerializableDictionary<TKey, TValue>:Dictionary<TKey, TValue>, IXmlSerializable
    {

        private const string _recordElementName = "record";
        private const string _keyElementName = "key";
        private const string _valueElementName = "value";

        public XmlSerializableDictionary()
            :base()
        {
        }

        public XmlSerializableDictionary(int i_iCapacity)
            :base(i_iCapacity)
        {
        }

        public XmlSerializableDictionary(IDictionary<TKey,TValue> dictionary)
            :base(dictionary)
        {
        }


        #region IXmlSerializable Members

            XmlSerializer _keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer _valueSerializer = new XmlSerializer(typeof(TValue));

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader xmlReader)
            {
                bool bWasEmpty = xmlReader.IsEmptyElement;

                xmlReader.Read();

                if (bWasEmpty)
                    return;

                while (xmlReader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    xmlReader.ReadStartElement(_recordElementName);

                    xmlReader.ReadStartElement(_keyElementName);

                    TKey key = (TKey)_keySerializer.Deserialize(xmlReader);

                    xmlReader.ReadEndElement();

                    xmlReader.ReadStartElement(_valueElementName);

                    TValue value = (TValue)_valueSerializer.Deserialize(xmlReader);

                    xmlReader.ReadEndElement();

                    if (!this.ContainsKey(key))
                        this.Add(key, value);

                    xmlReader.ReadEndElement();

                    xmlReader.MoveToContent();

                }

                xmlReader.ReadEndElement();

            }

            public void WriteXml(System.Xml.XmlWriter xmlWriter)
            {
                foreach (TKey aKey in this.Keys)
                {

                    xmlWriter.WriteStartElement(_recordElementName);

                    xmlWriter.WriteStartElement(_keyElementName);

                    _keySerializer.Serialize(xmlWriter, aKey);

                    xmlWriter.WriteEndElement();



                    xmlWriter.WriteStartElement(_valueElementName);

                    TValue aValue = this[aKey];

                    _valueSerializer.Serialize(xmlWriter, aValue);

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();

                }

            }

            #endregion



            
    }
}
