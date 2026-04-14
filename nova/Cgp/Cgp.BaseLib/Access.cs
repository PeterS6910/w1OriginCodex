using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Contal.Cgp.BaseLib
{
    public static class AccessConstans
    {
        public const int NULL_ACCESS_REFERENCE = -1;
        public const string PERFORM_DESCRIPTION = "Perform";
    }

    public class AccessAttribute : Attribute
    {
        public string Source { get; private set; }
        public string Group { get; private set; }
        public int? GeneralAccess { get; private set; }
        public int[] ForcedSetAccesses { get; private set; }
        public int[] ForcedUnsetAccesses { get; private set; }

        public AccessAttribute(string source, string group, int generalAccess)
            : this (source, group, generalAccess, null, null)
        {
        }

        public AccessAttribute(string source, string group, int generalAccess, int[] forcedSetAccesses, int[] forcedUnsetAccesses)
        {
            Source = source;
            Group = group;

            if (generalAccess == AccessConstans.NULL_ACCESS_REFERENCE)
            {
                GeneralAccess = null;
            }
            else
            {
                GeneralAccess = generalAccess;    
            }

            ForcedSetAccesses = forcedSetAccesses;
            ForcedUnsetAccesses = forcedUnsetAccesses;
        }
    }

    public class AccessPresentationAttribute : Attribute
    {
        public string Source { get; private set; }
        public string Group { get; private set; }
        public int? AccessView { get; private set; }
        public string CustomAccessViewDescription { get; private set; }
        public int? AccessAdmin { get; private set; }
        public string CustomAccessAdminDescription { get; private set; }
        public int ViewIndex { get; private set; }

        public AccessPresentationAttribute(
            string source,
            string group,
            int accessView,
            int accessAdmin,
            int viewIndex)
            : this(
                source,
                group,
                accessView,
                null,
                accessAdmin,
                null,
                viewIndex)
        {
        }

        public AccessPresentationAttribute(
            string source,
            string group,
            int accessView,
            string customAccessViewDescription,
            int accessAdmin,
            string customAccessAdminDescription,
            int viewIndex)
        {
            Source = source;
            Group = group;

            if (accessView == AccessConstans.NULL_ACCESS_REFERENCE)
            {
                AccessView = null;
            }
            else
            {
                AccessView = accessView;
            }

            CustomAccessViewDescription = customAccessViewDescription;

            if (accessAdmin == AccessConstans.NULL_ACCESS_REFERENCE)
            {
                AccessAdmin = null;
            }
            else
            {
                AccessAdmin = accessAdmin;
            }

            CustomAccessAdminDescription = customAccessAdminDescription;

            ViewIndex = viewIndex;
        }
    }

    [Serializable]
    public class Access
    {
        private readonly int _accessValue;
        public int AccessValue
        {
            get { return _accessValue; }
        }

        private readonly string _source;
        public string Source
        {
            get { return _source; }
        }

        private readonly string _group;
        public string Group
        {
            get { return _group; }
        }

        private readonly int? _generalAccessValue;
        public int? GeneralAccessValue
        {
            get { return _generalAccessValue; }
        }

        public Access(int accessValue, string source, string group, int? generalAccessValue)
        {
            _accessValue = accessValue;
            _source = source;
            _group = group;
            _generalAccessValue = generalAccessValue;
        }
    }

    [Serializable]
    public class AccessSourceAccessValue
    {
        public string AccessSource { get; private set; }
        public int AccessValue { get; private set; }

        public AccessSourceAccessValue(string accessSource, int accessValue)
        {
            AccessSource = accessSource;
            AccessValue = accessValue;
        }
    }

    [Serializable]
    public class AccessPresentation
    {
        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly string _group;
        public string Group
        {
            get { return _group; }
        }

        private readonly string _source;
        public string Source
        {
            get { return _source; }
        }

        private readonly int? _accessView;
        private readonly int? _generalAccessView;
        private readonly int[] _forcedSetAccessesView;
        private readonly int[] _forcedUnsetAccessesView;
        private readonly string _customAccessViewDescription;
        public string CustomAccessViewDescription
        {
            get { return _customAccessViewDescription; }
        }
        
        private readonly int? _accessAdmin;
        private readonly int? _generalAccessAdmin;
        private readonly int[] _forcedSetAccessesAdmin;
        private readonly int[] _forcedUnsetAccessesAdmin;
        private readonly string _customAccessAdminDescription;
        public string CustomAccessAdminDescription
        {
            get { return _customAccessAdminDescription; }
        }

        public Access ViewAccess
        {
            get
            {
                return _accessView == null
                    ? null
                    : new Access(_accessView.Value, _source, _group, _generalAccessView);
            }
        }

        public Access AdminAccess
        {
            get
            {
                return _accessAdmin == null
                    ? null
                    : new Access(_accessAdmin.Value, _source, _group, _generalAccessAdmin);
            }
        }

        public AccessSourceAccessValue[] ForcedSetAccessesView
        {
            get
            {
                if (_forcedSetAccessesView == null)
                    return null;

                var forcedSetAccessesView = new AccessSourceAccessValue[_forcedSetAccessesView.Length];
                for (int i = 0; i < _forcedSetAccessesView.Length; i++)
                {
                    forcedSetAccessesView[i] = new AccessSourceAccessValue(_source, _forcedSetAccessesView[i]);
                }

                return forcedSetAccessesView;
            }
        }

        public AccessSourceAccessValue[] ForcedSetAccessesAdmin
        {
            get
            {
                if (_forcedSetAccessesAdmin == null)
                    return null;

                var forcedSetAccessesAdmin = new AccessSourceAccessValue[_forcedSetAccessesAdmin.Length];
                for (int i = 0; i < _forcedSetAccessesAdmin.Length; i++)
                {
                    forcedSetAccessesAdmin[i] = new AccessSourceAccessValue(_source, _forcedSetAccessesAdmin[i]);
                }

                return forcedSetAccessesAdmin;
            }
        }

        public AccessSourceAccessValue[] ForcedUnsetAccessesView
        {
            get
            {
                if (_forcedUnsetAccessesView == null)
                    return null;

                var forcedUnsetAccessesView = new AccessSourceAccessValue[_forcedUnsetAccessesView.Length];
                for (int i = 0; i < _forcedUnsetAccessesView.Length; i++)
                {
                    forcedUnsetAccessesView[i] = new AccessSourceAccessValue(_source, _forcedUnsetAccessesView[i]);
                }

                return forcedUnsetAccessesView;
            }
        }

        public AccessSourceAccessValue[] ForcedUnsetAccessesAdmin
        {
            get
            {
                if (_forcedUnsetAccessesAdmin == null)
                    return null;

                var forcedUnsetAccessesAdmin = new AccessSourceAccessValue[_forcedUnsetAccessesAdmin.Length];
                for (int i = 0; i < _forcedUnsetAccessesAdmin.Length; i++)
                {
                    forcedUnsetAccessesAdmin[i] = new AccessSourceAccessValue(_source, _forcedUnsetAccessesAdmin[i]);
                }

                return forcedUnsetAccessesAdmin;
            }
        }

        public int ViewIndex { get; private set; }

        public AccessPresentation(
            string name,
            string source,
            string group,
            int? accessView,
            int? generalAccessView,
            int[] forcedSetAccessesView,
            int[] forcedUnsetAccessesView,
            string customAccessViewDescription,
            int? accessAdmin,
            int? generalAccessAdmin,
            int[] forcedSetAccessesAdmin,
            int[] forcedUnsetAccessesAdmin,
            string customAccessAdminDescription,
            int viewIndex)
        {
            _name = name;
            _source = source;
            _group = group;

            _accessView = accessView;
            _generalAccessView = generalAccessView;
            _forcedSetAccessesView = forcedSetAccessesView;
            _forcedUnsetAccessesView = forcedUnsetAccessesView;
            _customAccessViewDescription = customAccessViewDescription;

            _accessAdmin = accessAdmin;
            _generalAccessAdmin = generalAccessAdmin;
            _forcedSetAccessesAdmin = forcedSetAccessesAdmin;
            _forcedUnsetAccessesAdmin = forcedUnsetAccessesAdmin;
            _customAccessAdminDescription = customAccessAdminDescription;

            ViewIndex = viewIndex;
        }
    }
}
