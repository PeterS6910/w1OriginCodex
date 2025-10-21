using System;
using System.Diagnostics;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Server.DB
{
    public sealed class CentralTransactionControl : ASingleton<CentralTransactionControl>
    {
        private readonly SyncDictionary<IdAndObjectType, ObjectInControl> _listEditingObject =
            new SyncDictionary<IdAndObjectType, ObjectInControl>();

        private CentralTransactionControl() : base(null)
        {
        }

        public void SessionTimeOut(object sessionId)
        {
            _listEditingObject.RemoveWhere(
                (key, value) => value.RemoveFromClientList(sessionId));
        }

        /// <summary>
        /// Obejct for editing was read from database
        /// </summary>
        /// <param name="guidAndObjectType"></param>
        /// <summary>
        /// Obejct for editing was read from database
        /// </summary>
        public void EditStart(IdAndObjectType guidAndObjectType)
        {
            ObjectInControl editingObjectInControl;

            _listEditingObject.GetOrAddValue(
                guidAndObjectType,
                out editingObjectInControl,
                key => new ObjectInControl(),
                null);

            editingObjectInControl.EditStart(RemotingSessionHandler.CallingSessionId);
        }

        /// <summary>
        /// Renew old editing object by setting its version to 
        /// </summary>
        /// <param name="idAndObjectType">Id of editing object</param>
        /// <returns>Return false if editing of object was not started. Else return true.</returns>
        public bool RenewEditingObject(IdAndObjectType idAndObjectType)
        {
            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(
                idAndObjectType,
                out editingObjectInControl))
            {
                return false;
            }

            editingObjectInControl.EditStart(RemotingSessionHandler.CallingSessionId);
            
            return true;
        }

        /// <summary>
        /// This function check if edited object was changed since EditStart was called.
        /// </summary>
        /// <param name="guidAndObjectType"></param>
        /// <returns>Return true if object was not changed.</returns>
        public bool BeginUpdate(IdAndObjectType guidAndObjectType)
        {
            string sessionId = RemotingSessionHandler.CallingSessionId;

            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(guidAndObjectType, out editingObjectInControl))
                return false;

            if (editingObjectInControl.BeginUpdate(sessionId) &&
                _listEditingObject.ContainsKey(guidAndObjectType))
            {
                return true;
            }

            editingObjectInControl.EndUpdate(
                false,
                sessionId);

            return false;
        }

        /// <summary>
        /// This method is called after saving object to database.
        /// </summary>
        /// <param name="editingObject"></param>
        /// <param name="successful"></param>
        public void EndUpdate(IdAndObjectType guidAndObjectType, bool successful)
        {
            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(
                guidAndObjectType,
                out editingObjectInControl))
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                throw new InvalidOperationException();
            }

            editingObjectInControl.EndUpdate(
                successful,
                RemotingSessionHandler.CallingSessionId);
        }

		/// <summary>
        /// This method is called when user close form or server finish object updating
        /// </summary>
        /// <param name="editingObject"></param>        public void EditEnd(IdAndObjectType guidAndObjectType)
        /// <summary>
        /// This method is called when user close form or server finish object updating
        /// </summary>
        /// <param name="editingObject"></param>
        public void EditEnd(IdAndObjectType guidAndObjectType)
        {
            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(guidAndObjectType, out editingObjectInControl))
                return;

            if (editingObjectInControl.EditEnd(RemotingSessionHandler.CallingSessionId))
                _listEditingObject.Remove(guidAndObjectType);
        }

        /// <summary>
        /// This method check if internal dictionary contains editing object.
        /// </summary>
        /// <param name="guidAndObjectType"></param>
        /// <returns>Return true if calling of EndDelete is required after deleting of edited object.</returns>
        public bool BeginDelete(IdAndObjectType guidAndObjectType)
        {
            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(guidAndObjectType, out editingObjectInControl))
                return false;

            editingObjectInControl.Lock();

            if (!_listEditingObject.ContainsKey(guidAndObjectType))
            {
                editingObjectInControl.Unlock();
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method remove all resources used for locking of edited and deleted object
        /// </summary>
        /// <param name="guidAndObjectType"></param>
        /// <param name="successful"></param>
        public void EndDelete(IdAndObjectType guidAndObjectType, bool successful)
        {
            ObjectInControl editingObjectInControl;

            if (!_listEditingObject.TryGetValue(
                guidAndObjectType,
                out editingObjectInControl))
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                throw new InvalidOperationException();
            }

            if (successful)
                _listEditingObject.Remove(guidAndObjectType);

            editingObjectInControl.Unlock();
        }
    }
}
