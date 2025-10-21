using System.Security.Cryptography;


namespace Contal.IwQuick.Crypto
{
    internal abstract class CryptoTranformPool : 
        AObjectPool<ICryptoTransform>
    {
        protected readonly SymmetricAlgorithm Algorithm;

        protected CryptoTranformPool(SymmetricAlgorithm algorithm)
        {
            Algorithm = algorithm;
        }
    }
}