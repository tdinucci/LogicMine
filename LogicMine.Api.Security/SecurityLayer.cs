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
using LogicMine.Api.Delete;
using LogicMine.Api.DeleteCollection;
using LogicMine.Api.Get;
using LogicMine.Api.GetCollection;
using LogicMine.Api.GetSingle;
using LogicMine.Api.Patch;
using LogicMine.Api.Post;
using LogicMine.Api.Put;

namespace LogicMine.Api.Security
{
  /// <summary>
  /// <para>
  /// A very basic layer of security stations which may be placed in a mine.
  /// </para>
  /// <para>
  /// This mine simply allows or rejects operation requests.  A more complex security layer 
  /// may do things like blank out fields a user is not authorised to see, log access attempts, etc.
  /// </para>
  /// <para>
  /// The stations in this layer deal with the most general basket types to ensure it is compatible with the 
  /// widest range of shaft configurations, i.e. object, identity, return, etc. types are not specified on station 
  /// basket types which means any specialisation on the most primitive baskets are accepted
  /// </para>
  /// </summary>
  /// <typeparam name="T">The type the layer operates on</typeparam>
  /// <typeparam name="TUser">The type a user is represented as</typeparam>
  public abstract class SecurityLayer<T, TUser> : IStationLayer<T>,
    IGetStation<IGetBasket>,
    IGetSingleStation<IGetSingleBasket>,
    IGetCollectionStation<IGetCollectionBasket>,
    IPostStation<IPostBasket>,
    IPutStation<IPutBasket>,
    IPatchStation<IPatchBasket>,
    IDeleteStation<IDeleteBasket>,
    IDeleteCollectionStation<IDeleteCollectionBasket>
  {
    /// <summary>
    /// The user the encompassing mine is operating under
    /// </summary>
    protected TUser User { get; }

    /// <summary>
    /// Construct a new SecurityLayer
    /// </summary>
    /// <param name="user">The user the enclosing mine is operating under</param>
    protected SecurityLayer(TUser user)
    {
      if (user == null)
        throw new UnauthorizedAccessException();

      User = user;
    }

    /// <summary>
    /// Returns true if the user can perform the requested operation
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="operation">The requested operation</param>
    /// <returns></returns>
    protected abstract bool IsOperationAllowed(TUser user, Operations operation);

    /// <summary>
    /// If the user is not allowed to "Get" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetBasket basket)
    {
      EnsureOperationAllowed(Operations.Get);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Get" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetSingleBasket basket)
    {
      EnsureOperationAllowed(Operations.GetSingle);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetSingleBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Get" collections of T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IGetCollectionBasket basket)
    {
      EnsureOperationAllowed(Operations.GetCollection);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IGetCollectionBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Post" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPostBasket basket)
    {
      EnsureOperationAllowed(Operations.Post);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPostBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Put" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPutBasket basket)
    {
      EnsureOperationAllowed(Operations.Put);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPutBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Patch" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IPatchBasket basket)
    {
      EnsureOperationAllowed(Operations.Patch);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IPatchBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Delete" T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">The request</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IDeleteBasket basket)
    {
      EnsureOperationAllowed(Operations.Delete);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IDeleteBasket basket)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// If the user is not allowed to "Delete Collection's" of T's then an UnauthorizedAccessException will be thrown. 
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task DescendToAsync(IDeleteCollectionBasket basket)
    {
      EnsureOperationAllowed(Operations.DeleteCollection);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Performs no action
    /// </summary>
    /// <param name="basket">A basket</param>
    /// <returns>A Task that may be awaited</returns>
    public Task AscendFromAsync(IDeleteCollectionBasket basket)
    {
      return Task.CompletedTask;
    }

    private void EnsureOperationAllowed(Operations operation)
    {
      if (!IsOperationAllowed(User, operation))
        throw new UnauthorizedAccessException($"User is not allowed to perform '{operation}'");
    }
  }
}
