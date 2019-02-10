namespace LogicMine.DataObject.Filter.Parse
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