using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Contal.Cgp.NCAS.Globals
{
    public enum InterCCUCommunicationCommand : byte
    {
        RefreshObjectsStates = 0x5,
        InputChanged = 0x11,
        OutputChanged = 0x12
    }

    public class CCUBinding
    {
        private const byte OBJECT_FOR_ACTIVATION = 0;
        private const byte CONTROL_OBJECT = 1;

        private byte _bindingsObjectType;

        private byte[] _fromCCU = null;
        private byte[] _toCCU = null;

        private byte _command;
        private byte _dcuLogicalAddress;
        private byte _objectForActivationIndex;
        private byte _objectForActivationType;
        private byte[] _objectForActivationGuid = null;

        private byte _objectType;
        private byte[] _objectGuid = null;

        public bool IsObjectForActivation { get { return _bindingsObjectType == OBJECT_FOR_ACTIVATION; } }
        public bool IsControlObject { get { return _bindingsObjectType == CONTROL_OBJECT; } }

        public byte[] FromCCU { get { return _fromCCU; } }
        public byte Command { get { return _command; } }
        public byte DCULogicalAddres { get { return _dcuLogicalAddress; } }
        public byte ObjectForActivationIndex { get { return _objectForActivationIndex; } }
        public byte ObjectForActivationType { get { return _objectForActivationType; } }
        public byte[] ObjectForActivationGuid { get { return _objectForActivationGuid; } }

        public byte[] ToCCU { get { return _toCCU; } }
        public byte ObjectType { get { return _objectType; } }
        public byte[] ObjectGuid { get { return _objectGuid; } }

        public static CCUBinding CrateObjectForActiavation(string fromCCUIpAddress, string toCCUIpAddress, byte objectType, Guid objectGuid)
        {
            try
            {
                CCUBinding ccuBinding = new CCUBinding();
                ccuBinding._bindingsObjectType = CCUBinding.OBJECT_FOR_ACTIVATION;

                IPAddress ipAddress = IPAddress.Parse(fromCCUIpAddress);
                ccuBinding._fromCCU = ipAddress.GetAddressBytes();
                ipAddress = IPAddress.Parse(toCCUIpAddress);
                ccuBinding._toCCU = ipAddress.GetAddressBytes();

                ccuBinding._objectType = objectType;
                ccuBinding._objectGuid = objectGuid.ToByteArray();

                return ccuBinding;
            }
            catch { }

            return null;
        }

        public static CCUBinding CrateControlObject(string fromCCUIpAddress, string toCCUIpAddress, InterCCUCommunicationCommand command, byte dcuLogicalAddress, byte objectForActivationIndex, byte objectForActivationType, Guid objectForActivationGuid, byte objectType, Guid objectGuid)
        {
            try
            {
                CCUBinding ccuBinding = new CCUBinding();
                ccuBinding._bindingsObjectType = CCUBinding.CONTROL_OBJECT;

                IPAddress ipAddress = IPAddress.Parse(fromCCUIpAddress);
                ccuBinding._fromCCU = ipAddress.GetAddressBytes();
                ipAddress = IPAddress.Parse(toCCUIpAddress);
                ccuBinding._toCCU = ipAddress.GetAddressBytes();

                ccuBinding._command = (byte)command;
                ccuBinding._dcuLogicalAddress = dcuLogicalAddress;
                ccuBinding._objectForActivationIndex = objectForActivationIndex;
                ccuBinding._objectForActivationType = objectForActivationType;
                ccuBinding._objectForActivationGuid = objectForActivationGuid.ToByteArray();

                ccuBinding._objectType = objectType;
                ccuBinding._objectGuid = objectGuid.ToByteArray();

                return ccuBinding;
            }
            catch { }

            return null;
        }

        public static List<CCUBinding> GetFromBytes(byte[] bindingsData)
        {
            List<CCUBinding> ccuBindings = new List<CCUBinding>();

            try
            {
                if (bindingsData != null)
                {
                    int position = 0;
                    while (position < bindingsData.Length)
                    {
                        switch (bindingsData[position])
                        {
                            case OBJECT_FOR_ACTIVATION:
                                if (position + 26 <= bindingsData.Length)
                                {
                                    CCUBinding ccuBinding = new CCUBinding();
                                    ccuBinding._bindingsObjectType = OBJECT_FOR_ACTIVATION;
                                    byte[] fromCCU = new byte[4];
                                    for (int i = 0; i < fromCCU.Length; i++)
                                    {
                                        fromCCU[i] = bindingsData[position + 1 + i];
                                    }
                                    ccuBinding._fromCCU = fromCCU;
                                    byte[] toCCU = new byte[4];
                                    for (int i = 0; i < toCCU.Length; i++)
                                    {
                                        toCCU[i] = bindingsData[position + 5 + i];
                                    }
                                    ccuBinding._toCCU = toCCU;
                                    ccuBinding._objectType = bindingsData[position + 9];

                                    byte[] objectGuid = new byte[16];
                                    for (int i = 0; i < objectGuid.Length; i++)
                                    {
                                        objectGuid[i] = bindingsData[position + 10 + i];
                                    }
                                    ccuBinding._objectGuid = objectGuid;

                                    ccuBindings.Add(ccuBinding);
                                }
                                position += 26;

                                break;
                            case CONTROL_OBJECT:
                                if (position + 46 <= bindingsData.Length)
                                {
                                    CCUBinding ccuBinding = new CCUBinding();
                                    ccuBinding._bindingsObjectType = CONTROL_OBJECT;
                                    byte[] fromCCU = new byte[4];
                                    for (int i = 0; i < fromCCU.Length; i++)
                                    {
                                        fromCCU[i] = bindingsData[position + 1 + i];
                                    }
                                    ccuBinding._fromCCU = fromCCU;
                                    byte[] toCCU = new byte[4];
                                    for (int i = 0; i < toCCU.Length; i++)
                                    {
                                        toCCU[i] = bindingsData[position + 5 + i];
                                    }
                                    ccuBinding._toCCU = toCCU;
                                    ccuBinding._command = bindingsData[position + 9];
                                    ccuBinding._dcuLogicalAddress = bindingsData[position + 10];
                                    ccuBinding._objectForActivationIndex = bindingsData[position + 11];
                                    ccuBinding._objectForActivationType = bindingsData[position + 12];
                                    byte[] objectForActivationGuid = new byte[16];
                                    for (int i = 0; i < objectForActivationGuid.Length; i++)
                                    {
                                        objectForActivationGuid[i] = bindingsData[position + 13 + i];
                                    }
                                    ccuBinding._objectForActivationGuid = objectForActivationGuid;
                                    ccuBinding._objectType = bindingsData[position + 29];
                                    byte[] objectGuid = new byte[16];
                                    for (int i = 0; i < objectGuid.Length; i++)
                                    {
                                        objectGuid[i] = bindingsData[position + 30 + i];
                                    }
                                    ccuBinding._objectGuid = objectGuid;

                                    ccuBindings.Add(ccuBinding);
                                }
                                position += 46;
                                break;
                            default:
                                position = bindingsData.Length;
                                break;
                        }
                    }
                }
            }
            catch { }

            return ccuBindings;
        }

        public byte[] ToBytes()
        {
            try
            {
                switch (_bindingsObjectType)
                {
                    case OBJECT_FOR_ACTIVATION:
                        if (_toCCU != null && _objectGuid != null)
                        {
                            byte[] bytes = new byte[26];
                            bytes[0] = _bindingsObjectType;
                            _fromCCU.CopyTo(bytes, 1);
                            _toCCU.CopyTo(bytes, 5);
                            bytes[9] = _objectType;
                            _objectGuid.CopyTo(bytes, 10);
                            return bytes;
                        }
                        break;
                    case CONTROL_OBJECT:
                        if (_fromCCU != null && _objectGuid != null)
                        {
                            byte[] bytes = new byte[46];
                            bytes[0] = _bindingsObjectType;
                            _fromCCU.CopyTo(bytes, 1);
                            _toCCU.CopyTo(bytes, 5);
                            bytes[9] = _command;
                            bytes[10] = _dcuLogicalAddress;
                            bytes[11] = _objectForActivationIndex;
                            bytes[12] = _objectForActivationType;
                            _objectForActivationGuid.CopyTo(bytes, 13);
                            bytes[29] = _objectType;
                            _objectGuid.CopyTo(bytes, 30);
                            return bytes;
                        }
                        break;
                }
            }
            catch { }

            return null;
        }
    }
}
