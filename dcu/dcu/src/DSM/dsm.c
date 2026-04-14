#ifndef BOOTLOADER

#include "dsm.h"
#include "dsmConfig.h"
#include "IO\Inputs.h"
#include "IO\Outputs.h"
#include "System\Timer\sysTick.h"
#include "System\Tasks.h"
#ifndef LON
    #ifdef RS485
    #include "Communication\Clsp485\clsp485.h"
    #else
    #include "Clsp485\clsp485.h"
    #endif
#else
#include "Communication\clspGeneric.h"
#endif
#include "CardReaders\CrProtocol.h"
#include "CardReaders\CrCommunicator.h"
#include "System\dmHandler.h"

#define DSM_TESTING 1

#pragma diag_suppress=Pa082

volatile DSMMachine_t _dsmMachine;
volatile bool _dsmIsRunning = false;

/* DSM configuration */
extern volatile DSMConfiguration_t _dsmConfig;                                  
extern void (*DSM_OnStateChanged)(DSMSignals_e, DSMSignalInfo_e, DSMAccessGrantedSource_e pushButtonId);

void DSM_SetLocks(uint32 open);


/** 
 * Callback for sending messages to master node from DSM machine. Only requested 
 * message events are sent
 *
 * @param signal - event descriptor
 * @param info - event detail 
 */
void DSM_Callback(DSMSignals_e signal, DSMSignalInfo_e info, DSMAccessGrantedSource_e agSource)
{    
    if (DSM_OnStateChanged != null) 
    {
        switch (signal)
        {
            case SignalAjar:
                if (_dsmConfig.doorAjarAlarmEnabled)
                    DSM_OnStateChanged(signal, InfoNone, agSource);
                break;
            case SignalIntrusion:
                if (_dsmConfig.intrusionAlarmEnabled)
                    DSM_OnStateChanged(signal, info, agSource);
                break;
            case SignalSabotage:
                if (_dsmConfig.sabotageAlarmEnabled)
                    DSM_OnStateChanged(signal, info, agSource);
                break;
            default:
                DSM_OnStateChanged(signal, info, agSource);
                break;
        }
    }
    
    _dsmMachine.agSource = AGS_None;      // just to be sure that it is not send twice
}

void DSM_HostStateChanged(uint32 suggestedAddress, bool isAssigned)
{
    if (isAssigned)
    {
        DSM_Stop();
        
        Inputs_UnbindAll();
        Outputs_TurnAllOff();
    }
}

int32 DSM_IsCRAssigned(uint32 address)
{
    for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address == address)
            return 1;
    }
    
    return 0;
}

int32 DSM_IsCRSuppressed(uint32 address)
{
    int crId = -1;
    for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address == address)
        {
            crId = i;
            break;
        }
    }
    
    if (crId != -1)
        return (_dsmConfig.cardReaders[crId].flags & DSM_CR_SUPPRESSED);
    
    return 0;
}

volatile DSMSignals_e _lastSignalToCR = SignalNone;

void DSM_SendImplicitCRCommand(uint32 address, uint32 crId, bool revokeDisplayInfoTab) {
    
    byte implicitCode = _dsmConfig.cardReaders[crId].implicitCode;
    
    if (implicitCode == CRMC_DOOR_UNLOCKED &&
         ((_dsmConfig.cardReaders[0].address != 0 && (_dsmConfig.cardReaders[0].flags & DSM_CR_LOCKED)) ||
          (_dsmConfig.cardReaders[1].address != 0 && (_dsmConfig.cardReaders[1].flags & DSM_CR_LOCKED))))
        implicitCode = CRMC_DOOR_LOCKED;             
    /*else
        if (_dsmMachine.forceUnlocked && (implicitCode == CRMC_WAITING_FOR_CARD  || implicitCode == CRMC_WAITING_FOR_GIN_CODE ))
            implicitCode = CRMC_DOOR_UNLOCKED;*/
            
         
    // optional parameter below zero means either parameter not used (-1) or other flags
    DSM_SendCRRawCmd(address, (CRMessageCode_t)implicitCode, _dsmConfig.cardReaders[crId].optionalParameter);
    
    if (_dsmConfig.cardReaders[crId].flags & DSM_CR_MSG_SET)
        CrCommunicator_ConceptAndEnqueue(address, 
                                         (CRMessageCode_t)_dsmConfig.cardReaders[crId].msgCode,
                                         _dsmConfig.cardReaders[crId].optDataLength,
                                         _dsmConfig.cardReaders[crId].optData,
                                         false, false);
    
    if (revokeDisplayInfoTab)
        // nochange, nochange, nochange, empty
        CR_RawDisplayInfoTab(address, CRDITI_NO_CHANGE, 0, CRDITI_NO_CHANGE, 0, CRDITI_NO_CHANGE, 0, CRDITI_EMPTY, 0);
}


void DSM_InformCR(uint32 address, DSMSignals_e signal)
{    
    if (DSM_IsCRSuppressed(address))
        return;
    
    /* Check if the CR with given address is assigned */
    int crId = -1;
    for (int i = 0; i < CR_MAX_COUNT; i++)
    {
        if (_dsmConfig.cardReaders[i].address == address)
        {
            crId = i;
            break;
        }
    }
    
    /* CR address does not exists -> exit */
    if (crId == -1)
        return;
    
    switch (signal)
    {
        case SignalLocked:
            DSM_SendImplicitCRCommand(address, crId,true);
            break;

        case SignalUnlocking:
            //DSM_SendCRRawCmd(address, );
            break;
        case SignalUnlocked:
            
            // send optional cmd as well
            if (_dsmMachine.forceUnlocked)
            {
                // here should be enumeration of the virtual commands making the need of different CR visualisation
                // than CRMC_DOOR_UNLOCKED
                // BEWARE - DSM is still in unlocked state though
                if (_dsmConfig.cardReaders[crId].implicitCode != CRMC_ALARM_AREA_TMP_UNSET)
                {
                    DSM_SendCRRawCmd(address, CRMC_DOOR_UNLOCKED,-1);
                    
                    // nochange, nochange, nochange , doorUnlocked
                    // explicitly begfore setting the optional code, as that can modify it
                    CR_RawDisplayInfoTab(address, CRDITI_NO_CHANGE, 0, 0x80, 0, 0x80, 0, 0x1, 0);
                  
                    if (_dsmConfig.cardReaders[crId].flags & DSM_CR_MSG_SET)
                    {
                        CrCommunicator_ConceptAndEnqueue(address, 
                                                         (CRMessageCode_t)_dsmConfig.cardReaders[crId].msgCode,
                                                         _dsmConfig.cardReaders[crId].optDataLength,
                                                         _dsmConfig.cardReaders[crId].optData,
                                                         false, false);
                    }
                }
                else {
                    if (_dsmConfig.cardReaders[crId].optionalParameter >= 0)
                        _dsmConfig.cardReaders[crId].optionalParameter |= CRATUF_NO_MASTER_CMD;
                    else
                        _dsmConfig.cardReaders[crId].optionalParameter = CRATUF_NO_MASTER_CMD;
                      
                    DSM_SendCRRawCmd(address, CRMC_DOOR_UNLOCKED,-1);
                  
                    // makes transcription of the virtual commands
                    DSM_SendImplicitCRCommand(address, crId,false);
                }
            }
            else {
                DSM_SendCRRawCmd(address, CRMC_DOOR_UNLOCKED,-1);
            
                // nochange, nochange, nochange , doorUnlocked
                CR_RawDisplayInfoTab(address, CRDITI_NO_CHANGE, 0, 0x80, 0, 0x80, 0, 0x1, 0);
            }

            break;
        case SignalDoorOpened:
            DSM_SendCRRawCmd(address, CRMC_DOOR_OPEN,-1);
            
             // nochange, nochange, nochange, doorOpened
            CR_RawDisplayInfoTab(address, 0x80, 0, 0x80, 0, 0x80, 0, 0x1, 1);

            break;
        case SignalLocking:
            DSM_SendCRRawCmd(address, CRMC_DOOR_LOCKING,-1);
            break;
        case SignalIntrusion:
            if (_dsmConfig.cardReaders[crId].intrusionOnlyViaLed)
            {
                DSM_SendImplicitCRCommand(address,crId,false);
                CR_RawPeripheralMode(address, CRIM_HIGH_FREQUENCY, CRIM_OFF, CRIM_ON, CRIM_ON);
            }
            else
            {
                DSM_SendCRRawCmd(address, CRMC_ALARM_INTRUSION,-1);
            }
            
             // nochange, nochange,  nochange, closedoor
            CR_RawDisplayInfoTab(address, 0x80, 0, 0x80, 0,  0x80, 0, 0x5, 1);
            break;
            /*
        case SignalSabotage:
            DSM_SendCRRawCmd(_dsmConfig.cardReaders[0].address, CRMC_DOOR_UNLOCKED);
            break;*/
        case SignalPreAjar:
            if (_dsmConfig.cardReaders[crId].intrusionOnlyViaLed)
            {
                DSM_SendImplicitCRCommand(address,crId,false);
                CR_RawPeripheralMode(address, CRIM_OFF, CRIM_OFF, CRIM_HIGH_FREQUENCY, CRIM_HIGH_FREQUENCY);
            }
            else
                DSM_SendCRRawCmd(address, CRMC_WARNING_DOOR_OPENED_TOO_LONG,-1);
            
             // nochange, nochange, nochange, closedoor            
            CR_RawDisplayInfoTab(address, 0x80, 0, 0x80, 0,  0x80, 0, 0x5, 1);
            break;
        case SignalAjar:
            if (_dsmConfig.cardReaders[crId].intrusionOnlyViaLed)
            {
                DSM_SendImplicitCRCommand(address,crId,false);
                CR_RawPeripheralMode(address, CRIM_HIGH_FREQUENCY, CRIM_OFF, CRIM_ON, CRIM_ON);
            }
            else
                DSM_SendCRRawCmd(address, CRMC_ALARM_DOOR_OPENED_TOO_LONG,-1);
            
             // nochange, nochange, nochange, closedoor            
            CR_RawDisplayInfoTab(address, 0x80, 0, 0x80, 0,  0x80, 0, 0x5, 1);
            break;
    }
}

