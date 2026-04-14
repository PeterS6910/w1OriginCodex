#ifndef CRYPTO_AES_H__
#define CRYPTO_AES_H__

#define AES_BLOCK_SIZE  16
#define AES_KEY_SIZE    16
#define AES_IV_SIZE     16

//-------------------------------------- ccryptoaes definitions ----------------------------------------

#ifndef CCRYPTOAES
    extern byte in[16], out[16];
#endif

//************************************* AES encryption functions ***************************************
void AES_IVClr(void);                                // AES init vector clearing
void AES_CsBlockDec(byte *byData, byte *byAesKey);                   // AES block decryption with key expansion 

void AES_Cipher(void);                               // AES block encryption function
void AES_InvCipher(void);                            // AES block decryption function

void AES_CbcEnc(byte *byPlain, byte byBlockNum);     // AES CBC encryption
void AES_CbcDec(byte *byCipher, byte byBlockNum);    // AES CBC decryption


/* The important stuff ;) */
/* IV and key init routines */
void AES_IVInit(uint8* initVector);
void AES_KeyInit(byte *byAesKey);                    

/* Wrapper routines for encryption / decryption, also handles padding PKCS7, returns pointer to result array if ok, NULL on error */
uint8* AES_Encrypt(uint8* plainText, uint32 plainSize, uint32* cipherSize);
uint8* AES_Decrypt(uint8* cipherText, uint32 cipherSize, uint32* plainSize);



//void AESTEST(void);

#endif