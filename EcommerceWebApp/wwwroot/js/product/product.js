let dataTable;

$.ajax({
    url: "/Admin/api/product/all",
    type: "GET",
    dataType: "json",
    success: function (data) {
        var products = [];
        data.data.forEach(function (item) {
            var product = {
                productId: item.productId,
                proName: item.proName,
                quantity: item.qauntity,
                categoryName: item.category.catName,
                originCountry: item.originCountry,
                description: item.description,
                price: item.price
            };
            products.push(product);
        });

        $('#proTb').DataTable({
            data: products,
            columns: [
                { data: 'proName', title: 'Product Name' },
                { data: 'price', title: 'Price' },
                { data: 'quantity', title: 'Quantity' },
                { data: 'categoryName', title: 'Category' },
                { data: 'originCountry', title: 'Origin Country' },
                { data: 'description', title: 'Description' },
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
    },
    error: function (xhr, textStatus, errorThrown) {
        console.log("Error:", errorThrown);
    }
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

