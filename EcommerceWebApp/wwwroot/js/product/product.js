let DataTable;

$.ajax({
    url: "/Admin/api/product/all",
    type: "GET",
    dataType: "json",
    success: function (data) {
        let products = [];
        data.data.forEach(function (item) {
            var product = {
                productId: item.productId,
                proName: item.proName,
                quantity: item.quantity,
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
                { data: 'proName', title: 'Title', 'width': '10%'},
                { data: 'price', title: 'Price', 'width': '5%' },
                { data: 'quantity', title: 'Quantity', 'width': '5%' },
                { data: 'categoryName', title: 'Category', 'width': '10%' },
                { data: 'originCountry', title: 'Country', 'width': '10%' },
                {

                    data: 'description',
                    title: 'Description',
                    render: function (data) {
                        return `<div class="des-cell"> ${data}</div>`
                    },
                    'width': '25%'
                },
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
                    DataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}

