using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Parsing;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public partial class CardsEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Card>
#endif
    {
        private CardSystem _actCardSystem;
        private Person _actPerson;
        private bool _pinEntered;
        private Card _relatedCard;
        private bool _relatedCardChanged;
        private Card _alternateCard;
        
        private static string GetPersonDisplayText(Person person)
        {
            if (person == null)
                return string.Empty;

            return string.Format("{0} {1} - {2}",
                person.Surname,
                person.FirstName,
                person.Identification);
        }

        public CardsEditForm(Card card, ShowOptionsEditForm showOption)
            : base(card, showOption)
        {
            InitializeComponent();

            _editingObject = card;
            SetReferenceEditColors();
            WheelTabContorol = _tcCard;
            ControlAccessRights();

            InitCardStateImageList();

            _eDescription.MouseWheel += ControlMouseWheel;
            _icbCardState.MouseWheel += ControlMouseWheel;

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            if (!CgpClient.Singleton.MainServerProvider.CheckTimetecLicense())
                _tcCard.TabPages.Remove(_tpTimetecSettings);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageProperties(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsPropertiesView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsPropertiesAdmin)));

                DisablePin(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsPropertiesPinAdmin)));

                DisableCardState(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsPropertiesCardStateAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CardsDescriptionAdmin)));
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
                if (!view && !admin)
                {
                    _tcCard.TabPages.Remove(_tpProperties);
                    return;
                }

                _tpProperties.Enabled = admin;
            }
        }

        private void DisablePin(bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<bool>(DisablePin), 
                    admin);

                return;
            }

            _ePin.Enabled = admin; // && !_editingObject.SynchronizedWithTimetec;
        }

        private void DisableCardState(bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<bool>(DisableCardState),
                    admin);

                return;
            }

            _icbCardState.Enabled = admin && !_editingObject.SynchronizedWithTimetec;
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabFoldersSructure), view);
                return;
            }

            if (!view)
                _tcCard.TabPages.Remove(_tpUserFolders);
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<bool>(HideDisableTabReferencedBy), 
                    view);

                return;
            }

            if (!view)
                _tcCard.TabPages.Remove(_tpReferencedBy);
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDescription),
                    view,
                    admin);

                return;
            }

            if (!view && !admin)
            {
                _tcCard.TabPages.Remove(_tpDescription);
                return;
            }

            _tpDescription.Enabled = admin;
        }

        protected override void RegisterEvents()
        {

        }

        protected override void UnregisterEvents()
        {

        }

        private void InitCardStateImageList()
        {
            //represent pictures for card states
            var imagelist = new ImageList
            {
                ImageSize = new Size(
                    18,
                    18),
                ColorDepth = ColorDepth.Depth32Bit
            };

            imagelist.Images.Add(
                CardState.Active.ToString(), 
                ResourceGlobal.CardBlank128);

            imagelist.Images.Add(
                CardState.Blocked.ToString(), 
                ResourceGlobal.CardNewNO_128);

            imagelist.Images.Add(
                CardState.TemporarilyBlocked.ToString(),
                ResourceGlobal.CardNewNO_128);

            imagelist.Images.Add(
                CardState.Unused.ToString(),
                ResourceGlobal.CardNewUNKNOWN_128);

            imagelist.Images.Add(
                CardState.Lost.ToString(), 
                ResourceGlobal.CardNewNO_128);

            imagelist.Images.Add(
                CardState.Destroyed.ToString(), 
                ResourceGlobal.CardNewNO_128);

            imagelist.Images.Add(
                CardState.HybridActive.ToString(), 
                ResourceGlobal.HybridCardBlank128);

            imagelist.Images.Add(
                CardState.HybridBlocked.ToString(),
                ResourceGlobal.HybridCardNewNO_128);

            imagelist.Images.Add(
                CardState.HybridTemporarilyBlocked.ToString(),
                ResourceGlobal.HybridCardNewNO_128);

            imagelist.Images.Add(
                CardState.HybridUnused.ToString(),
                ResourceGlobal.HybridCardNewUNKNOWN_128);

            imagelist.Images.Add(
                CardState.HybridLost.ToString(),
                ResourceGlobal.HybridCardNewNO_128);

            imagelist.Images.Add(
                CardState.HybridDestroyed.ToString(), 
                ResourceGlobal.HybridCardNewNO_128);

            
            _icbCardState.ImageList = imagelist;
        }

        private void ControlAccessRights()
        {
            try
            {
                _tbmPerson.Enabled = PersonsForm.HasAccessUpdate();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
                Text = LocalizationHelper.GetString("CardsEditFormInsertText");

            _bCreateCard.Text = GetString(_alternateCard != null
                ? "General_bEdit"
                : "General_bCreate");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
            RefreshCardState(false);

            if (_editingObject.SynchronizedWithTimetec)
            {
                DisabledControls(this);

                EnabledControls(_bOk);
                EnabledControls(_bCancel);

                Text = string.Format("{0} [Timetec]", Text);
            }
        }

        protected override void BeforeInsert()
        {
            CardsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            CardsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;

            var card =
                CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEdit(
                    _editingObject.IdCard, 
                    out error);

            if (error != null)
            {
                allowEdit = false;

                if (error is AccessDeniedException)
                    card =
                        CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(
                            _editingObject.IdCard);
                else
                    throw error;

                DisabledForm();
            }
            else
                allowEdit = true;

            _editingObject = card;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;

            CgpClient.Singleton.MainServerProvider.Cards.RenewObjectForEdit(
                _editingObject.IdCard, 
                out error);

            if (error != null)
                throw error;
        }

        protected override void SetValuesInsert()
        {
            SetCardSystem(_editingObject.CardSystem);

            if (_editingObject.CardSystem != null && ShowOption != ShowOptionsEditForm.InsertWithData)
                _tbmCardSystem.Enabled = false;

            if (_editingObject.Person != null)
            {
                _actPerson = _editingObject.Person;

                _tbmPerson.Text = GetPersonDisplayText(_actPerson);
                _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                _tbmPerson.Enabled = false;
            }

            if (!String.IsNullOrEmpty(_editingObject.FullCardNumber))
            {
                CardSystem cardSystem = 
                    CgpClient.Singleton.MainServerProvider.CardSystems
                        .GetCardSystemByCard(_editingObject.FullCardNumber);

                if (cardSystem != null)
                {
                    _editingObject.CardSystem = cardSystem;

                    int lengthComCode = cardSystem.GetFullCompanyCode()
                        .Length;

                    _eCardNumber.Text =
                        _editingObject.FullCardNumber.Substring(
                            lengthComCode,
                            _editingObject.FullCardNumber.Length - lengthComCode);

                    SetCardSystem(_editingObject.CardSystem);
                }
                else
                    _eCardNumber.Text = _editingObject.FullCardNumber;
            }

            SetActualCardType();
            SetActualFullCardNumber();
            SetRelatedCard(true);

            _pinEntered = true;

            RefreshVisibleForAlternateCardNumber();
        }

        protected override void SetValuesEdit()
        {
            SetCardSystem(_editingObject.CardSystem);

            _eCardNumber.Text = _editingObject.Number;

            _actPerson = _editingObject.Person;

            if (_actPerson != null)
            {
                _tbmPerson.Text = GetPersonDisplayText(_actPerson);
                _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);
            }

            _eDateStateLastChange.Text = _editingObject.DateStateLastChange.ToString("dd.MM.yyyy HH:mm:ss");
            _eDescription.Text = _editingObject.Description;

            SetActualCardType();
            SetActualFullCardNumber();
            SetReferencedBy();

            SetRelatedCard(GetRelatedCardFromEditingObject(), true);

            if (string.IsNullOrEmpty(_editingObject.Pin))
                _pinEntered = true;
            else
            {
                _pinEntered = true;

                _ePin.Text = "6851";
                _ePin.PasswordChar = '●';

                _pinEntered = false;
            }

            _eAlternateCardNumber.Text = _editingObject.AlternateCardNumber;
            RefreshVisibleForAlternateCardNumber();

            _dpValidityDateFrom.Value = _editingObject.ValidityDateFrom;
            _dpValidityDateTo.Value = _editingObject.ValidityDateTo;
        }

        public override void SetValuesInsertFromObj(object obj)
        {
            if (obj == null)
                return;

            string cardNumber = string.Empty;
            Person person = null;
            if (obj is string)
            {
                cardNumber = obj as string;
            }
            else
            {
                var card = obj as Card;

                if (card != null)
                {
                    cardNumber = card.FullCardNumber;
                    person = card.Person;
                }
            }

            if (Validator.IsNotNullString(cardNumber))
            {
                CardSystem cardSystem = 
                    CgpClient.Singleton.MainServerProvider.CardSystems
                        .GetCardSystemByCard(cardNumber);

                if (cardSystem != null)
                {
                    _editingObject.CardSystem = cardSystem;

                    int lengthComCode = cardSystem.GetFullCompanyCode()
                        .Length;

                    _eCardNumber.Text =
                        cardNumber.Substring(
                            lengthComCode,
                            cardNumber.Length - lengthComCode);
                }
                else
                    _eCardNumber.Text = cardNumber;
            }

            if (person != null)
            {
                _actPerson = person;

                _tbmPerson.Text = GetPersonDisplayText(_actPerson);
                _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                _tbmPerson.Enabled = false;
            }

            RefreshCardState(true);
        }

        private void RefreshCardState(bool initial)
        {
            if (_actPerson == null)
                UnsetCardState();
            else
                ActiveCardState(initial);
        }

        private void UnsetCardState()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(UnsetCardState));
                return;
            }

            try
            {
                _icbCardState.SuspendLayout();
                _icbCardState.Items.Clear();

                var cardStates = CardStates.GetCardStatesList(LocalizationHelper);

                foreach (CardStates cardState in cardStates)
                    if (_relatedCard == null && cardState.Value == CardState.Unused
                        || _relatedCard != null && cardState.Value == CardState.HybridUnused)
                    {
                        _icbCardState.Items.Add(
                            new ImageComboBoxItem(
                                cardState,
                                cardState.Value.ToString()));
                    }

                _icbCardState.SelectedIndex = 0;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _icbCardState.ResumeLayout();
                _icbCardState.Enabled = false;
            }
        }

        private void ActiveCardState(bool initial)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(ActiveCardState));
                return;
            }

            try
            {
                _icbCardState.SuspendLayout();

                string imageKeyToBeSelected = null;

                if (!initial)
                    imageKeyToBeSelected = GetCardStateStringToBeSelected(_icbCardState.SelectedItem as ImageComboBoxItem);

                _icbCardState.Items.Clear();

                var cardStates = CardStates.GetCardStatesList(LocalizationHelper);

                foreach (CardStates cardState in cardStates)
                {
                    if (cardState.Value == CardState.HybridUnused)
                        continue;

                    if (cardState.Value == CardState.Unused)
                        continue;

                    if ((cardState.Value >= CardState.HybridActive)
                        == (_relatedCard == null))
                    {
                        continue;
                    }

                    if (_editingObject.State != (byte)CardState.TemporarilyBlocked
                        && _editingObject.State != (byte)CardState.HybridTemporarilyBlocked
                        && (cardState.Value == CardState.TemporarilyBlocked ||
                            cardState.Value == CardState.HybridTemporarilyBlocked))
                    {
                        continue;
                    }

                    var addItem = 
                        new ImageComboBoxItem(
                            cardState, 
                            cardState.Value.ToString());

                    _icbCardState.Items.Add(addItem);

                    if (addItem.ImageKey == imageKeyToBeSelected 
                        || initial && _editingObject.State == (byte)cardState.Value)
                    {
                        _icbCardState.SelectedItem = addItem;
                    }
                }

                if (_icbCardState.SelectedItem == null)
                    _icbCardState.SelectedItem = _icbCardState.Items[0];
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            finally
            {
                _icbCardState.ResumeLayout();
                _icbCardState.Enabled = !_editingObject.SynchronizedWithTimetec;
            }
        }

        private string GetCardStateStringToBeSelected(ImageComboBoxItem previouslySelectedItem)
        {
            if (previouslySelectedItem == null)
                return string.Empty;

            //no need to change if "hybrid" property is the same (can be get from value in state enum and _relatedCard property)
            //if state value or-ed with mask 0x10 results in same value, than it is hybrid state value
            //if state value and-en with mask 0x0f results in same value, than it is no hybrid state value

            var previousState = (CardState)Enum.Parse(typeof(CardState), previouslySelectedItem.ImageKey);

            if (TransformToNonHybridState(previousState) == previousState && _relatedCard == null
                || TransformToHybridState(previousState) == previousState && _relatedCard != null)
            {
                return previouslySelectedItem.ImageKey;
            }

            //was hybrid and now it is not

            return _relatedCard == null
                ? TransformToNonHybridState(previousState).ToString()
                : TransformToHybridState(previousState).ToString();

            //was not hybrid card but now it is
        }

        private static CardState TransformToHybridState(CardState state)
        {
            try
            {
                var stateByte = (byte)state;
                return (CardState)(byte)(stateByte | 0x10);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }

            return state;
        }

        private static CardState TransformToNonHybridState(CardState state)
        {
            try
            {
                return (CardState)(byte)((byte)state & 0x0f);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }

            return state;
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();
            _bCancel.Enabled = false;
        }

        private void SetActualCardType()
        {
            CardTypes cardType;

            if (_actCardSystem != null)
                cardType = CardTypes.GetCardType(_actCardSystem.CardType);
            else
            {
                cardType = CardTypes.GetCardType((byte)CardType.DirectSerial);
                _tbmCardSystem.Text = string.Empty;
            }

            if (cardType != null)
                _lActualCardType.Text = cardType.Name;
        }

        private void SetActualFullCardNumber()
        {
            _lActualFullCardNumber.Text =
                _editingObject.GetFullCardNumber(
                    _actCardSystem, 
                    _eCardNumber.Text);
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.CardSystem = _actCardSystem;
                _editingObject.Number = _eCardNumber.Text;

                SetActualFullCardNumber();

                _editingObject.FullCardNumber = _lActualFullCardNumber.Text;
                _editingObject.Name = _lActualFullCardNumber.Text;

                if (_pinEntered)
                {
                    if (_ePin.Text == String.Empty)
                    {
                        var cards = _actPerson.Cards as IList<Card>;
                        _editingObject.Pin = cards[0].Pin;
                    }
                    else
                        _editingObject.Pin = QuickHashes.GetCRC32String(_ePin.Text);

                    _editingObject.PinLength = (byte)_ePin.Text.Length;
                }

                _editingObject.Person = _actPerson;

                var cardStateAsByte =(byte)((CardStates)((ImageComboBoxItem)_icbCardState.SelectedItem).MyObject).Value;

                if (_editingObject.State != cardStateAsByte || Insert)
                {
                    _editingObject.DateStateLastChange = DateTime.Now;
                    _editingObject.UtcDateStateLastChange = DateTime.UtcNow;

                    _editingObject.State = cardStateAsByte;
                }

                _editingObject.Description = _eDescription.Text;
                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();
                _editingObject.AlternateCardNumber = _eAlternateCardNumber.Text;

                _editingObject.ValidityDateFrom = _dpValidityDateFrom.Value;
                _editingObject.ValidityDateTo = _dpValidityDateTo.Value;

                if (!Insert && _relatedCardChanged && !StoreRelatedCardInfo())
                    return false;
                
                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private bool StoreRelatedCardInfo()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return false;

            Guid? relatedCardId = null;
            if (_relatedCard != null)
                relatedCardId = _relatedCard.IdCard;

            return CgpClient.Singleton.MainServerProvider.CardPairs.SetRelatedCard(_editingObject.IdCard, relatedCardId);
        }

        protected override bool CheckValues()
        {
            if (_actCardSystem == null)
            {
                if (_eCardNumber.Text == "")
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCardNumber,
                        GetString("ErrorEntryCardNumber"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eCardNumber.Focus();
                    return false;
                }
            }
            else
            {
                _actCardSystem = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(_actCardSystem.IdCardSystem);

                if (_eCardNumber.Text == "" && _actCardSystem.LengthCardNumber() > 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCardNumber,
                        GetString("ErrorEntryCardNumber"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eCardNumber.Focus();
                    return false;
                }

                if (_eCardNumber.Text.Length != _actCardSystem.LengthCardNumber())
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCardNumber,
                        GetString("ErrorWrondLengthCardNumber") + _actCardSystem.LengthCardNumber(), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eCardNumber.Focus();
                    return false;
                }

                if (_eCardNumber.Text != GetValidCardNumber())
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCardNumber,
                        GetString("ErrorCardNumberNotValid"), ControlNotificationSettings.Default);
                    return false;
                }
            }

            if (_actPerson != null)
            {
                if ((_ePin.Text == string.Empty) && (_actPerson.Cards.Count > 0))
                {
                    if (Dialog.Question(String.Format(GetString("QuestionSamePIN"),
                        _actPerson.FirstName, _actPerson.Surname)))
                        return true;
                    if (_ePin.Text != String.Empty)
                        return true;
                }
            }

            if (_ePin.Text == String.Empty)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne, _ePin,
                    GetString("ErrorEntryPin"), CgpClient.Singleton.ClientControlNotificationSettings);
                _ePin.Focus();
                return false;
            }

            if (_ePin.Text != string.Empty
                && _ePin.Text.Length < Card.MinimalPinLength
                || _ePin.Text.Length > Card.MaximalPinLength)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _ePin,
                    string.Format(
                        GetString("ErrorWrongPinLength"),
                        Card.MinimalPinLength,
                        Card.MaximalPinLength),
                    CgpClient.Singleton.ClientControlNotificationSettings);
                _ePin.Focus();

                return false;
            }

            if (_dpValidityDateFrom.Value != null
                && _dpValidityDateTo.Value != null
                && _dpValidityDateTo.Value.Value <= _dpValidityDateFrom.Value.Value)
            {
                _tcCard.SelectedTab = _tpTimetecSettings;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _dpValidityDateTo,
                    GetString("ErrorWrongValidityDateTimeFrom"), CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

#if !DEBUG
            if (_ePin.Text != string.Empty && NotSecurityPin())
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePin,
                    GetString("ErrorNotSecurityPin"), CgpClient.Singleton.ClientControlNotificationSettings);
                _ePin.Focus();
                return false;
            }
