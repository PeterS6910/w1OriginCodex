using System;

namespace Contal.IwQuick
{
    /// <summary>
    /// general delegate for "void f()" method
    /// </summary>
    public delegate void DVoid2Void();

    /// <summary>
    /// general delegate for "void f(object target)" method
    /// </summary>
    public delegate void DObject2Void(Object inputObject);

    /// <summary>
    /// general delegate for "void f(params object[] targets)" method
    /// </summary>
    public delegate void DObjects2Void(params Object[] objectArray);

    /// <summary>
    /// general delegate for "void f(string input)" method
    /// </summary>
    public delegate void DString2Void(String inputString);

    /// <summary>
    /// general delegate for "string f()" method
    /// </summary>
    public delegate string DVoid2String();

    /// <summary>
    /// general delegate for "void f(Char input)" method
    /// </summary>
    public delegate void DChar2Void(char inputChars);

    /// <summary>
    /// general delegate for "void f(Char[] input)" method
    /// </summary>
    public delegate void DChars2Void(char[] inputChars);

    /// <summary>
    /// general delegate for "void f(bool input)" method
    /// </summary>
    public delegate void DBool2Void(bool inputBoolean);

    /// <summary>
    /// general delegate for "void f(int input)" method
    /// </summary>
    public delegate void DInt2Void(int inputInteger);

    /// <summary>
    /// general delegate for "void f(byte input)" method
    /// </summary>
    public delegate void DByte2Void(byte inputByte);

    /// <summary>
    /// general delegate for "void f(double input)" method
    /// </summary>
    public delegate void DDouble2Void(double inputDouble);

    /// <summary>
    /// general delegate for "void f(exception error)" method
    /// </summary>
    public delegate void DException2Void(Exception inputError);

    /// <summary>
    /// param-side generic delegate for "void f(T genericTypeInput)" method
    /// </summary>
    [Obsolete("Use Action<T1> delegate")]
    public delegate void DType2Void<T>(T param);

    /// <summary>
    /// return-side generic delegate for "T f()" method
    /// </summary>
    [Obsolete("Use Func<TResult> delegate")]
    public delegate T DVoid2Type<T>();

    /// <summary>
    /// both-side generic delegate for "TReturn f(TParam genericTypeInput)" method
    /// </summary>
    [Obsolete("Use Func<T1,TResult> delegate")]
    public delegate TReturn DType2Type<TReturn, TParam>(TParam param);

    /// <summary>
    /// param-side generic delegate for "void f(T genericTypeInput)" method
    /// </summary>
    [Obsolete("Use Action<T1,T2> delegate")]
    public delegate void D2Type2Void<T1, T2>(T1 param1, T2 param2);

    /// <summary>
    /// both-side generic delegate for "T2 f(T1 genericTypeInput)" method
    /// </summary>
    [Obsolete("Use Func<T1,T2,TResult> delegate")]
    public delegate TReturn D2Type2Type<TReturn, T1, T2>(T1 param1, T2 param2);

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use Action<T1,T2,T3> delegate")]
    public delegate void D3Type2Void<T1, T2, T3>(T1 param1, T2 param2, T3 param3);

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use Action<T1,T2,T3,T4> delegate")]
    public delegate void D4Type2Void<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3,T4 param4);

    /// <summary>
    ///
    /// </summary>
    [Obsolete("Use Func<T1,T2,T3,TResult> delegate")]
    public delegate TReturn D3Type2Type<TReturn, T1, T2,T3>(T1 param1, T2 param2,T3 param3);

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use Func<T1,T2,T3,T4,TResult> delegate")]
    public delegate TReturn D4Type2Type<TReturn, T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4);

}