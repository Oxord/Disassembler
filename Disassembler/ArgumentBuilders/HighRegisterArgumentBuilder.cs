using Disassembler.Helpers;

namespace Disassembler.ArgumentBuilders
{
    public class HighRegisterArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                if ( Array.IndexOf( values, value ) == 0 )
                {
                    string hexReg = ( Convert.ToInt32( value, 2 ) ).ToString( "X2" );
                    result.Add( CommandDeterminants.GetReservedRegisterName( hexReg ) );
                }
                else
                {
                    result.Insert( 0, "r" + ( Convert.ToInt32( value, 2 ) + 16 ).ToString() );
                }
            }

            return result;
        }
    }
}
