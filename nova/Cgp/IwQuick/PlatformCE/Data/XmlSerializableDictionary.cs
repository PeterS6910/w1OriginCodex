using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [XmlRoot("hashmap")]
    public class XmlSerializableDictionary<TKey, TValue>:Dictionary<TKey, TValue>, IXmlSerializable
    {

        private const string _recordElementName = "record";
        private const string _keyElementName = "key";
        private const string _valueElementName = "value";

        /// <summary>
        /// 
        /// </summary>
        public XmlSerializableDictionary()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_iCapacity"></param>
        public XmlSerializableDictionary(int i_iCapacity)
            :base(i_iCapacity)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public XmlSerializableDictionary(IDictionary<TKey,TValue> dictionary)
            :base(dictionary)
        {
        }


        #region IXmlSerializable Members

            XmlSerializer _keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer _valueSerializer = new XmlSerializer(typeof(TValue));

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public XmlSchema GetSchema()
            {
                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xmlReader"></param>
            public void ReadXml(XmlReader xmlReader)
            {
                bool bWasEmpty = xmlReader.IsEmptyElement;

                xmlReader.Read();

                if (bWasEmpty)
                    return;

                while (xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    xmlReader.ReadStartElement(_recordElementName);

                    xmlReader.ReadStartElement(_keyElementName);

                    TKey key = (TKey)_keySerializer.Deserialize(xmlReader);

                    xmlReader.ReadEndElement();

                    xmlReader.ReadStartElement(_valueElementName);

                    TValue value = (TValue)_valueSerializer.Deserialize(xmlReader);

                    xmlReader.ReadEndElement();

                    if (!ContainsKey(key))
                        Add(key, value);

                    xmlReader.ReadEndElement();

                    xmlReader.MoveToContent();

                }

                xmlReader.ReadEndElement();

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xmlWriter"></param>
            public void WriteXml(XmlWriter xmlWriter)
            {
                foreach (TKey aKey in Keys)
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
