#ifndef BOOTLOADER

#include "crNotify.h"
#include "CardReaders\CrCommunicator.h"
//#include "Communication\Clsp485\clsp485.h"
#include "Communication\clspGeneric.h"
#include "System\systemInfo.h"
#include "CardReaders\CrQueue.h"
#include "app_version.h"

#define ZERO_CHAR     0x30

const uint32 _crXtextPlacement = 1;

/********************************************
 *
 ********************************************/
void CrNotify_ShowLinkText(CardReader_t* cr,bool isUp) {
    if (null == cr || !cr->isOnline)
        return;
    
    char text[] = "LINK          "; // with reserve to UP, DOWN and number
    if (isUp) 
    {
        text[5] = 'U';      
        text[6] = 'P';
    }    
    else
    {
        text[5] = 'D';      
        text[6] = 'O';
        text[7] = 'W';      
        text[8] = 'N';
    }
    uint32 tl;
    
    uint32 saddr = Clsp_GetSuggestedAddress();
    if (saddr > 0) 
    {
        text[10] = '[';
        if (saddr / 10 > 0)
            text[11] = (saddr / 10) + ZERO_CHAR;
        text[12] = (saddr % 10) + ZERO_CHAR;
        text[13] = ']';
        tl = 14;
    }
    else
        tl = 9;

    CR_DisplayText(cr,_crXtextPlacement,0,text,tl);
    
}

/********************************************
 *
 ********************************************/
/*void CrNotify_ShowNoDipAddress(CardReader_t* cr) 
{
    
}*/

/********************************************
 *
 ********************************************/
void CrNotify_ShowCrInfo(CardReader_t* cr) {
    if (cr->address > CR_MAX_COUNT)
        return;

    uint32 maxTl = 21;
    char text[] = "CR                   "; // should be same length as maxTl
    uint32 tlPre = 5;
    uint32 dotChar = '.';
    
    uint32 tl = maxTl;
    
    text[2] = cr->address + ZERO_CHAR; // 0x30 == '0'
    
#ifdef DEBUG
           
    //itoa(cr->currentBaudRate,text+4, 14, false);
    
    //tl = 10;
    
#endif
    
    bool has45thLine = false;
           
    switch(cr->hardwareVersion) {
        case CRHW_Proximity8LineLcd:
        case CRHW_ProximityPremiumCCR:
            has45thLine = true;
        case CRHW_ProximityFull:
        case CRHW_ProximityLiteCCR:
        case CRHW_ASPMotorolaProximityFull:
            CopySafe("Proximity",9,text+tlPre,tl-tlPre,9);
            tl = 14;
            break;
        case CRHW_SmartPremiumCCR:
        case CRHW_Smart8LineLCD:
            has45thLine = true;
        case CRHW_SmartFull:
        case CRHW_SmartQTouchKeyboard:
        case CRHW_SmartLiteCCR:
        case CRHW_SmartQTouchCCR:
            CopySafe("Mifare",6,text+tlPre,tl-tlPre,6);
            tl = 11;
            break;
        case CRHW_HIDProximity8LineLCDFull:
            has45thLine = true;
        case CRHW_HIDProximityFull:
            CopySafe("HID",3,text+tlPre,tl-tlPre,3);
            tl = 8;
            break;
        //case CRHW_Unknown:
        default:
            return;
    }
    
    CR_DisplayText(cr,_crXtextPlacement,2,text,tl);  
    
    if (has45thLine) 
    {
        // this branch indicates, that CR is capable of showing 4-5th line
        
        memset(text,' ',maxTl);
        CopySafe("CR FW:",6,text,maxTl,6);
        int len = itoaCustom(
                       //1,
             cr->firmwareVersionHigh,
             text+7,11,false);
        
        text[7+len]=dotChar;
        itoaCustom(//0,
             cr->firmwareVersionLow,
             text+7+1+len,4, false);
     
        CR_DisplayText(cr,_crXtextPlacement,4,text,12);
        
        memset(text,' ',maxTl);
        CopySafe("DCU LON FW:",11,text,maxTl,11);
        uint32 shift = itoaCustom(MAJOR_VERSION,text+12,2,false);
        
        text[12+shift] = dotChar;
        shift++;
        shift += itoaCustom(MINOR_VERSION,text+12+shift,2,false);
        
        text[12+shift] = dotChar;
        shift++;
        itoaCustom(BUILD_NUMBER,text+12+shift,4,false);
     
        CR_DisplayText(cr,_crXtextPlacement,6,text,maxTl);
        
        uint32 bV,bR;
        SysInfo_GetBootloaderVersion(&bV,&bR);
        
        /*if (bV == 0 || bR == 0)
        {
            memset(text,' ',maxTl);
            CopySafe("DCU BootFW: none",16,text,maxTl,16);
            
        }
        else
        {*/
            memset(text,' ',maxTl);
            CopySafe("DCU BootFW:",11,text,maxTl,11);
            uint32 startFrom = 12;            
            shift = itoaCustom(bV,text+startFrom,2,false);
        
            text[startFrom+shift] = dotChar;
            shift++;
            itoaCustom(bR,text+startFrom+shift,2,false);
                
        //}
        
        CR_DisplayText(cr,_crXtextPlacement,8,text,16);
    }
    

}

