namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    /// <summary>
    /// 
    /// </summary>
    public enum NodeCommand : byte
    {
        /* Upgrade commands */
        ResetToBootloader           = 0x00,
        ResetToApplication          = 0x01,
        StartUpgrade                = 0x02,
        WriteData                   = 0x03,
        UpgradeDone                 = 0x04,
        DataWrittenOk               = 0x05,
        ResendData                  = 0x06,
        ErasePage                   = 0x07,
        EraseOk                     = 0x08,
        EraseError                  = 0x09,
        ReadyForUpgrade             = 0x0A,
        WriteChecksum               = 0x0B,
        WriteAppLength              = 0x0C,
        ControlDataOk               = 0x0D,
        ControlDataError            = 0x0E,
        RequestUpgrade              = 0x0F,
        InBootloader                = 0x10,
        ReadBaseAddress             = 0x11,
        BaseAddress                 = 0x12,

        /* DSM commands */
        SetPushButton               = 0x10,
        UnsetPushButton             = 0x11,
        SetSensor                   = 0x12,
        UnsetSensor                 = 0x13,
        SetElectricStrike           = 0x14,
        SetBypassAlarm              = 0x15,
        UnsetActuator               = 0x16,
        EnableAlarms                = 0x17,
        SetTimmings                 = 0x18,
        SetSpecialOutput            = 0x19,
        UnsetSpecialOutput          = 0x1A,
        StartDSM                    = 0x1B,
        StopDSM                     = 0x1C,
        SignalAccessGranted         = 0x1D,
         
        // obsolete
        //InvokeIntrusionAlarm      = 0x1E,
        //ForceUnlockedState        = 0x1F,
        

        /* Read commands */
        ReadDeviceInfo              = 0x20,
        //ReadInputCount            = 0x20,
        //ReadOutputCount           = 0x21,
        ReadFWVersion               = 0x22,
        ReadMemoryLoad              = 0x23,
        DebugSequenceRequest        = 0x24,
        
        /* Input configuration */
        UnsetInput                  = 0x30,
        SetBSILevels                = 0x31,
        SetBSIParams                = 0x32,
        SetDIParams                 = 0x33,
        BindOutputToInput           = 0x34,
        UnbindOutputFromInput       = 0x35,
        RemapBSI                    = 0x36,
        RemapDI                     = 0x37,
        
        /* Output configuration */
        SetReportedOutput           = 0x38,
        ConfigOutputLevel           = 0x39,
        ConfigOutputFrequency       = 0x3A,
        ConfigOutputPulse           = 0x3B,
        ActivateOutput              = 0x3C,
        UnsetReportedOutput         = 0x3D,
        ForceSwitchOff              = 0x3E,
        SetBlockedOutputsEx         = 0x3F,

        /* CR commands */
        CrRequest                   = 0x40,
        CrAck                       = 0x41,

        /* Report commands */
        Ack                         = 0x50,
        Nack                        = 0x51,
        InputChanged                = 0x52,
        SpecialInputChanged         = 0x53,
        DIPChanged                  = 0x54,
        OutputChanged               = 0x55,
        DSMChanged                  = 0x56,
        DeviceInfo                  = 0x57,
        //InputCount                = 0x57,
        //OutputCount               = 0x58,
        OutputLogicState            = 0x59,

        OutputBindingSuspendResume  = 0x5A,

        /* */
        CrOnlineStateChanged        = 0x60,
        CrDataReceived              = 0x61,
        FWVersion                   = 0x62,
        MemoryLoad                  = 0x63,
        DebugSequenceResponse       = 0x64,
        MemoryRestored              = 0x65,
        CrCountryCodeConfirm         = 0x66,

        /* Misc commands */
        SetTime                     = 0x80,
        RestartNode                 = 0x81,
        IOTestProgress              = 0x82,
        IOTestTimeout               = 0x83,
        StartIOTest                 = 0x84,
        IOTestOk                    = 0x85,
        SetCRPLevel                 = 0x86,

        ToggleCardGenerator         = 0x90,
        ToggleADCGenerator          = 0x91,

        /* DSM continued */
        SetCRs                      = 0xA0,
        SuppresCR                   = 0xA1,
        LooseCR                     = 0xA2,
        SetImplicitCode             = 0xA3,
        SetPushButtonPure           = 0xA4,

        /* input/output reporting */
        SetReportedOutputsLogic     = 0xC0,
        UnsetReportedOutputsLogic   = 0xC1,
        SetReportedOutputsLogicEx   = 0xC2,
        SetReportedOutputsEx        = 0xC3,

        SetReportedInputs           = 0xC4,
        UnsetReportedInputs         = 0xC5,
        SetReportedInputsEx         = 0xC6,
    }
}