using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using JetBrains.Annotations;

namespace Contal.Cgp.Server.DB
{
    partial class Eventlogs
    {
        public class ObjTypePriority
        {
            public string Name { get; private set; }
            public byte Priority { get; private set; }

            public ObjTypePriority(string name, byte priority)
            {
                Name = name;
                Priority = priority;
            }

            public ObjTypePriority(
                [NotNull] CentralNameRegister cnr,
                [NotNull] Dictionary<IdAndObjectType, string> objectsNameCache)
            {
                Priority = ObjTypeHelper.GetObjectTypePriority(cnr.ObjectType);

                var objectType = (ObjectType) cnr.ObjectType;

                switch (objectType)
                {
                    case ObjectType.Card:
                        var cardIdAndObjectType =
                            new IdAndObjectType(
                                cnr.Id,
                                objectType);

                        string fullCardNamber;

                        if (objectsNameCache.TryGetValue(
                            cardIdAndObjectType,
                            out fullCardNamber))
                        {
                            Name = fullCardNamber;
                            return;
                        }

                        var card = Cards.Singleton.GetById(cnr.Id);

                        if (card != null)
                        {
                            Name = card.FullCardNumber;

                            objectsNameCache.Add(
                                cardIdAndObjectType,
                                Name);

                            return;
                        }

                        break;

                    case ObjectType.Input:
                        Name = string.IsNullOrEmpty(cnr.AlternateName)
                            ? cnr.Name
                            : string.Format(
                                "{0} - {1}",
                                cnr.AlternateName,
                                cnr.Name);

                        return;

                    case ObjectType.AlarmArea:
                        var alarmAreaIdAndObjectType =
                            new IdAndObjectType(
                                cnr.Id,
                                objectType);

                        string alarmAreaName;

                        if (objectsNameCache.TryGetValue(
                            alarmAreaIdAndObjectType,
                            out alarmAreaName))
                        {
                            Name = alarmAreaName;
                            return;
                        }

                        var alarmArea = CgpServerRemotingProvider.Singleton.GetTableObject(
                            objectType,
                            cnr.Id.ToString());                               

                        if (alarmArea != null)
                        {
                            Name = alarmArea.ToString();

                            objectsNameCache.Add(
                                alarmAreaIdAndObjectType,
                                Name);

                            return;
                        }

                        break;

                    case ObjectType.CCU:
                        var ccuIdAndObjectType =
                            new IdAndObjectType(
                                cnr.Id,
                                objectType);

                        string ccuName;

                        if (objectsNameCache.TryGetValue(
                            ccuIdAndObjectType,
                            out ccuName))
                        {
                            Name = ccuName;
                            return;
                        }

                        var ccu = CgpServerRemotingProvider.Singleton.GetTableObject(
                                ObjectType.CCU,
                                cnr.Id.ToString());

                        if (ccu != null)
                        {
                            Name = ccu.ToString();

                            objectsNameCache.Add(
                                ccuIdAndObjectType,
                                Name);

                            return;
                        }

                        break;
                }

                Name = !string.IsNullOrEmpty(cnr.AlternateName) &&
                       cnr.AlternateName != Persons.PERSON_CONVERSION_STRING
                    ? string.Format(
                        "{0} {1}",
                        cnr.Name,
                        cnr.AlternateName)
                    : cnr.Name;
            }
        }
    }
}