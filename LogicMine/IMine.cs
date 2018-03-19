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

namespace LogicMine
{
  /// <summary>
  /// A mine consists of a set of shafts and each shaft performs processing on some data type.
  /// </summary>
  /// <typeparam name="T">The type which the mine operates on</typeparam>
  public interface IMine<T> : IMine
  {
    /// <summary>
    /// Add a new layer of stations to the bottom of the mine, but above the terminal
    /// </summary>
    /// <param name="layer">The layer of stations</param>
    /// <returns>The current mine</returns>
    IMine<T> Add(IStationLayer<T> layer);

    /// <summary>
    /// Get all stations in this mine which can deal with baskets of type TBasket
    /// </summary>
    /// <typeparam name="TBasket">The basket type stations are required for</typeparam>
    /// <returns>All stations which can handle TBaskets</returns>
    IStation<TBasket>[] GetStations<TBasket>() where TBasket : IBasket;
  }

  /// <summary>
  /// <para>A mine consists of a set of shafts and each shaft performs processing on some data type.</para>
  /// </summary>
  public interface IMine
  {
    /// <summary>
    /// The type which the mine operates on
    /// </summary>
    Type DataType { get; }
  }
}
