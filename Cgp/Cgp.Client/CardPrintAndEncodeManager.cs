using System;
using System.Collections.Generic;
using System.Text;
using Contal.Cgp.Server.Beans;
using System.Xml;

using Contal.IwQuick;
using Contal.IwQuick.Sys;
using System.Drawing;

namespace Contal.Cgp.Client
{
    public class CardPrintAndEncodeManager
    {
        private Dictionary<Guid, Person> _persons = new Dictionary<Guid, Person>();
        public Dictionary<Guid, Person> Persons
        {
            get { return _persons; }
            set { _persons = value; }
        }

        private Dictionary<string, XmlDocument> _loadedPreviewXMLTemplateDocuments = new Dictionary<string, XmlDocument>();
        private Dictionary<string, XmlDocument> _loadedProduceXMLTemplateDocuments = new Dictionary<string, XmlDocument>();
        private Dictionary<Guid, CardTemplate> _loadedCardTemplates = new Dictionary<Guid, CardTemplate>();

        /// <summary>
        /// Fills the template with data
        /// </summary>
        /// <param name="emptyTemplate"></param>
        /// <param name="card"></param>
        /// <param name="explicitAction">Ignores action defined in CardPrintEncode object</param>
        /// <returns></returns>
        public CardTemplate CreateFilledTemplate(CardTemplate emptyTemplate, CardPrintEncode card, CardAction? explicitAction)
        {
            return CreateFilledTemplate(emptyTemplate, card, null, explicitAction);
        }

        /// <summary>
        /// Fills the template with data
        /// </summary>
        /// <param name="emptyTemplate"></param>
        /// <param name="card"></param>
        /// <param name="relatedCard"></param>
        /// <param name="explicitAction">Ignores action defined in CardPrintEncode object</param>
        /// <returns></returns>
        public CardTemplate CreateFilledTemplate(CardTemplate emptyTemplate, CardPrintEncode card, Card relatedCard, CardAction? explicitAction)
        {
            //card must be specified, otherwise, no data will be filled
            if (card == null || card.Card == null)
                return emptyTemplate;

            Person person = card.Person;
            if (person == null && relatedCard != null)
                person = GetPersonFromCard(relatedCard);

            CardAction? action = null;
            if (explicitAction != null)
                action = explicitAction.Value;
            else
                action = GetCardProduceAction(card);

            if (action == null)
                return emptyTemplate;

            //create XML document (print/produce or encode) for specific template
            XmlDocument xmlDoc = GetXMLDocumentForTemplate(emptyTemplate.Name, emptyTemplate.TemplateData, action.Value);

            switch (action)
            {
                case CardAction.PreviewOnly:
                case CardAction.PrintOnly:
                    RemoveEncodingElements(xmlDoc);
                    if (!FillLayoutData(xmlDoc, card.Card, person))
                        return emptyTemplate;
                    break;
                case CardAction.EncodeOnly:
                    CheckEncodingForCardSystem(card.Card.CardSystem);
                    if (relatedCard != null)
                        CheckEncodingForCardSystem(relatedCard.CardSystem);
                    RemoveLayoutElements(xmlDoc);
                    if (!FillEncodingData(xmlDoc, card.Card.Number, card.Card.CardSystem))
                        return emptyTemplate;
                    if (relatedCard != null && !FillEncodingData(xmlDoc, relatedCard.Number, relatedCard.CardSystem))
                        return emptyTemplate;
                    break;
                case CardAction.PrintAndEncode:
                    CheckEncodingForCardSystem(card.Card.CardSystem);
                    if (relatedCard != null)
                        CheckEncodingForCardSystem(relatedCard.CardSystem);
                    if (!FillLayoutData(xmlDoc, card.Card, person))
                        return emptyTemplate;
                    if (!FillEncodingData(xmlDoc, card.Card.Number, card.Card.CardSystem))
                        return emptyTemplate;
                    if (relatedCard != null && !FillEncodingData(xmlDoc, relatedCard.Number, relatedCard.CardSystem))
                        return emptyTemplate;
                    break;
                default:
                    break;
            }

            return new CardTemplate()
            {
                Name = emptyTemplate.Name,
                TemplateData = Encoding.UTF8.GetBytes(xmlDoc.OuterXml)
            };
        }

