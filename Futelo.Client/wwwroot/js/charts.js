function _futeloTheme(ctx, height) {
    const s = getComputedStyle(document.documentElement);
    const v = n => s.getPropertyValue(n).trim();
    const primaryRgb = v('--bs-primary-rgb');
    const borderHex  = v('--color-border');
    const br = parseInt(borderHex.slice(1, 3), 16);
    const bg = parseInt(borderHex.slice(3, 5), 16);
    const bb = parseInt(borderHex.slice(5, 7), 16);
    const gradient = ctx.createLinearGradient(0, 0, 0, height);
    gradient.addColorStop(0, `rgba(${primaryRgb}, 0.25)`);
    gradient.addColorStop(1, `rgba(${primaryRgb}, 0)`);
    return {
        primary:    v('--color-primary'),
        border:     borderHex,
        textMuted:  v('--color-text-muted'),
        surface2:   v('--color-surface-2'),
        text:       v('--color-text'),
        bg:         v('--color-bg'),
        seasonLine: `rgba(${br}, ${bg}, ${bb}, 0.6)`,
        gradient,
    };
}

window.renderGlobalEloChart = function (canvasId, labels, data, seasons) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    if (canvas._chart) {
        canvas._chart.destroy();
    }

    const ctx = canvas.getContext('2d');
    const t = _futeloTheme(ctx, canvas.offsetHeight || 200);

    const seasonPlugin = {
        id: 'seasonBoundaries',
        afterDraw(chart) {
            const xAxis = chart.scales.x;
            const yAxis = chart.scales.y;
            const pCtx  = chart.ctx;

            seasons.forEach(season => {
                if (season.firstPointIndex <= 0 || season.firstPointIndex >= labels.length) return;

                const x = xAxis.getPixelForValue(season.firstPointIndex);

                pCtx.save();
                pCtx.beginPath();
                pCtx.moveTo(x, yAxis.top);
                pCtx.lineTo(x, yAxis.bottom);
                pCtx.lineWidth = 1;
                pCtx.strokeStyle = t.seasonLine;
                pCtx.setLineDash([4, 4]);
                pCtx.stroke();
                pCtx.setLineDash([]);
                pCtx.fillStyle = t.textMuted;
                pCtx.font = '10px sans-serif';
                pCtx.fillText(season.name, x + 4, yAxis.top + 12);
                pCtx.restore();
            });
        }
    };

    canvas._chart = new Chart(canvas, {
        type: 'line',
        plugins: [seasonPlugin],
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderColor: t.primary,
                backgroundColor: t.gradient,
                fill: true,
                tension: 0.4,
                pointRadius: labels.length > 60 ? 2 : 4,
                pointHoverRadius: 6,
                pointBackgroundColor: t.primary,
                pointBorderColor: t.bg,
                pointBorderWidth: 2,
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: t.surface2,
                    borderColor: t.border,
                    borderWidth: 1,
                    titleColor: t.textMuted,
                    bodyColor: t.text,
                    padding: 10,
                    displayColors: false,
                    callbacks: {
                        label: (item) => `ELO ${item.raw}`
                    }
                }
            },
            scales: {
                x: {
                    grid: { color: t.border },
                    ticks: {
                        color: t.textMuted,
                        font: { size: 11 },
                        maxTicksLimit: 8,
                        maxRotation: 0
                    }
                },
                y: {
                    beginAtZero: false,
                    grid: { color: t.border },
                    ticks: { color: t.textMuted, font: { size: 11 } }
                }
            }
        }
    });
};

window.renderEloChart = function (canvasId, labels, data) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    if (canvas._chart) {
        canvas._chart.destroy();
    }

    const ctx = canvas.getContext('2d');
    const t = _futeloTheme(ctx, canvas.offsetHeight || 200);

    canvas._chart = new Chart(canvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderColor: t.primary,
                backgroundColor: t.gradient,
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6,
                pointBackgroundColor: t.primary,
                pointBorderColor: t.bg,
                pointBorderWidth: 2,
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: t.surface2,
                    borderColor: t.border,
                    borderWidth: 1,
                    titleColor: t.textMuted,
                    bodyColor: t.text,
                    padding: 10,
                    displayColors: false,
                    callbacks: {
                        label: (item) => `ELO ${item.raw}`
                    }
                }
            },
            scales: {
                x: {
                    grid: { color: t.border },
                    ticks: { color: t.textMuted, font: { size: 11 } }
                },
                y: {
                    beginAtZero: false,
                    grid: { color: t.border },
                    ticks: { color: t.textMuted, font: { size: 11 } }
                }
            }
        }
    });
};
