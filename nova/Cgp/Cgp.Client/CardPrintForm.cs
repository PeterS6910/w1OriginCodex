using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using System.Xml;
using System.IO;
using Contal.IwQuick.UI;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Crypto;
using System.Diagnostics;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;
using System.Text.RegularExpressions;

namespace Contal.Cgp.Client
{


    public partial class CardPrintForm : ACgpEditForm<Card>
    {
        private class PasswordString
        {
            public const char PASSWORD_CHAR = '●';

            private string _value = null;
            public string Value
            {
                get
                {
                    if (_usePasswordChar)
                        return new string(_value.ToCharArray().Select(ch =>
                        {
                            return PASSWORD_CHAR;
                        }).ToArray());
                    else
                        return _value;

                }
                set
                {
                    _value = Regex.Match(value, "[a-fA-f0-9]{1,12}").Value;
                }
            }

            private bool _usePasswordChar = true;
            public bool UsePasswordChar
            {
                get { return _usePasswordChar; }
                set { _usePasswordChar = value; }
            }

            public PasswordString(string input)
                : this(input, true)
            {
            }

            public PasswordString(string input, bool usePasswordCharacter)
            {
                _value = input;
                _usePasswordChar = usePasswordCharacter;
            }

            public string GetNonPasswordValue()
            {
                return _value;
            }
        }

        private enum CardsSucceededDGVColumn
        {
            ColumnImage2 = 0,
            ColumnCard2 = 1,
            ColumnPerson2 = 2,
            ColumnSerialNumber2 = 3,
            ColumnDateTime2 = 4
        }

        private enum CardsFailedDGVColumn
        {
            ColumnImage3 = 0,
            ColumnCard3 = 1,
            ColumnPerson3 = 2
        }

        /// <summary>
        /// Be aware of: will throw exception if this kind of form is already opened
        /// </summary>
        public CardPrintForm()
            : this(null, null)
        {
        }

        private CardPrintAndEncodeManager _cardPrintAndEncodeManager = new CardPrintAndEncodeManager();
        private HashSet<Guid> _cards = new HashSet<Guid>();
        private Dictionary<Guid, Guid> _pickedRelatedCards = new Dictionary<Guid, Guid>();
        private Dictionary<Guid, CardTemplateShort> _cardTemplates = null;
        private static object _syncRoot = new object();
        private BindingList<CardPrintEncode> _bindingCards = null;
        ProcessingQueue<CardPrintEncode> _previewQueue = null;
        private List<PasswordString> _aKeys = null;
        private List<PasswordString> _bKeys = null;

        /// <summary>
        /// Be aware of: will throw exception if this kind of form is already opened
        /// </summary>
        /// <param name="cardsForPrint"></param>
        /// <param name="person"></param>
        public CardPrintForm(Card[] cardsForPrint, Person person)
            : base(cardsForPrint[0], ShowOptionsEditForm.Edit)
        {
            //this will prevent from opening more card print forms than one
            if (CardIdManagementManager.Singleton.CardPrintNovaDialogOpened)
                throw new IdManagementWindowOpenedException(IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());

            MdiParent = CgpClientMainForm.Singleton;

            InitializeComponent();
            InitializePreviewQueue();
            //CreateMenuPanelToForm();
            //AddMenuPanelToBackPanel();


            CardIdManagementManager.Singleton.CardPreviewFinished += new CardIdManagementManager.CardPreviewFinishedHandler(CardPreviewFinished);

            FillCardTemplates();
            SetDataGridViewSource();

            //adding cards into loaded cards
            AddCardsToLoadedCards(cardsForPrint, person);

            ApplyImplicitSort();

            //adding person into loaded persons
            if (person != null && !_cardPrintAndEncodeManager.Persons.ContainsKey(person.IdPerson))
                _cardPrintAndEncodeManager.Persons.Add(person.IdPerson, person);

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            SafeThread.StartThread(delegate { SetCardTemplatesToComboBox(); });
        }

        private void ApplyImplicitSort()
        {
            if (_dgvCards.Columns.Contains(CardPrintEncode.COLUMNCARD))
                _dgvCards.Sort(_dgvCards.Columns[CardPrintEncode.COLUMNCARD], ListSortDirection.Ascending);
        }

        private void InitializePreviewQueue()
        {
            _previewQueue = new ProcessingQueue<CardPrintEncode>();
            _previewQueue.LimitQueueCount = 1;
            _previewQueue.ShrinkOnLimitExceeded = true;
            _previewQueue.ItemProcessing += _previewQueue_ItemProcessing;
        }

        private void AddToPreview(CardPrintEncode card)
        {
            if (_previewQueue == null || card == null)
                return;

            _previewQueue.Enqueue(card);
        }

        void _previewQueue_ItemProcessing(CardPrintEncode card)
        {
            if (card == null)
                return;

            if (card.Template == null)
                return;

            CardTemplate template = _cardPrintAndEncodeManager.GetTemplate(card.Template.Id);
            if (template == null)
                return;

            CardTemplate filledTemplate = _cardPrintAndEncodeManager.CreateFilledTemplate(template, card, CardAction.PreviewOnly);

            string resultText = null;
            string resultData = null;

            CardIdManagementManager.Singleton.DoCardPreview(filledTemplate ?? template, out resultText, out resultData);
            if (resultText.ToLower() != "success")
                Dialog.Error(GetString("ExceptionOccured") + ": " + resultText);
            else
                PreviewData(resultData);
        }

