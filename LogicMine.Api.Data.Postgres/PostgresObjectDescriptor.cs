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
using System.Collections.Generic;

namespace LogicMine.Api.Data.Postgres
{
    /// <summary>
    /// A type which contains metadata related to objects which are stored in a Postgres database.
    /// </summary>
    /// <typeparam name="T">The type described</typeparam>
    public class PostgresObjectDescriptor<T> : DbObjectDescriptor<T>
    {
        /// <summary>
        /// Construct a new PostgresObjectDescriptor
        /// </summary>
        public PostgresObjectDescriptor() : base()
        {
        }

        /// <summary>
        /// Construct a new PostgresObjectDescriptor
        /// </summary>
        /// <param name="readOnlyPropertyNames">A collection of property names on T which should not be written to the database</param>
        public PostgresObjectDescriptor(IEnumerable<string> readOnlyPropertyNames) : base(readOnlyPropertyNames)
        {
        }
    }
}