using System;
using System.Collections.Generic;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(327)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class SecurityTimeZone : IDbObject
    {
        private const byte PRIORITY_UNLCKED = 1;
        private const byte PRIORITY_LOCKED = 7;
        private const byte PRIORITY_CARD = 5;
        private const byte PRIORITY_CARD_PIN = 6;
        private const byte PRIORITY_TOGGLE_CARD = 5;
        private const byte PRIORITY_TOGGLE_CARD_PIN = 6;
        private const byte PRIORITY_CODE = 2;
        private const byte PRIORITY_CODE_CARD = 3;
        private const byte PRIORITY_CODE_CARD_PIN = 4;

        [LwSerialize()]
        public virtual Guid IdSecurityTimeZone { get; set; }
        private Guid _guidCalendar = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidCalendar { get { return _guidCalendar; } set { _guidCalendar = value; } }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }

        public static State GetState(DateTime dateTime, Guid guidSecurityTimeZone)
        {
            SecurityTimeZone securityTimeZone = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityTimeZone, guidSecurityTimeZone) as SecurityTimeZone;
            if (securityTimeZone != null)
            {
                return securityTimeZone.GetState(dateTime);
            }

            return State.locked;
        }

        public Guid GetGuid()
        {
            return IdSecurityTimeZone;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.SecurityTimeZone;
        }

        public State GetState(DateTime dateTime)
        {
            if (GuidDateSettings != null)
            {
                State state = State.Unknown;

                List<Guid> guidSecurityDailyPlanList = GetGuidsActualSecurityDailyPlans(dateTime);
                
                foreach (Guid idSecurityDailyPlan in guidSecurityDailyPlanList)
                {
                    State actualState = SecurityDailyPlans.Singleton.GetActualState(idSecurityDailyPlan);

                    if (GetPriorityFromState(state) < GetPriorityFromState(actualState))
                        state = actualState;
                }

                if (state != State.Unknown)
                    return state;
            }

            return State.locked;
        }

        public List<Guid> GetGuidsActualSecurityDailyPlans(DateTime dateTime)
        {
            List<Guid> securityDailyPlans = new List<Guid>();
            List<Guid> explicitSecurityDailyPlans = new List<Guid>();

            if (GuidDateSettings != null && GuidDateSettings.Count > 0)
            {
                foreach (Guid guidDateSetting in GuidDateSettings)
                {
                    SecurityTimeZoneDateSetting dateSetting = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityTimeZoneDateSetting, guidDateSetting) as SecurityTimeZoneDateSetting;
                    if (dateSetting != null && dateSetting.IsActual(dateTime, GuidCalendar))
                    {
                        if (dateSetting.ExplicitSecurityDailyPlan)
                        {
                            explicitSecurityDailyPlans.Add(dateSetting.GuidSecurityDailyPlan);
                        }
                        else
                        {
                            securityDailyPlans.Add(dateSetting.GuidSecurityDailyPlan);
                        }
                    }
                }
            }

            if (explicitSecurityDailyPlans.Count > 0)
            {
                return explicitSecurityDailyPlans;
            }
            else
            {
                return securityDailyPlans;
            }
        }

        public static byte GetPriorityFromState(State state)
        {
            switch (state)
            {
                case State.unlocked:
                    return PRIORITY_UNLCKED;
                case State.code:
                    return PRIORITY_CODE;
                case State.codecard:
                    return PRIORITY_CODE_CARD;
                case State.codecardpin:
                    return PRIORITY_CODE_CARD_PIN;
                case State.card:
                    return PRIORITY_CARD;
                case State.togglecard:
                    return PRIORITY_TOGGLE_CARD;
                case State.cardpin:
                    return PRIORITY_CARD_PIN;
                case State.togglecardpin:
                    return PRIORITY_TOGGLE_CARD_PIN;
                case State.locked:
                    return PRIORITY_LOCKED;
                default:
                    return 0;
            }
        }
    }
}
