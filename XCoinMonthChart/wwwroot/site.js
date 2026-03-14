// Global chart instance
let chartInstance = null;

async function loadChart() {
  const canvas = document.getElementById('chartCanvas');
  const fallbackImg = document.getElementById('chartFallback');
  const status = document.getElementById('status');
  const coinSelect = document.getElementById('coinSelect');
  const coin = coinSelect ? coinSelect.value : 'bitcoin';
  if (status) status.textContent = 'Loading chart data...';
  fallbackImg.style.display = 'none';

  // Handle fallback image load errors
  fallbackImg.onerror = () => {
    console.warn('Fallback chart image also failed to load');
    if (status) status.textContent = `Could not load chart for ${coin}. Server may be unavailable.`;
    fallbackImg.style.display = 'none';
  };

  try {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 15000);
    const res = await fetch(`/api/monthly-averages?coin=${encodeURIComponent(coin)}&t=${Date.now()}`, {
      signal: controller.signal
    });
    clearTimeout(timeoutId);

    if (!res.ok) {
      console.warn(`API returned HTTP ${res.status} for ${coin}`);
      if (status) status.textContent = `Failed to load data for ${coin} (HTTP ${res.status}). Showing cached chart.`;
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?coin=${encodeURIComponent(coin)}&t=${Date.now()}`;
      return;
    }

    let data;
    try {
      data = await res.json();
    } catch (parseErr) {
      console.error('Failed to parse JSON response', parseErr);
      if (status) status.textContent = `Invalid response from server for ${coin}. Showing cached chart.`;
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?coin=${encodeURIComponent(coin)}&t=${Date.now()}`;
      return;
    }

    if (!data || typeof data !== 'object' || Object.keys(data).length === 0) {
      console.warn(`No data returned for ${coin}`);
      if (status) status.textContent = `No data available for ${coin}.`;
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?coin=${encodeURIComponent(coin)}&t=${Date.now()}`;
      return;
    }
    try {
      renderChartFromJson(data);
      if (status) status.textContent = '';
    }
    catch (renderErr) {
      console.error('Render error', renderErr);
      if (status) status.textContent = `Failed to render chart for ${coin}. Showing server-generated chart.`;
      fallbackImg.style.display = '';
      canvas.style.display = 'none';
      fallbackImg.src = `/chart?coin=${encodeURIComponent(coin)}&t=${Date.now()}`;
    }
  }
  catch (err) {
    const isTimeout = err.name === 'AbortError';
    console.error(isTimeout ? 'Request timed out' : 'Fetch error', err);
    if (status) status.textContent = isTimeout
      ? `Request timed out for ${coin}. Showing cached chart.`
      : `Failed to load data for ${coin}. Showing cached chart.`;
    fallbackImg.style.display = '';
    canvas.style.display = 'none';
    fallbackImg.src = `/chart?coin=${encodeURIComponent(coin)}&t=${Date.now()}`;
  }
}

// Theme management
let currentTheme = localStorage.getItem('theme') || 'light';

function applyTheme(theme) {
  document.documentElement.setAttribute('data-theme', theme);
  currentTheme = theme;
  localStorage.setItem('theme', theme);

  // Update toggle button icons
  const sunIcon = document.getElementById('sunIcon');
  const moonIcon = document.getElementById('moonIcon');
  if (theme === 'dark') {
    sunIcon.style.display = 'block';
    moonIcon.style.display = 'none';
  } else {
    sunIcon.style.display = 'none';
    moonIcon.style.display = 'block';
  }

  // Re-render chart if it exists
  if (chartInstance) {
    renderChartFromJson(chartInstance.data.originalData);
  }
}

function toggleTheme() {
  const newTheme = currentTheme === 'light' ? 'dark' : 'light';
  applyTheme(newTheme);
}

// Get theme-aware colors
function getChartColors() {
  const isDark = currentTheme === 'dark';

  if (isDark) {
    return [
      'rgba(255, 99, 132, 1)',    // Red
      'rgba(54, 162, 235, 1)',    // Blue
      'rgba(255, 205, 86, 1)',    // Yellow
      'rgba(75, 192, 192, 1)',    // Teal
      'rgba(153, 102, 255, 1)',   // Purple
      'rgba(255, 159, 64, 1)'     // Orange
    ];
  } else {
    return [
      'rgba(255, 99, 132, 1)',   // Red
      'rgba(54, 162, 235, 1)',   // Blue
      'rgba(255, 205, 86, 1)',   // Yellow
      'rgba(75, 192, 192, 1)',   // Green
      'rgba(153, 102, 255, 1)',  // Purple
      'rgba(255, 159, 64, 1)'    // Orange
    ];
  }
}

function getChartOptions(minPrice, maxPrice) {
  const isDark = currentTheme === 'dark';

  return {
    responsive: false,
    maintainAspectRatio: true,
    scales: {
      x: {
        title: {
          display: true,
          text: 'Month',
          color: isDark ? '#ffffff' : '#666666'
        },
        ticks: {
          color: isDark ? '#cccccc' : '#666666'
        },
        grid: {
          color: isDark ? '#495057' : '#e5e7eb'
        }
      },
      y: {
        title: {
          display: true,
          text: 'Price (USD)',
          color: isDark ? '#ffffff' : '#666666'
        },
        beginAtZero: false,
        min: minPrice,
        max: maxPrice,
        ticks: {
          callback: function(v){ return v>=1000?('$'+(v/1000).toFixed(1)+'k'):('$'+v); },
          color: isDark ? '#cccccc' : '#666666'
        },
        grid: {
          color: isDark ? '#495057' : '#e5e7eb'
        }
      },
      y1: {
        type: 'linear',
        display: true,
        position: 'right',
        title: {
          display: true,
          text: 'Price (USD)',
          color: isDark ? '#ffffff' : '#666666'
        },
        beginAtZero: false,
        min: minPrice,
        max: maxPrice,
        ticks: {
          callback: function(v){ return v>=1000?('$'+(v/1000).toFixed(1)+'k'):('$'+v); },
          color: isDark ? '#cccccc' : '#666666'
        },
        grid: {
          color: isDark ? '#495057' : '#e5e7eb'
        }
      }
    },
    plugins: {
      legend: {
        position: 'top',
        labels: {
          color: isDark ? '#ffffff' : '#666666'
        }
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        backgroundColor: isDark ? 'rgba(45, 52, 54, 0.9)' : 'rgba(255, 255, 255, 0.9)',
        titleColor: isDark ? '#ffffff' : '#000000',
        bodyColor: isDark ? '#cccccc' : '#666666',
        borderColor: isDark ? '#495057' : '#e5e7eb',
        borderWidth: 1
      }
    }
  };
}

function renderChartFromJson(data) {
  // data is { "2021": { "1": avg, "2": avg, ... }, "2022": {...}, ... }
  const years = Object.keys(data).filter(y => /^\d{4}$/.test(y)).sort();
  const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  const datasets = [];

  const colors = getChartColors();

  // Calculate min and max price for consistent scales
  let minPrice = Infinity;
  let maxPrice = -Infinity;
  years.forEach(year => {
    const yearData = data[year];
    Object.values(yearData).forEach(price => {
      if (price !== null && price !== undefined && isFinite(price)) {
        minPrice = Math.min(minPrice, price);
        maxPrice = Math.max(maxPrice, price);
      }
    });
  });
  if (minPrice === Infinity) minPrice = 0;
  if (maxPrice === -Infinity) maxPrice = 100;
  // Add some padding
  const padding = (maxPrice - minPrice) * 0.1;
  minPrice = Math.max(0, minPrice - padding);
  maxPrice += padding;

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
    options: getChartOptions(minPrice, maxPrice)
  });

  // Store original data for theme switching
  chartInstance.data.originalData = data;
}

document.addEventListener('DOMContentLoaded', () => {
  // Initialize theme
  applyTheme(currentTheme);

  // Set up theme toggle
  const themeToggle = document.getElementById('themeToggle');
  if (themeToggle) {
    themeToggle.addEventListener('click', toggleTheme);
  }

  // Load initial chart
  loadChart();

  // Set up coin selector
  const coinSelect = document.getElementById('coinSelect');
  if (coinSelect) {
    coinSelect.addEventListener('change', loadChart);
  }
});
