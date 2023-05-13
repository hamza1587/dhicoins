var datatable;
$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
    var i = 1;
    datatable = $('#Customer_Approval').DataTable({
        "ajax": {
            "url": "/Admin/GetCustomerApproval",
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
            { "data": "firstName", "autoWidth": true },
            { "data": "email", "autoWidth": true },
            {
                "data": "creationStatus",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    if (data === "Pending") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--danger">Pending</span>';
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Admin/CustomerDetail?id=${data}"><i class="fas fa-eye"></i></a>`;
                },
                "width": "5%"
            }

        ]
    });
}



















