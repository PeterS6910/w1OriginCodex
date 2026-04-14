#ifndef XPRESSO
	NAME StartApplication

    PUBLIC StartApplication
    PUBLIC StartBootloader

	SECTION `.text`:CODE:ROOT(2)
    THUMB
#else
	.syntax unified
	.text

	.align 2

    .section .text.StartApplication
    .global StartApplication
    .func
    .thumb_func
	.type	StartApplication, %function
#endif

StartApplication:
#ifndef BOOTLOADER16
    ldr r0, =0x00005008
    ldr r0, [r0]
    mov sp, r0
    ldr r0, =0x0000500C
    ldr r0, [r0]
    mov pc, r0
#else
    ldr r0, =0x00004008
    ldr r0, [r0]
    mov sp, r0
    ldr r0, =0x0000400C
    ldr r0, [r0]
    mov pc, r0
#endif

#ifdef XPRESSO
	.endfunc

	.align 2

    .section .text.StartBootloader
    .global StartBootloader
    .func
    .thumb_func
	.type	StartBootloader, %function
#endif

StartBootloader:
    ldr r0, =0x00000000
    ldr r0, [r0]
    mov sp, r0
    ldr r0, =0x00000004
    ldr r0, [r0]
	mov pc, r0

#ifndef XPRESSO
	END
#else
    .endfunc
#endif