void DSM_InformCRs(DSMSignals_e signal)
{
    _lastSignalToCR = signal;
    
    for (int i = 0; i < 2; i++)
        DSM_InformCR(_dsmConfig.cardReaders[i].address, signal);
}



void DSM_SendCRRawCmd(uint32 cr, CRMessageCode_t cmd, int32 param)
{
    if (!DSM_IsCRAssigned(cr))
        return;
    if (DSM_IsCRSuppressed(cr))
        return;
    
    CR_RawAccessCommand(cr, cmd, param);
}


Result_e DSM_SuppressCR(uint32 cr)
{
    if (cr > MAX_CARD_READER_ADDR)
        return R_CROutOfRange;
    
    int cardReaderId = -1;
    for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address == cr)
        {
            cardReaderId = i;
            break;
        }
    }
    
    if (cardReaderId != -1)
    {
        _dsmConfig.cardReaders[cardReaderId].flags |= DSM_CR_SUPPRESSED;
        return R_Ok;
    }
    
    return R_CardReaderNotAssigned;
}

Result_e DSM_LooseCR(uint32 cr)
{
    if (cr > MAX_CARD_READER_ADDR)
        return R_CROutOfRange;
    
    int cardReaderId = -1;
    for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address == cr)
        {
            cardReaderId = i;
            break;
        }
    }
    
    if (cardReaderId != -1)
    {
        _dsmConfig.cardReaders[cardReaderId].flags &= ~DSM_CR_SUPPRESSED;
        DSM_InformCR(cr, _lastSignalToCR);
        return R_Ok;
    }
    
    return R_CardReaderNotAssigned;
}

// predefinition
DSMSignals_e DSM_ForceUnlocked(uint32 isUnlocked,bool refreshCRs);

static DSMSignals_e DSM_HandleUnlockedState(bool refreshCRs)
{
    bool forceUnlocked = 
        (_dsmConfig.cardReaders[0].address != 0 && (_dsmConfig.cardReaders[0].implicitCode == CRMC_DOOR_UNLOCKED)) ||
        (_dsmConfig.cardReaders[1].address != 0 && (_dsmConfig.cardReaders[1].implicitCode == CRMC_DOOR_UNLOCKED));
    
    bool atLeastOneLocked = 
        (_dsmConfig.cardReaders[0].address != 0 && (_dsmConfig.cardReaders[0].flags & DSM_CR_LOCKED)) ||
        (_dsmConfig.cardReaders[1].address != 0 && (_dsmConfig.cardReaders[1].flags & DSM_CR_LOCKED));
    
    if (atLeastOneLocked)
        forceUnlocked = false;
    
    return DSM_ForceUnlocked(forceUnlocked,refreshCRs);
}

bool DSM_ShouldBeLockedByCrSettings(uint32 implicitCode,uint32 optionalParamUsed,uint32 optionalParam) {
    return 
        (implicitCode==CRMC_DOOR_LOCKED ||
         implicitCode==CRMC_ALARM_AREA_IS_SET ||
         implicitCode==CRMC_ALARM_AREA_IN_ALARM) &&
          (!optionalParamUsed || 
          ((optionalParam & DSM_CR_LOCKED_WITH_EXCEPTION) != DSM_CR_LOCKED_WITH_EXCEPTION)
           );
            
}

Result_e DSM_AssignCRs(uint8* data)/* (uint32 cr1, uint32 implicitCode1, uint32 isOptParam1, uint32 optParam1,
                       uint32 setCR1, uint32 msgCode1, uint32 optDataLength1, uint8* optData1,
                       uint32 cr2, uint32 implicitCode2, uint32 isOptParam2, uint32 optParam2,
                       uint32 setCR2, uint32 msgCode2, uint32 optDataLength2, uint8* optData2)*/
{
    uint32 crs[MAX_CARD_READER_ADDR];
    uint32 implicitCodes[MAX_CARD_READER_ADDR];
    uint32 isOptParams[MAX_CARD_READER_ADDR];
    uint32 optParams[MAX_CARD_READER_ADDR];
    uint32 crMsgPresents[MAX_CARD_READER_ADDR];
    bool intrusionOnlyViaLeds[MAX_CARD_READER_ADDR];
    
    int i;
    uint32 nextId = 0;
    const uint32 blockSize = 6;
    
    for (i=0;i<MAX_CARD_READER_ADDR;i++) 
    {
        
        crs[i] = data[i*blockSize];
        if (crs[i] > MAX_CARD_READER_ADDR)
            return R_CROutOfRange;
        
        implicitCodes[i] = data[i*blockSize+1];
        isOptParams[i] = data[i*blockSize+2];
        optParams[i] = data[i*blockSize+3];
        crMsgPresents[i] = data[i*blockSize+4];
        
        intrusionOnlyViaLeds[i] = data[i*blockSize+5];
        
        nextId =i*blockSize+6; 
    }
    
//    uint32 cr2 = data[7];
//    uint32 implicitCode2 = data[8];
//    uint32 isOptParam2 = data[9];
//    uint32 optParam2 = data[10];
//    uint32 crMsgPresent2 = data[11];
//    bool intrusionOnlyViaLeds2 = data[12];
    
    if (_dsmConfig.locked)
        return R_DSMSettingsLocked;
    
   
    
    /*if (implicitCode1 < 0x40 || implicitCode1 > 0x5f)
        return R_OutOfAccessCmdRange;
    
    if (implicitCode2 < 0x40 || implicitCode2 > 0x5f)
        return R_OutOfAccessCmdRange;*/
    
    uint8* tmpBuffer;
    
    
    for(i=0;i<MAX_CARD_READER_ADDR;i++) 
    {
        /* Configuration for the CR 1 */
        _dsmConfig.cardReaders[i].address = crs[i];
        _dsmConfig.cardReaders[i].flags = 0;
        
        _dsmConfig.cardReaders[i].implicitCode = implicitCodes[i];
        
        if (DSM_ShouldBeLockedByCrSettings(implicitCodes[i],isOptParams[i],optParams[i]))
            _dsmConfig.cardReaders[i].flags |= DSM_CR_LOCKED;
        
        _dsmConfig.cardReaders[i].optionalParameter = (isOptParams[i] != 0) ? optParams[i] : -1;
        _dsmConfig.cardReaders[i].intrusionOnlyViaLed = intrusionOnlyViaLeds[i];
        
        // this should be already handled by either previous StopDSM or by initial state of DCU
        // HOWEVER, as AssignCR can be called multiple times due various reasons,
        // do not rely on StopDSM deallocating this
            //if (_dsmConfig.cardReaders[i].optData != null) 
            {
                Delete(_dsmConfig.cardReaders[i].optData);
                _dsmConfig.cardReaders[i].optData = null;
            }
        
       
        if (crMsgPresents[i])
        {
            uint32 msgCode1 = data[nextId++];
            uint32 optDataLen1 = data[nextId++];
                   
            tmpBuffer = New(optDataLen1);
    
            if (tmpBuffer == null)
            {
                _dsmConfig.cardReaders[i].flags &= ~DSM_CR_MSG_SET;
                return R_BufferTooSmall;
            }
            
            _dsmConfig.cardReaders[i].msgCode = msgCode1;
            _dsmConfig.cardReaders[i].optDataLength = optDataLen1;
            _dsmConfig.cardReaders[i].optData = tmpBuffer;
            
            for (int k = 0; k < optDataLen1; k++)
                _dsmConfig.cardReaders[i].optData[k] = data[nextId + k];
            
            _dsmConfig.cardReaders[i].flags |= DSM_CR_MSG_SET;
            nextId += _dsmConfig.cardReaders[i].optDataLength;
        }
        else
        {
            _dsmConfig.cardReaders[i].msgCode = 0;
            _dsmConfig.cardReaders[i].optDataLength = 0;
            // nulled above
            //_dsmConfig.cardReaders[i].optData = null;
            _dsmConfig.cardReaders[i].flags &= ~DSM_CR_MSG_SET;
        }
        
    }
   
    
    
    DSM_HandleUnlockedState(true);
    
    return R_Ok;
}

