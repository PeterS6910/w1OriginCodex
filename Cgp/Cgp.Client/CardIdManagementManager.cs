using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Contal.Cgp.Server.Beans;
using System.IO;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public class IdManagementWindowOpenedException : Exception
    {
        public static string GetDefaultLocalisedExceptionMessage()
        {
            return CgpClient.Singleton.LocalizationHelper.GetString("ErrorIdManagementWindowOpened");
        }

        public IdManagementWindowOpenedException()
        { }
        public IdManagementWindowOpenedException(string message)
            : base(message)
        { }
    }

    public class CardIdManagementManager
    {
        private static volatile CardIdManagementManager _singleton = null;
        private static object _syncRoot = new object();
        public static CardIdManagementManager Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CardIdManagementManager();
                    }

                return _singleton;
            }
        }

        public volatile bool CardPrintNovaDialogOpened = false;

        private volatile bool _anyProductionWindowOpened = false;

        public bool AnyProductionWindowOpened
        {
            get { return _anyProductionWindowOpened; }
            set { _anyProductionWindowOpened = value; }
        }

        private CardIdManagement.IDProductionService _cardProductionService = new Contal.Cgp.Client.CardIdManagement.IDProductionService();

        private List<string> _databaseFields = null;

        public delegate void ConfigurationFinishedHandler(string result);
        public event ConfigurationFinishedHandler ConfigurationFinished = null;

        public delegate void ImageCaptureFinishedHandler(string result, string resultXML);
        public event ImageCaptureFinishedHandler ImageCaptureFinished = null;

        public delegate void CardComposerFinishedHandler(string result, CardTemplate template);
        public event CardComposerFinishedHandler CardComposerFinished = null;

        public delegate void CardPreviewFinishedHandler(string result, string templateData);
        public event CardPreviewFinishedHandler CardPreviewFinished = null;

        public delegate void CardJobFinishedHandler(string result, CardTemplate template);
        public event CardJobFinishedHandler CardJobFinished = null;

        public CardIdManagementManager()
        {
            FillDatabaseFields();

            _cardProductionService.ConfigurationManagerCompleted += new Contal.Cgp.Client.CardIdManagement.ConfigurationManagerCompletedEventHandler(ConfigurationManagerCompleted);
            _cardProductionService.ImageCaptureCompleted += new Contal.Cgp.Client.CardIdManagement.ImageCaptureCompletedEventHandler(ImageCaptureCompleted);
            _cardProductionService.CardComposerCompleted += new Contal.Cgp.Client.CardIdManagement.CardComposerCompletedEventHandler(CardComposerCompleted);
            _cardProductionService.CardPreviewCompleted += new Contal.Cgp.Client.CardIdManagement.CardPreviewCompletedEventHandler(CardPreviewCompleted);
            _cardProductionService.CardJobCompleted += new Contal.Cgp.Client.CardIdManagement.CardJobCompletedEventHandler(CardJobCompleted);
        }

        private void FillDatabaseFields()
        {
            if (_databaseFields != null)
                return;

            _databaseFields = new List<string>();

            _databaseFields.AddRange(new string[]{"FirstName", "MiddleName", "Surname", "Title", "Birthday", "Company", "Email", "PhoneNumber", "Identification", 
                "PersonDescription", "Department", "EmployeeNumber", "CostCenter", "EmploymentBeginningDate", "EmploymentEndDate", "Role", "Number", "FullCardNumber", 
                "CardDescription"});
        }

        public bool RunConfigurationDialog(out Exception exception)
        {
            exception = null;
            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode confNode = xmlDoc.CreateElement("configuration");
                confNode.Attributes.Append(xmlDoc.CreateAttribute("language"));
                confNode.Attributes["language"].Value = "EN";
                xmlDoc.AppendChild(confNode);

                _anyProductionWindowOpened = true;
                _cardProductionService.ConfigurationManagerAsync(xmlDoc.OuterXml);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;

                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        public bool RunCaptureDialog(out Exception exception)
        {
            exception = null;
            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlNode imageNode = xmlDoc.CreateElement("image");
                imageNode.Attributes.Append(xmlDoc.CreateAttribute("language"));
                imageNode.Attributes["language"].Value = "EN";
                imageNode.Attributes.Append(xmlDoc.CreateAttribute("type"));
                imageNode.Attributes["type"].Value = "photo";
                imageNode.Attributes.Append(xmlDoc.CreateAttribute("format"));
                imageNode.Attributes["format"].Value = "bmp";

                XmlNode resultNode = xmlDoc.CreateElement("result");
                resultNode.Attributes.Append(xmlDoc.CreateAttribute("data"));

                XmlNode previewNode = xmlDoc.CreateElement("preview");
                previewNode.Attributes.Append(xmlDoc.CreateAttribute("data"));
                previewNode.Attributes.Append(xmlDoc.CreateAttribute("max_width"));
                previewNode.Attributes["max_width"].Value = "200";
                previewNode.Attributes.Append(xmlDoc.CreateAttribute("max_height"));
                previewNode.Attributes["max_height"].Value = "200";

                xmlDoc.AppendChild(imageNode);
                imageNode.AppendChild(resultNode);
                imageNode.AppendChild(previewNode);

                _anyProductionWindowOpened = true;
                _cardProductionService.ImageCaptureAsync(xmlDoc.OuterXml);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;

                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        public bool RunTemplateInsert(out Exception exception)
        {
            exception = null;
            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement cardElement = xmlDoc.CreateElement("card");
                cardElement.Attributes.Append(xmlDoc.CreateAttribute("language"));
                cardElement.Attributes["language"].Value = "EN";

                XmlElement aplicationsElement = xmlDoc.CreateElement("applications");

                XmlElement fieldsElement = xmlDoc.CreateElement("fields");
                int index = 0;
                foreach (string item in _databaseFields)
                {
                    index++;
                    XmlElement fieldElement = xmlDoc.CreateElement("text");
                    fieldElement.Attributes.Append(xmlDoc.CreateAttribute("name"));
                    fieldElement.Attributes["name"].Value = "Field" + index;
                    fieldElement.Attributes.Append(xmlDoc.CreateAttribute("text"));
                    fieldElement.Attributes["text"].Value = item;

                    fieldsElement.AppendChild(fieldElement);
                }

                XmlElement photoFieldElement = xmlDoc.CreateElement("photo");
                photoFieldElement.Attributes.Append(xmlDoc.CreateAttribute("name"));
                photoFieldElement.Attributes["name"].Value = "Foto";

                fieldsElement.AppendChild(photoFieldElement);

                XmlElement previewElement = xmlDoc.CreateElement("preview");
                previewElement.Attributes.Append(xmlDoc.CreateAttribute("front_data"));
                previewElement.Attributes.Append(xmlDoc.CreateAttribute("back_data"));
                previewElement.Attributes.Append(xmlDoc.CreateAttribute("max_width"));
                previewElement.Attributes["max_width"].Value = "800";
                previewElement.Attributes.Append(xmlDoc.CreateAttribute("max_height"));
                previewElement.Attributes["max_height"].Value = "800";
                previewElement.Attributes.Append(xmlDoc.CreateAttribute("format"));
                previewElement.Attributes["format"].Value = "bmp";

                cardElement.AppendChild(aplicationsElement);
                cardElement.AppendChild(fieldsElement);
                cardElement.AppendChild(previewElement);
                xmlDoc.AppendChild(cardElement);

                _anyProductionWindowOpened = true;
                _cardProductionService.CardComposerAsync(xmlDoc.OuterXml);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;

                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        public bool RunTemplateEdit(CardTemplate cardTemplate, out Exception exception)
        {
            exception = null;

            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            if (cardTemplate == null)
            {
                exception = new NullReferenceException("Card template parameter is null");
                return false;
            }

            byte[] templateData = cardTemplate.TemplateData;
            if (templateData == null || templateData.Length == 0)
                return false;

            try
            {
                string dataAsString = Encoding.UTF8.GetString(templateData);
                _anyProductionWindowOpened = true;
                _cardProductionService.CardComposerAsync(dataAsString);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;

                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        public int DoCardPreview(CardTemplate cardTemplate, out string resultText, out string resultData)
        {
            resultText = string.Empty;
            resultData = string.Empty;

            if (cardTemplate == null)
            {
                resultText = "Card template parameter is null";
                return 0;
            }

            byte[] templateData = cardTemplate.TemplateData;
            if (templateData == null || templateData.Length == 0)
            {
                resultText = "Empty template data";
                return 0;
            }

            string dataAsString = string.Empty;
            try
            {
                dataAsString = Encoding.UTF8.GetString(templateData);
            }
            catch (Exception ex)
            {
                resultText = ex.Message;
                HandledExceptionAdapter.Examine(ex);
            }

            return _cardProductionService.CardPreview(dataAsString, out resultText, out resultData);
        }

        public bool RunCardPreview(CardTemplate cardTemplate, out Exception exception)
        {
            exception = null;

            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            if (cardTemplate == null)
            {
                exception = new NullReferenceException("Card template parameter is null");
                return false;
            }

            byte[] templateData = cardTemplate.TemplateData;
            if (templateData == null || templateData.Length == 0)
                return false;

            try
            {
                string dataAsString = Encoding.UTF8.GetString(templateData);
                _anyProductionWindowOpened = true;
                _cardProductionService.CardPreviewAsync(dataAsString);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;
                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        public bool DoCreateCardWithoutWindowCheck(string cardData, out string resultText, out string resultData)
        {
            resultText = string.Empty;
            resultData = string.Empty;

            _cardProductionService.CardJob(cardData, out resultText, out resultData);

            return resultText == "Success";
        }

        public bool RunCreateCard(string cardData, out Exception exception)
        {
            exception = null;

            if (_anyProductionWindowOpened)
            {
                exception = new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return false;
            }

            if (cardData == null || cardData.Length == 0)
            {
                exception = new NullReferenceException("Card data parameter is null or empty");
                return false;
            }

            try
            {
                _anyProductionWindowOpened = true;
                _cardProductionService.CardJobAsync(cardData);
            }
            catch (Exception ex)
            {
                _anyProductionWindowOpened = false;
                exception = ex;

                HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return true;
        }

        void CardJobCompleted(object sender, Contal.Cgp.Client.CardIdManagement.CardJobCompletedEventArgs e)
        {
            if (CardJobFinished != null)
                try
                {
                    CardJobFinished(e.ResultText, CreateCardTemplateFromData(e.ResultData));
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            _anyProductionWindowOpened = false;
        }

        void CardPreviewCompleted(object sender, Contal.Cgp.Client.CardIdManagement.CardPreviewCompletedEventArgs e)
        {
            if (CardPreviewFinished != null)
                try
                {
                    CardPreviewFinished(e.ResultText, e.ResultData);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            _anyProductionWindowOpened = false;
        }

        void CardComposerCompleted(object sender, Contal.Cgp.Client.CardIdManagement.CardComposerCompletedEventArgs e)
        {
            if (CardComposerFinished != null)
                try
                {
                    CardComposerFinished(e.ResultText, CreateCardTemplateFromData(e.ResultData));
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            _anyProductionWindowOpened = false;
        }

        void ImageCaptureCompleted(object sender, Contal.Cgp.Client.CardIdManagement.ImageCaptureCompletedEventArgs e)
        {
            if (ImageCaptureFinished != null)
                try
                {
                    ImageCaptureFinished(e.ResultText, e.ResultData);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            _anyProductionWindowOpened = false;
        }

        void ConfigurationManagerCompleted(object sender, Contal.Cgp.Client.CardIdManagement.ConfigurationManagerCompletedEventArgs e)
        {
            if (ConfigurationFinished != null)
                try
                {
                    ConfigurationFinished(e.ResultText);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            _anyProductionWindowOpened = false;
        }

        private CardTemplate CreateCardTemplateFromData(string templateText)
        {
            if (templateText == null || templateText.Length == 0)
                return null;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(templateText);

                XmlNode cardNode = xmlDoc.GetElementsByTagName("card")[0];
                string templateName = cardNode.Attributes["name"].Value;
                byte[] templateData = Encoding.UTF8.GetBytes(templateText);
                return new CardTemplate()
                {
                    Name = templateName,
                    TemplateData = templateData
                };
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }

        }
    }
}
