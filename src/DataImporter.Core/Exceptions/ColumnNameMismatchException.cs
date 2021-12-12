using System;

namespace DataImporter.Core.Exceptions
{
    public class ColumnNameMismatchException : Exception
    {
        public ColumnNameMismatchException(string message)
            : base(message) { }
    }
}
