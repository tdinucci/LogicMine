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
  /// A terminal is a waypoint at the bottom of an <see cref="IShaft{TBasket}"/> which adds a result to a basket
  /// </summary>
  /// <typeparam name="TBasket">The type of basket the station deals with</typeparam>
  public interface ITerminal<in TBasket> where TBasket : IBasket
  {
    /// <summary>
    /// Add the result to the basket
    /// </summary>
    /// <param name="basket">The basket to process and add the result to</param>
    /// <param name="visit">The visit the basket is currently making</param>
    /// <returns>A Task that may be awaited</returns>
    Task AddResultAsync(TBasket basket, IVisit visit);
  }
}
