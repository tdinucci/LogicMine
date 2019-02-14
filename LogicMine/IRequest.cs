using System;
using System.Collections.Generic;

namespace LogicMine
{   
    /// <summary>
    /// Represents a request that a caller is making, e.g. to get some data object or to perform an operation
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// The unique id of the request, this is managed by LogicMine and can be used to tie up
        /// requests with responses in logs.  For example if logs aren't being actively monitored for errors
        /// a user may report that there was an error with request "1dfae19c-a6a1-4794-a695-aee43e596b04" and then
        /// this can be found in the log quickly 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// While one request is being processed another may fork off for some reason.  By setting the ParentId to
        /// the Id of the parent request it is possible to tie these requests together in recorded traces.
        /// </summary>
        Guid? ParentId { get; set; }
        
        /// <summary>
        /// May contain any supplemental data which you want to travel with the request, e.g. security token,
        /// calling system, etc.  Any objects which the request passes through may make use of this however they
        /// see fit.
        /// </summary>
        IDictionary<string, object> Options { get; }
    }
}