#if DEBUG
volatile uint32 _setImpCnt = 0;
#endif
Result_e DSM_SetImplicitCRCode(uint32 address, uint32 code, uint32 useOptParam, uint32 optParam, 
                               uint32 setCR, uint32 msgCode, uint32 optDataLength, byte* optData,
                               bool intrusionOnlyViaLed)
{
 
#if DEBUG
    _setImpCnt++;
#endif
    
    if (address > MAX_CARD_READER_ADDR)
        return R_CROutOfRange;
    
    /*if (code < 0x40 || code > 0x5f)
        return R_OutOfAccessCmdRange;*/
    
    int32 crId = -1;
    for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address == address)
        {
            crId = i;
            break;
        }
    }
    
    if (crId == -1)
       return R_CardReaderNotAssigned; 
    
    byte* tmpBuffer;
    
    bool implicitCmdChanged = false;    
    /* Implicit code with possible single byte parameter */
    if (_dsmConfig.cardReaders[crId].implicitCode != code ||
        _dsmConfig.cardReaders[crId].intrusionOnlyViaLed != intrusionOnlyViaLed)
        implicitCmdChanged = true;
        
    _dsmConfig.cardReaders[crId].implicitCode = code;
    _dsmConfig.cardReaders[crId].intrusionOnlyViaLed = intrusionOnlyViaLed;    
    
    // check & mark if CR is in locked state
    if (DSM_ShouldBeLockedByCrSettings(code,useOptParam,optParam))
        _dsmConfig.cardReaders[crId].flags |= DSM_CR_LOCKED;
    else
        _dsmConfig.cardReaders[crId].flags &= ~DSM_CR_LOCKED;
    
    
    
    if (useOptParam) 
    {
        if (_dsmConfig.cardReaders[crId].optionalParameter != optParam)
            implicitCmdChanged = true;
        _dsmConfig.cardReaders[crId].optionalParameter = optParam;
    }
    else 
    {
        if (_dsmConfig.cardReaders[crId].optionalParameter != -1)
            implicitCmdChanged = true;
        _dsmConfig.cardReaders[crId].optionalParameter = -1;
    }
    
    // implicit code (nor its paramter if any) has not been changed so mark it
    //if (!implicitCmdChanged)
    //_dsmConfig.cardReaders[crId].flags |= (1 << 2);
    

    bool optionalMsgChanged = false;    
    
    // if set now and not set before -> changed
    if (setCR && !(_dsmConfig.cardReaders[crId].flags & DSM_CR_MSG_SET))
        optionalMsgChanged = true;
    // if set before and not now -> changed
    if (!setCR && (_dsmConfig.cardReaders[crId].flags & DSM_CR_MSG_SET))
        optionalMsgChanged = true;
    
    // check if message the same
    if (setCR)
    {
        if (_dsmConfig.cardReaders[crId].msgCode != msgCode || 
            _dsmConfig.cardReaders[crId].optDataLength != optDataLength)
            optionalMsgChanged = true;
        
        if (_dsmConfig.cardReaders[crId].optData != null)
            for (int i = 0; i < optDataLength; i++)
            {
                if (_dsmConfig.cardReaders[crId].optData[i] != optData[i])
                    optionalMsgChanged = true;
            }
    }
    
    //if (!optionalMsgChanged)
    //   _dsmConfig.cardReaders[crId].flags |= (1 << 3);
    
    /* If there was some previous CR message free the buffer first */
    //if (_dsmConfig.cardReaders[crId].optData != null) 
    {
        Delete(_dsmConfig.cardReaders[crId].optData);
        _dsmConfig.cardReaders[crId].optData = null;
    }
    
    /* There is request to set optional CR message */
    if (setCR)
    {
        tmpBuffer = New(optDataLength);
        /* If there is no free memory clear the CR_MSG_SET flag and return error */
        if (tmpBuffer == null)
        {
            _dsmConfig.cardReaders[crId].flags &= ~DSM_CR_MSG_SET;
            return R_BufferTooSmall;
        }
        /* Set the data & copy opt. data */
        _dsmConfig.cardReaders[crId].msgCode = msgCode;
        _dsmConfig.cardReaders[crId].optDataLength = optDataLength;
        _dsmConfig.cardReaders[crId].optData = tmpBuffer;
        
        for (int i = 0; i < optDataLength; i++)
        {
            _dsmConfig.cardReaders[crId].optData[i] = optData[i];
        }
        /* Mark that the CR message is ready */
        _dsmConfig.cardReaders[crId].flags |= DSM_CR_MSG_SET;
    }
    else /* Additional CR message is not used so clear the configuration */
    {
        _dsmConfig.cardReaders[crId].msgCode = 0;
        _dsmConfig.cardReaders[crId].optDataLength = 0;
        // nulled above
        //_dsmConfig.cardReaders[crId].optData = null;
        _dsmConfig.cardReaders[crId].flags &= ~DSM_CR_MSG_SET;
    }
    
    DSMSignals_e forceUnlockedDsmState = SignalNone;
    
    // handle UNLOCKED state 
    if (implicitCmdChanged || optionalMsgChanged)
    {
        forceUnlockedDsmState = DSM_HandleUnlockedState(false);
    
        if (_dsmIsRunning) 
        { 
          // this order of conditions should be maintained  
          // first verify if there's no need to signal both CRs
          if (forceUnlockedDsmState != SignalNone)
              DSM_InformCRs(forceUnlockedDsmState);
          else
              if (_dsmMachine.state == StateLocked)  // whether is locked forcefully or by usual state workflow
                  DSM_InformCR(address, SignalLocked);
              else
                  if (_dsmMachine.state == StateUnlocked)
                      DSM_InformCR(address,SignalUnlocked);
        }
    }
    
    
    
    return R_Ok;
}


/**
 * Activate / deactivate requested actuator
 *
 * @param actuatorId - actuator enumeration
 * @param activate - 1 = activate, 0 = deactivate
 * @return result of the operation 
 */
Result_e DSM_ActivateActuator(DSMActuators_e actuatorId, uint32 activate)
{
    if (_dsmConfig.actuator[actuatorId] == UNCONFIGURED_IO)
        return R_OutputNotAssigned;
    
    Outputs_ActivateOutput(_dsmConfig.actuator[actuatorId], activate);
    Outputs_FireOutputLogicChanged(_dsmConfig.actuator[actuatorId], activate);
    
    return R_Ok;
}

