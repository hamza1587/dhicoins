var datatable;
$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
    var i = 1;
    datatable = $('#CryptoReceiveAddress').DataTable({

        "ajax": {
            "url": "/Admin/GetCryptoReceiveAddress",
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

            
            { "data": "cryptoReceiverAddressDetails.chainName", "autoWidth": true },
            { "data": "commesionOnChain", "autoWidth": true },
           
            { "data": "receiveAddress", "autoWidth": true },
            {
                "data": "status",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    if (data === "Active") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--success">Active</span>';
                    } else if (data === "Inactive") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--danger">Inactive</span>';
                    } else {
                        return data;
                    }
                }
            },


            {
                "data": "id",
                "render": function (data) {
                    return `

                    <a href="/Admin/EditCryptoAddress?id=${data}"><i class="fas fa-edit"></i></i></a>
                    
                    <a href="javascript:void(0)" onclick="DeleteCryptoAddress('${data}')" title="Delete">
                    <i class="fas fa-trash"></i>
                    </a>
`
                },
                "width": "5%"
            }

        ]
    });
}

function DeleteCryptoAddress(id) {
    swal({
        title: "Are you sure?",
        text: "You are about to delete this Crypto Address. This action cannot be undone.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            // Perform the delete action
            $.ajax({
                url: "/Admin/DeleteCryptoAddress/" + id,
                method: "GET",
                dataType: "json",
                success: function (result) {
                    swal("Crypto Address deleted!", "The crypto address has been deleted.", "success");
                    datatable.ajax.reload();
                },
                error: function () {
                    swal("Error", "An error occurred while deleting the crypto address.", "error");
                }
            });
        }
    });
}

function View(id) {
    $.ajax({
        url: "/Admin/ViewCryptoChains/" + id,
        method: "GET",
        dataType: "json",
        success: function (result) {
            console.log(result);

            var nameList = $('#nameList');
            nameList.empty(); // clear previous list items

            $.each(result.data, function (index, value) {
                nameList.append($('<li>').text(value));
            });

            $('#AddNewUser').modal('show');
            $('#btnSave').text('Edit');
        },
        error: function () {
            alert("An error occurred while updating the User.");
        }
    });
}


//function View(id) {
//    $.ajax({
//        url: "/Admin/ViewCryptoChains/" + id,
//        method: "Get",
//        dataType: "json",
//        success: function (result) {
//            console.log(result);

            
//            $('#ViewChain').modal('show');
//            $("#btnSave").text('Edit');
//        },
//        error: function () {
//            alert("An error occurred while updating the User.");
//        }
//    })
//}









