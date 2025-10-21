using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbCardsAccessor: SqlCeDbConfigObjectsAccessor<DB.Card>, ICardsStorage
    {
        private class FullCardNumberIndexedDbColumn : IDbColumn<DB.Card>
        {
            public object GetValue(DB.Card card)
            {
                return card.FullCardNumber;
            }

            public string Name { get { return FULL_CARD_NUMBER_COLUMN_NAME; } }
            public string DbTypeString { get { return "nVarChar(255)"; } }
            public SqlDbType DbType { get { return SqlDbType.NVarChar; } }
            public int? DbSize { get { return 255; } }
            public bool AllowNull { get { return false; } }
        }

        private class StateIndexedDbColumn : IDbColumn<DB.Card>
        {
            public object GetValue(DB.Card card)
            {
                return card.State;
            }

            public string Name { get { return STATE_COLUMN_NAME; } }
            public string DbTypeString { get { return "tinyint"; } }
            public SqlDbType DbType { get { return SqlDbType.TinyInt; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private const string TABLE_NAME = "Card";
        private const string ID_CARD_COLUMN_NAME = "IdCard";
        private const string FULL_CARD_NUMBER_COLUMN_NAME = "FullCardNumber";
        private const string STATE_COLUMN_NAME = "State";

        private readonly ISelectByIndexedColumnsCommand<DB.Card> _getCardByFullCardNumberCommand;
        private readonly SqlCeDbCommand _getFullCardNumberByIdCardCommand;
        private readonly SqlCeDbCommand _getCardIdsByState;

        public SqlCeDbCardsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalHandlerGroup)
            : this(
                sqlCeDbCommandFactory,
                new FullCardNumberIndexedDbColumn(),
                new StateIndexedDbColumn(),
                dbObjectRemovalHandlerGroup)
        {
        }

        private SqlCeDbCardsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            FullCardNumberIndexedDbColumn fullCardNumberIndexedDbColumn,
            StateIndexedDbColumn stateIndexedDbColumn,
            EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalHandlerGroup)
            : base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.Card,
                ID_CARD_COLUMN_NAME,
                new []
                {
                    new DbIndex<DB.Card>(fullCardNumberIndexedDbColumn),
                    new DbIndex<DB.Card>(stateIndexedDbColumn)
                },
                new ObjectCreatorFromSerializedData<DB.Card>(),
                CardPinAccessManager.Singleton,
                dbObjectRemovalHandlerGroup)
        {
            _getCardByFullCardNumberCommand = CreateSelectByIndexedColumnsCommand(
                new IDbParameterDefinition[] {fullCardNumberIndexedDbColumn});

            _getFullCardNumberByIdCardCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2}",
                    FULL_CARD_NUMBER_COLUMN_NAME,
                    TABLE_NAME,
                    ID_CARD_COLUMN_NAME));

            _getFullCardNumberByIdCardCommand.Prepare(PrimaryKeyDbColumns);

            _getCardIdsByState = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2}",
                    ID_CARD_COLUMN_NAME,
                    TABLE_NAME,
                    STATE_COLUMN_NAME));

            _getCardIdsByState.Prepare(
                Enumerable.Repeat(
                    (IDbParameterDefinition) stateIndexedDbColumn,
                    1));
        }

        ICard ICardsStorage.GetCard(string fullCardNumber)
        {
            lock (OperationLock)
            {
                var cards = _getCardByFullCardNumberCommand.ExecuteReader(
                    Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            FULL_CARD_NUMBER_COLUMN_NAME,
                            fullCardNumber),
                        1));

                return cards != null
                    ? cards.FirstOrDefault()
                    : null;
            }
        }

        ICard ICardsStorage.GetFirstCard()
        {
            lock (OperationLock)
            {
                if (PrimaryKeyValues.Count == 0)
                    return null;

                var primaryKey = PrimaryKeyValues.First();

                return GetFromDatabaseInternal(primaryKey);
            }
        }

        string ICardsStorage.GetFullCardNumber(Guid idCard)
        {
            lock (OperationLock)
            {
                return _getFullCardNumberByIdCardCommand.ExecuteScalar(
                    Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            ID_CARD_COLUMN_NAME,
                            idCard),
                        1)) as string;
            }
        }

        public ICollection<Guid> GetCardIdsByState(byte state)
        {
            lock (OperationLock)
            {
                var cardIdsByState = new HashSet<Guid>();

                var rows = _getCardIdsByState.ExecuteReader(
                    Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            STATE_COLUMN_NAME,
                            state),
                        1));

                foreach (var row in rows)
                {
                    cardIdsByState.Add((Guid) row[0]);
                }

                return cardIdsByState;
            }
        }
    }
}
