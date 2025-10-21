using System;

using System.Collections.Generic;
namespace Contal.IwQuick
{
// ReSharper disable RedundantNameQualifier
public partial class IwQuickLibraryInfo
{

protected override IEnumerable<Type> GetLwSerializableClasses()
{
// LwSerialize.Code 0x03FC
yield return typeof(Contal.IwQuick.ADisposable);

// LwSerialize.Code 0x03FD
yield return typeof(Contal.IwQuick.Data.TimeoutDictionary<,>);

// LwSerialize.Code 0x03FE
// nested private type
yield return GetExecutingAssembly().GetType("Contal.IwQuick.Data.TimeoutDictionary`2+TimeoutDictionaryValueCarrier");

// LwSerialize.Code 0x03FF
yield return typeof(Contal.IwQuick.Data.SyncDictionary<,>);


}
}
}
// ReSharper restore RedundantNameQualifier