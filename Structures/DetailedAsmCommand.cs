using System.Text;

namespace Disassembler.Structures
{
    public struct DetailedAsmCommand
    {
        public string PrefixBinaryCode;
        public string Name;
        public string Mask;
        public int Weight;
        public List<string> Data;

        public override string ToString()
        {
            var sb = new StringBuilder();
            string commandWithData = BuildCommandWithData();
            sb.AppendLine( $"{commandWithData}" );

            return sb.ToString();
        }

        public string BuildCommandWithData()
        {
            string res = Name.Split( " " ).First();

            if ( Data.Count > 0 )
            {
                res += " ";
                for ( int i = 0; i < Data.Count; i++ )
                {
                    res = res + Data[ i ];
                    if ( i < Data.Count - 1)
                    {
                        res += ",";
                    }
                }

            }
            res += ";";

            return res;
        }
    }
}
