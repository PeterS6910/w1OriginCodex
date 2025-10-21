using System;
using System.Drawing;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys.Microsoft;
using Microsoft.Win32;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public class Support
    {
        private static bool? _enableParentInFullName = null;
        public static bool EnableParentInFullName
        {
            get
            {
                RegistryKey registryKey = null;
                if (_enableParentInFullName == null)
                    try
                    {
                        registryKey = RegistryHelper.GetOrAddKey(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS);
                        if (null == registryKey)
                            return false;
                        if (registryKey.GetValue(CgpServerGlobals.CGP_ENABLE_PARENT_IN_FULL_NAME) == null)
                            registryKey.SetValue(CgpServerGlobals.CGP_ENABLE_PARENT_IN_FULL_NAME, false);
                        _enableParentInFullName = Convert.ToBoolean(registryKey.GetValue(CgpServerGlobals.CGP_ENABLE_PARENT_IN_FULL_NAME));
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                return _enableParentInFullName != null ? (bool)_enableParentInFullName : false;
            }
        }
    }

    public class NameAttribute : Attribute
    {
        public string Name { get; protected set; }

        public NameAttribute(string value)
        {
            this.Name = value;
        }
    }

    public class PriorityAttribute : Attribute
    {
        public byte Priority { get; protected set; }

        public PriorityAttribute(byte value)
        {
            this.Priority = value;
        }

        public byte GetPriority()
        {
            return Priority;
        }
    }

    public class SecurityLevelColor
    {
        private Color _colorUnlocked = Color.LightGreen;
        private Color _colorLocked = Color.Red;
        private Color _colorCard = Color.Yellow;
        private Color _colorCardPin = Color.Magenta;
        private Color _colorToggleCard = Color.DarkGoldenrod;
        private Color _colorToggleCardPin = Color.Indigo;
        private Color _colorCode = Color.Blue;
        private Color _colorCodeOrCard = Color.Aqua;
        private Color _colorCodeOrCardPIN = Color.Lavender;

        public Color ColorUnlocked
        {
            get { return _colorUnlocked; }
        }
        public Color ColorLocked
        {
            get { return _colorLocked; }
        }
        public Color ColorCard
        {
            get { return _colorCard; }
        }
        public Color ColorCardPin
        {
            get { return _colorCardPin; }
        }
        public Color ColorToggleCard
        {
            get { return _colorToggleCard; }
        }
        public Color ColorToggleCardPin
        {
            get { return _colorToggleCardPin; }
        }
        public Color ColorGin
        {
            get { return _colorCode; }
        }

        public Color ColorGinOrCardPIN
        {
            get { return _colorCodeOrCardPIN; }
        }

        public Color ColorGinOrCard
        {
            get { return _colorCodeOrCard; }
        }

        public Color GetCollorForSecurityLevel(SecurityLevel4SLDP securityLevel)
        {
            if (securityLevel == SecurityLevel4SLDP.unlocked)
            {
                return _colorUnlocked;
            }
            else if (securityLevel == SecurityLevel4SLDP.locked)
            {
                return _colorLocked;
            }
            else if (securityLevel == SecurityLevel4SLDP.card)
            {
                return _colorCard;
            }
            else if (securityLevel == SecurityLevel4SLDP.cardpin)
            {
                return _colorCardPin;
            }
            else if (securityLevel == SecurityLevel4SLDP.togglecard)
            {
                return _colorToggleCard;
            }
            else if (securityLevel == SecurityLevel4SLDP.togglecardpin)
            {
                return _colorToggleCardPin;
            }
            else if (securityLevel == SecurityLevel4SLDP.code)
            {
                return _colorCode;
            }
            else if (securityLevel == SecurityLevel4SLDP.codeorcard)
            {
                return _colorCodeOrCard;
            }
            else if (securityLevel == SecurityLevel4SLDP.codeorcardpin)
            {
                return _colorCodeOrCardPIN;
            }
            return Color.White;
        }

        public Color GetCollorForSecurityLevel(byte securityLevel)
        {
            try
            {
                SecurityLevel4SLDP sls = (SecurityLevel4SLDP)securityLevel;
                return GetCollorForSecurityLevel(sls);
            }
            catch
            {
                return Color.White;
            }
        }
    }

    public class SecurityLevelPriority
    {
        private byte _priorityUnlocked = 1;
        private byte _priorityLocked = 7;
        private byte _priorityCard = 5;
        private byte _priorityCardPin = 6;
        private byte _priorityToggleCard = 5;
        private byte _priorityToggleCardPin = 6;
        private byte _priorityCode = 2;
        private byte _priorityCodeOrCard = 3;
        private byte _priorityCodeOrCardPin = 4;  

        public byte PriorityGinCardPin
        {
            get { return _priorityCodeOrCardPin; }
        }
        public byte PriorityGinCard
        {
            get { return _priorityCodeOrCard; }
        }
        public byte PriorityUnlocked
        {
            get { return _priorityUnlocked; }
        }
        public byte PriorityLocked
        {
            get { return _priorityLocked; }
        }
        public byte PriorityCard
        {
            get { return _priorityCard; }
        }
        public byte PriorityCardPin
        {
            get { return _priorityCardPin; }
        }
        public byte PriorityToggleCard
        {
            get { return _priorityToggleCard; }
        }
        public byte PriorityToggleCardPin
        {
            get { return _priorityToggleCardPin; }
        }
        public byte PriorityGin
        {
            get { return _priorityCode; }
        }

        public byte GetPriorityForSecurityLevel(SecurityLevel4SLDP securityLevel)
        {
            switch (securityLevel)
            {
                case SecurityLevel4SLDP.unlocked: return _priorityUnlocked;
                case SecurityLevel4SLDP.locked: return _priorityLocked;
                case SecurityLevel4SLDP.card: return _priorityCard;
                case SecurityLevel4SLDP.cardpin: return _priorityCardPin;
                case SecurityLevel4SLDP.code: return _priorityCode;
                case SecurityLevel4SLDP.codeorcard: return _priorityCodeOrCard;
                case SecurityLevel4SLDP.codeorcardpin: return _priorityCodeOrCardPin;
                case SecurityLevel4SLDP.togglecard: return _priorityToggleCard;
                case SecurityLevel4SLDP.togglecardpin: return _priorityToggleCardPin;
                default:
                    return 0;
                    //break;
            }
            //return 0;
        }

        public byte GetPriorityForSecurityLevel(byte securityLevel)
        {
            try
            {
                SecurityLevel4SLDP sls = (SecurityLevel4SLDP)securityLevel;
                return GetPriorityForSecurityLevel(sls);
            }
            catch
            {
                return 0;
            }
        }
    }

    [Serializable]
    public enum UpgradeType : byte
    {
        CCUUpgrade = 0,
        DCUUpgrade = 1,
        CEUpgrade = 2,
        CRUpgrade = 3
    }
}
