namespace LogicMine.DataObject.Filter.Parse
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