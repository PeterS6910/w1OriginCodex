using System;
using System.Collections.Generic;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface IServerGenaralOptionsProvider
    {
        void SaveToRegistrySmtp(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistrySerialPort(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryDatabaseBackup(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryDatabaseEventlogExpiration(ServerGeneralOptions serverGeneralOptions);
        void SaveToDatabaseCustomerAndSupplierInfo(ServerGeneralOptions serverGeneralOptions);
        void SaveSupplierLogo(BinaryPhoto binaryPhoto);
        BinaryPhoto GetSupplierLogo();
        void SaveToRegistryColourSettings(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryAutoCloseSettings(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistrySecuritySettings(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryEventlogs(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryAdvancedAccessSettings(ServerGeneralOptions serverGeneralOptions);
        void SaveToRegistryAdvancedSettings(ServerGeneralOptions serverGeneralOptions);
        bool IsSetSMTP();
        bool IsSetSerialPort();
        bool IsLockCLientApplication();
        ServerGeneralOptions ReturnServerGeneralOptions();
        bool TimetecCommunicationIsEnabled();
    }
}
