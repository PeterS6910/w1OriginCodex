using System;
using Contal.BoolExpressions.CrossPlatform;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IDcuStateAndSettings
    {
        IBoolExpression SabotageDcuInputs { get; }
        Guid SabotageDcuInputsOutputId { get; }
    }
}