/**
 * Activate / deactivate requested special output
 *
 * @param outputId - id of the output to activate
 * @param activate - 1 = activate, 0 = deactivate
 * @return result of the operation 
 */
Result_e DSM_ActivateSpecialOutput(DSMSpecialDOType_e outputId, uint32 activate)
{
    if (_dsmConfig.specialOutput[outputId] == UNCONFIGURED_IO)
        return R_OutputNotAssigned;
    
    if (outputId == SpecialDOAjar)
    {
        if (!_dsmConfig.doorAjarAlarmEnabled ||
            activate == _dsmMachine.doorAjarAlarmOutActivated)           
            return R_Ok;
    }
    
    if (outputId == SpecialDOIntrusion)
    {
        if (!_dsmConfig.intrusionAlarmEnabled ||
            activate == _dsmMachine.intrusionAlarmOutActivated)
            return R_Ok;
    }
    
    if (outputId == SpecialDOSabotage)
    {
        if (!_dsmConfig.sabotageAlarmEnabled ||
            activate == _dsmMachine.sabotageAlarmOutActivated)
            return R_Ok;
    }
    
    if (outputId == SpecialDOAjar)
        _dsmMachine.doorAjarAlarmOutActivated = activate;
    else if (outputId == SpecialDOIntrusion)
        _dsmMachine.intrusionAlarmOutActivated = activate;
    else if (outputId == SpecialDOSabotage)
        _dsmMachine.sabotageAlarmOutActivated = activate;
    
    Outputs_ActivateOutput(_dsmConfig.specialOutput[outputId], activate);
    Outputs_FireOutputLogicChanged(_dsmConfig.specialOutput[outputId], activate);
    
    return R_Ok;
}

/**
 * Checks the status of specified sensor
 * @param sensor - reading will be done for this sensor
 * @return SensorActivated (Alarm), SensorNotActivated (Normal), SensorNotChanged (there 
 * was no change since last reading), SensorSabotaged (Short or Break) or error message
 */
Result_e DSM_CheckSensor(DSMSensors_e sensor) 
{
    if (_dsmConfig.sensor[sensor] == UNCONFIGURED_IO)
        return R_OutputNotAssigned;
    
    uint32 inputId = _dsmConfig.sensor[sensor];
    
    /* if input with requested id has changed its status */
    if (_inputChanged & (1 << inputId))
    {
        /* Clear the input change signal */
        _inputChanged &= ~(1 << inputId);
        
        if (_inputStates[inputId] == Normal)
            return R_SensorNotActivated;
        else if (_inputStates[inputId] == Alarm)
            return R_SensorActivated;
        else if (_inputStates[inputId] == Break || _inputStates[inputId] == Short)
            return R_SensorSabotage;
    }
    
    return R_SensorNotChanged;
}

/**
 * Sends access granted signal to the DSM 
 */
Result_e DSM_SignalAccessGranted(DSMAGSourceCard_e cardType)
{
    bool suppresAG = false;
    
    if (_dsmConfig.cardReaders[0].address != 0 && _dsmConfig.cardReaders[1].address != 0)
    {
        // check that on one CR is locked  and on the other is unlocked code
        if (((_dsmConfig.cardReaders[0].flags & DSM_CR_LOCKED) &&
             (_dsmConfig.cardReaders[1].implicitCode == CRMC_DOOR_UNLOCKED)) ||

             ((_dsmConfig.cardReaders[1].flags & DSM_CR_LOCKED) &&
             (_dsmConfig.cardReaders[0].implicitCode == CRMC_DOOR_UNLOCKED)))
            suppresAG = true;
        
        // both are locked
        if (_dsmConfig.cardReaders[0].flags & DSM_CR_LOCKED && 
            _dsmConfig.cardReaders[1].flags & DSM_CR_LOCKED)
            suppresAG = true;
    }
    
    // check that one is assigned and locked and the other is not assigned
    if (_dsmConfig.cardReaders[0].address != 0 && _dsmConfig.cardReaders[1].address == 0 && 
        (_dsmConfig.cardReaders[0].flags & DSM_CR_LOCKED))
        suppresAG = true;
    
    if (_dsmConfig.cardReaders[1].address != 0 && _dsmConfig.cardReaders[0].address == 0 && 
        (_dsmConfig.cardReaders[1].flags & DSM_CR_LOCKED))
        suppresAG = true;
    
    if (suppresAG && cardType != DSM_EmergencyCard)
        return R_Ok;
        
    _dsmMachine.signalAccessGranted = 1;
    _dsmMachine.agSource = AGS_CardReader;
    
    return R_Ok;
}

void DSM_ReportActivatorStatuses()
{

    uint32 i;
    for (i = 0; i < DSM_ACTUATOR_COUNT; i++)
    {
        if (_dsmConfig.actuator[i] != UNCONFIGURED_IO)
            Outputs_FireOutputLogicChanged(_dsmConfig.actuator[i], Outputs_IsActivated(_dsmConfig.actuator[i]));
    }
    
    for (i = 0; i < DSM_SPECIAL_OUTPUT_COUNT; i++)
    {
        if (_dsmConfig.specialOutput[i] != UNCONFIGURED_IO)
            Outputs_FireOutputLogicChanged(_dsmConfig.specialOutput[i], Outputs_IsActivated(_dsmConfig.specialOutput[i]));
    }

}

uint32 _tamperReturnAssigned = false;
void DSM_OnCRTamperReturn(CardReader_t* cr)
{
    if (_dsmIsRunning)
    {
        DSM_InformCR(cr->address, _lastSignalToCR);
    }
}

void DSM_OnCRCountryCodeConfirmed(CardReader_t* cr)
{
    Result_e lr = DSM_LooseCR(cr->address);
    
    if (lr == R_CardReaderNotAssigned)
        CR_AccessCommand(cr,CRMC_OUT_OF_ORDER,-1);   
}

uint32 _crOnlineAssigned = false;
void DSM_OnCROnline(CardReader_t* cr, bool online)
{
    if (_dsmIsRunning && online)
    {
        // this does the same DSM_InformCR(cr->address, _lastSignalToCR); inside
        // plus it flushes the supression flag if present
        DSM_LooseCR(cr->address);
        
    }
}

void DSM_OnSpecialOutputAssigned(DSMSpecialDOType_e outputType)
{
    if (!_dsmIsRunning)
        return;
    
    switch (outputType)
    {
        case SpecialDOAjar:
            if (_dsmMachine.ajarActive)
                DSM_ActivateSpecialOutput(outputType, 1);
            break;
        case SpecialDOIntrusion:
            if (_dsmMachine.state == StateIntrusion)
                DSM_ActivateSpecialOutput(outputType, 1);
            break;
        case SpecialDOSabotage:
            if (_dsmMachine.sabotaged)
                DSM_ActivateSpecialOutput(outputType, 1);
            break;
    }
}

void DSM_OnSpecialOutputDeassigned(DSMSpecialDOType_e outputType)
{
    if (!_dsmIsRunning)
        return;
    
    switch (outputType)
    {
        case SpecialDOAjar:
            if (_dsmMachine.ajarActive)
                DSM_ActivateSpecialOutput(outputType, 0);
            break;
        case SpecialDOIntrusion:
            if (_dsmMachine.state == StateIntrusion)
                DSM_ActivateSpecialOutput(outputType, 0);
            break;
        case SpecialDOSabotage:
            if (_dsmMachine.sabotaged)
                DSM_ActivateSpecialOutput(outputType, 0);
            break;
    }
}

void DSM_OnEnabledAlarmsChanged(uint32 doorAjarAlarm, uint32 intrusionAlarm, uint32 sabotageAlarm)
{
    if (!_dsmIsRunning)
        return;
    
    // if was enabled and is not now, check if the output is on, switch it off if it is 
    if (_dsmConfig.doorAjarAlarmEnabled && !doorAjarAlarm && _dsmMachine.ajarActive)
        DSM_ActivateSpecialOutput(SpecialDOAjar, 0);
    
    if (_dsmConfig.intrusionAlarmEnabled && !intrusionAlarm && _dsmMachine.state == StateIntrusion)
        DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);
    
    if (_dsmConfig.sabotageAlarmEnabled && !sabotageAlarm && _dsmMachine.sabotaged)
        DSM_ActivateSpecialOutput(SpecialDOSabotage, 0);
}

