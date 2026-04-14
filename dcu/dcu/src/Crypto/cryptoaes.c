/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
;
;	File:		Cryptoaes.c
;	Project:	CONTAL Card Reader FW
;	Created:	11/06/06
;	Version:	V1.0
;	by:	        Pavel SPANIK, CONTAL OK Ltd., ZILINA, SLOVAKIA
;				
;	Description:	Smart card handler - AES crypto module
;~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

/*
This is the source code for encryption using the latest AES algorithm.
AES algorithm is also called Rijndael algorithm. AES algorithm is 
recommended for non-classified by the National Institute of Standards 
and Technology(NIST), USA. Now-a-days AES is being used for almost 
all encryption applications all around the world.
It is not possible to describe the complete AES algorithm in detail 
here. For the complete description of the algorithm, point your 
browser to:
http://www.csrc.nist.gov/publications/fips/fips197/fips-197.pdf

Find the Wikipedia page of AES at:
http://en.wikipedia.org/wiki/Advanced_Encryption_Standard
******************************************************************
*/

#define CCRYPTOAES
#include <stdint.h>
//#include <string.h>

#include "System\baseTypes.h"
#include "cryptoaes.h"

#define MAX_MESSAGE_LENGTH  48

#define AesKeyLn    128             // used user key length
#define Nk          (AesKeyLn / 32) // The number of 32 bit words in the key.
#define Nr          (Nk + 6)        // The number of rounds in AES Cipher.
#define Nb          4               // The number of columns comprising a state in AES
#define AesBlockLim 3               // maximum of block (size buffer)

// in - it is the array that holds the plain text to be encrypted.
// out - it is the array that holds the key for encryption.
// state - the array that holds the intermediate results during encryption.
byte AES_in[AES_BLOCK_SIZE], AES_out[AES_BLOCK_SIZE];
static dword state[4][4];

// The array that stores the round keys.
byte RoundKey[16 * 11];         // expanded key array: 16 * 11 = 176 bytes for 128 bits key

byte AesKey[AES_KEY_SIZE];              // AES Key
byte byAesCbcIV[AES_IV_SIZE];          // encryption initialization vector

uint8_t _AES_buffer[MAX_MESSAGE_LENGTH];

//*************************************************************************************************
// SBOX table
const byte SBoxTable[256] = 
{
    //0     1     2     3     4     5     6     7     8    9     A      B     C     D     E     F
    0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,  //0
    0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,  //1
    0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,  //2
    0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,  //3
    0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,  //4
    0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,  //5
    0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,  //6
    0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,  //7
    0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,  //8
    0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,  //9
    0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,  //A
    0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,  //B
    0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,  //C
    0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,  //D
    0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,  //E
    0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16   //F
};

//-------------------------------------------------------------------------------------------------
// inverse SBOX table
const byte SBoxInvertTable[256] = 
{
    0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
    0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
    0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
    0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
    0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
    0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
    0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
    0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
    0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
    0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
    0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
    0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
    0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
    0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
    0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
    0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
};

//-------------------------------------------------------------------------------------------------
// The round constant dword array, Rcon[i], contains the values given by 
// x to th e power (i-1) being powers of x (x is denoted as {02}) in the field GF(28)
// Note that i starts at 1, not 0).
const byte Rcon[255] = 
{
    0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
    0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39,
    0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a,
    0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8,
    0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef,
    0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc,
    0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b,
    0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3,
    0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94,
    0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20,
    0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35,
    0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f,
    0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04,
    0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63,
    0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd,
    0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb  
};

