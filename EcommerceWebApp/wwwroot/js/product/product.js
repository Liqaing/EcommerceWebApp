let dataTable;

$(document).ready(function () {
    dataTable = $("#proTb").DataTable({
        "ajax": { url: "/Admin/api/product/all" },
        "columns": [
            { data: "proName", "width": "15%", "className": "text-center" },
            { data: "price", "width": "10%", "className": "text-center" },
            { data: "qauntity", "width": "10%", "className": "text-center" },
            { data: "category.catName", "width": "15%", "className": "text-center" },
            { data: "originCountry", "width": "15%", "className": "text-center" },
            { data: "description", "width": "20%", "className": "text-center" },
            {
                data: "productId",
                render: function (data) {
                    return `
                        <div class="btn-group d-flex" role="group">
                            <a href="/admin/product/upsert?proId=${data}" class="btn btn-primary mx-1 w-50 rounded">Edit</a>
                            <a onClick=_delete("/admin/api/product/delete?proId=${data}") class="btn btn-danger mx-1 w-50 rounded">Delete</a>
                        </div>
                    `
                },
                "width": "15%",
                "orderable": false
            }
        ]
    });
});
/*
{
    data: "imageUrl",               
    render: function (data, type, row) {
        console.log(data)
        return "<img src=" + data + "></img>";
    }
},
*/


// Alert
const _delete = (url) => {
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
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}