        /// <summary>
        /// Get CardAction of specific CardPrintEncode object
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public CardAction? GetCardProduceAction(CardPrintEncode card)
        {
            if (card == null || card.Card == null)
                return null;

            if (card.Print)
            {
                if (card.Encode)
                    return CardAction.PrintAndEncode;
                else
                    return CardAction.PrintOnly;
            }
            else if (card.Encode)
                return CardAction.EncodeOnly;

            return null;
        }

        /// <summary>
        /// Returns non filled card template
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public CardTemplate GetTemplate(Guid templateId)
        {
            if (templateId == null || templateId.Equals(Guid.Empty))
                return null;

            if (_loadedCardTemplates.ContainsKey(templateId))
                return _loadedCardTemplates[templateId];

            CardTemplate fullTemplate = CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectById(templateId);
            if (fullTemplate == null)
                return null;

            if (!_loadedCardTemplates.ContainsKey(templateId))
                _loadedCardTemplates.Add(templateId, fullTemplate);

            return fullTemplate;
        }

        private void RemoveEncodingElements(XmlDocument xmlDoc)
        {
            MifareSectorManager.RemoveEncodingElements(xmlDoc);
            MagneticManager.RemoveEncodingElements(xmlDoc);
        }

        private void CheckEncodingForCardSystem(CardSystem cardSystem)
        {
            if (cardSystem == null)
                throw new NotSupportedException("The specific card dows not have a card system assigned, therefore can not be encoded");

            if (cardSystem.CardSubType != (byte)CardSubType.MifareStandardSectorReadin
                && cardSystem.CardType != (byte)CardType.Magnetic)
                throw new NotSupportedException("Card system " + cardSystem.ToString() + " does not have an encoding support");
        }

        private bool FillLayoutData(XmlDocument cardXML, Card card, Person person)
        {
            XmlNodeList nodes = cardXML.GetElementsByTagName("text");
            if (nodes == null || nodes.Count == 0)
                return false;

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];
                XmlAttribute textAttribute = node.Attributes["text"];
                if (textAttribute == null)
                    continue;

                FillNode(ref node, textAttribute.Value, card, person);
            }

            XmlNodeList photoNodes = cardXML.GetElementsByTagName("photo");
            if (photoNodes != null && photoNodes.Count > 0)
            {
                XmlNode photoNode = photoNodes[0];
                FillPhotoNode(ref photoNode, person);
            }