/********************************************
 *
 ********************************************/
void CrNotify_ShowLink(CardReader_t* cr,bool clearDisplay,int mode) {
    if (null == cr || !cr->isOnline)
        return;
    
    CrQueue_Clear(cr->address);
    
    bool hasDisplay = CR_HasDisplay(cr);
    
    if (hasDisplay && clearDisplay)
        CR_ClearDisplay(cr);
    
    if (mode == 0)  // down
    {
        uint32 saddr = Clsp_GetSuggestedAddress();
        if (saddr == 0) {
            if (hasDisplay) {
                // too simple to make it separate function
                //CrNotify_ShowNoDipAddress(cr);
                char text[] = "NO DIP ADDRESS !";
    
                CR_DisplayText(cr,_crXtextPlacement,1,text,16);
            }
    
            CR_RawPeripheralMode(cr->address,CRIM_LOW_FREQUENCY,CRIM_ON,CRIM_ULTRA_LOW_FREQUENCY,CRIM_OFF);
        }
        else
            CR_RawPeripheralMode(cr->address,CRIM_ULTRA_LOW_FREQUENCY,CRIM_ON,CRIM_LONG_PULSE,CRIM_OFF);
    }
    else
        if (mode == 1) // up
            CR_RawPeripheralMode(cr->address,CRIM_OFF,CRIM_HIGH_FREQUENCY,CRIM_OFF,CRIM_OFF);
        else
            // adress conflict
            CR_RawPeripheralMode(cr->address,CRIM_HIGH_FREQUENCY,CRIM_ON,CRIM_HIGH_FREQUENCY,CRIM_OFF);

#ifdef TEST_MODE_VIA_CR
    CR_SingleKeyMode(cr,mode == 0);    
#endif
    
    if (hasDisplay) 
    {
        if (mode < 0)
            CR_DisplayText(cr,_crXtextPlacement,1,"ADDRESS CONFLICT !",18);
        
        CrNotify_ShowLinkText(cr,mode == 1);
    
        CrNotify_ShowCrInfo(cr);
    }
    
}

/********************************************
 *
 ********************************************/
/*
void CrNotify_ShowAddressConflictText(CardReader_t* cr) {
    if (null == cr || !cr->isOnline)
        return;
    
    
    
}*/


/********************************************
 *
 ********************************************/
/*void CrNotify_ShowLinkAddressConflict(CardReader_t* cr,bool clearDisplay) {    
    if (null == cr || !cr->isOnline)
        return;
    
    CrQueue_Clear(cr->address);
    
    bool hasDisplay = CR_HasDisplay(cr);
    
    if (clearDisplay)
        CR_ClearDisplay(cr);
    
    if (hasDisplay) 
    {
        // no need to separate as function
        //CrNotify_ShowAddressConflictText(cr);
      
        //char text3[] = "ADDRESS CONFLICT !!!";
        //uint32 tl = 20;
    
        //CR_DisplayText(cr,_crXtextPlacement,1,text3,20);
        CR_DisplayText(cr,_crXtextPlacement,1,"ADDRESS CONFLICT !!!",20);
    }

#ifdef TEST_MODE_VIA_CR    
    CR_SingleKeyMode(cr,false);
#endif
    
    CR_RawPeripheralMode(cr->address,CRIM_HIGH_FREQUENCY,CRIM_ON,CRIM_HIGH_FREQUENCY,CRIM_OFF);

    if (hasDisplay) {    
        CrNotify_ShowLinkText(cr,false);    
        
        CrNotify_ShowCrInfo(cr);
    }
    
}*/

