using System;
using System.Collections.Generic;
using System.Linq;
using DataImporter.Core.Entities;

namespace DataImporter.Core.Seeds
{
    public static class DataSeed
    {
        public static List<Group> Groups()
        {
            var rnd = new Random();
            return Enumerable.Range(1, 15).Select(id => new Group
            {
                Id = id,
                Name = "Group " + id.ToString(),
                ApplicationUserId = new Guid("00000000-0000-0000-0000-0000000000a1")
            }).ToList();
        }

        public static List<Header> Headers()
        {
            var l1 = new List<string> {"Name", "Age", "Email", "Salary"};
            return Enumerable.Range(1, 4).Select(id => new Header
            {
                Id = id,
                Name = l1.ElementAt(id - 1),
                GroupId = 1,
                Position = id
            }).ToList();
        }

        public static List<Row> Rows()
        {
            return Enumerable.Range(1, 2).Select(id => new Row
            {
                Id = id,
                GroupId = 1,
                CreatedAt = DateTime.Now
            }).ToList();
        }

        public static List<Cell> Cells()
        {
            var l = new string[2, 4] {{"One", "10", "one@mail.com", "200.11"}, {"Two", "20", "two@mail.com", "200.20"}};
            var cells = new List<Cell>();
            var id = 1;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    cells.Add(new Cell()
                    {
                        Id = id++,
                        RowId = i + 1,
                        HeaderId = j + 1,
                        Value = l[i, j],
                        Position = j + 1
                    });
                }
            }

            return cells;
        }

        public static List<Import> Imports()
        {
            return Enumerable.Range(1, 15).Select(id => new Import
            {
                Id = id,
                GroupId = 1,
                DisplayFileName = $"File-{id}.xlsx",
                StorageFileName = $"{Guid.NewGuid()}.xlsx",
                Status = "Done",
                CreatedAt = DateTime.Now
            }).ToList();
        }

        public static List<Export> Exports()
        {
            return Enumerable.Range(1, 15).Select(id =>
            {
                var dt = DateTime.Now;
                return new Export
                {
                    Id = id,
                    Email = "email" + id + "@mail.com",
                    EmailStatus = "Done",
                    Status = "Done",
                    DisplayFileName = $"File-{id}.xlsx",
                    StorageFileName = $"{Guid.NewGuid()}.xlsx",
                    CreatedAt = dt,
                    ApplicationUserId = new Guid("00000000-0000-0000-0000-0000000000a1")
                };
            }).ToList();
        }

        public static List<ExportGroup> ExportGroups()
        {
            var rnd = new Random();
            return Enumerable.Range(1, 15).Select(id => new ExportGroup
            {
                Id = id,
                GroupId = id,
                ExportId = id
            }).ToList();
        }
    }
}