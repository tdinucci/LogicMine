/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;

namespace LogicMine.Api.Web.Filter
{
  /// <summary>
  /// Contains general utility methods for string conversion
  /// </summary>
  public static class StringConverter
  {
    private static readonly HashSet<string> ConvertableTypes = new HashSet<string>();

    static StringConverter()
    {
      AddConvertableType(typeof(string), false);
      AddConvertableType(typeof(bool));
      AddConvertableType(typeof(byte));
      AddConvertableType(typeof(char));
      AddConvertableType(typeof(DateTime));
      AddConvertableType(typeof(decimal));
      AddConvertableType(typeof(float));
      AddConvertableType(typeof(double));
      AddConvertableType(typeof(short));
      AddConvertableType(typeof(int));
      AddConvertableType(typeof(long));
      AddConvertableType(typeof(ushort));
      AddConvertableType(typeof(uint));
      AddConvertableType(typeof(ulong));
    }

    /// <summary>
    /// Returns true if the provided type is convertable from/to string
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsConvertable(Type type)
    {
      return type != null && ConvertableTypes.Contains(type.ToString());
    }

    /// <summary>
    /// Converts a string to a given type
    /// </summary>
    /// <param name="stringValue"></param>
    /// <param name="toType"></param>
    /// <returns></returns>
    public static IComparable FromString(string stringValue, Type toType)
    {
      if (string.IsNullOrWhiteSpace(stringValue) || stringValue.ToLower() == "null")
        return null;

      if (toType == typeof(string))
        return stringValue;

      if (!toType.IsValueType)
        throw new InvalidOperationException($"Cannot convert '{stringValue}' to a '{toType}'");

      try
      {
        if (toType == typeof(bool) || toType == typeof(bool?))
          return Convert.ToBoolean(stringValue);
        if (toType == typeof(byte) || toType == typeof(byte?))
          return Convert.ToByte(stringValue);
        if (toType == typeof(char) || toType == typeof(char?))
          return Convert.ToChar(stringValue);
        if (toType == typeof(DateTime) || toType == typeof(DateTime?))
          return Convert.ToDateTime(stringValue);
        if (toType == typeof(decimal) || toType == typeof(decimal?))
          return Convert.ToDecimal(stringValue);
        if (toType == typeof(float) || toType == typeof(float?))
          return Convert.ToSingle(stringValue);
        if (toType == typeof(double) || toType == typeof(double?))
          return Convert.ToDouble(stringValue);
        if (toType == typeof(short) || toType == typeof(short?))
          return Convert.ToInt16(stringValue);
        if (toType == typeof(int) || toType == typeof(int?))
          return Convert.ToInt32(stringValue);
        if (toType == typeof(long) || toType == typeof(long?))
          return Convert.ToInt64(stringValue);
        if (toType == typeof(ushort) || toType == typeof(ushort?))
          return Convert.ToUInt16(stringValue);
        if (toType == typeof(uint) || toType == typeof(uint?))
          return Convert.ToUInt32(stringValue);
        if (toType == typeof(ulong) || toType == typeof(ulong?))
          return Convert.ToUInt64(stringValue);

        throw new InvalidOperationException($"Unexpected type for conversion: {toType}");
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Conversion from string to {toType} failed, see inner exception", ex);
      }
    }

    private static void AddConvertableType(Type type, bool includeNullable = true)
    {
      ConvertableTypes.Add(type.ToString());
      if (includeNullable)
      {
        var nullable = typeof(Nullable<>).MakeGenericType(type);
        ConvertableTypes.Add(nullable.ToString());
      }
    }
  }
}
