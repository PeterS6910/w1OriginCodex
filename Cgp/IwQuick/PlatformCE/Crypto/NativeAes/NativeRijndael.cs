using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto.NativeAes
{
    public abstract class NativeRijndael : Rijndael
    {
        internal abstract class AesCryptoTransform : ICryptoTransform
        {
            public abstract void Dispose();

            public abstract int TransformBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount,
                byte[] outputBuffer,
                int outputOffset);

            public abstract byte[] TransformFinalBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount);

            public bool CanReuseTransform { get { return true; } }
            public bool CanTransformMultipleBlocks { get { return true; } }

            public int InputBlockSize { get { return 16; } }
            public int OutputBlockSize { get { return 16; } }
        }

        private readonly RijndaelManaged _rijndaelManaged = new RijndaelManaged();

        public override void GenerateKey()
        {
            _rijndaelManaged.KeySize = KeySize;
            _rijndaelManaged.GenerateKey();

            KeyValue = _rijndaelManaged.Key;
       }

        public override void GenerateIV()
        {
            _rijndaelManaged.KeySize = KeySize;
            _rijndaelManaged.GenerateIV();

            IVValue = _rijndaelManaged.IV;
        }
    }
}