void DSM_DoBinding()
{
    DSM_BindOnSpecialOutputAdded(DSM_OnSpecialOutputAssigned);
    DSM_BindOnSpecialOutputRemoved(DSM_OnSpecialOutputDeassigned);
    DSM_BindOnEnabledAlarmChanged(DSM_OnEnabledAlarmsChanged);
    CrCommunicator_BindCountryCodeConfirmed(DSM_OnCRCountryCodeConfirmed);
}

/**
 * Start & init the DSM
 * @return Ok
 */
Result_e DSM_Start(DoorEnviromentType_e enviromentType)
{
    if (_dsmIsRunning)
        return R_Ok;
    
    if (!_tamperReturnAssigned)
    {
        CrCommunicator_BindTamperReturn(DSM_OnCRTamperReturn);
        _tamperReturnAssigned = true;
    }
    
    if (!_crOnlineAssigned)
    {
        CrCommunicator_BindOnlineStateChanged(DSM_OnCROnline);
        _crOnlineAssigned = true;
    }
    
    if (enviromentType > 5)
        return R_InvalidParameter;
    
    /* Mark down old values that might be of use */
    uint32 markForceUnlocked = _dsmMachine.forceUnlockedByStart;
    uint32 stoppedTime = _dsmMachine.stopTime;
    uint32 markDoorOpenedAtStop = _dsmMachine.doorOpenedAtStop;
    DSMAccessGrantedSource_e lastAGSource = _dsmMachine.agSource;
    
    // some initialization
    memset((void*)&_dsmMachine, 0, sizeof(_dsmMachine));
    
    _dsmMachine.enviromentType = enviromentType;
    
    switch (_dsmMachine.enviromentType)
    {
        /* electric strike, door opened */
        case DEStandard:
            if (_dsmConfig.sensor[DoorOpened] == UNCONFIGURED_IO)
                return R_OpenDoorNotConfigured;
            if (_dsmConfig.actuator[ElectricStrike] == UNCONFIGURED_IO)
                return R_ElectricStrikeNotConfigured;
            
            if (_dsmConfig.sensor[DoorLocked] != UNCONFIGURED_IO)
                return R_DoorLockedNotSupported;
            if (_dsmConfig.sensor[DoorFullyOpened] != UNCONFIGURED_IO)
                return R_DoorFullyOpenedNotSupported;
            break;
            
        /* description needed */
        case DERotating:
            break;
            
        /* same as standard + door locked sensor */
        case DEStandardWithLocking:
            if (_dsmConfig.sensor[DoorOpened] == UNCONFIGURED_IO)
                return R_OpenDoorNotConfigured;
            if (_dsmConfig.sensor[DoorLocked] == UNCONFIGURED_IO)
                return R_DoorLockedNotConfigured;
            if (_dsmConfig.actuator[ElectricStrike] == UNCONFIGURED_IO)
                return R_ElectricStrikeNotConfigured;
            
            if (_dsmConfig.sensor[DoorFullyOpened] != UNCONFIGURED_IO)
                return R_DoorFullyOpenedNotSupported;
            break;
            
        /* description needed */
        case DEStandardWithMaxOpened:
            break;
            
        /* Only single electric strike required */
        case DEMinimal:
            if (_dsmConfig.actuator[ElectricStrike] == UNCONFIGURED_IO)
                return R_ElectricStrikeNotConfigured;
            
            if (_dsmConfig.sensor[DoorOpened] != UNCONFIGURED_IO)
                return R_DoorOpenedNotSupported;
            if (_dsmConfig.sensor[DoorLocked] != UNCONFIGURED_IO)
                return R_DoorLockedNotSupported;
            if (_dsmConfig.sensor[DoorFullyOpened] != UNCONFIGURED_IO)
                return R_DoorFullyOpenedNotSupported;
            break;
    }
    
    /* Check that assigned card readers have implicit codes defined */
    // this checking has no meaning anymore, as there are other implicit commands allowed
    /*for (int i = 0; i < 2; i++)
    {
        if (_dsmConfig.cardReaders[i].address != 0 &&
            (_dsmConfig.cardReaders[i].implicitCode < 0x40 || _dsmConfig.cardReaders[i].implicitCode > 0x5f))
        {
            return R_ImplicitCodeNotDefined;
        }
    }*/
    
    
    _dsmIsRunning = 1;
    _dsmConfig.locked = 1;
    _dsmMachine.intrusionStarted = 0;
    _dsmMachine.intrusionSignalStarted = 0;
    _dsmMachine.doorOpened = 0;
    _dsmMachine.locksOpen = 0;
    _dsmMachine.forceUnlocked = markForceUnlocked;
    _dsmMachine.forceUnlockedByStart = markForceUnlocked;
    _dsmMachine.agSource = AGS_None;
    
    if (markDoorOpenedAtStop)
    {
        uint32 elapsedSinceStop = SysTick_GetElapsedTime(stoppedTime);
        if (elapsedSinceStop < DSM_MAX_STOPPED_TIME)
        {
            DSM_ActivateActuator(AlarmBypass, 1);
            
            _dsmMachine.state = StateDoorOpened;
            _dsmMachine.timer = SysTick_GetTickCount();
            DSM_InformCRs(SignalDoorOpened);
            
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalDoorOpened, InfoNormalAccess, AGS_None);
        }
    }    
    /* Signal the default startup state */
    else if (!_dsmMachine.forceUnlockedByStart)
        DSM_InformCRs(SignalLocked);
    
    Tasks_Register(DSM_Task, 10);
    
    DSM_ReportActivatorStatuses();
    
    return R_Ok;
}

void DSM_DeactivateSpecialOutputs()
{
    if (_dsmMachine.ajarActive &&
        _dsmConfig.doorAjarAlarmEnabled && 
        _dsmConfig.specialOutput[SpecialDOAjar] != UNCONFIGURED_IO)
    {
        DSM_ActivateSpecialOutput(SpecialDOAjar, 0);
    }
    
    if (_dsmMachine.state == StateIntrusion &&
        _dsmConfig.intrusionAlarmEnabled &&
        _dsmConfig.specialOutput[SpecialDOIntrusion] != UNCONFIGURED_IO)
    {
        DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);
    }
    
    if (_dsmMachine.sabotaged &&
        _dsmConfig.sabotageAlarmEnabled &&
        _dsmConfig.specialOutput[SpecialDOSabotage] != UNCONFIGURED_IO)
    {
        DSM_ActivateSpecialOutput(SpecialDOSabotage, 0);
    }
}




/**
 * Stop the DSM
 * @return Ok
 */
Result_e DSM_Stop()
{
    if (_dsmIsRunning == 1)
    {
        _dsmMachine.doorOpenedAtStop = (_dsmMachine.state == StateDoorOpened) ? 1 : 0;
        
        DSM_SetLocks(0);
        DSM_ActivateActuator(AlarmBypass, 0);
        DSM_ActivateSpecialOutput(SpecialDOSabotage, 0);
        DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);
        DSM_ActivateSpecialOutput(SpecialDOAjar, 0);
        
        _dsmIsRunning = 0;
        _dsmConfig.locked = 0;
        
        Tasks_Unregister(DSM_Task);
        
        DSM_DeactivateSpecialOutputs();
        DSM_UnsetAll();
        
        DSM_Init();
        
        _dsmMachine.stopTime = SysTick_GetTickCount();
    }
    
    return R_Ok;
}

/** 
 * Check if access granted signal was sent by upper layer (CR, CCU or pushbutton)
 * @return 1 if access granted, 0 if not
 */
bool DSM_IsAccessGranted()
{
    if (_dsmMachine.signalAccessGranted)
    {
        _dsmMachine.signalAccessGranted = 0;
        return true;
    }
    else
        return false;
}

/**
 * Check status of the pushbuttons (if any configured) and signal access granted
 * if pushbutton active or signal sabotage if required
 */
