// Chart.js interop functions for Blazor
let priceChart = null;

window.renderChart = (canvasId, chartData) => {
    const canvas = document.getElementById(canvasId);

    if (!canvas) {
        console.error("Chart canvas not found:", canvasId);
        return;
    }

    const ctx = canvas.getContext('2d');

    new Chart(ctx, {
        type: chartData.type,
        data: chartData.data,
        options: chartData.options
    });
};