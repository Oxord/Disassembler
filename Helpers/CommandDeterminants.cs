using Disassembler.ArgumentBuilders;
using Disassembler.Structures;

namespace Disassembler.Helpers
{
    public static class CommandDeterminants
    {
        private const string LdiPrefixBinaryCode = "1110";
        private const string BrnePrefixBinaryCode = "111101";
        private const string JmpPrefixBinaryCode = "1001010";
        private const string EorPrefixBinaryCode = "001001";

        private const string LdiCommand = "ldi Rd,K";
        private const string BrneCommand = "brne k";
        private const string JmpCommand = "jmp k";
        private const string EorCommand = "eor Rd, R";
        private const string CallCommand = "call k";
        private const string CliCommand = "cli";

        private const string LdiMask = "1110KKKKddddKKKK";
        private const string BrneMask = "111101kkkkkkk001";
        private const string JmpMask = "1001010kkkkk110kkkkkkkkkkkkkkkkk";
        private const string EorMask = "001001rdddddrrrr";
        private const string CallMask = "1001010kkkkk111kkkkkkkkkkkkkkkkk";
        private const string CliMask = "1001010011111000";

        private static Dictionary<string, string> ReservedRegistersMap = new Dictionary<string, string>()
        {
            { "03", "PINB" },
            { "04", "DDRB" },
            { "05", "PORTB" },
            { "06", "PINC" },
            { "07", "DDRC" },
            { "08", "PORTC" },
            { "09", "PIND" },
            { "0A", "DDRD" },
            { "0B", "PORTD" },
            { "15", "TIFR0" },
            { "16", "TIFR1" },
            { "17", "TIFR2" },
            { "1B", "PCIFR" },
            { "1C", "EIFR" },
            { "1D", "EIMSK" },
            { "1E", "GPIOR0" },
            { "1F", "EECR" },
            { "20", "EEDR" },
            { "21", "EEARL" },
            { "22", "EEARH" },
            { "23", "GTCCR" },
            { "24", "TCCR0A" },
            { "25", "TCCR0B" },
            { "26", "TCNT0" },
            { "27", "OCR0A" },
            { "28", "OCR0B" },
            { "2A", "GPIOR1" },
            { "2B", "GPIOR1" },
            { "2C", "SPCR" },
            { "2D", "SPSR" },
            { "2E", "SPDR" },
            { "30", "ACSR" },
            { "33", "SMCR" },
            { "34", "MCUSR" },
            { "35", "MCUCR" },
            { "37", "SPMCSR" },
            { "3D", "SPL" },
            { "3E", "SPH" },
            { "3F", "SREG" }
        };

        private static readonly List<string> СommandsNeededInTwosComplement = new()
        {
            "rjmp", "breq", "brne",
            "brpl", "brmi", "brvc", "brvs", "brge", "brlt", "brhc", "brhs",
            "brtc", "brts", "brid", "brie", "brcc", "brcs", "brsh", "brlo",
            "rcall"
        };

        private static readonly List<string> СommandsForRegister = new()
        {
            "sbr", "cbr"
        };

        private static readonly List<string> CommandsForBothRegisters = new() {
            "eor",
            "add", "adc", "sub", "sbc", "and", "or", "mov", "cpse", "cp", "cpc", "movw",
            "com", "neg", "inc", "dec", "tst", "clr", "ser", "lsl", "lsr", "rol", "ror", "asr", "swap",
            "push", "pop"
        };
        private static readonly List<string> CommandsForHighRegister = new() 
        { 
            "subi", "sbci" 
        };
        private static readonly List<string> CommandsNeededInHexData = new() {
            "jmp", "call"
        };

        private static readonly List<string> CommandsForRegisterAsData = new() {
            "out"
        };

        private static readonly List<string> CommandsForReservedRegisters = new() {
            "sbi", "sbic", "cbi", "sbis"
        };

