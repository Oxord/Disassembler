
namespace Disassembler.ArgumentBuilders
{
    public class HexDataArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                result.Add( ( Convert.ToInt32( value, 2 ) * 2 ).ToString( "X2" ) );
            }

            return result;
        }
    }
}
