window.renderEloChart = function (canvasId, labels, data) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    if (canvas._chart) {
        canvas._chart.destroy();
    }

    const ctx = canvas.getContext('2d');
    const height = canvas.offsetHeight || 200;
    const gradient = ctx.createLinearGradient(0, 0, 0, height);
    gradient.addColorStop(0, 'rgba(34, 197, 94, 0.25)');
    gradient.addColorStop(1, 'rgba(34, 197, 94, 0)');

    canvas._chart = new Chart(canvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderColor: '#22C55E',
                backgroundColor: gradient,
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6,
                pointBackgroundColor: '#22C55E',
                pointBorderColor: '#0F1115',
                pointBorderWidth: 2,
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: '#1F2430',
                    borderColor: '#2B3240',
                    borderWidth: 1,
                    titleColor: '#9AA4B2',
                    bodyColor: '#F5F7FA',
                    padding: 10,
                    displayColors: false,
                    callbacks: {
                        label: (item) => `ELO ${item.raw}`
                    }
                }
            },
            scales: {
                x: {
                    grid: { color: '#2B3240' },
                    ticks: { color: '#9AA4B2', font: { size: 11 } }
                },
                y: {
                    beginAtZero: false,
                    grid: { color: '#2B3240' },
                    ticks: { color: '#9AA4B2', font: { size: 11 } }
                }
            }
        }
    });
};
