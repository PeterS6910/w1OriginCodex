using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    public class BindingInterCCUCommunication
    {
        private static BindingInterCCUCommunication _singleton;
        private static object _syncRoot = new object();

        private Dictionary<ObjectType, Dictionary<Guid, ObjectBindings>> _objectTypesBindings = new Dictionary<ObjectType, Dictionary<Guid, ObjectBindings>>();
        private Dictionary<Guid, List<SendChangesObject>> _sendChangesObjects = new Dictionary<Guid, List<SendChangesObject>>();
        private Dictionary<Guid, List<ControlledObject>> _controlledObjects = new Dictionary<Guid, List<ControlledObject>>();
        private Dictionary<Guid, bool> _sendBindingsToCCU = new Dictionary<Guid, bool>();

        public static BindingInterCCUCommunication Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new BindingInterCCUCommunication();
                    }

                return _singleton;
            }
        }

        public void Init()
        {
            ICollection<Output> outputs = Outputs.Singleton.List();
            if (outputs != null && outputs.Count > 0)
            {
                foreach (Output output in outputs)
                {
                    DoObjectChanged(ObjectType.Output, output.IdOutput, false);
                }
            }

            ICollection<Input> inputs = Inputs.Singleton.List();
            if (inputs != null && inputs.Count > 0)
            {
                foreach (Input input in inputs)
                {
                    DoObjectChanged(ObjectType.Input, input.IdInput, false);
                }
            }

            ICollection<AlarmArea> alarmAreas = AlarmAreas.Singleton.List();
            if (alarmAreas != null && alarmAreas.Count > 0)
            {
                foreach (AlarmArea alarmArea in alarmAreas)
                {
                    DoObjectChanged(ObjectType.AlarmArea, alarmArea.IdAlarmArea, false);
                }
            }

            CUDObjectHandlerForServer.Singleton.Register(
                ObjectChanged,
                ObjectType.Output,
                ObjectType.Input,
                ObjectType.AlarmArea);
        }

        private void ObjectChanged(ObjectType objectType, object objectId, bool isInserted)
        {
            SafeThread<ObjectType, object, bool>.StartThread(
                DoObjectChanged,
                objectType,
                objectId,
                true);
        }

        private void DoObjectChanged(ObjectType objectType, object objectGuid, bool sendToCCUs)
        {
            if (objectGuid == null)
                return;

            lock (_objectTypesBindings)
            {
                switch (objectType)
                {
                    case ObjectType.Output:
                        Output output = Outputs.Singleton.GetById(objectGuid);
                        if (output != null)
                        {
                            DeleteAllBindings(ObjectType.Output, output.IdOutput);
                            if (output.ControlType == (byte)OutputControl.controledByObject && output.OnOffObject != null)
                            {
                                if (output.OnOffObject is Input)
                                {
                                    Input onOffObject = output.OnOffObject as Input;
                                    if (onOffObject != null)
                                    {
                                        Guid outputCCUGuid = Outputs.Singleton.GetParentCCU(output.IdOutput);
                                        Guid onOffObjectCCUGuid = Inputs.Singleton.GetParentCCU(onOffObject.IdInput);

                                        if (outputCCUGuid != Guid.Empty && onOffObjectCCUGuid != Guid.Empty && outputCCUGuid != onOffObjectCCUGuid)
                                        {
                                            SendChangesObject sendChangesObject = new SendChangesObject(onOffObjectCCUGuid, outputCCUGuid, ObjectType.Input, onOffObject.IdInput);
                                            ControlledObject controlledObject = new ControlledObject(onOffObjectCCUGuid, outputCCUGuid, ObjectType.Input, onOffObject.IdInput, ObjectType.Output, output.IdOutput);
                                            ObjectBindings objectBindings = new ObjectBindings(outputCCUGuid, controlledObject, onOffObjectCCUGuid, sendChangesObject);

                                            AddToObjectTypeBindings(ObjectType.Output, output.IdOutput, objectBindings);
                                            AddToCCUObjectsForActivation(sendChangesObject, onOffObjectCCUGuid);
                                            AddToCCUControlObjects(controlledObject, outputCCUGuid);

                                            _sendBindingsToCCU[outputCCUGuid] = true;
                                            _sendBindingsToCCU[onOffObjectCCUGuid] = true;
                                        }
                                    }
                                }
                                else if (output.OnOffObject is Output)
                                {
                                    Output onOffObject = output.OnOffObject as Output;
                                    if (onOffObject != null)
                                    {
                                        Guid outputCCUGuid = Outputs.Singleton.GetParentCCU(output.IdOutput);
                                        Guid onOffObjectCCUGuid = Outputs.Singleton.GetParentCCU(onOffObject.IdOutput);

                                        if (outputCCUGuid != Guid.Empty && onOffObjectCCUGuid != Guid.Empty && outputCCUGuid != onOffObjectCCUGuid)
                                        {
                                            SendChangesObject sendChangesObject = new SendChangesObject(onOffObjectCCUGuid, outputCCUGuid, ObjectType.Output, onOffObject.IdOutput);
                                            ControlledObject controlledObject = new ControlledObject(onOffObjectCCUGuid, outputCCUGuid, ObjectType.Output, onOffObject.IdOutput, ObjectType.Output, output.IdOutput);
                                            ObjectBindings objectBindings = new ObjectBindings(outputCCUGuid, controlledObject, onOffObjectCCUGuid, sendChangesObject);

                                            AddToObjectTypeBindings(ObjectType.Output, output.IdOutput, objectBindings);
                                            AddToCCUObjectsForActivation(sendChangesObject, onOffObjectCCUGuid);
                                            AddToCCUControlObjects(controlledObject, outputCCUGuid);

                                            _sendBindingsToCCU[outputCCUGuid] = true;
                                            _sendBindingsToCCU[onOffObjectCCUGuid] = true;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case ObjectType.Input:
                        Input input = Inputs.Singleton.GetById(objectGuid);
                        if (input != null)
                        {
                            DeleteAllBindings(ObjectType.Input, input.IdInput);
                            if (input.BlockingType == (byte)BlockingType.BlockedByObject && input.OnOffObject != null)
                            {
                                if (input.OnOffObject is Input)
                                {
                                    Input onOffObject = (Input)input.OnOffObject;
                                    Guid inputCCUGuid = Inputs.Singleton.GetParentCCU(input.IdInput);
                                    Guid onOffObjectCCUGuid = Inputs.Singleton.GetParentCCU(onOffObject.IdInput);

                                    if (inputCCUGuid != Guid.Empty && onOffObjectCCUGuid != Guid.Empty && inputCCUGuid != onOffObjectCCUGuid)
                                    {
                                        SendChangesObject sendChangesObject = new SendChangesObject(onOffObjectCCUGuid, inputCCUGuid, ObjectType.Input, onOffObject.IdInput);
                                        ControlledObject controlledObject = new ControlledObject(onOffObjectCCUGuid, inputCCUGuid, ObjectType.Input, onOffObject.IdInput, ObjectType.Input, input.IdInput);
                                        ObjectBindings objectBindings = new ObjectBindings(inputCCUGuid, controlledObject, onOffObjectCCUGuid, sendChangesObject);

                                        AddToObjectTypeBindings(ObjectType.Input, input.IdInput, objectBindings);
                                        AddToCCUObjectsForActivation(sendChangesObject, onOffObjectCCUGuid);
                                        AddToCCUControlObjects(controlledObject, inputCCUGuid);

                                        _sendBindingsToCCU[inputCCUGuid] = true;
                                        _sendBindingsToCCU[onOffObjectCCUGuid] = true;
                                    }
                                }
                                else if (input.OnOffObject is Output)
                                {
                                    Output onOffObject = (Output)input.OnOffObject;
                                    Guid inputCCUGuid = Inputs.Singleton.GetParentCCU(input.IdInput);
                                    Guid onOffObjectCCUGuid = Outputs.Singleton.GetParentCCU(onOffObject.IdOutput);

                                    if (inputCCUGuid != Guid.Empty && onOffObjectCCUGuid != Guid.Empty && inputCCUGuid != onOffObjectCCUGuid)
                                    {
                                        SendChangesObject sendChangesObject = new SendChangesObject(onOffObjectCCUGuid, inputCCUGuid, ObjectType.Output, onOffObject.IdOutput);
                                        ControlledObject controlledObject = new ControlledObject(onOffObjectCCUGuid, inputCCUGuid, ObjectType.Output, onOffObject.IdOutput, ObjectType.Input, input.IdInput);
                                        ObjectBindings objectBindings = new ObjectBindings(inputCCUGuid, controlledObject, onOffObjectCCUGuid, sendChangesObject);

                                        AddToObjectTypeBindings(ObjectType.Input, input.IdInput, objectBindings);
                                        AddToCCUObjectsForActivation(sendChangesObject, onOffObjectCCUGuid);
                                        AddToCCUControlObjects(controlledObject, inputCCUGuid);

                                        _sendBindingsToCCU[inputCCUGuid] = true;
                                        _sendBindingsToCCU[onOffObjectCCUGuid] = true;
                                    }
                                }
                            }
                        }
                        break;
                    case ObjectType.AlarmArea:
                        AlarmArea alarmArea = AlarmAreas.Singleton.GetById(objectGuid);
                        if (alarmArea != null)
                        {
                            DeleteAllBindings(ObjectType.AlarmArea, alarmArea.IdAlarmArea);
                            if (alarmArea.ObjForAutomaticAct != null)
                            {
                                if (alarmArea.ObjForAutomaticAct is Input)
                                {
                                    Input onOffObject = alarmArea.ObjForAutomaticAct as Input;
                                    if (onOffObject != null)
                                    {
                                        CCU alarmAreaCCU = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(alarmArea.IdAlarmArea);
                                        if (alarmAreaCCU != null)
                                        {
                                            Guid alarmAreaCCUGuid = alarmAreaCCU.IdCCU;
                                            Guid onOffObjectCCUGuid = Inputs.Singleton.GetParentCCU(onOffObject.IdInput);

                                            if (alarmAreaCCUGuid != Guid.Empty && onOffObjectCCUGuid != Guid.Empty && alarmAreaCCUGuid != onOffObjectCCUGuid)
                                            {
                                                SendChangesObject sendChangesObject = new SendChangesObject(onOffObjectCCUGuid, alarmAreaCCUGuid, ObjectType.Input, onOffObject.IdInput);
                                                ControlledObject controlledObject = new ControlledObject(onOffObjectCCUGuid, alarmAreaCCUGuid, ObjectType.Input, onOffObject.IdInput, ObjectType.AlarmArea, alarmArea.IdAlarmArea);
                                                ObjectBindings objectBindings = new ObjectBindings(alarmAreaCCUGuid, controlledObject, onOffObjectCCUGuid, sendChangesObject);

                                                AddToObjectTypeBindings(ObjectType.AlarmArea, alarmArea.IdAlarmArea, objectBindings);
                                                AddToCCUObjectsForActivation(sendChangesObject, onOffObjectCCUGuid);
                                                AddToCCUControlObjects(controlledObject, alarmAreaCCUGuid);

                                                _sendBindingsToCCU[alarmAreaCCUGuid] = true;
                                                _sendBindingsToCCU[onOffObjectCCUGuid] = true;
                                            }
                                        }
                                    }
                                }
                                else if (alarmArea.ObjForAutomaticAct is Output)
                                {
                                    Output onOffOutput = alarmArea.ObjForAutomaticAct as Output;
                                    if (onOffOutput != null)
                                    {
                                        CCU alarmAreaCCU = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(alarmArea.IdAlarmArea);
                                        if (alarmAreaCCU != null)
                                        {
                                            Guid alarmAreaCCUGuid = alarmAreaCCU.IdCCU;
                                            Guid onOffOutputCCUGuid = Outputs.Singleton.GetParentCCU(onOffOutput.IdOutput);

                                            if (alarmAreaCCUGuid != Guid.Empty && onOffOutputCCUGuid != Guid.Empty && alarmAreaCCUGuid != onOffOutputCCUGuid)
                                            {
                                                SendChangesObject sendChangesObject = new SendChangesObject(onOffOutputCCUGuid, alarmAreaCCUGuid, ObjectType.Output, onOffOutput.IdOutput);
                                                ControlledObject controlledObject = new ControlledObject(onOffOutputCCUGuid, alarmAreaCCUGuid, ObjectType.Output, onOffOutput.IdOutput, ObjectType.AlarmArea, alarmArea.IdAlarmArea);
                                                ObjectBindings objectBindings = new ObjectBindings(alarmAreaCCUGuid, controlledObject, onOffOutputCCUGuid, sendChangesObject);

                                                AddToObjectTypeBindings(ObjectType.AlarmArea, alarmArea.IdAlarmArea, objectBindings);
                                                AddToCCUObjectsForActivation(sendChangesObject, onOffOutputCCUGuid);
                                                AddToCCUControlObjects(controlledObject, alarmAreaCCUGuid);

                                                _sendBindingsToCCU[alarmAreaCCUGuid] = true;
                                                _sendBindingsToCCU[onOffOutputCCUGuid] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }

                if (sendToCCUs)
                    SendBindingsToCCUs();
            }
        }

        private void DeleteAllBindings(ObjectType objectType, Guid objectGuid)
        {
            Dictionary<Guid, ObjectBindings> objectsBindings;
            if (_objectTypesBindings.TryGetValue(objectType, out objectsBindings) && objectsBindings != null)
            {
                ObjectBindings objectBindings;
                if (objectsBindings.TryGetValue(objectGuid, out objectBindings) && objectBindings != null)
                {
                    if (objectBindings.SendChangesObject != null)
                    {
                        List<SendChangesObject> ccuSendChangesObject;
                        if (_sendChangesObjects.TryGetValue(objectBindings.GuidFromCCU, out ccuSendChangesObject) && ccuSendChangesObject != null)
                        {
                            if (ccuSendChangesObject.Contains(objectBindings.SendChangesObject))
                            {
                                ccuSendChangesObject.Remove(objectBindings.SendChangesObject);
                                _sendBindingsToCCU[objectBindings.GuidFromCCU] = true;
                            }
                        }
                    }

                    if (objectBindings.ControlledObject != null)
                    {
                        List<ControlledObject> ccuControlledObjects;
                        if (_controlledObjects.TryGetValue(objectBindings.GuidCCU, out ccuControlledObjects) && ccuControlledObjects != null)
                        {
                            if (ccuControlledObjects.Contains(objectBindings.ControlledObject))
                            {
                                ccuControlledObjects.Remove(objectBindings.ControlledObject);
                                _sendBindingsToCCU[objectBindings.GuidCCU] = true;
                            }
                        }
                    }

                    objectsBindings.Remove(objectGuid);
                }
            }
        }

        private void AddToCCUObjectsForActivation(SendChangesObject sendChangesObject, Guid ccuGuid)
        {
            List<SendChangesObject> ccuSendChangesObject;
            if (!_sendChangesObjects.TryGetValue(ccuGuid, out ccuSendChangesObject))
            {
                ccuSendChangesObject = new List<SendChangesObject>();
                _sendChangesObjects.Add(ccuGuid, ccuSendChangesObject);
            }

            if (ccuSendChangesObject != null)
            {
                if (!ccuSendChangesObject.Contains(sendChangesObject))
                    ccuSendChangesObject.Add(sendChangesObject);
            }
        }

        private void AddToCCUControlObjects(ControlledObject controlledObject, Guid ccuGuid)
        {
            List<ControlledObject> ccuControlledObjects;
            if (!_controlledObjects.TryGetValue(ccuGuid, out ccuControlledObjects))
            {
                ccuControlledObjects = new List<ControlledObject>();
                _controlledObjects.Add(ccuGuid, ccuControlledObjects);
            }

            if (ccuControlledObjects != null)
            {
                if (!ccuControlledObjects.Contains(controlledObject))
                    ccuControlledObjects.Add(controlledObject);
            }
        }

        private void AddToObjectTypeBindings(ObjectType objectType, Guid objectGuid, ObjectBindings objectBindings)
        {
            Dictionary<Guid, ObjectBindings> objectsBindings;
            if (!_objectTypesBindings.TryGetValue(objectType, out objectsBindings))
            {
                objectsBindings = new Dictionary<Guid, ObjectBindings>();
                _objectTypesBindings.Add(objectType, objectsBindings);
            }

            if (objectsBindings != null)
            {
                if (!objectsBindings.ContainsKey(objectGuid))
                    objectsBindings.Add(objectGuid, objectBindings);
            }
        }

        private void SendBindingsToCCUs()
        {
            if (_sendBindingsToCCU != null && _sendBindingsToCCU.Count > 0)
            {
                List<Guid> ccusToSendBindings = new List<Guid>();

                foreach (KeyValuePair<Guid, bool> guidSendBindings in _sendBindingsToCCU)
                {
                    if (guidSendBindings.Value)
                    {
                        ccusToSendBindings.Add(guidSendBindings.Key);
                    }
                }

                if (ccusToSendBindings != null && ccusToSendBindings.Count > 0)
                {
                    foreach (Guid ccuGuid in ccusToSendBindings)
                    {
                        DoSendBindingsToCCU(ccuGuid);
                    }
                }
            }
        }

        public void SendBindingsToCCU(Guid ccuGuid)
        {
            lock (_objectTypesBindings)
            {
                if (_sendBindingsToCCU != null && _sendBindingsToCCU.Count > 0)
                {
                    bool sendBindings;
                    if (_sendBindingsToCCU.TryGetValue(ccuGuid, out sendBindings) && sendBindings)
                    {
                        DoSendBindingsToCCU(ccuGuid);
                    }
                }
            }
        }

        public void MustSendingBindingsToCCU(Guid ccuGuid)
        {
            lock (_objectTypesBindings)
            {
                if (_sendBindingsToCCU != null)
                {
                    if (_sendBindingsToCCU.ContainsKey(ccuGuid))
                    {
                        _sendBindingsToCCU[ccuGuid] = true;
                    }
                    else
                    {
                        _sendBindingsToCCU.Add(ccuGuid, true);
                    }
                }
            }
        }

        private void DoSendBindingsToCCU(Guid ccuGuid)
        {
            try
            {
                List<byte> ccuBindings = new List<byte>();

                List<SendChangesObject> ccuSendChangesObject;
                if (_sendChangesObjects.TryGetValue(ccuGuid, out ccuSendChangesObject) &&
                    ccuSendChangesObject != null && ccuSendChangesObject.Count > 0)
                {
                    foreach (SendChangesObject sendChangesObject in ccuSendChangesObject)
                    {
                        if (sendChangesObject != null)
                        {
                            CCU toCCU = CCUs.Singleton.GetById(sendChangesObject.GuidToCCU);
                            CCU fromCCU = CCUs.Singleton.GetById(sendChangesObject.GuidFromCCU);
                            if (toCCU != null && fromCCU != null)
                            {
                                CCUBinding ccuBinding = CCUBinding.CrateObjectForActiavation(fromCCU.IPAddress, toCCU.IPAddress, (byte)sendChangesObject.ObjectType, sendChangesObject.ObjectGuid);
                                if (ccuBinding != null)
                                {
                                    ccuBindings.AddRange(ccuBinding.ToBytes());
                                }
                            }
                        }
                    }
                }

                List<ControlledObject> ccuControlledObjects;
                if (_controlledObjects.TryGetValue(ccuGuid, out ccuControlledObjects) &&
                    ccuControlledObjects != null && ccuControlledObjects.Count > 0)
                {
                    foreach (ControlledObject controlledObject in ccuControlledObjects)
                    {
                        if (controlledObject != null)
                        {
                            CCU fromCCU = CCUs.Singleton.GetById(controlledObject.GuidFromCCU);
                            CCU toCCU = CCUs.Singleton.GetById(controlledObject.GuidToCCU);
                            if (fromCCU != null && toCCU != null)
                            {
                                switch (controlledObject.ObjectForActivationType)
                                {
                                    case ObjectType.Input:
                                        Input input = Inputs.Singleton.GetById(controlledObject.ObjectForActivationGuid);
                                        if (input != null)
                                        {
                                            byte dcuLogicalAddress = 0;
                                            if (input.DCU != null)
                                                dcuLogicalAddress = input.DCU.LogicalAddress;

                                            CCUBinding ccuBinding = CCUBinding.CrateControlObject(fromCCU.IPAddress, toCCU.IPAddress, InterCCUCommunicationCommand.InputChanged,
                                                dcuLogicalAddress, input.InputNumber, (byte)ObjectType.Input, input.IdInput, (byte)controlledObject.ObjectType,
                                                controlledObject.ObjectGuid);
                                            if (ccuBinding != null)
                                            {
                                                ccuBindings.AddRange(ccuBinding.ToBytes());
                                            }
                                        }
                                        break;
                                    case ObjectType.Output:
                                        Output output = Outputs.Singleton.GetById(controlledObject.ObjectForActivationGuid);
                                        if (output != null)
                                        {
                                            byte dcuLogicalAddress = 0;
                                            if (output.DCU != null)
                                                dcuLogicalAddress = output.DCU.LogicalAddress;

                                            CCUBinding ccuBinding = CCUBinding.CrateControlObject(fromCCU.IPAddress, toCCU.IPAddress, InterCCUCommunicationCommand.OutputChanged,
                                                dcuLogicalAddress, output.OutputNumber, (byte)ObjectType.Output, output.IdOutput, (byte)controlledObject.ObjectType,
                                                controlledObject.ObjectGuid);
                                            if (ccuBinding != null)
                                            {
                                                ccuBindings.AddRange(ccuBinding.ToBytes());
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                if (CCUConfigurationHandler.Singleton.SendBindingsToCCU(ccuGuid, ccuBindings.ToArray()))
                {
                    if (_sendBindingsToCCU.ContainsKey(ccuGuid))
                        _sendBindingsToCCU[ccuGuid] = false;
                }
            }
            catch { }
        }
    }

    public class ObjectBindings
    {
        private Guid _guidCCU;
        private ControlledObject _controlledObject = null;
        private Guid _guidFromCCU;
        private SendChangesObject _sendChangesObject = null;

        public Guid GuidCCU { get { return _guidCCU; } }
        public ControlledObject ControlledObject { get { return _controlledObject; } }
        public Guid GuidFromCCU { get { return _guidFromCCU; } }
        public SendChangesObject SendChangesObject { get { return _sendChangesObject; } }

        public ObjectBindings(Guid guidCCU, ControlledObject controlledObject, Guid guidFromCCU, SendChangesObject sendChangesObject)
        {
            _guidCCU = guidCCU;
            _controlledObject = controlledObject;
            _guidFromCCU = guidFromCCU;
            _sendChangesObject = sendChangesObject;
        }
    }

    public class ControlledObject
    {
        private Guid _guidFromCCU;
        private Guid _guidToCCU;

        private ObjectType _objectForActivationType;
        private Guid _objectForActivationGuid;

        private ObjectType _objectType;
        private Guid _objectGuid;

        public Guid GuidFromCCU { get { return _guidFromCCU; } }
        public Guid GuidToCCU { get { return _guidToCCU; } }

        public ObjectType ObjectForActivationType { get { return _objectForActivationType; } }
        public Guid ObjectForActivationGuid { get { return _objectForActivationGuid; } }

        public ObjectType ObjectType { get { return _objectType; } }
        public Guid ObjectGuid { get { return _objectGuid; } }

        public ControlledObject(Guid guidFromCCU, Guid guidToCCU, ObjectType objectForActivationType, Guid objectForActivationGuid, ObjectType objectType, Guid objectGuid)
        {
            _guidFromCCU = guidFromCCU;
            _guidToCCU = guidToCCU;

            _objectForActivationType = objectForActivationType;
            _objectForActivationGuid = objectForActivationGuid;

            _objectType = objectType;
            _objectGuid = objectGuid;
        }
    }

    public class SendChangesObject
    {
        private Guid _guidFromCCU;
        private Guid _guidToCCU;
        private ObjectType _objectType;
        private Guid _objectGuid;

        public Guid GuidFromCCU { get { return _guidFromCCU; } }
        public Guid GuidToCCU { get { return _guidToCCU; } }
        public ObjectType ObjectType { get { return _objectType; } }
        public Guid ObjectGuid { get { return _objectGuid; } }

        public SendChangesObject(Guid guidFromCCU, Guid guidToCCU, ObjectType objectType, Guid objectGuid)
        {
            _guidFromCCU = guidFromCCU;
            _guidToCCU = guidToCCU;
            _objectType = objectType;
            _objectGuid = objectGuid;
        }
    }
}
