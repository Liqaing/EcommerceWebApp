let dataTable;

$.ajax({
    url: "/admin/api/category/all",
    type: "GET",
    dataType: "json",
    success: function (data) {
        let categories = [];

        data.data.forEach(function (item) {
            let category = {
                catId: item.catId,
                catName: item.catName,                
            };
            categories.push(category);
        });

        $('#catTb').DataTable({
            data: categories,
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
                    "Width": "15%",
                    "orderable": false
                }
            ]
        });
    },
    error: function (xhr, textStatus, errorThrown) {
        console.log("Error:", errorThrown);
    }
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