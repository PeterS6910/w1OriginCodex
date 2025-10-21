using System.Xml;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public  static class XmlReaderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static bool ReadToNextElementStart(this XmlReader xmlReader)
        {
            if (ReferenceEquals(xmlReader, null))
                return false;

            do
            {
                if (!xmlReader.Read() ||
                    xmlReader.NodeType == XmlNodeType.EndElement)
                    return false;


            } while (xmlReader.NodeType != XmlNodeType.Element);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static bool TryClose(this XmlReader xmlReader)
        {
            if (ReferenceEquals(xmlReader, null))
                return false;

            try
            {
                xmlReader.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
