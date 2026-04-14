using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Net;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Crypto;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class InterCcuCommunication : ASingleton<InterCcuCommunication>
    {
        private enum MessagePeerEventType : byte
        {
            BinaryMessageRecieved = 0,
            BinaryMessageRetryingFailed = 1,
            RemotePeerAliveAgain = 2
        }

        private class MessagePeerEventData : IProcessingQueueRequest
        {
            private readonly MessagePeerEventType _eventType;
            private readonly IPEndPoint _remoteAddress;
            private readonly ByteDataCarrier _data;

            public MessagePeerEventData(MessagePeerEventType eventType, IPEndPoint remoteAddress, ByteDataCarrier data)
            {
                _eventType = eventType;
                _remoteAddress = remoteAddress;
                _data = data;
            }

            public void Execute()
            {
                switch (_eventType)
                {
                    case MessagePeerEventType.BinaryMessageRecieved:

                        Singleton.BinaryMessageReceived(
                            _remoteAddress, 
                            _data);

                        break;

                    case MessagePeerEventType.BinaryMessageRetryingFailed:

                        Singleton.BinaryMessageRetryingFailed(
                            _remoteAddress, 
                            _data);

                        break;

                    case MessagePeerEventType.RemotePeerAliveAgain:

                        Singleton.RemotePeerAliveAgain(_remoteAddress);
                        break;
                }
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private const string BINDINGS_SETTINGS_NAME = @"\BindingsSettings.dat";
        public const string THREAD_NAME_SAVE_TO_FILE = "InterCCUCommunication: save to file";
        public const int TIME_FOR_SEND_STATE_CHANGE = 15;

        private readonly object _lockBindings = new object();

        private readonly Dictionary<IPAddress, List<SendChangesObject>> _sendChangesObjects = 
            new Dictionary<IPAddress, List<SendChangesObject>>();

        private readonly Dictionary<ControlledObjectIdentificator, List<ControlledObjectSettings>> _controlledObjects =
            new Dictionary<ControlledObjectIdentificator, List<ControlledObjectSettings>>();
        private readonly MessagingPeer _messagePeer;

        private readonly ThreadPoolQueue<MessagePeerEventData> _messagePeerEventsQueue =
            new ThreadPoolQueue<MessagePeerEventData>(ThreadPoolGetter.Get());

        private InterCcuCommunication() : base(null)
        {
            try
            {
                _messagePeer = new MessagingPeer();
                _messagePeer.Start(null, Definitions.NCASConstants.TCP_ICCU_PORT);
                _messagePeer.BinaryMessageReceived += _messagePeer_BinaryMessageRecieved;
                _messagePeer.BinaryMessageRetryingFailed += _messagePeer_BinaryMessageRetryingFailed;
                _messagePeer.RemotePeerAliveAgain += _messagePeer_RemotePeerAliveAgain;
                
                Events.ProcessEvent(
                    new EventICcuPortAlreadyUsed(
                        State.Normal));

                var idCcu = Ccus.Singleton.GetCcuId();

                if (idCcu != null)
                {
                    AlarmsManager.Singleton.StopAlarm(
                        IccuPortAlreadyUsedAlarm.CreateAlarmKey(
                            idCcu.Value));
                }

            }
            catch (Exception error)
            {
                Events.ProcessEvent(
                    new EventICcuPortAlreadyUsed(
                        State.Alarm));

                var idCcu = Ccus.Singleton.GetCcuId();

                if (idCcu != null)
                {
                    AlarmsManager.Singleton.AddAlarm(
                        new IccuPortAlreadyUsedAlarm(
                            idCcu.Value));
                }

                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Load bingings from the file
        /// </summary>
        public void LoadFromFile()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void InterCCUCommunication.LoadFromFile()");

            MemoryStream memoryStream = null;
            Stream inputStrem = null;
            try
            {
                if (File.Exists(CcuCore.Singleton.RootPath + CcuCore.TEMP + BINDINGS_SETTINGS_NAME))
                {
                    inputStrem = PatchedFileStream.Open(
                        CcuCore.Singleton.RootPath + CcuCore.TEMP + BINDINGS_SETTINGS_NAME,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);

                    if (inputStrem.Length > 0)
                    {
                        memoryStream = new MemoryStream();
                        var buffer = new byte[512];

                        do
                        {
                            var length = inputStrem.Read(buffer, 0, buffer.Length);

                            if (length == 0)
                                break;

                            memoryStream.Write(buffer, 0, length);

                        }
                        while (true);

                        SaveBindings(memoryStream.ToArray(), false);
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (memoryStream != null)
                {
                    try
                    {
                        memoryStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }

                if (inputStrem != null)
                {
                    try
                    {
                        inputStrem.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }
        }

        /// <summary>
        /// Change bindings and save to file
        /// </summary>
        /// <param name="bindingsData"></param>
        public void SaveBindings(byte[] bindingsData)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void InterCCUCommunication.SaveBindings(byte[] bindingsData): [{0}]", Log.GetStringFromParameters(bindingsData)));
            SaveBindings(bindingsData, true);
        }

        /// <summary>
        /// Change bindings and save to file
        /// </summary>
        /// <param name="bindingsData"></param>
        /// <param name="saveToFile"></param>
        private void SaveBindings(byte[] bindingsData, bool saveToFile)
        {
            try
            {
                var ccusBindingsForControlledObject = new List<CCUBinding>();
                var oldCcuIpAddresses = new List<IPAddress>();
                lock (_lockBindings)
                {
                    if (_sendChangesObjects.Count > 0)
                    {
                        foreach (var ccuSendChangesObjects in _sendChangesObjects)
                        {
                            oldCcuIpAddresses.Add(ccuSendChangesObjects.Key);

                            if (ccuSendChangesObjects.Value != null && ccuSendChangesObjects.Value.Count > 0)
                            {
                                foreach (var sendChangesObject in ccuSendChangesObjects.Value)
                                {
                                    if (sendChangesObject != null)
                                        sendChangesObject.Close();
                                }
                            }
                        }
                    }

                    _sendChangesObjects.Clear();

                    if (_controlledObjects != null && _controlledObjects.Count > 0)
                    {
                        foreach (var kvpListControlledObjectSettings in _controlledObjects)
                        {
                            var fromCcuIpAddress = kvpListControlledObjectSettings.Key.FromCcuIpAddress;
                            if (!oldCcuIpAddresses.Contains(fromCcuIpAddress))
                            {
                                oldCcuIpAddresses.Add(fromCcuIpAddress);
                            }

                            if (kvpListControlledObjectSettings.Value != null && kvpListControlledObjectSettings.Value.Count > 0)
                            {
                                foreach (var controlledObjectSettings in kvpListControlledObjectSettings.Value)
                                {
                                    if (controlledObjectSettings != null)
                                        controlledObjectSettings.Close();
                                }
                            }
                        }
                    }
                    if (null != _controlledObjects)
                    {
                        _controlledObjects.Clear();

                        var ccuBindings = CCUBinding.GetFromBytes(bindingsData);
                        if (ccuBindings != null && ccuBindings.Count > 0)
                        {
                            foreach (var ccuBinding in ccuBindings)
                            {
                                if (ccuBinding.IsObjectForActivation)
                                {
                                    var toCcuIp = new IPAddress(ccuBinding.ToCCU);

                                    if (oldCcuIpAddresses.Contains(toCcuIp))
                                    {
                                        oldCcuIpAddresses.Remove(toCcuIp);
                                    }

                                    var sendChangesObject = 
                                        new SendChangesObject(
                                            ccuBinding.ToCCU, 
                                            ccuBinding.ObjectType, 
                                            ccuBinding.ObjectGuid);

                                    List<SendChangesObject> ccuSendChangesObjects;
                                    if (!_sendChangesObjects.TryGetValue(toCcuIp, out ccuSendChangesObjects))
                                    {
                                        ccuSendChangesObjects = new List<SendChangesObject>();
                                        _sendChangesObjects.Add(toCcuIp, ccuSendChangesObjects);
                                    }

                                    if (ccuSendChangesObjects != null)
                                        ccuSendChangesObjects.Add(sendChangesObject);
                                }
                                else if (ccuBinding.IsControlObject)
                                {
                                    var fromCcuIp = new IPAddress(ccuBinding.FromCCU);

                                    if (oldCcuIpAddresses.Contains(fromCcuIp))
                                    {
                                        oldCcuIpAddresses.Remove(fromCcuIp);
                                    }

                                    var controlledObjectIdentificator =
                                        new ControlledObjectIdentificator(ccuBinding.FromCCU, ccuBinding.Command,
                                            ccuBinding.DCULogicalAddres, ccuBinding.ObjectForActivationIndex);
                                    var controlledObjectSettings =
                                        new ControlledObjectSettings(ccuBinding.ObjectForActivationType,
                                            ccuBinding.ObjectForActivationGuid, ccuBinding.ObjectType,
                                            ccuBinding.ObjectGuid);

                                    List<ControlledObjectSettings> listControlObjectSettings;
                                    if (
                                        !_controlledObjects.TryGetValue(controlledObjectIdentificator,
                                            out listControlObjectSettings))
                                    {
                                        listControlObjectSettings = new List<ControlledObjectSettings>();
                                        _controlledObjects.Add(controlledObjectIdentificator, listControlObjectSettings);
                                    }

                                    if (listControlObjectSettings != null)
                                        listControlObjectSettings.Add(controlledObjectSettings);

                                    ccusBindingsForControlledObject.Add(ccuBinding);
                                }
                            }
                        }
                    }

                    UnbindFromMessagePeer(oldCcuIpAddresses);
                }

                if (saveToFile)
                    SafeThread<byte[]>.StartThread(SaveToFile, bindingsData, THREAD_NAME_SAVE_TO_FILE);

                SendRefresh(ccusBindingsForControlledObject);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Close not used connections for ccus
        /// </summary>
        /// <param name="ccuIpAddresses"></param>
        private void UnbindFromMessagePeer(List<IPAddress> ccuIpAddresses)
        {
            if (ccuIpAddresses != null && ccuIpAddresses.Count > 0)
            {
                foreach (var ipAddress in ccuIpAddresses)
                {
                    _messagePeer.Unbind(ipAddress);
                }
            }
        }

        /// <summary>
        /// Get states for controled objects from all ccus
        /// </summary>
        /// <param name="ccusBindingsForControlledObject"></param>
        private void SendRefresh(List<CCUBinding> ccusBindingsForControlledObject)
        {
            if (ccusBindingsForControlledObject != null && ccusBindingsForControlledObject.Count > 0)
            {
                foreach (var ccuBindingForControlledObject in ccusBindingsForControlledObject)
                {
                    if (ccuBindingForControlledObject != null)
                    {
                        try
                        {
                            var ipAddress = new IPAddress(ccuBindingForControlledObject.FromCCU);
                            var dataToSend = InterCcuCommunicationMessage.GetMessage((byte)InterCCUCommunicationCommand.RefreshObjectsStates,
                                0, 0, 0);

                            SendData(ipAddress, dataToSend);
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save bindings to the file
        /// </summary>
        /// <param name="bindingsData"></param>
        public void SaveToFile(byte[] bindingsData)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void InterCCUCommunication.SaveToFile(byte[] bindingsData): [{0}]", Log.GetStringFromParameters(bindingsData)));
            Stream outputStream = null;
            try
            {
                var fileName = CcuCore.Singleton.RootPath + CcuCore.TEMP + BINDINGS_SETTINGS_NAME;

                if (bindingsData != null && bindingsData.Length > 0)
                {
                    if (!Directory.Exists(CcuCore.Singleton.RootPath + CcuCore.TEMP))
                        Directory.CreateDirectory(CcuCore.Singleton.RootPath + CcuCore.TEMP);

                    outputStream = PatchedFileStream.Open(
                        fileName,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.Read);

                    outputStream.Write(bindingsData, 0, bindingsData.Length);
                }
                else
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (outputStream != null)
                {
                    try
                    {
                        outputStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }
        }

        private void _messagePeer_BinaryMessageRecieved(MessagingPeer messagePeer, IPEndPoint remoteAddress, ByteDataCarrier data)
        {
            _messagePeerEventsQueue.Enqueue(
                new MessagePeerEventData(
                    MessagePeerEventType.BinaryMessageRecieved,
                    remoteAddress,
                    data));
        }

        private void _messagePeer_BinaryMessageRetryingFailed(MessagingPeer messagingPeer, IPEndPoint remoteAddress, ByteDataCarrier binaryData)
        {
            if (binaryData == null || remoteAddress == null)
                return;

            _messagePeerEventsQueue.Enqueue(
                new MessagePeerEventData(MessagePeerEventType.BinaryMessageRetryingFailed,
                    remoteAddress,
                    binaryData));
        }

        private void _messagePeer_RemotePeerAliveAgain(MessagingPeer messagingPeer, IPEndPoint remoteAddress)
        {
            _messagePeerEventsQueue.Enqueue(
                new MessagePeerEventData(MessagePeerEventType.RemotePeerAliveAgain,
                    remoteAddress,
                    null));
        }


        private void BinaryMessageReceived(IPEndPoint remoteAddress, ByteDataCarrier data)
        {
            if (data != null)
            {
                var message = InterCcuCommunicationMessage.GetFromBytes(data.Buffer, data.Length);
                if (message != null)
                {
                    switch (message.Command)
                    {
                        case (byte)InterCCUCommunicationCommand.RefreshObjectsStates:
                            SendActualObjectsStates(remoteAddress.Address.GetAddressBytes());
                            break;
                        default:
                            lock (_lockBindings)
                            {
                                List<ControlledObjectSettings> listControlledObjectSettings;
                                if (_controlledObjects.TryGetValue(new ControlledObjectIdentificator(remoteAddress.Address.GetAddressBytes(), message.Command, message.DcuLogicalAddress, message.OnOffObjectIndex),
                                    out listControlledObjectSettings) && listControlledObjectSettings != null)
                                {
                                    if (listControlledObjectSettings.Count > 0)
                                    {
                                        foreach (var controlledObjectSettings in listControlledObjectSettings)
                                        {
                                            if (controlledObjectSettings != null)
                                            {
                                                controlledObjectSettings.ChangedObject((State) message.OnOffObjectState);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void BinaryMessageRetryingFailed(IPEndPoint remoteAddress, ByteDataCarrier binaryData)
        {
            var message = InterCcuCommunicationMessage.GetFromBytes(binaryData.Buffer, binaryData.Length);
            if (message != null)
            {
                switch (message.Command)
                {
                    case (byte)InterCCUCommunicationCommand.InputChanged:
                    case (byte)InterCCUCommunicationCommand.OutputChanged:
                        lock (_lockBindings)
                        {
                            var ccuIpAddress = new IPAddress(remoteAddress.Address.GetAddressBytes());

                            if (_sendChangesObjects != null && _sendChangesObjects.Count > 0)
                            {
                                List<SendChangesObject> ccuSendChangesObjects;
                                if (_sendChangesObjects.TryGetValue(ccuIpAddress, out ccuSendChangesObjects) && ccuSendChangesObjects != null &&
                                    ccuSendChangesObjects.Count > 0)
                                {
                                    foreach (var sendChangesObject in ccuSendChangesObjects)
                                    {
                                        if (sendChangesObject != null && sendChangesObject.Compare(message.Command, message.DcuLogicalAddress, message.OnOffObjectIndex))
                                        {
                                            Events.ProcessEvent(
                                                new EventICcuSendingOfObjectStateFailed(
                                                    sendChangesObject.OnOffObjectGuid,
                                                    sendChangesObject.OnOffObjectType));

                                            AlarmsManager.Singleton.AddAlarm(
                                                new IccuSendingOfObjectStateFailedAlarm(
                                                    sendChangesObject.OnOffObjectGuid,
                                                    sendChangesObject.OnOffObjectType));
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void RemotePeerAliveAgain(IPEndPoint remoteAddress)
        {
            SendActualObjectsStates(remoteAddress.Address.GetAddressBytes());
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="data"></param>
        /// <param name="secondsForRetry"></param>
        public void SendData(IPAddress ipAddress, byte[] data, int secondsForRetry)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void InterCCUCommunication.SendData(IPAddress ipAddress, byte[] data): [{0}]", Log.GetStringFromParameters(ipAddress, data)));
            if (data != null)
                _messagePeer.SendMessage(ipAddress, new ByteDataCarrier(data, data.Length), secondsForRetry);
        }

        /// <summary>
        /// Send message with infinity retry
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="data"></param>
        public void SendData(IPAddress ipAddress, byte[] data)
        {
            SendData(ipAddress, data, -1);
        }

        private void SendActualObjectsStates(byte[] toCcu)
        {
            var strToCcu = new IPAddress(toCcu);
            lock (_lockBindings)
            {
                List<SendChangesObject> ccuSendChangedsObject;
                if (_sendChangesObjects.TryGetValue(strToCcu, out ccuSendChangedsObject) &&
                    ccuSendChangedsObject != null &&
                    ccuSendChangedsObject.Count > 0)
                {
                    foreach (var sendChangesObject in ccuSendChangedsObject)
                    {
                        if (sendChangesObject != null)
                            sendChangesObject.SendActualState();
                    }
                }
            }
        }

        /// <summary>
        /// Set object state if is controled by object from another CCU
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectGuid"></param>
        /// <returns></returns>
        public bool SetObjectActualState(ObjectType objectType, Guid objectGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool InterCCUCommunication.SetObjectActualState(ObjectType objectType, Guid objectGuid): [{0}]", Log.GetStringFromParameters(objectType, objectGuid)));
            lock (_lockBindings)
            {
                if (_controlledObjects != null && _controlledObjects.Count > 0)
                {
                    foreach (var listControlledObjectSettings in _controlledObjects.Values)
                    {
                        if (listControlledObjectSettings != null && listControlledObjectSettings.Count > 0)
                        {
                            foreach (var controlledObjectSettings in listControlledObjectSettings)
                            {
                                if (controlledObjectSettings != null)
                                {
                                    if (controlledObjectSettings.ObjectType == objectType && controlledObjectSettings.ObjectGuid == objectGuid)
                                    {
                                        controlledObjectSettings.SetToAcutalState();
                                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool InterCCUCommunication.SetObjectActualState return true");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool InterCCUCommunication.SetObjectActualState return false");
            return false;
        }

    }
    public class SendChangesObject
    {
        private class OnOffInputChangedListener : IInputChangedListener
        {
            private readonly SendChangesObject _sendChangesObject;

            public OnOffInputChangedListener(SendChangesObject sendChangesObject)
            {
                _sendChangesObject = sendChangesObject;
            }

            public override int GetHashCode()
            {
                return _sendChangesObject._toCcuIpAddress.GetHashCode();
            }

            public bool Equals(IInputChangedListener other)
            {
                var otherListener = other as OnOffInputChangedListener;

                return otherListener != null &&
                       _sendChangesObject._toCcuIpAddress.Equals(
                           otherListener._sendChangesObject._toCcuIpAddress);
            }

            public void OnInputChanged(
                Guid guidInput,
                State state)
            {
                _sendChangesObject.OnOffObjectChanged(
                    guidInput,
                    state);
            }
        }

        //private byte[] _fromCCUIpAddress;
        private readonly IPAddress _toCcuIpAddress;
        private readonly byte _command;
        private readonly byte _dcuLogicalAddress;
        private readonly byte _onOffObjectIndex;

        private Action<Guid, State> _onOfObjectChangedEvent;

        private IInputChangedListener _onOffInputChangedListener;

        private readonly ObjectType _onOffObjectType;
        private readonly Guid _onOffObjectGuid;

        public ObjectType OnOffObjectType { get { return _onOffObjectType; } }
        public Guid OnOffObjectGuid { get { return _onOffObjectGuid; } }



        public SendChangesObject(
            byte[] toCcuIpAddress, 
            byte onOffObjectType, 
            byte[] onOffObjectGuid)
        {
            _toCcuIpAddress = new IPAddress(toCcuIpAddress);
            _onOffObjectType = (ObjectType)onOffObjectType;
            _onOffObjectGuid = new Guid(onOffObjectGuid);
            _onOfObjectChangedEvent = OnOffObjectChanged;

            switch (_onOffObjectType)
            {
                case ObjectType.Input:
                    _command = (byte)InterCCUCommunicationCommand.InputChanged;
                    var input = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.Input, _onOffObjectGuid) as Input;
                    if (input != null)
                    {
                        _onOffObjectIndex = input.InputNumber;

                        if (input.GuidDCU != Guid.Empty)
                        {
                            var dcu = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.DCU, input.GuidDCU) as DCU;
                            if (dcu != null)
                            {
                                _dcuLogicalAddress = dcu.LogicalAddress;
                            }
                        }
                    }

                    _onOffInputChangedListener = 
                        new OnOffInputChangedListener(this);

                    Inputs.Singleton.AddInputChangedListener(
                        _onOffObjectGuid,
                        _onOffInputChangedListener);

                    SendActualState();
                    break;

                case ObjectType.Output:
                    _command = (byte)InterCCUCommunicationCommand.OutputChanged;
                    var output = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.Output, _onOffObjectGuid) as Output;
                    if (output != null)
                    {
                        _onOffObjectIndex = output.OutputNumber;

                        if (output.GuidDCU != Guid.Empty)
                        {
                            var dcu = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.DCU, output.GuidDCU) as DCU;
                            if (dcu != null)
                            {
                                _dcuLogicalAddress = dcu.LogicalAddress;
                            }
                        }
                    }

                    Outputs.Singleton.AddOutputChanged(_onOffObjectGuid, _onOfObjectChangedEvent);
                    SendActualState();
                    break;
            }
        }

        public bool Compare(byte command, byte dcuLogicalAddress, byte onOffObjectIndex)
        {
            return _command == command && _dcuLogicalAddress == dcuLogicalAddress && _onOffObjectIndex == onOffObjectIndex;
        }

        public void SendActualState()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void SendChangesObject.SendActualState()");
            switch (_onOffObjectType)
            {
                case ObjectType.Input:
                    var inputState = Inputs.Singleton.GetInputLogicalState(_onOffObjectGuid);
                    if (inputState != State.Unknown)
                        OnOffObjectChanged(_onOffObjectGuid, inputState);
                    break;
                case ObjectType.Output:
                    var outputState = Outputs.Singleton.GetOutputState(_onOffObjectGuid);
                    if (outputState != State.Unknown)
                        OnOffObjectChanged(_onOffObjectGuid, outputState);
                    break;
            }
        }

        public void Close()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void SendChangesObject.Close()");

            switch (_onOffObjectType)
            {
                case ObjectType.Input:

                    if (_onOffInputChangedListener != null)
                    {
                        Inputs.Singleton.RemoveInputChangedListener(
                            _onOffObjectGuid,
                            _onOffInputChangedListener);

                        _onOffInputChangedListener = null;
                    }

                    break;

                case ObjectType.Output:

                    if (_onOfObjectChangedEvent != null)
                    {
                        Outputs.Singleton.RemoveOutputChanged(
                            _onOffObjectGuid, 
                            _onOfObjectChangedEvent);

                        _onOfObjectChangedEvent = null;
                    }

                    break;
            }
        }

        private void OnOffObjectChanged(Guid objectGuid, State state)
        {
            InterCcuCommunication.Singleton.SendData(
                _toCcuIpAddress,
                InterCcuCommunicationMessage.GetMessage(
                    _command, 
                    _dcuLogicalAddress, 
                    _onOffObjectIndex, 
                    (byte)state), 
                    InterCcuCommunication.TIME_FOR_SEND_STATE_CHANGE);
        }
    }

    public class ControlledObjectSettings
    {
        private readonly ObjectType _changedObjectType;
        private readonly Guid _changedObjectGuid;
        private readonly ObjectType _objectType;
        private readonly Guid _objectGuid;
        private State _actualState = State.Unknown;

        public ObjectType ObjectType { get { return _objectType; } }
        public Guid ObjectGuid { get { return _objectGuid; } }
        public State ActualState { get { return _actualState; } }

        public ControlledObjectSettings(
            byte changedObjectType, 
            byte[] changedObjectGuid, 
            byte objectType, 
            byte[] objectGuid)
        {
            _changedObjectType = (ObjectType)changedObjectType;
            _changedObjectGuid = new Guid(changedObjectGuid);
            _objectType = (ObjectType)objectType;
            _objectGuid = new Guid(objectGuid);
        }

        public void ChangedObject(State changedObjectState)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void ControlledObjectSettings.ChangedObject(State changedObjectState): [{0}]", Log.GetStringFromParameters(changedObjectState)));
            _actualState = changedObjectState;
            switch (_changedObjectType)
            {
                case ObjectType.Input:
                    switch (changedObjectState)
                    {
                        case State.Alarm:
                            SetObjectToOn();
                            break;
                        case State.Normal:
                            SetObjectToOff();
                            break;

                    }
                    break;
                case ObjectType.Output:
                    switch (changedObjectState)
                    {
                        case State.On:
                            SetObjectToOn();
                            break;
                        case State.Off:
                            SetObjectToOff();
                            break;
                    }
                    break;
            }
        }

        public void SetToAcutalState()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void ControlledObjectSettings.SetToAcutalState()");
            ChangedObject(_actualState);
        }

        private void SetObjectToOn()
        {
            switch (_objectType)
            {
                case ObjectType.Input:

                    Inputs.Singleton.IccuInputOrOutputStateChanged(
                        _objectGuid,
                        true);

                    break;
                
                case ObjectType.Output:

                    Outputs.Singleton.On(
                        _changedObjectGuid.ToString(), 
                        _objectGuid);

                    break;

                case ObjectType.AlarmArea:

                    AlarmAreas.Singleton.SetAlarmAreaOnOffObject(_objectGuid);

                    break;
            }
        }

        private void SetObjectToOff()
        {
            switch (_objectType)
            {
                case ObjectType.Input:

                    Inputs.Singleton.IccuInputOrOutputStateChanged(
                        _objectGuid,
                        false);

                    break;

                case ObjectType.Output:

                    Outputs.Singleton.Off(
                        _changedObjectGuid.ToString(),
                        _objectGuid);

                    break;

                case ObjectType.AlarmArea:

                    AlarmAreas.Singleton.UnsetAlarmAreaOnOffObject(_objectGuid);

                    break;
            }
        }

        public void Close()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void ControlledObjectSettings.Close()");
            SetObjectToOff();
        }
    }

    public class ControlledObjectIdentificator
    {
        private readonly byte[] _fromCcuIpAddress;
        private readonly byte _command;
        private readonly byte _dcuLogicalAddress;
        private readonly byte _onOffObjectIndex;

        public IPAddress FromCcuIpAddress
        {
            get { return new IPAddress(_fromCcuIpAddress); }   
        }

        public ControlledObjectIdentificator(
            byte[] fromCcuIpAddress, 
            byte command, 
            byte dcuLogicalAddress, 
            byte onOffObjectIndex)
        {
            _fromCcuIpAddress = fromCcuIpAddress;
            _command = command;
            _dcuLogicalAddress = dcuLogicalAddress;
            _onOffObjectIndex = onOffObjectIndex;
        }

        public static bool operator ==(ControlledObjectIdentificator obj1, ControlledObjectIdentificator obj2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)obj1 == null) || ((object)obj2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return obj1.GetString == obj2.GetString;
        }

        public static bool operator !=(ControlledObjectIdentificator obj1, ControlledObjectIdentificator obj2)
        {
            return !(obj1 == obj2);
        }

        public override int GetHashCode()
        {
            return QuickHashes.GetSHA1String(GetString).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var controlledObjectIdentificator = obj as ControlledObjectIdentificator;
            if ((Object)controlledObjectIdentificator == null)
            {
                return false;
            }

            return GetString == controlledObjectIdentificator.GetString;
        }

        private string GetString
        {
            get
            {
                var str = string.Empty;

                if (_fromCcuIpAddress != null)
                {
                    for (var i = 0; i < _fromCcuIpAddress.Length; i++)
                    {
                        str += _fromCcuIpAddress[i] + " ";
                    }
                }
                str += _command + " " + _dcuLogicalAddress + " " + _onOffObjectIndex;

                return str;
            }
        }
    }

    public class InterCcuCommunicationMessage
    {
        private const byte PROTOCOL_VERSION = 2;

        private byte _command;
        private byte _dcuLogicalAddress;
        private byte _onOffObjectIndex;
        private byte _onOffObjectState;

        public byte Command { get { return _command; } }
        public byte DcuLogicalAddress { get { return _dcuLogicalAddress; } }
        public byte OnOffObjectIndex { get { return _onOffObjectIndex; } }
        public byte OnOffObjectState { get { return _onOffObjectState; } }

        public static byte[] GetMessage(byte command, byte dcuLogicalAddress, byte onOffObjectIndex,
            byte onOffObjectstate)
        {
            try
            {
                var bytes = new byte[6];

                bytes[0] = PROTOCOL_VERSION;
                bytes[1] = command;
                bytes[2] = dcuLogicalAddress;
                bytes[3] = onOffObjectIndex;
                bytes[4] = onOffObjectstate;
                bytes[5] = Crc8.ComputeChecksum(bytes, 0, 5);

                return bytes;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return null;
        }

        public static InterCcuCommunicationMessage GetFromBytes(byte[] bytes, int length)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("InterCCUCommunicationMessage InterCCUCommunicationMessage.InterCCUCommunicationMessage GetFromBytes(byte[] bytes, int length): [{0}]",
                Log.GetStringFromParameters(bytes, length)));
            if (bytes != null && length == 6)
            {
                if (bytes[0] == PROTOCOL_VERSION && Crc8.ComputeChecksum(bytes, 0, 5) == bytes[5])
                {
                    var message = new InterCcuCommunicationMessage
                    {
                        _command = bytes[1],
                        _dcuLogicalAddress = bytes[2],
                        _onOffObjectIndex = bytes[3],
                        _onOffObjectState = bytes[4]
                    };

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("InterCCUCommunicationMessage InterCCUCommunicationMessage.InterCCUCommunicationMessage return {0}:", Log.GetStringFromParameters(message)));
                    return message;
                }
            }

            CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "InterCCUCommunicationMessage InterCCUCommunicationMessage.InterCCUCommunicationMessage return null");
            return null;
        }
    }
}
