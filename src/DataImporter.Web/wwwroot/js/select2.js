$("#groupSelect").select2({
    placeholder: "Select a Group",
    //theme: "bootstrap4",
    allowClear: true,
    ajax: {
        url: "/Group/GetAll",
        contentType: "application/json; charset=utf-8",
        data: function (params) {
            var query =
            {
                term: params.term,
            };
            return query;
        },
        processResults: function (result) {
            return {
                results: $.map(result, function (item) {
                    return {
                        id: item.id,
                        text: item.name
                    };
                }),
            };
        }
    }
});