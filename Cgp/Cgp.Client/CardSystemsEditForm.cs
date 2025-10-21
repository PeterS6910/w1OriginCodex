using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using System.Reflection;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Parsing;
using Contal.IwQuick.Data;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using System.Threading;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CardSystemsEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<CardSystem>
#endif

    {
        public const string PASSWORD_TEXT = "●●●●●●●●●●●●";

        private double _maxCardNumber = 0;
        private bool _firstTimeOnEnter = true;

        public CardSystemsEditForm(CardSystem cardSystem, ShowOptionsEditForm showOption)
            : base(cardSystem, showOption)
        {
            InitializeComponent();
            FormOnEnter += new Action<Form>(LoadCards);
            SetReferenceEditColors();
            WheelTabContorol = _tcCardSystem;

            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbCardSubType.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbCardType.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbEncoding.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _tbGenerationResult.MouseWheel += new MouseEventHandler(ControlMouseWheel);

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageProperties(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsPropertiesView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsPropertiesAdmin)));

                HideDisableTabPageSmartCardData(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsSmartCardDataView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsSmartCardDataAdmin)));

                HideDisableTabPageCardSerie(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsCardSerieView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsCardSerieAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardSystemsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageProperties(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageProperties),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcCardSystem.TabPages.Remove(_tpProperties);
                    return;
                }

                _tpProperties.Enabled = admin;
            }
        }

        private void HideDisableTabPageSmartCardData(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSmartCardData),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCardSystem.TabPages.Remove(_tpSmartCardData);
                    return;
                }

                _tpSmartCardData.Enabled = admin;
            }
        }

        private void HideDisableTabPageCardSerie(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageCardSerie),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCardSystem.TabPages.Remove(_tpCardGeneration);
                    return;
                }

                _tpCardGeneration.Enabled = admin;
            }
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabFoldersSructure), view);
            }
            else
            {
                if (!view)
                {
                    _tcCardSystem.TabPages.Remove(_tpUserFolders);
                    return;
                }
            }
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabReferencedBy), view);
            }
            else
            {
                if (!view)
                {
                    _tcCardSystem.TabPages.Remove(_tpReferencedBy);
                    return;
                }
            }
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDescription),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCardSystem.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private Action<Guid, int> _eventCardsGenerated = null;
        protected override void RegisterEvents()
        {
            _eventCardsGenerated = new Action<Guid, int>(CardsGenerated);
            CardGenerationFinisheEventHandler.Singleton.RegisterCardGenerationFinished(_eventCardsGenerated);
        }

        protected override void UnregisterEvents()
        {
            CardGenerationFinisheEventHandler.Singleton.UnregisterCardGenerationFinished(_eventCardsGenerated);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("CardSystemsEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            CardSystemsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            CardSystemsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            CardSystem obj = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectForEdit(_editingObject.IdCardSystem, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(_editingObject.IdCardSystem);
                }
                else
                {
                    throw error;
                }
                DisabledForm();
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;
            CgpClient.Singleton.MainServerProvider.CardSystems.RenewObjectForEdit(_editingObject.IdCardSystem, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            //LoadCardTypes();
            _bCreate.Visible = false;
            SetActualFullCompanyCode();


            ShowTabSmartCardData();
            SetControlsForCardType();

            if (_tcCardSystem.TabPages.Contains(_tpCardGeneration))
                _tcCardSystem.TabPages.Remove(_tpCardGeneration);
        }

        protected override void SetValuesEdit()
        {
            //LoadCardTypes();

            SetCardDataLengthRange();

            _eName.Text = _editingObject.Name;

            //LoadCardSubTypes(true);
            SetActualFullCompanyCode();
            SetReferencedBy();
            ShowTabSmartCardData();
            SetControlsForCardType();
            SetMaxCardNumber();

            _eCompanyCode.Text = _editingObject.CompanyCode;
            _eDescription.Text = _editingObject.Description;
        }

        private void CardDataLengthChanged(object sender, EventArgs e)
        {
            if (_sectorManager != null)
                _sectorManager.CardNumberDigitsStoreLength = (byte)(_tbCardDataLength.Value - 2);
            this.EditTextChanger(sender, e);
        }

        private void SetBKeysVisibilityThread()
        {
            SafeThread.StartThread(SetBKeysVisibility);
        }

        private void SetBKeysVisibility()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            bool hasAccessTemplateAdmin = CardTemplatesForm.HasAccessUpdate();
            bool superAdmin = CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs);

            bool visible = true;
            if (!superAdmin && !hasAccessTemplateAdmin)
                visible = false;

            Invoke(new MethodInvoker(delegate
                {
                    if (_dgvSectors.Columns.Contains("BKeyText"))
                        _dgvSectors.Columns["BKeyText"].Visible = visible;

                    if (_dgvSectors.Columns.Contains("InheritBKey"))
                        _dgvSectors.Columns["InheritBKey"].Visible = visible;

                    _lBkey.Visible = visible;
                    _eBkey.Visible = visible;
                }));
        }

        private void CardsGenerated(Guid cardSystemGuid, int successCount)
        {
            _stopAutoProgress = true;
            if (InvokeRequired)
                BeginInvoke(new Action<Guid, int>(CardsGenerated), cardSystemGuid, successCount);
            else
            {
                _tbGenerationResult.Text += GetString("CardGenerationFinished") + successCount + "/" + _cardsToGenerate + Environment.NewLine;
                _pbCardGeneration.Value = 0;
                CardsForm.Singleton.RefreshData();
            }
        }

        private void SetMaxCardNumber()
        {
            try
            {
                double freeDigitCount = _editingObject.LengthCardData - _editingObject.GetFullCompanyCode(GetActCardSubType(), _editingObject.CompanyCode).Length;

                if (freeDigitCount == 0)
                {
                    _nudCardNumberFrom.Maximum = 0;
                    _nudCardNumberTo.Maximum = 0;
                    _bGenerate.Enabled = false;
                    return;
                }
                if (!_bGenerate.Enabled) _bGenerate.Enabled = true;

                _maxCardNumber = Math.Pow(10, freeDigitCount) - 1;
                _nudCardNumberFrom.Maximum = (decimal)_maxCardNumber - 1;
                _nudCardNumberTo.Maximum = (decimal)_maxCardNumber;
            }
            catch
            {
                _maxCardNumber = (double)decimal.MaxValue;
                _nudCardNumberFrom.Maximum = decimal.MaxValue - 1;
                _nudCardNumberTo.Maximum = decimal.MaxValue;
            }
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = false;
        }

        private void LoadCardTypes()
        {
            IList<CardTypes> cardTypes = CardTypes.GetCardTypesList();

            foreach (CardTypes cardType in cardTypes)
            {
                if (cardType.Value == CardType.DirectSerial) continue;

                _cbCardType.Items.Add(cardType);
            }

            if (!Insert)
            {
                CardTypes cardType = CardTypes.GetCardType(cardTypes, _editingObject.CardType);
                _cbCardType.SelectedItem = cardType;
            }
        }

        private void LoadCardSubTypes()
        {
            _cbCardSubType.DataSource = CardSubTypes.GetCardSubTypesList((CardType)_editingObject.CardType);
        }

        private void SetActualFullCompanyCode()
        {
            _lActualFullCompanyCode.Text = _editingObject.GetFullCompanyCode(GetActCardSubType(), _eCompanyCode.Text);
        }

        private CardSubTypes GetActCardSubType()
        {
            if (_cbCardSubType.SelectedItem != null)
            {
                if (_cbCardSubType.SelectedItem is CardSubTypes)
                    return _cbCardSubType.SelectedItem as CardSubTypes;
            }

            return null;
        }

        private void LoadCards(Form form)
        {
            LoadCards(form, true);
        }

        private void LoadCards(Form form, bool refreshCards)
        {
            if (!Insert)
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                try
                {
                    if (!_firstTimeOnEnter)
                        _editingObject = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(_editingObject.IdCardSystem);

                    if (_editingObject == null)
                        Close();

                    if (refreshCards)
                        _lbCards.Items.Clear();

                    if (_editingObject.Cards != null && _editingObject.Cards.Count > 0)
                    {
                        if (refreshCards)
                        {
                            List<Card> cards = new List<Card>();
                            foreach (Card card in _editingObject.Cards.OrderBy(c => c.FullCardNumber))
                            {
                                if (_eFilterCards.Text == "" || card.ToString().IndexOf(_eFilterCards.Text) >= 0)
                                    cards.Add(card);
                            }

                            _lbCards.Items.AddRange(cards.ToArray());
                        }

                        DisabledControls(_tpProperties);
                    }
                    else
                    {
                        if (_isOpenCreateCardForm)
                            DisabledControls(_tpProperties);
                        else
                            EnabledControls(_tpProperties);
                    }
                }
                catch
                {
                }
            }
            _firstTimeOnEnter = false;
        }

        private void SetCardDataLengthRange()
        {
            if (_cbCardType.SelectedItem != null)
            {
                if (_cbCardType.SelectedItem is CardTypes)
                {
                    _tbCardDataLength.Minimum = (_cbCardType.SelectedItem as CardTypes).CardDataLengthMin;
                    _tbCardDataLength.Maximum = (_cbCardType.SelectedItem as CardTypes).CardDataLengthMax;
                    EditTextChanger(_tbCardDataLength, null);
                }
            }
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.LengthCompanyCode = (byte)_eCompanyCode.Text.Length;
                _editingObject.CompanyCode = _eCompanyCode.Text;
                _editingObject.Description = _eDescription.Text;

                if (Insert)
                {
                    _editingObject.CardSystemNumber = CgpClient.Singleton.MainServerProvider.CardSystems.GetCardSystemNumber();
                }

                if (IsMifareSmartCardDataUsed())
                {
                    MakeCypherData(_mifareCardData);
                    _editingObject.CardData = _mifareCardData;
                }

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));

                return false;
            }
        }

        private void UpdateSectorsInfo(MifareSectorData parentObject)
        {
            if (parentObject == null || _sectorManager == null)
                return;

            if (parentObject.SectorsInfo == null)
                parentObject.SectorsInfo = new List<MifareSectorSectorInfo>();
            parentObject.SectorsInfo.Clear();
            foreach (MifareSectorSetting setting in _sectorManager.SectorSettings.Values)
            {
                parentObject.SectorsInfo.Add(new MifareSectorSectorInfo()
                {
                    AKey = setting.AKey,
                    BKey = setting.BKey,
                    Bank = setting.Bank,
                    InheritAKey = setting.InheritAKey,
                    InheritBKey = setting.InheritBKey,
                    Length = setting.Length,
                    Offset = setting.Offset,
                    SectorNumber = setting.SectorNumber,
                });
            }
        }

        private void SetKeysToEditingObject()
        {
            if (!(_editingObject.CardData is MifareSectorData))
                return;

            int discarded = 0;

            if (_eAkey.Text != PASSWORD_TEXT)
                (_editingObject.CardData as MifareSectorData).GeneralAKey = GetXteaEncryptedValue(HexEncoding.GetBytes(_eAkey.Text, out discarded));

            if (_eBkey.Text != PASSWORD_TEXT)
                (_editingObject.CardData as MifareSectorData).GeneralBKey = GetXteaEncryptedValue(HexEncoding.GetBytes(_eBkey.Text, out discarded));

            if ((_editingObject.CardData as MifareSectorData).SectorsInfo == null)
                return;

            //this will order the sector info records for better control
            Dictionary<int, MifareSectorSectorInfo> orderedSectorInfo = new Dictionary<int, MifareSectorSectorInfo>();
            foreach (MifareSectorSectorInfo sectorInfo in (_editingObject.CardData as MifareSectorData).SectorsInfo)
            {
                if (!orderedSectorInfo.ContainsKey(sectorInfo.SectorNumber))
                    orderedSectorInfo.Add(sectorInfo.SectorNumber, sectorInfo);
            }

            //this will be not needed later
            foreach (MifareSectorSetting sectorSetting in _sectorManager.SectorSettings.Values)
            {
                //means that this record is not in database, so must be added to SectorsInfo property
                //(situation may happen after database conversion 1.91)
                if (!orderedSectorInfo.ContainsKey(sectorSetting.SectorNumber))
                {
                    MifareSectorSectorInfo sectorInfo = new MifareSectorSectorInfo();
                    (_editingObject.CardData as MifareSectorData).SectorsInfo.Add(sectorInfo);
                    orderedSectorInfo.Add(sectorSetting.SectorNumber, new MifareSectorSectorInfo());
                }

                orderedSectorInfo[sectorSetting.SectorNumber].AKey = sectorSetting.AKey;
                orderedSectorInfo[sectorSetting.SectorNumber].BKey = sectorSetting.BKey;
                orderedSectorInfo[sectorSetting.SectorNumber].InheritAKey = sectorSetting.InheritAKey;
                orderedSectorInfo[sectorSetting.SectorNumber].InheritBKey = sectorSetting.InheritBKey;
                orderedSectorInfo[sectorSetting.SectorNumber].SectorNumber = sectorSetting.SectorNumber;

            }
        }

        private byte[] GetXteaEncryptedValue(byte[] dataToEncrypt)
        {
            if (dataToEncrypt == null)
                return null;

            IwQuick.Crypto.XTEA cryptoXtea = new Contal.IwQuick.Crypto.XTEA();
            cryptoXtea.XTeaInit();

            return cryptoXtea.XTeaFrameEnc(dataToEncrypt);
        }

        private byte[] GetXteaDecryptedValue(byte[] dataToDecrypt)
        {
            if (dataToDecrypt == null)
                return null;

            IwQuick.Crypto.XTEA cryptoXtea = new Contal.IwQuick.Crypto.XTEA();
            cryptoXtea.XTeaInit();

            return cryptoXtea.XTeaFrameDec(dataToDecrypt);
        }

        protected override bool CheckValues()
        {
            if (_eName.Text == "")
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorEntryName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            if (_cbCardType.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbCardType,
                GetString("ErrorEntryCardType"), CgpClient.Singleton.ClientControlNotificationSettings);
                _cbCardType.Focus();
                return false;
            }

            if (_cbCardSubType.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbCardSubType,
                GetString("ErrorEntryCardSubType"), CgpClient.Singleton.ClientControlNotificationSettings);
                _cbCardType.Focus();
                return false;
            }

            if (_eCompanyCode.Text != GetValidCompanyCode())
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eCompanyCode,
                    GetString("ErrorCompanyCodeNotValid"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return false;
            }

            if (IsMifareSmartCardDataUsed())
            {
                if (_sectorManager != null)
                {
                    UpdateSectorsInfo(_mifareCardData);
                    if (!_sectorManager.IsSumBankLengthValid())
                    {
                        if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                            _tcCardSystem.SelectedTab = _tpSmartCardData;

                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _dgvSectors,
                       GetString("WarningSumBankLengthNotValid"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                        return false;
                    }
                    if (_mifareCardData != null && _mifareCardData.GeneralAKey == null && _sectorManager.SectorSettings.Values.Any(sectorSetting => sectorSetting.InheritAKey))
                    {
                        if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                            _tcCardSystem.SelectedTab = _tpSmartCardData;

                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eAkey,
                       GetString("WarningKeyInheritedButEmpty"), ControlNotificationSettings.Default);
                        return false;
                    }
                    if (_mifareCardData != null && _mifareCardData.GeneralBKey == null && _sectorManager.SectorSettings.Values.Any(sectorSetting => sectorSetting.InheritBKey))
                    {
                        if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                            _tcCardSystem.SelectedTab = _tpSmartCardData;

                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eBkey,
                       GetString("WarningKeyInheritedButEmpty"), ControlNotificationSettings.Default);
                        return false;
                    }
                    if (_sectorManager.SectorSettings.Values.Any(sectorSetting => (sectorSetting.InheritBKey || sectorSetting.BKey != null) && (sectorSetting.AKey == null && !sectorSetting.InheritAKey)))
                    {
                        if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                            _tcCardSystem.SelectedTab = _tpSmartCardData;

                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _dgvSectors,
                       GetString("WarningEmptyAKeyWhenBDefined"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                        return false;
                    }
                    if (GeneralOptionsForm.Singleton.UniqueAKeyCSRestriction && CgpClient.Singleton.MainServerProvider.CardSystems.IsMifareSectorDataInCollision(_mifareCardData))
                    {
                        if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                            _tcCardSystem.SelectedTab = _tpSmartCardData;

                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _dgvSectors,
                       GetString("ErrorAKeyConflict"), new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
                        return false;
                    }
                }
            }
            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.CardSystems.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorNameOrCompanyCodeExists"));
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            return SaveToDatabaseEditCore(false);
        }

        protected override bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEditCore(true);
        }

        private bool SaveToDatabaseEditCore(bool onlyInDatabase)
        {
            Exception error;
            bool retValue;

            if (onlyInDatabase)
                retValue = CgpClient.Singleton.MainServerProvider.CardSystems.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = CgpClient.Singleton.MainServerProvider.CardSystems.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorNameOrCompanyCodeExists"));
                }
                else
                    throw error;
            }

            return retValue;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override void AfterInsert()
        {
            CardSystemsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            CardSystemsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            if (sender == _eCompanyCode)
            {
                string validText = GetValidCompanyCode();
                if (_eCompanyCode.Text != validText)
                {
                    _eCompanyCode.Text = validText;
                    _eCompanyCode.SelectionStart = _eCompanyCode.Text.Length;
                    _eCompanyCode.SelectionLength = 0;
                }
            }

            base.EditTextChanger(sender, e);

            if (sender == _tbCardDataLength)
            {
                _lCardDataLengthActual.Text = _tbCardDataLength.Value.ToString();
            }

            if (sender == _tbCardDataLength || sender == _cbCardSubType || sender == _eCompanyCode)
            {
                CardSubTypes subType = GetActCardSubType();
                int prefixLength = subType == null ? 0 : subType.Number.Length;
                int fullCardPrefixLength = _eCompanyCode.Text.Length + prefixLength;

                //at least one character must be available for card number
                if (fullCardPrefixLength + 1 > _tbCardDataLength.Value)
                {
                    if (fullCardPrefixLength + 1 > _tbCardDataLength.Maximum)
                        _tbCardDataLength.Value = _tbCardDataLength.Maximum;
                    else
                        _tbCardDataLength.Value = fullCardPrefixLength + 1;
                }

                _lCompanyCodeLengthActual.Text = _eCompanyCode.Text.Length.ToString();
            }

            if (sender == _cbCardType)
            {
                //LoadCardSubTypes(false);
                SetCardDataLengthRange();
            }

            if (sender == _cbCardSubType || sender == _eCompanyCode || sender == _cbCardType)
            {
                SetActualFullCompanyCode();
            }
        }

        /// <summary>
        /// Get valid string for company code
        /// </summary>
        /// <returns></returns>
        private string GetValidCompanyCode()
        {
            bool onlyDigitString = true;

            CardTypes cardTypes = _cbCardType.SelectedItem as CardTypes;
            if (cardTypes != null)
            {
                switch (cardTypes.Value)
                {
                    case CardType.Mifare:
                        CardSubTypes cardSubTypes = _cbCardSubType.SelectedItem as CardSubTypes;
                        if (cardSubTypes != null && (cardSubTypes.Value == CardSubType.MifareStandardSectorReadin || cardSubTypes.Value == CardSubType.MifareSectorReadinWithMAD))
                        {
                            EncodingCBObject encodingCBObjectMifare = _cbEncoding.SelectedItem as EncodingCBObject;
                            if (encodingCBObjectMifare != null && encodingCBObjectMifare.EncodingType != EncodingType.Bcd)
                            {
                                onlyDigitString = false;
                            }
                        }
                        break;
                    case CardType.MifarePlus:
                        EncodingCBObject encodingCBObjectMifarePlus = _cbEncoding.SelectedItem as EncodingCBObject;
                        if (encodingCBObjectMifarePlus != null && encodingCBObjectMifarePlus.EncodingType != EncodingType.Bcd)
                        {
                            onlyDigitString = false;
                        }
                        break;
                }
            }

            if (onlyDigitString)
                return QuickParser.GetValidDigitString(_eCompanyCode.Text);
            else
                return QuickParser.GetValidPrintableString(_eCompanyCode.Text);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        private void _lbCards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_lbCards.SelectedItem != null)
            {
                CardsForm.Singleton.OpenEditForm(_lbCards.SelectedItem as Card);
            }
        }

        private System.Threading.Timer _keyUpTimer = null;
        private TimerCallback _tcbkeyUpTimer = null;
        private bool _isWaitingForKey = false;
        private const int KEY_UP_DELAY = 1000;


        private void _eFilterCards_KeyUp(object sender, KeyEventArgs e)
        {
            if (_tcbkeyUpTimer == null)
            {
                _tcbkeyUpTimer = new TimerCallback(OnKeyUpTimer);
            }
            if (!_isWaitingForKey)
            {
                _isWaitingForKey = true;

                //ensure dispose of old timer
                if (_keyUpTimer != null)
                {
                    _keyUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _keyUpTimer.Dispose();
                }

                _keyUpTimer = new System.Threading.Timer(_tcbkeyUpTimer, null, KEY_UP_DELAY, Timeout.Infinite);
            }
            else
            {
                _isWaitingForKey = true;
                _keyUpTimer.Change(KEY_UP_DELAY, Timeout.Infinite);
            }
        }

        private void OnKeyUpTimer(object obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Form>(LoadCards), this);
            }
            else
            {
                LoadCards(this);
            }
            _isWaitingForKey = false;
        }

        private bool _isOpenCreateCardForm = false;
        private void _bCreate_Click(object sender, EventArgs e)
        {
            if (ValueChanged)
            {
                if (Contal.IwQuick.UI.Dialog.Question(LocalizationHelper.GetString("QuestionSaveCardSystemBeforeCreateCard")))
                {
                    if (!SaveData(false))
                        return;
                }
                else
                {
                    return;
                }
            }

            Card card = new Card();
            card.CardSystem = _editingObject;

            //if (CardsForm.Singleton.OpenInsertDialg(ref card))
            //{
            //    LoadCards(this);
            //    EditTextChanger(_lbCards, null);
            //}
            CardsForm.Singleton.OpenInsertFromEdit(ref card, null, new DVoid2Void(AfterInsertClosed));
            _isOpenCreateCardForm = true;
        }

        private void AfterInsertClosed()
        {
            _isOpenCreateCardForm = false;
            LoadCards(this, false);
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.CardSystems != null)
                CgpClient.Singleton.MainServerProvider.CardSystems.EditEnd(_editingObject);
        }

        private void SetCyhperValues()
        {
            /*byte[] msg = _editingObject.CypherData;
            if (msg == null) return;

            _nudLengthBytes1.Value = msg[3];
            _nudLengthBytes2.Value = msg[4];
            _nudLengthBytes3.Value = msg[5];

            byte[] crypted = new byte[msg.Length - 6];
            Array.Copy(msg, 6, crypted, 0, msg.Length - 6);
            XTEA xtea = new XTEA();
            xtea.XTeaInit();
            byte[] decrypted = xtea.XTeaFrameDec(crypted);

            _eBank1Sector.Value = decrypted[0] < _eBank1Sector.Minimum ? _eBank1Sector.Minimum : decrypted[0];
            _eBank1Offset.Value = decrypted[2];

            _eBank2Sector.Value = decrypted[4] < _eBank2Sector.Minimum ? _eBank2Sector.Minimum : decrypted[4];
            _eBank2Offset.Value = decrypted[6];

            _eBank3Sector.Value = decrypted[8] < _eBank3Sector.Minimum ? _eBank3Sector.Minimum : decrypted[8];
            _eBank3Offset.Value = decrypted[10];

            _cbExplicitSmartCardDataPopulation.Checked = _editingObject.ExplicitSmartCardDataPopulation;*/
        }

        private void MakeCypherData(MifareSectorData mifareSectorData)
        {
            if (_sectorManager == null || mifareSectorData == null)
                return;

            Dictionary<byte, BankInfo> banksInfo = _sectorManager.GetBanksInfo();

            ByteDataCarrier dataCS = new ByteDataCarrier(6, true);
            dataCS[0] = _editingObject.CardSystemNumber;

            if (_eAid.Text.Length >= 4)
            {
                byte[] AID = new byte[2];
                string ps = string.Empty;
                ps = _eAid.Text.Substring(0, 2);
                AID[0] = Convert.ToByte(ps, 16);
                ps = _eAid.Text.Substring(2, 2);
                AID[1] = Convert.ToByte(ps, 16);
                dataCS[0] = 0;
                dataCS[1] = AID[1];
                dataCS[2] = AID[0];
            }
            else
            {
                dataCS[1] = 0;
                dataCS[2] = 0;
            }
            dataCS[3] = banksInfo.ContainsKey(1) ? banksInfo[1].Length : (byte)0;
            dataCS[4] = banksInfo.ContainsKey(2) ? banksInfo[2].Length : (byte)0;
            dataCS[5] = banksInfo.ContainsKey(3) ? banksInfo[3].Length : (byte)0;

            byte[] plainData = new byte[19];
            plainData[0] = banksInfo.ContainsKey(1) ? banksInfo[1].SectorNumber : (byte)0;
            plainData[1] = 0;
            plainData[2] = banksInfo.ContainsKey(1) ? banksInfo[1].Offset : (byte)0;
            plainData[3] = 0;
            plainData[4] = banksInfo.ContainsKey(2) ? banksInfo[2].SectorNumber : (byte)0;
            plainData[5] = 0;
            plainData[6] = banksInfo.ContainsKey(2) ? banksInfo[2].Offset : (byte)0;
            plainData[7] = 0;
            plainData[8] = banksInfo.ContainsKey(3) ? banksInfo[3].SectorNumber : (byte)0;
            plainData[9] = 0;
            plainData[10] = banksInfo.ContainsKey(3) ? banksInfo[3].Offset : (byte)0;
            plainData[11] = 0;

            byte[] akey = GetXteaDecryptedValue(mifareSectorData.GeneralAKey);

            if (akey != null && akey.Length == 6)
            {
                plainData[12] = 0;
                plainData[13] = akey[0];
                plainData[14] = akey[1];
                plainData[15] = akey[2];
                plainData[16] = akey[3];
                plainData[17] = akey[4];
                plainData[18] = akey[5];
            }
            else
            {
                byte[] bkey = GetXteaDecryptedValue(mifareSectorData.GeneralBKey);
                if (bkey != null && bkey.Length == 6)
                {
                    plainData[12] = 1;
                    plainData[13] = bkey[0];
                    plainData[14] = bkey[1];
                    plainData[15] = bkey[2];
                    plainData[16] = bkey[3];
                    plainData[17] = bkey[4];
                    plainData[18] = bkey[5];
                }
            }


            XTEA xtea = new XTEA();
            xtea.XTeaInit();
            byte[] cypher = xtea.XTeaFrameEnc(plainData);
            dataCS.Append(cypher, true);
            mifareSectorData.CypherData = dataCS.ToByteArray();
        }

        private bool ShowTabSmartCardData()
        {
            if (_cbCardType.SelectedItem is CardTypes &&
                 (_cbCardType.SelectedItem as CardTypes).Value == CardType.MifarePlus)
            {
                if (!_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                    _tcCardSystem.TabPages.Insert(1, _tpSmartCardData);

                AllowAid();

                return true;
            }
            else if (_cbCardType.SelectedItem is CardTypes &&
                 (_cbCardType.SelectedItem as CardTypes).Value == CardType.Mifare)
            {
                if (_cbCardSubType.SelectedItem is CardSubTypes &&
                  ((_cbCardSubType.SelectedItem as CardSubTypes).Value != CardSubType.MifareSerialNumber &&
                   (_cbCardSubType.SelectedItem as CardSubTypes).Value != CardSubType.MifareSerialNumberWithoutPrefix))
                {
                    if (!_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                        _tcCardSystem.TabPages.Insert(1, _tpSmartCardData);

                    AllowAid();

                    return true;
                }
            }

            if (_tcCardSystem.TabPages.Contains(_tpSmartCardData))
                _tcCardSystem.TabPages.Remove(_tpSmartCardData);

            return false;
        }

        private void AllowAid()
        {
            if (_cbCardSubType.SelectedItem is CardSubTypes &&
              (_cbCardSubType.SelectedItem as CardSubTypes).Value.ToString().ToUpper().IndexOf("WITHMAD") > 0)
            {
                _lAid.Visible = true;
                _eAid.Visible = true;
            }
            else
            {
                _lAid.Visible = false;
                _eAid.Visible = false;
            }
        }

        private bool IsMifareSmartCardDataUsed()
        {
            if (_editingObject == null) return false;
            if (_editingObject.CardType == (byte)CardType.MifarePlus)
            {
                return true;
            }
            else if (_editingObject.CardType == (byte)CardType.Mifare &&
                (_editingObject.CardSubType != (byte)CardSubType.MifareSerialNumber &&
                 _editingObject.CardSubType != (byte)CardSubType.MifareSerialNumberWithoutPrefix))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.CardSystems.
                GetReferencedObjects(_editingObject.IdCardSystem as object, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void AidKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                if (e.KeyChar < (char)Keys.A || e.KeyChar > (char)Keys.F)
                    e.Handled = true;
            }
        }

        private void _eAid_TextChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (_eAid.Text.Length > 4)
            {
                _eAid.Text = _eAid.Text.Substring(0, 4);
                _eAid.SelectionStart = _eAid.Text.Length;
                _eAid.SelectionLength = 0;
            }
        }

        private void _cbCardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCardSubTypes();
            ShowTabSmartCardData();
            SetControlsForCardType();
        }

        private void SetControlsForCardType()
        {
            if (_cbCardType.SelectedItem is CardTypes &&
                (_cbCardType.SelectedItem as CardTypes).Value == CardType.Magnetic)
            {
                if (_cbCardSubType.Items.Count > 0)
                    _cbCardSubType.SelectedItem = _cbCardSubType.Items[0];
                _cbCardSubType.Enabled = false;
            }
            else
                _cbCardSubType.Enabled = true;
        }

        private void _cbCardSubType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShowTabSmartCardData())
                LoadMifareSmartCardData();

            CardSubTypes subType = GetActCardSubType();
            int prefixLength = subType == null ? 0 : subType.Number.Length;
            _eCompanyCode.MaxLength = (_cbCardType.SelectedItem as CardTypes).CardDataLengthMax - (prefixLength + 1);
        }

        private void _nudCardNumberFrom_ValueChanged(object sender, EventArgs e)
        {
            if (_nudCardNumberFrom.Value >= _nudCardNumberTo.Value)
            {
                decimal newValue = _nudCardNumberTo.Value - 1;
                if (newValue < 0) newValue = 0;
                _nudCardNumberFrom.Value = newValue;
            }
        }

        private void _nudCardNumberTo_ValueChanged(object sender, EventArgs e)
        {
            if (_nudCardNumberTo.Value <= _nudCardNumberFrom.Value)
            {
                decimal newValue = _nudCardNumberTo.Value - 1;
                if (newValue < 0) newValue = 0;
                _nudCardNumberFrom.Value = newValue;
            }
        }

        private int _cardsToGenerate = 0;
        private void _bGenerate_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (ValueChanged)
            {
                Contal.IwQuick.UI.Dialog.Info(GetString("InfoInEditedCsGenerationCanceled"));
                return;
            }

            var selectStructuredSubSiteForm = new SelectStructuredSubSiteForm();

            ICollection<int> idSelectedSubSites;

            if (!selectStructuredSubSiteForm.SelectStructuredSubSites(
                false,
                out idSelectedSubSites))
            {
                return;
            }

            var idSelectedSubSite = idSelectedSubSites != null
                ? (int?)idSelectedSubSites.First()
                : null;

            string fullPrefix = string.Empty;
            int numberLength = _editingObject.LengthCardData - _editingObject.GetFullCompanyCode().Length;
            try
            {
                fullPrefix = GetActCardSubType().Number + _editingObject.CompanyCode;
            }
            catch
            {
                fullPrefix = string.Empty;
            }
            _cardsToGenerate = (int)(_nudCardNumberTo.Value - _nudCardNumberFrom.Value + 1);

            if (!CgpClient.Singleton.MainServerProvider.GenerateCards(
                fullPrefix,
                _editingObject.CardSystemNumber,
                numberLength,
                _nudCardNumberFrom.Value,
                _nudCardNumberTo.Value,
                "1111",
                CardState.Unused,
                idSelectedSubSite != -1
                    ? idSelectedSubSite
                    : null))
            {
                _tbGenerationResult.Text += GetString("CardsAlreadyGenerating") + Environment.NewLine;
            }
            else
            {
                _stopAutoProgress = false;
                SafeThread.StartThread(ActivateCardGenerationProgressBar);
            }
        }

        private volatile bool _stopAutoProgress = false;
        private void ActivateCardGenerationProgressBar()
        {
            while (!_stopAutoProgress)
            {
                SetProgressValue((_pbCardGeneration.Value + 1) % _pbCardGeneration.Maximum);
                Thread.Sleep(10);
            }
        }

        private void SetProgressValue(int value)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetProgressValue), value);
            else
            {
                _pbCardGeneration.Value = value;
            }
        }

        private void _tpProperties_Click(object sender, EventArgs e)
        {

        }

        private bool _firstEnter = true;
        SectorManager _sectorManager = null;
        MifareSectorData _mifareCardData = null;

        private bool _mifareSmartDataBound = false;
        private void LoadMifareSmartCardData()
        {
            if (Insert || _editingObject.CardData == null)
                _mifareCardData = CreateDefaultMifareSectorData();
            else
                _mifareCardData = _editingObject.CardData as MifareSectorData;

            _sectorManager = new SectorManager(
                _mifareCardData.SizeKB,
                (byte)(_editingObject.LengthCardData - 2),
                (EncodingType)_mifareCardData.Encoding);

            if (!_mifareSmartDataBound)
            {
                _mifareSmartDataBound = true;
                BindMifareSectorDataProperties(_mifareCardData);
            }

            SetAndShowMifareSectorSettings(_sectorManager);
        }

        private void SetAndShowMifareSectorSettings(SectorManager sectorManager)
        {
            if (sectorManager == null)
                return;

            if (_dgvSectors.DataSource == null)
            {
                _dgvSectors.AutoGenerateColumns = true;
                _dgvSectors.DataSource = _sectorManager.GetSectorsBindingSource();
                ModifyColumnsForSectorsDGV();
                _dgvSectors.AutoGenerateColumns = false;
            }
            else
                _dgvSectors.DataSource = _sectorManager.GetSectorsBindingSource();

            SetValuesFromEditingObject();
            if (_dgvSectors.Columns.Contains("SectorNumber"))
                _dgvSectors.Sort(_dgvSectors.Columns["SectorNumber"], ListSortDirection.Ascending);
        }

        private void ModifyColumnsForSectorsDGV()
        {
            _dgvSectors.Columns.Remove("AKey");
            _dgvSectors.Columns.Remove("BKey");
            _dgvSectors.Columns.Remove("Offset");
            _dgvSectors.Columns.Remove("Bank");
            _dgvSectors.Columns.Remove("Length");
            _dgvSectors.Columns["Reserved"].ReadOnly = true;
            _dgvSectors.Columns["Reserved"].MinimumWidth = 100;
            _dgvSectors.Columns["Reserved"].DefaultCellStyle.BackColor = Color.LightGray;
            _dgvSectors.Columns["SectorNumber"].ReadOnly = true;
            _dgvSectors.Columns["SectorNumber"].DefaultCellStyle.BackColor = Color.LightGray;

            //length combobox column
            DataGridViewComboBoxColumn lengthColumn = new DataGridViewComboBoxColumn();
            lengthColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            lengthColumn.DataPropertyName = "Length";
            lengthColumn.DisplayMember = "Display";
            lengthColumn.ValueMember = "Value";
            lengthColumn.Name = "Length";

            List<ComboBoxByteNullableItem> lengthSource = new List<ComboBoxByteNullableItem>();
            lengthSource.Add(new ComboBoxByteNullableItem(null));
            for (int i = 1; i <= 255; i++)
            {
                lengthSource.Add(new ComboBoxByteNullableItem((byte)i));
            }
            lengthColumn.DataSource = lengthSource;
            _dgvSectors.Columns.Insert(_dgvSectors.Columns["Reserved"].Index, lengthColumn);

            //offset combobox column
            DataGridViewComboBoxColumn offsetColumn = new DataGridViewComboBoxColumn();
            offsetColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            offsetColumn.DataPropertyName = "Offset";
            offsetColumn.DisplayMember = "Display";
            offsetColumn.ValueMember = "Value";
            offsetColumn.Name = "Offset";

            List<ComboBoxByteNullableItem> offsetSource = new List<ComboBoxByteNullableItem>();
            offsetSource.Add(new ComboBoxByteNullableItem(null));
            for (int i = 0; i <= 255; i++)
            {
                offsetSource.Add(new ComboBoxByteNullableItem((byte)i));
            }
            offsetColumn.DataSource = offsetSource;
            _dgvSectors.Columns.Insert(_dgvSectors.Columns["Length"].Index, offsetColumn);

            //bank combobox column
            DataGridViewComboBoxColumn bankColumn = new DataGridViewComboBoxColumn();
            bankColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            bankColumn.DataPropertyName = "Bank";
            bankColumn.DisplayMember = "Display";
            bankColumn.ValueMember = "Value";
            bankColumn.Name = "Bank";

            List<ComboBoxByteNullableItem> bankSource = new List<ComboBoxByteNullableItem>() { new ComboBoxByteNullableItem(null), new ComboBoxByteNullableItem(1), 
                new ComboBoxByteNullableItem(2), new ComboBoxByteNullableItem(3) };
            bankColumn.DataSource = bankSource;
            _dgvSectors.Columns.Insert(_dgvSectors.Columns["AKeyText"].Index, bankColumn);

            _dgvSectors.Columns[_dgvSectors.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            foreach (DataGridViewColumn column in _dgvSectors.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvSectors);
        }

        private MifareSectorData CreateDefaultMifareSectorData()
        {
            MifareSectorData result = new MifareSectorData();
            result.SizeKB = (byte)CardSize.OneKB;
            result.Encoding = (byte)EncodingType.Ascii;

            return result;
        }

        private void BindMifareSectorDataProperties(AOrmObject cardData)
        {
            if (cardData == null)
                cardData = _editingObject.CardData;

            if (cardData is MifareSectorData)
            {
                MifareSectorData mifareSectorData = cardData as MifareSectorData;
                _cbSize.DataSource = new byte[] { 1, 4 };
                _cbSize.DataBindings.Add(AsyncBindingHelper.GetBinding(_cbSize, "SelectedItem", mifareSectorData, "SizeKB"));
                _eAid.DataBindings.Add(AsyncBindingHelper.GetBinding(_eAid, "Text", mifareSectorData, "Aid"));
                _eAkey.DataBindings.Add(AsyncBindingHelper.GetBinding(_eAkey, "Text", mifareSectorData, "GeneralAKeyString"));
                _eBkey.DataBindings.Add(AsyncBindingHelper.GetBinding(_eBkey, "Text", mifareSectorData, "GeneralBKeyString"));

                List<EncodingCBObject> source = new List<EncodingCBObject>() { new EncodingCBObject(EncodingType.Ascii), new EncodingCBObject(EncodingType.Bcd) };
                _cbEncoding.DataSource = source;
                _cbEncoding.DisplayMember = "Display";
                _cbEncoding.ValueMember = "Value";
                _cbEncoding.DataBindings.Add(AsyncBindingHelper.GetBinding(_cbEncoding, "SelectedValue", mifareSectorData, "Encoding"));
            }

            _cbExplicitSmartCardDataPopulation.DataBindings.Add(AsyncBindingHelper.GetBinding(_cbExplicitSmartCardDataPopulation, "Checked", _editingObject, "ExplicitSmartCardDataPopulation"));
        }

        private void SetValuesFromEditingObject()
        {
            if (_editingObject == null || _editingObject.CardData == null)
                return;

            if (!(_editingObject.CardData is MifareSectorData))
                return;

            MifareSectorData sectorData = _editingObject.CardData as MifareSectorData;
            foreach (MifareSectorSectorInfo info in sectorData.SectorsInfo)
            {
                if (_sectorManager.SectorSettings.ContainsKey((byte)info.SectorNumber))
                {
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].SectorNumber = (byte)info.SectorNumber;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].AKey = info.AKey;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].BKey = info.BKey;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].Bank = info.Bank;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].InheritAKey = info.InheritAKey;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].InheritBKey = info.InheritBKey;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].Offset = info.Offset;
                    _sectorManager.SectorSettings[(byte)info.SectorNumber].Length = info.Length;
                }
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CardSystemsLocalAlarmInstructionView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CardSystemsLocalAlarmInstructionAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        /// <summary>
        /// Ensures, this will not be anabled in some cases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cbCardSubType_EnabledChanged(object sender, EventArgs e)
        {
            if (_cbCardSubType.Enabled)
            {
                if (_cbCardType.SelectedItem is CardTypes &&
                    (_cbCardType.SelectedItem as CardTypes).Value == CardType.Magnetic)
                    _cbCardSubType.Enabled = false;
            }

        }

        private void EncodingTypeChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }


        private class MifareSectorSetting
        {
            public MifareSectorSetting(SectorManager sectorManager)
            {
                _xtea = new XTEA();
                _xtea.XTeaInit();
                _sectorManager = sectorManager;
            }

            public const string PASSWORD_TEXT = "●●●●●●●●●●●●";

            private SectorManager _sectorManager = null;

            private XTEA _xtea;

            public byte SectorNumber { get; set; }

            private bool _inheritAKey = false;
            public bool InheritAKey
            {
                get
                {
                    return _inheritAKey;
                }
                set
                {
                    if (_inheritAKey == value)
                        return;

                    if (value == false && _bank != null)
                        return;

                    _inheritAKey = value;
                }
            }

            private bool _inheritBKey = false;
            public bool InheritBKey
            {
                get
                {
                    return _inheritBKey;
                }
                set
                {
                    _inheritBKey = value;
                }
            }

            private byte[] _aKey;
            public byte[] AKey
            {
                get
                {
                    return _aKey;
                }
                set
                {
                    _aKey = value;

                    if (value == null || value.Length == 0)
                        _aKeyText = string.Empty;
                    else
                    {
                        _aKeyText = PASSWORD_TEXT;
                    }
                }
            }

            private string _aKeyText;

            public string AKeyText
            {
                get
                {
                    return _aKeyText;
                }
                set
                {
                    if (value == PASSWORD_TEXT)
                        return;
                    try
                    {
                        int dis = 0;
                        byte[] data = _xtea.XTeaFrameEnc(HexEncoding.GetBytes(value, out dis));
                        if (dis > 0)
                            throw new Exception();
                        _aKeyText = PASSWORD_TEXT;
                        _aKey = data;
                    }
                    catch (Exception)
                    {
                        _aKeyText = string.Empty;
                        _aKey = null;
                    }
                }
            }

            private byte[] _bKey;
            public byte[] BKey
            {
                get
                {
                    return _bKey;
                }
                set
                {
                    _bKey = value;

                    if (value == null || value.Length == 0)
                        _bKeyText = string.Empty;
                    else
                        _bKeyText = PASSWORD_TEXT;
                }
            }

            private string _bKeyText;
            public string BKeyText
            {
                get
                {
                    return _bKeyText;
                }
                set
                {
                    if (value == PASSWORD_TEXT)
                        return;
                    try
                    {
                        int dis = 0;
                        byte[] data = _xtea.XTeaFrameEnc(HexEncoding.GetBytes(value, out dis));
                        if (dis > 0)
                            throw new Exception();
                        _bKey = data;
                        _bKeyText = PASSWORD_TEXT;
                    }
                    catch (Exception)
                    {
                        _bKeyText = string.Empty;
                        _bKey = null;
                    }
                }
            }

            private byte? _bank = null;
            public byte? Bank
            {
                get
                {
                    return _bank;
                }
                set
                {
                    if (_bank == value)
                        return;

                    byte? sectorWithSameBank = null;
                    if (_sectorManager.BankAlreadyUsed(value, out sectorWithSameBank))
                    {
                        if (!Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionBankUsed", sectorWithSameBank)))
                            return;

                        MifareSectorSetting settingWithSameBank = _sectorManager.UsedBanks[value.Value];

                        if (_bank != null && Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionSwitchBanks", SectorNumber, sectorWithSameBank)))
                        {
                            settingWithSameBank._bank = _bank;
                            if (_bank != null)
                                _sectorManager.UsedBanks[_bank.Value] = settingWithSameBank;
                        }
                        else
                        {
                            settingWithSameBank.Bank = null;
                            //ensure previous bank to became available
                            if (_bank != null)
                                _sectorManager.UsedBanks.Remove(_bank.Value);
                        }

                        //updating UsedBanks with actual MifareSectorSetting (this)
                        _sectorManager.UsedBanks[value.Value] = this;
                    }
                    else
                    {
                        if (value == null)
                        {
                            _sectorManager.UsedBanks.Remove(_bank.Value);
                            Offset = null;
                            Length = null;
                        }
                        else
                            _sectorManager.UsedBanks[value.Value] = this;
                    }

                    //In Nova system, bank sectors must inherit A key
                    if (value != null)
                        InheritAKey = true;

                    _bank = value;
                }
            }

            private byte? _offset = null;
            public byte? Offset
            {
                get { return _offset; }
                set
                {
                    //setting offset to bigger value may affest available length (sector overlapping)
                    if (_offset != null && value != null && _offset < value)
                        _sectorManager.ProcessOffsetGrowth(SectorNumber, value, _length);
                    _offset = value;
                }
            }

            private byte? _length = null;
            public byte? Length
            {
                get { return _length; }
                set
                {
                    _length = value;
                    if (value != null && _offset == null)
                        Offset = _sectorManager.GetFirstAvailableOffset(SectorNumber);
                }
            }

            public bool Reserved
            {
                get
                {
                    return (_bKey != null || InheritBKey);
                }
            }
        }

        private class BankInfo
        {
            private byte _bankNumber = 0;
            public byte BankNumber
            {
                get { return _bankNumber; }
                set { _bankNumber = value; }
            }

            private byte _sectorNumber = 0;
            public byte SectorNumber
            {
                get { return _sectorNumber; }
                set { _sectorNumber = value; }
            }

            private byte _offset = 0;
            public byte Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }

            private byte _length = 0;
            public byte Length
            {
                get { return _length; }
                set { _length = value; }
            }
        }

        private class SectorManager
        {
            public SectorManager(byte cardSizeKB, byte cardNumberLength, EncodingType encoding)
            {
                _cardSizeKB = cardSizeKB;
                _cardNumberDigitsStoreLength = cardNumberLength;
            }

            private BindingSource _binding = null;

            private Dictionary<byte, MifareSectorSetting> _sectorSettings = null;
            public Dictionary<byte, MifareSectorSetting> SectorSettings
            {
                get { return _sectorSettings; }
                set { _sectorSettings = value; }
            }

            public MifareSectorSetting CurrentMifareSectorSetting
            {
                get
                {
                    if (_binding == null)
                        return null;

                    return _binding.Current as MifareSectorSetting;
                }
            }

            private EncodingType _encodingType = EncodingType.Ascii;
            public EncodingType EncodingType
            {
                get { return _encodingType; }
                set
                {
                    if (_encodingType == value)
                        return;

                    ProcessEncodingChange(_encodingType, value);

                    _encodingType = value;
                    //processing change
                }
            }

            public Dictionary<byte, BankInfo> GetBanksInfo()
            {
                Dictionary<byte, BankInfo> result = new Dictionary<byte, BankInfo>();
                if (_sectorSettings == null || _sectorSettings.Count == 0)
                    return result;

                foreach (MifareSectorSetting setting in _sectorSettings.Values)
                {
                    if (setting.Bank != null)
                        result.Add(setting.Bank.Value, new BankInfo()
                        {
                            BankNumber = setting.Bank.Value,
                            Length = setting.Length ?? 0,
                            Offset = setting.Offset ?? 0,
                            SectorNumber = setting.SectorNumber
                        });
                }

                return result;
            }

            private void ProcessEncodingChange(EncodingType previousEncoding, EncodingType currentEncoding)
            {
                if (previousEncoding == EncodingType.Bcd && currentEncoding == EncodingType.Ascii)
                {
                    foreach (MifareSectorSetting setting in _binding)
                    {
                        if (setting.Length > 0)
                        {
                            byte maxAvailableUsingOffsetCondition = GetMaxAvailableDigitsOffsetCondition(setting.SectorNumber, setting.Offset, currentEncoding);

                            if (setting.Length > maxAvailableUsingOffsetCondition)
                                setting.Length = maxAvailableUsingOffsetCondition;
                        }
                    }
                }
            }

            private byte _cardSizeKB = 0;
            public byte CardSizeKB
            {
                get { return _cardSizeKB; }
                set { _cardSizeKB = value; }
            }

            private byte _cardNumberDigitsStoreLength = 0;
            public byte CardNumberDigitsStoreLength
            {
                get { return _cardNumberDigitsStoreLength; }
                set
                {
                    if (value < _cardNumberDigitsStoreLength)
                        ProcessCardNumberLengthDecrease(value);

                    _cardNumberDigitsStoreLength = value;
                }
            }

            private void ProcessCardNumberLengthDecrease(byte newLength)
            {
                //increasing data length is safe
                byte actualLength = GetActualCardNumberLength();
                if (newLength >= actualLength)
                    return;

                for (int i = 0; i < _binding.Count; i++)
                {
                    if ((_binding[i] as MifareSectorSetting).Length > 0)
                    {
                        while (actualLength > newLength && (_binding[i] as MifareSectorSetting).Length > 0)
                        {
                            if ((_binding[i] as MifareSectorSetting).Length == 1)
                                (_binding[i] as MifareSectorSetting).Length = null;
                            else
                                (_binding[i] as MifareSectorSetting).Length--;
                            actualLength--;
                        }
                    }
                }
            }

            private Dictionary<byte, MifareSectorSetting> _usedBanks = new Dictionary<byte, MifareSectorSetting>();
            public Dictionary<byte, MifareSectorSetting> UsedBanks
            {
                get { return _usedBanks; }
                set { _usedBanks = value; }
            }

            public bool BankAlreadyUsed(byte? bank, out byte? bySectorNumber)
            {
                bySectorNumber = null;
                if (bank == null)
                    return false;

                bool result = _usedBanks.ContainsKey(bank.Value);
                if (result)
                    bySectorNumber = _usedBanks[bank.Value].SectorNumber;

                return result;
            }

            public bool IsSwitchingRequired(byte? fromBank, byte? toBank, out MifareSectorSetting settingToSwitchWith)
            {
                settingToSwitchWith = null;
                if (toBank == null)
                    return false;

                if (fromBank == null)
                    return false;

                bool result = Dialog.Question("Do you want to switch these values?");
                if (result)
                    _usedBanks.TryGetValue(toBank.Value, out settingToSwitchWith);

                return result;
            }

            public List<ComboBoxByteNullableItem> GetAvailableOffsets(byte sectorNumber)
            {
                List<ComboBoxByteNullableItem> result = new List<ComboBoxByteNullableItem>() { new ComboBoxByteNullableItem(null) };

                if (_cardSizeKB == 1)
                {
                    for (byte i = 0; i < 48; i++)
                    {
                        result.Add(new ComboBoxByteNullableItem(i));
                    }
                    if (sectorNumber == 0)
                        //element at position 0 is null element, therefore removing from position 1
                        result.RemoveRange(1, 16);
                }
                else if (_cardSizeKB == 4)
                {
                    byte offsetStart = 0;
                    byte offsetEnd = 0;
                    if (sectorNumber == 0)
                    {
                        offsetStart = 16;
                        offsetEnd = 47;
                    }
                    else if (sectorNumber > 0 && sectorNumber < 32)
                    {
                        offsetStart = 0;
                        offsetEnd = 47;
                    }
                    else if (sectorNumber >= 32)
                    {
                        offsetStart = 0;
                        offsetEnd = 223;
                    }
                    for (byte i = offsetStart; i <= offsetEnd; i++)
                    {
                        result.Add(new ComboBoxByteNullableItem(i));
                    }
                }

                return result;
            }

            /// <summary>
            /// Returns max available card number length (digits) according to Offset and Sum condition
            /// </summary>
            /// <param name="sectorNumber">number of sector to get result for</param>
            /// <param name="actualOffset">actual offset of card data for that sector</param>
            /// <returns>max available card number length (digits)</returns>
            public byte GetAvailableCardNumberDigitsLength(byte sectorNumber, byte? actualOffset)
            {
                byte result = GetMaxAvailableDigitsOffsetCondition(sectorNumber, actualOffset, _encodingType);

                byte maxAvailableOnSumCondition = GetMaxAvailableDigitsSumCondition(sectorNumber);
                if (maxAvailableOnSumCondition < result)
                    result = maxAvailableOnSumCondition;

                return result;
            }

            private byte GetMaxAvailableDigitsSumCondition(byte sectorNumber)
            {
                byte lengthUsedByOtherSectors = GetSumDigitsOtherSectors(sectorNumber);
                //sum condition
                return (byte)(_cardNumberDigitsStoreLength - lengthUsedByOtherSectors);
            }

            private byte GetMaxAvailableDigitsOffsetCondition(byte sectorNumber, byte? offset, EncodingType encodingType)
            {
                if (offset == null)
                    offset = 0;

                byte result = _cardNumberDigitsStoreLength;
                byte lastPossibleDataEnd = 0;
                if (_cardSizeKB == 1)
                {
                    lastPossibleDataEnd = 48;
                }
                else if (_cardSizeKB == 4)
                {
                    if (sectorNumber < 32)
                        lastPossibleDataEnd = 48;
                    else
                        lastPossibleDataEnd = 224;
                }

                //offset condition
                if ((lastPossibleDataEnd - offset) < result)
                    result = (byte)(lastPossibleDataEnd - offset);

                //when BCD encoding, each byte can store 2 digits
                if (encodingType == EncodingType.Bcd)
                    result = (byte)(result * 2);

                return result;
            }

            private byte GetSumDigitsOtherSectors(byte sectorToAvoid)
            {
                byte result = 0;
                foreach (MifareSectorSetting setting in _binding)
                {
                    if (setting.SectorNumber == sectorToAvoid)
                        continue;

                    if (setting.Length != null)
                        result += setting.Length.Value;
                }

                return result;
            }

            public byte GetActualCardNumberLength()
            {
                byte result = 0;
                foreach (MifareSectorSetting setting in _binding)
                {
                    if (setting.Length != null)
                        result += setting.Length.Value;
                }

                return result;
            }

            public BindingSource GetSectorsBindingSource()
            {
                if (_binding == null)
                {
                    _sectorSettings = new Dictionary<byte, MifareSectorSetting>();
                    byte sectors = 0;
                    if (_cardSizeKB == 1)
                        sectors = 16;
                    else if (_cardSizeKB == 4)
                        sectors = 40;

                    for (byte i = 0; i < sectors; i++)
                    {
                        _sectorSettings.Add(i, new MifareSectorSetting(this)
                        {
                            SectorNumber = i,
                        });
                    }
                    _binding = new BindingSource();
                    _binding.DataSource = new SortableBindingList<MifareSectorSetting>(_sectorSettings.Values);
                }

                return _binding;
            }

            public void ProcessOffsetGrowth(byte sectorNumber, byte? newOffsetValue, byte? length)
            {
                if (newOffsetValue == null || length == null)
                    return;

                byte maxDigitsOffsetCondition = GetMaxAvailableDigitsOffsetCondition(sectorNumber, newOffsetValue, _encodingType);
                if (length > maxDigitsOffsetCondition)
                    CurrentMifareSectorSetting.Length = maxDigitsOffsetCondition;

                //byte lastPossibleDataEnd = 0;
                //if (_cardSizeKB == 1)
                //{
                //    lastPossibleDataEnd = 48;
                //}
                //else if (_cardSizeKB == 4)
                //{
                //    if (sectorNumber < 32)
                //        lastPossibleDataEnd = 48;
                //    else
                //        lastPossibleDataEnd = 224;
                //}

                //int dataEnd = newOffsetValue.Value + length.Value;
                //if (dataEnd > lastPossibleDataEnd)
                //{
                //    byte newLength = (byte)(length.Value - (dataEnd - lastPossibleDataEnd));
                //    CurrentMifareSectorSetting.Length = newLength;
                //}

            }

            public byte GetFirstAvailableOffset(byte sectorNumber)
            {
                if (_cardSizeKB == 1 || _cardSizeKB == 4)
                {
                    if (sectorNumber == 0)
                        return 16;
                    else
                        return 0;
                }
                return 0;
            }

            public bool IsSumBankLengthValid()
            {
                byte sumLengthBanks = 0;
                foreach (MifareSectorSetting setting in _sectorSettings.Values)
                {
                    if (setting.Length != null)
                        sumLengthBanks += (byte)setting.Length;
                }

                return _cardNumberDigitsStoreLength == sumLengthBanks;
            }
        }

        public class ComboBoxByteNullableItem
        {
            public ComboBoxByteNullableItem(byte? value)
            {
                _value = value;
            }

            protected byte? _value = null;
            public byte? Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }

            protected string _display = string.Empty;
            public string Display
            {
                get
                {
                    if (_value == null)
                        return string.Empty;
                    else
                        return _value.ToString();
                }
                set
                {
                    _display = value;
                }
            }
        }

        private void _dgvSectors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            if (_sectorManager.CurrentMifareSectorSetting.Bank == null && (e.ColumnIndex == _dgvSectors.Columns["Offset"].Index || e.ColumnIndex == _dgvSectors.Columns["Length"].Index))
                return;

            if (_sectorManager.CurrentMifareSectorSetting.Bank != null && (e.ColumnIndex == _dgvSectors.Columns["InheritAKey"].Index))
            {
                _dgvSectors.EndEdit();
                return;
            }

            if (!_dgvSectors[e.ColumnIndex, e.RowIndex].IsInEditMode)
                _dgvSectors.BeginEdit(false);
        }

        private void _dgvSectors_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_sectorManager.CurrentMifareSectorSetting.Bank == null && (e.ColumnIndex == _dgvSectors.Columns["Offset"].Index || e.ColumnIndex == _dgvSectors.Columns["Length"].Index))
                return;

            _dgvSectors.BeginEdit(false);
        }

        private void _dgvSectors_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {
                if (_dgvSectors.CurrentCell.ColumnIndex == _dgvSectors.Columns["AKeyText"].Index ||
                    _dgvSectors.CurrentCell.ColumnIndex == _dgvSectors.Columns["BKeyText"].Index)
                {
                    (e.Control as TextBox).UseSystemPasswordChar = true;
                    (e.Control as TextBox).MaxLength = 12;
                    (e.Control as TextBox).KeyDown += new KeyEventHandler(delegate
                    {
                        if ((e.Control as TextBox).Text == PASSWORD_TEXT)
                            (e.Control as TextBox).Text = string.Empty;
                    });
                }
            }
            if (e.Control is ComboBox)
            {
                if (_dgvSectors.CurrentCell.ColumnIndex == _dgvSectors.Columns["Offset"].Index)
                {
                    try
                    {
                        byte? selectedValue = _sectorManager.CurrentMifareSectorSetting.Offset;

                        List<ComboBoxByteNullableItem> offsetsAvailable = _sectorManager.GetAvailableOffsets(_sectorManager.CurrentMifareSectorSetting.SectorNumber);
                        (e.Control as ComboBox).DataSource = offsetsAvailable;

                        ComboBoxByteNullableItem o = offsetsAvailable.First(item => item.Value == selectedValue);
                        if (o == null)
                            return;

                        (e.Control as ComboBox).SelectedItem = o;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (_dgvSectors.CurrentCell.ColumnIndex == _dgvSectors.Columns["Length"].Index)
                {
                    try
                    {
                        byte? selectedValue = _sectorManager.CurrentMifareSectorSetting.Length;
                        byte availableLength = _sectorManager.GetAvailableCardNumberDigitsLength(_sectorManager.CurrentMifareSectorSetting.SectorNumber, _sectorManager.CurrentMifareSectorSetting.Offset);

                        List<ComboBoxByteNullableItem> availableLengths = new List<ComboBoxByteNullableItem>();
                        availableLengths.Add(new ComboBoxByteNullableItem(null));
                        for (byte i = 1; i <= availableLength; i++)
                        {
                            availableLengths.Add(new ComboBoxByteNullableItem(i));
                        }

                        (e.Control as ComboBox).DataSource = availableLengths;

                        ComboBoxByteNullableItem l = availableLengths.First(item => item.Value == selectedValue);
                        if (l == null)
                            return;

                        (e.Control as ComboBox).SelectedItem = l;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void _dgvSectors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _dgvSectors.Refresh();
        }

        private void CardSystemsEditForm_Load(object sender, EventArgs e)
        {
            BindCardSystemProperties();
        }

        private void BindCardSystemProperties()
        {
            _tbCardDataLength.DataBindings.Add(AsyncBindingHelper.GetBinding(_tbCardDataLength, "Value", _editingObject, "LengthCardData", DataSourceUpdateMode.OnPropertyChanged));

            _cbCardType.DataSource = CardTypes.GetCardTypesList().Where(ct => ct.Value != CardType.DirectSerial).ToList();
            _cbCardType.ValueMember = "ValueByte";
            _cbCardType.DisplayMember = "Name";
            _cbCardType.DataBindings.Add(AsyncBindingHelper.GetBinding(_cbCardType, "SelectedValue", _editingObject, "CardType", DataSourceUpdateMode.OnPropertyChanged));

            _cbCardSubType.DataSource = CardSubTypes.GetCardSubTypesList((CardType)_editingObject.CardType);
            _cbCardSubType.DisplayMember = "Name";
            _cbCardSubType.ValueMember = "ValueByte";
            _cbCardSubType.DataBindings.Add(AsyncBindingHelper.GetBinding(_cbCardSubType, "SelectedValue", _editingObject, "CardSubType", DataSourceUpdateMode.OnPropertyChanged));

        }

        private void _cbSize_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!Dialog.Question(GetString("QuestionConfirmCardSizeChange")))
                _cbSize.SelectedItem = _mifareCardData.SizeKB;
            else
            {
                _mifareCardData.SizeKB = (byte)_cbSize.SelectedItem;
                ClearAndReloadSectorsSettings();
            }
        }

        private void ClearAndReloadSectorsSettings()
        {
            _sectorManager = new SectorManager(
                _mifareCardData.SizeKB,
                (byte)(_editingObject.LengthCardData - 2),
                (EncodingType)_mifareCardData.Encoding);
            SetAndShowMifareSectorSettings(_sectorManager);
        }

        private void _cbEncoding_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_sectorManager != null)
            {
                _sectorManager.EncodingType = (EncodingType)_cbEncoding.SelectedValue;
                _dgvSectors.Refresh();
            }
        }

        private void _tpSmartCardData_Enter(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }
    }

    class EncodingCBObject
    {
        EncodingType _encodingType;

        private byte _value = 0;
        public byte Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _display = null;
        public string Display
        {
            get
            {
                return _display;
            }
            set
            {
                _display = value;
            }
        }

        public EncodingType EncodingType
        {
            get { return _encodingType; }
        }

        public EncodingCBObject(EncodingType encType)
        {
            _encodingType = encType;
            _value = (byte)encType;

            Type type = EncodingType.GetType();
            MemberInfo[] memInfo = type.GetMember(_encodingType.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(NameAttribute), false);
            _display = ((NameAttribute)attributes[0]).Name;
        }

        public override string ToString()
        {
            return _display;
        }
    }
}
