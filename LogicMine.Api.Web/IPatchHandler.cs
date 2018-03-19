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
using LogicMine.Api.Patch;
using Microsoft.AspNetCore.Mvc;

namespace LogicMine.Api.Web
{
  /// <summary>
  /// A handler for "Patch" operations
  /// </summary>
  /// <typeparam name="TId">The identity type on T</typeparam>
  /// <typeparam name="T">The type which the handler deals with</typeparam>
  public interface IPatchHandler<TId, T>
  {
    /// <summary>
    /// Handle a "Patch" request
    /// </summary>
    /// <param name="delta">The difference between a T and the desired state of that T</param>
    /// <returns>A shaft dependant value</returns>
    Task<IActionResult> PatchAsync(Delta<TId, T> delta);
  }
}