//-------------------------------------------------------------------------------------------------
// xtime(x) = ((x<<1) ^ (((x>>7) & 1) * 0x1b))
const byte xtime[256] =
{
    0x00, 0x02, 0x04, 0x06, 0x08, 0x0a, 0x0c, 0x0e, 0x10, 0x12, 0x14, 0x16, 0x18, 0x1a, 0x1c, 0x1e,
    0x20, 0x22, 0x24, 0x26, 0x28, 0x2a, 0x2c, 0x2e, 0x30, 0x32, 0x34, 0x36, 0x38, 0x3a, 0x3c, 0x3e,
    0x40, 0x42, 0x44, 0x46, 0x48, 0x4a, 0x4c, 0x4e, 0x50, 0x52, 0x54, 0x56, 0x58, 0x5a, 0x5c, 0x5e,
    0x60, 0x62, 0x64, 0x66, 0x68, 0x6a, 0x6c, 0x6e, 0x70, 0x72, 0x74, 0x76, 0x78, 0x7a, 0x7c, 0x7e,
    0x80, 0x82, 0x84, 0x86, 0x88, 0x8a, 0x8c, 0x8e, 0x90, 0x92, 0x94, 0x96, 0x98, 0x9a, 0x9c, 0x9e,
    0xa0, 0xa2, 0xa4, 0xa6, 0xa8, 0xaa, 0xac, 0xae, 0xb0, 0xb2, 0xb4, 0xb6, 0xb8, 0xba, 0xbc, 0xbe,
    0xc0, 0xc2, 0xc4, 0xc6, 0xc8, 0xca, 0xcc, 0xce, 0xd0, 0xd2, 0xd4, 0xd6, 0xd8, 0xda, 0xdc, 0xde,
    0xe0, 0xe2, 0xe4, 0xe6, 0xe8, 0xea, 0xec, 0xee, 0xf0, 0xf2, 0xf4, 0xf6, 0xf8, 0xfa, 0xfc, 0xfe,
    0x1b, 0x19, 0x1f, 0x1d, 0x13, 0x11, 0x17, 0x15, 0x0b, 0x09, 0x0f, 0x0d, 0x03, 0x01, 0x07, 0x05,
    0x3b, 0x39, 0x3f, 0x3d, 0x33, 0x31, 0x37, 0x35, 0x2b, 0x29, 0x2f, 0x2d, 0x23, 0x21, 0x27, 0x25,
    0x5b, 0x59, 0x5f, 0x5d, 0x53, 0x51, 0x57, 0x55, 0x4b, 0x49, 0x4f, 0x4d, 0x43, 0x41, 0x47, 0x45,
    0x7b, 0x79, 0x7f, 0x7d, 0x73, 0x71, 0x77, 0x75, 0x6b, 0x69, 0x6f, 0x6d, 0x63, 0x61, 0x67, 0x65,
    0x9b, 0x99, 0x9f, 0x9d, 0x93, 0x91, 0x97, 0x95, 0x8b, 0x89, 0x8f, 0x8d, 0x83, 0x81, 0x87, 0x85,
    0xbb, 0xb9, 0xbf, 0xbd, 0xb3, 0xb1, 0xb7, 0xb5, 0xab, 0xa9, 0xaf, 0xad, 0xa3, 0xa1, 0xa7, 0xa5,
    0xdb, 0xd9, 0xdf, 0xdd, 0xd3, 0xd1, 0xd7, 0xd5, 0xcb, 0xc9, 0xcf, 0xcd, 0xc3, 0xc1, 0xc7, 0xc5,
    0xfb, 0xf9, 0xff, 0xfd, 0xf3, 0xf1, 0xf7, 0xf5, 0xeb, 0xe9, 0xef, 0xed, 0xe3, 0xe1, 0xe7, 0xe5
};

//*************************************************************************************************
// Internal functions
void KeyExpansion(void);
void MixColumns(void);
void SubBytes(void);
void ShiftRows(void);
void AddRoundKey(byte byRound);
void InvSubBytes(void);
void InvShiftRows(void);
void InvMixColumns(void);
void AES_BlockEnc(void);
void AES_BlockDec(void);

