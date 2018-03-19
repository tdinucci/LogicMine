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
  /// <para>Parses string terms and produces instances of IFilterTerm.</para>
  /// <para>Example string terms are:</para>
  /// 
  /// <para>MyStringField eq 'hello'</para>
  /// <para>MyIntField lt 100</para>
  /// <para>MyIntField in (1, 2, 3, 4)</para>
  /// <para>MyDateField from '2017-01-01' to '2017-12-31'</para>
  /// </summary>
  public interface IFilterTermParser
  {
    /// <summary>
    /// Parses a filter term expression
    /// </summary>
    /// <returns>An IFilterTerm</returns>
    IFilterTerm Parse();
  }
}
