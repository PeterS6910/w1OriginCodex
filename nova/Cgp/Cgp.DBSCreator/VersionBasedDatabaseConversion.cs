using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.Cgp.ORM;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;
using Contal.IwQuick.Crypto;

namespace Contal.Cgp.DBSCreator
{
    public class VersionBasedDatabaseConversion
    {
        private const string CGP_SERVER_BEANS_ASSEMBLY_NAME = "Cgp.Server.Beans.dll";
        private const string CGP_SERVER_BEANS_EXTERN_ASSEMBLY_NAME = "Cgp.Server.Beans.Extern.dll";
        private const string CGP_NCAS_SERVER_BEANS_ASSEMBLY_NAME = "Cgp.NCAS.Server.Beans.dll";

        private readonly CreatorProperties _creatorProperties;
        private readonly DatabaseCommandExecutor _databaseCommandExecutor;

        public event Action<string, bool> ShowMessage;

        public VersionBasedDatabaseConversion(
            DatabaseCommandExecutor databaseCommandExecutor)
        {
            _creatorProperties = databaseCommandExecutor.CreatorProperties;

            _databaseCommandExecutor = databaseCommandExecutor;
        }

        private void WriteLogToEdit(string text, bool translate)
        {
            if (ShowMessage == null)
                return;

            try
            {
                ShowMessage(text, translate);
            }
            catch
            {
            }
        }

        private void WriteLogToEdit(string text)
        {
            WriteLogToEdit(text, true);
        }

        public bool Execute()
        {
            bool resultMainDatabase =
                Execute(
                    _creatorProperties.Assemblies, false);

            bool resultExternDatabase = 
                Execute(
                    _creatorProperties.AssembliesEventlogDatabase, true);

            return resultMainDatabase && resultExternDatabase;
        }

        private bool Execute(
            IEnumerable<Assembly> assemblies,
            bool externDatabase)
        {
            bool result = true;

            foreach (Assembly assembly in assemblies)
            {
                var assemblyName = assembly.ManifestModule.Name;

                if (Execute(
                    assemblyName,
                    _databaseCommandExecutor.GetVersionFromDatabase(
                        assemblyName,
                        externDatabase),
                    conversionVersion =>
                        _databaseCommandExecutor.SaveVersionToDatabase(
                            assemblyName,
                            conversionVersion,
                            externDatabase)))
                {
                    _databaseCommandExecutor.SaveVersionToDatabase(
                        assembly,
                        externDatabase);
                }
                else
                    result = false;
            }

            return result;
        }

        private bool Execute(
            string assemblyName,
            string versionInDatabase,
            Action<string> saveVersionToDabase)
        {
            double version;

            if (string.IsNullOrEmpty(versionInDatabase) ||
                !Double.TryParse(
                    versionInDatabase,
                    System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out version))
            {
                switch (assemblyName)
                {
                    case CGP_SERVER_BEANS_EXTERN_ASSEMBLY_NAME:

                        WriteLogToEdit("ErrorTableSystemVersionInEventlogDatabaseCorrupted");
                        break;

                    default:

                        WriteLogToEdit("ErrorTableSystemVersionInConfigurationDatabaseCorrupted");
                        break;
                }

                return false;
            }

            switch (assemblyName)
            {
                case CGP_SERVER_BEANS_ASSEMBLY_NAME:
                    return DoConvertServerBeans(
                        version,
                        saveVersionToDabase);

                case CGP_SERVER_BEANS_EXTERN_ASSEMBLY_NAME:
                    return DoConvertServerExternBeans(
                        version,
                        saveVersionToDabase);

                case CGP_NCAS_SERVER_BEANS_ASSEMBLY_NAME:
                    return DoConvertNCASServerBeans(
                        version,
                        saveVersionToDabase);

                default:
                    return true;
            }
        }

        public delegate bool VersionUpdateDelegate(out Exception error);

        public bool DoConvertToVersion(
            double version, 
            double routineVersion, 
            VersionUpdateDelegate routine, 
            bool failureRecoverable, 
            string conversionTypeString,
            Action<string> saveVersionToDatabase)
        {
            if (version >= routineVersion)
               return true;

            string routineVersionString = routineVersion.ToString(System.Globalization.CultureInfo.InvariantCulture);
                
            WriteLogToEdit(
                string.Format(
                    "{0} {1}",
                    conversionTypeString,
                    routineVersionString),
                false);

            Exception error;

            if (routine(out error))
            {
                WriteLogToEdit(
                    string.Format(
                        "{0} {1} {2}",
                        conversionTypeString,
                        routineVersionString,
                        GetString("cgpCorrectionPartSucceeded")),
                    false);

                saveVersionToDatabase(routineVersionString);

                return true;
            }

            WriteLogToEdit(
                string.Format(
                    "{0} {1} {2}",
                    conversionTypeString,
                    routineVersionString,
                    GetString("cgpCorrectionPartFailed")),
                false);

            if (error != null)
                WriteLogToEdit(
                    string.Format("Exception: {0}", error.Message),
                    false);

            return
                failureRecoverable &&
                Dialog.ErrorQuestion(
                    string.Format(
                        "{0} {1} {2}. {3} {4}",
                        conversionTypeString,
                        routineVersionString,
                        GetString("cgpCorrectionPartFailed"),
                        GetString("cgpCorrectionContinueAnyway"),
                        GetString("cgpCorrectionDatabaseHasCorrectStructure")));
        }

        private bool DoConvertServerBeans(
            double version,
            Action<string> saveVersionToDabase)
        {
            // Cgp.Server.Beans
            if (version < 1.0)
            {
                WriteLogToEdit("cgpCorrection1");
                Exception error;

                if (ConversionCgpServerBeans1(out error))
                    WriteLogToEdit("cgpCorrection1success");
                else
                {
                    WriteLogToEdit("cgpCorrection1fail");

                    if (error != null)
                        WriteLogToEdit(
                            string.Format("Exception: {0}", error.Message),
                            false);

                    return false;
                }
            }

            string conversionTypeString = GetString("cgpCorrectionPart");

            if (!DoConvertToVersion(
                version,
                1.5,
                ConversionCgpServerBeans1_5,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.6,
                ConversionCgpServerBeans1_6,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.8,
                ConversionCgpServerBeans1_8,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.9,
                ConversionCgpServerBeans1_9,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.91,
                ConversionCgpServerBeans1_91,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.92,
                ConversionCgpServerBeans1_92,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.93,
                ConversionCgpServerBeans1_93,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.94,
                ConversionCgpServerBeans1_94,
                true,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.95,
                ConversionCgpServerBeans1_95,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.96,
                ConversionCgpServerBeans1_96,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(version,
                1.97,
                ConversionCgpServerBeans1_97,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(version,
                1.98,
                ConversionCgpServerBeans1_98,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(version,
                2.10,
                ConversionCgpServerBeans2_10,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(version,
                2.11,
                ConversionCgpServerBeans2_11,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(version,
                2.12,
                ConversionCgpServerBeans2_12,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            return true;
        }

        // Cgp.Server.Beans.Extern
        private bool DoConvertServerExternBeans(
            double version,
            Action<string> saveVersionToDabase)
        {
            string conversionTypeString = GetString("cgpEventlogCorrectionPart");

            if (!DoConvertToVersion(
                version,
                1.91,
                ConversionCgpServerBeansExtern1_91,
                true,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.92,
                ConversionCgpServerBeansExtern1_92,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.93,
                ConversionCgpServerBeansExtern1_93,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.94,
                ConversionCgpServerBeansExtern1_94,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.95,
                ConversionCgpServerBeansExtern1_95,
                true,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.96,
                ConversionCgpServerBeansExtern1_96,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.97,
                ConversionCgpServerBeansExtern1_97,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.00,
                ConversionCgpServerBeansExtern2_00,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            return true;
        }

        private bool DoConvertNCASServerBeans(
            double version,
            Action<string> saveVersionToDabase)
        {
            string conversionTypeString = GetString("cgpNCASCorrectionPart");

            if (!DoConvertToVersion(
                    version,
                    1.1,
                    ConversionCgpNCASServerBeans1_1,
                    false,
                    conversionTypeString,
                    saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.2,
                ConversionCgpNCASServerBeans1_2,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }


            if (!DoConvertToVersion(
                version,
                1.3,
                ConversionCgpNCASServerBeans1_3,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.4,
                ConversionCgpNCASServerBeans1_4,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.5,
                ConversionCgpNCASServerBeans1_5,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.9,
                ConversionCgpNCASServerBeans1_9,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.91,
                ConversionCgpNCASServerBeans1_91,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.92,
                ConversionCgpNCASServerBeans1_92,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.93,
                ConversionCgpNCASServerBeans1_93,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.94,
                ConversionCgpNCASServerBeans1_94,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.95,
                ConversionCgpNCASServerBeans1_95,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                1.96,
                ConversionCgpNCASServerBeans1_96,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.01,
                ConversionCgpNCASServerBeans2_01,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.02,
                ConversionCgpNCASServerBeans2_02,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.03,
                ConversionCgpNCASServerBeans2_03,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.04,
                ConversionCgpNCASServerBeans2_04,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.05,
                ConversionCgpNCASServerBeans2_05,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.06,
                ConversionCgpNCASServerBeans2_06,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.07,
                ConversionCgpNCASServerBeans2_07,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.08,
                ConversionCgpNCASServerBeans2_08,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.09,
                ConversionCgpNCASServerBeans2_09,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.10,
                ConversionCgpNCASServerBeans2_10,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.11,
                ConversionCgpNCASServerBeans2_11,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.12,
                ConversionCgpNCASServerBeans2_12,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.13,
                ConversionCgpNCASServerBeans2_13,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.14,
                ConversionCgpNCASServerBeans2_14,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.15,
                ConversionCgpNCASServerBeans2_15,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.16,
                ConversionCgpNCASServerBeans2_16,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.17,
                ConversionCgpNCASServerBeans2_17,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.18,
                ConversionCgpNCASServerBeans2_18,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.19,
                ConversionCgpNCASServerBeans2_19,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.20,
                ConversionCgpNCASServerBeans2_20,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.21,
                ConversionCgpNCASServerBeans2_21,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.22,
                ConversionCgpNCASServerBeans2_22,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            if (!DoConvertToVersion(
                version,
                2.23,
                ConversionCgpNCASServerBeans2_23,
                false,
                conversionTypeString,
                saveVersionToDabase))
            {
                return false;
            }

            return true;
        }

        private string GetString(string text)
        {
            return _databaseCommandExecutor.LocalizationHelper.GetString(text);
        }

        private bool ConversionCgpServerBeans1(out Exception error)
        {
            try
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "drop table systemversion", 
                        false, 
                        out error))
                    return false;

                try
                {
                    NhHelper.Singleton.ConnectionString =
                        _creatorProperties.GetConnectionString();

                    NhHelper.Singleton.UpdateSchema(_creatorProperties.Assemblies);

                    return true;
                }
                catch (Exception ex)
                {
                    Dialog.Error(ex.ToString());

                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_5(out Exception error)
        {
            try
            {
                string command = "Update Card set FullCardNumber = (select FullCardNumber = CASE CardSubType" +
                    " when 0 then '0' + CompanyCode + Number" +
                    " when 1 then '1' + CompanyCode + Number" +
                    " when 2 then '20' + CompanyCode + Number" +
                    " when 4 then '40' + CompanyCode + Number" +
                    " when 5 then '41' + CompanyCode + Number" +
                    " when 6 then '42' + CompanyCode + Number" +
                    " when 7 then '43' + CompanyCode + Number" +
                    " when 8 then '44' + CompanyCode + Number" +
                    " when 9 then '45' + CompanyCode + Number" +
                    " when 10 then '46' + CompanyCode + Number" +
                    " when 11 then '47' + CompanyCode + Number" +
                    " when 12 then '48' + CompanyCode + Number" +
                    " when 13 then '49' + CompanyCode + Number" +
                    " when 14 then '4A' + CompanyCode + Number" +
                    " when 15 then '4B' + CompanyCode + Number" +
                    " else CompanyCode + Number" +
                    " END" +
                    " from CardSystem where IdCardSystem = CardSystem)" +
                    " where CardSystem is not null";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command,
                        false,
                        out error))
                    return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "Update Card set FullCardNumber = Number where CardSystem is null",
                        false,
                        out error))
                    return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "Alter table Card alter column FullCardNumber nvarchar(255) NOT NULL",
                        false,
                        out error))
                    return false;

                command = "if not exists ( select * from INFORMATION_SCHEMA.COLUMNS" +
                    " where TABLE_NAME='CardSystem' and COLUMN_NAME='CardSystemNumber' )" +
                    " alter table CardSystem add CardSystemNumber tinyint ";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command,
                        false,
                        out error))
                    return false;

                command = "DECLARE @counter tinyint" +
                    " SET @counter = (select count(*) from CardSystem)" +
                    " SET @counter = @counter + 10" +
                    " UPDATE CardSystem" +
                    " SET @counter = CardSystemNumber = @counter + 1" +
                    " where CardSystemNumber is null";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command,
                        false,
                        out error))
                    return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE CardSystem ALTER COLUMN CardSystemNumber tinyint NOT NULL",
                        false,
                        out error))
                    return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE CardSystem ADD UNIQUE (CardSystemNumber)",
                        false,
                        out error))
                    return false;