/********************************************
 *
 ********************************************/
/*void CrNotify_ShowLinkUp(CardReader_t* cr,bool clearDisplay) {    
    if (null == cr || !cr->isOnline)
        return;
    
    //if (cr->address == 3)
    //    asm("NOP");
    
    bool hasDisplay = CR_HasDisplay(cr);
    
    if (hasDisplay && clearDisplay)
        CR_ClearDisplay(cr);
    
#ifdef TEST_MODE_VIA_CR
    CR_SingleKeyMode(cr,false);
#endif
    
    CR_RawPeripheralMode(cr->address,CRIM_OFF,CRIM_HIGH_FREQUENCY,CRIM_OFF,CRIM_OFF);
    
    if (hasDisplay) {
        
        CrNotify_ShowLinkText(cr,true);  
        
        CrNotify_ShowCrInfo(cr);
    }
}*/

/********************************************
 *
 ********************************************/
void CrNotify_CROnlineStateChanged(CardReader_t* cr,bool isOnline) {
    if (isOnline && !Clsp_IsAssigned()) {
        if (Clsp_IsInConflict())
            CrNotify_ShowLink(cr,true,-1);
        else {
            //CR_RawAccessCommandParametric(1,CRMC_WAITING_FOR_GIN_CODE, 6);
            //CR_RawPeripheralMode(1,CRIM_HIGH_FREQUENCY, CRIM_ON, CRIM_ULTRA_LOW_FREQUENCY, CRIM_ON);
            CrNotify_ShowLink(cr,true,0);
        }
    }
    
    /*byte text[] ="OFFLINE";
    uint32 tl = 7;
    if (isOnline)
        tl = CopySafe("ONLINE",6,text,7,6);
    
    CrNotify_ReportCRonCR(cr->address,text,tl);*/
}

/********************************************
 *
 ********************************************/
void CrNotify_ClspNodeAssignmentChanged(uint32 logicalAddress,bool isAssigned) {
    if (!isAssigned) 
    {
        if (Clsp_IsInConflict())
        {
            SysInfo_SignalAddressConflict(true);
            
            {
                for(int i=1;i<=CR_MAX_COUNT;i++)
                    CrNotify_ShowLink(CrCommunicator_GetCR(i),true,-1);
            }
        }
        else 
        {  
            SysInfo_SignalConnectionDown(true);
           
            {
                for(int i=1;i<=CR_MAX_COUNT;i++)
                  CrNotify_ShowLink(CrCommunicator_GetCR(i),true,0);
                
            }
        }
    }
    else {
        SysInfo_StopSignalling();
        
        {
            for(int i=1;i<=CR_MAX_COUNT;i++)
                CrNotify_ShowLink(CrCommunicator_GetCR(i),true,1);
        }
    }
}

/********************************************
 *
 ********************************************/
void CrNotify_ClspNodeAddressConflicted(uint32 suggestedAddress) {
    SysInfo_SignalAddressConflict(true);
    
    for(int i=1;i<=CR_MAX_COUNT;i++)
        CrNotify_ShowLink(CrCommunicator_GetCR(i),true,-1);
}

/********************************************
 *
 ********************************************/
void CrNotify_Init() {
    CrCommunicator_BindOnlineStateChanged(CrNotify_CROnlineStateChanged);
    
    Clsp_BindOnNodeAssignmentChanged(CrNotify_ClspNodeAssignmentChanged);
    Clsp_BindOnNodeAddressConflicted(CrNotify_ClspNodeAddressConflicted);
}

/*
void CrNotify_ReportCRonCR(byte reportedCrAddress, byte* message,uint32 messageLength) {
    if (reportedCrAddress < 1 || reportedCrAddress > 2 ||
        messageLength == 0 || message == null)
        return;
    
    byte crAddressToReportOn = reportedCrAddress == 1 ? 2 : 1;
    
    CardReader_t* cr = CrCommunicator_GetCR(crAddressToReportOn);
    byte prefix[]= " :"; 
    prefix[0] = reportedCrAddress+ZERO_CHAR;
    
    if (cr->isOnline) {
        CR_DisplayText(cr,6,2,prefix,2);
        if (messageLength > 12)
            messageLength=12;
        CR_DisplayText(cr,8,2,message,messageLength);
    }
}*/

#endif // BOOTLOADER