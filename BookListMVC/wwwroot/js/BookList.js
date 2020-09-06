var dataTable;

$(document).ready(() => {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/books/getall/",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name", "width": "30%" },
            { "data": "author", "width": "30%" },
            { "data": "isbn", "width": "30%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Books/Upsert?id=${data}" class="btn btn-success text-white" style="cursor: pointer; width:100px;">
                            Edit
                        </a>
                        <a class="btn btn-danger text-white" style="cursor: pointer; width:100px;"
                            onclick="Delete('/books/Delete?id=' + ${data})" >
                            Delete
                        </a>
                    </div>
                    `;
                },
                "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"

    });
}

function Delete(url) {
    swal({ //Ask the user if they are sure.
        title: "Delete?",
        text: "Do you want to delete this entry permanently?",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax(
                { //The user is sure, send the request.
                    type: "DELETE",
                    url: url,
                    success: function (data) {
                        if (data.success) { //Deleted, tell the user.
                            toastr.success(data.message); //"message" is part of BookController.cs.
                            dataTable.ajax.reload();
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                });
        }
    });
}