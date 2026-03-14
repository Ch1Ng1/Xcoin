// Global chart instance
let chartInstance = null;

// Embedded cryptocurrency data for GitHub Pages
const cryptoData = {
    bitcoin: {
        2019: {1: 3450, 2: 3440, 3: 4100, 4: 5200, 5: 7800, 6: 9500, 7: 9800, 8: 10500, 9: 10200, 10: 9200, 11: 8800, 12: 7200},
        2020: {1: 8400, 2: 9500, 3: 6400, 4: 8800, 5: 9700, 6: 9200, 7: 11000, 8: 11800, 9: 10200, 10: 11400, 11: 16800, 12: 29000},
        2021: {1: 33000, 2: 45000, 3: 59000, 4: 58000, 5: 37000, 6: 35000, 7: 33000, 8: 47000, 9: 43000, 10: 61000, 11: 65000, 12: 47000},
        2022: {1: 43000, 2: 38000, 3: 47000, 4: 38000, 5: 29000, 6: 19000, 7: 23000, 8: 24000, 9: 19000, 10: 20000, 11: 16000, 12: 16500},
        2023: {1: 23000, 2: 25000, 3: 28000, 4: 30000, 5: 27000, 6: 25000, 7: 30000, 8: 29000, 9: 26000, 10: 34000, 11: 37000, 12: 42000}
    },
    ethereum: {
        2019: {1: 140, 2: 135, 3: 165, 4: 170, 5: 240, 6: 290, 7: 220, 8: 180, 9: 170, 10: 180, 11: 150, 12: 130},
        2020: {1: 180, 2: 220, 3: 130, 4: 200, 5: 240, 6: 230, 7: 380, 8: 400, 9: 380, 10: 360, 11: 580, 12: 730},
        2021: {1: 1100, 2: 1600, 3: 1800, 4: 2600, 5: 2700, 6: 1800, 7: 2100, 8: 3200, 9: 2900, 10: 3600, 11: 4500, 12: 3700},
        2022: {1: 2500, 2: 2600, 3: 2900, 4: 2900, 5: 1800, 6: 1100, 7: 1700, 8: 1700, 9: 1300, 10: 1300, 11: 1200, 12: 1200},
        2023: {1: 1600, 2: 1600, 3: 1700, 4: 1900, 5: 1800, 6: 1700, 7: 1900, 8: 1600, 9: 1600, 10: 1700, 11: 2000, 12: 2200}
    },
    solana: {
        2019: {1: 0.5, 2: 0.8, 3: 1.2, 4: 1.8, 5: 2.5, 6: 1.8, 7: 1.2, 8: 1.0, 9: 1.2, 10: 1.5, 11: 1.8, 12: 1.3},
        2020: {1: 1.8, 2: 2.2, 3: 1.0, 4: 2.5, 5: 3.2, 6: 2.8, 7: 3.5, 8: 3.8, 9: 3.2, 10: 2.8, 11: 3.8, 12: 1.8},
        2021: {1: 3.2, 2: 18, 3: 25, 4: 45, 5: 45, 6: 35, 7: 38, 8: 45, 9: 45, 10: 55, 11: 65, 12: 55},
        2022: {1: 95, 2: 80, 3: 75, 4: 45, 5: 35, 6: 25, 7: 40, 8: 45, 9: 30, 10: 30, 11: 55, 12: 12},
        2023: {1: 25, 2: 25, 3: 20, 4: 25, 5: 20, 6: 15, 7: 25, 8: 30, 9: 20, 10: 25, 11: 60, 12: 70}
    },
    xrp: {
        2019: {1: 0.32, 2: 0.31, 3: 0.35, 4: 0.38, 5: 0.42, 6: 0.45, 7: 0.32, 8: 0.28, 9: 0.27, 10: 0.29, 11: 0.22, 12: 0.21},
        2020: {1: 0.23, 2: 0.25, 3: 0.18, 4: 0.20, 5: 0.22, 6: 0.18, 7: 0.22, 8: 0.25, 9: 0.24, 10: 0.25, 11: 0.28, 12: 0.22},
        2021: {1: 0.28, 2: 0.45, 3: 0.75, 4: 0.85, 5: 0.95, 6: 0.65, 7: 0.70, 8: 0.85, 9: 0.75, 10: 1.05, 11: 1.15, 12: 0.85},
        2022: {1: 0.75, 2: 0.70, 3: 0.75, 4: 0.45, 5: 0.40, 6: 0.35, 7: 0.45, 8: 0.50, 9: 0.35, 10: 0.45, 11: 0.38, 12: 0.32},
        2023: {1: 0.38, 2: 0.38, 3: 0.42, 4: 0.45, 5: 0.42, 6: 0.38, 7: 0.42, 8: 0.45, 9: 0.42, 10: 0.52, 11: 0.62, 12: 0.65}
    },
    cardano: {
        2019: {1: 0.045, 2: 0.048, 3: 0.055, 4: 0.065, 5: 0.085, 6: 0.055, 7: 0.045, 8: 0.035, 9: 0.032, 10: 0.035, 11: 0.032, 12: 0.028},
        2020: {1: 0.032, 2: 0.038, 3: 0.028, 4: 0.045, 5: 0.055, 6: 0.055, 7: 0.125, 8: 0.135, 9: 0.115, 10: 0.095, 11: 0.155, 12: 0.175},
        2021: {1: 0.35, 2: 0.85, 3: 1.15, 4: 1.35, 5: 1.45, 6: 1.25, 7: 1.35, 8: 1.55, 9: 1.25, 10: 1.85, 11: 1.95, 12: 1.45},
        2022: {1: 0.85, 2: 0.75, 3: 0.85, 4: 0.55, 5: 0.45, 6: 0.35, 7: 0.45, 8: 0.50, 9: 0.35, 10: 0.32, 11: 0.32, 12: 0.25},
        2023: {1: 0.32, 2: 0.32, 3: 0.35, 4: 0.38, 5: 0.35, 6: 0.32, 7: 0.38, 8: 0.32, 9: 0.28, 10: 0.32, 11: 0.38, 12: 0.42}
    }
};

async function loadChart() {
  const canvas = document.getElementById('chartCanvas');
  const fallbackImg = document.getElementById('chartFallback');
  const status = document.getElementById('status');
  const coinSelect = document.getElementById('coinSelect');
  const coin = coinSelect ? coinSelect.value : 'bitcoin';

  if (status) status.textContent = 'Loading chart data...';
  fallbackImg.style.display = 'none';

  try {
    // Use embedded data instead of API call
    const data = cryptoData[coin];
    if (!data) {
      throw new Error(`No data available for ${coin}`);
    }

    renderChartFromJson(data);
    if (status) status.textContent = '';
  }
  catch (err) {
    console.error('Error loading chart data', err);
    if (status) status.textContent = `Failed to load data for ${coin}.`;
    fallbackImg.style.display = '';
    canvas.style.display = 'none';
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
