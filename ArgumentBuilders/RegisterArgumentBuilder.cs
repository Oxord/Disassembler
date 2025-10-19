
namespace Disassembler.ArgumentBuilders
{
    public class RegisterArgumentBuilder : IArgumentBuilder
    {
        public List<string> Build( string[] values )
        {
            var result = new List<string>();
            foreach ( var value in values )
            {
                int data = Convert.ToInt32( value, 2 );
                string resultedDataStr = Array.IndexOf( values, value ) == 0
                    ? $"r{data + 16}"
                    : data.ToString();
                result.Add( resultedDataStr );
            }

            return result;
        }
    }
}
