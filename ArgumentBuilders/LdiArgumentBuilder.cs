
namespace Disassembler.ArgumentBuilders
{
    public class LdiArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                int data = Convert.ToInt32( value, 2 );
                if ( Array.IndexOf( values, value ) == 0 )
                {
                    result.Add( data.ToString( "X2" ) );
                }
                else
                {
                    result.Insert( 0, $"r{data + 16}" );
                }
            }

            return result;
        }
    }
}
