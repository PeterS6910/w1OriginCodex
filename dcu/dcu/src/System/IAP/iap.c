#include "iap.h"
//#include "stdio.h"
#include "System\baseTypes.h"
#include "System\Core\system_LPC122x.h"
#include "System\LPC122x.h"

#define IAP_ADDRESS 0x1fff1ff1
#define IAP_CLK _systemFrequency

#define MAX_RESULT_CNT      4
#define MAX_COMMAND_CNT     5

#define IAP_CMD_PREPARE_SECTORS         50
#define IAP_CMD_COPY_RAM_TO_FLASH       51
#define IAP_CMD_ERASE_SECTORS           52
#define IAP_CMD_BLANK_CHECK_SECTORS     53
#define IAP_CMD_READ_PART_ID            54
#define IAP_CMD_READ_BOOT_CODE_VERSION  55
#define IAP_CMD_COMPARE                 56
#define IAP_CMD_REINVOKE_ISP            57
#define IAP_CMD_READ_UID                58
#define IAP_CMD_ERASE_PAGE              59
#define IAP_CMD_ERASE_INFO_PAGE         60

typedef struct
{
    uint32 command;
    uint32 params[MAX_COMMAND_CNT];
} IAP_Command_t;

typedef struct
{
    uint32 returnCode;
    uint32 result[MAX_RESULT_CNT];
} IAP_Result_t;
 
IAP_Result_t _iap_return;
IAP_Command_t _iap_command;
 
void iap_entry(unsigned param_tab[], unsigned result_tab[])
{
    void (*iap)(unsigned [], unsigned []); 
    iap = (void (*)(unsigned [], unsigned []))IAP_ADDRESS;
    iap(param_tab, result_tab);
}

IAP_StatusCodes_e IAP_PrepareSector(uint32 startSecNum, uint32 endSecNum) 
{
    if (endSecNum < startSecNum)
      return IAP_InvalidParameter;
    
    _iap_command.command = IAP_CMD_PREPARE_SECTORS;
    _iap_command.params[0] = startSecNum;
    _iap_command.params[1] = endSecNum;
    
    iap_entry((uint32*)(&_iap_command), (uint32*)(&_iap_return));
    
    return (IAP_StatusCodes_e)_iap_return.returnCode;
}

IAP_StatusCodes_e IAP_CopyRAMToFlash(uint32 dstAddress, uint32 srcAddress, uint32 number)
{
    _iap_command.command = IAP_CMD_COPY_RAM_TO_FLASH;
    _iap_command.params[0] = dstAddress;
    _iap_command.params[1] = srcAddress;
    _iap_command.params[2] = number;
    _iap_command.params[3] = IAP_CLK / 1000; // Fcclk in KHz
    
    iap_entry((uint32*)(&_iap_command), (uint32*)(&_iap_return));
    
    return (IAP_StatusCodes_e)_iap_return.returnCode;
}

IAP_StatusCodes_e IAP_EraseSector(uint32 startSecNum, uint32 endSecNum) 
{
    if (endSecNum < startSecNum)
        return IAP_InvalidParameter;
    
    _iap_command.command = IAP_CMD_ERASE_SECTORS;
    _iap_command.params[0] = startSecNum;
    _iap_command.params[1] = endSecNum;
    _iap_command.params[2] = IAP_CLK / 1000;
    
    iap_entry((uint32*)(&_iap_command), (uint32*)(&_iap_return));
    
    return (IAP_StatusCodes_e)_iap_return.returnCode;
}

IAP_StatusCodes_e IAP_ErasePage(uint32 startPage, uint32 endPage)
{
    if (endPage < startPage)
        return IAP_InvalidParameter;
    
    _iap_command.command = IAP_CMD_ERASE_PAGE;
    _iap_command.params[0] = startPage;
    _iap_command.params[1] = endPage;
    _iap_command.params[2] = IAP_CLK / 1000;
    
    iap_entry((uint32*)(&_iap_command), (uint32*)(&_iap_return));
    
    return (IAP_StatusCodes_e)_iap_return.returnCode;
}

volatile byte _uidArray[UID_LENGTH];
volatile bool _uidRead = false;

/* Read UID */
byte* IAP_ReadSerialNumber() 
{
    if (!_uidRead) {
        /* Read Unique Serial Number */
        _iap_command.command = IAP_CMD_READ_UID; /* IAP command ReadUID */
        iap_entry((uint32*)(&_iap_command), (uint32*)(&_iap_return));
    
        if(_iap_return.returnCode == 0) /* return: CODE SUCCESS */
        {
            _uidArray[0] = (_iap_return.result[3]&0xFF000000) >> 24;
            _uidArray[1] = (_iap_return.result[3]&0x00FF0000) >> 16;
            _uidArray[2] = (_iap_return.result[3]&0x0000FF00) >> 8;
            _uidArray[3] = (_iap_return.result[3]&0x000000FF);
            
            _uidArray[4] = (_iap_return.result[2]&0xFF000000) >> 24;
            _uidArray[5] = (_iap_return.result[2]&0x00FF0000) >> 16;
            _uidArray[6] = (_iap_return.result[2]&0x0000FF00) >> 8;
            _uidArray[7] = (_iap_return.result[2]&0x000000FF);
            
            _uidArray[8] = (_iap_return.result[1]&0xFF000000) >> 24;
            _uidArray[9] = (_iap_return.result[1]&0x00FF0000) >> 16;
            _uidArray[10] = (_iap_return.result[1]&0x0000FF00) >> 8;
            _uidArray[11] = (_iap_return.result[1]&0x000000FF);
            
            _uidArray[12] = (_iap_return.result[0]&0xFF000000) >> 24;
            _uidArray[13] = (_iap_return.result[0]&0x00FF0000) >> 16;
            _uidArray[14] = (_iap_return.result[0]&0x0000FF00) >> 8;
            _uidArray[15] = (_iap_return.result[0]&0x000000FF);
            
        }
        
        _uidRead = true;
    }

    return (byte*)_uidArray;
}