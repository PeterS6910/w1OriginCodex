using System.Security.Cryptography;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Crypto
#else
namespace Contal.IwQuick.Crypto
#endif
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