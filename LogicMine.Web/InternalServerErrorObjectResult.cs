using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace LogicMine.Web
{
    /// <summary>
    /// Represents an HTTP 500 Internal Server Error
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// Construct a new InternalServerErrorObjectResult
        /// </summary>
        public InternalServerErrorObjectResult() : base(null)
        {
            StatusCode = (int) HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Construct a new InternalServerErrorObjectResult
        /// </summary>
        /// <param name="result">The result</param>
        public InternalServerErrorObjectResult(object result) : base(result)
        {
            StatusCode = (int) HttpStatusCode.InternalServerError;
        }
    }
}