//*************************************************************************************************
// This function produces Nb(Nr+1) round keys. The round keys are used in each round to encrypt the states. 
void KeyExpansion(void)
{
    dword i,j;
    int32 jj;
    dword temp[4],k;

    // The first round key is the key itself.
    for(i=0;i<Nk;i++)
    {
        for(j=0;j<=3;j++)
            RoundKey[i*4+j]=AesKey[i*4+j];
        /*RoundKey[i*4]=AesKey[i*4];
        RoundKey[i*4+1]=AesKey[i*4+1];
        RoundKey[i*4+2]=AesKey[i*4+2];
        RoundKey[i*4+3]=AesKey[i*4+3];*/
    }
    // All other round keys are found from the previous round keys.

    while (i < (Nb * (Nr+1)))
    {
        for(j=0;j<4;j++)
        {
            temp[j]=RoundKey[(i-1) * 4 + j];
        }
        if (i % Nk == 0)
        {
            // This function rotates the 4 bytes in a dword to the left once.
            // [a0,a1,a2,a3] becomes [a1,a2,a3,a0]

            // Function RotWord()
            {
                k = temp[0];
                for(jj=0;jj<=2;jj++)
                    temp[jj] = temp[jj+1];
                /*temp[0] = temp[1];
                temp[1] = temp[2];
                temp[2] = temp[3];*/
                temp[3] = k;
            }

            // SubWord() is a function that takes a four-byte input dword and 
            // applies the S-box to each of the four bytes to produce an output dword.

            // Function Subword()
            {
                for(jj=0;jj<=3;jj++)
                    temp[jj]=SBoxTable[temp[jj]];
                /*temp[0]=SBoxTable[temp[0]];
                temp[1]=SBoxTable[temp[1]];
                temp[2]=SBoxTable[temp[2]];
                temp[3]=SBoxTable[temp[3]];*/
            }

            temp[0] =  temp[0] ^ Rcon[i/Nk];
        }
        else if (Nk > 6 && i % Nk == 4)
        {
            // Function Subword()
            {
                for (jj=0;jj<=3;jj++)
                    temp[jj]=SBoxTable[temp[jj]];
                /*temp[0]=SBoxTable[temp[0]];
                temp[1]=SBoxTable[temp[1]];
                temp[2]=SBoxTable[temp[2]];
                temp[3]=SBoxTable[temp[3]];*/
            }
        }
        
        for(jj=0;jj<=3;jj++)
            RoundKey[i*4+jj] = RoundKey[(i-Nk)*4+jj] ^ temp[jj];
        /*RoundKey[i*4+0] = RoundKey[(i-Nk)*4+0] ^ temp[0];
        RoundKey[i*4+1] = RoundKey[(i-Nk)*4+1] ^ temp[1];
        RoundKey[i*4+2] = RoundKey[(i-Nk)*4+2] ^ temp[2];
        RoundKey[i*4+3] = RoundKey[(i-Nk)*4+3] ^ temp[3];*/
        
        i++;
    }
}

//-------------------------------------------------------------------------------------------------
void MixColumns(void)
{
    dword i;
    dword Tmp,Tm,t;
    for(i=0;i<4;i++)
    {	
        t=state[0][i];
        Tmp = state[0][i] ^ state[1][i] ^ state[2][i] ^ state[3][i] ;
        Tm = state[0][i] ^ state[1][i] ; Tm = xtime[Tm]; state[0][i] ^= Tm ^ Tmp ;
        Tm = state[1][i] ^ state[2][i] ; Tm = xtime[Tm]; state[1][i] ^= Tm ^ Tmp ;
        Tm = state[2][i] ^ state[3][i] ; Tm = xtime[Tm]; state[2][i] ^= Tm ^ Tmp ;
        Tm = state[3][i] ^ t ; Tm = xtime[Tm]; state[3][i] ^= Tm ^ Tmp ;
    }
}

//-------------------------------------------------------------------------------------------------
// The SubBytes Function Substitutes the values in the
// state matrix with values in an S-box.
void SubBytes(void)
{
    dword i,j;
    for(i=0;i<4;i++)
    {
        for(j=0;j<4;j++)
        {
            state[i][j] = SBoxTable[state[i][j]];
        }
    }
}

//-------------------------------------------------------------------------------------------------
// The ShiftRows() function shifts the rows in the state to the left.
// Each row is shifted with different offset.
// Offset = Row number. So the first row is not shifted.
void ShiftRows(void)
{
    dword temp;

    // Rotate first row 1 columns to left	
    temp=state[1][0];
    state[1][0]=state[1][1];
    state[1][1]=state[1][2];
    state[1][2]=state[1][3];
    state[1][3]=temp;

    // mirror second row 2 columns	
    temp=state[2][0];
    state[2][0]=state[2][2];
    state[2][2]=temp;

    temp=state[2][1];
    state[2][1]=state[2][3];
    state[2][3]=temp;

    // Rotate third row 3 columns to left
    temp=state[3][0];
    state[3][0]=state[3][3];
    state[3][3]=state[3][2];
    state[3][2]=state[3][1];
    state[3][1]=temp;

}

