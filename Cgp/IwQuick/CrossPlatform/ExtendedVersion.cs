using System;

using System.Reflection;
using JetBrains.Annotations;

namespace Contal.IwQuick
{

    /// <summary>
    /// defines what parts of the extended version will be present
    /// in the string report
    /// </summary>
    [Flags]
    public enum VersionDescriptionPart
    {
        /// <summary>
        /// 
        /// </summary>
        Major = 0x0,
        /// <summary>
        /// 
        /// </summary>
        Minor = 0x1,
        /// <summary>
        /// 
        /// </summary>
        Revision = 0x2,
        /// <summary>
        /// 
        /// </summary>
        Suffix = 0x4,
        /// <summary>
        /// 
        /// </summary>
        DevelopmentStage = 0x8,
        /// <summary>
        /// 
        /// </summary>
        Build = 0x10,
        /// <summary>
        /// 
        /// </summary>
        Language = 0x20
    }

    /// <summary>
    /// defines development stage of the assembly
    /// </summary>
    public enum DevelopmentStage
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown,
        /// <summary>
        /// 
        /// </summary>
        Alpha,
        /// <summary>
        /// 
        /// </summary>
        Unstable,
        /// <summary>
        /// 
        /// </summary>
        Beta,
        /// <summary>
        /// 
        /// </summary>
        Testing,
        /// <summary>
        /// 
        /// </summary>
        RC,
        /// <summary>
        /// 
        /// </summary>
        Stable,
        /// <summary>
        /// 
        /// </summary>
        Release,
        /// <summary>
        /// 
        /// </summary>
        Final,
        /// <summary>
        /// 
        /// </summary>
        Special, //fair, non-productional
        /// <summary>
        /// 
        /// </summary>
        NotForCustomer,
        /// <summary>
        /// 
        /// </summary>
        Internal
    }

    /// <summary>
    /// encapsulation class for reporting the version identifiers from assembly's metabase
    /// and additional parameters
    /// </summary>
    [Serializable]
    public class ExtendedVersion
    {
        // ExtendedVersion cannot be child of Version , cause it's sealed, so it encapsulates it
        private readonly Version _version = null;

        private readonly string _versionSuffix = string.Empty;
        private readonly DevelopmentStage _developmentStage = DevelopmentStage.Release;
        private readonly string _preferredLanguage = string.Empty;
        private readonly VersionDescriptionPart _descriptionParts;

        /// <summary>
        /// direct assembly's version
        /// </summary>
        public Version SimpleVersion
        {
            get { return _version; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Version CreateSimple(int major, int minor, int build)
        {
            return new Version(major, minor, build);
        } 

        /// <summary>
        /// major assembly version
        /// </summary>
        public int Major
        {
            get
            {
                return _version.Major;
            }
        }

        /// <summary>
        /// minor assembly version
        /// </summary>
        public int Minor
        {
            get
            {
                return _version.Minor;
            }
        }


        /// <summary>
        /// revision of the assembly; 
        /// usually based on daily number
        /// </summary>
        public int Revision
        {
            get
            {
                return _version.Revision;
            }
        }
        
        /// <summary>
        /// build of the assembly
        /// </summary>
        public int Build
        {
            get
            {
                return _version.Build;
            }   
        }

        /// <summary>
        /// custom version suffix
        /// </summary>
        public string Suffix
        {
            get { return _versionSuffix; }
        }

        /// <summary>
        /// development stage of the assembly
        /// </summary>
        public DevelopmentStage DevelopmentStage
        {
            get { return _developmentStage; }
        }

        /// <summary>
        /// preferred language abbreviation of the assembly
        /// </summary>
        public string Language
        {
            get { return _preferredLanguage; }
        }

        /// <summary>
        /// only usable, if IwQuick library integrated into other solution as project
        /// </summary>
        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly"></param>
        /// <param name="hostingAssembly"></param>
        /// <returns></returns>
        public static Version VersionFromAssembly([CanBeNull] Type anyTypeFromAssembly, Assembly hostingAssembly)
        {
#if !COMPACT_FRAMEWORK
            if (null == anyTypeFromAssembly && null == hostingAssembly)
                throw new ArgumentException("Type, neither assembly is defined");

            Assembly a = null;
            if (null != anyTypeFromAssembly)
                a = Assembly.GetAssembly(anyTypeFromAssembly);

            if (null == a)
                a = hostingAssembly;

            return a.GetName().Version;
#else
            if (null == anyTypeFromAssembly)
                return Assembly.GetExecutingAssembly().GetName().Version;
            
            return anyTypeFromAssembly.Assembly.GetName().Version;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringVersion"></param>
        public ExtendedVersion(string stringVersion)
        {
            // would proceed with the parsing
            var tmpVersion = new Version(stringVersion);

            bool fixNegativeParts = false;

            if (tmpVersion.Major >= 0)
                _descriptionParts = VersionDescriptionPart.Major;
            else
                fixNegativeParts = true;

            if (tmpVersion.Minor >= 0)
                _descriptionParts = VersionDescriptionPart.Minor;
            else
                fixNegativeParts = true;

            if (tmpVersion.Revision >= 0)
                _descriptionParts |= VersionDescriptionPart.Revision;
            else
                fixNegativeParts = true;

            if (tmpVersion.Build >= 0)
                _descriptionParts |= VersionDescriptionPart.Build;
            else
                fixNegativeParts = true;

            if (fixNegativeParts)
                _version =
                    new Version
                        (
                        tmpVersion.Major >= 0 ? tmpVersion.Major : 0,
                        tmpVersion.Minor >= 0 ? tmpVersion.Minor : 0,
                        tmpVersion.Build >= 0 ? tmpVersion.Build : 0,
                        tmpVersion.Revision >= 0 ? tmpVersion.Revision : 0
                        );
            else
                _version = tmpVersion;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly"></param>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        private ExtendedVersion(
            Type anyTypeFromAssembly, 
            Assembly hostingAssembly,
            bool showBuild, 
            bool showRevision, 
            string versionSuffix, 
            DevelopmentStage developmentStage, 
            string preferredLanguage)
        {

            Version assemblyVersion = VersionFromAssembly(anyTypeFromAssembly, hostingAssembly);
            _version = (Version)assemblyVersion.Clone();

            _descriptionParts = VersionDescriptionPart.Major | VersionDescriptionPart.Minor;

            if (showBuild)
                _descriptionParts |= VersionDescriptionPart.Build;

            if (showRevision)
                _descriptionParts |= VersionDescriptionPart.Revision;

            if (Validator.IsNotNullString(versionSuffix))
            {
                _versionSuffix = versionSuffix;
                _descriptionParts |= VersionDescriptionPart.Suffix;
            }

            if (developmentStage != DevelopmentStage.Unknown)
            {
                _developmentStage = developmentStage;
                _descriptionParts |= VersionDescriptionPart.DevelopmentStage;
            }

            if (Validator.IsNotNullString(preferredLanguage))
            {
                _preferredLanguage = preferredLanguage;
                _descriptionParts |= VersionDescriptionPart.Language;
            }
        }

        private ExtendedVersion(int major, int minor, int build, int revision,
            bool showBuild,
            bool showRevision, 
            string versionSuffix, 
            DevelopmentStage developmentStage, 
            string preferredLanguage)
        {
            _version = new Version(major, minor, build, revision);

            _descriptionParts = VersionDescriptionPart.Major | VersionDescriptionPart.Minor;

            if (showBuild)
                _descriptionParts |= VersionDescriptionPart.Build;

            if (showRevision)
                _descriptionParts |= VersionDescriptionPart.Revision;

            if (Validator.IsNotNullString(versionSuffix))
            {
                _versionSuffix = versionSuffix;
                _descriptionParts |= VersionDescriptionPart.Suffix;
            }

            if (developmentStage != DevelopmentStage.Unknown)
            {
                _developmentStage = developmentStage;
                _descriptionParts |= VersionDescriptionPart.DevelopmentStage;
            }

            if (Validator.IsNotNullString(preferredLanguage))
            {
                _preferredLanguage = preferredLanguage;
                _descriptionParts |= VersionDescriptionPart.Language;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        public ExtendedVersion(
            Assembly hostingAssembly, bool showBuild, bool showRevision, string versionSuffix, DevelopmentStage developmentStage, string preferredLanguage)
            : this(null, hostingAssembly, showBuild, showRevision, versionSuffix, developmentStage, preferredLanguage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        public ExtendedVersion(Assembly hostingAssembly, bool showBuild, string versionSuffix, DevelopmentStage developmentStage, string preferredLanguage)
            : this(null,hostingAssembly, showBuild, false, versionSuffix, developmentStage, preferredLanguage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        public ExtendedVersion(int major, int minor, int build, int revision,
            bool showBuild, string versionSuffix, DevelopmentStage developmentStage, string preferredLanguage)
            : this(major, minor, build, revision, showBuild, false, versionSuffix, developmentStage, preferredLanguage)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Assembly hostingAssembly, bool showBuild, string versionSuffix, DevelopmentStage developmentStage)
            : this(null, hostingAssembly, showBuild, false, versionSuffix, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(int major, int minor, int build, int revision,
            bool showBuild, string versionSuffix, DevelopmentStage developmentStage)
            : this(major, minor, build, revision, showBuild, false, versionSuffix, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(Assembly hostingAssembly, bool showBuild, string versionSuffix)
            : this(null, hostingAssembly, showBuild, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(int major, int minor, int build, int revision,
            bool showBuild, string versionSuffix)
            : this(major, minor, build, revision, showBuild, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(Assembly hostingAssembly, string versionSuffix)
            : this(null, hostingAssembly, true, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(int major, int minor, int build, int revision, string versionSuffix)
            : this(major, minor, build, revision, true, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Assembly hostingAssembly, bool showBuild, bool showRevision, DevelopmentStage developmentStage)
            : this(null, hostingAssembly, showBuild, showRevision, null, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(int major, int minor, int build, int revision,
            bool showBuild, bool showRevision, DevelopmentStage developmentStage)
            : this(major, minor, build, revision, showBuild, showRevision, null, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Assembly hostingAssembly, bool showBuild, DevelopmentStage developmentStage)
            : this(null, hostingAssembly, showBuild, false, null, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <param name="revision"></param>
        /// <param name="showBuild"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(int major, int minor, int build, int revision, bool showBuild, DevelopmentStage developmentStage)
            : this(major, minor, build, revision, showBuild, false, null, developmentStage, null)
        {
        }

        //---

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly"></param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        public ExtendedVersion(
           Type anyTypeFromAssembly, bool showBuild, bool showRevision, string versionSuffix, DevelopmentStage developmentStage, string preferredLanguage)
            : this(anyTypeFromAssembly, null, showBuild, showRevision, versionSuffix, developmentStage, preferredLanguage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        /// <param name="preferredLanguage"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, bool showBuild, string versionSuffix, DevelopmentStage developmentStage, string preferredLanguage)
            : this(anyTypeFromAssembly, null, showBuild, false, versionSuffix, developmentStage, preferredLanguage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, bool showBuild, string versionSuffix, DevelopmentStage developmentStage)
            : this(anyTypeFromAssembly, null, showBuild, false, versionSuffix, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="showBuild"></param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, bool showBuild, string versionSuffix)
            : this(anyTypeFromAssembly, null, showBuild, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="versionSuffix"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, string versionSuffix)
            : this(anyTypeFromAssembly, null, true, false, versionSuffix, DevelopmentStage.Unknown, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="showBuild"></param>
        /// <param name="showRevision"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, bool showBuild, bool showRevision, DevelopmentStage developmentStage)
            : this(anyTypeFromAssembly, null, showBuild, showRevision, null, developmentStage, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyTypeFromAssembly">specifies any type from the assembly, from which the version will be extracted</param>
        /// <param name="showBuild"></param>
        /// <param name="developmentStage"></param>
        public ExtendedVersion(Type anyTypeFromAssembly, bool showBuild, DevelopmentStage developmentStage)
            : this(anyTypeFromAssembly, null, showBuild, false, null, developmentStage, null)
        {
        }

        private volatile string _toStringCached = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (null == _toStringCached)
            {
                string report = String.Empty;

                if ((_descriptionParts & VersionDescriptionPart.Major) == VersionDescriptionPart.Major)
                    report += Major;

                if ((_descriptionParts & VersionDescriptionPart.Minor) == VersionDescriptionPart.Minor)
                {
                    if (report != String.Empty)
                        report += StringConstants.DOT;

                    report += Minor;
                }

                if ((_descriptionParts & VersionDescriptionPart.Build) == VersionDescriptionPart.Build)
                {
                    if (report != String.Empty)
                        report += StringConstants.DOT;

                    report += Build;
                }

                if ((_descriptionParts & VersionDescriptionPart.Revision) == VersionDescriptionPart.Revision)
                {
                    if (report != String.Empty)
                        report += StringConstants.DOT;

                    report += Revision;
                }


                if ((_descriptionParts & VersionDescriptionPart.Suffix) == VersionDescriptionPart.Suffix)
                {

                    string strSuffix = _versionSuffix ?? String.Empty;

                    if (strSuffix != String.Empty)
                    {
                        if (report != String.Empty)
                            report += StringConstants.SPACE;

                        report += strSuffix;
                    }
                }



                if ((_descriptionParts & VersionDescriptionPart.DevelopmentStage) ==
                    VersionDescriptionPart.DevelopmentStage)
                {
                    if (report != String.Empty)
                        report += StringConstants.SPACE;

                    report = String.Concat(report, _developmentStage);
                }

                if ((_descriptionParts & VersionDescriptionPart.Language) == VersionDescriptionPart.Language)
                {
                    if (_preferredLanguage != String.Empty)
                    {
                        if (report != String.Empty)
                            report += StringConstants.SPACE;

                        report += _preferredLanguage;
                    }
                }

                _toStringCached = report;
            }



            return _toStringCached;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static implicit operator string(ExtendedVersion version)
        {
            if (ReferenceEquals(version,null))
                return String.Empty;
            return version.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static implicit operator Version(ExtendedVersion version)
        {
            if (ReferenceEquals(version, null))
                return null;
            
            return version.SimpleVersion;
        }

        #region Comparing operators ExtendedVersion <-> Version
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                return (!ReferenceEquals(v2, null));

            if (ReferenceEquals(v2, null))
                return true; // expression (!ReferenceEquals(v1, null)) always true here

            return v1.SimpleVersion != v2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                return (ReferenceEquals(v2, null));

            if (ReferenceEquals(v2, null))
                return false; // expression(ReferenceEquals(v1, null)) always false if reached this point

            return v1.SimpleVersion == v2;
        }

        private const string ERROR_V1_NULL = "v1 cannot be null";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >=(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1,null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2, null))
                return true;

            return v1.SimpleVersion >= v2;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <=(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);


            if (ReferenceEquals(v2, null))
                return false;

            return v1.SimpleVersion <= v2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2, null))
                return false;

            return v1.SimpleVersion < v2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >(ExtendedVersion v1, Version v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2, null))
                return true;

            return v1.SimpleVersion > v2;
        }

        #endregion

        #region Comparing operators ExtendedVersion <-> ExtendedVersion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals(v1, null))
                return (!ReferenceEquals(v2, null));

            if (ReferenceEquals(v2, null))
                return true; // expression (!ReferenceEquals(v1, null)) always true

            return v1.SimpleVersion != v2.SimpleVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals( v1, null))
                return (ReferenceEquals(v2, null));

            if (ReferenceEquals(v2, null))
                return false; // expression (ReferenceEquals(v1, null)) always false if reached this point

            return v1.SimpleVersion == v2.SimpleVersion;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >=(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2, null))
                return true;

            return v1.SimpleVersion >= v2.SimpleVersion;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <=(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);


            if (ReferenceEquals(v2, null))
                return false;

            return v1.SimpleVersion <= v2.SimpleVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2, null))
                return false;

            return v1.SimpleVersion < v2.SimpleVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >(ExtendedVersion v1, ExtendedVersion v2)
        {
            if (ReferenceEquals(v1, null))
                throw new ArgumentNullException(ERROR_V1_NULL);

            if (ReferenceEquals(v2,null))
                return true;

            return v1.SimpleVersion > v2.SimpleVersion;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (obj is ExtendedVersion)
            {
                ExtendedVersion otherEV = (ExtendedVersion) obj;

                if (otherEV.Major != Major)
                    return false;

                if (otherEV.Minor != Minor)
                    return false;

                if (otherEV.Build != Build)
                    return false;

                if (otherEV.Revision != Revision)
                    return false;

                if (DevelopmentStage != DevelopmentStage.Unknown &&
                    otherEV.DevelopmentStage != DevelopmentStage.Unknown &&
                    otherEV.DevelopmentStage != DevelopmentStage)
                    return false;

                return true;
            }
            
// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (obj is Version)
            {
                Version otherV = (Version) obj;
                if (otherV.Major != Major)
                    return false;

                if (otherV.Minor != Minor)
                    return false;

                if (otherV.Build != Build)
                    return false;

                if (otherV.Revision != Revision)
                    return false;

                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            long transcribedVal =
                Major * 100000000000000 + // 100000000000000 = mmmmmbbbbbrrrrrr
                Minor * 10000000000 + // 1000000000 = bbbbbrrrrrr
                Build*100000 + // 100000 = rrrrrr
                Revision;

            return transcribedVal.GetHashCode();
        }

    }
}
