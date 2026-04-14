#include "DIPSwitch.h"
#include "System\GPIO\gpio.h"
#include "System\LPC122x.h"
#include "System\baseTypes.h"

void DIP_Init() 
{
    /* DIP 0 */
    LPC_IOCON->PIO0_20 |= (1 << 4) | (1 << 7); // pull-up & digital
    //GPIO_SetDir(PORT0, 20, 0);
    /* DIP 1 */
    LPC_IOCON->PIO0_21 |= (1 << 4) | (1 << 7); // pull-up & digital
    //GPIO_SetDir(PORT0, 21, 0);
    /* DIP 2 */
    LPC_IOCON->PIO0_22 |= (1 << 4) | (1 << 7); // pull-up & digital
    //GPIO_SetDir(PORT0, 22, 0);
    /* DIP 3 */
    LPC_IOCON->PIO0_23 |= (1 << 4) | (1 << 7); // pull-up & digital
    //GPIO_SetDir(PORT0, 23, 0);
    /* DIP 4 */
    LPC_IOCON->PIO0_24 |= (1 << 4) | (1 << 7); // pull-up & digital
    //GPIO_SetDir(PORT0, 24, 0);
    
    
    for (int i=20;i<=24;i++)            // optimization as replacement for Nx GPIO_SetDir call
        GPIO_SetDir(PORT0, i, 0);
    
    /* DIP 5 */
    LPC_IOCON->PIO0_27 |= (1 << 4) | (1 << 7); // pull-up & digital
    GPIO_SetDir(PORT0, 27, 0);
}

//volatile uint32 _dipvalues[DIP_COUNT];

int32 DIP_GetFrom(uint32 index)
{
    int32 val = -1;
    if (index < DIP_COUNT) {
        val = GPIO_GetValue(PORT0, index + 20);

        // in old version index 5 stays for pin index 27

        //_dipvalues[index] = val;
    }
    return val;
}