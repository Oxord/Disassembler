using System.Data;
using ExcelDataReader;

namespace Disassembler.Helpers
{
    public static class ExcelDataReader
    {
        private static readonly string _filePath = Path.Combine( Directory.GetParent( AppContext.BaseDirectory ).Parent.Parent.Parent.FullName, "ASM_AVR_Codes.xlsx" );
        private const string _sheetName = "MainData";

        static ExcelDataReader()
        {
            System.Text.Encoding.RegisterProvider( System.Text.CodePagesEncodingProvider.Instance );

            if ( !File.Exists( _filePath ) )
            {
                throw new ArgumentNullException( $"Ошибка: Файл не найден по пути {_filePath}" );
            }
        }

        public static List<int> FindInExcelColumn( string searchText, int columnIndex = 4 )
        {
            var foundRows = new List<int>();

            try
            {
                using ( var stream = File.Open( _filePath, FileMode.Open, FileAccess.Read ) )
                {
                    using ( var reader = ExcelReaderFactory.CreateReader( stream ) )
                    {
                        var result = reader.AsDataSet( new ExcelDataSetConfiguration() );

                        if ( !result.Tables.Contains( _sheetName ) )
                        {
                            Console.WriteLine( $"Ошибка: Лист с именем '{_sheetName}' не найден." );
                            return foundRows;
                        }

                        var dataTable = result.Tables[ _sheetName ];

                        if ( columnIndex < 0 || columnIndex >= dataTable.Columns.Count )
                        {
                            Console.WriteLine( $"Ошибка: Неверный индекс столбца {columnIndex}. В таблице всего {dataTable.Columns.Count} столбцов." );
                            return foundRows;
                        }

                        for ( int i = 0; i < dataTable.Rows.Count; i++ )
                        {
                            var cellValue = dataTable.Rows[ i ][ columnIndex ]?.ToString().Replace( " ", "" );

                            if ( !string.IsNullOrEmpty( cellValue ) && cellValue.StartsWith( searchText, StringComparison.OrdinalIgnoreCase ) )
                            {
                                foundRows.Add( i );
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Произошла ошибка при чтении файла: {ex.Message}" );
            }

            return foundRows;
        }

        public static string ExcelLookup( int returnRowIndex, int searchColumnIndex = 4 )
        {
            try
            {
                using ( var stream = File.Open( _filePath, FileMode.Open, FileAccess.Read ) )
                {
                    using ( var reader = ExcelReaderFactory.CreateReader( stream ) )
                    {
                        var result = reader.AsDataSet( new ExcelDataSetConfiguration() );

                        if ( !result.Tables.Contains( _sheetName ) )
                        {
                            Console.WriteLine( $"Ошибка: Лист с именем '{_sheetName}' не найден." );
                            return null;
                        }

                        var dataTable = result.Tables[ _sheetName ];

                        return dataTable.Rows[ returnRowIndex ][ searchColumnIndex ]?.ToString();
                    }
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Произошла ошибка при чтении файла: {ex.Message}" );
            }

            return null;
        }
    }
}
