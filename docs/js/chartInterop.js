// Chart.js interop functions for Blazor
let priceChart = null;

window.renderChart = (canvasId, monthlyData, isDark) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error('Chart canvas not found:', canvasId);
        return;
    }

    // Destroy existing chart if it exists
    if (priceChart) {
        priceChart.destroy();
    }

    // Dark mode colors
    const textColor = isDark ? '#e0e0e0' : '#212529';
    const gridColor = isDark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.07)';
    const bgColor = isDark ? '#1e1e2e' : '#ffffff';

    // Prepare data for Chart.js
    const years = Object.keys(monthlyData).sort();
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const datasets = [];

    // Color palette
    const colors = [
        'rgba(255, 99, 132, 1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 205, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)'
    ];

    years.forEach((year, index) => {
        const yearData = monthlyData[year];
        const prices = [];

        for (let month = 1; month <= 12; month++) {
            prices.push(yearData[month] || null);
        }

        datasets.push({
            label: year,
            data: prices,
            borderColor: colors[index % colors.length],
            backgroundColor: colors[index % colors.length].replace('1)', '0.1)'),
            borderWidth: 2,
            fill: false,
            tension: 0.1,
            pointRadius: 3,
            pointHoverRadius: 5
        });
    });

    // Create chart
    const ctx = canvas.getContext('2d');
    canvas.style.backgroundColor = bgColor;

    priceChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: monthNames,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Cryptocurrency Monthly Average Prices (USD)',
                    color: textColor,
                    font: {
                        size: 16
                    }
                },
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        color: textColor
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                const val = context.parsed.y;
                                if (val < 0.001) {
                                    label += '$' + val.toFixed(8);
                                } else if (val < 1) {
                                    label += '$' + val.toFixed(4);
                                } else {
                                    label += '$' + val.toLocaleString();
                                }
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    ticks: { color: textColor },
                    grid: { color: gridColor }
                },
                y: {
                    beginAtZero: false,
                    ticks: {
                        color: textColor,
                        callback: function(value) {
                            if (value < 0.001) return '$' + value.toFixed(8);
                            if (value < 1) return '$' + value.toFixed(4);
                            return '$' + value.toLocaleString();
                        }
                    },
                    grid: { color: gridColor }
                }
            }
        }
    });
};