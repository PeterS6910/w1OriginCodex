using System.Collections.Generic;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class GlobalEvaluator
    {
        public class LocalEvaluatorManagerType
        {
            private readonly IDictionary<string, ILocalEvaluator> _localEvaluators =
                new Dictionary<string, ILocalEvaluator>();

            private readonly GlobalEvaluator _globalEvaluator;

            public LocalEvaluatorManagerType(GlobalEvaluator globalEvaluator)
            {
                _globalEvaluator = globalEvaluator;
            }

            public ILocalEvaluator GetLocalEvaluator(
                IStructuredSiteBuilderClient structuredSiteBuilderClient,
                Login login)
            {
                _globalEvaluator._accessLock.EnterWriteLock();

                try
                {
                    var username = login.Username;

                    ILocalEvaluator localEvaluator;
                    if (_localEvaluators.TryGetValue(username, out localEvaluator))
                    {
                        localEvaluator.SetClient(structuredSiteBuilderClient);
                        return localEvaluator;
                    }

                    localEvaluator =
                        new LocalEvaluator(
                            _globalEvaluator,
                            login,
                            structuredSiteBuilderClient);

                    _localEvaluators.Add(
                        username,
                        localEvaluator);

                    _globalEvaluator.ObjectAdded += localEvaluator.OnObjectAdded;
                    _globalEvaluator.ObjectDeleted += localEvaluator.OnObjectDeleted;
                    _globalEvaluator.ObjectUpdated += localEvaluator.OnObjectUpdated;

                    _globalEvaluator.ObjectMoved += localEvaluator.OnObjectMoved;
                    _globalEvaluator.ObjectParentChanged += localEvaluator.OnObjectParentChanged;
                    _globalEvaluator.ObjectRenamed += localEvaluator.OnObjectRenamed;

                    _globalEvaluator.SubTreeReferenceAdded += localEvaluator.OnSubTreeReferenceAdded;
                    _globalEvaluator.SubTreeReferenceRemoved += localEvaluator.OnSubTreeReferenceRemoved;

                    _globalEvaluator.DirectBrokenReferencesChanged += localEvaluator.OnDirectBrokenReferencesChanged;

                    _globalEvaluator.SiteCreated += localEvaluator.OnSiteCreated;
                    _globalEvaluator.SiteUpdated += localEvaluator.OnSiteUpdated;
                    _globalEvaluator.SiteRemoved += localEvaluator.OnSiteRemoved;

                    _globalEvaluator.BeginUpdate += localEvaluator.OnBeginUpdate;
                    _globalEvaluator.EndUpdate += localEvaluator.OnEndUpdate;

                    _globalEvaluator.UserFoldersStructureChanged += localEvaluator.OnUserFoldersStructureChanged;

                    return localEvaluator;
                }
                finally
                {
                    _globalEvaluator._accessLock.ExitWriteLock();
                }
            }

            public void RemoveLocalEvaluator(
                GlobalEvaluator globalEvaluator,
                string username)
            {
                globalEvaluator._accessLock.EnterWriteLock();

                try
                {
                    ILocalEvaluator localEvaluator;
                    if (!_localEvaluators.TryGetValue(username, out localEvaluator))
                        return;

                    try
                    {
                        _globalEvaluator.ObjectAdded -= localEvaluator.OnObjectAdded;
                        _globalEvaluator.ObjectDeleted -= localEvaluator.OnObjectDeleted;
                        _globalEvaluator.ObjectUpdated -= localEvaluator.OnObjectUpdated;
                        _globalEvaluator.ObjectMoved -= localEvaluator.OnObjectMoved;

                        _globalEvaluator.ObjectParentChanged -= localEvaluator.OnObjectParentChanged;
                        _globalEvaluator.ObjectRenamed -= localEvaluator.OnObjectRenamed;

                        _globalEvaluator.SubTreeReferenceAdded -= localEvaluator.OnSubTreeReferenceAdded;
                        _globalEvaluator.SubTreeReferenceRemoved -= localEvaluator.OnSubTreeReferenceRemoved;

                        _globalEvaluator.DirectBrokenReferencesChanged -= localEvaluator.OnDirectBrokenReferencesChanged;

                        _globalEvaluator.SiteCreated -= localEvaluator.OnSiteCreated;
                        _globalEvaluator.SiteUpdated -= localEvaluator.OnSiteUpdated;
                        _globalEvaluator.SiteRemoved -= localEvaluator.OnSiteRemoved;

                        _globalEvaluator.BeginUpdate -= localEvaluator.OnBeginUpdate;
                        _globalEvaluator.EndUpdate -= localEvaluator.OnEndUpdate;

                        _globalEvaluator.UserFoldersStructureChanged -= localEvaluator.OnUserFoldersStructureChanged;
                    }
                    finally
                    {
                        _localEvaluators.Remove(username);
                    }
                }
                finally
                {
                    globalEvaluator._accessLock.ExitWriteLock();
                }
            }

            public void OnBeginSave()
            {
                _globalEvaluator._accessLock.EnterWriteLock();

                foreach (var localEvaluator in _localEvaluators.Values)
                    localEvaluator.OnBeginSave();
            }

            public void OnEndSave()
            {
                foreach (var localEvaluator in _localEvaluators.Values)
                    localEvaluator.OnEndSave();

                _globalEvaluator._accessLock.ExitWriteLock();
            }
        }
    }
}