        private static readonly List<DetailedAsmCommand> _detailedAsmCommands = new()
        {
            { new DetailedAsmCommand(){ PrefixBinaryCode = LdiPrefixBinaryCode, Name = LdiCommand, Mask = LdiMask, Weight = 2 } },
            { new DetailedAsmCommand(){ PrefixBinaryCode = BrnePrefixBinaryCode, Name = BrneCommand, Mask = BrneMask, Weight = 2 } },
            { new DetailedAsmCommand(){ PrefixBinaryCode = JmpPrefixBinaryCode, Name = JmpCommand, Mask = JmpMask, Weight = 4 } },
            { new DetailedAsmCommand(){ PrefixBinaryCode = EorPrefixBinaryCode, Name = EorCommand, Mask = EorMask, Weight = 2 } },
            { new DetailedAsmCommand(){ PrefixBinaryCode = JmpPrefixBinaryCode, Name = CallCommand, Mask = CallMask, Weight = 4 } },
            { new DetailedAsmCommand(){ PrefixBinaryCode = JmpPrefixBinaryCode, Name = CliCommand, Mask = CliMask, Weight = 2 } },
        };

        public static bool IsCommandDeterminated( string binaryCommand )
        {
            return _detailedAsmCommands.Any( x => x.PrefixBinaryCode == binaryCommand );
        }

        public static DetailedAsmCommand GetByBinaryPrefix( string binaryCommand )
        {
            return _detailedAsmCommands.FirstOrDefault( x => x.PrefixBinaryCode == binaryCommand );
        }

        public static bool IsCommandCli( string binaryCommand )
        {
            return binaryCommand.StartsWith( JmpPrefixBinaryCode ) && binaryCommand.Length == 16;
        }

        public static DetailedAsmCommand GetCommandCli()
        {
            return new DetailedAsmCommand() { PrefixBinaryCode = JmpPrefixBinaryCode, Name = CliCommand, Mask = CliMask, Weight = 2 };
        }

        public static bool IsCommandCall( string binaryCommand )
        {
            return binaryCommand.StartsWith( JmpPrefixBinaryCode ) && binaryCommand[ 14 ] == '1';
        }

        public static DetailedAsmCommand GetCommandCall()
        {
            return new DetailedAsmCommand() { PrefixBinaryCode = JmpPrefixBinaryCode, Name = CallCommand, Mask = CallMask, Weight = 4 };
        }

        public static string GetReservedRegisterName( string hexReg )
        {
            if ( ReservedRegistersMap.ContainsKey( hexReg ) )
            {
                return ReservedRegistersMap[ hexReg ];
            }

            return hexReg;
        }

        public static string GetStrategyForCommand( string command )
        {
            if ( СommandsNeededInTwosComplement.Contains( command ) )
                return typeof( TwosComplementArgumentBuilder ).Name;

            else if ( СommandsForRegister.Contains( command ) )
                return typeof( RegisterArgumentBuilder ).Name;

            else if ( CommandsForBothRegisters.Contains( command ) )
                return typeof( BothRegistersArgumentBuilder ).Name;

            else if ( CommandsNeededInHexData.Contains( command ) )
                return typeof( HexDataArgumentBuilder ).Name;

            else if ( CommandsForRegisterAsData.Contains( command ) )
                return typeof( RegisterAsDataArgumentBuilder ).Name;

            else if ( CommandsForBothRegisters.Contains( command ) )
                return typeof( BothRegistersArgumentBuilder ).Name;

            else if ( CommandsForReservedRegisters.Contains( command ) )
                return typeof( ReservedRegisterArgumentBuilder ).Name;

            else if ( LdiCommand.StartsWith( command ) )
                return typeof( LdiArgumentBuilder ).Name;

            else if (CommandsForHighRegister.Contains(command))
                return typeof(HighRegisterArgumentBuilder).Name;

            else return typeof( DefaultArgumentBuilder ).Name;
        }
    }
}