        private void AddCardsToLoadedCards(Card[] cards, Person person)
        {
            if (cards != null)
            {
                foreach (Card cardItem in cards)
                {
                    if (person != null)
                        cardItem.Person = person;
                    AddCardAndItsRelatedCard(cardItem);
                }
            }
        }

        private void AddMenuPanelToBackPanel()
        {
            _panelBack.MouseMove += new MouseEventHandler(ShowRemovePanel);
            CreateHighlightPanelForDockMaximize(_panelBack);
        }

        private System.Windows.Forms.Panel _pHighLightPanel;
        private void CreateHighlightPanelForDockMaximize(Control control)
        {
            _pHighLightPanel = new System.Windows.Forms.Panel();
            _pHighLightPanel.Location = new System.Drawing.Point(this.Width - 67, 2);
            _pHighLightPanel.Size = new System.Drawing.Size(59, 18);
            _pHighLightPanel.BackColor = Color.Aquamarine;
            _pHighLightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _pHighLightPanel.MouseMove += new MouseEventHandler(ShowRemovePanel);
            _pHighLightPanel.BorderStyle = BorderStyle.None;
            control.Controls.Add(_pHighLightPanel);
            _pHighLightPanel.BringToFront();
        }

        private void ShowRemovePanel(object sender, MouseEventArgs e)
        {
            try
            {
                Panel panel = sender as Panel;
                if ((e.Y < 20 && e.Y > 1) &&
                    (e.X > panel.Width - 60))
                {
                    ShowMenuPanel();
                }
                else
                {
                    RemoveBackPanel();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            base.OnMouseMove(e);
        }

        private void ShowMenuPanel()
        {
            if (_panelBack != null)
            {
                int positionX = _panelBack.Width - _pMenuPanel.Width;
                _pMenuPanel.Location = new System.Drawing.Point(positionX, 0);
                if (!_panelBack.Controls.Contains(_pMenuPanel))
                {
                    _panelBack.Controls.Add(_pMenuPanel);
                    _pHighLightPanel.SendToBack();
                    _pMenuPanel.BringToFront();
                    _panelBack.Controls.SetChildIndex(_pMenuPanel, 0);
                }
            }
        }

        private void RemoveBackPanel()
        {
            if (_panelBack != null)
            {
                if (_panelBack.Controls.Contains(_pMenuPanel))
                {
                    _panelBack.Controls.Remove(_pMenuPanel);
                    if (_pHighLightPanel != null)
                    {
                        _pHighLightPanel.BringToFront();
                    }
                }
            }
        }


        private System.Windows.Forms.Panel _pMenuPanel;
        private System.Windows.Forms.Button _bMenuDock;
        private System.Windows.Forms.Button _bMenuMaximize;
        private System.Windows.Forms.Button _bMenuClose;

        private void CreateMenuPanelToForm()
        {
            //create control for panel menu
            _pMenuPanel = new System.Windows.Forms.Panel();
            _bMenuDock = new System.Windows.Forms.Button();
            _bMenuMaximize = new System.Windows.Forms.Button();
            _bMenuClose = new System.Windows.Forms.Button();

            _pMenuPanel.SuspendLayout();
            // set menu properties
            _pMenuPanel.Controls.Add(_bMenuDock);
            _pMenuPanel.Controls.Add(_bMenuMaximize);
            _pMenuPanel.Controls.Add(_bMenuClose);
            _pMenuPanel.Location = new System.Drawing.Point(0, 0);
            _pMenuPanel.Size = new System.Drawing.Size(106, 80);
            _pMenuPanel.Name = "_pMenuPanel";
            //_pMenuPanel.
            // set dock button properties
            _bMenuDock.Location = new System.Drawing.Point(3, 3);
            _bMenuDock.Name = "_bMenuDock";
            _bMenuDock.Size = new System.Drawing.Size(100, 23);
            _bMenuDock.TabIndex = 0;
            _bMenuDock.Text = LocalizationHelper.GetString("TextInfoUndock");
            _bMenuDock.FlatStyle = FlatStyle.Flat;
            _bMenuDock.UseVisualStyleBackColor = true;
            _bMenuDock.Click += new EventHandler(DockUndock);
            // set maximaze button properties
            _bMenuMaximize.Location = new System.Drawing.Point(3, 28);
            _bMenuMaximize.Name = "_bMenuMaximize";
            _bMenuMaximize.Size = new System.Drawing.Size(100, 23);
            _bMenuMaximize.TabIndex = 1;
            _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
            _bMenuMaximize.FlatStyle = FlatStyle.Flat;
            _bMenuMaximize.UseVisualStyleBackColor = true;
            _bMenuMaximize.Click += new EventHandler(Maximize_Click);
            _pMenuPanel.ResumeLayout(false);
            // set close button properties
            _bMenuClose.Location = new System.Drawing.Point(3, 53);
            _bMenuClose.Name = "_bMenuClose";
            _bMenuClose.Size = new System.Drawing.Size(100, 23);
            _bMenuClose.TabIndex = 2;
            _bMenuClose.Text = LocalizationHelper.GetString("TextInfoClose");
            _bMenuClose.FlatStyle = FlatStyle.Flat;
            _bMenuClose.UseVisualStyleBackColor = true;
            _bMenuClose.Click += new EventHandler(Close_Click);
            _bMenuClose.ResumeLayout(false);
        }

        void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        void Maximize_Click(object sender, EventArgs e)
        {
            if (MdiParent == null)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoNormalSize");
                }
            }
            else
            {
                if (Dock == DockStyle.Fill)
                {
                    this.Dock = DockStyle.None;
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
                }
                else
                {
                    this.Dock = DockStyle.Fill;
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoNormalSize");
                }
            }
        }