//-------------------------------------------------------------------------------------------------
void AddRoundKey(byte byRound)
{
    dword i,j;
    for(i=0;i<4;i++)
    {
        for(j=0;j<4;j++)
        {
            state[j][i] ^= RoundKey[byRound * Nb * 4 + i * Nb + j];
        }
    }
}

//-------------------------------------------------------------------------------------------------
// The SubBytes Function Substitutes the values in the
// state matrix with values in an S-box.
void InvSubBytes(void)
{
    dword i,j;
    for(i=0;i<4;i++)
    {
        for(j=0;j<4;j++)
        {
            state[i][j] = SBoxInvertTable[state[i][j]];
        }
    }
}

//-------------------------------------------------------------------------------------------------
void InvShiftRows(void)
{
    dword temp;

    // Rotate first row 1 columns to rigth	
    temp=state[1][3];
    state[1][3]=state[1][2];
    state[1][2]=state[1][1];
    state[1][1]=state[1][0];
    state[1][0]=temp;

    // mirror second row 2 columns	
    temp=state[2][0];
    state[2][0]=state[2][2];
    state[2][2]=temp;

    temp=state[2][1];
    state[2][1]=state[2][3];
    state[2][3]=temp;

    // Rotate third row 3 columns to left
    temp=state[3][0];
    state[3][0]=state[3][1];
    state[3][1]=state[3][2];
    state[3][2]=state[3][3];
    state[3][3]=temp;
}

//-------------------------------------------------------------------------------------------------
// MixColumns function mixes the columns of the state matrix.
// The method used to multiply may be difficult to understand for the inexperienced.
// Please use the references to gain more information.
void InvMixColumns(void)
{
    dword u, v;

    // preprocessing for use MixColumn
    for(dword i=0; i<4; i++)
    {
        // col i
        u = xtime[xtime[state[0][i] ^ state[2][i]]];
        state[0][i] ^= u; 
        state[2][i] ^= u;
        v = xtime[xtime[state[1][i] ^ state[3][i]]];
        state[1][i] ^= v;
        state[3][i] ^= v;
    }
    // and use MixColumn function
    MixColumns();
}

//-------------------------------------------------------------------------------------------------
// AesBlockEnc is function that encrypts one block of the PlainText.
// input/output: state[4][4]

void AES_BlockEnc(void)
{
    // Add the First round key to the state before starting the rounds
    AddRoundKey(0);
    // There will be Nr rounds.
    // The first Nr-1 rounds are identical.
    // These Nr-1 rounds are executed in the loop below.
    for(dword Round = 1; Round < Nr; Round++)
    {
        SubBytes();
        ShiftRows();
        MixColumns();
        AddRoundKey(Round);
    }
    // The last round is given below.
    // The MixColumns function is not here in the last round
    SubBytes();
    ShiftRows();
    AddRoundKey(Nr);
}

//-------------------------------------------------------------------------------------------------
// AesBlockDec is function that decrypts one block of the PlainText.
// input/output: state[4][4]
void AES_BlockDec(void)
{
    // Add the last round key to the state before starting the rounds
    AddRoundKey(Nr);
    // There will be Nr rounds.
    // The first Nr-1 rounds are identical.
    // These Nr-1 rounds are executed in the loop below.
    for(dword Round = (Nr-1); Round > 0; Round--)
    {
        InvShiftRows();
        InvSubBytes();
        AddRoundKey(Round);
        InvMixColumns();
    }
    // The last round is given below.
    // The MixColumns function is not here in the last round
    InvShiftRows();
    InvSubBytes();
    AddRoundKey(0);
}

//*************************************************************************************************
//                                           AES ENCRYPTION
//*************************************************************************************************
// Cipher is the main function that encrypts the PlainText.
void AES_Cipher(void)
{
    dword i,j;

    //Copy the input PlainText to state array.
    for(i = 0; i < 4; i++)
    {
        for(j = 0; j < 4; j++)
        {
            state[j][i] = AES_in[i * 4 + j];
        }
    }

    AES_BlockEnc();          // block encryption

    // The encryption process is over.
    // Copy the state array to output array.
    for(i = 0; i < 4; i++)
    {
        for(j=0; j < 4; j++)
        {
            AES_out[i * 4 + j] = state[j][i];
        }
    }
}

