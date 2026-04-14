#pragma once

/**
 * defines priorities from highest=0 to lowest=3
 * in mapping defined by Cortex M0 - 2bit interrupt priority value
 */

#define SYSTICK_PRIO    0
#define TIMER32_PRIO    2
#define TIMER16_PRIO    3
#define UART_PRIO       1
#define WATCHDOG_PRIO   1
