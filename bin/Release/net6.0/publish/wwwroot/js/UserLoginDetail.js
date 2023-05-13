var datatable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    var i = 1;
    datatable = $('#LoginTbl').DataTable({

        "ajax": {
            "url": "/Admin/GetUserLoginDetail",
            "type": "GET",
            "datatype": "json",
            "cache": false
        },
        "columns": [
            {
                "render": function (data, type, full, meta) {
                    return i++;
                }
            },
            {
                "data": "isOnline",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    if (data === "Logged In") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--success">Logged In</span>';
                    } else if (data === "Logged out") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--danger">Logged out</span>';
                    } else {
                        return data;
                    }
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return` 
                    <input type="checkbox" class="tgl_checkbox tgl-ios"  data-id="${full.userId}" data-checked="${full.isOnline}"/>
                       `
                     
                }
            },
            { "data": "fullName", "autoWidth": true },
            { "data": "email", "autoWidth": true },
            { "data": "ipAddress", "autoWidth": true },
            { "data": "browserName", "autoWidth": true },
            { "data": "country", "autoWidth": true },
            { "data": "date", "autoWidth": true },
        ]
    });

    $(document).on('change', '.tgl_checkbox', function () {
        var userId = $(this).data('id');
        var isChecked = $(this).is(':checked');

        $.ajax({
            url: '/Admin/ForceLogout',
            type: 'POST',
            data: { userId: userId, isChecked: isChecked },
            success: function (result) {
                console.log();
                // Reload the data table to update the status of the user
                datatable.ajax.reload();
            },
            
            error: function () {
                // Display an error message
                alert("An error occured");
            }
        });
    });
}











  