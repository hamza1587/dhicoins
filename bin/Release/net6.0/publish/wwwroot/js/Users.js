var datatable;
$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
    var i = 1;
    datatable = $('#UserList').DataTable({

        "ajax": {
            "url": "/Admin/GetUserMaster",
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

            { "data": "userType", "autoWidth": true },
            { "data": "userName", "autoWidth": true },
            { "data": "email", "autoWidth": true },
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
                    <a  href="javascript:void(0)" onclick="Edit('${data}')" title="Edit" >
                          <i class="fas fa-edit"></i></a>
                    <a href="javascript:void(0)" onclick="DeleteUser('${data}')" title="Delete">
                    <i class="fas fa-trash"></i>
                    </a>
                    <a href="javascript:void(0)" onclick="ResetPassword('${data}')" title="Reset Password">
                      <i class="fas fa-unlock-alt"></i>
                       </a>
`
                },
                "width": "5%"
            }

        ]
    });
}

$("#btnOpen").click(function () {
    $("#AddNewUser").modal("show");
    $("#pass").show();
    $("#exampleModalLabel").text('Add New User');
    $("#btnSave").text('Add New');
})

$("#btnSave").click(function () {
    $.ajax({
        url: "/Admin/UpsertUser",
        method: "Post",
        data: $('#form').serialize(),
        datatype: "json",
        success: function (result) {
            datatable.ajax.reload();
            $("#AddNewUser").modal("hide");
            $('#form')[0].reset();


        },
        error: function () {
            alert("An error occurred while saving the User.");
        }
    })
})

function Edit(id) {
    $.ajax({
        url: "/Admin/EditUser/" + id,
        method: "Get",
        dataType: "json",
        success: function (result) {
            console.log(result);
            $("#exampleModalLabel").text('Edit User');

            $('[name="UserName"]').val(result.obj.userName);
            $('[name="Email"]').val(result.obj.email);
            $('[name="UserType"]').val(result.obj.userType);
            $('[name="Status"]').val(`${result.obj.status}`);
            $('[name="Id"]').val(result.obj.id);
            $("#pass").hide();
            $('#AddNewUser').modal('show');
            $("#btnSave").text('Edit');
        },
        error: function () {
            alert("An error occurred while updating the User.");
        }
    })
}
function ResetPassword(id) {

    swal({
        title: "Are you sure?",
        text: "You are about to reset this user's password. This action cannot be undone.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            // Perform the reset link sent action
            $.ajax({
                url: "/Admin/ResetUserPassword/" + id,
                method: "GET",
                dataType: "json",
                success: function (result) {
                    swal("Reset Link Sent!", "Check your email to reset the password .", "success");
                  
                },
                error: function () {
                    swal("Error", "An error occurred while sending email to the user.", "error");
                }
            });
        }
    });


}


function DeleteUser(id) {
    swal({
        title: "Are you sure?",
        text: "You are about to delete this user. This action cannot be undone.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            // Perform the delete action
            $.ajax({
                url: "/Admin/DeleteUser/" + id,
                method: "GET",
                dataType: "json",
                success: function (result) {
                    swal("User deleted!", "The user has been deleted.", "success");
                    datatable.ajax.reload();
                },
                error: function () {
                    swal("Error", "An error occurred while deleting the user.", "error");
                }
            });
        }
    });
}











