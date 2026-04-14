/*
 * GENERATED FILE - DO NOT EDIT
 * (c) Code Red Technologies Ltd, 2008-2014
 * (c) NXP Semiconductors 2013-2014
 * Linker script memory definitions
 * Created from LinkMemoryTemplate
 * By LPCXpresso v7.2.0 [Build 153] [2014-05-19]  on Tue May 27 10:11:14 CEST 2014)
*/

/*-Sizes-*/
  __ICFEDIT_size_cstack__   = 0x600;
  __ICFEDIT_size_heap__     = 0x3a0;

MEMORY
{
  /* Define each memory region */
  CRC	   (rx) : ORIGIN = 0x00004000, LENGTH = 0x0008 /* 8 bytes */
  INTVEC   (rx) : ORIGIN = 0x00000000, LENGTH = 0x00C0 /* 192 bytes */
  ROM	   (rx) : ORIGIN = 0x00000124, LENGTH = 0xBEDC /* 64K bytes */
  RamLoc8 (rwx) : ORIGIN = 0x10000000, LENGTH = 0x1FA0 /* 8K bytes */
}
  

  /* Define a symbol for the top of each memory region */
  __top_ROM = 0x412C + 0xBED3;
  __top_RamLoc8 = 0x10000000 + 0x1FA0;

