#pragma once
#include "System\baseTypes.h"

#define I2C_MASTER		            ((uint8) (0x01))
#define I2C_SLAVE		            ((uint8) (0x02))
#define PARAM_I2C_MODE(MODE)                (((MODE) == I2C_MASTER) || ((MODE) == I2C_SLAVE))

#define I2C_POLLING_MODE		    ((uint8) (0x00))
#define I2C_INTERRUPT_MODE		    ((uint8) (0x01))
#define PARAM_I2C_INTERRUPT_MODE(MODE) 	    (((MODE) == I2C_INTERRUPT_MODE) || ((MODE) == I2C_POLLING_MODE))

#define I2C_I2CONSET_AA			    ((uint8)(0x04))	/*!< Assert acknowledge flag */
#define I2C_I2CONSET_SI			    ((uint8)(0x08)) 	/*!< I2C interrupt flag */
#define I2C_I2CONSET_STO		    ((uint8)(0x10)) 	/*!< STOP flag */
#define I2C_I2CONSET_STA		    ((uint8)(0x20)) 	/*!< START flag */
#define I2C_I2CONSET_I2EN		    ((uint8)(0x40)) 	/*!< I2C interface enable */
#define PARAM_I2C_I2CONSET(I2C_I2CONSET)    ((((I2C_I2CONSET) & (uint8)0x83) == 0x00) && ((I2C_I2CONSET) != 0x00))

#define I2C_I2CONCLR_AAC		    ((uint8)(0x04))  	/*!< Assert acknowledge Clear bit */
#define I2C_I2CONCLR_SIC		    ((uint8)(0x08))	/*!< I2C interrupt Clear bit */
#define I2C_I2CONCLR_STAC		    ((uint8)(0x20))	/*!< START flag Clear bit */
#define I2C_I2CONCLR_I2ENC		    ((uint8)(0x40))	/*!< I2C interface Disable bit */
#define PARAM_I2C_I2CONCLR(I2C_I2CONCLR)    ((((I2C_I2CONCLR) & (uint8)0x93) == 0x00) && ((I2C_I2CONCLR) != 0x00))

#define I2C_I2MMCTRL_MM_ENA		    ((1<<0))		/*!< Monitor mode enable */
#define I2C_I2MMCTRL_ENA_SCL		    ((1<<1))		/*!< SCL output enable */
#define I2C_I2MMCTRL_MATCH_ALL		    ((1<<2))		/*!< Select interrupt register match */
#define PARAM_I2C_I2MMCTRL(I2C_I2MMCTRL)    ((((I2C_I2MMCTRL) & (uint8)0xF9) == 0x00) && ((I2C_I2MMCTRL) != 0x00))

#define I2C_I2ADR_GC			    ((1<<0))		/*!< General Call enable bit */
#define I2C_I2MASK_MASK(n)		    ((n&0xFE))		/*!< I2C Mask Register mask field */

#define I2C_I2STAT_M_TX_START		    ((0x08)) 		/*!< A start condition has been transmitted */
#define I2C_I2STAT_M_TX_RESTART		    ((0x10))	 	/*!< A repeat start condition has been transmitted */
#define I2C_I2STAT_M_TX_SLAW_ACK	    ((0x18))	 	/*!< SLA+W has been transmitted, ACK has been received */
#define I2C_I2STAT_M_TX_SLAW_NACK	    ((0x20))	  	/*!< SLA+W has been transmitted, NACK has been received */
#define I2C_I2STAT_M_TX_DAT_ACK		    ((0x28))		/*!< Data has been transmitted, ACK has been received */
#define I2C_I2STAT_M_TX_DAT_NACK	    ((0x30))	 	/*!< Data has been transmitted, NACK has been received */
#define I2C_I2STAT_M_TX_ARB_LOST	    ((0x38))	 	/*!< Arbitration lost in SLA+R/W or Data bytes */

