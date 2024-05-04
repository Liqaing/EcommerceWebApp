let dataTable;

$.ajax({
    url: "/Admin/api/order/all",
    type: "GET",
    dataType: "json",
    success: function (data) {
        let orders = [];
        const USDollar = new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        });

        data.data.forEach(function (item) {
            var order = {
                id: item.orderHeaderId,
                name: item.appUser.name,
                phoneNumber: item.appUser.phoneNumber,
                orderStatus: item.orderStatus,
                paymentStatus: item.paymentStatus,
                orderTotal: USDollar.format(item.orderTotal)
            };
            orders.push(order);
        });

        dataTable = $('#orderTb').DataTable({
            data: orders,
            columns: [
                { data: 'id', title: 'Id', 'width': '5%'},
                { data: 'name', title: 'Name', 'width': '15%' },
                { data: 'phoneNumber', title: 'Phone Number', 'width': '15%' },
                { data: 'orderStatus', title: 'Order Status', 'width': '10%' },
                { data: 'paymentStatus', title: 'Payment Status', 'width': '10%' },
                { data: 'orderTotal', title: 'Order Total', 'width': '10%' },                
                {
                    data: "id",
                    render: function (data) {
                        return `
                        <div class="btn-group d-flex" role="group">
                            <a href="/admin/order/edit?proId=${data}" class="btn btn-primary mx-1 w-50 rounded">Edit</a>                            
                        </div>
                    `
                    },
                    "width": "10%",
                    "orderable": false
                }
            ]
        });
    },
    error: function (xhr, textStatus, errorThrown) {
        console.log("Error:", errorThrown);
    }
});