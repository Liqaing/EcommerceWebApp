let dataTable;
$(document).ready(function () {
    dataTable = $("#catTb").dataTable({
        ajax: {
            url: "/admin/api/category/all",
            type: "GET"
        },
        columns: [
            { data: "catName", "width": "85%" },
            {
                data: "catId",
                render: function (data) {
                    return `
                        <div class="btn-group text-center d-flex" role="group">
                            <a href="/admin/category/edit?catId=${data}" class="btn btn-info mx-1 w-50 rounded">Edit</a>
                            <a onClick=_delete("/admin/api/category/delete?catId=${data}") class="btn btn-danger mx-1 w-50 rounded">Delete</a>
                        </div>
                    `
                },
                "Width": "15%"
            }
        ]
    });
});


const _delete = function (url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: (data) => {
                    dataTable.api().ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}