void DSM_CheckPushButton()
{
    Result_e sensorStatus;
    
    uint32 isSabotaged = 0;
    
    uint32 i;
    for (i = 0; i < DSM_AG_SRC_COUNT; i++)
    {
        /* check the access granted if configured as dsm button input */
        if (_dsmConfig.agSources[i] == UNCONFIGURED_IO)
            continue;
        
        sensorStatus = R_SensorNotChanged;
        
        uint32 inputId = _dsmConfig.agSources[i];
        if (_inputChanged & (1 << inputId))
        {
            _inputChanged &= ~(1 << inputId);
            
            if (_inputStates[inputId] == Normal)
                sensorStatus = R_SensorNotActivated;
            else if (_inputStates[inputId] == Alarm)
                sensorStatus = R_SensorActivated;
            else if (_inputStates[inputId] == Break || _inputStates[inputId] == Short)
                sensorStatus = R_SensorSabotage;
        }
        
        if (sensorStatus == R_SensorActivated)
        {
            // TODO add check
            bool isCrLocked = false;
            for (int i = 0; i < 2; i++)
            {
                 if (_dsmConfig.cardReaders[i].address != 0 &&
                    DSM_ShouldBeLockedByCrSettings(
                        _dsmConfig.cardReaders[i].implicitCode,
                        (_dsmConfig.cardReaders[i].optionalParameter >= 0),
                        (_dsmConfig.cardReaders[i].optionalParameter < 0 ? 0 : _dsmConfig.cardReaders[i].optionalParameter))
                        )
                    isCrLocked = true;
            }
            // no AG if either cr is in locked state or area is set
            if (isCrLocked)
                break;
          
            _dsmMachine.signalAccessGranted = 1;
            _dsmMachine.agSource = (DSMAccessGrantedSource_e)i;
            break;
        }       
        
        if (sensorStatus == R_SensorSabotage)
            isSabotaged = 1;
        
        if (!isSabotaged)
        {
            if (sensorStatus != R_SensorNotChanged && sensorStatus != R_SensorSabotage)
                _dsmMachine.pushButtonSabotaged = 0;        
        }
    }
    
    if (isSabotaged)
        _dsmMachine.pushButtonSabotaged = 1;
}

/**
 * Activate / deactivate all configured electric strikes (locks)
 *
 * @param open - 1 = locks are open, 0 = locks are locked
 */
void DSM_SetLocks(uint32 open)
{
    if (open !=  _dsmMachine.locksOpen)
    {
        DSM_ActivateActuator(ElectricStrike, open);
        DSM_ActivateActuator(ExtraElectricStrike, open);
        
        DSM_ActivateActuator(ElectricStrikeOpposite, open);
        DSM_ActivateActuator(ExtraElectricStrikeOpposite, open);
        
        _dsmMachine.locksOpen = open;        
    }
}
        
DSMSignals_e DSM_ForceUnlocked(uint32 isUnlocked,bool refreshCRs)
{
    if (_dsmIsRunning)
    {
        /* Switching off */
        if (_dsmMachine.forceUnlocked == 1 && isUnlocked == 0)
        {
            _dsmMachine.forceUnlocked = 0;
            
            /* Lock again */
            DSM_SetLocks(0);
            
            if (_dsmMachine.state == StateDoorOpened)
            {
                DSM_SignalAccessGranted(DSM_NormalCard);
            }
            else if (_dsmMachine.state == StateUnlocked)
            {
                /* if Door Locked sensor configured go to locking state */
                if (_dsmConfig.sensor[DoorLocked] != UNCONFIGURED_IO)
                {
                    _dsmMachine.state = StateLocking;
                    if (!_dsmMachine.sabotaged)
                        DSM_Callback(SignalLocking, InfoNone, AGS_None);
                    
                    if (refreshCRs)
                        DSM_InformCRs(SignalLocking);
                    
                    return SignalLocking;
                }
                else
                {
                    _dsmMachine.state = StateLocked;
                    /* Disable alarm bypass */
                    DSM_ActivateActuator(AlarmBypass, 0);
                    if (!_dsmMachine.sabotaged)
                        DSM_Callback(SignalLocked, InfoApasRestored, AGS_None);
                    
                    if (refreshCRs)
                        DSM_InformCRs(SignalLocked);
                    
                    return SignalLocked;
                }
            }
            
            return SignalNone;
        }
        
        /* Switching on */
        if (_dsmMachine.forceUnlocked == 0 && isUnlocked == 1)
        {
            _dsmMachine.forceUnlocked = 1;
             
            if (_dsmMachine.state == StateLocked)
            {
                DSM_SetLocks(1);
                DSM_ActivateActuator(AlarmBypass, 1);
            }
            
            if (_dsmMachine.state == StateLocked || _dsmMachine.state == StateUnlocking || _dsmMachine.state == StateLocking)
            {
                _dsmMachine.state = StateUnlocked;
                if (!_dsmMachine.sabotaged)
                    DSM_Callback(SignalUnlocked, InfoNone, AGS_None);
                
                if (refreshCRs)
                    DSM_InformCRs(SignalUnlocked);
            }
            
            if (_dsmMachine.state == StateDoorOpened)
            {
                DSM_SetLocks(1);
                DSM_SignalAccessGranted(DSM_NormalCard);
            }
            
            if (_dsmMachine.state == StateIntrusion)
            {
                DSM_SetLocks(1);
                DSM_SignalAccessGranted(DSM_NormalCard);
            }
            
            return SignalUnlocked;
        }
    }
    else
    {
        _dsmMachine.forceUnlockedByStart = (isUnlocked != 0) ? 1 : 0;
    }
    
    _dsmMachine.forceUnlocked = (isUnlocked != 0) ? 1 : 0;
    
    return SignalNone;
}

/**
 * Helper function. Returns DSM signal (for event reporting) for requested state
 * @param state - state for which signal should be returned
 * @return signal that reflect the coresponding state
 */
DSMSignals_e DSM_GetSignalForState(DSMMachineStates_e state)
{
    switch (state)
    {
        case StateLocked:
            return SignalLocked;
        case StateUnlocking:
            return SignalUnlocking;
        case StateUnlocked:
            return SignalUnlocked;
        case StateDoorOpened:
            return SignalDoorOpened;
        case StateLocking:
            return SignalLocking;
        case StateIntrusion:
            return SignalIntrusion;
    }
    
    return SignalLocked;
}

/**
 * Checks if there is sabotage on the configured door sensor
 *
 * @param sensor - enum of the sensor that should be checked 
 * @param result - result from the DSM_CheckSensor function for this sensor
 */
void DSM_CheckDoorSensorSabotage(DSMSensors_e sensor, Result_e result)
{
    if (_dsmConfig.sensor[sensor] == UNCONFIGURED_IO)
        return;
    
    if (!_dsmMachine.sabotagedSensor[sensor])
    {
        if (result == R_SensorSabotage)
            _dsmMachine.sabotagedSensor[sensor] = 1;
    }
    else
    {
        if (result != R_SensorNotChanged && result != R_SensorSabotage)
            _dsmMachine.sabotagedSensor[sensor] = 0;
    }
}

/**
 * Make reading from all configured sensors (done in every walk through the DSM). Checks 
 * for changes in sensor status (activated, deactivated, sabotaged) and signals sabotage
 * if there is any
 */
