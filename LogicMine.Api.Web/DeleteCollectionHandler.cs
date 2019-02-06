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
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LogicMine.Api.Web
{
  /// <summary>
  /// A handler for "Delete Collection" requests
  /// </summary>
  /// <typeparam name="T">The type which the handler operates on</typeparam>
  /// <typeparam name="TResult">The type which the handler returns</typeparam>
  public class DeleteCollectionHandler<T, TResult> : Handler, IDeleteCollectionHandler
  {
    private readonly IDeleteCollectionShaft<T, TResult> _shaft;

    /// <summary>
    /// Construct a new DeleteCollectionHandler
    /// </summary>
    /// <param name="shaft">The shaft to process requests in</param>
    public DeleteCollectionHandler(IDeleteCollectionShaft<T, TResult> shaft)
    {
      _shaft = shaft ?? throw new ArgumentNullException(nameof(shaft));
    }
    
    /// <inheritdoc />
    public async Task<IActionResult> DeleteCollectionAsync(string filter)
    {
      try
      {
        var parsedFilter = new FilterParser<T>(filter).Parse();

        var basket = new DeleteCollectionBasket<T, TResult>(new DeleteCollectionRequest<T>(parsedFilter));
        await _shaft.SendAsync(basket);

        return GetOkActionResult(basket.AscentPayload);
      }
      catch (Exception ex)
      {
        return GetErrorResult(ex);
      }
    }
  }
}
