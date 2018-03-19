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
using System.Collections.ObjectModel;
using System.Linq;

namespace LogicMine.Api.Patch
{
  /// <summary>
  /// A description of a modification to an object of type T
  /// </summary>
  /// <typeparam name="TId">The identity type of T</typeparam>
  /// <typeparam name="T">The type which the modification applies to</typeparam>
  public class Delta<TId, T> : IDelta<TId, T>
  {
    private readonly IDictionary<string, object> _modifiedProperties = new Dictionary<string, object>();

    /// <inheritdoc />
    public TId Identity { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> ModifiedProperties =>
      new ReadOnlyDictionary<string, object>(_modifiedProperties);

    /// <summary>
    /// Construct a new Delta
    /// </summary>
    /// <param name="identity">The identity of the T to modify</param>
    /// <param name="modifiedProperties">The modifications to apply to the identified T</param>
    public Delta(TId identity, IEnumerable<KeyValuePair<string, object>> modifiedProperties)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof(identity));
      if (modifiedProperties == null || !modifiedProperties.Any())
        throw new ArgumentException("Must not be null or empty", nameof(modifiedProperties));

      Identity = identity;
      ProcessModifiedProperties(modifiedProperties);
    }

    private void ProcessModifiedProperties(IEnumerable<KeyValuePair<string, object>> modifiedProperties)
    {
      var propertyNames = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p.Name);
      foreach (var modProp in modifiedProperties)
      {
        var lowerModProp = modProp.Key.ToLower();
        if (propertyNames.ContainsKey(lowerModProp))
        {
          var propName = propertyNames[lowerModProp];
          _modifiedProperties[propName] = modProp.Value;
        }
      }
    }

    /// <inheritdoc />
    public IDelta<TId, U> Convert<U>(Func<string, string> propertyNameConverter)
    {
      var convertedProps = new Dictionary<string, object>();
      foreach (var prop in ModifiedProperties)
      {
        var convertedPropName = propertyNameConverter(prop.Key);
        if (string.IsNullOrWhiteSpace(convertedPropName))
          throw new InvalidOperationException(
            $"No mapping from '{prop.Key}' on '{typeof(T)}' to '{typeof(U)}'");

        convertedProps[convertedPropName] = prop.Value;
      }

      return new Delta<TId, U>(Identity, convertedProps);
    }
  }
}
