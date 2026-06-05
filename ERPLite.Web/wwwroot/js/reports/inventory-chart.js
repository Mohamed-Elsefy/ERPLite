document.addEventListener('DOMContentLoaded', function() {
    var ctx = document.getElementById('inventoryChart');
    if (!ctx) return;
    var chart = new Chart(ctx.getContext('2d'), {
        type: 'doughnut',
        data: {
            labels: ['Normal','Low Stock','Out of Stock'],
            datasets: [{
                data: [60, 30, 10],
                backgroundColor: ['#28a745', '#ffc107', '#dc3545']
            }]
        },
        options: {}
    });
});