#endif

            if (_icbCardState.SelectedItem == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _icbCardState,
                GetString("ErrorEntryCardState"), CgpClient.Singleton.ClientControlNotificationSettings);
                _icbCardState.Focus();
                return false;
            }

            return true;
        }

        private bool NotSecurityPin()
        {
            if (!GeneralOptionsForm.Singleton.RequiredSecurePin) return false;

            if (!_pinEntered) return false;

            if (_ePin.Text.Length > 2)
            {
                if (_ePin.Text[0] == _ePin.Text[1])
                {
                    for (int i = 2; i < _ePin.Text.Length; i++)
                    {
                        if (_ePin.Text[i - 1] != _ePin.Text[i])
                            return false;
                    }
                }
                else if (_ePin.Text[0] == _ePin.Text[1] + 1)
                {
                    for (int i = 2; i < _ePin.Text.Length; i++)
                    {
                        if (_ePin.Text[i - 1] != _ePin.Text[i] + 1)
                            return false;
                    }
                }
                else if (_ePin.Text[0] == _ePin.Text[1] - 1)
                {
                    for (int i = 2; i < _ePin.Text.Length; i++)
                    {
                        if (_ePin.Text[i - 1] != _ePin.Text[i] - 1)
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.Cards.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    Dialog.Error(GetString("ErrorCardNumberExists"));
                }
                else
                    throw error;
            }

            if (retValue && _relatedCardChanged && !_editingObject.IdCard.Equals(Guid.Empty))
                StoreRelatedCardInfo();

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

            bool retValue = onlyInDatabase
                ? CgpClient.Singleton.MainServerProvider.Cards.UpdateOnlyInDatabase(_editingObject, out error)
                : CgpClient.Singleton.MainServerProvider.Cards.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    Dialog.Error(GetString("ErrorCardNumberExists"));
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
            CardsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            CardsForm.Singleton.AfterEdit(_editingObject);
        }

        private void DoAfterCardSystemCreated(object newCardSystem)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCardSystemCreated), newCardSystem);
            }
            else
            {
                if (newCardSystem is CardSystem)
                {
                    SetCardSystem(newCardSystem as CardSystem);
                }
            }
        }

        private void SetRelatedCard(object relatedCard)
        {
            SetRelatedCard(relatedCard as Card, false);
        }

        private void SetRelatedCard(Card relatedCard, bool initial)
        {
            _relatedCard = relatedCard;
            RefreshCardState(initial);

            if (_relatedCard == null)
            {
                _lRelatedCardTypeValue.Text = GetString("NotAHybridCard");
                return;
            }

            _tbmRelatedCard.Text = _relatedCard.ToString();
            _tbmRelatedCard.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_relatedCard);

            _lRelatedCardTypeValue.Text = string.Empty;

            if (_relatedCard.CardSystem == null)
                return;

            CardTypes cardType = CardTypes.GetCardType(_relatedCard.CardSystem.CardType);
            if (cardType == null)
                return;

            _lRelatedCardTypeValue.Text = cardType.Name;
        }

        private Card GetRelatedCardFromEditingObject()
        {
            Exception ex;
            return CgpClient.Singleton.MainServerProvider.CardPairs.GetRelatedCard(_editingObject.IdCard, out ex);
        }

        private void ModifyCardSystem()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                var listModObj = new List<IModifyObject>();

                IList<IModifyObject> listCardSystemsFromDatabase = 
                    CgpClient.Singleton.MainServerProvider.CardSystems.ListModifyObjects(out error);

                if (error != null)
                    throw error;

                listModObj.AddRange(listCardSystemsFromDatabase);

                var formAdd = 
                    new ListboxFormAdd(
                        listModObj, 
                        GetString("CardSystemsFormCardSystemsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var cardSystem = CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(outModObj.GetId);

                    if (!CheckValiditionCardNumberLength(cardSystem))
                        return;

                    SetCardSystem(cardSystem);
                    CgpClientMainForm.Singleton.AddToRecentList(_actCardSystem);
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void _ePin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_pinEntered)
            {
                _ePin.Text = string.Empty;
                _ePin.PasswordChar = '\0';
                _pinEntered = true;
            }
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void _eCardSystem_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eCardSystem_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddCardSystem(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddCardSystem(object cardSystem)
        {
            try
            {
                if (cardSystem.GetType() == typeof(CardSystem))
                {
                    SetCardSystem(cardSystem as CardSystem);
                    CgpClientMainForm.Singleton.AddToRecentList(cardSystem);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardSystem.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }

        private void SetCardSystem(CardSystem cardSystem)
        {
            _actCardSystem = cardSystem;
            ShowCardSystem();
            SetValidCardNumberLength();
            RefreshVisibleForAlternateCardNumber();
        }

        private void ShowCardSystem()
        {
            if (_actCardSystem == null)
            {
                _tbmCardSystem.Text = string.Empty;
            }
            else
            {
                _tbmCardSystem.Text = _actCardSystem.ToString();
                _tbmCardSystem.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actCardSystem);
            }
            SetActualFullCardNumber();
        }

        private void RelatedCardTextChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            _relatedCardChanged = true;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            if (sender == _eCardNumber)
            {
                string validText = GetValidCardNumber();
                if (_eCardNumber.Text != validText)
                {
                    _eCardNumber.Text = validText;
                    _eCardNumber.SelectionStart = _eCardNumber.TextLength;
                    _eCardNumber.SelectionLength = 0;
                }
            }
            else if (sender == _ePin)
            {
                if (!_pinEntered)
                {
                    _pinEntered = true;
                    _ePin.PasswordChar = '\0';
                    _ePin.Text = string.Empty;
                }
                string validText = QuickParser.GetValidDigitString(_ePin.Text);
                if (_ePin.Text != validText)
                {
                    _ePin.Text = validText;
                    _ePin.SelectionStart = _ePin.TextLength;
                    _ePin.SelectionLength = 0;
                }
            }

            base.EditTextChanger(sender, e);

            if (IsSetValues)
            {
                if (sender == _eCardNumber && _actCardSystem != null)
                {
                    SetValidCardNumberLength();
                }

                if (sender == _ePin)
                {
                    if (_ePin.Text.Length > Card.MaximalPinLength)
                    {
                        _ePin.Text = _ePin.Text.Substring(0, Card.MaximalPinLength);
                        _ePin.SelectionStart = _ePin.Text.Length;
                        _ePin.SelectionLength = 0;
                    }
                }
            }

            if (sender == _eCardNumber || sender == _tbmCardSystem)
                SetActualFullCardNumber();

            if (sender == _tbmCardSystem.ImageTextBox)
                SetActualCardType();
        }

        /// <summary>
        /// Get valid string for card number
        /// </summary>
        /// <returns></returns>
        private string GetValidCardNumber()
        {
            if (_actCardSystem == null)
                return Regex.Replace(
                    _eCardNumber.Text,
                    @"[^\u0021-\u007E]",
                    string.Empty);

            bool onlyDigitString = true;


            switch (_actCardSystem.CardType)
            {
                case (byte)CardType.Mifare:
                    if ((_actCardSystem.CardSubType == (byte)CardSubType.MifareStandardSectorReadin
                         || _actCardSystem.CardSubType == (byte)CardSubType.MifareSectorReadinWithMAD))
                    {
                        if (_actCardSystem.CardData is MifareSectorData && (_actCardSystem.CardData as MifareSectorData).Encoding == (byte)EncodingType.Bcd)
                        {
                            return QuickParser.GetValidHexaString(_eCardNumber.Text).Replace("-", "");
                        }
                        onlyDigitString = false;
                    }
                    break;

                case (byte)CardType.MifarePlus:

                    if (_actCardSystem.CardData is MifareSectorData && (_actCardSystem.CardData as MifareSectorData).Encoding != (byte)EncodingType.Bcd)
                    {
                        onlyDigitString = false;
                    }
                    break;
            }

            return onlyDigitString
                ? Regex.Replace(_eCardNumber.Text, @"[^\u0021-\u007E]", string.Empty)
                : QuickParser.GetValidPrintableString(_eCardNumber.Text);
        }

        /// <summary>
        /// Get valid string for alternate card number
        /// </summary>
        /// <returns></returns>
        private string GetValidAlternateCardNumber()
        {
            return QuickParser.GetValidPrintableString(_eAlternateCardNumber.Text);
        }

        private bool CheckValiditionCardNumberLength(CardSystem cardSystem)
        {
            if (cardSystem == null)
                return true;

            string cardNumber = _eCardNumber.Text;

            if (!string.IsNullOrEmpty(cardSystem.GetFullCompanyCode())
                && cardNumber.IndexOf(cardSystem.GetFullCompanyCode(), StringComparison.Ordinal) == 0)
            {
                cardNumber = cardNumber.Replace(
                    cardSystem.GetFullCompanyCode(),
                    string.Empty);
            }

            if (cardNumber.Length != cardSystem.LengthCardNumber())
            {
                string fullCardNumberFormat = cardSystem.GetFullCompanyCode();

                for (int i = 0; i < cardSystem.LengthCardNumber(); i++)
                    fullCardNumberFormat += "X";

                Dialog.Warning(string.Format(GetString("WarningCardNumberLength", cardSystem.LengthCardNumber(), fullCardNumberFormat)));

                return false;
            }

            return true;
        }

        private void SetValidCardNumberLength()
        {
            if (_actCardSystem == null
                || _eCardNumber.Text.Length <= _actCardSystem.LengthCardNumber())
            {
                return;
            }

            if (_eCardNumber.Text.IndexOf(_actCardSystem.GetFullCompanyCode(), StringComparison.Ordinal) == 0)
            {
                _eCardNumber.Text = _eCardNumber.Text.Replace(
                    _actCardSystem.GetFullCompanyCode(),
                    string.Empty);
            }

            //if (_eCardNumber.Text.Length > _actCardSystem.LengthCardNumber())
            //{
            //    _eCardNumber.Text =
            //        _eCardNumber.Text.Substring(
            //            0,
            //            _actCardSystem.LengthCardNumber());
            //}

            _eCardNumber.SelectionStart = _eCardNumber.Text.Length;
            _eCardNumber.SelectionLength = 0;
        }

        private const int MAX_ALTERNATE_CARD_NUMBER_LENGTH = 30;
        /// <summary>
        /// Check and set max length for alternate card number
        /// </summary>
        private void SetValidAlternateCardNumberLength()
        {
            if (_eAlternateCardNumber.Text.Length <= MAX_ALTERNATE_CARD_NUMBER_LENGTH)
                return;

            _eAlternateCardNumber.Text = 
                _eAlternateCardNumber.Text.Substring(
                    0, 
                    MAX_ALTERNATE_CARD_NUMBER_LENGTH);

            _eAlternateCardNumber.SelectionStart = 
                _eAlternateCardNumber.Text.Length;

            _eAlternateCardNumber.SelectionLength = 0;
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            if (sender == _eAlternateCardNumber)
            {
                string validText = GetValidAlternateCardNumber();

                if (_eAlternateCardNumber.Text != validText)
                {
                    _eAlternateCardNumber.Text = validText;

                    _eAlternateCardNumber.SelectionStart = _eAlternateCardNumber.TextLength;
                    _eAlternateCardNumber.SelectionLength = 0;
                }

                SetValidAlternateCardNumberLength();
                RefreshButtonCreateCard();
            }

            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        private void ModifyPerson()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) 
                return;

            Exception error = null;

            try
            {
                var listModObj = new List<IModifyObject>();

                var listPersonsFromDatabase = 
                    CgpClient.Singleton.MainServerProvider.Persons.ListModifyObjects(out error);

                if (error != null) 
                    throw error;

                listModObj.AddRange(listPersonsFromDatabase);

                var formAdd = 
                    new ListboxFormAdd(
                        listModObj, 
                        GetString("PersonsFormPersonsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);

                if (outModObj != null)
                {
                    _actPerson = 
                        CgpClient.Singleton.MainServerProvider.Persons
                            .GetObjectById(outModObj.GetId);

                    _tbmPerson.Text = GetPersonDisplayText(_actPerson);

                    _tbmPerson.TextImage =
                        ObjectImageList.Singleton
                            .GetImageForAOrmObject(_actPerson);

                    CgpClientMainForm.Singleton.AddToRecentList(_actPerson);
                }

                RefreshCardState(false);
            }
            catch
            {
                Dialog.Error(error);
            }
        }
        private void RemovePerson()
        {
            _actPerson = null;
            _tbmPerson.Text = "";

            RefreshCardState(false);
        }

        private void _tbmPerson_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmPerson_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddPerson(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddPerson(object obj)
        {
            try
            {
                var person = obj as Person;

                if (person == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmPerson.ImageTextBox,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                _actPerson = person;

                _tbmPerson.Text = GetPersonDisplayText(_actPerson);

                _tbmPerson.TextImage =
                    ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                CgpClientMainForm.Singleton.AddToRecentList(obj);
                RefreshCardState(false);
            }
            catch
            {
            }
        }

        private void AddDroppedRelatedCard(object obj)
        {
            try
            {
                var relatedCard = obj as Card;

                if (relatedCard == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmRelatedCard.ImageTextBox,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                if (relatedCard.IdCard.Equals(_editingObject.IdCard))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmRelatedCard.ImageTextBox,
                        GetString("ErrorRelatedCardItself"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (CgpClient.Singleton.MainServerProvider.CardPairs
                    .IsCardRelated(relatedCard.IdCard))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmRelatedCard.ImageTextBox,
                        GetString("ErrorRelatedCard"),
                        ControlNotificationSettings.Default);

                    return;
                }

                SetRelatedCard(
                    relatedCard,
                    false);

                CgpClientMainForm.Singleton.AddToRecentList(obj);
            }
            catch
            {
            }
        }

        private void _eCardSystem_DoubleClick(object sender, EventArgs e)
        {
            if (_actCardSystem != null)
                CardSystemsForm.Singleton.OpenEditForm(_actCardSystem);
        }

        private void _tbmPerson_DoubleClick(object sender, EventArgs e)
        {
            if (_actPerson != null)
                PersonsForm.Singleton.OpenEditForm(_actPerson);
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null
                && CgpClient.Singleton.MainServerProvider.Cards != null)
            {
                CgpClient.Singleton.MainServerProvider.Cards.EditEnd(_editingObject);
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects, 
                    CgpClient.Singleton.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.Cards.
                GetReferencedObjects(_editingObject.IdCard, CgpClient.Singleton.GetListLoadedPlugins());
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

        private void _tbmPerson_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            switch (item.Name)
            {
                case "_tsiModify1":

                    ModifyPerson();
                    break;

                case "_tsiRemove1":

                    RemovePerson();
                    break;
            }
        }

        private void _tbmCardSystem_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            switch (item.Name)
            {
                case "_tsiModify":

                    ModifyCardSystem();
                    break;

                case "_tsiCreate":

                {
                    var cardSystem = new CardSystem();
                    CardSystemsForm.Singleton.OpenInsertFromEdit(ref cardSystem, DoAfterCardSystemCreated);
                    break;
                }

                case "_tsiRemove":

                    string companyCode = _actCardSystem == null 
                        ? string.Empty 
                        : _actCardSystem.GetFullCompanyCode();

                    SetCardSystem(null);
                    _eCardNumber.Text = companyCode + _eCardNumber.Text;

                    break;
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CardsLocalAlarmInstructionView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.CardsLocalAlarmInstructionAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _tbmRelatedCard_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            switch (item.Name)
            {
                case "_tsiModify2":

                    ModifyCard();
                    break;

                case "_tsiRemove2":

                    RemoveRelatedCard();
                    break;
            }
        }

        private void RemoveRelatedCard()
        {
            _relatedCard = null;

            _tbmRelatedCard.Text = string.Empty;
            _lRelatedCardTypeValue.Text = GetString("NotAHybridCard");

            RefreshCardState(false);
        }

        private void ModifyCard()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            Exception error = null;
            try
            {
                var listModObj = new List<IModifyObject>();

                var listCardsFromDatabase = 
                    CgpClient.Singleton.MainServerProvider.Cards.ListModifyObjects(
                        new[] { _editingObject.IdCard }, 
                        false, 
                        out error);

                if (error != null)
                    throw error;

                listModObj.AddRange(listCardsFromDatabase);

                var formAdd = 
                    new ListboxFormAdd(
                        listModObj, 
                        GetString("CardsFormCardsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);

                if (outModObj == null)
                    return;
                Card relatedCard =
                    CgpClient.Singleton.MainServerProvider.Cards
                    .GetObjectById(outModObj.GetId);

                SetRelatedCard(relatedCard, false);

                CgpClientMainForm.Singleton.AddToRecentList(_relatedCard);
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void _tbmRelatedCard_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDroppedRelatedCard(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmRelatedCard_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmRelatedCard_DoubleClick(object sender, EventArgs e)
        {
            if (_relatedCard == null)
                return;

            CardsForm.Singleton.OpenEditForm(_relatedCard, SetRelatedCard);
        }

        /// <summary>
        /// Get and set visibility for all alternate card controls 
        /// </summary>
        private void RefreshVisibleForAlternateCardNumber()
        {
            bool visible = false;

            if (_actCardSystem != null)
            {
                switch (_actCardSystem.CardType)
                {
                    case (byte)CardType.Mifare:

                        if (_actCardSystem.CardSubType == (byte)CardSubType.MifareStandardSectorReadin ||
                            _actCardSystem.CardSubType == (byte)CardSubType.MifareSectorReadinWithMAD)
                        {
                            visible = true;
                        }

                        break;

                    case (byte)CardType.MifarePlus:

                        visible = true;
                        break;
                }
            }

            SetVisibleForAlternateCardNumber(visible);
        }

        /// <summary>
        /// Set visibility for all alternate card controls 
        /// </summary>
        /// <param name="visible"></param>
        private void SetVisibleForAlternateCardNumber(bool visible)
        {
            _lAlternateCardNumber.Visible = visible;
            _eAlternateCardNumber.Visible = visible;

            if (visible)
                RefreshButtonCreateCard();
            else
                _bCreateCard.Visible = false;
        }

        /// <summary>
        /// Set visibility and text for create card button
        /// </summary>
        private void RefreshButtonCreateCard()
        {

            if (_eAlternateCardNumber.Enabled == false || string.IsNullOrEmpty(_eAlternateCardNumber.Text))
            {
                _bCreateCard.Visible = false;
                return;
            }

            _bCreateCard.Visible = true;
            _bCreateCard.Enabled = false;

            SafeThread<string>.StartThread(
                LoadAlternateCard, 
                _eAlternateCardNumber.Text);
        }

        private readonly object _lockLoadAlternateCard = new object();
        /// <summary>
        /// Load actual alternate card from the alternate card number
        /// </summary>
        private void LoadAlternateCard(string alternateCardNumber)
        {
            lock (_lockLoadAlternateCard)
            {
                if (alternateCardNumber != GetActualAlternateCardNumber())
                    return;

                try
                {
                    if (!CgpClient.Singleton.IsConnectionLost(false))
                        _alternateCard =
                            CgpClient.Singleton.MainServerProvider.Cards
                                .GetCardByFullNumber(alternateCardNumber);
                }
                catch { }

                RefreshCreateCardButtonText(alternateCardNumber);
            }
        }

        /// <summary>
        /// Get actual alternate card number from the edit
        /// </summary>
        /// <returns></returns>
        private string GetActualAlternateCardNumber()
        {
            return InvokeRequired
                ? Invoke(new Func<string>(GetActualAlternateCardNumber)) as string
                : _eAlternateCardNumber.Text;
        }

        /// <summary>
        /// Refresh text for button create card
        /// </summary>
        private void RefreshCreateCardButtonText(string alternateCardNumber)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<string>(RefreshCreateCardButtonText), 
                    alternateCardNumber);

                return;
            }

            if (alternateCardNumber != _eAlternateCardNumber.Text)
                return;

            _bCreateCard.Text = GetString(_alternateCard != null
                ? "General_bEdit"
                : "General_bCreate");

            _bCreateCard.Enabled = true;
        }

        private void _bCreateCard_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_eAlternateCardNumber.Text))
                return;

            if (_alternateCard != null)
                CardsForm.Singleton.OpenEditForm(
                    _alternateCard,
                    AfterEditAlternateCard);
            else
            {
                _bCreateCard.Enabled = false;
                SafeThread.StartThread(DoCreateAlternateCard);
            }
        }

        /// <summary>
        /// Show form for create card
        /// </summary>
        private void DoCreateAlternateCard()
        {
            try
            {
                CardsForm.Singleton.InsertWithData(
                    _eAlternateCardNumber.Text, 
                    AfterEditAlternateCard);
            }
            catch { }

            DoAfterShowCardForm();
        }

        /// <summary>
        /// Enable button create card for alternate card after show form for create card
        /// </summary>
        private void DoAfterShowCardForm()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(DoAfterShowCardForm));
                return;
            }

            _bCreateCard.Enabled = true;
        }

        /// <summary>
        /// Load actual alternate card from the alternate card number after create or edit alternate card
        /// </summary>
        /// <param name="obj"></param>
        private void AfterEditAlternateCard(object obj)
        {
            SafeThread<string>.StartThread(
                LoadAlternateCard, 
                GetActualAlternateCardNumber());
        }
    }
}
