// Global chart instance
let chartInstance = null;

// Embedded cryptocurrency data for GitHub Pages
const cryptoData = {
    bitcoin: {
        2021: {1: 33000, 2: 45000, 3: 58000, 4: 63000, 5: 37000, 6: 35000, 7: 33000, 8: 47000, 9: 43000, 10: 61000, 11: 65000, 12: 47000},
        2022: {1: 47000, 2: 44000, 3: 47000, 4: 46000, 5: 38000, 6: 30000, 7: 20000, 8: 24000, 9: 20000, 10: 19000, 11: 17000, 12: 17000},
        2023: {1: 17000, 2: 23000, 3: 28000, 4: 30000, 5: 27000, 6: 30000, 7: 31000, 8: 29000, 9: 26000, 10: 34000, 11: 37000, 12: 42000},
        2024: {1: 42000, 2: 52000, 3: 73000, 4: 64000, 5: 61000, 6: 64000, 7: 58000, 8: 59000, 9: 55000, 10: 63000, 11: 96000, 12: 126000},
        2025: {1: 108000, 2: 95000, 3: 80000, 4: 90000, 5: 95000, 6: 100000, 7: 95000, 8: 85000, 9: 75000, 10: 80000, 11: 85000, 12: 95000},
        2026: {1: 90751, 2: 69251, 3: 68928}
    },
    ethereum: {
        2021: {1: 2000, 2: 3000, 3: 3500, 4: 4000, 5: 2500, 6: 2200, 7: 1800, 8: 3200, 9: 2900, 10: 4300, 11: 4700, 12: 3700},
        2022: {1: 3700, 2: 3400, 3: 3500, 4: 3300, 5: 2800, 6: 1800, 7: 1000, 8: 1600, 9: 1300, 10: 1200, 11: 1100, 12: 1200},
        2023: {1: 1200, 2: 1600, 3: 1800, 4: 1900, 5: 1700, 6: 1800, 7: 1900, 8: 1700, 9: 1600, 10: 2000, 11: 2200, 12: 2500},
        2024: {1: 2500, 2: 3200, 3: 3800, 4: 3500, 5: 3300, 6: 3500, 7: 3200, 8: 3300, 9: 3000, 10: 3500, 11: 4000, 12: 4200},
        2025: {1: 4200, 2: 3800, 3: 3500, 4: 4000, 5: 4200, 6: 4500, 7: 4200, 8: 3800, 9: 3300, 10: 3500, 11: 3800, 12: 4200},
        2026: {1: 3085, 2: 2040, 3: 2018}
    },
    solana: {
        2021: {1: 20, 2: 50, 3: 100, 4: 150, 5: 200, 6: 180, 7: 140, 8: 160, 9: 130, 10: 200, 11: 250, 12: 170},
        2022: {1: 170, 2: 150, 3: 100, 4: 120, 5: 90, 6: 30, 7: 20, 8: 40, 9: 30, 10: 25, 11: 20, 12: 15},
        2023: {1: 15, 2: 20, 3: 25, 4: 30, 5: 20, 6: 25, 7: 30, 8: 25, 9: 20, 10: 30, 11: 40, 12: 50},
        2024: {1: 50, 2: 80, 3: 120, 4: 150, 5: 140, 6: 160, 7: 130, 8: 140, 9: 120, 10: 150, 11: 180, 12: 200},
        2025: {1: 200, 2: 180, 3: 150, 4: 170, 5: 190, 6: 200, 7: 180, 8: 160, 9: 140, 10: 150, 11: 170, 12: 190},
        2026: {1: 133, 2: 87, 3: 86}
    },
    xrp: {
        2021: {1: 0.5, 2: 0.8, 3: 1.0, 4: 1.2, 5: 0.9, 6: 0.7, 7: 0.6, 8: 1.0, 9: 0.9, 10: 1.3, 11: 1.4, 12: 1.0},
        2022: {1: 1.0, 2: 0.9, 3: 0.8, 4: 0.7, 5: 0.6, 6: 0.4, 7: 0.3, 8: 0.5, 9: 0.4, 10: 0.3, 11: 0.3, 12: 0.3},
        2023: {1: 0.3, 2: 0.4, 3: 0.5, 4: 0.5, 5: 0.4, 6: 0.5, 7: 0.5, 8: 0.4, 9: 0.4, 10: 0.6, 11: 0.7, 12: 0.8},
        2024: {1: 0.8, 2: 0.9, 3: 1.0, 4: 0.9, 5: 0.8, 6: 0.9, 7: 0.8, 8: 0.8, 9: 0.7, 10: 0.9, 11: 1.0, 12: 1.1},
        2025: {1: 1.1, 2: 1.0, 3: 0.9, 4: 1.0, 5: 1.1, 6: 1.2, 7: 1.1, 8: 1.0, 9: 0.9, 10: 1.0, 11: 1.1, 12: 1.2},
        2026: {1: 2.01, 2: 1.44, 3: 1.38}
    },
    cardano: {
        2021: {1: 1.0, 2: 1.5, 3: 2.0, 4: 2.5, 5: 1.8, 6: 1.5, 7: 1.2, 8: 2.0, 9: 1.8, 10: 2.5, 11: 2.8, 12: 2.0},
        2022: {1: 2.0, 2: 1.8, 3: 1.5, 4: 1.3, 5: 1.0, 6: 0.5, 7: 0.3, 8: 0.6, 9: 0.5, 10: 0.4, 11: 0.3, 12: 0.3},
        2023: {1: 0.3, 2: 0.4, 3: 0.5, 4: 0.5, 5: 0.4, 6: 0.5, 7: 0.5, 8: 0.4, 9: 0.4, 10: 0.6, 11: 0.7, 12: 0.8},
        2024: {1: 0.8, 2: 1.0, 3: 1.2, 4: 1.1, 5: 1.0, 6: 1.1, 7: 1.0, 8: 1.0, 9: 0.9, 10: 1.1, 11: 1.3, 12: 1.4},
        2025: {1: 1.4, 2: 1.3, 3: 1.1, 4: 1.2, 5: 1.4, 6: 1.5, 7: 1.4, 8: 1.3, 9: 1.1, 10: 1.2, 11: 1.3, 12: 1.4},
        2026: {1: 0.38, 2: 0.28, 3: 0.27}
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
