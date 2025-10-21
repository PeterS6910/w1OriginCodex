using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.Cgp.Globals
{
    public class ObjTypeHelper
    {
        public static byte GetObjectTypePriority(byte objTypeByte)
        {
            try
            {
                ObjectType objType = (ObjectType)objTypeByte;
                switch (objType)
                {
                    case ObjectType.AlarmArea:
                        return 100;
                    case ObjectType.CCU:
                        return 90;
                    case ObjectType.DCU:
                        return 85;
                    case ObjectType.DoorEnvironment:
                        return 82;
                    case ObjectType.MultiDoor:
                        return 82;
                    case ObjectType.MultiDoorElement:
                        return 81;
                    case ObjectType.CardReader:
                        return 80;
                    case ObjectType.Output:
                        return 75;
                    case ObjectType.Input:
                        return 75;
                    case ObjectType.Card:
                        return 70;
                    case ObjectType.Person:
                        return 10;
                    default:
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public const string CardReaderBlocked = "CardReaderBlocked";
        public const string DCUOffline = "DCUOffline";
        public const string CCUOffline = "CCUOffline";
    }
}
