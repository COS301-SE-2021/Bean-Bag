
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