//-------------------------------------------------------------------------------------------------
// AES encryption initialization for Card system service and data block decryption
void AES_CsBlockDec(byte *byData, byte *byAesKey)
{
    memcpy(AesKey, byAesKey, (Nk*4));           // key loading
    KeyExpansion();                             // ... expansion

    //Copy the input PlainText to state array.
    for(dword i = 0; i < 4; i++)
    {
        for(dword j = 0; j < 4; j++)
        {
            state[j][i] = byData[i * 4 + j];
        }
    }

    AES_BlockDec();          // block encryption

    // The decryption process is over.
    // Copy the state array to output array.
    for(dword i = 0; i < 4; i++)
    {
        for(dword j = 0; j < 4; j++)
        {
            byData[i * 4 + j] = state[j][i];
        }
    }
}

//*************************************************************************************************
//                                           AES DECRYPTION
//*************************************************************************************************
// InvCipher is the main function that decrypts the CipherText.
void AES_InvCipher(void)
{
    uint32 i,j;

    //Copy the input CipherText to state array.
    for(i = 0; i < 4; i++)
    {
        for(j = 0; j < 4; j++)
        {
            state[j][i] = AES_in[i * 4 + j];
        }
    }

    AES_BlockDec();          // block encryption

    // The decryption process is over.
    // Copy the state array to output array.
    for(i = 0; i < 4; i++)
    {
        for(j = 0; j < 4; j++)
        {
            AES_out[i * 4 + j] = state[j][i];
        }
    }
}

//---------------------------------------------------------------------------//
// AES Init Vector clear                                                     //
//---------------------------------------------------------------------------//
void AES_IVClr(void)
{
    uint32 i;
    for(i = 0; i < 16; i++)
    {
        byAesCbcIV[i]=0;
    }
}

void AES_IVInit(uint8_t* initVector)
{
    memcpy(byAesCbcIV, initVector, AES_IV_SIZE);
}

//---------------------------------------------------------------------------//
// AES encryption initialization                                             //
// input: pointer to AES key                                                 //
// result: AES key initializing                                              //
//---------------------------------------------------------------------------//
void AES_KeyInit(byte *byAesKey)
{
    uint32 i;

    for(i = 0; i < Nk * 4; i++)
        AesKey[i] = byAesKey[i];
    
    // The KeyExpansion routine must be called before encryption.
    KeyExpansion();
}

//---------------------------------------------------------------------------//
// AES CBC encryption                                                        //
// input: pointer to buffer with plain text, number of block                 //
//        (size of buffer must be respected)                                 //
// result: Cipher text in buffer                                             //
//---------------------------------------------------------------------------//
void AES_CbcEnc(byte *byBuffer, byte byBlockNum)
{
    uint32 i, j, byCnt;

    for(byCnt = 0; byCnt < (byBlockNum * 16); byCnt += 16)
    {
        //Copy the input PlainText to state array and initialization with init vector
        for(i = 0; i < 4; i++)
        {
            for(j=0; j < 4; j++)
            {
                state[j][i] = byBuffer[byCnt + i * 4 + j] ^ byAesCbcIV[i * 4 + j];
            }
        }

        AES_BlockEnc();                  // block encryption

        // The encryption process is over.
        // Copy the state array to output array and  next block initialization vector
        for(i = 0; i < 4; i++)
        {
            for(j = 0; j < 4; j++)
            {
                byBuffer[byCnt + i * 4 + j] = state[j][i];
                byAesCbcIV[i * 4 + j] = state[j][i];
            }
        }
    }
}

//---------------------------------------------------------------------------//
// AES CBC decryption                                                        //
// input: pointer to buffer with cipher text, number of block                //
//        (size of buffer must be respected)                                 //
// result: plain text in buffer                                             //
//---------------------------------------------------------------------------//
void AES_CbcDec(byte *byBuffer, byte byBlockNum)
{
    int32 i, j, byCnt;

    for(byCnt = 0; byCnt < (byBlockNum * 16); byCnt += 16)
    {
        //Copy the input CipherText to state array
        for(i = 0; i < 4; i++)
        {
            for(j=0; j < 4; j++)
            {
                state[j][i] = byBuffer[byCnt + i * 4 + j];
            }
        }

        AES_BlockDec();          // block encryption

        // The decryption process is over.
        // Copy the state array to output array.
        for(i = 0; i < 4; i++)
        {
            for(j = 0; j < 4; j++)
            {
                state[j][i] = state[j][i] ^ byAesCbcIV[i * 4 + j];          // XOR with init vector
                byAesCbcIV[i * 4 + j] = byBuffer[byCnt + i * 4 + j];        // init for next block 
                byBuffer[byCnt + i * 4 + j] = state[j][i];                  // save block
            }
        }
    }
}

