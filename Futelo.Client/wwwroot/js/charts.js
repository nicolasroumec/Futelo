window.renderEloChart = function (canvasId, labels, data) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    if (canvas._chart) {
        canvas._chart.destroy();
    }

    canvas._chart = new Chart(canvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderColor: 'rgb(13, 110, 253)',
                backgroundColor: 'rgba(13, 110, 253, 0.08)',
                fill: true,
                tension: 0.3,
                pointRadius: 3,
                pointHoverRadius: 5
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: { beginAtZero: false }
            }
        }
    });
};
