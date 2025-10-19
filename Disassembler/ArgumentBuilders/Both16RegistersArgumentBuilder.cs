namespace Disassembler.ArgumentBuilders
{
    public class Both16RegistersArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                result.Add( "r" + ( Convert.ToInt32( value, 2 ) ).ToString() );
            }

            return result;
        }
    }
}
