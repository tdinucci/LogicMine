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
  /// A shaft is a processing pipeline.  An <see cref="IBasket"/> enters and decends through zero or 
  /// more <see cref="IStation{TBasket}"/>s until it reaches an <see cref="ITerminal{TBasket}"/>.  Each of the stations 
  /// has a chance to inspect and minipulate the baskets contents (and it may return the basket to the surface early if it wants to).  
  /// Once the basket hits the terminal it is filled with a result and sent bask up the shaft again, through the stations it met on the way down.  
  /// Each station again has the chance to inspect and manipulate the baskets contents before it emerges from the shaft.
  /// </summary>
  /// <typeparam name="TBasket">The type of basket the shaft deals with</typeparam>
  public interface IShaft<TBasket> : IShaft where TBasket : IBasket
  {
    /// <summary>
    /// Send a basket down the shaft.
    /// </summary>
    /// <param name="basket">A basket to send down the shaft</param>
    /// <returns>The basket which was sent down the shaft.  N.B. The 'basket' parameter and the returned TBasket will both point to the same object</returns>
    Task<TBasket> SendAsync(TBasket basket);
  }

  /// <summary>
  /// <para>A marker interface for mine shafts</para>
  /// <para>
  /// A shaft is a processing pipeline.  An <see cref="IBasket"/> enters and decends through zero or 
  /// more <see cref="IStation{TBasket}"/>s until it reaches an <see cref="ITerminal{TBasket}"/>.  Each of the stations 
  /// has a chance to inspect and minipulate the baskets contents (and it may return the basket to the surface early if it wants to).  
  /// Once the basket hits the terminal it is filled with a result and sent bask up the shaft again, through the stations it met on the way down.  
  /// Each station again has the chance to inspect and manipulate the baskets contents before it emerges from the shaft.
  /// </para>
  /// </summary>
  public interface IShaft
  {
  }
}