#pragma once

#include "System\Core\system_LPC122x.h"

#define ONE_TENTH_MS_TIME_INTERVAL (_systemAHBFrequency / 10000 - 1) // every 100 usec
#define ONE_MS_TIME_INTERVAL	(_systemAHBFrequency / 1000 - 1) // every 1 ms
#define TEN_MS_TIME_INTERVAL	(_systemAHBFrequency / 100 - 1) // every 10 ms
