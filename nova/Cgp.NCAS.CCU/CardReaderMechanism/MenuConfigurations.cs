using Contal.Drivers.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public static class MenuConfigurations
    {
        public static readonly CRMenuConfiguration LegacyMenuConfiguration;

        private static readonly CRMenuConfiguration NewMainMenuConfiguration;
        private static readonly CRMenuConfiguration NewTimeBuyingMenuConfiguration;
        private static readonly CRMenuConfiguration NewAlarmAreasMenuConfiguration;
        private static readonly CRMenuConfiguration NewSensorsMenuConfiguration;
        private static readonly CRMenuConfiguration NewSensorEditMenuConfiguration;
        private static readonly CRMenuConfiguration NewEditAlarmAreaMenuConfiguration;
        private static readonly CRMenuConfiguration NewSensorsByAlarmAreaMenuConfiguration;
        private static readonly CRMenuConfiguration NewUnconditionalSetMenuConfiguration;
        private static readonly CRMenuConfiguration NewEventlogMenuConfiguration;
        private static readonly CRMenuConfiguration NewAlarmStateForAlarmAreaMenuConfiguration;
        private static readonly CRMenuConfiguration NewEventlogsMenuConfiguration;
        private static readonly CRMenuConfiguration NewAvailableAlarmAreasMenuConfiguration;
        private static readonly CRMenuConfiguration NewAvailableSensorsMenuConfiguration;

        static MenuConfigurations()
        {
            LegacyMenuConfiguration = new CRMenuConfiguration(true);

            NewMainMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    VisibleColumns = 2,
                    VisibleRows = 3
                };

            NewTimeBuyingMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    VisibleColumns = 2,
                    VisibleRows = 2
                };

            NewAlarmAreasMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewSensorsMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewSensorEditMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewEditAlarmAreaMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewSensorsByAlarmAreaMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewUnconditionalSetMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewAlarmStateForAlarmAreaMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewEventlogMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewEventlogsMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };

            NewAvailableAlarmAreasMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8,
                    MenuTimeoutDisabled = true
                };

            NewAvailableSensorsMenuConfiguration =
                new CRMenuConfiguration(false)
                {
                    GridLinesVisible = false,
                    MenuHeaderEnabled = true,
                    NavigationButtonsEnabled = true,
                    TextAlignCenter = false,
                    VisibleColumns = 1,
                    VisibleRows = 8
                };
        }

        private static bool NewMenuConfigurationShouldBeUsed(CardReader cardReader)
        {
            return cardReader.FwVersion >= CRMenuCommands.GraphicalMenuVersionThreshold;
        }

        public static CRMenuConfiguration GetMainMenuConfiguration(CardReader cardReader)
        {
            return
                NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewMainMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetTimeBuyingMenuConfiguration(CardReader cardReader)
        {
            return
                NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewTimeBuyingMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetSensorEditMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewSensorEditMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetEditAlarmAreaMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewEditAlarmAreaMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetSensorsByAlarmAreaMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewSensorsByAlarmAreaMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetUnconditionalSetMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewUnconditionalSetMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetEventlogMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewEventlogMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetAlarmStateForAlarmAreaMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewAlarmStateForAlarmAreaMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetEventlogsMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewEventlogsMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetAvailableAlarmAreasMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewAvailableAlarmAreasMenuConfiguration
                    : LegacyMenuConfiguration;
        }

        public static CRMenuConfiguration GetAvailableSensorsMenuConfiguration(CardReader cardReader)
        {
            return NewMenuConfigurationShouldBeUsed(cardReader)
                    ? NewAvailableSensorsMenuConfiguration
                    : LegacyMenuConfiguration;
        }
    }
}
