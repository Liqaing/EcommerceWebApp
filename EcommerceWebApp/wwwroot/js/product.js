$(document).ready(() => {
    loadDataTable();
})

const loadDataTable = () => {    
    dataTable = $('#productTable').DataTable({
        "ajax": {
            "url": "https://localhost:7137/Admin/api/product",
            "type": "GET"
        },
        "columns": [            
            { data: "proName" , "width": "15%"},
            { data: "price", "width": "10%" },
            { data: "qauntity", "width": "10%" },
            { data: "category.catName", "width": "15%" },
            { data: "originCountry", "width": "10%" },            
            { data: "description"},            
        ]
    });       
}
/*
{
    data: "imageUrl",               
    render: function (data, type, row) {
        console.log(data)
        return "<img src=" + data + "></img>";
    }
},
*/    
   
