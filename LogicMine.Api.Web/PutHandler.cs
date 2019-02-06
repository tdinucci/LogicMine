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
using System.Threading.Tasks;
using LogicMine.Api.Put;
using Microsoft.AspNetCore.Mvc;

namespace LogicMine.Api.Web
{
  /// <summary>
  /// A handler for "Put" operations
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type the handler deals with</typeparam>
  /// <typeparam name="TResult">The result type of the put operation</typeparam>
  public class PutHandler<TId, T, TResult> : Handler, IPutHandler<TId, T>
  {
    private readonly IPutShaft<TId, T, TResult> _shaft;

    /// <summary>
    /// Construct a new PutHandler
    /// </summary>
    /// <param name="shaft">The shaft which operations are performed within</param>
    public PutHandler(IPutShaft<TId, T, TResult> shaft)
    {
      _shaft = shaft ?? throw new ArgumentNullException(nameof(shaft));
    }

    /// <inheritdoc />
    public async Task<IActionResult> PutAsync(TId identity, T obj)
    {
      try
      {
        var request = new PutRequest<TId, T>(identity, obj);
        var putBasket = await _shaft.SendAsync(new PutBasket<TId, T, TResult>(request));

        return GetOkActionResult(putBasket.AscentPayload);
      }
      catch (Exception ex)
      {
        return GetErrorResult(ex);
      }
    }
  }
}
