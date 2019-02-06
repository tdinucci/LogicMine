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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LogicMine.Api.Web
{
  /// <summary>
  /// A base class for an API operation handler
  /// </summary>
  public class Handler
  {
    /// <summary>
    /// If this is set it will effectively override the default error result processor used by GetErrorResult()
    /// </summary>
    public Func<Exception, IActionResult> ErrorResultProcessor { get; set; }

    /// <summary>
    /// Construct a new Handler
    /// </summary>
    protected Handler()
    {
    }

    /// <summary>
    /// Covert an object into a standard result OkObjectResult
    /// </summary>
    /// <param name="result">The object to wrap in the OkObjectResult</param>
    /// <returns>An object wrapped in an OkObjectResult</returns>
    protected virtual OkObjectResult GetOkActionResult(object result)
    {
      return new OkObjectResult(new JObject {{"result", JToken.FromObject(result)}});
    }

    /// <summary>
    /// Convert an exception to an IActionResult in preperation for returning to the caller
    /// </summary>
    /// <param name="ex">The exception to convert</param>
    /// <returns>An IActionResult describing the exception</returns>
    protected virtual IActionResult GetErrorResult(Exception ex)
    {
      if (ErrorResultProcessor != null)
        return ErrorResultProcessor(ex);

      var errorObj = new JObject {{"errors", JToken.FromObject(GetExceptionMessages(ex))}};
      switch (ex)
      {
        case UnauthorizedAccessException _:
          return new UnauthorizedResult();
        case ValidationException _:
          return new BadRequestObjectResult(errorObj.ToString());
        default:
          return new InternalServerErrorObjectResult(errorObj.ToString());
      }
    }

    /// <summary>
    /// Get an exception message from an exception (including messages from inner exceptions)
    /// </summary>
    /// <param name="ex">The exception to extract the message from</param>
    /// <returns>A string array containing the exception message(s)</returns>
    protected virtual string[] GetExceptionMessages(Exception ex)
    {
      var level = 0;
      var messages = new List<string>();

      // caller won't care about this outer exception
      if (ex is ShaftException && ex.InnerException != null)
        ex = ex.InnerException;

      while (ex != null)
      {
        messages.Add($"{level++}: {ex.Message}.");

        //#if DEBUG
        //        message += $"===\n{ex.StackTrace}\n===\n";
        //#endif

        ex = ex.InnerException;
      }

      return messages.ToArray();
    }
  }
}
