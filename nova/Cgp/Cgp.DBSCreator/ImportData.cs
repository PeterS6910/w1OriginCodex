using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.IwQuick.Crypto;

namespace Contal.Cgp.DBSCreator
{
    [Serializable()]
    public enum SymbolDataType
    {
        Vector, Raster
    }

    public static class ImportData
    {
        private static string FILES_PATH = "Graphics symbols\\";

        private static int GetSymbolType(string symbolTypeName)
        {
            if (string.IsNullOrEmpty(symbolTypeName))
                return -1;

            foreach (SymbolType symbolType in Enum.GetValues(typeof (SymbolType)))
            {
                if (symbolType.ToString().ToLower() == symbolTypeName.ToLower())
                {
                    return (int) symbolType;
                }
            }

            return -1;
        }

        private static int GetState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return -1;

            foreach (State state in Enum.GetValues(typeof(State)))
            {
                if (state.ToString().ToLower() == stateName.ToLower())
                {
                    return (int)state;
                }
            }

            return -1;
        }

        public static bool InsertTimetecSetting(DatabaseCommandExecutor databaseCommandExecutor, out Exception error)
        {
            error = null;

            var type = typeof (ImportData);
            var thisAssembly = type.Assembly;
            string thisNamespace = type.Namespace;

            var stream = thisAssembly.GetManifestResourceStream(
                thisNamespace + ".contaltimetecnova.cer");

            var certificateByteArray = QuickCrypto.Encrypt(
                StreamToByteArray(stream), CgpServerGlobals.DATABASE_KEY.ToString());

            int insertRowsCount = databaseCommandExecutor.RunSqlNonQueryWithParameters(
                "INSERT INTO TimetecSetting (IdTimetecSetting, IsEnabled, IpAddress, Port, CertificateData) VALUES(0,0,'127.0.0.1', 9821, @data)",
                false,
                new SqlParameterTypeAndValue(SqlDbType.Image, certificateByteArray));

            if (insertRowsCount == 0)
                return false;

            return true;
        }

        private static byte[] StreamToByteArray(Stream input)
        {
            var buffer = new byte[16 * 1024];

            using (var ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);

                return ms.ToArray();
            }
        }

        public static bool ImportGraphicsSymbols(DatabaseCommandExecutor databaseCommandExecutor, out Exception error)
        {
            //remove old datas from tables
            if (!databaseCommandExecutor.RunSqlNonQuery("TRUNCATE TABLE GraphicSymbolTemplate", false, out error))
                return false;

            if (!databaseCommandExecutor.RunSqlNonQuery("TRUNCATE TABLE GraphicSymbolRawData", false, out error))
                return false;

            if (!databaseCommandExecutor.RunSqlNonQuery("TRUNCATE TABLE GraphicSymbol", false, out error))
                return false;

            string[] filesPath;
            try
            {
                filesPath = 
                    Directory.GetFiles(
                        Path.Combine(
                            Application.StartupPath,
                            FILES_PATH));
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
            
            string templateName = "Vector";

            if (filesPath == null || filesPath.Length == 0)
                return false;

            //create template
            var idTemplate = Guid.NewGuid();

            string command = string.Format("INSERT INTO GraphicSymbolTemplate (Id, Name) VALUES ('{0}','{1}');", 
                idTemplate.ToString(), templateName);

            if (!databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                return false;

            //create graphics symbols
            foreach (string filePath in filesPath)
            {
                if (string.IsNullOrEmpty(filePath))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string ext = Path.GetExtension(filePath);
                var parameters = fileName.Split('_');
                SymbolDataType dataType;

                switch (ext.ToLower())
                {
                    case ".bmp":
                    case ".jpg":
                    case ".png":

                        dataType = SymbolDataType.Raster;

                        break;

                    case ".svg":

                        dataType = SymbolDataType.Vector;

                        break;

                    default:
                        continue;
                }

                if (parameters == null || parameters.Length < 2)
                    continue;

                //insert rawdata
                var idRawData = Guid.NewGuid();
                byte[] rawData;
                try
                {
                    rawData = File.ReadAllBytes(filePath);
                }
                catch (Exception ex)
                {
                    error = ex;
                    continue;
                }

                command = "INSERT INTO GraphicSymbolRawData (Id, RawData, DataType) VALUES(@Id, @RawData, @DataType)";
                int insertedCount= databaseCommandExecutor.RunSqlNonQueryWithParameters(command, false,
                    new SqlParameterTypeAndValue(SqlDbType.UniqueIdentifier, idRawData),
                    new SqlParameterTypeAndValue(SqlDbType.Image, rawData),
                    new SqlParameterTypeAndValue(SqlDbType.TinyInt, dataType));
                
                if (insertedCount == 0)
                    return false;

                int symbolTypeNumber = GetSymbolType(parameters[0]);
                int stateNumber = GetState(parameters[1]);

                if (symbolTypeNumber == -1 || stateNumber == -1)
                    continue;

                string filterKey = string.Empty;

                if (parameters.Length == 3)
                {
                    filterKey = parameters[2];
                }
                else if ((State) stateNumber == State.Unknown ||
                         (State) stateNumber == State.Offline)
                {
                    filterKey = parameters[0];
                }

                //insert graphics symbol
                command = string.Format("INSERT INTO GraphicSymbol (Id, SymbolType, SymbolState, FilterKey, IdTemplate, IdRawData) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                    Guid.NewGuid().ToString(), 
                    symbolTypeNumber.ToString(),
                    stateNumber.ToString(),
                    filterKey,
                    idTemplate.ToString(),
                    idRawData.ToString());

                if (!databaseCommandExecutor.RunSqlNonQuery(command, false, out error))
                    return false;
            }

            return true;
        }
    }
}
