namespace Disassembler.ArgumentBuilders
{
    public interface IArgumentBuilder
    {
        List<string> Build( string[] values );
    }
}
