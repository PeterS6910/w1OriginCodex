using System;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.Cgp.NCAS.CCU
{
    internal abstract class AQueuedStateAndSettingsObject<TQueuedStateAndSettingsObject, TDbObject> 
        : AStateAndSettingsObjectBase<TDbObject>
        where TQueuedStateAndSettingsObject : AQueuedStateAndSettingsObject<TQueuedStateAndSettingsObject, TDbObject> 
        where TDbObject : DB.IDbObject
    {
        private abstract class ARequest : WaitableProcessingRequest<TQueuedStateAndSettingsObject>
        {
            protected readonly TDbObject DbObject;

            protected ARequest(TDbObject dbObject)
            {
                DbObject = dbObject;
            }
        }

        private class ConfigureRequest : ARequest
        {
            public ConfigureRequest(TDbObject dbObject)
                : base(dbObject)
            {
            }

            protected override void ExecuteInternal(TQueuedStateAndSettingsObject stateAndSettingsObject)
            {
                stateAndSettingsObject.Configure(DbObject);
            }
        }

        private class UnconfigureRequest : ARequest
        {
            public UnconfigureRequest(TDbObject dbObject)
                : base(dbObject)
            {
            }

            protected override void ExecuteInternal(TQueuedStateAndSettingsObject stateAndSettingsObject)
            {
                stateAndSettingsObject.Unconfigure(DbObject);
            }
        }

        private class ApplyHwSetupOnDcuConnectedRequest : ARequest
        {
            public ApplyHwSetupOnDcuConnectedRequest(TDbObject dbObject)
                : base(dbObject)
            {
            }

            protected override void ExecuteInternal(TQueuedStateAndSettingsObject stateAndSettingsObject)
            {
                stateAndSettingsObject.ApplyHwSetupOnDcuOnline(DbObject);
            }
        }

        private class OnObjectSavedRequest : ARequest
        {
            public OnObjectSavedRequest(TDbObject dbObject)
                : base(dbObject)
            {
            }

            protected override void ExecuteInternal(TQueuedStateAndSettingsObject stateAndSettingsObject)
            {
                stateAndSettingsObject.OnObjectSaved(DbObject);
            }
        }

        private readonly ThreadPoolQueue<WaitableProcessingRequest<TQueuedStateAndSettingsObject>, TQueuedStateAndSettingsObject> _processingQueue;

        private WaitableProcessingRequest<TQueuedStateAndSettingsObject> _configurationRequest;
        private WaitableProcessingRequest<TQueuedStateAndSettingsObject> _unconfigurationRequest;
        private WaitableProcessingRequest<TQueuedStateAndSettingsObject> _applyHwSetupOnDcuConnectedRequest;
        private WaitableProcessingRequest<TQueuedStateAndSettingsObject> _onObjectSavedRequest;

        protected abstract TQueuedStateAndSettingsObject This
        {
            get;
        }

        protected AQueuedStateAndSettingsObject(Guid id) : base(id)
        {
            _processingQueue =
                new ThreadPoolQueue<WaitableProcessingRequest<TQueuedStateAndSettingsObject>, TQueuedStateAndSettingsObject>(
                    ThreadPoolGetter.Get(),
                    This);
        }

        public override void ConfigureStart(TDbObject dbObject)
        {
            _configurationRequest = new ConfigureRequest(dbObject);

            _configurationRequest.EnqueueSync(_processingQueue);
        }

        public override void FinishConfiguration()
        {
            _configurationRequest.WaitForCompletion();
            _configurationRequest = null;
        }

        public override void ApplyHwSetupOnDcuOnlineStart(TDbObject dbObject)
        {
            _applyHwSetupOnDcuConnectedRequest = new ApplyHwSetupOnDcuConnectedRequest(dbObject);

            _applyHwSetupOnDcuConnectedRequest.EnqueueSync(_processingQueue);
        }

        public override void FinishApplyHwSetupOnDcuOnline()
        {
            _applyHwSetupOnDcuConnectedRequest.WaitForCompletion();
            _applyHwSetupOnDcuConnectedRequest = null;
        }

        public override void UnconfigureStart(TDbObject newDbObject)
        {
            _unconfigurationRequest = new UnconfigureRequest(newDbObject);

            _unconfigurationRequest.EnqueueSync(_processingQueue);
        }

        public override void FinishUnconfiguration(bool removed)
        {
            _unconfigurationRequest.WaitForCompletion();
            _unconfigurationRequest = null;

            if (removed)
            {
                _processingQueue.Dispose();
            }
        }

        public override void StartOnObjectSaved(TDbObject newDbObject)
        {
            _onObjectSavedRequest = new OnObjectSavedRequest(newDbObject);

            _onObjectSavedRequest.EnqueueSync(_processingQueue);
        }

        public override void FinishOnObjectSaved()
        {
            _onObjectSavedRequest.WaitForCompletion();
            _onObjectSavedRequest = null;
        }

        public void EnqueueSyncRequest(WaitableProcessingRequest<TQueuedStateAndSettingsObject> request)
        {
            request.EnqueueSync(_processingQueue);
        }

        public void EnqueueAsyncRequest(WaitableProcessingRequest<TQueuedStateAndSettingsObject> request)
        {
            request.EnqueueAsync(_processingQueue);
        }
    }
}