        void DockUndock(object sender, EventArgs e)
        {
            if (MdiParent == null)
            {
                DockWindow();
            }
            else
            {
                UndockWindow();
            }

            AfterDockUndock();
        }

        private void AfterDockUndock()
        {

        }

        public void DockWindow()
        {
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            this.MdiParent = CgpClientMainForm.Singleton;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.Dock = DockStyle.None;
            this.ControlBox = false;
            CgpClientMainForm.Singleton.AddToOpenWindows(this);
            CgpClientMainForm.Singleton.SetActOpenWindow(this);
            CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
            _bMenuDock.Text = LocalizationHelper.GetString("TextInfoUndock");
            _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
        }

        public void UndockWindow()
        {
            this.SuspendLayout();
            this.MdiParent = null;
            this.Location = Screen.FromControl(CgpClientMainForm.Singleton).WorkingArea.Location;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.ControlBox = true;
            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            CgpClientMainForm.Singleton.AddToUndockedForms(this);
            _bMenuDock.Text = LocalizationHelper.GetString("TextInfoDock");
            _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
            this.ResumeLayout(true);
        }

        void CardPreviewFinished(string result, string templateData)
        {
            PreviewData(templateData);
        }

        private void CardPrintForm_Load(object sender, EventArgs e)
        {
            CardIdManagementManager.Singleton.CardPrintNovaDialogOpened = true;
            SafeThread.StartThread(SetConfigureButtonAvailability);
            SafeThread.StartThread(SetAlternateAuthKeys);
        }

        private void SetAlternateAuthKeys()
        {
            if (_aKeys == null)
                _aKeys = new List<PasswordString>();

            if (MifareSectorManager.Singleton.AlternativeAuthAKeysHexa != null)
                MifareSectorManager.Singleton.AlternativeAuthAKeysHexa.ForEach(key => _aKeys.Add(new PasswordString(key)));

            Invoke(new MethodInvoker(delegate()
            {
                _dgvAAuthKeys.DataSource = new BindingSource()
                {
                    DataSource = _aKeys
                };
                _dgvAAuthKeys.Columns[_dgvAAuthKeys.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgvAAuthKeys.Columns[_dgvAAuthKeys.Columns.Count - 1].MinimumWidth = 50;
                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvAAuthKeys);
            }));


            if (_bKeys == null)
                _bKeys = new List<PasswordString>();

            if (MifareSectorManager.Singleton.AlternativeAuthBKeysHexa != null)
                MifareSectorManager.Singleton.AlternativeAuthBKeysHexa.ForEach(key => _bKeys.Add(new PasswordString(key)));

            Invoke(new MethodInvoker(delegate()
                {
                    _dgvBAuthKeys.DataSource = new BindingSource()
                    {
                        DataSource = _bKeys
                    };
                    _dgvBAuthKeys.Columns[_dgvAAuthKeys.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    _dgvBAuthKeys.Columns[_dgvAAuthKeys.Columns.Count - 1].MinimumWidth = 50;
                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvBAuthKeys);
                }));
        }

