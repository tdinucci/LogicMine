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
using System.Threading.Tasks;
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogicMine.Api.Web
{
  /// <summary>
  /// A controller which offers the standard set of API operations
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the controller deals with</typeparam>
  public class GeneralResourceController<TId, T> : Controller
  {
    /// <summary>
    /// The mine which operations will be processed in
    /// </summary>
    protected IMine<T> Mine { get; }

    /// <summary>
    /// Construct a new GeneralResourceController
    /// </summary>
    /// <param name="mine">The mine which operations will be processed in</param>
    public GeneralResourceController(IMine<T> mine)
    {
      Mine = mine ?? throw new ArgumentNullException(nameof(mine));
    }

    private TShaft GetShaft<TShaft>() where TShaft : IShaft
    {
      if (!(Mine is TShaft castShaft))
        throw new InvalidCastException($"Mine is not a '{typeof(TShaft)}'");

      return castShaft;
    }

    /// <summary>
    /// If the mine for T contains a "Get Single" shaft then the sole T will be returned
    /// </summary>
    /// <returns>The only object in the set of T</returns>
    [HttpGet("[controller]/single")]
    public virtual Task<IActionResult> GetSingleAsync([FromQuery] string filter)
    {
      return new GetSingleHandler<T>(GetShaft<IGetSingleShaft<T>>()).GetSingleAsync(filter);
    }

    /// <summary>
    /// If the mine for T contains a "Get" shaft then a the T with the provided id will be returned
    /// </summary>
    /// <param name="identity">The identity of the T that's desired</param>
    /// <returns></returns>
    [HttpGet("[controller]/{identity}")]
    public virtual Task<IActionResult> GetAsync([FromRoute] TId identity)
    {
      return new GetHandler<TId, T>(GetShaft<IGetShaft<TId, T>>()).GetAsync(identity);
    }

    /// <summary>
    /// If the mine for T contains a "GetCollection" shaft then a the T's which match the filter will be returned
    /// </summary>
    /// <param name="filter">The filter to apply to the set of T</param>
    /// <param name="max">The maximum number of T's to return</param>
    /// <param name="page">The page of T's to return</param>
    /// <returns>A collection of T's</returns>
    [HttpGet("[controller]")]
    public virtual Task<IActionResult> GetCollectionAsync([FromQuery] string filter = null,
      [FromQuery] int? max = null, [FromQuery] int? page = null)
    {
      return new GetCollectionHandler<T>(GetShaft<IGetCollectionShaft<T>>()).GetCollectionAsync(filter, max, page);
    }

    /// <summary>
    /// If the mine for T contains a "Post" shaft then a post the new T
    /// </summary>
    /// <param name="obj">The new T to post</param>
    /// <param name="get">If true then the newly posted T should be returned</param>
    /// <returns>If get == true then the newly posted T, otherwise the value will be dependant on the mine shaft</returns>
    [HttpPost("[controller]")]
    public virtual async Task<IActionResult> PostAsync([FromBody] T obj, [FromQuery] bool get)
    {
      var postResult = await new PostHandler<T, TId>(GetShaft<IPostShaft<T, TId>>()).PostAsync(obj);

      if (!get || !(postResult is OkObjectResult castResult))
        return postResult;

      if (castResult.Value is JObject jobj)
      {
        var id = jobj["result"].Value<TId>();
        return await GetAsync(id);
      }

      var error = new JObject {{"error", "Did not receive expected result after posting record"}};
      return new InternalServerErrorObjectResult(error.ToString());
    }

    /// <summary>
    /// If the mine for T contains a "Put" shaft then put the provided T
    /// </summary>
    /// <param name="identity">The identity of the T to put</param>
    /// <param name="obj">The T to put</param>
    /// <param name="get">If true then the newly posted T should be returned</param>
    /// <returns>If get == true then the newly put T, otherwise the value will be dependant on the mine shaft</returns>
    [HttpPut("[controller]/{identity}")]
    public virtual async Task<IActionResult> PutAsync([FromRoute] TId identity, [FromBody] T obj, [FromQuery] bool get)
    {
      var putResult = await new PutHandler<TId, T, int>(GetShaft<IPutShaft<TId, T, int>>()).PutAsync(identity, obj);

      if (!get || !(putResult is OkObjectResult))
        return putResult;

      return await GetAsync(identity);
    }

    /// <summary>
    /// If the mine for T contains a "Patch" shaft then a patch the identified T
    /// </summary>
    /// <param name="identity">The identity of the T to patch</param>
    /// <param name="delta">A description of a modifications to a T</param>
    /// <param name="get">If true then the newly posted T should be returned</param>
    /// <returns>If get == true then the patched T, otherwise the value will be dependant on the mine shaft</returns>
    [HttpPatch("[controller]/{identity}")]
    public virtual async Task<IActionResult> PatchAsync([FromRoute] TId identity,
      [FromBody] IDictionary<string, object> delta, [FromQuery] bool get)
    {
      var deltaObj = new Delta<TId, T>(identity, delta);
      var patchResult = await new PatchHandler<TId, T, int>(GetShaft<IPatchShaft<TId, T, int>>()).PatchAsync(deltaObj);

      if (!get || !(patchResult is OkObjectResult))
        return patchResult;

      return await GetAsync(identity);
    }

    /// <summary>
    /// If the mine for T contains a "Delete" shaft then a delete the identified T
    /// </summary>
    /// <param name="id">The identity of the T to delete</param>
    /// <returns>A shaft depenant value</returns>
    [HttpDelete("[controller]/{id}")]
    public virtual Task<IActionResult> DeleteAsync([FromRoute] TId id)
    {
      return new DeleteHandler<TId, T, int>(GetShaft<IDeleteShaft<TId, int>>()).DeleteAsync(id);
    }

    /// <summary>
    /// If the mine for T contains a "Delete Collection" shaft then delete the T's which match the provided filter
    /// </summary>
    /// <param name="filter">The filter to apply to the set of T</param>
    /// <returns>A shaft dependant value</returns>
    [HttpDelete("[controller]")]
    public Task<IActionResult> DeleteCollectionAsync([FromQuery] string filter)
    {
      return new DeleteCollectionHandler<T, int>(GetShaft<IDeleteCollectionShaft<T, int>>())
        .DeleteCollectionAsync(filter);
    }
  }
}