void DSM_CheckSensors()
{
    uint32 i;
    
    Result_e doorOpened = DSM_CheckSensor(DoorOpened);
    Result_e doorLocked = DSM_CheckSensor(DoorLocked);
    
    if (doorOpened == R_SensorActivated)
        _dsmMachine.doorOpened = 1;
    if (doorOpened == R_SensorNotActivated)
        _dsmMachine.doorOpened = 0;

    if (doorLocked == R_SensorActivated)
        _dsmMachine.doorLocked = 1;
    if (doorLocked == R_SensorNotActivated)
        _dsmMachine.doorLocked = 0;
    
    DSM_CheckPushButton();    
    DSM_CheckDoorSensorSabotage(DoorOpened, doorOpened);
    DSM_CheckDoorSensorSabotage(DoorLocked, doorLocked);
    
    uint32 isSabotaged = 0;
        
    for (i = 0; i < DSM_SENSOR_COUNT; i++)
    {
        if (_dsmMachine.sabotagedSensor[i])
        {
            isSabotaged = 1;
            break;
        }
    }
    
    if (!_dsmMachine.sabotaged)
    {
        if (isSabotaged == 1 || _dsmMachine.pushButtonSabotaged)
        {
            _dsmMachine.sabotaged = 1;
            DSM_Callback(SignalSabotage, InfoAccessViolated, AGS_None);
            DSM_ActivateSpecialOutput(SpecialDOSabotage, 1);
        }
    }
    else
    {
        if (isSabotaged != 1 && _dsmMachine.pushButtonSabotaged != 1)
        {
            _dsmMachine.sabotaged = 0;
            DSM_Callback(DSM_GetSignalForState(_dsmMachine.state), InfoViolationReturn, AGS_None);
            DSM_ActivateSpecialOutput(SpecialDOSabotage, 0);
        }
    }
}

/**
 * Function handles the intrusion checking and timming
 *
 * @return 1 if there is intrusion, 0 otherwise
 */
uint32 DSM_ProcessIntrusion()
{
    /* No intrusion checking if force unlocked is on */
    if (_dsmMachine.forceUnlocked)
        return 0;
        
    if (!_dsmMachine.intrusionStarted)
    {
        /* Check if door opened, if so start intrusion delay */
        if (_dsmMachine.doorOpened && 
            _dsmMachine.state != StateUnlocked && 
            _dsmMachine.state != StateDoorOpened && 
            _dsmMachine.state != StateIntrusion)
        {
            _dsmMachine.intrusionTimer = SysTick_GetTickCount();
            _dsmMachine.intrusionStarted = 1;
        }                 
    }
    
    uint32 elapsed;
    if (_dsmMachine.intrusionStarted)
    {
        elapsed = SysTick_GetElapsedTime(_dsmMachine.intrusionTimer);
        
        /* If intrusion delay run out -> signal with impulse or go to the intrusion state */
        if (elapsed >= _dsmConfig.delay[DelayBeforeIntrusion])
        {
            _dsmMachine.intrusionStarted = 0;
            
            if (_dsmMachine.doorOpened && _dsmMachine.state != StateUnlocked && _dsmMachine.state != StateDoorOpened)
            {
                DSM_Callback(SignalIntrusion, InfoAccessViolated, AGS_None);
                DSM_ActivateSpecialOutput(SpecialDOIntrusion, 1);
                DSM_InformCRs(SignalIntrusion);
                
                _dsmMachine.state = StateIntrusion;
            }
            else if (_dsmConfig.invokeIntrusionAlarm)
            {
                DSM_Callback(SignalIntrusion, InfoAccessViolated, AGS_None);
                DSM_ActivateSpecialOutput(SpecialDOIntrusion, 1);
                
                _dsmMachine.intrusionSignalStarted = 1;                
                _dsmMachine.intrusionSignalTimer = SysTick_GetTickCount();                
            }
            
            return 1;
        }
//        else
//        {
//            // check if door still opened
//            if (!_dsmMachine.doorOpened)
//                _dsmMachine.intrusionStarted = 0;
//        }
    }
    
    if (_dsmMachine.intrusionSignalStarted)
    {
        elapsed = SysTick_GetElapsedTime(_dsmMachine.intrusionSignalTimer);
        if (elapsed > DSM_INTRUSION_LENGTH)
        {
            DSM_Callback(DSM_GetSignalForState(_dsmMachine.state), InfoViolationReturn, AGS_None);
            DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);
            _dsmMachine.intrusionSignalStarted = 0;
        }
    }
    
    return 0;
}

/**
 * Process locked state. 
 * If access granted signal detected and DoorLocked sensor configured go to Unlocking state
 * If access granted signal detected and DoorLocked sensor is not configured go to Unlocked state
 */
void DSM_ProcessLockedState()
{
    /* If force unlocked before start */
    if (_dsmMachine.forceUnlocked && _dsmMachine.forceUnlockedByStart)
    {
        DSM_SetLocks(1);
        DSM_ActivateActuator(AlarmBypass, 1);
        if (_dsmMachine.doorOpened)
        {
            _dsmMachine.state = StateDoorOpened;
            _dsmMachine.timer = SysTick_GetTickCount();
            DSM_InformCRs(SignalDoorOpened);
            
            /* Signal normal access to the upper layer */
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalDoorOpened, InfoNormalAccess, AGS_None);
        }
        else
        {
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalUnlocked, InfoNone, AGS_None);
            _dsmMachine.state = StateUnlocked;
            DSM_InformCRs(SignalUnlocked);
        }
        
        _dsmMachine.forceUnlockedByStart = 0;
    }
    
    /* If access granted signal present -> open the lock(s) */
    if (DSM_IsAccessGranted())
    {
        /* Open the lock */
        DSM_SetLocks(1);
        /* Raise alarm bypass */
        DSM_ActivateActuator(AlarmBypass, 1);
        
        /* if doorlocked sensor is used -> unlocking */
        if (_dsmConfig.sensor[DoorLocked] != UNCONFIGURED_IO)
        {
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalUnlocking, InfoNone, _dsmMachine.agSource);

            _dsmMachine.state = StateUnlocking;
            DSM_InformCRs(SignalUnlocking);
        }    
        else
        {
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalUnlocked, InfoNone, _dsmMachine.agSource);

            _dsmMachine.state = StateUnlocked;
            DSM_InformCRs(SignalUnlocked);
        }
        /* Start counting time for max unlock time */
        _dsmMachine.timer = SysTick_GetTickCount(); 
        
        return;
    }
}

/**
 * Process unlocking state. 
 * If access granted signal detected refresh the time for the lock to be open
 * If MaxUnlockTime reached lock the door and go to Locked state
 * If DoorLocked sensor signal unlocked state go to Unlocked state
 */
void DSM_ProcessUnlockingState()
{
    uint32 elapsedTime;
    
    /* Access granted signal... refresh timer */
    if (DSM_IsAccessGranted())
    {
        _dsmMachine.timer = SysTick_GetTickCount();
    }
    
    /* Check if Max. Unlock timeout occured */
    elapsedTime = SysTick_GetElapsedTime(_dsmMachine.timer);
    if (elapsedTime >= /*_dsmConfig.delay[DelayBeforeUnlock] +*/ _dsmConfig.timming[MaxUnlockTime])
    {
        /* Lock again */
        DSM_SetLocks(0);
        /* Disable alarm bypass */
        DSM_ActivateActuator(AlarmBypass, 0);
        
        _dsmMachine.state = StateLocked;
        DSM_InformCRs(SignalLocked);
        /* Signal interrupted access */
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalLocked, InfoInterruptedAccess, AGS_None);
        
        return;
    }
    
    if (!_dsmMachine.doorLocked)
    {
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalUnlocked, InfoNone, AGS_None);
        _dsmMachine.state = StateUnlocked;
        DSM_InformCRs(SignalUnlocked);
    }
}

/** 
 * Process Unlocked state
 * If access granted signal detected refresh the time for the lock to be open
 * If MaxUnlockTime reached lock the door and go to Locked state
 * If DoorOpened sensor activated go the DoorOpened state
 */
#pragma diag_suppress=Pa082
void DSM_ProcessUnlockedState()
{
    uint32 elapsedTime;
    
    /* Check for timeouts only if force unlocked is off */
    if (_dsmMachine.forceUnlocked == 0)
    {
        /* Access granted signal... refresh timer */
        if (DSM_IsAccessGranted())
        {
            _dsmMachine.timer = SysTick_GetTickCount();
        }
    
        /* Check if Max. Unlock timeout occured */
        elapsedTime = SysTick_GetElapsedTime(_dsmMachine.timer);
        if (elapsedTime >= _dsmConfig.delay[DelayBeforeUnlock] + _dsmConfig.timming[MaxUnlockTime])
        {
            /* Lock again */
            DSM_SetLocks(0);
            /* Disable alarm bypass */
            DSM_ActivateActuator(AlarmBypass, 0);
            
            _dsmMachine.state = StateLocked;
            DSM_InformCRs(SignalLocked);
            /* Signal interrupted access */
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalLocked, InfoInterruptedAccess, AGS_None);
            
            return;
        }
    }
    
    /* Check if doors are open -> close the lock */
    if (_dsmMachine.doorOpened)
    {
        if (!_dsmMachine.forceUnlocked)
            DSM_SetLocks(0);
        
        _dsmMachine.state = StateDoorOpened;
        _dsmMachine.timer = SysTick_GetTickCount();
        DSM_InformCRs(SignalDoorOpened);
        
        /* Signal normal access to the upper layer */
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalDoorOpened, InfoNormalAccess, AGS_None);
        
        return;
    }
}


