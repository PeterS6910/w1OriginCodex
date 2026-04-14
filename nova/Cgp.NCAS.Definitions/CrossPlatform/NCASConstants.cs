namespace Contal.Cgp.NCAS.Definitions
{
    public class NCASConstants
    {
        #region TCP ports

        public const int TCP_CCU_REMOTING_PORT = 63002;
        public const int TCP_DATA_CHANNEL_PORT = 63003;
        public const int TCP_LW_MESSAGING_PORT = 63004;
        public const int TCP_ICCU_PORT = 63005;
        #endregion

        #region CCU40
        public const int CCU40_MAX_DSM_COUNT = 8;
        public const int CCU40_MAX_EXPANDALBLE_DSM_COUNT = 16;

        public const int CCU40_MAX_CR_COUNT = 16;

        public const int CCU40_MAX_STANDARD_INPUT_COUNT = 24;
        public const int CCU40_MAX_STANDARD_OUTPUT_COUNT = 8;

        public const int CCU40_MAX_CAPABLE_INPUT_COUNT = 24;
        public const int CCU40_MAX_CAPABLE_OUTPUT_COUNT = 16;

       
        #endregion

        #region CCU12

        public const int CCU12_MAX_DSM_COUNT = 4;
        public const int CCU12_MAX_CR_COUNT = 8;

        public const int CCU12_MAX_STANDARD_INPUT_COUNT = 8;
        public const int CCU12_MAX_STANDARD_OUTPUT_COUNT = 4;

        #endregion

        #region CCU05
        public const int CCU05_MAX_DSM_COUNT   = 2;
        public const int CCU05_MAX_CR_COUNT = 4;

        public const int CCU05_STANDARD_INPUT_COUNT = 3;
        public const int CCU05_STANDARD_OUTPUT_COUNT = 2;
        #endregion

        #region CAT12CE
        public const int CAT12CE_MAX_DSM_COUNT = 4;
        public const int CAT12CE_MAX_CR_COUNT = 8;

        public const int CAT12CE_MAX_STANDARD_INPUT_COUNT = 8;
        public const int CAT12CE_MAX_STANDARD_OUTPUT_COUNT = 4;

        public const int CAT12CE_MAX_CAPABLE_INPUT_COUNT = 16;
        public const int CAT12CE_MAX_CAPABLE_OUTPUT_COUNT = 8;

        #endregion

        
        public const string EVENTS_FROM_AUTONOMOUS_RUN_STREAM_NAME = "EventsFromAutonomousRun";
        public const uint CCU_MEMORY_LOAD_TRESHOLD = 91;
        public const int MAX_WAITING_TIME_FOR_FINISHED_PROCESSING_STREAM = 300000;

        #region Version thresholds
        public const int NEW_AES_CE_VERSION_THRESHOLD = 3040;
        public const int FW_VERSION_GET_LOCAL_TIME_WITH_MILISCONDS = 3055;

        public const int FW_VERSION_ENABLED_UNCONFIGURE_OUTPUT = 3037;

        public const int MinimalCeVersionForCcu21 = 3091;
        public const int MinimalReleasedCcuVersionForNova21 = 6018;

        #endregion

        public class InputBalancingLevels
        {
            public readonly decimal ToLevel1;
            public readonly decimal ToLevel2;
            public readonly decimal ToLevel3;

            public InputBalancingLevels(decimal toLevel1, decimal toLevel2, decimal toLevel3)
            {
                ToLevel1 = toLevel1;
                ToLevel2 = toLevel2;
                ToLevel3 = toLevel3;
            }
        }

        public static InputBalancingLevels Balancing4k7 = new InputBalancingLevels(3.6m, 9m, 15.3m);
        public static InputBalancingLevels BalancingLegacy = new InputBalancingLevels(6.4m, 8.6m, 11.6m);
        public static InputBalancingLevels BalancingCat12ce = new InputBalancingLevels(5m, 10m, 15m);



        public static InputBalancingLevels DefaultBalancingLevels = Balancing4k7;
    }
}
