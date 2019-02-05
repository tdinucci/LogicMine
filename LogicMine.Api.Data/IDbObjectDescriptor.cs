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

namespace LogicMine.Api.Data
{
  /// <summary>
  /// A type which contains metadata related to objects which are stored in a database.
  /// </summary>
  public interface IDbObjectDescriptor
  {
    /// <summary>
    /// Check whether a particular property can be written to the database
    /// </summary>
    /// <param name="propertyName">The property to check</param>
    /// <returns>True if the property is writable</returns>
    bool CanWrite(string propertyName);

    /// <summary>
    /// Check whether a particular property can be read from the database
    /// </summary>
    /// <param name="propertyName">The property to check</param>
    /// <returns>True if the property is readable</returns>
    bool CanRead(string propertyName);

    /// <summary>
    /// Get the underlying column name which is mapped to a property on T
    /// </summary>
    /// <param name="propertyName">The property to get the mapped column name for</param>
    /// <returns>The column mapped to the provided property name</returns>
    string GetMappedColumnName(string propertyName);
    
    /// <summary>
    /// After a column value is read from the database but before it is mapped to a property on a 
    /// T it is passed through this method, giving the opportunity to project the value to a different 
    /// type ahead of being set on the T.
    /// </summary>
    /// <param name="columnValue">The read column value</param>
    /// <param name="propertyType">The type which the column value should be projected to</param>
    /// <returns>The value to set the mapped property on T to</returns>
    object ProjectColumnValue(object columnValue, Type propertyType);

    /// <summary>
    /// After a column value is read from the database but before it is mapped to a property on a 
    /// T it is passed through this method, giving the opportunity to project the value to a different 
    /// type ahead of being set on the T.
    /// </summary>
    /// <param name="columnValue">The read column value</param>
    /// <typeparam name="TProjected">The type which the column value should be projected to</typeparam>
    /// <returns>The value to set the mapped property on T to</returns>
    TProjected ProjectColumnValue<TProjected>(object columnValue);

    /// <summary>
    /// After a property value is read from a T but before it is written to the database it is passed 
    /// through this method, giving the opportunity to project the value to a different type ahead of 
    /// the write the the database.
    /// </summary>
    /// <param name="propertyValue">The property value</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>A value which is suitable to write to the database</returns>
    object ProjectPropertyValue(object propertyValue, string propertyName);
  }
}