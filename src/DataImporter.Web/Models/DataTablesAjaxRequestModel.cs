using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImporter.Web.Models
{
    public class DataTablesAjaxRequestModel
    {
        private readonly HttpRequest _request;
        private readonly IEnumerable<KeyValuePair<string, StringValues>> _requestValues;

        public DataTablesAjaxRequestModel(HttpRequest request)
        {
            _request = request;
            var method = _request.Method.ToLower();
            if (method == "get")
                _requestValues = _request.Query;
            else if (method == "post")
                _requestValues = _request.Form;
            else
                throw new InvalidOperationException("Http method not supported, use get or post");
        }

        public int Length => GetRequestValue<int>("length");
        public string SearchText => GetRequestValue("search[value]");
        public int PageIndex => Length > 0 ? (Start / Length) + 1 : 1;
        public int PageSize => Length <= 0 ? 10 : Length;
        public DateTime StartDate => TryParseDateTime("fStartDate");
        public DateTime EndDate => TryParseDateTime("fEndDate");

        public static object EmptyResult => new
        {
            recordsTotal = 0,
            recordsFiltered = 0,
            data = (Array.Empty<string>()).ToArray()
        };

        public string GetRequestValue(string key) => GetRequestValue<string>(key) ?? string.Empty;
        public T GetRequestValue<T>(string key) where T : IConvertible
        {
            var value = _requestValues.FirstOrDefault(x => x.Key == key).Value.ToArray()[0];
            if (string.IsNullOrEmpty(value))
                return default;
            return (T)Convert.ChangeType(value, typeof(T));
        }
        public string GetSortText(List<string> columnNames)
        {
            var sortText = new StringBuilder();
            for (var i = 0; i < columnNames.Count; i++)
            {
                if (!_requestValues.Any(x => x.Key == $"order[{i}][column]")) continue;
                if (sortText.Length > 0)
                    sortText.Append(",");

                var (columnName, direction) = GetNameAndDirection(columnNames.ToList(), i);
                var sortDirection = $"{columnName} {direction}";
                sortText.Append(sortDirection);
            }
            return sortText.ToString();
        }
        public (string sortField, string sortOrder) GetSortFieldAndOrder(List<string> columns)
        {
            for (var i = 0; i < columns.Count; i++)
            {
                if (_requestValues.Any(x => x.Key == $"order[{i}][column]"))
                {
                    return GetNameAndDirection(columns, i);
                }
            }
            return default;
        }

        private int Start => GetRequestValue<int>("start");

        private DateTime TryParseDateTime(string key)
        {
            var dateString = GetRequestValue(key);
            if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out var date))
            {
                return date;
            }
            return default;
        }
        private (string columnName, string direction) GetNameAndDirection(List<string> columns, int i)
        {
            var column = GetRequestValue<int>($"order[{i}][column]");
            var direction = GetRequestValue($"order[{i}][dir]");
            var columnName = $"{columns.ElementAt(column)}";
            direction = $"{(direction == "asc" ? "asc" : "desc")}";
            return (columnName, direction);
        }
    }
}