        private void SetConfigureButtonAvailability()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            bool hasAccessCardPrintAdmin =
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.IdManagementCardPrintAdmin));

            Invoke(new MethodInvoker(delegate()
                {
                    _bConfigure.Enabled = hasAccessCardPrintAdmin;
                }));
        }

        private void CardPrintForm_Shown(object sender, EventArgs e)
        {
            //this will prevent calling this event before Form_Load
            _dgvCards.SelectionChanged += new System.EventHandler(_dgvCards_SelectionChanged);
        }

        private void SetCardTemplatesToComboBox()
        {
            if (_cardTemplates == null)
                FillCardTemplates();

            Invoke(new MethodInvoker(delegate
                {
                    _cbCardTemplates.Items.Clear();
                    if (_cardTemplates != null)
                        _cbCardTemplates.DataSource = _cardTemplates.Values.OrderBy(t => t.Name).ToArray();
                }));
        }

        private void FillCardTemplates()
        {
            Exception ex = null;
            ICollection<CardTemplateShort> cardTemplates = CgpClient.Singleton.MainServerProvider.CardTemplates.ShortSelectByCriteria(null, out ex);

            if (cardTemplates == null || cardTemplates.Count == 0)
                return;

            cardTemplates = cardTemplates.OrderBy(t => t.Name).ToArray();

            _cardTemplates = new Dictionary<Guid, CardTemplateShort>();
            foreach (CardTemplateShort template in cardTemplates)
            {
                _cardTemplates.Add(template.Id, template);
            }
        }

        private void PreviewData(string resultData)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resultData);

                XmlNodeList resultNodes = xmlDoc.GetElementsByTagName("preview");
                if (resultNodes != null && resultNodes.Count > 0)
                {
                    XmlAttributeCollection attributes = resultNodes[0].Attributes;
                    if (attributes != null && attributes.Count > 0)
                    {
                        Image image = Image.FromStream(new MemoryStream(Convert.FromBase64String(attributes["front_data"].Value)));
                        Invoke(new MethodInvoker(delegate
                        {
                            _pbFrontLayout.Image = image;
                        }));

                        image = Image.FromStream(new MemoryStream(Convert.FromBase64String(attributes["back_data"].Value)));
                        Invoke(new MethodInvoker(delegate
                        {
                            _pbBackLayout.Image = image;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void CardPrintForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CardIdManagementManager.Singleton.CardPreviewFinished -= new CardIdManagementManager.CardPreviewFinishedHandler(CardPreviewFinished);
            CardIdManagementManager.Singleton.CardPrintNovaDialogOpened = false;
            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            UpdateAuthenticationKeys();
            if (this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
            }

            if (_printInProgress)
                CardIdManagementManager.Singleton.AnyProductionWindowOpened = false;
        }

        private void AddDroppedObject(object droppedObject)
        {
            if (droppedObject == null)
                return;

            if (droppedObject.GetType() == typeof(Person))
            {
                Person person = droppedObject as Person;
                if (person.Cards == null || person.Cards.Count == 0)
                    return;

                foreach (Card card in person.Cards)
                {
                    card.Person = person;
                    AddCardAndItsRelatedCardThread(card);
                }
            }
            else if (droppedObject.GetType() == typeof(Card))
            {
                Card card = droppedObject as Card;
                if (card == null)
                    return;

                AddCardAndItsRelatedCardThread(card);
            }
            else
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _dgvCards,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="person">card.Person can be a proxy, therefore full Person parameter can be used</param>
        private void AddCardAndItsRelatedCardThread(Card card)
        {
            if (card == null)
                return;

            SafeThread<Card>.StartThread<Card>(AddCardAndItsRelatedCard, card);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        private void AddCardAndItsRelatedCard(Card card)
        {
            if (card == null)
                return;

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            Exception ex = null;
            Card relatedCard = CgpClient.Singleton.MainServerProvider.CardPairs.GetRelatedCard(card.IdCard, out ex);

            lock (_syncRoot)
            {
                if (_cards.Add(card.IdCard))
                {
                    Invoke(new MethodInvoker(delegate()
                        {
                            _bindingCards.Add(new CardPrintEncode()
                                {
                                    Card = card,
                                    Person = _cardPrintAndEncodeManager.GetPersonFromCard(card),
                                    Template = CgpClient.Singleton.MainServerProvider.CardTemplates.ShortGetById(card.LastPrintEncodeTemplate, out ex)
                                });
                        }));
                }

                if (relatedCard != null)
                {
                    if (!_pickedRelatedCards.ContainsKey(card.IdCard) && !_pickedRelatedCards.ContainsKey(relatedCard.IdCard))
                        _pickedRelatedCards.Add(card.IdCard, relatedCard.IdCard);

                    if (_cards.Add(relatedCard.IdCard))
                    {
                        Invoke(new MethodInvoker(delegate()
                        {
                            _bindingCards.Add(new CardPrintEncode()
                            {
                                Card = relatedCard,
                                Person = _cardPrintAndEncodeManager.GetPersonFromCard(relatedCard),
                                Template = CgpClient.Singleton.MainServerProvider.CardTemplates.ShortGetById(relatedCard.LastPrintEncodeTemplate, out ex)
                            });
                        }));
                    }
                }
            }
        }

        private void SetDataGridViewSource()
        {
            if (_cardTemplates == null)
                FillCardTemplates();
            Invoke(new MethodInvoker(delegate()
                {
                    if (_dgvCards.DataSource == null)
                    {
                        if (_bindingCards == null)
                            _bindingCards = new SortableBindingList<CardPrintEncode>();

                        _dgvCards.DataSource = _bindingCards;

                        DataGridViewComboBoxColumn templateColumn = new DataGridViewComboBoxColumn();
                        templateColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                        templateColumn.Name = CardPrintEncode.COLUMNTEMPLATE;
                        if (_cardTemplates != null)
                            templateColumn.DataSource = _cardTemplates.Values.Select(value => new { Value = value, Display = value.Name }).ToList();
                        templateColumn.ValueType = typeof(CardTemplateShort);
                        templateColumn.DisplayMember = "Display";
                        templateColumn.ValueMember = "Value";
                        templateColumn.DataPropertyName = "Template";

                        _dgvCards.Columns.Remove(CardPrintEncode.COLUMNTEMPLATE);
                        _dgvCards.Columns.Insert(_dgvCards.Columns[CardPrintEncode.COLUMNPRINT].Index, templateColumn);

                        _dgvCards.Columns[CardPrintEncode.COLUMNCARD].ReadOnly = true;
                        _dgvCards.Columns[CardPrintEncode.COLUMNPERSON].ReadOnly = true;
                        _dgvCards.Columns[CardPrintEncode.COLUMNSYMBOL].ReadOnly = true;
                        _dgvCards.Columns[_dgvCards.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        foreach (DataGridViewColumn column in _dgvCards.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.Automatic;
                        }
                    }
                }));
        }

        private void AddCardToDataGridSuccess(CardPrintEncode card, string serialNumber)
        {
            Invoke(new MethodInvoker(delegate
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(_dgvSucceededCards, card.Symbol, card.Card, serialNumber, card.Person, DateTime.Now.ToString());
                _dgvSucceededCards.Rows.Add(row);
            }));
        }

        private void AddCardToDataGridFailed(CardPrintEncode card)
        {
            Invoke(new MethodInvoker(delegate
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(_dgvFailedCards, card.Symbol, card.Card, card.Person, DateTime.Now.ToString());
                _dgvFailedCards.Rows.Add(row);
            }));
        }


        private void _ilbCards_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bPrintEncode_Click(object sender, EventArgs e)
        {
            if (_cbCardTemplates.SelectedItem != null && _dgvCards.Rows.Count > 0)
            {
                if (_aKeys == null || _aKeys.Count == 0 || _bKeys == null || _bKeys.Count == 0)
                    if (!Dialog.WarningQuestion(GetString("QuestionAlternativeKeysNotDefined")))
                        return;

                _tcCardPrinting.SelectedTab = _tcCardPrinting.TabPages["_tpPrintProgress"];
                SafeThread.StartThread(PrintCardsThread);
            }
        }

        private bool _printInProgress = false;

        private void PrintCardsThread()
        {
            HashSet<CardPrintEncode> selectedCards = null;

            if (CardIdManagementManager.Singleton.AnyProductionWindowOpened)
            {
                Dialog.Error(GetString("ExceptionOccured") + ": " + IdManagementWindowOpenedException.GetDefaultLocalisedExceptionMessage());
                return;
            }

            PrepareCardPrintEncode(out selectedCards);

            CardIdManagementManager.Singleton.AnyProductionWindowOpened = true;
            _printInProgress = true;

            PrintEncodeCards(selectedCards);
            ProcessCardsPrintEncodeFinish();

            _printInProgress = false;
            CardIdManagementManager.Singleton.AnyProductionWindowOpened = false;
        }

        private void PrepareCardPrintEncode(out HashSet<CardPrintEncode> selectedCards)
        {
            Invoke(new MethodInvoker(delegate
            {
                _dgvSucceededCards.Rows.Clear();
                _dgvFailedCards.Rows.Clear();
                _rtbCardPrintProgress.Clear();
                _bConfigure.Enabled = false;
                _bPrintEncode.Enabled = false;
                _pbOverallProgress.Maximum = _cards.Count;
                _pbOverallProgress.Value = 0;
            }));

            UpdateAuthenticationKeys();
            selectedCards = new HashSet<CardPrintEncode>(_bindingCards);
        }

        private void UpdateAuthenticationKeys()
        {
            if (MifareSectorManager.Singleton.AlternativeAuthAKeysHexa == null)
                MifareSectorManager.Singleton.AlternativeAuthAKeysHexa = new List<string>();
            MifareSectorManager.Singleton.AlternativeAuthAKeysHexa.Clear();

            if (MifareSectorManager.Singleton.AlternativeAuthBKeysHexa == null)
                MifareSectorManager.Singleton.AlternativeAuthBKeysHexa = new List<string>();
            MifareSectorManager.Singleton.AlternativeAuthBKeysHexa.Clear();

            if (_aKeys != null)
                _aKeys.ForEach(key => MifareSectorManager.Singleton.AlternativeAuthAKeysHexa.Add(key.GetNonPasswordValue()));
            if (_bKeys != null)
                _bKeys.ForEach(key => MifareSectorManager.Singleton.AlternativeAuthBKeysHexa.Add(key.GetNonPasswordValue()));
        }

        private void ProcessCardsPrintEncodeFinish()
        {
            Invoke(new MethodInvoker(delegate
            {
                Dialog.Info(GetString("CardPrintFinished") + ": " + _dgvSucceededCards.Rows.Count + "/" + _dgvFailedCards.Rows.Count);
                _bConfigure.Enabled = true;
                _bPrintEncode.Enabled = true;
                _pbOverallProgress.Value = 0;
            }));
        }

        private void PrintEncodeCards(HashSet<CardPrintEncode> cards)
        {
            string resultText = string.Empty;
            string resultData = string.Empty;

            Dictionary<Guid, Guid> reversedRelatedCards = ReverseDictionary(_pickedRelatedCards);

            foreach (CardPrintEncode card in cards)
            {
                if (OmitCard(card.Card, _pickedRelatedCards, reversedRelatedCards))
                {
                    Invoke(new MethodInvoker(delegate
                        {
                            _pbOverallProgress.Value++;
                        }));
                    continue;
                }

                Card relatedCard = GetRelatedCardFromLoadedData(card.Card.IdCard, _pickedRelatedCards, reversedRelatedCards);
                CardAction? action = _cardPrintAndEncodeManager.GetCardProduceAction(card);

                if (ProduceSpecificCard(card, relatedCard, out resultText, out resultData))
                {
                    string serialNumber = GetSerialNumberFromResult(resultData);
                    Invoke(new MethodInvoker(delegate
                    {
                        AddCardToDataGridSuccess(card, serialNumber);
                        AppendResultText(card.ToString() + ": " + GetString("SuccessfullyPrinted") + Environment.NewLine, Color.Green);
                        _pbOverallProgress.Value++;
                    }));

                    SafeThread<CardPrintEncode, Card, string>.StartThread<CardPrintEncode, Card, string>(ProcessCardEncoded, card, relatedCard, serialNumber);
                }
                else
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        AddCardToDataGridFailed(card);
                        AppendResultText(card.ToString() + ": " + GetString("PrintFailed") + ": " + resultText + Environment.NewLine, Color.Red);
                        _pbOverallProgress.Value++;
                    }));
                }
            }
        }

        private void ProcessCardEncoded(CardPrintEncode card, Card relatedCard, string serialNumber)
        {
            if (card == null || card.Card == null || card.Template == null)
                return;

            if (card.Encode)
                InsertEventCardEncoded(card.Card, relatedCard, serialNumber);

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            Exception ex = null;
            Card editCard = CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEdit(card.Card.IdCard, out ex);
            if (editCard == null || ex != null)
                return;

            editCard.AlternateCardNumber = serialNumber;
            editCard.LastPrintEncodeTemplate = card.Template.Id;
            CgpClient.Singleton.MainServerProvider.Cards.Update(editCard, out ex);

            if (relatedCard != null)
            {
                relatedCard = CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEdit(relatedCard.IdCard, out ex);
                relatedCard.AlternateCardNumber = serialNumber;
                relatedCard.LastPrintEncodeTemplate = card.Template.Id;
                CgpClient.Singleton.MainServerProvider.Cards.Update(relatedCard, out ex);
            }
        }

        private void InsertEventCardEncoded(Card card, Card relatedCard, string serialNumber)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (card == null)
                return;

            List<Guid> eventSources = new List<Guid>();
            List<EventlogParameter> parameters = new List<EventlogParameter>();

            eventSources.Add(card.IdCard);
            if (card.Person != null)
                eventSources.Add(card.Person.IdPerson);

            if (relatedCard != null)
            {
                eventSources.Add(relatedCard.IdCard);
                if (relatedCard.Person != null)
                    eventSources.Add(relatedCard.Person.IdPerson);
            }

            if (serialNumber != null && serialNumber != string.Empty)
                parameters.Add(new EventlogParameter(EventlogParameter.TYPECARDSERIALNUMBER, serialNumber));
            parameters.Add(new EventlogParameter(EventlogParameter.TYPEDATETIME, DateTime.Now.ToString()));

            string description = string.Empty;
            if (relatedCard == null)
                description = "Card has been encoded";
            else
                description = "Hybrid card has been encoded";

            CgpClient.Singleton.MainServerProvider.Eventlogs.InsertEvent(Eventlog.TYPE_CARD_ENCODED,
                this.GetType().Assembly.GetName().Name,
                eventSources.ToArray(),
                description,
                parameters.ToArray());
        }

        private string GetSerialNumberFromResult(string xmlResultData)
        {
            if (xmlResultData == null || xmlResultData.Length == 0)
                return string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlResultData);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            XmlNodeList textNodes = xmlDoc.GetElementsByTagName("text");
            if (textNodes == null || textNodes.Count == 0)
                return string.Empty;

            foreach (XmlNode node in textNodes)
            {
                if (node.Attributes["text"] != null && node.Attributes["text"].Value.ToLower() == "uid")
                {
                    return ConvertToSerialNumber(node.InnerText);
                }
            }

            return string.Empty;
        }

        private string ConvertToSerialNumber(string hexaString)
        {
            try
            {
                byte[] bytes = Enumerable.Range(0, hexaString.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hexaString.Substring(x, 2), 16))
                         .ToArray();


                return "20" + BitConverter.ToUInt32(bytes, 0).ToString();
            }
            catch { }

            return string.Empty;
        }

        private Card GetRelatedCardFromLoadedData(Guid cardId, Dictionary<Guid, Guid> originalRelatedCards, Dictionary<Guid, Guid> reversedRelatedCards)
        {
            if (originalRelatedCards == null || reversedRelatedCards == null
                || originalRelatedCards.Count == 0 || reversedRelatedCards.Count == 0)
                return null;

            if (!originalRelatedCards.ContainsKey(cardId) && !reversedRelatedCards.ContainsKey(cardId))
                return null;

            Guid? relatedCardId = null;
            if (originalRelatedCards.ContainsKey(cardId))
                relatedCardId = originalRelatedCards[cardId];
            else if (reversedRelatedCards.ContainsKey(cardId))
                relatedCardId = reversedRelatedCards[cardId];

            if (relatedCardId == null)
                return null;

            if (!_cards.Contains(relatedCardId.Value))
                return null;

            return _bindingCards.First(c => c.Card.IdCard.Equals(relatedCardId.Value)).Card;
        }

        private Card GetRelatedCard(Guid cardId)
        {
            throw new NotImplementedException();
        }

        private bool OmitCard(Card card, Dictionary<Guid, Guid> originalRelatedTable, Dictionary<Guid, Guid> reversedRelatedCards)
        {
            if (card == null)
                return true;

            if (originalRelatedTable == null || reversedRelatedCards == null
                || originalRelatedTable.Count == 0 || reversedRelatedCards.Count == 0)
                return false;

            if (reversedRelatedCards.ContainsKey(card.IdCard))
                return true;

            return false;
        }

        private Dictionary<Guid, Guid> ReverseDictionary(Dictionary<Guid, Guid> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return null;

            Dictionary<Guid, Guid> result = new Dictionary<Guid, Guid>();
            foreach (KeyValuePair<Guid, Guid> item in dictionary)
            {
                try
                {
                    result.Add(item.Value, item.Key);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    return null;
                }
            }

            return result;
        }

        private void AppendResultText(string text, Color textColor)
        {
            _rtbCardPrintProgress.SelectionStart = _rtbCardPrintProgress.Text.Length;
            _rtbCardPrintProgress.SelectionLength = 0;
            _rtbCardPrintProgress.SelectionColor = textColor;
            _rtbCardPrintProgress.AppendText(text);
            _rtbCardPrintProgress.SelectionColor = _rtbCardPrintProgress.ForeColor;
        }

        private bool ProduceSpecificCard(CardPrintEncode card, out string resultText, out string resultData)
        {
            return ProduceSpecificCard(card, null, out resultText, out resultData);
        }

        private bool ProduceSpecificCard(CardPrintEncode card, Card relatedCard, out string resultText, out string resultData)
        {
            resultText = string.Empty;
            resultData = string.Empty;
            if (card == null)
                return false;

            Invoke(new MethodInvoker(delegate
                {
                    if (_rtbCardPrintProgress.Text != null && _rtbCardPrintProgress.Text != string.Empty)
                        AppendResultText(Environment.NewLine, Color.Black);
                    AppendResultText(card.ToString() + ": " + GetString("PrintEncodeStart") + Environment.NewLine, Color.Black);
                }));

            if (card.Template == null)
                return false;

            CardTemplate template = _cardPrintAndEncodeManager.GetTemplate(card.Template.Id);

            CardTemplate filledTemplate;
            try
            {
                filledTemplate = _cardPrintAndEncodeManager.CreateFilledTemplate(template, card, relatedCard, null);
                if (filledTemplate == null)
                    return false;
            }
            catch (Exception ex)
            {
                resultText = ex.Message;
                //HandledExceptionAdapter.Examine(ex);
                return false;
            }

            return CardIdManagementManager.Singleton.DoCreateCardWithoutWindowCheck(filledTemplate.GetDataAsString(), out resultText, out resultData);
        }

        private void _bConfigure_Click(object sender, EventArgs e)
        {
            Exception ex = null;
            if (!CardIdManagementManager.Singleton.RunConfigurationDialog(out ex))
            {
                if (ex == null)
                    Dialog.Error(GetString("FailedToOpenIdManagementDialog"));
                else
                    Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
            }
        }

        private void _bRemoveCard_Click(object sender, EventArgs e)
        {
            if (_dgvCards.SelectedRows == null || _dgvCards.SelectedRows.Count == 0)
                return;

            for (int i = _dgvCards.SelectedRows.Count - 1; i >= 0; i--)
            {
                if (_dgvCards.SelectedRows[i].Cells[CardPrintEncode.COLUMNCARD] == null || !(_dgvCards.SelectedRows[i].Cells[CardPrintEncode.COLUMNCARD].Value is Card))
                    continue;

                _cards.Remove((_dgvCards.SelectedRows[i].Cells[CardPrintEncode.COLUMNCARD].Value as Card).IdCard);
                RemoveRelationRecord((_dgvCards.SelectedRows[i].Cells[CardPrintEncode.COLUMNCARD].Value as Card).IdCard);
                _bindingCards.RemoveAt(_dgvCards.SelectedRows[i].Index);
            }
        }

        private void RemoveRelationRecord(Guid cardId)
        {
            if (_pickedRelatedCards.Remove(cardId))
                return;

            Guid keyToRemove = Guid.Empty;
            foreach (KeyValuePair<Guid, Guid> item in _pickedRelatedCards)
            {
                if (item.Value.Equals(cardId))
                {
                    keyToRemove = item.Key;
                    break;
                }
            }

            _pickedRelatedCards.Remove(keyToRemove);
        }

        private void CardPrintForm_Enter(object sender, EventArgs e)
        {
            CgpClientMainForm.Singleton.AddToOpenWindows(this);
            CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (MdiParent != null && this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                Dock = DockStyle.Fill;
            }
            if (MdiParent != null && this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            if (this.WindowState == FormWindowState.Maximized || Dock == DockStyle.Fill)
            {
                if (_bMenuMaximize != null)
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoNormalSize");
            }
            else
            {
                if (_bMenuMaximize != null)
                    _bMenuMaximize.Text = LocalizationHelper.GetString("TextInfoMaximize");
            }
            base.OnSizeChanged(e);
            RemoveBackPanel();
        }

        private void _dgvCards_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgvCards_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null)
                    return;
                AddDroppedObject((object)e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _dgvCards_SelectionChanged(object sender, EventArgs e)
        {
            if (!_chbRunPreview.Checked)
                return;

            DataGridViewRow currentRow = _dgvCards.CurrentRow;
            if (currentRow == null)
                return;

            if (_bindingCards == null || _bindingCards.Count <= currentRow.Index)
                return;

            AddToPreview(_bindingCards[currentRow.Index]);
        }

        private void _bCSVExport_Click(object sender, EventArgs e)
        {
            if (_dgvSucceededCards.Rows == null || _dgvSucceededCards.Rows.Count == 0)
                return;

            if (_cbCSVSeparator.Text == null || _cbCSVSeparator.Text == string.Empty)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _cbCSVSeparator, GetString("NoSelectedSeparator"), ControlNotificationSettings.Default);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "csv";
            sfd.Filter = "All types (*.*)|*.*|Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            sfd.FilterIndex = 2;
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            TextWriter tw = null;
            try
            {
                tw = File.CreateText(sfd.FileName);
                string separator = _cbCSVSeparator.Text;
                if (separator.ToLower() == "tab")
                    separator = "\t";

                foreach (DataGridViewRow row in _dgvSucceededCards.Rows)
                {
                    tw.WriteLine((row.Cells[CardsSucceededDGVColumn.ColumnPerson2.ToString()].Value == null ? string.Empty : row.Cells[CardsSucceededDGVColumn.ColumnPerson2.ToString()].Value.ToString())
                        + separator +
                        (row.Cells[CardsSucceededDGVColumn.ColumnCard2.ToString()].Value == null ? string.Empty : row.Cells[CardsSucceededDGVColumn.ColumnCard2.ToString()].Value.ToString())
                        + separator +
                        (row.Cells[CardsSucceededDGVColumn.ColumnSerialNumber2.ToString()].Value == null ? string.Empty : row.Cells[CardsSucceededDGVColumn.ColumnSerialNumber2.ToString()].Value.ToString())
                        + separator +
                        (row.Cells[CardsSucceededDGVColumn.ColumnDateTime2.ToString()].Value == null ? string.Empty : row.Cells[CardsSucceededDGVColumn.ColumnDateTime2.ToString()].Value.ToString()));
                }
                tw.Flush();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (tw != null)
                    try
                    {
                        tw.Close();
                    }
                    catch { }
            }
        }

        private void _dgvCards_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void _bApplyTemplate_Click(object sender, EventArgs e)
        {
            if (_dgvCards.SelectedRows == null)
                return;

            if (!(_cbCardTemplates.SelectedItem is CardTemplateShort))
                return;

            CardTemplateShort template = _cbCardTemplates.SelectedItem as CardTemplateShort;

            foreach (DataGridViewRow row in _dgvCards.SelectedRows)
            {
                _bindingCards[row.Index].Template = template;
            }
            _dgvCards.Refresh();
            _dgvCards_SelectionChanged(null, null);
            _cbCardTemplates.Focus();
        }

        private void _bApplyPrint_Click(object sender, EventArgs e)
        {
            if (_dgvCards.SelectedRows == null)
                return;

            bool print = _chbPrint.Checked;

            foreach (DataGridViewRow row in _dgvCards.SelectedRows)
            {
                _bindingCards[row.Index].Print = print;
            }
            _dgvCards.Refresh();
            _chbPrint.Focus();
        }

        private void _bApplyEncode_Click(object sender, EventArgs e)
        {
            if (_dgvCards.SelectedRows == null)
                return;

            bool encode = _chbEncode.Checked;

            foreach (DataGridViewRow row in _dgvCards.SelectedRows)
            {
                _bindingCards[row.Index].Encode = encode;
            }
            _dgvCards.Refresh();
            _chbEncode.Focus();
        }

        private void _dgvCards_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _dgvCards.Columns[CardPrintEncode.COLUMNTEMPLATE].Index)
                _dgvCards_SelectionChanged(null, null);
        }

        private void _chbRunPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (_chbRunPreview.Checked)
                _dgvCards_SelectionChanged(null, null);
        }

        private void _dgvAAuthKeys_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                if (((sender as DataGridView).DataSource as BindingSource).Current != null &&
                    (((sender as DataGridView).DataSource as BindingSource).Current as PasswordString).UsePasswordChar)
                {
                    (e.Control as TextBox).PasswordChar = PasswordString.PASSWORD_CHAR;
                }
                else
                    (e.Control as TextBox).PasswordChar = (char)0;

                (e.Control as TextBox).MaxLength = 12;
            }
        }

        private void _bAddAKey_Click(object sender, EventArgs e)
        {
            (_dgvAAuthKeys.DataSource as BindingSource).Insert(0, new PasswordString(string.Empty));
        }

        private void _bRemoveAKey_Click(object sender, EventArgs e)
        {
            if ((_dgvAAuthKeys.DataSource as BindingSource).Current != null)
                (_dgvAAuthKeys.DataSource as BindingSource).RemoveCurrent();
        }

        private void _bRemoveBKey_Click(object sender, EventArgs e)
        {
            if ((_dgvBAuthKeys.DataSource as BindingSource).Current != null)
                (_dgvBAuthKeys.DataSource as BindingSource).RemoveCurrent();
        }

        private void _bAddBKey_Click(object sender, EventArgs e)
        {
            (_dgvBAuthKeys.DataSource as BindingSource).Insert(0, new PasswordString(string.Empty));
        }

        private void SplitContainerPaint(object sender, PaintEventArgs e)
        {
            var control = sender as SplitContainer;
            //paint the three dots'
            Point[] points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;

            //calculate the position of the points'
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w / 2), d + (sW / 2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);
            }
            else
            {
                points[0] = new Point(d + (sW / 2), (h / 2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);
            }

            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(Brushes.Black,
                    new Rectangle(p, new Size(3, 3)));

                p.Offset(1, 1);
                e.Graphics.FillEllipse(Brushes.LightGray,
                    new Rectangle(p, new Size(3, 3)));
            }
        }

        protected override void RegisterEvents()
        {
        }

        protected override void BeforeInsert()
        {
        }

        protected override void BeforeEdit()
        {
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            allowEdit = false;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
        }

        protected override void SetValuesInsert()
        {
        }

        protected override void SetValuesEdit()
        {
        }

        protected override bool CheckValues()
        {
            return true;
        }

        protected override bool GetValues()
        {
            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            return true;
        }

        protected override bool SaveToDatabaseEdit()
        {
            return true;
        }

        protected override void UnregisterEvents()
        {
        }

        protected override void EditEnd()
        {
        }

        protected override void AfterInsert()
        {
        }

        protected override void AfterEdit()
        {
        }
    }
}
