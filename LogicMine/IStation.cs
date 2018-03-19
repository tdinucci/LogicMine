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
using System.Threading.Tasks;

namespace LogicMine
{
  /// <summary>
  /// A station is a waypoint within an <see cref="IShaft{TBasket}"/> which can process baskets as they descend to and 
  /// ascent from an <see cref="ITerminal{TBasket}"/>
  /// </summary>
  /// <typeparam name="TBasket">The type of basket the station deals with</typeparam>
  public interface IStation<in TBasket> where TBasket : IBasket
  {
    /// <summary>
    /// Called whenever the station is encountered within a shaft when the basket is descending
    /// </summary>
    /// <param name="basket">The basket travelling down the shaft</param>
    /// <returns>A Task that may be awaited</returns>
    Task DescendToAsync(TBasket basket);

    /// <summary>
    /// Called whenever the station is encountered within a shaft when the basket is ascending
    /// </summary>
    /// <param name="basket">The basket travelling up the shaft</param>
    /// <returns>A Task that may be awaited</returns>
    Task AscendFromAsync(TBasket basket);
  }
}