                string[] droppedColumns =
                {
                    "Bank1Sector",
                    "Bank1Offset",
                    "Bank1Length",
                    "Bank2Sector",
                    "Bank2Offset",
                    "Bank2Length",
                    "Bank3Sector",
                    "Bank3Offset",
                    "Bank3Length"
                };

                foreach (var droppedColumn in droppedColumns)
                    if (!_databaseCommandExecutor.SqlDropColumn(
                            "CardSystem", droppedColumn,
                            false,
                            out error))
                        return false;

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_6(out Exception error)
        {
            try
            {
                if (!_databaseCommandExecutor.SqlDropColumn("CardSystem", "Bank1Encoding", false, out error))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("CardSystem", "Bank2Encoding", false, out error))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("CardSystem", "Bank3Encoding", false, out error))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("Login", "LockClientAplication", false, out error))
                    return false;

                var command = new StringBuilder();

                command.Append("declare @TitleLength int ");
                command.Append(" select @TitleLength = CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS");
                command.Append(" where TABLE_NAME='Person' and COLUMN_NAME='Tiltle'");
                command.Append(" if @TitleLength != 30");
                command.Append(" ALTER TABLE Person ALTER COLUMN Tiltle varchar(30)");

                return
                    _databaseCommandExecutor.RunSqlNonQuery(
                        command.ToString(),
                        false,
                        out error) &&
                    _databaseCommandExecutor.SqlDropColumn(
                        "CardReader",
                        "AllowCardAndPIN",
                        false,
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_8(out Exception error)
        {
            try
            {
                string command = "select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='GlobalAlarmInstruction' and COLUMN_NAME='Instuctions'";

                if (_databaseCommandExecutor.RowExists(command, false))
                {
                    command = "UPDATE GlobalAlarmInstruction set Instructions = Instuctions";

                    if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                        return false;
                }

                return
                    _databaseCommandExecutor.SqlDropColumn(
                        "GlobalAlarmInstruction",
                        "Instuctions",
                        false,
                        out error) &&
                    _databaseCommandExecutor.SqlDropColumn(
                        "GlobalAlarmInstruction",
                        "Description",
                        false,
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_9(out Exception error)
        {
            try
            {
                string command = "UPDATE Person set FirstName = (select Name from CentralNameRegister where CentralNameRegister.Id = Person.IdPerson) where FirstName is NULL or FirstName = ''";
                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                command = "UPDATE Person set Surname = (select AlternateName from CentralNameRegister where CentralNameRegister.Id = Person.IdPerson) where Surname is NULL or Surname = ''";
                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                command = "UPDATE CentralNameRegister set Name = Name + ' ' + AlternateName where objectType=27 and AlternateName is not NULL and AlternateName != '%PERSON_CONV1%'";
                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                command = "UPDATE CentralNameRegister set AlternateName='%PERSON_CONV1%' where objectType=27";
                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                command = "ALTER TABLE Person ALTER COLUMN FirstName nvarchar(225) NOT NULL";

                return _databaseCommandExecutor.RunSqlNonQuery(command, false, out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_91(out Exception error)
        {
            error = null;

            byte[] bank1AKey = null;
            byte[] bank1BKey = null;

            byte[] bank2AKey = null;
            byte[] bank2BKey = null;

            byte[] bank3AKey = null;
            byte[] bank3BKey = null;

            foreach (int cardSybType in new[] { 4, 5 })
            {
                string command = "Select IdCardSystem, AID, Akey, Bkey, Encoding, CypherData from CardSystem where CardType='1' and CardSubType='" + cardSybType + "'";

                Exception innerError;

                IList<object[]> mifareResults = _databaseCommandExecutor.RunSqlQuery(command, false, out innerError);

                if (mifareResults == null || mifareResults.Count == 0 || mifareResults[0].Length == 0)
                    continue;

                foreach (object[] mifareResult in mifareResults)
                {
                    command = "Select AKeyBank1, AKeyBank2, AKeyBank3, BKeyBank1, BKeyBank2, BKeyBank3 from CardSystem where IdCardSystem='" + mifareResult[0] + "'";

                    IList<object[]> sectorResults = _databaseCommandExecutor.RunSqlQuery(command, false, out innerError);

                    if (sectorResults != null && sectorResults.Count != 0 && sectorResults[0].Length > 0)
                    {
                        object[] sectorResult = sectorResults[0];

                        bank1AKey = (byte[])sectorResult[0];
                        bank2AKey = (byte[])sectorResult[1];
                        bank3AKey = (byte[])sectorResult[2];
                        bank1BKey = (byte[])sectorResult[3];
                        bank2BKey = (byte[])sectorResult[4];
                        bank3BKey = (byte[])sectorResult[5];
                    }

                    var cypherData = mifareResult[5] as byte[];
                    if (cypherData == null || cypherData.Length == 0)
                        continue;

                    byte bank1Length = cypherData[3];
                    byte bank2Length = cypherData[4];
                    byte bank3Length = cypherData[5];

                    int bank1Sector;
                    int bank1Offset;
                    int bank2Sector;
                    int bank2Offset;
                    int bank3Sector;
                    int bank3Offset;

                    try
                    {
                        var cryptoXtea = new IwQuick.Crypto.XTEA();

                        cryptoXtea.XTeaInit();

                        var sendToDecript = new byte[cypherData.Length - 6];

                        Array.Copy(cypherData, 6, sendToDecript, 0, cypherData.Length - 6);

                        byte[] decrypted = cryptoXtea.XTeaFrameDec(sendToDecript);

                        if (decrypted == null || decrypted.Length == 0)
                            continue;

                        bank1Sector = decrypted[0];
                        bank1Offset = decrypted[2];
                        bank2Sector = decrypted[4];
                        bank2Offset = decrypted[6];
                        bank3Sector = decrypted[8];
                        bank3Offset = decrypted[10];
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                        HandledExceptionAdapter.Examine(ex);
                        return false;
                    }

                    command =
                        "Insert into MifareSectorData " +
                        "values(@Id,@Aid,@GeneralAKey,@GeneralBKey,@Encoding,@CypherData, @SizeKB)";

                    int insertedCount =
                        _databaseCommandExecutor.RunSqlNonQueryWithParameters(
                            command,
                            false,
                            new SqlParameterTypeAndValue(
                                SqlDbType.UniqueIdentifier,
                                mifareResult[0]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.NVarChar,
                                mifareResult[1]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.Binary,
                                mifareResult[2]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.Binary,
                                mifareResult[3]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.TinyInt,
                                mifareResult[4]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.Binary,
                                mifareResult[5]),
                            new SqlParameterTypeAndValue(
                                SqlDbType.TinyInt,
                                1));

                    if (insertedCount == 0)
                        return false;

                    bool inheritedAKey;
                    bool inheritedBKey;

                    if (bank1Length > 0)
                    {
                        inheritedAKey = bank1AKey == null;
                        inheritedBKey = bank1BKey == null;

                        command =
                            "Insert into MifareSectorSectorInfo(SectorNumber, InheritAKey, InheritBKey, AKey, BKey, Bank, Offset, Length, MifareSectorData) " +
                            "values(@SectorNumber, @InheritAKey, @InheritBKey, @AKey, @BKey, @Bank, @Offset, @Length, @MifareSectorData)";

                        insertedCount =
                            _databaseCommandExecutor.RunSqlNonQueryWithParameters(
                                command,
                                false,
                                new SqlParameterTypeAndValue(bank1Sector),
                                new SqlParameterTypeAndValue(inheritedAKey),
                                new SqlParameterTypeAndValue(inheritedBKey),
                                new SqlParameterTypeAndValue(SqlDbType.Binary, bank1AKey),
                                new SqlParameterTypeAndValue(SqlDbType.Binary, bank1BKey),
                                new SqlParameterTypeAndValue(1),
                                new SqlParameterTypeAndValue(bank1Offset),
                                new SqlParameterTypeAndValue(bank1Length),
                                new SqlParameterTypeAndValue(mifareResult[0]));

                        if (insertedCount == 0)
                            return false;
                    }

                    if (bank2Length > 0)
                    {
                        inheritedAKey = bank2AKey == null;
                        inheritedBKey = bank2BKey == null;

                        command =
                            "Insert into MifareSectorSectorInfo(SectorNumber, InheritAKey, InheritBKey, AKey, BKey, Bank, Offset, Length, MifareSectorData) " +
                            "values(@SectorNumber, @InheritAKey, @InheritBKey, @AKey, @BKey, @Bank, @Offset, @Length, @MifareSectorData)";

                        insertedCount =
                            _databaseCommandExecutor.RunSqlNonQueryWithParameters(
                                command,
                                false,
                                new SqlParameterTypeAndValue(bank2Sector),
                                new SqlParameterTypeAndValue(inheritedAKey),
                                new SqlParameterTypeAndValue(inheritedBKey),
                                new SqlParameterTypeAndValue(SqlDbType.Binary, bank2AKey),
                                new SqlParameterTypeAndValue(SqlDbType.Binary, bank2BKey),
                                new SqlParameterTypeAndValue(2),
                                new SqlParameterTypeAndValue(bank2Offset),
                                new SqlParameterTypeAndValue(bank2Length),
                                new SqlParameterTypeAndValue(mifareResult[0]));

                        if (insertedCount == 0)
                            return false;
                    }

                    if (bank3Length <= 0)
                        continue;

                    inheritedAKey = bank3AKey == null;
                    inheritedBKey = bank3BKey == null;

                    command =
                        "Insert into MifareSectorSectorInfo(SectorNumber, InheritAKey, InheritBKey, AKey, BKey, Bank, Offset, Length, MifareSectorData) " +
                        "values(@SectorNumber, @InheritAKey, @InheritBKey, @AKey, @BKey, @Bank, @Offset, @Length, @MifareSectorData)";

                    insertedCount =
                        _databaseCommandExecutor.RunSqlNonQueryWithParameters(
                            command,
                            false,
                            new SqlParameterTypeAndValue(bank3Sector),
                            new SqlParameterTypeAndValue(inheritedAKey),
                            new SqlParameterTypeAndValue(inheritedBKey),
                            new SqlParameterTypeAndValue(SqlDbType.Binary, bank3AKey),
                            new SqlParameterTypeAndValue(SqlDbType.Binary, bank3BKey),
                            new SqlParameterTypeAndValue(3),
                            new SqlParameterTypeAndValue(bank3Offset),
                            new SqlParameterTypeAndValue(bank3Length),
                            new SqlParameterTypeAndValue(mifareResult[0]));

                    if (insertedCount == 0)
                        return false;
                }
            }

            string[] droppedColumns =
            {
                "AID",
                "Akey",
                "Bkey",
                "AkeyBank1",
                "AkeyBank2",
                "AkeyBank3",
                "BkeyBank1",
                "BkeyBank2",
                "Encoding",
                "CypherData"
            };

            foreach (var droppedColumn in droppedColumns)
                if (!_databaseCommandExecutor.SqlDropColumn(
                        "CardSystem",
                        droppedColumn,
                        false,
                        out error))
                    return false;

            return true;
        }

        private bool ConversionCgpServerBeans1_92(out Exception error)
        {
            try
            {
                return _databaseCommandExecutor.SqlDropTable("DatesRestriction", false, out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_93(out Exception error)
        {
            try
            {
                return _databaseCommandExecutor.RunSqlNonQuery("UPDATE PresentationGroup set InheritedEmailSubject = 1", false, out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_94(out Exception error)
        {
            return
                SetRecoveryAndAutogrowth(
                    _creatorProperties.MainDatabaseDataFileParams,
                    _creatorProperties.MainDatabaseLogFileParams,
                    false,
                    out error);
        }

        private bool ConversionCgpServerBeans1_95(out Exception error)
        {
            if (/*_creatorProperties.EnableExternDatabase &&*/ _databaseCommandExecutor.TableExists("SystemVersion", true))
            {
                if (!_databaseCommandExecutor.DeleteVersionFromDatabase(
                        CGP_SERVER_BEANS_EXTERN_ASSEMBLY_NAME,
                        false,
                        out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool ConversionCgpServerBeans1_96(out Exception error)
        {
            try
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "UPDATE StructuredSubSiteObject set IsReference = 1 where ObjectType = {0} or ObjectType = {1}",
                            (byte) ObjectType.Login,
                            (byte) ObjectType.LoginGroup),
                        false,
                        out error))
                {
                    return false;
                }

                var loginsInStructuredSiteFromDatabase = _databaseCommandExecutor.RunSqlQuery(
                    string.Format(
                        "Select ObjectId from StructuredSubSiteObject where ObjectType = {0}",
                        (byte) ObjectType.Login),
                    false,
                    out error);

                var hashSetLoginsInStructuredSiteFromDatabase = new HashSet<string>();
                if (loginsInStructuredSiteFromDatabase != null)
                {
                    foreach (object[] loginInStructuredSiteFromDatabase in loginsInStructuredSiteFromDatabase)
                    {
                        hashSetLoginsInStructuredSiteFromDatabase.Add(loginInStructuredSiteFromDatabase[0].ToString());
                    }
                }

                var loginsFromDatabase = _databaseCommandExecutor.RunSqlQuery(
                    "Select Username from Login",
                    false,
                    out error);

                if (loginsFromDatabase != null)
                {
                    foreach (object[] loginFromDatabase in loginsFromDatabase)
                    {
                        if (hashSetLoginsInStructuredSiteFromDatabase.Contains(loginFromDatabase[0].ToString()))
                            continue;

                        if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format(
                                "Insert into StructuredSubSiteObject (StructuredSubSite, ObjectType, ObjectId, IsReference) values (NULL, {0}, '{1}', 1);",
                                (byte) ObjectType.Login,
                                loginFromDatabase[0].ToString()),
                            false,
                            out error))
                        {
                            return false;
                        }
                    }
                }

                var loginGroupsInStructuredSiteFromDatabase = _databaseCommandExecutor.RunSqlQuery(
                    string.Format(
                        "Select ObjectId from StructuredSubSiteObject where ObjectType = {0}",
                        (byte) ObjectType.LoginGroup),
                    false,
                    out error);

                var hashSetLoginGroupsInStructuredSiteFromDatabase = new HashSet<string>();
                if (loginGroupsInStructuredSiteFromDatabase != null)
                {
                    foreach (object[] loginGroupInStructuredSiteFromDatabase in loginGroupsInStructuredSiteFromDatabase)
                    {
                        hashSetLoginGroupsInStructuredSiteFromDatabase.Add(loginGroupInStructuredSiteFromDatabase[0].ToString());
                    }
                }

                var loginGroupsFromDatabase = _databaseCommandExecutor.RunSqlQuery(
                    "Select LoginGroupName from LoginGroup",
                    false,
                    out error);

                if (loginGroupsFromDatabase != null)
                {
                    foreach (object[] loginGroupFromDatabase in loginGroupsFromDatabase)
                    {
                        if (hashSetLoginGroupsInStructuredSiteFromDatabase.Contains(loginGroupFromDatabase[0].ToString()))
                            continue;

                        if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format(
                                "Insert into StructuredSubSiteObject (StructuredSubSite, ObjectType, ObjectId, IsReference) values (NULL, {0}, '{1}', 1);",
                                (byte) ObjectType.LoginGroup,
                                loginGroupFromDatabase[0].ToString()),
                            false,
                            out error))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_97(out Exception error)
        {
            try
            {
                bool convertLoginGroup = false;
                string command;

                //delete table QuickMenu
                if (!_databaseCommandExecutor.RunSqlNonQuery("DROP TABLE QuickMenu", false, out error))
                    return false;

                //create new table oldLogin
                if (!_databaseCommandExecutor.RunSqlNonQuery("CREATE TABLE oldLogin (Username nvarchar(255), PasswordHash nvarchar(255), PublicKey nvarchar(255), IsDisabled bit, " +
                                                             "MustChangePassword bit, ExpirationDate datetime, LastPasswordChangeDate datetime, ClientLanguage nvarchar(255)," +
                                                             "LoginGroup nvarchar(255), LocalAlarmInstruction nvarchar(255), Description nvarchar(255), IdPerson uniqueidentifier, IdLogin uniqueidentifier);", false, out error))
                    return false;

                //exists column LoginGroupName in table LoginGroup?
                var result =
                    _databaseCommandExecutor.RunSqlQuery(
                        "SELECT * FROM sys.columns WHERE [name] = N'LoginGroupName' AND [object_id] = OBJECT_ID(N'LoginGroup')", false, out error);

                if (result.Count > 0)
                    convertLoginGroup = true;

                //create new table oldLoginGroup
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery("CREATE TABLE oldLoginGroup (LoginGroupName nvarchar(255), IsDisabled bit, ExpirationDate datetime, Description nvarchar(max), IdLoginGroup uniqueidentifier);", false, out error))
                        return false;

                //copy all from table Login into table oldLogin
                string parameters = "PasswordHash, PublicKey, IsDisabled, ExpirationDate, MustChangePassword, LastPasswordChangeDate, " +
                                    "ClientLanguage, LocalAlarmInstruction, LoginGroup, Description, IdPerson, Username";
                if (!_databaseCommandExecutor.RunSqlNonQuery(string.Format("INSERT INTO oldLogin ({0}, IdLogin) SELECT {0}, NEWID() FROM Login",
                    parameters), false, out error))
                    return false;

                //copy all from table LoginGroup into table oldLoginGroup
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery
                        ("INSERT INTO oldLoginGroup (LoginGroupName, IsDisabled, ExpirationDate, Description, IdLoginGroup) " +
                         "SELECT LoginGroupName, IsDisabled, ExpirationDate, Description, NEWID() FROM LoginGroup",
                            false, out error))
                        return false;

                //remove all Constraints for table name's Login
                if (!DeleteConstraints("Login", string.Empty, out error, "FOREIGN KEY"))
                    return false;

                //remove all Constraints for table name's AccessControl
                if (!DeleteConstraints("AccessControl", string.Empty, out error, "FOREIGN KEY"))
                    return false;

                //remove all Constraints for table name's UserOpenedWindow
                if (!DeleteConstraints("UserOpenedWindow", string.Empty, out error, "FOREIGN KEY"))
                    return false;

                //remove all Constraints for table name's LoginGroup
                if (convertLoginGroup)
                    if (!DeleteConstraints("LoginGroup", string.Empty, out error, "FOREIGN KEY"))
                        return false;

                //rename columns in tables UserOpenedWindow and AccessControl
                if (!_databaseCommandExecutor.RunSqlNonQuery("EXEC sp_rename 'AccessControl.Login', 'oldLogin', 'COLUMN'", false, out error))
                    return false;

                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                            "EXEC sp_rename 'AccessControl.LoginGroup', 'oldLoginGroup', 'COLUMN'", false, out error))
                        return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery("EXEC sp_rename 'UserOpenedWindow.Login', 'oldLogin', 'COLUMN'", false, out error))
                    return false;

                //delete table Login
                if (!_databaseCommandExecutor.RunSqlNonQuery("DROP TABLE Login", false, out error))
                    return false;

                //delete table LoginGroup
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery("DROP TABLE LoginGroup", false, out error))
                        return false;

                NhHelper.Singleton.UpdateSchema(_creatorProperties.Assemblies);

                //insert login group datas
                if (convertLoginGroup)
                {
                    command = "INSERT INTO LoginGroup (IdLoginGroup, IsDisabled, ExpirationDate, Description) " +
                                     "SELECT IdLoginGroup, IsDisabled, ExpirationDate, Description FROM oldLoginGroup";

                    if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                        return false;
                }

                //insert login datas
                parameters =
                        "IdLogin, PasswordHash, PublicKey, IsDisabled, ExpirationDate, MustChangePassword, LastPasswordChangeDate, " +
                        "ClientLanguage, LoginGroup,LocalAlarmInstruction, Description, IdPerson";
                string parameters2;
                if (convertLoginGroup)
                {
                    parameters2 =
                        "IdLogin, PasswordHash, PublicKey, oldLogin.IsDisabled, oldLogin.ExpirationDate, MustChangePassword, LastPasswordChangeDate, " +
                        "ClientLanguage, IdLoginGroup,LocalAlarmInstruction, oldLogin.Description, IdPerson";

                    command = string.Format("INSERT INTO Login ({0}) SELECT {1} FROM oldLogin LEFT OUTER JOIN oldLoginGroup ON oldLogin.LoginGroup = oldLoginGroup.LoginGroupName",
                        parameters, parameters2);
                }
                else
                {
                    parameters2 =
                        "IdLogin, PasswordHash, PublicKey, oldLogin.IsDisabled, oldLogin.ExpirationDate, MustChangePassword, LastPasswordChangeDate, " +
                        "ClientLanguage, NULL, LocalAlarmInstruction, oldLogin.Description, IdPerson";

                    command = string.Format("INSERT INTO Login ({0}) SELECT {1} FROM oldLogin", parameters, parameters2);
                }

                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                //insert Login names into CentralNameRegister
                command = string.Format("INSERT INTO CentralNameRegister (Id, Name, ObjectType) SELECT IdLogin, Username, {0} FROM oldLogin", (byte)ObjectType.Login);

                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                //insert LoginGroup names into CentralNameRegister
                if (convertLoginGroup)
                {
                    command =
                        string.Format(
                            "INSERT INTO CentralNameRegister (Id, Name, ObjectType) SELECT IdLoginGroup, LoginGroupName, {0} FROM oldLoginGroup",
                            (byte)ObjectType.LoginGroup);

                    if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                        return false;
                }

                //insert IdLogin into AccessControl table
                if (!_databaseCommandExecutor.RunSqlNonQuery("UPDATE AccessControl SET AccessControl.Login = (SELECT IdLogin FROM oldLogin WHERE oldLogin.Username = AccessControl.oldLogin)",
                    false, out error))
                    return false;

                //insert IdLoginGroup into AccessControl table
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                            "UPDATE AccessControl SET AccessControl.LoginGroup = (SELECT IdLoginGroup FROM oldLoginGroup WHERE oldLoginGroup.LoginGroupName = AccessControl.oldLoginGroup)",
                            false, out error))
                        return false;

                //insert IdLogin into UserOpenedWindow table
                if (!_databaseCommandExecutor.RunSqlNonQuery("UPDATE UserOpenedWindow SET UserOpenedWindow.Login = (SELECT IdLogin FROM oldLogin WHERE oldLogin.Username = UserOpenedWindow.oldLogin)",
                    false, out error))
                    return false;

                //remove old columns from AccessControl and UserOpenedWindow tables
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                            "ALTER TABLE AccessControl DROP COLUMN oldLogin, oldLoginGroup", false, out error))
                        return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery("ALTER TABLE UserOpenedWindow DROP COLUMN oldLogin", false, out error))
                    return false;

                //update IdLogin in UserFoldersStructureObject table
                if (!_databaseCommandExecutor.RunSqlNonQuery(string.Format(
                    "UPDATE UserFoldersStructureObject SET ObjectId = (SELECT IdLogin FROM oldLogin WHERE oldLogin.Username = ObjectId) WHERE ObjectType = {0}",
                    (byte)ObjectType.Login),
                    false, out error))
                {
                    return false;
                }

                //update IdLoginGroup in UserFoldersStructureObject table
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(string.Format(
                        "UPDATE UserFoldersStructureObject SET ObjectId = (SELECT IdLoginGroup FROM oldLoginGroup WHERE oldLoginGroup.LoginGroupName = ObjectId) WHERE ObjectType = {0}",
                        (byte)ObjectType.LoginGroup), false, out error))
                        return false;

                //update IdLogin in StructuredSubSiteObject table
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    string.Format(
                        "UPDATE StructuredSubSiteObject SET ObjectId = (SELECT IdLogin FROM oldLogin WHERE oldLogin.Username = ObjectId) WHERE ObjectType = {0}",
                        (byte)ObjectType.Login),
                    false,
                    out error))
                {
                    return false;
                }

                //delete the LoginGroup name in StructuredSubSiteObject table if does not exists 
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "DELETE FROM StructuredSubSiteObject WHERE ObjectType = {0} AND (SELECT COUNT(LoginGroupName) from OldLoginGroup where LoginGroupName = ObjectId) = 0",
                            (byte)ObjectType.LoginGroup),
                        false, out error))
                    {
                        return false;
                    }

                //update IdLoginGroup in StructuredSubSiteObject table
                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "UPDATE StructuredSubSiteObject SET ObjectId = (SELECT IdLoginGroup FROM oldLoginGroup WHERE oldLoginGroup.LoginGroupName = ObjectId) WHERE ObjectType = {0}",
                            (byte)ObjectType.LoginGroup),
                        false, out error))
                    {
                        return false;
                    }

                //remove tables oldLogin and oldLoginGroup
                if (!_databaseCommandExecutor.RunSqlNonQuery("DROP TABLE oldLogin", false, out error))
                    return false;

                if (convertLoginGroup)
                    if (!_databaseCommandExecutor.RunSqlNonQuery("DROP TABLE oldLoginGroup", false, out error))
                        return false;

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeans1_98(out Exception error)
        {
            if (!DeleteConstraints("Person", "Department", out error, string.Empty))
                return false;

            return _databaseCommandExecutor.SqlDropColumn("Person", "Department", false, out error);
        }

        private bool ConversionCgpServerBeans2_10(out Exception error)
        {
            return
                _databaseCommandExecutor.RunSqlNonQuery(
                    "update Card set UtcDateStateLastChange = DateStateLastChange where UtcDateStateLastChange is null",
                    false,
                    out error);
        }

        private bool ConversionCgpServerBeans2_11(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE CentralNameRegister SET Name = 'CustomerAdmins' WHERE ObjectType='122' AND Name='admins'",
                false,
                out error))
            {
                return false;
            }

            return
                _databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE CentralNameRegister SET Name = 'admins' WHERE ObjectType='122' AND Name='admin'",
                    false,
                    out error);
        }

        private bool ConversionCgpServerBeans2_12(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                    @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Car' AND xtype='U')
                    BEGIN
                        CREATE TABLE Car (
                            IdCar uniqueidentifier not null primary key,
                            Brand nvarchar(255) null,
                            ValidityDateFrom datetime null,
                            ValidityDateTo datetime null,
                            SecurityLevel tinyint not null,
                            Description nvarchar(max) null,
                            SynchronizedWithTimetec bit not null,
                            UtcDateStateLastChange datetime not null,
                            Version int not null
                        )
                    END",
                    false,
                    out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                    @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CarCard' AND xtype='U')
                    BEGIN
                        CREATE TABLE CarCard (
                            IdCarCard uniqueidentifier not null primary key,
                            IdCar uniqueidentifier not null,
                            IdCard uniqueidentifier not null
                        )
                    END",
                    false,
                    out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "ALTER TABLE CarCard ADD CONSTRAINT FK_CarCard_Car FOREIGN KEY (IdCar) REFERENCES Car(IdCar)",
                    false,
                    out error))
            {
                return false;
            }

            return _databaseCommandExecutor.RunSqlNonQuery(
                    "ALTER TABLE CarCard ADD CONSTRAINT FK_CarCard_Card FOREIGN KEY (IdCard) REFERENCES Card(IdCard)",
                    false,
                    out error);
        }
        private bool ConversionCgpNCASServerBeans1_1(out Exception error)
        {
            try
            {
                error = null;

                bool indexExists = 
                    _databaseCommandExecutor.RowExists(
                        "select name from sys.indexes where name = 'DCU_CCULogicalAddress'",
                        false);

                if (!indexExists)
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                            "ALTER TABLE DCU ADD CONSTRAINT DCU_CCULogicalAddress UNIQUE (CCU, LogicalAddress)",
                            false,
                            out error))
                        return false;

                indexExists =
                    _databaseCommandExecutor.RowExists(
                        "select name from sys.indexes where name = 'CardReader_DCUCCUPortAddress'",
                        false);

                return 
                    indexExists ||
                    _databaseCommandExecutor.RunSqlNonQuery("ALTER TABLE CardReader ADD CONSTRAINT CardReader_DCUCCUPortAddress UNIQUE (DCU, CCU, Port, Address)", 
                        false, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_2(out Exception error)
        {
            try
            {
                return
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "update CCU set MaxNodeLookupSequence = 31", 
                        false, 
                        out error) &&
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "update CentralNameRegister set Name = REVERSE(SUBSTRING(REVERSE(Name), 1, CHARINDEX('/', REVERSE(Name)) - 1)) where ObjectType = 25 AND CHARINDEX('/', Name) > 0", 
                        false, 
                        out error);
            }
            catch (Exception excetpion)
            {
                error = excetpion;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_3(out Exception error)
        {
            try
            {
                error = null;

                if (!_databaseCommandExecutor.ColumnExists("AACardReader", "ACK", false))
                    return true;

                return
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "update AACardReader set AAUnconditionalSet=ACK", 
                        false, 
                        out error) &&
                    _databaseCommandExecutor.SqlDropColumn(
                        "AACardReader", 
                        "ACK", 
                        false, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_4(out Exception error)
        {
            try
            {
                return _databaseCommandExecutor.SqlDropColumn("CCU", "SNTPHostNames", false, out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_5(out Exception error)
        {
            try
            {
                return
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "UPDATE DCU set InputsCount=4 where InputsCount is null or InputsCount=0", 
                        false, 
                        out error) &&
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "UPDATE DCU set OutputsCount=4 where OutputsCount is null or OutputsCount=0", 
                        false, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_9(out Exception error)
        {
            try
            {
                return
                    _databaseCommandExecutor.RunSqlNonQuery(
                        "UPDATE AAInput set BlockTemporarilyUntil = 0 where NoCriticalInput = 0", 
                        false, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_91(out Exception error)
        {
            try
            {
                return
                    _databaseCommandExecutor.SqlDropColumn(
                        "CardReader", 
                        "EventlogDuringBlockAlarmAccessPermitted", 
                        false, 
                        out error) &&
                    _databaseCommandExecutor.SqlDropColumn(
                        "DevicesAlarmSetting", 
                        "EventlogDuringBlockAlarmCrAccessPermitted", 
                        false, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_92(out Exception error)
        {
            try
            {
                return
                    ImportData.ImportGraphicsSymbols(_databaseCommandExecutor, out error);

            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_93(out Exception error)
        {
            try
            {
                //exists column Name in table Scene?
                var result =
                    _databaseCommandExecutor.RunSqlQuery(
                        "SELECT * FROM sys.columns WHERE [name] = N'Name' AND [object_id] = OBJECT_ID(N'Scene')", false, out error);

                if (result.Count == 0)
                    return true;

                string command = string.Format("INSERT INTO CentralNameRegister (Id, Name, ObjectType) SELECT IdScene, Name, {0} FROM Scene",
                    (byte)ObjectType.Scene);

                if (!_databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;

                if (!DeleteConstraints("Scene", string.Empty, out error, "UNIQUE"))
                    return false;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                            "ALTER TABLE Scene DROP COLUMN Name", false, out error))
                    return false;

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_94(out Exception error)
        {
            try
            {
                return _databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE ACLGroup SET RemoveAllAcls = 1",
                    false,
                    out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_95(out Exception error)
        {
            try
            {
                if (_databaseCommandExecutor.ColumnExists(
                    "AlarmArea", "TimeBuying", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE AlarmArea DROP COLUMN TimeBuying",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                if (_databaseCommandExecutor.ColumnExists(
                    "AlarmArea", "TimeBuyingDuration", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE AlarmArea DROP COLUMN TimeBuyingDuration",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                if (_databaseCommandExecutor.ColumnExists(
                    "ACLSettingAA", "AlarmAreaUnconditionalUnset", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE ACLSettingAA DROP COLUMN AlarmAreaUnconditionalUnset",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                if (_databaseCommandExecutor.ColumnExists(
                    "ACLSettingAA", "UnconditionalUnset", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE ACLSettingAA DROP COLUMN UnconditionalUnset",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                if (_databaseCommandExecutor.ColumnExists(
                    "ACLSettingAA", "TimeBuying", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE ACLSettingAA DROP COLUMN TimeBuying",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                if (_databaseCommandExecutor.ColumnExists(
                    "CardReader", "EnableEventlog", false))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "ALTER TABLE CardReader DROP COLUMN EnableEventlog",
                        false,
                        out error))
                    {
                        return false;
                    }
                }

                return _databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE ACLGroup SET RemoveAllAcls = 1",
                    false,
                    out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans1_96(out Exception error)
        {
            byte[] COMBO_LICENCE_KEY = { 0x5E, 0x9B, 0x3A, 0x76, 0x11, 0x00, 0x2D, 0x35, 0x47, 0x20, 0x6B, 0xC8, 0x28, 0x8F, 0xAA, 0xEA };
            byte[] COMBO_LICENCE_SALT = { 0x01, 0x59, 0x55, 0x5A, 0x4D, 0x11, 0xFB, 0x14, 0x77, 0xC7, 0xA1, 0x51, 0x59, 0x2D, 0xDB, 0x22 };
            Random random = new Random();

            List<object[]> ccus = _databaseCommandExecutor.RunSqlQuery("SELECT CCU.IdCCU from CCU", false, out error);
            foreach (var ccu in ccus)
            {
                Guid guidCcu = (Guid)ccu[0];
                
                byte[] rawData = new byte[24];
                Array.Copy(guidCcu.ToByteArray(), 0, rawData, 2, 16);
                rawData[1] = (byte)random.Next(255);
                rawData[18] = (byte)random.Next(255);
                rawData[0] = (byte)0;

                var tempCrc = BitConverter.GetBytes(
                    Crc32.ComputeChecksum(rawData, 0, rawData.Length - 4));

                Array.Copy(tempCrc, 0, rawData, rawData.Length - 4, 4);

                byte[] data = QuickCrypto.Encrypt(
                    rawData,
                    COMBO_LICENCE_KEY,
                    COMBO_LICENCE_SALT);


                if (_databaseCommandExecutor.RunSqlNonQueryWithParameters(
                    "UPDATE CCU " +
                    "SET Cat12ComboLicence=@Cat12ComboLicence " +
                    "WHERE CCU.IdCCU=@IdCCU ",
                    false,
                    new SqlParameterTypeAndValue(SqlDbType.Binary, data),
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, guidCcu)) == 0)
                {
                    return false;
                }
            }



            return true;
        }

        private bool ConversionCgpNCASServerBeans2_01(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE DevicesAlarmSetting " +
                "SET AlarmInvalidPinRetriesLimitReached=1, " +
                "AlarmInvalidGinRetriesLimitReached=1, " +
                "InvalidPinRetriesLimitEnabled=1, " +
                "InvalidPinRetriesCount=3, " +
                "InvalidPinRetriesLimitReachedTimeout=5, " +
                "InvalidGinRetriesLimitEnabled=1, " +
                "InvalidGinRetriesCount=3, " +
                "InvalidGinRetriesLimitReachedTimeout=5",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE CardReader SET InvalidGinRetriesLimitEnabled=1",
                false,
                out error))
            {
                return false;
            }

            var logins = _databaseCommandExecutor.RunSqlQuery(
                "SELECT [Login] from AccessControl WHERE [Source] = 'Access' AND [Access] = 1010 AND NOT([Login] is NULL)",
                false,
                out error);

            foreach (var login in logins)
            {
                var idLogin = (Guid) login[0];

                if (_databaseCommandExecutor.RunSqlNonQueryWithParameters(
                    "INSERT INTO AccessControl ([IdAccessControl], [Source], [Access], [Login], [LoginGroup]) " +
                    "VALUES (NEWID(), 'Access', 1016, @IdLogin, NULL)",
                    false,
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, idLogin)) == 0)
                {
                    return false;
                }
            }

            logins = _databaseCommandExecutor.RunSqlQuery(
                "SELECT [Login] from AccessControl WHERE [Source] = 'Access' AND [Access] = 1011 AND NOT([Login] is NULL)",
                false,
                out error);

            foreach (var login in logins)
            {
                var idLogin = (Guid) login[0];

                if (_databaseCommandExecutor.RunSqlNonQueryWithParameters(
                    "INSERT INTO AccessControl ([IdAccessControl], [Source], [Access], [Login], [LoginGroup]) " +
                    "VALUES (NEWID(), 'Access', 1017, @IdLogin, NULL)",
                    false,
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, idLogin)) == 0)
                {
                    return false;
                }
            }

            var loginGroups = _databaseCommandExecutor.RunSqlQuery(
                "SELECT [LoginGroup] from AccessControl WHERE [Source] = 'Access' AND [Access] = 1010 AND NOT([LoginGroup] is NULL)",
                false,
                out error);

            foreach (var loginGroup in loginGroups)
            {
                var idLoginGroup = (Guid)loginGroup[0];

                if (_databaseCommandExecutor.RunSqlNonQueryWithParameters(
                    "INSERT INTO AccessControl ([IdAccessControl], [Source], [Access], [Login], [LoginGroup]) " +
                    "VALUES (NEWID(), 'Access', 1016, NULL, @IdLoginGroup)",
                    false,
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, idLoginGroup)) == 0)
                {
                    return false;
                }
            }

            loginGroups = _databaseCommandExecutor.RunSqlQuery(
                "SELECT [LoginGroup] from AccessControl WHERE [Source] = 'Access' AND [Access] = 1011 AND NOT([LoginGroup] is NULL)",
                false,
                out error);

            foreach (var loginGroup in loginGroups)
            {
                var idLoginGroup = (Guid)loginGroup[0];

                if (_databaseCommandExecutor.RunSqlNonQueryWithParameters(
                    "INSERT INTO AccessControl ([IdAccessControl], [Source], [Access], [Login], [LoginGroup]) " +
                    "VALUES (NEWID(), 'Access', 1017, NULL, @IdLoginGroup)",
                    false,
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, idLoginGroup)) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_02(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE DevicesAlarmSetting " +
                "SET AlarmCcuCatUnreachable=1, " +
                "AlarmCcuTransferToArcTimedOut=1",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_03(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists("AlarmArea", "Id", false))
            {
                if (!DeleteConstraints("AlarmArea", "Id", out error, "UNIQUE"))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("AlarmArea", "Id", false, out error))
                    return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "Alter table AlarmArea add Id int;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "declare @AlarmAreaTmp table(IdAlarmArea uniqueidentifier primary key clustered, NewId int identity(1,1));" +
                 "insert into @AlarmAreaTmp (IdAlarmArea) select IdAlarmArea from AlarmArea;" +
                 "update AlarmArea set Id = NewId FROM @AlarmAreaTmp as AlarmAreaTmp WHERE AlarmArea.IdAlarmArea = AlarmAreaTmp.IdAlarmArea;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "ALTER TABLE AlarmArea ALTER COLUMN Id int NOT NULL;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "ALTER TABLE AlarmArea ADD UNIQUE (Id);",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_04(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists("AAInput", "Id", false))
            {
                if (!DeleteConstraints("AAInput", "Id", out error, null))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("AAInput", "Id", false, out error))
                    return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "Alter table AAInput add Id int",
                false,
                out error))
            {
                return false;
            }

            var alarmAreaIds = _databaseCommandExecutor.RunSqlQuery(
                "Select IdAlarmArea from AlarmArea",
                false,
                out error);

            if (alarmAreaIds != null)
            {
                foreach (var alarmAreaId in alarmAreaIds)
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "declare @AAInputTmp table(IdAAInput uniqueidentifier primary key clustered, NewId int identity(1,1));" +
                            "insert into @AAInputTmp (IdAAInput) select IdAAInput from AAInput where AlarmArea = '{0}';" +
                            "update AAInput set Id = NewId FROM @AAInputTmp as AAInputTmp WHERE AAInput.IdAAInput = AAInputTmp.IdAAInput;",
                            alarmAreaId[0]),
                        false,
                        out error))
                    {
                        return false;
                    }
                }
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "ALTER TABLE AAInput ALTER COLUMN Id int NOT NULL",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_05(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE DevicesAlarmSetting SET AlarmAreaSetByOnOffObjectFailed = 1;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "AlarmArea", 
                "AlarmAreaAlarmArcsOverwrited", 
                false, 
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_06(out Exception error)
        {
            if (!_databaseCommandExecutor.SqlDropColumn(
                "Output",
                "DelayToOn",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "Output",
                "DelayToOff",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_07(out Exception error)
        {

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE CardReader SET InvalidGinRetriesLimitEnabled=NULL WHERE InvalidGinRetriesLimitEnabled=1",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_08(out Exception error)
        {
            var loginsWithAccessInDcu = _databaseCommandExecutor.RunSqlQuery(
                "SELECT Login FROM AccessControl WHERE Login IS NOT NULL AND Source = 'Access' AND Access = 310",
                false,
                out error);

            if (error != null)
                return false;

            var loginsWithAccessInDe = _databaseCommandExecutor.RunSqlQuery(
                "SELECT Login FROM AccessControl WHERE Login IS NOT NULL AND Source = 'Access' AND Access = 609",
                false,
                out error);

            if (error != null)
                return false;

            var idLoginsWithAccessInDe = new HashSet<Guid>(loginsWithAccessInDe.Select(row => (Guid) row[0]));

            foreach (var idLogin in loginsWithAccessInDcu.Select(row => (Guid) row[0]))
            {
                if (!idLoginsWithAccessInDe.Contains(idLogin))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "UPDATE AccessControl SET Access = 609 WHERE Login = '{0}' AND Source = 'Access' AND Access = 310",
                            idLogin),
                        false,
                        out error))
                    {
                        return false;
                    }
                }
            }

            var loginGroupsWithAccessInDcu = _databaseCommandExecutor.RunSqlQuery(
                "SELECT LoginGroup FROM AccessControl WHERE LoginGroup IS NOT NULL AND Source = 'Access' AND Access = 310",
                false,
                out error);

            if (error != null)
                return false;

            var loginGroupsWithAccessInDe = _databaseCommandExecutor.RunSqlQuery(
                "SELECT LoginGroup FROM AccessControl WHERE LoginGroup IS NOT NULL AND Source = 'Access' AND Access = 609",
                false,
                out error);

            if (error != null)
                return false;

            var idLoginGroupsWithAccessInDe = new HashSet<Guid>(loginGroupsWithAccessInDe.Select(row => (Guid)row[0]));

            foreach (var idLoginGroup in loginGroupsWithAccessInDcu.Select(row => (Guid)row[0]))
            {
                if (!idLoginGroupsWithAccessInDe.Contains(idLoginGroup))
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                        string.Format(
                            "UPDATE AccessControl SET Access = 609 WHERE LoginGroup = '{0}' AND Source = 'Access' AND Access = 310",
                            idLoginGroup),
                        false,
                        out error))
                    {
                        return false;
                    }
                }
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "DELETE FROM AccessControl WHERE Source = 'Access' AND Access = 310",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_09(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE DevicesAlarmSetting SET SecurityLevelForEnterToMenu = 5",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE CardReader SET SLForEnterToMenu = NULL where SLForEnterToMenu = 1",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE CardReader SET SLForEnterToMenu = 4 where SLForEnterToMenu is NULL OR SLForEnterToMenu = 0",
                false,
                out error))
            {
                return false;
            }

            var implicitAaCardReaders = _databaseCommandExecutor.RunSqlQuery(
                "SELECT AlarmArea, CardReader FROM AACardReader WHERE PermanentlyUnlock = 0",
                false,
                out error);

            foreach (var implicitAaCardReader in implicitAaCardReaders)
            {
                var idAlarmArea = (Guid) implicitAaCardReader[0];
                var idCardReader = (Guid) implicitAaCardReader[1];

                var implicitAlarmAreas = _databaseCommandExecutor.RunSqlQuery(
                    string.Format(
                        "SELECT LockAASecurityLevel, UnlockAASecurityLevel, LockUnlockAAGin, LockUnlockAAGinLength FROM AlarmArea WHERE IdAlarmArea = '{0}'",
                        idAlarmArea),
                    false,
                    out error);

                var implicitAlarmArea = implicitAlarmAreas != null && implicitAlarmAreas.Count > 0
                    ? implicitAlarmAreas[0]
                    : null;

                if (implicitAlarmArea != null)
                {
                    var lockSecurityLevel = (byte?) implicitAlarmArea[0];
                    var unlockSecurityLevel = (byte?)implicitAlarmArea[1];
                    var ginLength = (byte?) implicitAlarmArea[3];
                    var gin = ginLength.HasValue && ginLength > 0
                        ? (string) implicitAlarmArea[2]
                        : string.Empty;

                    var cardReaders = _databaseCommandExecutor.RunSqlQuery(
                        string.Format(
                            "SELECT SLForEnterToMenu FROM CardReader WHERE IdCardReader = '{0}'",
                            idCardReader),
                        false,
                        out error);

                    var cardReader = cardReaders != null && cardReaders.Count > 0
                        ? cardReaders[0]
                        : null;


                    if (cardReader != null)
                    {
                        var slForEnterToMenu = (byte?) cardReader[0];

                        if (lockSecurityLevel == 2 || unlockSecurityLevel == 2)
                            slForEnterToMenu = slForEnterToMenu.HasValue
                                ? (byte) 2
                                : (byte) 3;

                        if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format(
                                "UPDATE CardReader SET SLForEnterToMenu = {0}, GinForEnterToMenu = '{1}', GinLengthForEnterToMenu = {2} where IdCardReader = '{3}'",
                                slForEnterToMenu != null
                                    ? slForEnterToMenu.ToString()
                                    : "NULL",
                                gin,
                                ginLength,
                                idCardReader),
                            false,
                            out error))
                        {
                            return false;
                        }
                    }
                }
            }



            if (!_databaseCommandExecutor.SqlDropColumn(
                "CCU",
                "SecurityLevel",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "CCU",
                "GIN",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "CCU",
                "GinLenght",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "AlarmArea",
                "LockAASecurityLevel",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "AlarmArea",
                "UnlockAASecurityLevel",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "AlarmArea",
                "LockUnlockAAGin",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "AlarmArea",
                "LockUnlockAAGinLength",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_10(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE APBZCardReaders SET EntryExitBy = 1 where EntryByAccessInterrupted is NULL OR EntryByAccessInterrupted = 0",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE APBZCardReaders SET EntryExitBy = 2 where EntryByAccessInterrupted is NOT NULL AND EntryByAccessInterrupted = 1",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "APBZCardReaders",
                "EntryByAccessInterrupted",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_11(out Exception error)
        {
            if (!_databaseCommandExecutor.SqlDropColumn(
                "DevicesAlarmSetting",
                "AllowSensorReportinDuringAAUnset",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.SqlDropColumn(
                "DevicesAlarmSetting",
                "ReportTampersOfUnsetAA",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_12(out Exception error)
        {
            try
            {
                return
                    ImportData.ImportGraphicsSymbols(_databaseCommandExecutor, out error);

            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpNCASServerBeans2_13(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists("CCU", "IndexCCU", false))
            {
                if (!DeleteConstraints("CCU", "IndexCCU", out error, "UNIQUE"))
                    return false;

                if (!_databaseCommandExecutor.SqlDropColumn("CCU", "IndexCCU", false, out error))
                    return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "Alter table CCU add IndexCCU int;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "declare @CCUTmp table(IdCCU uniqueidentifier primary key clustered, NewIndexCCU int identity(1,1));" +
                 "insert into @CCUTmp (IdCCU) select IdCCU from CCU;" +
                 "update CCU set IndexCCU = NewIndexCCU FROM @CCUTmp as CCUTmp WHERE CCU.IdCCU = CCUTmp.IdCCU;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "ALTER TABLE CCU ALTER COLUMN IndexCCU int NOT NULL;",
                false,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "ALTER TABLE CCU ADD UNIQUE (IndexCCU);",
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_14(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists("ACLGroup", "ApplyEndValidity", false))
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE ACLGroup SET ApplyEndlessValidity = ApplyEndValidity",
                    false,
                    out error))
                {
                    return false;
                }

                if (!_databaseCommandExecutor.SqlDropColumn("ACLGroup", "ApplyEndValidity", false, out error))
                    return false;
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_15(out Exception error)
        {
            //set ccus FullTextSearchString
            var centralRegisterTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT Id, Name FROM CentralNameRegister WHERE ObjectType = 21", 
                false, 
                out error);

            if (error != null)
                return false;

            if (centralRegisterTable != null)
            {
                foreach (object[] row in centralRegisterTable)
                {
                    var idCcu = (Guid) row[0];
                    var nameCcu = (string) row[1];

                    var ccuTable = _databaseCommandExecutor.RunSqlQuery(
                        string.Format("SELECT IndexCCU FROM CCU WHERE IdCCU = '{0}'", idCcu),
                        false,
                        out error);

                    if (error != null)
                        return false;

                    if (ccuTable.Count > 0)
                    {
                        var indexCcu = (int)ccuTable[0][0];

                        string fullTextSearchString = string.Format("{0}\t{1}",
                            nameCcu,
                            string.Format("{0} - {1}",
                                indexCcu.ToString("D3"),
                                nameCcu));

                        if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format("UPDATE CentralNameRegister SET FullTextSearchString = '{0}' WHERE Id = '{1}'",
                                fullTextSearchString,
                                idCcu),
                            false,
                            out error))
                        {
                            return false;
                        }
                    }
                }
            }

            //set alarm areas FullTextSearchString
            centralRegisterTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT Id, Name, AlternateName FROM CentralNameRegister WHERE ObjectType = 18",
                false,
                out error);

            if (error != null)
                return false;

            if (centralRegisterTable != null)
            {
                foreach (object[] row in centralRegisterTable)
                {
                    var idAlarmArea = (Guid)row[0];
                    var nameAlarmArea = (string)row[1];
                    var alternateName = (string)row[2];

                    var alarmAreasTable = _databaseCommandExecutor.RunSqlQuery(
                        string.Format("SELECT Id FROM AlarmArea WHERE IdAlarmArea = '{0}'", idAlarmArea),
                        false,
                        out error);

                    if (error != null)
                        return false;

                    if (alarmAreasTable.Count > 0)
                    {
                        var indexAlarmArea = (int) alarmAreasTable[0][0];

                        string fullTextSearchString = string.Format("{0}\t{1}\t{2}",
                            nameAlarmArea,
                            alternateName,
                            string.Format("{0} - {1}",
                                indexAlarmArea.ToString("D2"),
                                nameAlarmArea));

                        if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format("UPDATE CentralNameRegister SET FullTextSearchString = '{0}' WHERE Id = '{1}'",
                                fullTextSearchString,
                                idAlarmArea),
                            false,
                            out error))
                        {
                            return false;
                        }
                    }
                }
            }

            //set inputs FullTextSearchString
            centralRegisterTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT Id, Name FROM CentralNameRegister WHERE ObjectType = 11",
                false,
                out error);

            if (error != null)
                return false;

            if (centralRegisterTable != null)
            {
                foreach (object[] row in centralRegisterTable)
                {
                    var idInput = (Guid)row[0];
                    var nameInput = (string)row[1];

                    var inputsTable = _databaseCommandExecutor.RunSqlQuery(
                        string.Format("SELECT NickName FROM Input WHERE IdInput = '{0}'", idInput),
                        false,
                        out error);

                    if (error != null)
                        return false;

                    if (inputsTable.Count == 0)
                        continue;

                    var nickName = (string) inputsTable[0][0];

                    var alarmAreaInputsTable = _databaseCommandExecutor.RunSqlQuery(
                        string.Format("SELECT Id, AlarmArea FROM AAInput WHERE Input = '{0}'", idInput),
                        false,
                        out error);

                    if (error != null)
                        return false;

                    if (alarmAreaInputsTable.Count == 0)
                        continue;

                    var inputIndexes = new StringBuilder();

                    foreach (object[] aaInputRecord in alarmAreaInputsTable)
                    {
                        var indexAAInput = (int) aaInputRecord[0];
                        var alarmAreaId = (Guid) aaInputRecord[1];

                        var alarmAreasTable = _databaseCommandExecutor.RunSqlQuery(
                            string.Format("SELECT Id FROM AlarmArea WHERE IdAlarmArea = '{0}'", alarmAreaId),
                            false,
                            out error);

                        if (error != null)
                            return false;

                        if (alarmAreasTable.Count > 0)
                        {
                            var indexAlarmArea = (int) alarmAreasTable[0][0];

                            if (inputIndexes.Length == 0)
                            {
                                inputIndexes.Append(
                                    string.Format(
                                        "{0}{1}",
                                        indexAlarmArea.ToString("D2"),
                                        indexAAInput.ToString("D2")));
                            }
                            else
                            {
                                inputIndexes.Append(
                                    string.Format(
                                        ",{0}{1}",
                                        indexAlarmArea.ToString("D2"),
                                        indexAAInput.ToString("D2")));
                            }
                        }
                    }

                    string fullTextSearchString = string.Format("{0}\t{1}\t{2}",
                        nameInput,
                        nickName,
                        string.Format("{0} - {1}",
                            inputIndexes.ToString(),
                            nameInput));

                    if (!_databaseCommandExecutor.RunSqlNonQuery(
                            string.Format("UPDATE CentralNameRegister SET FullTextSearchString = '{0}' WHERE Id = '{1}'",
                                fullTextSearchString,
                                idInput),
                            false,
                            out error))
                    {
                        return false;
                    }
                }
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_16(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE AlarmArea SET OutputSetNotCalmAaByObjectForAaOnPeriod = 1000",
                false,
                out error))
            {
                return false;
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_17(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE AlarmArea SET NotForcedTimeBuyingProvideOnlyUnset = '1'",
                    false,
                    out error))
            {
                return false;
            }

            var alarmAreasTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT IdAlarmArea FROM AlarmArea WHERE ObjForForcedTimeBuyingId IS NULL",
                false,
                out error);

            if (error != null)
                return false;

            foreach (var row in alarmAreasTable)
            {
                var idAlarmArea = (Guid)row[0];

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    string.Format("UPDATE AlarmArea SET AlwaysProvideUnsetForTimeBuying = '1' WHERE IdAlarmArea = '{0}'",
                        idAlarmArea),
                    false,
                    out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_18(out Exception error)
        {
            var inputsIdsTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT ObjForAutomaticActId FROM AlarmArea WHERE ObjForAutomaticActId is not NULL AND ObjForAutomaticActType LIKE '%Beans.Input%'",
                false,
                out error);

            if (error != null)
                return false;

            var inputsIds = inputsIdsTable.Select(obj => (Guid) obj[0]);

            inputsIdsTable = _databaseCommandExecutor.RunSqlQuery(
                "SELECT ObjForForcedTimeBuyingId FROM AlarmArea WHERE ObjForForcedTimeBuyingId is not NULL AND ObjForForcedTimeBuyingType LIKE '%Beans.Input%'",
                false,
                out error);

            if (error != null)
                return false;

            inputsIds = inputsIds.Concat(inputsIdsTable.Select(obj => (Guid) obj[0]));

            var inputIdsString = new StringBuilder();

            foreach (var inputId in inputsIds)
            {
                inputIdsString.AppendFormat(
                    inputIdsString.Length > 0
                        ? ", '{0}'"
                        : "'{0}'",
                    inputId);
            }

            if (inputIdsString.Length == 0)
                return true;

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                string.Format(
                    "UPDATE Input SET AlarmTamper = '1' WHERE InputType = '1' AND IdInput IN ({0})",
                    inputIdsString),
                false,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpNCASServerBeans2_19(out Exception error)
        {
            return ImportData.InsertTimetecSetting(
                _databaseCommandExecutor,
                out error);
        }

        private bool ConversionCgpNCASServerBeans2_20(out Exception error)
        {
            var tablesForSetVersion = new[]
            {
                "Calendar",
                "CalendarDateSetting",
                "Card",
                "CardSystem",
                "DailyPlan",
                "DayType",
                "Person",
                "TimeZone",
                "TimeZoneDateSetting",
                "AACardReader",
                "AAInput",
                "AccessControlList",
                "AccessZone",
                "ACLPerson",
                "ACLSetting",
                "ACLSettingAA",
                "AlarmArc",
                "AlarmArea",
                "AlarmTransmitter",
                "AntiPassBackZone",
                "CardReader",
                "CCU",
                "DCU",
                "DevicesAlarmSetting",
                "DoorEnvironment",
                "Input",
                "MultiDoor",
                "MultiDoorElement",
                "Output",
                "SecurityDailyPlan",
                "SecurityDayInterval",
                "SecurityTimeZone",
                "SecurityTimeZoneDateSetting"
            };

            foreach (var tableForSetVerion in tablesForSetVersion)
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    string.Format(
                        "UPDATE {0} SET [Version] = 0",
                        tableForSetVerion),
                    false,
                    out error))
                {
                    return false;
                }
            }

            var objectTypesForCreateMaximumVersion = new[]
            {
                ObjectType.Calendar,
                ObjectType.CalendarDateSetting,
                ObjectType.Card,
                ObjectType.CardSystem,
                ObjectType.DailyPlan,
                ObjectType.DayType,
                ObjectType.Person,
                ObjectType.TimeZone,
                ObjectType.TimeZoneDateSetting,
                ObjectType.AACardReader,
                ObjectType.AAInput,
                ObjectType.AccessControlList,
                ObjectType.AccessZone,
                ObjectType.ACLPerson,
                ObjectType.ACLSetting,
                ObjectType.ACLSettingAA,
                ObjectType.AlarmArc,
                ObjectType.AlarmArea,
                ObjectType.AlarmTransmitter,
                ObjectType.AntiPassBackZone,
                ObjectType.CardReader,
                ObjectType.CCU,
                ObjectType.DCU,
                ObjectType.DevicesAlarmSetting,
                ObjectType.DoorEnvironment,
                ObjectType.Input,
                ObjectType.MultiDoor,
                ObjectType.MultiDoorElement,
                ObjectType.Output,
                ObjectType.SecurityDailyPlan,
                ObjectType.SecurityDayInterval,
                ObjectType.SecurityTimeZone,
                ObjectType.SecurityTimeZoneDateSetting
            };

            foreach (var objectTypeForCreateMaximumVersion in objectTypesForCreateMaximumVersion)
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    string.Format(
                        "INSERT INTO MaximumVersionForObjectType (IdMaximumVersionForObjectType, ObjectType, Version) VALUES ('{0}', {1}, 0)",
                        Guid.NewGuid(),
                        (int)objectTypeForCreateMaximumVersion),
                    false,
                    out error))
                {
                    return false;
                }
            }

            if (!_databaseCommandExecutor.SqlDropTable(
                "VersionOfSendedObjectsToCCU)",
                false,
                out error))
            {
                return false;
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_21(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists(
                "AlarmAreaAlarmArc",
                "IdAlarmArc",
                false))
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        "UPDATE AlarmAreaAlarmArc SET AlarmArc = IdAlarmArc",
                        false,
                        out error))
                {
                    return false;
                }

                if (!_databaseCommandExecutor.SqlDropColumn(
                        "AlarmAreaAlarmArc",
                        "IdAlarmArc",
                        false,
                        out error))
                {
                    return false;
                }
            }

            if (_databaseCommandExecutor.ColumnExists(
                "DevicesAlarmSettingAlarmArc",
                "IdAlarmArc",
                false))
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE DevicesAlarmSettingAlarmArc SET AlarmArc = IdAlarmArc",
                    false,
                    out error))
                {
                    return false;
                }

                if (!_databaseCommandExecutor.SqlDropColumn(
                        "DevicesAlarmSettingAlarmArc",
                        "IdAlarmArc",
                        false,
                        out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_22(out Exception error)
        {
            if (_databaseCommandExecutor.TableExists(
                    "PersonAttribute",
                    false)
                && _databaseCommandExecutor.ColumnExists(
                        "PersonAttribute",
                        "IsWatched",
                        false))
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "DELETE FROM PersonAttribute WHERE IsWatched = 0",
                    false,
                    out error))
                {
                    return false;
                }

                if (!_databaseCommandExecutor.SqlDropColumn(
                    "PersonAttribute",
                    "IsWatched",
                    false,
                    out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool ConversionCgpNCASServerBeans2_23(out Exception error)
        {
            if (_databaseCommandExecutor.ColumnExists(
                        "CardReader",
                        "SlCodeLedPresentation",
                        false))
            {
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    "UPDATE CardReader SET SlCodeLedPresentation = 1 WHERE SlCodeLedPresentation is NULL",
                    false,
                    out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool DeleteConstraints(string tableName, string columnName, out Exception error, string constraintType)
        {
            try
            {
                var condition = new StringBuilder(
                    string.Format(
                        "INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.TABLE_NAME='{0}'",
                        tableName));

                if (!string.IsNullOrEmpty(columnName))
                    condition.AppendFormat(
                        " AND COLUMN_NAME = '{0}'", 
                        columnName);

                if (!string.IsNullOrEmpty(constraintType))
                    condition.AppendFormat(
                        " AND CONSTRAINT_TYPE='{0}'", 
                        constraintType);

                var loginConstraint = _databaseCommandExecutor.RunSqlQuery(string.Format(
                    "SELECT INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS " +
                    "ON INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME = INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME " +
                    "WHERE {0}",
                    condition),
                    false,
                    out error);

                if (loginConstraint == null)
                    return false;

                //delete these constraints from table
                foreach (object[] constraintName in loginConstraint)
                {
                    if (!_databaseCommandExecutor.RunSqlNonQuery(string.Format("ALTER TABLE {1} DROP CONSTRAINT {0}",
                        constraintName[0].ToString(), tableName), false, out error))
                        return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        /// <summary>
        /// Conversion routine for Eventlogs for version 1.91
        /// </summary>
        /// <returns></returns>
        private bool ConversionCgpServerBeansExtern1_91(out Exception error)
        {
            try
            {
                if (!_databaseCommandExecutor.ColumnExists(
                    "Eventlog", 
                    "Date", 
                    /*_creatorProperties.EnableExternDatabase*/ true))
                {
                    error = new Exception("Column [Date] does not exist in the table [Eventlog]");
                    return false;
                }

                Exception notFatalError;

                string command = "Drop index IndexEventlogDateTime on [Eventlog]";

                _databaseCommandExecutor.RunSqlNonQuery(
                    command, 
                    /*_creatorProperties.EnableExternDatabase*/ true, 
                    out notFatalError);

                command = "Alter table [Eventlog] drop column [EventlogDateTime]";

                _databaseCommandExecutor.RunSqlNonQuery(
                    command,
                    /*_creatorProperties.EnableExternDatabase*/ true,
                    out notFatalError);

                command = "Alter table [Eventlog] add [EventlogDateTime] DateTime2(3) null";
                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error))
                    return false;

                command = "Create index IndexEventlogDateTime on [Eventlog] ([EventlogDateTime])";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error))
                    return false;

                command = "UPDATE [Eventlog] set [EventlogDateTime] = [Date]";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error))
                    return false;

                command = "UPDATE [Eventlog] set [EventlogDateTime] = DATEADD(MILLISECOND, [Miliseconds], [EventlogDateTime])";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error))
                    return false;

                command = "Drop index IndexEventlogDate on [Eventlog]";

                _databaseCommandExecutor.RunSqlNonQuery(
                    command, 
                    /*_creatorProperties.EnableExternDatabase*/ true, 
                    out notFatalError);

                command = "Alter table [Eventlog] drop column [Date]";

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error))
                    return false;

                command = "UPDATE [Eventlog] set [Miliseconds] = -1";

                return 
                    _databaseCommandExecutor.RunSqlNonQuery(
                        command, 
                        /*_creatorProperties.EnableExternDatabase*/ true, 
                        out error);
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
        }

        private bool ConversionCgpServerBeansExtern1_92(out Exception error)
        {
            Type type = GetType();

            Assembly thisAssembly = type.Assembly;
            string thisNamespace = type.Namespace;

            //bool enableExternDatabase = _creatorProperties.EnableExternDatabase;

            if (!_databaseCommandExecutor.TableExists("EventLog", /*enableExternDatabase*/true) ||
                !_databaseCommandExecutor.TableExists("EventSource", /*enableExternDatabase*/true) ||
                !_databaseCommandExecutor.TableExists("EventlogParameter", /*enableExternDatabase*/true))
            {
                error = null;
                return false;
            }

            if (!_databaseCommandExecutor.CreateProcedure(
                    "dbo.sp_dropkeys",
                    thisAssembly.GetManifestResourceStream(thisNamespace + ".CreateDropKeysProcedure.sql"),
                    /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.CreateProcedure(
                    "dbo.sp_dropindices",
                    thisAssembly.GetManifestResourceStream(
                        thisNamespace + ".CreateDropIndicesProcedure.sql"),
                /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.RunSqlNonQueryFromStream(
                    thisAssembly.GetManifestResourceStream(
                        thisNamespace + ".PrepareSchemaUpdate.sql"),
                    14400,
                /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.DropProcedure(
                    "dbo.sp_dropkeys",
                /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.DropProcedure(
                    "dbo.sp_dropkeys",
                /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.RunSqlNonQueryFromStream(
                    thisAssembly.GetManifestResourceStream(
                        thisNamespace + ".UpdateSchema.sql"),
                    14400,
                /*enableExternDatabase*/true,
                    out error))
                return false;

            if (!_databaseCommandExecutor.RunSqlNonQueryFromStream(
                    thisAssembly.GetManifestResourceStream(
                        thisNamespace + ".CreateSchemaIndices.sql"),
                    14400,
                /*enableExternDatabase*/true,
                    out error))
                return false;

            return true;
        }

        private bool ConversionCgpServerBeansExtern1_93(out Exception error)
        {
            return _databaseCommandExecutor.RunSqlNonQuery(
                "alter table EventlogParameter alter column Value nvarchar(max)",
                /*_creatorProperties.EnableExternDatabase*/true,
                out error);
        }

        private bool ConversionCgpServerBeansExtern1_94(out Exception error)
        {
            return _databaseCommandExecutor.RunSqlNonQuery(
                "UPDATE [Eventlog] set [Type] = 'CCU exception occurred' where [Type] = 'Exception occurred'",
                /*_creatorProperties.EnableExternDatabase*/true,
                out error);
        }

        private bool SetRecoveryAndAutogrowth(
            CreatorProperties.DatabaseFileParams dataFileParams,
            CreatorProperties.DatabaseFileParams logFileParams,
            bool isExtern,
            out Exception error)
        {
            string databaseName =
                isExtern
                    ? _creatorProperties.ExternDatabaseName
                    : _creatorProperties.DatabaseName;

            string alterRecoveryModeCommand =
                string.Format(
                    "alter database [{0}] set recovery simple",
                    databaseName);

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                alterRecoveryModeCommand,
                isExtern,
                out error))
            {
                return false;
            }

            var dbFiles = 
                _databaseCommandExecutor.RunSqlQuery(
                    "select name, type, size, max_size, growth, is_percent_growth from sys.database_files",
                    isExtern,
                    out error);

            if (dbFiles == null)
                return false;

            foreach (var currentDbFileParams in dbFiles)
            {
                var type = (byte)currentDbFileParams[1];

                CreatorProperties.DatabaseFileParams currentFileParams;

                switch (type)
                {
                    case 0:
                        currentFileParams = dataFileParams;
                        break;

                    case 1:
                        currentFileParams = logFileParams;
                        break;

                    default:
                        continue;
                }

                var updateCommand =
                    new StringBuilder();

                updateCommand.AppendFormat(
                    "alter database [{0}] modify file ",
                    databaseName);

                var maxSize = (int)currentDbFileParams[3];

                if (maxSize > 0)
                    maxSize /= 128;

                var growth = (int)currentDbFileParams[4];
                var isPercentGrowth = (bool)currentDbFileParams[5];

                bool fileParametersChanged = 
                    currentFileParams.AppendToUpdateCommandText(
                        updateCommand,
                        (string)currentDbFileParams[0],
                        (int)currentDbFileParams[2] / 128,
                        maxSize,
                        isPercentGrowth
                            ? growth
                            : growth / 128,
                        isPercentGrowth);

                if (!fileParametersChanged)
                    return true;

                if (!_databaseCommandExecutor.RunSqlNonQuery(
                    updateCommand.ToString(),
                    isExtern,
                    out error))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ConversionCgpServerBeansExtern1_95(out Exception error)
        {
            return
                SetRecoveryAndAutogrowth(
                    _creatorProperties.ExternDatabaseDataFileParams,
                    _creatorProperties.ExternDatabaseLogFileParams,
                    true,
                    out error);
        }

        private bool ConversionCgpServerBeansExtern1_96(out Exception error)
        {
            Type type = GetType();

            Assembly thisAssembly = type.Assembly;
            string thisNamespace = type.Namespace;

            if (!_databaseCommandExecutor.RunSqlNonQueryFromStream(
                    thisAssembly.GetManifestResourceStream(
                        thisNamespace + ".CreateTableSystemVersion.sql"),
                    14400,
                    true,
                    out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.DeleteVersionFromDatabase(
                    CGP_SERVER_BEANS_EXTERN_ASSEMBLY_NAME,
                    false,
                    out error))
            {
                return false;
            }

            error = null;
            return true;
        }

        private bool ConversionCgpServerBeansExtern1_97(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "CREATE TABLE TimetecData (id int not null primary key, LastEventId bigint)",
                true,
                out error))
            {
                return false;
            }

            if (!_databaseCommandExecutor.RunSqlNonQuery(
                "CREATE TABLE TimetecErrorEvents (id int identity(1,1) not null primary key, ErrorEventId bigint)",
                true,
                out error))
            {
                return false;
            }

            return true;
        }

        private bool ConversionCgpServerBeansExtern2_00(out Exception error)
        {
            if (!_databaseCommandExecutor.RunSqlNonQuery(
                  @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ConsecutiveEvents' AND xtype='U')
                    BEGIN
                        CREATE TABLE ConsecutiveEvents (id int identity(1,1) primary key, LastEventlogId bigint null, SourceId uniqueidentifier, ReasonId uniqueidentifier, LastEventDateTime datetime2(3) null)
                    END
                   ",
                true,
                out error))
            {
                return false;
            }

            return true;
        }
    }
}