#define I2C_I2STAT_M_RX_START		    ((0x08))		/*!< A start condition has been transmitted */
#define I2C_I2STAT_M_RX_RESTART		    ((0x10))		/*!< A repeat start condition has been transmitted */
#define I2C_I2STAT_M_RX_ARB_LOST	    ((0x38))		/*!< Arbitration lost */
#define I2C_I2STAT_M_RX_SLAR_ACK	    ((0x40))		/*!< SLA+R has been transmitted, ACK has been received */
#define I2C_I2STAT_M_RX_SLAR_NACK	    ((0x48))   		/*!< SLA+R has been transmitted, NACK has been received */
#define I2C_I2STAT_M_RX_DAT_ACK		    ((0x50))	  	/*!< Data has been received, ACK has been returned */
#define I2C_I2STAT_M_RX_DAT_NACK	    ((0x58))	 	/*!< Data has been received, NACK has been return */

#define I2C_I2STAT_S_RX_SLAW_ACK	    ((0x60))		/*!< Own slave address has been received, ACK has been returned */
#define I2C_I2STAT_S_RX_ARB_LOST_M_SLA	    ((0x68))		/*!< Arbitration lost in SLA+R/W as master */
#define I2C_I2STAT_S_RX_GENCALL_ACK		((0x70))        /*!< General call address has been received, ACK has been returned */
#define I2C_I2STAT_S_RX_ARB_LOST_M_GENCALL	((0x78))	/*!< Arbitration lost in SLA+R/W (GENERAL CALL) as master */
#define I2C_I2STAT_S_RX_PRE_SLA_DAT_ACK		((0x80))	/*!< Previously addressed with own SLV address;
 								 * Data has been received, ACK has been return */
#define I2C_I2STAT_S_RX_PRE_SLA_DAT_NACK	((0x88))	/*!< Previously addressed with own SLA;
								 * Data has been received and NOT ACK has been return */
#define I2C_I2STAT_S_RX_PRE_GENCALL_DAT_ACK	((0x90))	/*!< Previously addressed with General Call;
                                                                 * Data has been received and ACK has been return */
#define I2C_I2STAT_S_RX_PRE_GENCALL_DAT_NACK	((0x98))	/*!< Previously addressed with General Call;
								 * Data has been received and NOT ACK has been return */
#define I2C_I2STAT_S_RX_STA_STO_SLVREC_SLVTRX   ((0xA0))	/*!< A STOP condition or repeated START condition has
                                                                 * been received while still addressed as SLV/REC
                                                                 * (Slave Receive) or SLV/TRX (Slave Transmit) */

#define I2C_I2STAT_S_TX_SLAR_ACK	        ((0xA8))		/*!< Own SLA+R has been received, ACK has been returned */	
#define I2C_I2STAT_S_TX_ARB_LOST_M_SLA		((0xB0))		/*!< Arbitration lost in SLA+R/W as master */
#define I2C_I2STAT_S_TX_DAT_ACK			((0xB8))		/*!< Data has been transmitted, ACK has been received */
#define I2C_I2STAT_S_TX_DAT_NACK		((0xC0))	  	/*!< Data has been transmitted, NACK has been received */
#define I2C_I2STAT_S_TX_LAST_DAT_ACK		((0xC8))		/*!< Last data byte in I2DAT has been transmitted (AA = 0);
						                            ACK has been received */
#define I2C_SLAVE_TIME_OUT			0x10000UL		/*!< Time out in case of using I2C slave mode */

/**
 * @brief I2C Init structure definition
 */
typedef struct {
    uint8 Mode;               /*!< Specifies whether the I2C works in master or slave mode. This parameter can be a value of @ref I2C_mode */     
    uint32 ClockRate;         /*!< Specifies the I2C clock rate. */
    uint32 SlaveAddress;      /*!< Specifies the address in I2C slave mode. */
    uint8 InterruptMode;	/*!< Specifies I2C works in I2C interrupt mode or polling mode. */
} I2C_InitType_t;

void I2C_DeInit(void);
void I2C_Init(I2C_InitType_t* initStruct);

void I2C_SendByte(uint8 DataByte);
uint32 I2C_GetByte(void);

uint32 I2C_GetI2CStatus( void );
uint32 I2C_ReadFlag( uint32 I2C_I2CONSET );
void I2C_SetFlag( uint32 I2C_I2CONSET );
void I2C_ClearFlag( uint32 I2CONCLR );