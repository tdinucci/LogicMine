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
using System.Data;

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A basic object-relational mapper
  /// </summary>
  /// <typeparam name="T">The type which the mapper operates on</typeparam>
  public interface IDbMapper<T>
  {
    /// <summary>
    /// Converts an IDataRecord to a T
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    T MapObject(IDataRecord record);

    /// <summary>
    /// Converts all records available in an IDataReader to a collection of T's
    /// </summary>
    /// <param name="reader">A datase reader</param>
    /// <returns>A collection of T's</returns>
    T[] MapObjects(IDataReader reader);
  }
}