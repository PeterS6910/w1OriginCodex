#pragma once
//#include "System\baseTypes.h"

/* I/O configuration */
#define BASE_INPUT_COUNT    4
#define BASE_OUTPUT_COUNT   4
#define MAX_INPUT_COUNT     16
#define MAX_OUTPUT_COUNT    12
//#define OUTPUTS_COUNT       4
//#define INPUTS_COUNT        4
#define INPUT_TOLEVEL1      327
#define INPUT_TOLEVEL2      440
#define INPUT_TOLEVEL3      593
#define INPUT_DIG_TOLEVEL1  512

/* DSM default timming configuration */
#define DSM_UNLOCK_TIME                         5000
#define DSM_OPEN_TIME                           20000
#define DSM_PREALARM_TIME                       5000
#define DSM_ARRAY_DELAY                         0
#define DSM_UNLOCK_DELAY                        0
#define DSM_LOCK_DELAY                          0
#define DSM_CLOSE_DELAY                         0
#define DSM_INTRUSION_DELAY                     0
#define DSM_STRIKE_PULSE_TIME                   1000
#define DSM_INTRUSION_LENGTH                    500                             /* Intrusion signal length (500 ms) */
#define DSM_MIN_BEFORE_UNLOCK_TIME              50                              /* Minimal time for before unlock (only if bypass is used) */
#define DSM_MAX_STOPPED_TIME                    5000                            /* If DSM is restarted within this time, and was stopped while door
                                                                                    was opened, it will go to door opened and not intrusion */

#define MAX_CARD_READER_ADDR    2

#define FLASH_MAX_BUFFER        64

#ifndef LON
#define UPGRADE_CHUNK_SIZE      64
#else
#define UPGRADE_CHUNK_SIZE      32
#endif

#define WATCHDOG_SWITCH_ADDR    0x10001fb0
#define HARDFAULT_SWITCH_ADDR   0x10001fb4
#define TIMER_SWITCH_ADDR       0x10001fb8
#define CTS_SWITCH_ADDR         0x10001fbc

#define APP_SWITCH_ADDR         0x10001fc0  // 0x10001BD8
#define UART_SWITCH_ADDR        0x10001fc4  // 0x10001BD0  
#define SYSTICK_SWITCH_ADDR     0x10001fc8  // 0x10001BD4


#define BOOT_SYSTICK_HANDLER    0x10001fcc  // 0x10001BC0          /* Pointer to systick handler for bootloader */
#define BOOT_UART_HANDLER       0x10001fd0  // 0x10001BC4          /* Pointer to UART0 handler for bootloader */

#ifndef LON
#define FORCE_SET_ADDR          0x10001fd4  // 0x10001BDC           /* Marks that the forced address should be used */
#define FORCE_ADDRESS_ADDR      0x10001fd8                          /* Forced address that should be used */
#else   // not LON version
#define BOOT_TIMER_HANDLER      0x10001fd4
#define BOOT_CTS_HANDLER        0x10001fd8
#endif

#define BOOT_HARDFAULT_HANDLER  0x10001fdc
#define BOOT_WDT_HANDLER        0x10001fe0

#define RUN_APPLICATION     0x12345678
#define RUN_UPGRADE         0x87654321
#define FORCE_SET_ADDRESS   0x12345678

#define FLASH_BUFFER        256
#define FLASH_WRITE_TRIGGER 128

#define FLASH_START_SECTOR  3
#define FLASH_END_SECTOR    8

#ifndef BOOTLOADER16

#define APPLICATION_BASE_ADDRESS        0x00005000
#define APPLICATION_LENGTH_ADDRESS      0x00005000 // 0x0000FFF8
#define APPLICATION_CHECKSUM_ADDRESS    0x00005004 // 0x0000FFFC

#define DCU_UART_HANDLER        0x00005090          /* Pointer to systick handler for DCU app. */
#define DCU_SYSTICK_HANDLER     0x00005044          /* Pointer to UART0 handler for DCU app. */
#define DCU_TIMER_HANDLER       0x00005088
#define DCU_CTS_HANDLER         0x000050ac

#else

#define APPLICATION_BASE_ADDRESS        0x00004000
#define APPLICATION_LENGTH_ADDRESS      0x00004000 // 0x0000FFF8
#define APPLICATION_CHECKSUM_ADDRESS    0x00004004 // 0x0000FFFC

#define DCU_HARDFAULT_HANDLER   0x00004014
#define DCU_UART_HANDLER        0x00004090          /* Pointer to systick handler for DCU app. */
#define DCU_SYSTICK_HANDLER     0x00004044          /* Pointer to UART0 handler for DCU app. */
#define DCU_TIMER_HANDLER       0x00004088
#define DCU_WTD_HANDLER         0x000040a0
#define DCU_CTS_HANDLER         0x000040ac

#endif

#define LAST_FLASH_ADDRESS              0x0000ffff

#define APP_LENGTH_CODE         0xfffe
#define APP_CHECKSUM_CODE       0xffff

#define MEMORY_WARNING_THRESHOLD01    80
#define MEMORY_WARNING_THRESHOLD10    70
#define MEMORY_RESET_THRESHOLD      95



