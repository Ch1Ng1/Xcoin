function fillMonths() {
  const sel = document.getElementById('month');
  sel.innerHTML = '';
  for (let m = 1; m <= 12; m++) {
    const o = document.createElement('option');
    o.value = m;
    o.text = new Date(2000, m-1, 1).toLocaleString(undefined, { month: 'long' });
    sel.appendChild(o);
  }
  sel.value = new Date().getMonth() + 1;
}

async function loadChart() {
  const canvas = document.getElementById('chartCanvas');
  const fallbackImg = document.getElementById('chartFallback');
  const status = document.getElementById('status');
  if (status) status.textContent = 'Loading...';
  fallbackImg.style.display = 'none';

  try {
    const res = await fetch(`/api/monthly-averages?t=${Date.now()}`);
    if (!res.ok) {
      if (status) status.textContent = 'Network error: ' + res.status;
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?t=${Date.now()}`;
      return;
    }

    const data = await res.json();
    if (Object.keys(data).length === 0) {
      if (status) status.textContent = 'No data available.';
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?t=${Date.now()}`;
      return;
    }
    try {
      renderChartFromJson(data);
      if (status) status.textContent = '';
    }
    catch (renderErr) {
      console.error('Render error', renderErr);
      if (status) status.textContent = 'Render error: ' + (renderErr && renderErr.message ? renderErr.message : String(renderErr));
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?t=${Date.now()}`;
    }
  }
  catch (err) {
    if (status) status.textContent = 'Network error: ' + (err.message || err);
    fallbackImg.style.display = '';
    canvas.style.display = 'none';
    fallbackImg.src = `/chart?t=${Date.now()}`;
  }
}

// Chart.js instance holder
let chartInstance = null;

function hslToRgba(h, s, l, a) {
  return `hsla(${h}, ${s}%, ${l}%, ${a})`;
}

function renderChartFromJson(data) {
  // data is { "2021": { "1": avg, "2": avg, ... }, "2022": {...}, ... }
  const years = Object.keys(data).sort();
  const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  const datasets = [];

  const colors = [
    'rgba(255, 99, 132, 1)',   // Red
    'rgba(54, 162, 235, 1)',   // Blue
    'rgba(255, 205, 86, 1)',   // Yellow
    'rgba(75, 192, 192, 1)',   // Green
    'rgba(153, 102, 255, 1)',  // Purple
    'rgba(255, 159, 64, 1)'    // Orange
  ];

  years.forEach((year, index) => {
    const yearData = data[year];
    const prices = [];
    for (let m = 1; m <= 12; m++) {
      prices.push(yearData[m] || null);
    }
    datasets.push({
      label: year,
      data: prices,
      borderColor: colors[index % colors.length],
      backgroundColor: colors[index % colors.length].replace('1)', '0.2)'),
      tension: 0.1,
      fill: false
    });
  });

  const canvas = document.getElementById('chartCanvas');
  const fallbackImg = document.getElementById('chartFallback');
  fallbackImg.style.display = 'none';
  canvas.style.display = '';

  const ctx = canvas.getContext('2d');
  if (chartInstance) {
    chartInstance.destroy();
    chartInstance = null;
  }
  chartInstance = new Chart(ctx, {
    type: 'line',
    data: {
      labels: monthNames,
      datasets: datasets
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        x: {
          title: {
            display: true,
            text: 'Month'
          }
        },
        y: {
          title: {
            display: true,
            text: 'Price (USD)'
          },
          beginAtZero: false,
          ticks: { callback: function(v){ return v>=1000?('$'+(v/1000).toFixed(1)+'k'):('$'+v); } }
        }
      },
      plugins: {
        legend: { position: 'top' },
        tooltip: { mode: 'index', intersect: false }
      }
    }
  });
}

document.addEventListener('DOMContentLoaded', () => {
  loadChart();
});
