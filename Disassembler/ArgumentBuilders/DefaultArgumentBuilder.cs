namespace Disassembler.ArgumentBuilders
{
    public class DefaultArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                result.Add( Convert.ToInt32( value, 2 ).ToString() );
            }

            return result;
        }
    }
}
