﻿/*
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

namespace LogicMine.Api.Patch
{
  /// <summary>
  /// A description of a modification to an object of type T
  /// </summary>
  /// <typeparam name="TId">The identity type of T</typeparam>
  /// <typeparam name="T">The type which the modification applies to</typeparam>
  public interface IDelta<TId, T>
  {
    /// <summary>
    /// The identity of the object to modify
    /// </summary>
    TId Identity { get; }

    /// <summary>
    /// The modifications to apply to the T with the identity 'Identity'.
    /// Key = the T's property name
    /// Value = the new values for the property
    /// </summary>
    IReadOnlyDictionary<string, object> ModifiedProperties { get; }

    /// <summary>
    /// Convert the current delta to a delta on a different (but amost certainly similar in description) type
    /// </summary>
    /// <typeparam name="U">The type to bind the converted delta to</typeparam>
    /// <param name="propertyNameConverter">A delegate to convert property names from the current delta into property names for the new one</param>
    /// <returns>A delta bound to U which is based on the current delta</returns>
    IDelta<TId, U> Convert<U>(Func<string, string> propertyNameConverter);
  }
}