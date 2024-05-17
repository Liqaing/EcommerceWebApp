$('#orderTb').DataTable({
    ajax: {
        url: "/api/customer/order",
        type: "GET",
        dataType: "json"
    },
    columns: [
        { data: "orderHeaderId", "width": "10%" },
        { data: "paymentStatus", "width": "25%" },
        { data: "orderStatus", "width": "25%" },
        { data: "orderTotal", "width": "20%" },
        {
            data: "orderHeaderId",
            render: function (data) {
                return `
                        <div class="btn-group text-center d-flex" role="group">                           
                            <a href="/customer/order/Detail?orderId=${data}" class="btn btn-primary rounded">
                                    View Order Detail
                            </a>
                        </div>
                    `
            },
            "Width": "10%",
            "orderable": false
        }
    ]
});