using System.Collections.Generic;

namespace DataImporter.Web.Models.Import
{
    public class PreviewTaskModel
    {
        public int Id { get; set; }
        public string DisplayFileName { get; set; }
        public List<string> HeadersRow { get; set; }
        public List<List<string>> DataRows { get; set; }
        public int TotalRows { get; set; }
        public int TotalCells { get; set; }
    }
}