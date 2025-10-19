using Disassembler.Extensions;

namespace Disassembler.ArgumentBuilders
{
    public class TwosComplementArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                string normalizedValue = value.TrimStart( '0' );
                int decimalValue;
                char sign = '+';

                if ( value.IsBinanryValueNegative() )
                {
                    sign = '-';
                    decimalValue = 2 * Convert.ToInt32( normalizedValue.GetTwosComplementForNegativeNum(), 2 );
                }
                else
                {
                    decimalValue = 2 * Convert.ToInt32( normalizedValue, 2 );
                }

                result.Add( $"{sign}{decimalValue.ToString()}" );
            }

            return result;
        }
    }
}