uint8_t* AES_Encrypt(uint8_t* plainText, uint32_t plainSize, uint32_t* cipherSize)
{
    if (plainSize >= MAX_MESSAGE_LENGTH)    
        return null;
    
    uint32_t i;
    
    /* Copy to working buffer and add padding as neccessary */
    memcpy(_AES_buffer, plainText, plainSize);
    
    /* Assume smallest possible */
    uint32_t encryptedLength = AES_BLOCK_SIZE;
    while (encryptedLength <= plainSize)
        encryptedLength += AES_BLOCK_SIZE;
    
    uint32_t nValue = encryptedLength - plainSize;
    for (i = plainSize; i < encryptedLength; i++)
        _AES_buffer[i] = nValue;
   
    *cipherSize = encryptedLength;    
    AES_CbcEnc(_AES_buffer, encryptedLength / AES_BLOCK_SIZE);
    
    return _AES_buffer;
}

uint8_t* AES_Decrypt(uint8_t* cipherText, uint32_t cipherSize, uint32_t* plainSize)
{
    if (cipherSize % AES_BLOCK_SIZE != 0)
        return null;
    
    uint32_t numberOfBlocks = cipherSize / AES_BLOCK_SIZE;
    
    AES_CbcDec(cipherText, numberOfBlocks);
    
    /* If last character value is greater than max block size -> decryption error */
    uint32_t lastId = numberOfBlocks * AES_BLOCK_SIZE - 1;
    if (cipherText[lastId] > AES_BLOCK_SIZE)
        return null;
    
    /* Remove padding from the final size */    
    *plainSize = (numberOfBlocks * AES_BLOCK_SIZE) - cipherText[lastId];
    /* Copy data and set the pointer to the local buffer */
    memcpy(_AES_buffer, cipherText, *plainSize);
    
    return _AES_buffer;
}

//*************************************************************************************************
//                                              AES TEST
//*************************************************************************************************

#ifdef AEStest
    void AESTEST(void)
    {
	    uint32 i;

        // Part 1 is for demonstrative purpose. The key and plaintext are given in the program itself.
        // 	Part 1: ********************************************************
    	
	    // The array temp stores the key.
	    // The array temp2 stores the plaintext.
	    byte temp[16] = {0x00  ,0x01  ,0x02  ,0x03  ,0x04  ,0x05  ,0x06  ,0x07  ,0x08  ,0x09  ,0x0a  ,0x0b  ,0x0c  ,0x0d  ,0x0e  ,0x0f};
	    byte temp2[16]= {0x00  ,0x11  ,0x22  ,0x33  ,0x44  ,0x55  ,0x66  ,0x77  ,0x88  ,0x99  ,0xaa  ,0xbb  ,0xcc  ,0xdd  ,0xee  ,0xff};
    	
        // vypocitana hodnota OUT cez c-spy:
        // 0x69,0xC4,0xE0,0xD8,0x6A,0x7B,0x04,0x30,0xD8,0xCD,0xB7,0x80,0x70,0xB4,0xC5,0x5A

        // ************************************** ENCRYPTION *****************************************


	    // Copy the Key and PlainText
	    for(i=0;i<Nk*4;i++)
	    {
		    AesKey[i]=temp[i];
		    in[i]=temp2[i];
	    }

	    // The KeyExpansion routine must be called before encryption.
	    KeyExpansion();

	    // The next function call encrypts the PlainText with the Key using AES algorithm.
	    AesCipher();

        // ************************************** DECRYPTION *****************************************

        // Copy the CipherText
	    for(i=0;i<Nk*4;i++)
	    {
		    in[i]=out[i];
	    }

	    // The next function call decrypts the CipherText with the Key using AES algorithm.
	    AesInvCipher();
    }
#endif
