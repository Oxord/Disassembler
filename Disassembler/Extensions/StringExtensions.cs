using System.Text;

namespace Disassembler.Extensions
{
    public static class StringExtensions
    {
        public static string Swap( this string hexString )
        {
            if ( string.IsNullOrEmpty( hexString ) )
            {
                return string.Empty;
            }
            if ( hexString.Length % 4 != 0 )
            {
                throw new ArgumentException( "Длина строки должна быть кратна 4.", nameof( hexString ) );
            }

            var resultBuilder = new StringBuilder( hexString.Length );

            for ( int i = 0; i < hexString.Length; i += 4 )
            {
                string word = hexString.Substring( i, 4 );

                string byte1 = word.Substring( 0, 2 );
                string byte2 = word.Substring( 2, 2 );

                resultBuilder.Append( byte2 );
                resultBuilder.Append( byte1 );
            }

            return resultBuilder.ToString();
        }

        public static bool IsBinanryValueNegative( this string binaryCommand )
        {
            return binaryCommand[ 0 ] == '1';
        }

        public static string GetTwosComplementForNegativeNum( this string str )
        {
            return str.InverseBinaryString().AddBinaryString( "1" );
        }

        private static string InverseBinaryString( this string str )
        {
            string invertedValue = string.Empty;
            for ( int i = 0; i < str.Length; i++ )
            {
                invertedValue += str[ i ] == '1' ? '0' : '1';
            }

            return invertedValue;
        }

        private static string AddBinaryString( this string a, string b )
        {
            StringBuilder result = new StringBuilder();
            int i = a.Length - 1;
            int j = b.Length - 1;
            int carry = 0;

            while ( i >= 0 || j >= 0 || carry == 1 )
            {
                int sum = carry;

                if ( i >= 0 )
                {
                    sum += a[ i ] - '0';
                    i--;
                }

                if ( j >= 0 )
                {
                    sum += b[ j ] - '0';
                    j--;
                }

                result.Insert( 0, sum % 2 );
                carry = sum / 2;
            }

            return result.ToString();
        }
    }
}
