using System;
using System.ComponentModel;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAntiPassBackZoneEditForm
    {
        private class CardInZoneView
        {
            public const string COLUMN_ISCHECKED = "IsChecked";
            public const string COLUMN_NAME = "Name";
            public const string COLUMN_CARDNUMBER = "CardNumber";
            public const string COLUMN_ENTRYDATETIME = "EntryDateTime";
            public const string COLUMN_ENTRYCARDREADERNAME = "EntryCardReaderName";
            public const string COLUMN_APBZ_CR_ENTRY_BY = "ApbzCrEntryBy";

            private readonly CardInAntiPassBackZone _cardInAntiPassBackZone;

            public CardInZoneView(CardInAntiPassBackZone cardInAntiPassBackZone)
            {
                _cardInAntiPassBackZone = cardInAntiPassBackZone;
            }

            public bool IsChecked { get; set; }

            public string Name
            {
                get { return _cardInAntiPassBackZone.Name; }
            }

            public string CardNumber
            {
                get { return _cardInAntiPassBackZone.CardNumber; }
            }

            public DateTime EntryDateTime
            {
                get { return _cardInAntiPassBackZone.EntryDateTime; }
            }

            public string EntryCardReaderName
            {
                get { return _cardInAntiPassBackZone.EntryCardReaderName; }
            }

            public string ApbzCrEntryBy
            {
                get
                {
                    if (NCASClient.LocalizationHelper != null)
                        return
                            NCASClient.LocalizationHelper.GetString("ApbzCardReaderEntryExitBy_" +
                                                                    _cardInAntiPassBackZone.EntryBy);

                    return string.Empty;
                }
            }

            [Browsable(false)]
            public Guid Id
            {
                get { return _cardInAntiPassBackZone.IdCard; }
            }

            public bool Contains(string expression)
            {
                return
                    AOrmObject.RemoveDiacritism(Name)
                        .ToLower()
                        .Contains(AOrmObject.RemoveDiacritism(expression).ToLower())
                        || CardNumber.Contains(expression);
            }
        }

        internal class CardReaderView
        {
            public const string COLUMN_NAME = "Name";
            public const string COLUMN_ENTRY_EXIT_BY = "EntryExitBy";

            private readonly NCASAntiPassBackZoneEditForm _form;
            private readonly CardReader _cardReader;
            private readonly bool _isEntryCardReader;

            public CardReaderView(
                NCASAntiPassBackZoneEditForm form,
                CardReader cardReader,
                bool isEntryCardReader)
            {
                _form = form;
                _cardReader = cardReader;
                _isEntryCardReader = isEntryCardReader;
            }

            public string Name
            {
                get { return _cardReader.Name; }
            }

            [Browsable(false)]
            public CardReader CardReader
            {
                get
                {
                    return _cardReader;
                }
            }

            public ApbzCardReaderEntryExitBy EntryExitBy
            {
                get
                {
                    return _form.GetEntryExitBy(
                        CardReader,
                        _isEntryCardReader);
                }

                set
                {
                    _form.SetEntryExitBy(
                        CardReader,
                        _isEntryCardReader,
                        value);
                }
            }
        }

        private class ApbzCardReaderEntryExitByView
        {
            public const string COLUMN_NAME = "Name";
            public const string COLUMN_APBZ_CARD_READER_ENTRY_EXIT_BY = "ApbzCardReaderEntryExitBy";

            private readonly ApbzCardReaderEntryExitBy _apbzCardReaderEntryExitBy;

            public ApbzCardReaderEntryExitBy ApbzCardReaderEntryExitBy
            {
                get { return _apbzCardReaderEntryExitBy; }
            }

            public ApbzCardReaderEntryExitByView(ApbzCardReaderEntryExitBy apbzCardReaderEntryExitBy)
            {
                _apbzCardReaderEntryExitBy = apbzCardReaderEntryExitBy;
            }

            public string Name
            {
                get { return ToString(); }
            }

            public override string ToString()
            {
                if (NCASClient.LocalizationHelper != null)
                    return
                        NCASClient.LocalizationHelper.GetString("ApbzCardReaderEntryExitByView_" +
                                                                _apbzCardReaderEntryExitBy);

                return string.Empty;
            }
        }
    }
}