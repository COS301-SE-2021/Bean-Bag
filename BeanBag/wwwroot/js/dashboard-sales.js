
    //Ajax function to get the most recent Items
    $(document).ready(function ()
    {
        $("#InventoryDropDown").change(function () {

            $.getJSON('/Dashboard/GetItems/' +  $("#InventoryDropDown Option:Selected").val(), function (data) {
                let items = '';
                $.each(data, function (i, item) {
                    if (i<4){
                        let x=i+1;
                        items += '<tr>'+
                            '                                      '+
                            '                                <td>'+x+'</td>'+
                            '                                <td>'+
                            '                                    <div class="m-r-10"><img src="'+item.imageURL+'" alt="user" class="rounded" width="95"></div>'+
                            '                                </td>'+
                            '                                <td>'+$("#InventoryDropDown Option:Selected").text()+'</td>'+
                            '                                <td>'+item.name+'</td>'+
                            '                                <td>'+item.type+'</td>'+
                            '                                <td>'+item.quantity +'</td>'+
                            '                                <td>'+item.price+'</td>'+
                            ''+
                            '                                <td>'+item.entryDate+'</td>'+
                            '                            </tr>';

                    }
                });
                $('#recentItems').html(items);
            });
        })
    })


    //Ajax function to get the chart total items
    $(document).ready(function ()
    {

        $("#InventoryDropDownChart").change(function () {

            let inventoryid=$("#InventoryDropDownChart Option:Selected").val();
            let totalItems=0;
            let items='';
            let otherTotal = 0;
            let other=0;
            let item1='';
            let item2='';
            let item3='';
            var quantity1=0, quantity2=0, quantity3=0;

            //Top 3 Items 
            $.getJSON('/Dashboard/TopItems/' +  inventoryid, function (data) {

                $.each(data, function (i, item) { //make if statement for colors

                    if (i===0)
                    {
                        item1=item.type;
                        quantity1= item.qsum;
                    }else if (i===1 )
                    {
                        item2=item.type;
                        quantity2= item.qsum;
                    }else {
                        item3=item.type;
                        quantity3= item.qsum;
                    }


                    items += '<p>'+
                        '                            <span class="fa-xs text-primary mr-1 legend-title"><i class="fa fa-fw fa-square-full"></i></span><span class="legend-text">'+item.type+'</span>'+
                        '                            <span class="float-right">'+item.qsum+'</span>'+
                        '                        </p>';
                    otherTotal+=item.qsum;
                    totalItems=item.total;
                });
                //Other
                other=totalItems-otherTotal;

                if (other!==0){
                    items += '<p>'+
                        '                            <span class="fa-xs text-primary mr-1 legend-title"><i class="fa fa-fw fa-square-full"></i></span><span class="legend-text"> Other </span>'+
                        '                            <span class="float-right">'+other+'</span>'+
                        '                        </p>';}
                $("#topItems").html(items);

                //Chart
                if (item1!==''){
                    let ctx = document.getElementById("total-sale").getContext('2d');
                    new Chart(ctx, {
                        type: 'doughnut',

                        data: {
                            labels: [item1, item2, item3, "Other"],
                            datasets: [{
                                backgroundColor: [
                                    "#5969ff",
                                    "#ff407b",
                                    "#25d5f2",
                                    "#ffc750"
                                ],
                                data: [quantity1, quantity2, quantity3, other]
                            }]
                        },
                        options: {
                            legend: {
                                display: false

                            }
                        }

                    });
                }
            });

            //Total Items                    
            $.getJSON('/Dashboard/TotalItems/' +  inventoryid, function (data) {
                $("#totalItems").text(data);
            });


        } )})
