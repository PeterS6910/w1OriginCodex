using System;
using System.Text;

using System.IO;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Contal.IwQuick.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    public static class QuickCrypto
    {
        /// <summary>
        /// Encrypt a byte array into a byte array using a key and an IV 
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="key"></param>
        /// <param name="initializationVector"></param>
        /// <returns></returns>
        public static byte[] Encrypt(
            [NotNull] byte[] plainData, 
            [NotNull] byte[] key, 
            [NotNull] byte[] initializationVector)
        {
            // Create a MemoryStream to accept the encrypted bytes 

            var memoryStream = new MemoryStream();

            // Create a symmetric algorithm. 

            // We are going to use Rijndael because it is strong and
            // available on all platforms. 

            var algorithm = Rijndael.Create();
            //PaddingMode mode = aAlgorithm.Padding;

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because
            // the algorithm is operating in its default 
            // mode called CBC (Cipher Block Chaining).
            // The IV is XORed with the first block (8 byte) 
            // of the data before it is encrypted, and then each
            // encrypted block is XORed with the 
            // following block of plaintext.

            // This is done to make encryption more secure. 

            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 

            algorithm.Key = key;
            algorithm.IV = initializationVector;

            // Create a CryptoStream through which we are going to be
            // pumping our data. 

            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream and the output will be written
            // in the MemoryStream we have provided. 

            using (var cryptoStream = 
                    new CryptoStream(
                        memoryStream,
                        algorithm.CreateEncryptor(), 
                        CryptoStreamMode.Write))
            {
                // Write the data and make it do the encryption 

                cryptoStream.Write(plainData, 0, plainData.Length);

                // Close the crypto stream (or do FlushFinalBlock). 

                // This will tell it that we have done our encryption and
                // there is no more data coming in, 
                // and it is now a good time to apply the padding and
                // finalize the encryption process. 
                //
                // done by Dispose at the end
            }

            

            // Now get the encrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 

            var encryptedData = memoryStream.ToArray();

            return encryptedData;
        }

        private const string _simpleSaltPhrase = "some very simple salt";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saltPhrase"></param>
        /// <returns></returns>
        private static byte[] GetSaltBytes([CanBeNull] string saltPhrase)
        {
            if (Validator.IsNullString(saltPhrase))
                saltPhrase = _simpleSaltPhrase;

            return Encoding.ASCII.GetBytes(saltPhrase);
        }

        /// <summary>
        /// Encrypt a string into a string using a password 
        /// </summary>
        /// <returns></returns>
        /// <remarks>uses Encrypt(byte[], byte[], byte[]) </remarks>
        public static string Encrypt(
            [CanBeNull] string plainText, 
            [CanBeNull] string plainPassword, 
            [CanBeNull] string saltPhrase)
        {
            // First we need to turn the input string into a byte array. 

            var plainBytes = Encoding.Unicode.GetBytes(plainText ?? String.Empty);

            // Then, we need to turn the password into Key and IV 

            // We are using salt to make it harder to guess our key
            // using a dictionary attack

            var derivedBytes = new PasswordDeriveBytesSHA1(plainPassword ?? String.Empty, GetSaltBytes(saltPhrase));
                
            // Now get the key/IV and do the encryption using the
            // function that accepts byte arrays. 

            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key 
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV. 

            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael. 

            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size. 

            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes. 

            var encryptedData = 
                Encrypt(plainBytes,
                derivedBytes.GetBytes(32), 
                derivedBytes.GetBytes(16));

            // Now we need to turn the resulting byte array into a string. 

            // A common mistake would be to use an Encoding class for that.

            //It does not work because not all byte values can be

            // represented by characters. 

            // We are going to be using Base64 encoding that is designed

            //exactly for what we are trying to do. 

            return Convert.ToBase64String(encryptedData);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string plainPassword)
        {
            return Encrypt(plainText, plainPassword, null);
        }

        // Encrypt bytes into bytes using a password 

        //    Uses Encrypt(byte[], byte[], byte[]) 

        private static readonly byte[] _saltForDerivedBytes =
             {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] clearData, string plainPassword)
        {
            // We need to turn the password into Key and IV. 

            // We are using salt to make it harder to guess our key

            // using a dictionary attack - 

            // trying to guess a password by enumerating all possible words. 

            var derivedBytes = new PasswordDeriveBytesSHA1(plainPassword, _saltForDerivedBytes);
                

            // Now get the key/IV and do the encryption using the function
            // that accepts byte arrays. 

            // Using PasswordDeriveBytes object we are first getting
            // 32 bytes for the Key 
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV. 
            // IV should always be the block size, which is by default
            // 16 bytes (128 bit) for Rijndael. 
            // If you are using DES/TripleDES/RC2 the block size is 8
            // bytes and so should be the IV size. 
            // You can also read KeySize/BlockSize properties off the
            // algorithm to find out the sizes. 

            return Encrypt(clearData, derivedBytes.GetBytes(32), derivedBytes.GetBytes(16));

        }

        // Encrypt a file into another file using a password 

        /*
        public static void Encrypt(string fileIn,string fileOut, string Password)
        {

            // First we are going to open the file streams 

            FileStream fsIn = new FileStream(fileIn,
                FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from the

            // Password and create an algorithm 

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            Rijndael alg = Rijndael.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going

            // to be pumping data. 

            // Our fileOut is going to be receiving the encrypted bytes. 

            CryptoStream cs = new CryptoStream(fsOut,
                alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be processing

            // the input file in chunks. 

            // This is done to avoid reading the whole file (which can

            // be huge) into memory. 

            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 

                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // encrypt it 

                cs.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);

            // close everything 


            // this will also close the unrelying fsOut stream

            cs.Close();
            fsIn.Close();
        }
        */
        // Decrypt a byte array into a byte array using a key and an IV 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        public static byte[] Decrypt([NotNull] byte[] encryptedData, [NotNull] string plainPassword)
        {
            var derivedBytes = new PasswordDeriveBytesSHA1(plainPassword, _saltForDerivedBytes);

            return Decrypt(encryptedData, derivedBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="derivedBytes"></param>
        /// <returns></returns>
        private static byte[] Decrypt([NotNull] byte[] encryptedData, [NotNull] PasswordDeriveBytesSHA1 derivedBytes)
        {
            return Decrypt(encryptedData, derivedBytes.GetBytes(32), derivedBytes.GetBytes(16));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="key"></param>
        /// <param name="initializationVector"></param>
        /// <returns></returns>
        public static byte[] Decrypt([NotNull] byte[] encryptedData, [NotNull] byte[] key, [NotNull] byte[] initializationVector)
        {
            // Create a MemoryStream that is going to accept the
            // decrypted bytes 

            var memoryStream = new MemoryStream();

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 

            var algorithm = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because the algorithm
            // is operating in its default 
            // mode called CBC (Cipher Block Chaining). The IV is XORed with
            // the first block (8 byte) 
            // of the data after it is decrypted, and then each decrypted
            // block is XORed with the previous 
            // cipher block. This is done to make encryption more secure. 

            algorithm.Key = key;
            algorithm.IV = initializationVector;

            // Create a CryptoStream through which we are going to be
            // pumping our data. 

           // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream 
            // and the output will be written in the MemoryStream
            // we have provided. 

            using ( var cryptoStream = new CryptoStream(
                memoryStream,
                algorithm.CreateDecryptor(), 
                CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedData, 0, encryptedData.Length);

                // Close the crypto stream (or do FlushFinalBlock). 
                // This will tell it that we have done our decryption
                // and there is no more data coming in, 
                // and it is now a good time to remove the padding
                // and finalize the decryption process. 

            }
           
            // get the decrypted data from the MemoryStream. 
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 

            var plainData = memoryStream.ToArray();

            return plainData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="plainPassword"></param>
        /// <param name="saltPhrase"></param>
        /// <returns></returns>
        public static string Decrypt(
            [CanBeNull] string encryptedText, 
            [CanBeNull] string plainPassword, 
            [CanBeNull] string saltPhrase)
        {
            // First we need to turn the input string into a byte array. 
            // We presume that Base64 encoding was used 

            var encryptedBytes = Convert.FromBase64String(encryptedText ?? String.Empty);

            // Then, we need to turn the password into Key and IV 
            // We are using salt to make it harder to guess our key
            // using a dictionary attack - 
            // trying to guess a password by enumerating all possible words. 

            var derivedBytes = new PasswordDeriveBytesSHA1(plainPassword ?? String.Empty, GetSaltBytes(saltPhrase));

            // Now get the key/IV and do the decryption using
            // the function that accepts byte arrays. 
            // Using PasswordDeriveBytes object we are first
            // getting 32 bytes for the Key 
            // (the default Rijndael key length is 256bit = 32bytes)
            // and then 16 bytes for the IV. 
            // IV should always be the block size, which is by
            // default 16 bytes (128 bit) for Rijndael. 
            // If you are using DES/TripleDES/RC2 the block size is
            // 8 bytes and so should be the IV size. 
            // You can also read KeySize/BlockSize properties off
            // the algorithm to find out the sizes. 

            var plainData = Decrypt(encryptedBytes, derivedBytes.GetBytes(32), derivedBytes.GetBytes(16));

            // Now we need to turn the resulting byte array into a string. 
            // A common mistake would be to use an Encoding class for that.

            return Encoding.Unicode.GetString(plainData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string plainPassword)
        {
            return Decrypt(encryptedText, plainPassword, null);
        }

        /*

        public static byte[] Decrypt(byte[] cipherData, string Password)
        {
            // We need to turn the password into Key and IV. 

            // We are using salt to make it harder to guess our key

            // using a dictionary attack - 

            // trying to guess a password by enumerating all possible words. 

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            // Now get the key/IV and do the Decryption using the 

            //function that accepts byte arrays. 

            // Using PasswordDeriveBytes object we are first getting

            // 32 bytes for the Key 

            // (the default Rijndael key length is 256bit = 32bytes)

            // and then 16 bytes for the IV. 

            // IV should always be the block size, which is by default

            // 16 bytes (128 bit) for Rijndael. 

            // If you are using DES/TripleDES/RC2 the block size is

            // 8 bytes and so should be the IV size. 


            // You can also read KeySize/BlockSize properties off the

            // algorithm to find out the sizes. 

            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        // Decrypt a file into another file using a password 

        public static void Decrypt(string fileIn,
                    string fileOut, string Password)
        {

            // First we are going to open the file streams 

            FileStream fsIn = new FileStream(fileIn,
                        FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                        FileMode.OpenOrCreate, FileAccess.Write);

            // Then we are going to derive a Key and an IV from

            // the Password and create an algorithm 

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            Rijndael alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            // Now create a crypto stream through which we are going

            // to be pumping data. 

            // Our fileOut is going to be receiving the Decrypted bytes. 

            CryptoStream cs = new CryptoStream(fsOut,
                alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Now will will initialize a buffer and will be 

            // processing the input file in chunks. 

            // This is done to avoid reading the whole file (which can be

            // huge) into memory. 

            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 

                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // Decrypt it 

                cs.Write(buffer, 0, bytesRead);

            } while (bytesRead != 0);

            // close everything 

            cs.Close(); // this will also close the unrelying fsOut stream 

            fsIn.Close();
        }*/

    }
}
