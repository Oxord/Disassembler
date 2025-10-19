using Disassembler.Helpers;
using Disassembler.Structures;

namespace Disassembler
{
    public class Program
    {
        private const string InputPath = "in.txt";
        private static readonly string OutputPath = Path.Combine( Directory.GetParent( AppContext.BaseDirectory ).Parent.Parent.Parent.FullName, "result.txt" );

        static void Main( string[] args )
        {
            File.WriteAllText( OutputPath, String.Empty );
            string[] hexCodeLines = File.ReadAllLines( InputPath );
            int address = 0;

            try
            {
                var commandsBits = new List<string>();
                foreach ( string hexCodeLine in hexCodeLines )
                {
                    commandsBits.AddRange( HexCodeParser.GetCommandsBits( hexCodeLine ) );
                }

                var asmCommands = new List<DetailedAsmCommand>();
                foreach ( string command in commandsBits )
                {
                    asmCommands.Add( HexCodeParser.GetAsmCommand( command ) );
                }

                foreach ( var asmCommand in asmCommands )
                {
                    File.AppendAllText( OutputPath, $"{address.ToString( "X2" )}: {asmCommand.ToString()}" );
                    address += asmCommand.Weight;
                }
            }
            catch ( Exception ex )
            {
                File.WriteAllText( OutputPath, ex.Message );
            }
        }
    }
}
