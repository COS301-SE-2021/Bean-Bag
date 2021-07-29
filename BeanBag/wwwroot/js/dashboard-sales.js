//DEFAULTS

$(function() {
    "use strict";
// ============================================================== 
    // Total Sale -Graphic 
    // ============================================================== 
    let ctx = document.getElementById("total-sale").getContext('2d');
    new Chart(ctx, {
        type: 'doughnut',

        data: {
            labels: ["Furniture", " Clothing", "Vehicles", " Gadgets"],
            datasets: [{
                backgroundColor: [
                    "#5969ff",
                    "#ff407b",
                    "#25d5f2",
                    "#ffc750"
                ],
                data: [10, 30, 50, 10]
            }]
        },
        options: {
            legend: {
                display: false

            }
        }

    });
});

//Clicking Functions
/*

                  //Chart
                 $(function() {
                     "use strict";           
                // ============================================================== 
                // Total Items -Graphic 
                // ============================================================== 
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
                });
             
 */