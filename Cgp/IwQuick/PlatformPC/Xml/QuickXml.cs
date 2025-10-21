using System;
using System.Xml;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class QuickXml:ADisposable
    {
        private XmlDocument _doc = null;
        private XmlNode _relNode = null;
        private string _filePath = String.Empty;
        private bool _loaded = false;
        private bool _readOnly = false;

        private const string ELEMENT = "element";

        private bool _absoluteAddressPrefered = false;

        public bool AbsoluteAddressPrefered
        {
            get { return _absoluteAddressPrefered; }
            set { _absoluteAddressPrefered = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public QuickXml()
        {
            _doc = new XmlDocument();
            _relNode = _doc;
        }

        /// <summary>
        /// copying constructor
        /// </summary>
        /// <param name="quickXml"></param>
        public QuickXml([NotNull] QuickXml quickXml)
        {
            Validator.CheckForNull(quickXml,"quickXml");

            _doc = new XmlDocument();
            
            // let the eventual exception flow up
            _doc.LoadXml(quickXml._doc.OuterXml);
            _loaded = true;
            
            _relNode = _doc;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OuterXml
        {
            get
            {
                return _doc.OuterXml;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean IsLoaded()
        {
            return _loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public virtual Boolean Load(String fileName, bool isReadOnly)
        {
            return Load(fileName, isReadOnly, false);
        }

        /// <summary>
        /// used for loading existing file in read-only or read-write mode
        /// or for assigning new non-existing file for read-write mode
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="fileMustExist"></param>
        /// <returns></returns>
        public virtual Boolean Load(String fileName, bool isReadOnly, bool fileMustExist)
        {
            Unload();

            if (null == _doc)
                _doc = new XmlDocument();

            _readOnly = isReadOnly;

            if (fileName == null)
                return false;

            bool bFileExists = File.Exists(fileName);
            if (fileMustExist &&
                !bFileExists)
                return false;

            if (isReadOnly && !bFileExists)
                return false;

            _filePath = fileName;
            try
            {
                if (bFileExists)
                    _doc.Load(fileName);

                _loaded = true;
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public virtual Boolean LoadXml(String xmlString)
        {
            Unload();

            _readOnly = false;

            _filePath = null;
            try
            {
                _doc.LoadXml(xmlString);

                _loaded = true;
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Save(string filePath)
        {
            if (Validator.IsNullString(filePath))
                return false;

            try
            {
                _doc.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (null != _filePath)
                return Save(_filePath);
            return false;
        }

        private readonly object _unloadLock = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean Unload()
        {
            lock (_unloadLock)
            {
                if (!_loaded)
                    return false;

                try
                {
                    //if (!_readOnly)
                    //    Save();

                    if (!_readOnly && !Save())
                        return false;

                    _filePath = String.Empty;
                    _loaded = false;
                    _doc = null;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private XmlNode SearchNode(XmlNode root, String xpath)
        {
            if (ReferenceEquals(root,null) ||
                string.IsNullOrEmpty(xpath))
                return null;

            return root.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        private XmlNodeList SearchNodes(XmlNode root, String xpath)
        {
            if (ReferenceEquals(root,null) ||
                string.IsNullOrEmpty(xpath))
                return null;

            return root.SelectNodes(xpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public String GetText(String xpath)
        {
            if (Validator.IsNullString(xpath))
                return null;

            XmlNode aNode = SearchNode(_relNode, xpath);
            if (aNode == null)
                return null;

            return aNode.InnerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="outputValue"></param>
        /// <returns></returns>
        public bool TryGetInt(String xpath,ref int outputValue)
        {
            string strValue = GetText(xpath);
            if (Validator.IsNullString(strValue))
                return false;

            return int.TryParse(strValue, out outputValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="outputValue"></param>
        /// <returns></returns>
        public bool TryGetFloat(String xpath, ref double outputValue)
        {
            string strValue = GetText(xpath);
            if (Validator.IsNullString(strValue))
                return false;

            return double.TryParse(strValue, out outputValue);
        }

        public string[] GetTexts(String xpath)
        {
            if (Validator.IsNullString(xpath))
                return null;

            XmlNodeList aNodeList = SearchNodes(_relNode, xpath);
            if (null == aNodeList ||
                0 == aNodeList.Count)
                return null;

            string[] arTexts = new string[aNodeList.Count];

            int i = 0;
            foreach (XmlNode aNode in aNodeList)
            {
                arTexts[i++] = aNode.InnerText;
            }

            return arTexts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="textsToOutput"></param>
        public void GetTexts(String xpath,ref IList<string> textsToOutput)
        {
            if (Validator.IsNullString(xpath))
                return;

            if (null == textsToOutput)
                textsToOutput = new List<string>(4);

            XmlNodeList aNodeList = SearchNodes(_relNode, xpath);
            if (null == aNodeList ||
                0 == aNodeList.Count)
                return;

            foreach (XmlNode aNode in aNodeList)
            {
                textsToOutput.Add(aNode.InnerText);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNode GetNode(String xpath)
        {
            if (Validator.IsNullString(xpath))
                return null;

            XmlNode aNode = SearchNode(_relNode, xpath);
            return aNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNodeList GetNodes(String xpath)
        {
            if (Validator.IsNullString(xpath))
                return null;

            XmlNodeList aNodes = SearchNodes(_relNode, xpath);
            return aNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultText"></param>
        /// <param name="fromRootNode"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public XmlNode GetOrAddNode(string defaultText, bool fromRootNode, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return null;

            XmlNode aRelNode = fromRootNode ? _doc : _relNode;

            bool bNew = false;
            XmlNode aTmpNode = null;
            foreach (string strNode in pathNodes)
            {
                aTmpNode = SearchNode(aRelNode, strNode);
                if (null == aTmpNode)
                {
                    try
                    {
                        aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);
                        aRelNode.AppendChild(aTmpNode);
                        bNew = true;
                    }
                    catch
                    {
                        return null;
                    }
                }

                aRelNode = aTmpNode;
            }

            if (bNew && null != defaultText)
                aRelNode.InnerText = defaultText;

            return aTmpNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultText"></param>
        /// <param name="fromRootNode"></param>
        /// <param name="simpleXPath"></param>
        /// <returns></returns>
        public XmlNode GetOrAddNode(string defaultText, bool fromRootNode, string simpleXPath)
        {
            if (Validator.IsNullString(simpleXPath))
                return null;

            XmlNode aRelNode = fromRootNode ? _doc : _relNode;

            bool bNew = false;
            XmlNode aTmpNode = null;

            string[] arPathNodes = simpleXPath.Split(StringConstants.SLASH[0]);

            int i = 0;
            foreach (string strNode in arPathNodes)
            {
                if (0 == i && arPathNodes.Length > 1) // this condition has only meaning for simpleXPath starting with '/' character
                {
                    i++;
                    continue;
                }

                aTmpNode = SearchNode(aRelNode, strNode);
                if (null == aTmpNode)
                {
                    try
                    {
                        aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);
                        aRelNode.AppendChild(aTmpNode);
                        bNew = true;
                    }
                    catch
                    {
                        return null;
                    }
                }

                aRelNode = aTmpNode;

                i++;
            }

            if (bNew && null != defaultText)
                aRelNode.InnerText = defaultText;

            return aTmpNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public XmlNode GetOrAddNode(params string[] pathNodes)
        {
            return GetOrAddNode(null, _absoluteAddressPrefered, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromRootNode"></param>
        /// <param name="count"></param>
        /// <param name="simpleXPath"></param>
        /// <returns></returns>
        public XmlNodeList GetOrAddNodes(bool fromRootNode, int count, string simpleXPath)
        {
            if (Validator.IsNullString(simpleXPath))
                return null;

            if (0 >= count)
                return null;

            XmlNode aRelNode = fromRootNode ? _doc : _relNode;


            XmlNode aTmpNode = null;
            XmlNodeList aNodeList = null;

            string[] arPathNodes = simpleXPath.Split(StringConstants.SLASH[0]);

            int i = 0;
            foreach (string strNode in arPathNodes)
            {
                if (0 == i)
                {
                    i++;
                    continue;
                }

                if (i < arPathNodes.Length - 1)
                {
                    aTmpNode = SearchNode(aRelNode, strNode);
                    if (null == aTmpNode)
                    {
                        try
                        {
                            aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);

                            if (aRelNode == null)
                                return null;

                            aRelNode.AppendChild(aTmpNode);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    aNodeList = SearchNodes(aRelNode, strNode);
                    if (null == aNodeList)
                        return null;

                    // do not apply for root element - can be only one
                    if (1 != i &&
                        aNodeList.Count < count)
                    {
                        // fill with remaining nodes
                        for (int k = aNodeList.Count; k < count; k++)
                        {
                            try
                            {
                                aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);
                                if (aRelNode == null)
                                    return null;
                                aRelNode.AppendChild(aTmpNode);
                            }
                            catch
                            {
                                return null;
                            }
                        }

                        aNodeList = SearchNodes(aRelNode, strNode);
                    }
                }

                i++;
                aRelNode = aTmpNode;
            }

            return aNodeList;
        }

        public XmlNodeList GetOrAddNodes(bool fromRootNode, int count, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return null;

            if (0 >= count)
                return null;

            XmlNode aRelNode = fromRootNode ? _doc : _relNode;


            XmlNode aTmpNode = null;
            XmlNodeList aNodeList = null;


            int i = 0;
            foreach (string strNode in pathNodes)
            {
                if (i < pathNodes.Length - 1)
                {
                    aTmpNode = SearchNode(aRelNode, strNode);
                    if (null == aTmpNode)
                    {
                        try
                        {
                            aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);
                            if (aRelNode == null)
                                return null;
                            aRelNode.AppendChild(aTmpNode);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    aNodeList = SearchNodes(aRelNode, strNode);
                    if (null == aNodeList)
                        return null;

                    // do not apply for root element - can be only one
                    if (0 != i &&
                        aNodeList.Count < count)
                    {
                        // fill with remaining nodes
                        for (int k = aNodeList.Count; k < count; k++)
                        {
                            try
                            {
                                aTmpNode = _doc.CreateNode(ELEMENT, strNode, String.Empty);
                                if (aRelNode == null)
                                    return null;
                                aRelNode.AppendChild(aTmpNode);
                            }
                            catch
                            {
                                return null;
                            }
                        }

                        aNodeList = SearchNodes(aRelNode, strNode);
                    }
                }

                i++;
                aRelNode = aTmpNode;
            }

            return aNodeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="simpleXPath"></param>
        /// <returns></returns>
        public XmlNodeList GetOrAddNodes(int count,string simpleXPath)
        {
            return GetOrAddNodes(_absoluteAddressPrefered, count, simpleXPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public XmlNodeList GetOrAddNodes(int count, params string[] pathNodes)
        {
            return GetOrAddNodes(_absoluteAddressPrefered, count, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultText"></param>
        /// <param name="fromRootNode"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public String GetOrAddText(string defaultText, bool fromRootNode, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return null;

            XmlNode aNode = GetOrAddNode(defaultText, fromRootNode, pathNodes);
            if (null == aNode)
                return null;

            return aNode.InnerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public String GetOrAddText(params string[] pathNodes)
        {
            return GetOrAddText(null, _absoluteAddressPrefered, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromRootNode"></param>
        /// <param name="multiplicity"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public String[] GetOrAddTexts(bool fromRootNode, int multiplicity, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length ||
                0 >= multiplicity)
                return null;

            XmlNodeList aNodeList = GetOrAddNodes(fromRootNode, multiplicity, pathNodes);

            if (null == aNodeList ||
                0 == aNodeList.Count)
                return null;

            string[] arTexts = new string[aNodeList.Count];
            int k = 0;
            foreach (XmlNode aCurNode in aNodeList)
                arTexts[k++] = aCurNode.InnerText;

            return arTexts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromRootNode"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public String[] GetOrAddTexts(bool fromRootNode, params string[] pathNodes)
        {
            return GetOrAddTexts(fromRootNode, 1, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public String[] GetOrAddTexts(params string[] pathNodes)
        {
            return GetOrAddTexts(_absoluteAddressPrefered, 1, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromRootNode"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddText(string value, bool fromRootNode, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return false;

            if (null == value)
                return false;

            XmlNode aNode = GetOrAddNode(null, fromRootNode, pathNodes);
            if (null == aNode)
                return false;

            try
            {
                aNode.InnerText = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddText(string value, params string[] pathNodes)
        {
            return SetOrAddText(value, _absoluteAddressPrefered, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromRootNode"></param>
        /// <param name="simpleXPath"></param>
        /// <returns></returns>
        public bool SetOrAddText(string value, bool fromRootNode, string simpleXPath)
        {
            if (Validator.IsNullString(simpleXPath))
                return false;

            if (null == value)
                return false;

            XmlNode aNode = GetOrAddNode(null, fromRootNode, simpleXPath);
            if (null == aNode)
                return false;

            try
            {
                aNode.InnerText = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="i_strSimpleXPath"></param>
        /// <returns></returns>
        public bool SetOrAddText(string value, string i_strSimpleXPath)
        {
            return SetOrAddText(value, _absoluteAddressPrefered, i_strSimpleXPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromRootNode"></param>
        /// <param name="values"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddTexts(bool fromRootNode, String[] values, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return false;

            if (null == values ||
                values.Length == 0)
            {
                return false;
            }

            XmlNodeList aNodeList = GetOrAddNodes(fromRootNode, values.Length, pathNodes);

            if (null == aNodeList ||
                0 == aNodeList.Count)
                return false;

            int k = 0;
            foreach (XmlNode aCurNode in aNodeList)
                aCurNode.InnerText = values[k++];

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddTexts(String[] values, params string[] pathNodes)
        {
            return SetOrAddTexts(_absoluteAddressPrefered, values, pathNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromRootNode"></param>
        /// <param name="values"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddTexts(bool fromRootNode, IList values, params string[] pathNodes)
        {
            if (null == pathNodes ||
                0 == pathNodes.Length)
                return false;

            if (null == values ||
                values.Count == 0)
            {
                return false;
            }

            XmlNodeList aNodeList = GetOrAddNodes(fromRootNode, values.Count, pathNodes);

            if (null == aNodeList ||
                0 == aNodeList.Count)
                return false;

            int k = 0;
            foreach (XmlNode aCurNode in aNodeList)
            {
                if (null != values[k])
                    aCurNode.InnerText = values[k].ToString();
                k++;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public bool SetOrAddTexts(IList values, params string[] pathNodes)
        {
            return SetOrAddTexts(_absoluteAddressPrefered, values, pathNodes);
        }

        public bool SetOrAddTexts(bool fromRootNode, IList values, string simpleXPath)
        {
            if (Validator.IsNullString(simpleXPath))
                return false;

            if (null == values ||
                values.Count == 0)
            {
                return false;
            }

            XmlNodeList aNodeList = GetOrAddNodes(fromRootNode, values.Count, simpleXPath);

            if (null == aNodeList ||
                0 == aNodeList.Count)
                return false;

            int k = 0;
            foreach (XmlNode aCurNode in aNodeList)
            {
                if (null != values[k])
                    aCurNode.InnerText = values[k].ToString();
                k++;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="simpleXPath"></param>
        /// <returns></returns>
        public bool SetOrAddTexts(IList values, string simpleXPath)
        {
            return SetOrAddTexts(_absoluteAddressPrefered, values, simpleXPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public Boolean SetText(String value,String xpath)
        {
            if (xpath == null)
                return false;

            XmlNode aNode = SearchNode(_relNode, xpath);
            if (aNode == null)
                return false;

            aNode.InnerText = value;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean SetCdata(String xpath, String value)
        {
            if (xpath == null)
                return false;

            XmlNode aNode = SearchNode(_relNode, xpath);
            if (aNode == null)
                return false;

            aNode.InnerXml =
                "<![CDATA[" + value + "]]>";

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public Boolean SetRoot(String xpath)
        {
            if (xpath == null)
                return false;

            XmlNode aNode = SearchNode(_doc, xpath);
            if (aNode == null)
                return false;

            _relNode = aNode;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public bool EnsureRoot(string rootName)
        {
// ReSharper disable once PossiblyMistakenUseOfParamsMethod
            XmlNode aRoot = GetOrAddNode(rootName);
            if (null == aRoot)
                return false;

            SetRoot(aRoot);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public Boolean SetRelativeRoot(String xpath)
        {
            if (xpath == null)
                return false;

            XmlNode aNode = SearchNode(_relNode, xpath);
            if (aNode == null)
                return false;

            _relNode = aNode;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean SetRoot(XmlNode node)
        {
            if (null == node)
                return false;

            _relNode = node;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlNode GetRoot()
        {
            return _relNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlNode GetRootNode()
        {
            XmlNode aRoot = null;
            if (_relNode == _doc)
                aRoot = _relNode.FirstChild;

            while(null != aRoot && !(aRoot is XmlElement)) {
                aRoot = aRoot.NextSibling;
            }

            return aRoot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRooted()
        {
            return (null != _relNode && _relNode != _doc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnsetRoot()
        {
            _relNode = _doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetNodeAttrValue(XmlNode node, string attributeName)
        {
            if (null == node)
                return null;

            if (Validator.IsNullString(attributeName))
            {
                return null;
            }

            XmlNode aAttribute = node.Attributes[attributeName];
            if (null == aAttribute)
                return null;

            return aAttribute.Value;
        }

        /// <summary>
        /// Sets attribute for given XmlNode
        /// </summary>
        /// <param name="node">XmlNode for which the attribute will be set</param>
        /// <param name="attributeName">Name of the attribute to be set</param>
        /// <param name="attributeValue">Value of the attribute to be set</param>
        /// <returns>True if set succesfully, false otherwise</returns>
        public bool SetNodeAttrValue(XmlNode node, string attributeName, string attributeValue)
        {
            if (node == null)
                return false;

            if (Validator.IsNullString(attributeName))
                return false;

            if (Validator.IsNullString(attributeValue))
                return false;

            XmlAttribute attr = _doc.CreateAttribute(attributeName);
            attr.Value = attributeValue;

            node.Attributes.Append(attr);

            return true;
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            Unload();
        }
    }
}
