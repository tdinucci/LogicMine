﻿/*
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
namespace LogicMine
{
  /// <summary>
  /// <para>A marker interface for layer of stations.</para>
  /// <para>
  /// A layer should be considered a horizontal slice through a mine which contains a station for each shaft.
  /// </para>
  /// <para>
  /// Layers are organisational, rather than operational.  This is to say, the don't actually do anything 
  /// other than help to better organise collections of shafts.  As an example imagine a layer which offers caching 
  /// facilities, this layer may contain Get, Post, Delete, etc. stations which use and maintain the cache.
  /// </para>
  /// </summary>
  /// <typeparam name="T">The type that this layer applies to</typeparam>
  public interface IStationLayer<T> 
  {
  }
}
