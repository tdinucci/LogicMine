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
using Microsoft.AspNetCore.Mvc;

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
    /// Convert an exception to an IActionResult in preperation for returning to the caller
    /// </summary>
    /// <param name="ex">The exception to convert</param>
    /// <returns>An IActionResult describing the exception</returns>
    protected virtual IActionResult GetErrorResult(Exception ex)
    {
      if (ErrorResultProcessor != null)
        return ErrorResultProcessor(ex);

      var message = GetExceptionMessage(ex);
      switch (ex)
      {
        case UnauthorizedAccessException _:
          return new UnauthorizedResult();
        case InvalidOperationException _:
          return new BadRequestObjectResult(message);
        default:
          return new InternalServerErrorObjectResult(message);
      }
    }

    /// <summary>
    /// Get an exception message from an exception (including messages from inner exceptions)
    /// </summary>
    /// <param name="ex">The exception to extract the message from</param>
    /// <returns>A string containing the exception message(s)</returns>
    protected virtual string GetExceptionMessage(Exception ex)
    {
      var level = 0;
      var message = string.Empty;
      while (ex != null)
      {
        message += $"{level++}: {ex.Message}.\r\n";

        //#if DEBUG
        //        message += $"===\n{ex.StackTrace}\n===\n";
        //#endif

        ex = ex.InnerException;
      }

      return message;
    }
  }
}
