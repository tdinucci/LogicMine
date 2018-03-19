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
using LogicMine.Api.Filter;

namespace LogicMine.Api.Web.Filter
{
  /// <summary>
  /// <para>
  /// Parses string filter expressions and produces instances of IFilter which is bound to T.
  /// </para>
  /// <para>Example filter expressions are:</para>
  /// <para>MyStringField in ('one', 'two', 'three')</para>
  /// <para>MyStringField in ('one', 'two', 'three') and MyIntField from 1 to 10 and MyDateField lt '2018-06-25'</para>
  /// </summary>
  public interface IFilterParser
  {
    /// <summary>
    /// Generates an IFilter from an expression
    /// </summary>
    /// <returns>An IFilter</returns>
    IFilter Parse();
  }
}