/**
 * Process DoorOpened state
 * If access granted signal detected refresh the MaxOpenTime timer and disable preajar or ajar if active
 * If Ajar timeout reached start the ajar
 * If pre AJAR configured and timeout reached start the pre AJAR
 * If the DoorOpened is deactivated and DoorLocked sensor is configured go to the Locking state
 * If the DoorOpened is deactivated and DoorLocked sensor is not configured go to the Locked state
 */
void DSM_ProcessDoorOpenedState()
{
    uint32 elapsedTime;
    
    /* Access granted signal received, so the door can be opened a little longer */
    if (DSM_IsAccessGranted())
    {
        /* signaled while door opened -> refresh max held open interval + stop ajar if there is any */
        if (_dsmMachine.ajarActive) 
        {
            _dsmMachine.ajarActive = 0;
            DSM_ActivateSpecialOutput(SpecialDOAjar, 0);
            DSM_Callback(SignalDoorOpened, InfoNone, _dsmMachine.agSource);
            DSM_InformCRs(SignalDoorOpened);
            
            /* Was in ajar before, and AG received -> should also raise bypass? */
            DSM_ActivateActuator(AlarmBypass, 1);
        }
        if (_dsmMachine.preAjarActive) 
        {
            _dsmMachine.preAjarActive = 0;
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalDoorOpened, InfoNone, _dsmMachine.agSource);
            DSM_InformCRs(SignalDoorOpened);
        }
        
        _dsmMachine.timer = SysTick_GetTickCount();
    }
    
    elapsedTime = SysTick_GetElapsedTime(_dsmMachine.timer);
    
    /* Check for pre-Ajar and Ajar timeouts only if force unlocked is off */
    if (_dsmMachine.forceUnlocked == 0)
    {
        /* Check the max held open time -> start AJAR */
        if (!_dsmMachine.ajarActive && 
            elapsedTime >= _dsmConfig.timming[MaxOpenTime])
        {
            /* start AJAR */
            _dsmMachine.ajarActive = 1;
            _dsmMachine.preAjarActive = 0;
            DSM_InformCRs(SignalAjar);
            
            /* Signal door ajar to the upper layer */
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalAjar, InfoNone, AGS_None);
            
            DSM_ActivateSpecialOutput(SpecialDOAjar, 1);
            
            /* Disable bypass when going into AJAR */
            DSM_ActivateActuator(AlarmBypass, 0);
        }
        /* check for pre ajar if configured (it is configured if pre ajar is greater than 0) */
        else if (!_dsmMachine.ajarActive && !_dsmMachine.preAjarActive && 
                 elapsedTime >= (_dsmConfig.timming[MaxOpenTime] - _dsmConfig.timming[DoorPreAjarTime]))
        {
            /* start pre ajar */
            _dsmMachine.preAjarActive = 1;
            DSM_InformCRs(SignalPreAjar);
            
            /* Signal pre ajar warning to the upper layer */
            if (!_dsmMachine.sabotaged)
                DSM_Callback(SignalPreAjar, InfoNone, AGS_None);
        }
    }
    
    /* Check if door are closed, if so start closing sequence */
    if (!_dsmMachine.doorOpened)
    {
        /* Disable Ajar if active */
        if (_dsmMachine.ajarActive)
        {
            _dsmMachine.ajarActive = 0;
            DSM_ActivateSpecialOutput(SpecialDOAjar, 0);
        }
        /* Disable pre-Ajar if active */
        if (_dsmMachine.preAjarActive)
            _dsmMachine.preAjarActive = 0;
        
        if (_dsmMachine.forceUnlocked == 0)
        {
            /* if Door Locked sensor configured go to locking state */
            if (_dsmConfig.sensor[DoorLocked] != UNCONFIGURED_IO)
            {
                _dsmMachine.state = StateLocking;
                DSM_InformCRs(SignalLocking);
                if (!_dsmMachine.sabotaged)
                    DSM_Callback(SignalLocking, InfoNone, AGS_None);
            }
            else
            {
                _dsmMachine.state = StateLocked;
                DSM_InformCRs(SignalLocked);
                if (!_dsmMachine.sabotaged)
                    DSM_Callback(SignalLocked, InfoApasRestored, AGS_None);
                DSM_ActivateActuator(AlarmBypass, 0);
            }
        }
        /* If force unlocked go to the unlocked state instead of locked(locking) */
        else
        {
            _dsmMachine.state = StateUnlocked;
            DSM_InformCRs(SignalUnlocked);
            if (!_dsmMachine.sabotaged)
                    DSM_Callback(SignalUnlocked, InfoNone, AGS_None);
        }
    }
}

/**
 * Process Locking state
 * If DoorLocked sensor activated go to the Locked state 
 */
void DSM_ProcessLockingState()
{
    if (_dsmMachine.doorLocked)
    {
        /* Restore APAS */
        _dsmMachine.state = StateLocked;
        DSM_InformCRs(SignalLocked);
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalLocked, InfoApasRestored, AGS_None);
        /* Disable alarm bypass */
        DSM_ActivateActuator(AlarmBypass, 0);
    }
}

/** 
 * Process intrusion state
 * In case intrusion lasted longer then before intrusion delay DSM goes here
 * If access granted signal detected stop the intrusion and go to the DoorOpened state
 * If door closed stop the intrusion and go to the Locked state
 */
void DSM_ProcessIntrusionState()
{
    if (DSM_IsAccessGranted())
    {
        /* signaled while door opened -> refresh max held open interval */
        _dsmMachine.preAjarActive = 0;
        _dsmMachine.timer = SysTick_GetTickCount();
        _dsmMachine.state = StateDoorOpened;
        DSM_InformCRs(SignalDoorOpened);
        
        /* Signal normal access to the upper layer */
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalDoorOpened, InfoNormalAccess, _dsmMachine.agSource);
        
        DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);        
        DSM_ActivateActuator(AlarmBypass, 1);   
    }
    
    /* Check if door are closed, if so cancel intrusion */
    if (!_dsmMachine.doorOpened)
    {
        _dsmMachine.state = StateLocked;
        DSM_InformCRs(SignalLocked);
        
        if (!_dsmMachine.sabotaged)
            DSM_Callback(SignalLocked, InfoViolationReturn, AGS_None);
        DSM_ActivateSpecialOutput(SpecialDOIntrusion, 0);
    
        DSM_ActivateActuator(AlarmBypass, 0);   
    }
}

/**
 * Main DSM task
 */
void DSM_Task()
{
    if (_dsmIsRunning)
    {
        DSM_CheckSensors();
        
        DSM_ProcessIntrusion();
        
        /* Process the DSM state */
        switch (_dsmMachine.state)
        {
            /* Idle state, lock is closed, door is closed */
            case StateLocked:
                DSM_ProcessLockedState();
                break;
            
            /* Wainting for the door to be unlocked */
            case StateUnlocking:
                DSM_ProcessUnlockingState();
                break;
                
            /* Access Granted received -> lock is opened, waiting for the opened door */
            case StateUnlocked:
                DSM_ProcessUnlockedState();
                break;
                
            /* Door has been opened -> close the lock */
            case StateDoorOpened:
                DSM_ProcessDoorOpenedState();
                break;
                
            /* State closing - important in case APAS Unlocked signal used */
            case StateLocking:
                DSM_ProcessLockingState();
                break;
                
            /* Intrusion detected */
            case StateIntrusion:
                DSM_ProcessIntrusionState();
                break;
        }    
    }
}

#endif // BOOTLOADER