            return true;
        }

        private bool FillEncodingData(XmlDocument xmlDoc, string cardNumber, CardSystem cardSystem)
        {
            if (xmlDoc == null || cardNumber == null || cardSystem == null)
                return false;

            if (cardSystem.CardType == (byte)CardType.Magnetic)
            {
                return MagneticManager.Singleton.FillEncodingData(xmlDoc, cardNumber, cardSystem);
            }
            else if (cardSystem.CardType == (byte)CardType.Mifare
                && cardSystem.CardSubType == (byte)CardSubType.MifareStandardSectorReadin)
            {
                return MifareSectorManager.Singleton.FillEncodingData(xmlDoc, cardNumber, cardSystem);
            }

            return false;
        }

        public static string GetHexaDecryptedStringFromKey(byte[] keyData)
        {
            if (keyData == null || keyData.Length == 0)
                return string.Empty;

            byte[] decryptedKeyData = null;

            IwQuick.Crypto.XTEA cryptoXtea = new Contal.IwQuick.Crypto.XTEA();
            cryptoXtea.XTeaInit();
            byte[] sendToDecript = new byte[keyData.Length];
            Array.Copy(keyData, sendToDecript, keyData.Length);
            decryptedKeyData = cryptoXtea.XTeaFrameDec(sendToDecript);

            if (decryptedKeyData == null)
                return string.Empty;

            return BitConverter.ToString(decryptedKeyData).Replace("-", "");
        }

        public static int GetLastUsedFieldIndexFromXML(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return 0;

            XmlNodeList fieldNodes = xmlDoc.GetElementsByTagName("text");
            if (fieldNodes == null || fieldNodes.Count == 0)
                return 0;

            return fieldNodes.Count;
        }

        private void RemoveLayoutElements(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return;

            try
            {
                xmlDoc.GetElementsByTagName("applications")[0].RemoveChild(xmlDoc.GetElementsByTagName("front")[0]);
            }
            catch { }

            try
            {
                xmlDoc.GetElementsByTagName("applications")[0].RemoveChild(xmlDoc.GetElementsByTagName("back")[0]);
            }
            catch { }
        }

        public static string EncodeString(string inputString, EncodingType encoding)
        {
            if (inputString == null)
                return null;

            StringBuilder hexaString = new StringBuilder();
            try
            {
                int index = 0;
                foreach (char letter in inputString)
                {
                    switch (encoding)
                    {
                        case EncodingType.Bcd:
                            if (index % 2 == 0)
                            {
                                if (index + 2 <= inputString.Length)
                                    hexaString.Append(inputString.Substring(index, 2));
                                else
                                    hexaString.Append(inputString.Substring(index, 1));
                            }
                            break;
                        case EncodingType.Ascii:
                            hexaString.Append(((int)(letter)).ToString(StringConstants.HEX2_FORMAT));
                            break;
                        case EncodingType.Hex8:
                        case EncodingType.Hex16:
                        case EncodingType.Hex32:
                        case EncodingType.Hex64:
                            hexaString.Append(int.Parse(letter.ToString()).ToString(StringConstants.HEX2_FORMAT));
                            break;
                        default:
                            break;
                    }
                    index++;
                }

                return hexaString.ToString();
            }
            catch { }

            return null;
        }

        private void FillPhotoNode(ref XmlNode photoNode, Person person)
        {
            if (photoNode == null)
                return;

            if (person == null)
            {
                photoNode.InnerText = string.Empty;
                return;
            }

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;


            BinaryPhoto photo = CgpClient.Singleton.MainServerProvider.Persons.GetPhoto(person.IdPerson);
            if (photo == null)
            {
                photoNode.InnerText = string.Empty;
                return;
            }

            photoNode.InnerText = Convert.ToBase64String(photo.BinaryData);

        }

        private void FillNode(ref XmlNode node, string textAttributeValue, Card card, Person person)
        {
            if (node == null || textAttributeValue == null || textAttributeValue == string.Empty)
                return;

            if (textAttributeValue == "FirstName")
                node.InnerText = person == null ? string.Empty : person.FirstName;
            else if (textAttributeValue == "MiddleName")
                node.InnerText = person == null ? string.Empty : person.MiddleName;
            else if (textAttributeValue == "Surname")
                node.InnerText = person == null ? string.Empty : person.Surname;
            else if (textAttributeValue == "Title")
                node.InnerText = person == null ? string.Empty : person.Tiltle;
            else if (textAttributeValue == "Birthday")
                node.InnerText = person == null ? string.Empty : person.Birthday.ToString();
            else if (textAttributeValue == "Company")
                node.InnerText = person == null ? string.Empty : person.Company;
            else if (textAttributeValue == "Email")
                node.InnerText = person == null ? string.Empty : person.Email;
            else if (textAttributeValue == "PhoneNumber")
                node.InnerText = person == null ? string.Empty : person.PhoneNumber;
            else if (textAttributeValue == "Identification")
                node.InnerText = person == null ? string.Empty : person.Identification;
            else if (textAttributeValue == "PersonDescription")
                node.InnerText = person == null ? string.Empty : person.Description;
            else if (textAttributeValue == "EmployeeNumber")
                node.InnerText = person == null ? string.Empty : person.EmployeeNumber;
            else if (textAttributeValue == "Department")
            {
                string department = string.Empty;
                if (person != null && person.Department != null)
                    department = person.Department.ToString();
                node.InnerText = department;
            }
            else if (textAttributeValue == "CostCenter")
                node.InnerText = person == null ? string.Empty : person.CostCenter;
            else if (textAttributeValue == "EmploymentBeginningDate")
                node.InnerText = person == null ? string.Empty : person.EmploymentBeginningDate.ToString();
            else if (textAttributeValue == "EmploymentEndDate")
                node.InnerText = person == null ? string.Empty : person.EmploymentEndDate.ToString();
            else if (textAttributeValue == "Role")
                node.InnerText = person == null ? string.Empty : person.Role;
            else if (textAttributeValue == "Number")
                node.InnerText = card == null ? string.Empty : card.Number;
            else if (textAttributeValue == "FullCardNumber")
                node.InnerText = card == null ? string.Empty : card.FullCardNumber;
            else if (textAttributeValue == "CardDescription")
                node.InnerText = card == null ? string.Empty : card.Description;
        }

        /// <summary>
        /// Gets XML document in form VPS can work with
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateData"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public XmlDocument GetXMLDocumentForTemplate(string templateName, byte[] templateData, CardAction action)
        {
            if (action != CardAction.PreviewOnly)
            {
                if (!_loadedProduceXMLTemplateDocuments.ContainsKey(templateName))
                {
                    string templateDataAsString = Encoding.UTF8.GetString(templateData);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(templateDataAsString);
                    //this will change template xml to xml for card producing (printing/encoding)
                    xmlDoc = CreateCardProduceXML(xmlDoc);
                    _loadedProduceXMLTemplateDocuments.Add(templateName, xmlDoc);
                    return (XmlDocument)xmlDoc.Clone();
                }
                else
                    return (XmlDocument)_loadedProduceXMLTemplateDocuments[templateName].Clone();
            }
            else
            {
                if (!_loadedPreviewXMLTemplateDocuments.ContainsKey(templateName))
                {
                    string templateDataAsString = Encoding.UTF8.GetString(templateData);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(templateDataAsString);
                    _loadedPreviewXMLTemplateDocuments.Add(templateName, xmlDoc);
                    return (XmlDocument)xmlDoc.Clone();
                }
                else
                    return (XmlDocument)_loadedPreviewXMLTemplateDocuments[templateName].Clone();
            }
        }

        private XmlDocument CreateCardProduceXML(XmlDocument xmlTemplateDoc)
        {
            if (xmlTemplateDoc == null)
                return null;

            try
            {
                XmlNode cardNode = xmlTemplateDoc.GetElementsByTagName("card")[0];

                XmlNode appNode = xmlTemplateDoc.GetElementsByTagName("applications")[0];

                appNode.Attributes.Append(xmlTemplateDoc.CreateAttribute("produce"));
                appNode.Attributes["produce"].Value = "true";

                XmlElement jobElement = xmlTemplateDoc.CreateElement("card_job");
                jobElement.Attributes.Append(xmlTemplateDoc.CreateAttribute("language"));
                jobElement.Attributes["language"].Value = "EN";

                jobElement.AppendChild(cardNode);

                xmlTemplateDoc.LoadXml(jobElement.OuterXml);

                return xmlTemplateDoc;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return null;
        }

        public Person GetPersonFromCard(Card card)
        {
            if (card == null || card.Person == null || card.Person.IdPerson.Equals(Guid.Empty))
                return null;

            if (_persons != null && _persons.ContainsKey(card.Person.IdPerson))
                return _persons[card.Person.IdPerson];

            Person person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(card.Person.IdPerson);
            if (person == null)
                return null;

            _persons.Add(person.IdPerson, person);
            return person;
        }

        public static string AddCompanyCodeToCardNumber(string cardNumber, CardSystem cardSystem)
        {
            if (cardSystem == null)
                return cardNumber;

            StringBuilder result = new StringBuilder();
            result.Append(cardNumber);
            if (cardSystem.CompanyCode != null)
                result.Insert(0, cardSystem.CompanyCode);

            return result.ToString();
        }
    }

    public enum CardAction : byte
    {
        PreviewOnly = 0,
        PrintOnly = 1,
        EncodeOnly = 2,
        PrintAndEncode = 3
    }

    public class CardPrintEncode
    {
        public const string COLUMNCARD = "Card";
        public const string COLUMNPERSON = "Person";
        public const string COLUMNTEMPLATE = "Template";
        public const string COLUMNPRINT = "Print";
        public const string COLUMNENCODE = "Encode";
        public const string COLUMNSYMBOL = "Symbol";

        private Card _card = null;
        private Person _person = null;
        private CardTemplateShort _template = null;
        private bool _print = false;
        private bool _encode = false;
        private Image _symbol = null;

        public Image Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public Card Card
        {
            get { return _card; }
            set
            {
                _card = value;
                _symbol = ObjectImageList.Singleton.ClientObjectImages.Images[value.GetSubTypeImageString("State")];
            }
        }

        public Person Person
        {
            get { return _person; }
            set { _person = value; }
        }

        public CardTemplateShort Template
        {
            get { return _template; }
            set { _template = value; }
        }

        public bool Print
        {
            get { return _print; }
            set { _print = value; }
        }

        public bool Encode
        {
            get { return _encode; }
            set { _encode = value; }
        }

        public CardPrintEncode()
        {

        }

        public CardPrintEncode(Card card, Person person, CardTemplateShort template)
        {
            _card = card;
            _person = person;
            _template = template;
            _symbol = ObjectImageList.Singleton.ClientObjectImages.Images[card.GetSubTypeImageString("State")];
        }
    }
}
