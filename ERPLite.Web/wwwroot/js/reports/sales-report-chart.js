document.addEventListener('DOMContentLoaded', function() {
    var ctx = document.getElementById('salesChart');
    if (!ctx) return;
    var chart = new Chart(ctx.getContext('2d'), {
        type: 'line',
        data: {
            labels: ['Jan','Feb','Mar','Apr','May','Jun'],
            datasets: [{
                label: 'Revenue',
                data: [1200, 1500, 1800, 1400, 2000, 2400],
                borderColor: 'rgba(75,192,192,1)',
                fill: false
            }]
        },
        options: {}
    });
});
