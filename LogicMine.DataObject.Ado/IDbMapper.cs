using System.Data;

namespace LogicMine.DataObject.Ado
{
    /// <summary>
    /// A basic object-relational mapper
    /// </summary>
    /// <typeparam name="T">The type which the mapper operates on</typeparam>
    public interface IDbMapper<T>
    {
        /// <summary>
        /// Converts an IDataRecord to a T
        /// </summary>
        /// <param name="record"></param>
        /// <param name="selectedFields">The fields in the result set.  If null/empty then all record fields are assumed</param>
        /// <returns></returns>
        T MapObject(IDataRecord record, string[] selectedFields = null);

        /// <summary>
        /// Converts all records available in an IDataReader to a collection of T's
        /// </summary>
        /// <param name="reader">A datase reader</param>
        /// <param name="selectedFields">The fields in the result set.  If null/empty then all record fields are assumed</param>
        /// <returns>A collection of T's</returns>
        T[] MapObjects(IDataReader reader, string[] selectedFields = null);
    }
}