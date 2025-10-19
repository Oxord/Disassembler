using Disassembler.ArgumentBuilders;
using Disassembler.Extensions;
using Disassembler.Structures;

namespace Disassembler.Helpers
{
    public static class HexCodeParser
    {
        private const int TwoByteCommandWeight = 2;
        private const int FourByteCommandWeight = 4;
        private const int DefaultCommandLength = 4;
        private const int SpecialCommandLength = 8;
        private const int OnePartCommand = 2;
        private const int JmpCommandOpcodeLength = 4;
        private static readonly List<string> JmpCommandOpcodes = new()
        {
            "0C94",
            "0E94"
        };
        private static readonly List<string> FourBytesCommands = new()
        {
            "jmp",
            "call"
        };

        private static Dictionary<string, IArgumentBuilder> _builders = new Dictionary<string, IArgumentBuilder>()
        {
            { typeof( TwosComplementArgumentBuilder ).Name, new TwosComplementArgumentBuilder() },
            { typeof( RegisterArgumentBuilder ).Name, new RegisterArgumentBuilder() },
            { typeof( Both16RegistersArgumentBuilder ).Name, new Both16RegistersArgumentBuilder() },
            { typeof( BothRegistersArgumentBuilder ).Name, new BothRegistersArgumentBuilder() },
            { typeof( HexDataArgumentBuilder ).Name, new HexDataArgumentBuilder() },
            { typeof( RegisterAsDataArgumentBuilder ).Name, new RegisterAsDataArgumentBuilder() },
            { typeof( ReservedRegisterArgumentBuilder ).Name, new ReservedRegisterArgumentBuilder() },
            { typeof( LdiArgumentBuilder ).Name, new LdiArgumentBuilder() },
            { typeof( HighRegisterArgumentBuilder ).Name, new HighRegisterArgumentBuilder() },
            { typeof( DefaultArgumentBuilder ).Name, new DefaultArgumentBuilder() }
        };

        public static List<string> GetCommandsBits( string hexLine )
        {
            if ( string.IsNullOrWhiteSpace( hexLine ) || hexLine.Length < 11 )
            {
                return new List<string>();
            }

            var trimmedStr = hexLine.Trim();
            string dataHex = trimmedStr.Substring( 9, trimmedStr.Length - 9 - 2 );

            var result = new List<string>();
            int i = 0;

            while ( i < dataHex.Length )
            {
                if ( dataHex.Length - i >= SpecialCommandLength &&
                    JmpCommandOpcodes.Contains( dataHex.Substring( i, JmpCommandOpcodeLength ) ) )
                {
                    result.Add( dataHex.Substring( i, SpecialCommandLength ) );
                    i += SpecialCommandLength;
                }
                else
                {
                    if ( dataHex.Length - i < DefaultCommandLength )
                    {
                        break;
                    }
                    result.Add( dataHex.Substring( i, DefaultCommandLength ) );
                    i += DefaultCommandLength;
                }
            }

            return result;
        }

        public static DetailedAsmCommand GetAsmCommand( string hexCommand )
        {
            DetailedAsmCommand result = new();

            string commandName;
            string mask;

            var swappedBytesStr = hexCommand.Swap();
            var commandBytes = swappedBytesStr.Chunk( OnePartCommand )
                             .Select( subString => new string( subString ) )
                             .ToList();

            string binaryCommand = string.Empty;

            for ( int i = 0; i < commandBytes.Count; i++ )
            {
                int decimalValue = Convert.ToInt32( commandBytes[ i ], 16 );
                string binaryValue = $"{decimalValue:B8}";

                binaryCommand += binaryValue;
            }

            //ищем команду 
            int response = -1;
            for ( int i = 4; i < binaryCommand.Length && response == -1; i++ )
            {
                string separatedCommand = binaryCommand.Substring( 0, i );

                if ( CommandDeterminants.IsCommandDeterminated( separatedCommand ) )
                {
                    DetailedAsmCommand asmCommand = CommandDeterminants.IsCommandCli( binaryCommand )
                        ? CommandDeterminants.GetCommandCli()
                        : CommandDeterminants.IsCommandCall( binaryCommand )
                            ? CommandDeterminants.GetCommandCall()
                            : CommandDeterminants.GetByBinaryPrefix( separatedCommand );
                    asmCommand.Data = BuildCommandData( asmCommand.Mask, binaryCommand, asmCommand.Name );
                    return asmCommand;
                }
                else
                {
                    var x = ExcelDataReader.FindInExcelColumn( separatedCommand );

                    if ( x.Count == 1 )
                    {
                        response = x.FirstOrDefault();
                    }
                }
            }

            mask = ExcelDataReader.ExcelLookup( response, 4 ).Replace( " ", string.Empty );
            commandName = ExcelDataReader.ExcelLookup( response, 0 );

            result.Name = commandName;
            result.Mask = mask;
            result.Weight = FourBytesCommands.Contains( commandName ) ? FourByteCommandWeight : TwoByteCommandWeight;
            result.Data = BuildCommandData( mask, binaryCommand, commandName );

            return result;

        }

        private static List<string> BuildCommandData( string mask, string command, string commandName )
        {
            try
            {
                Dictionary<char, string> parameters = new();

                for ( int i = 0; i < mask.Length; i++ )
                {
                    if ( mask[ i ] != '1' && mask[ i ] != '0' )
                    {
                        if ( parameters.ContainsKey( mask[ i ] ) )
                        {
                            parameters[ mask[ i ] ] = parameters[ mask[ i ] ] + command[ i ];
                        }
                        else
                        {
                            parameters.Add( mask[ i ], command[ i ].ToString() );
                        }
                    }
                }

                var values = parameters.Values.ToArray();
                string cleanedCommand = commandName.Split( " " ).First();

                var strategyName = CommandDeterminants.GetStrategyForCommand( cleanedCommand );
                var builder = _builders[ strategyName ];

                return builder.Build( values );
            }
            catch ( Exception ex )
            {
                throw ex;
            }
        }
    }
}
