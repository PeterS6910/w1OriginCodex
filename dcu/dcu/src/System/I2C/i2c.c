#include "i2c.h"
#include "System\LPC122x.h"
#include "System\Core\system_LPC122x.h"

/**
  * @brief  De-initializes the I2C peripheral registers to their default reset values.
  *
  * @param  None
  * @retval None  
  */
void I2C_DeInit(void)
{
    LPC_I2C->CONCLR = I2C_I2CONCLR_I2ENC;		/*!< Disable I2C control */
    LPC_SYSCON->SYSAHBCLKCTRL &= ~(1 << 5);	        /*!< Disable power for I2C module */
}

/**
  * @brief  Setup clock rate for I2C peripheral.
  *
  * @param TargetClock: speed of I2C bus(bps).
  * @retval None  
  */
void I2C_SetClock(uint32_t TargetClock)
{
    uint32_t temp;

    temp = _systemAHBFrequency  / TargetClock;

    /* Set the I2C clock value to register */
    LPC_I2C->SCLH = (uint32_t)(temp / 2);
    LPC_I2C->SCLL = (uint32_t)(temp - LPC_I2C->SCLH);
}

/**
  * @brief  Initializes the i2c peripheral with specified parameter.
  *
  * @param  i2C_InitStruct: pointer to a I2C_InitStruct structure that 
  *         contains the configuration information for the I2C peripheral.
  * @retval None
  */
void I2C_Init(I2C_InitType_t* initStruct)
{
    /* Enable I2C clock and de-assert reset */
    LPC_SYSCON->PRESETCTRL |= (0x1 << 1);
    LPC_SYSCON->SYSAHBCLKCTRL |= (1 << 5);

	/*  I2C I/O config */    
    LPC_IOCON->PIO0_10 &= ~0x3F;	/*  I2C I/O config */
    LPC_IOCON->PIO0_10 |= 0x02;		/* I2C SCL */
    LPC_IOCON->PIO0_11 &= ~0x3F;
    LPC_IOCON->PIO0_11 |= 0x02;		/* I2C SDA */
    
    /*--- Clear flags ---*/
    LPC_I2C->CONCLR = I2C_I2CONCLR_AAC | I2C_I2CONCLR_SIC | I2C_I2CONCLR_STAC | I2C_I2CONCLR_I2ENC;    

    /*--- Enable Ture Open Drain mode ---*/
    LPC_IOCON->PIO0_10 |= (0x1 << 10);
    LPC_IOCON->PIO0_11 |= (0x1 << 10);

    /*--- Set Clock rate ---*/
    I2C_SetClock(initStruct->ClockRate);

    if (initStruct->Mode == I2C_SLAVE)
    {
        LPC_I2C->ADR0 = initStruct->SlaveAddress;
    }    

    /* Enable the I2C Interrupt */
    if (initStruct->InterruptMode == I2C_INTERRUPT_MODE)
    {
        NVIC_EnableIRQ(I2C_IRQn);
    }
    else if (initStruct->InterruptMode == I2C_POLLING_MODE) 	
    {
        /* Disable the I2C Interrupt */
        NVIC_DisableIRQ(I2C_IRQn);		
    }

    if (initStruct->Mode == I2C_MASTER)
    {
        LPC_I2C->CONSET = I2C_I2CONSET_I2EN;
    } 
    else if (initStruct->Mode == I2C_SLAVE)
    {
        LPC_I2C->CONSET = I2C_I2CONSET_I2EN | I2C_I2CONSET_SI;
    }
}

/**
  * @brief  
  *
  *	@param  DataByte: specifies the data byte will be sent.
  * @retval None 
  */
void I2C_SendByte(uint8_t dataByte)
{
    LPC_I2C->DAT = dataByte; 
}
/**
  * @brief  
  *
  *	@param	
  * @retval The byte read from DAT register.  
  */

uint32 I2C_GetByte(void)
{
    return (LPC_I2C->DAT);
}

/**
  * @brief  Get I2C Status Byte.
  *
  *	@param	
  * @retval The value of the status byte.  
  */
uint32 I2C_GetI2CStatus(void)
{
    return (LPC_I2C->STAT);
}

/**
  * @brief  Read the I2C_I2CONSET bit.
  *
  *	@param  I2C_I2CONSET: specifies the bits will be read.
  *         This parameter can be one of the following values:
  *				@arg I2C_I2CONSET_AA: Assert acknowledge flag
  *				@arg I2C_I2CONSET_SI: I2C interrupt flag
  *				@arg I2C_I2CONSET_STO: STOP flag
  *  			@arg I2C_I2CONSET_STA: START flag
  *				@arg I2C_I2CONSET_I2EN: I2C interface enable
  * @retval The I2C_CONSET bit value.  
  */
uint32 I2C_ReadFlag(uint32 consetFlag)
{
    return (LPC_I2C->CONSET & consetFlag);      /* return flag */
}

/**
  * @brief  Set the I2C_I2CONSET bit.
  *
  *	@param  I2C_I2CONSET: specifies the bits will be set.
  *         This parameter can be one of the following values:
  *				@arg I2C_I2CONSET_AA: Assert acknowledge flag
  *				@arg I2C_I2CONSET_SI: I2C interrupt flag
  *				@arg I2C_I2CONSET_STO: STOP flag
  *  			@arg I2C_I2CONSET_STA: START flag
  *				@arg I2C_I2CONSET_I2EN: I2C interface enable
  * @retval None  
  */
void I2C_SetFlag(uint32 consetFlags)
{
    LPC_I2C->CONSET = consetFlags;      /* Set flag */
}

/**
  * @brief  Clear the I2C_I2CONCLR bit.
  *
  *	@param  I2C_I2CONCLR: specifies the bits will be clear.
  *         This parameter can be one of the following values:
  *				@arg I2C_I2CONSET_AA: Assert acknowledge flag
  *				@arg I2C_I2CONSET_SI: I2C interrupt flag
  *  			@arg I2C_I2CONSET_STA: START flag
  *				@arg I2C_I2CONSET_I2EN: I2C interface enable
  * @retval  
  */
void I2C_ClearFlag(uint32 clearFlags)
{
    LPC_I2C->CONCLR = clearFlags;      /* Clear flag */
}