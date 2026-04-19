// Global chart instance
let dashboardChart = null;

function initializeDashboardChart(data) {
    const ctx = document.getElementById('dashboardChart').getContext('2d');

    dashboardChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.dates,
            datasets: [
                {
                    label: 'Bookings',
                    data: data.bookingCounts,
                    borderColor: '#4e73df',
                    backgroundColor: 'rgba(78, 115, 223, 0.05)',
                    borderWidth: 3,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBackgroundColor: '#4e73df',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    tension: 0.3,
                    fill: true
                },
                {
                    label: 'New Members',
                    data: data.memberCounts,
                    borderColor: '#1cc88a',
                    backgroundColor: 'rgba(28, 200, 138, 0.05)',
                    borderWidth: 3,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBackgroundColor: '#1cc88a',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    tension: 0.3,
                    fill: true,
                    hidden: true
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: 'rgba(0,0,0,0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    borderColor: '#4e73df',
                    borderWidth: 1,
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            let value = context.parsed.y;
                            return `${label}: ${value.toLocaleString()}`;
                        }
                    }
                },
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: {
                        usePointStyle: true,
                        boxWidth: 10,
                        padding: 15
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0,0,0,0.05)',
                        drawBorder: false
                    },
                    title: {
                        display: true,
                        text: 'Count',
                        color: '#6c757d',
                        font: {
                            size: 12,
                            weight: '500'
                        }
                    },
                    ticks: {
                        stepSize: 1,
                        callback: function (value) {
                            return value.toLocaleString();
                        }
                    }
                },
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    title: {
                        display: true,
                        text: 'Date',
                        color: '#6c757d',
                        font: {
                            size: 12,
                            weight: '500'
                        }
                    },
                    ticks: {
                        maxRotation: 45,
                        minRotation: 45,
                        autoSkip: true,
                        maxTicksLimit: 10
                    }
                }
            },
            elements: {
                line: {
                    borderJoin: 'round'
                }
            },
            hover: {
                mode: 'nearest',
                intersect: true
            }
        }
    });
}

function updateChartType(type) {
    if (!dashboardChart) return;

    const bookingsDataset = dashboardChart.data.datasets[0];
    const membersDataset = dashboardChart.data.datasets[1];

    switch (type) {
        case 'bookings':
            bookingsDataset.hidden = false;
            membersDataset.hidden = true;
            break;
        case 'members':
            bookingsDataset.hidden = true;
            membersDataset.hidden = false;
            break;
        case 'both':
            bookingsDataset.hidden = false;
            membersDataset.hidden = false;
            break;
    }

    dashboardChart.update();
}

// Refresh chart data via AJAX
async function refreshChartData() {
    try {
        showLoading();

        const response = await fetch('/Admin/GetChartData', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            }
        });

        if (!response.ok) throw new Error('Failed to fetch chart data');

        const newData = await response.json();

        dashboardChart.data.labels = newData.dates;
        dashboardChart.data.datasets[0].data = newData.bookingCounts;
        dashboardChart.data.datasets[1].data = newData.memberCounts;
        dashboardChart.update();

    } catch (error) {
        console.error('Error refreshing chart:', error);
        showError('Failed to load chart data. Please try again.');
    } finally {
        hideLoading();
    }
}

function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : '';
}

function showLoading() {
    let overlay = document.querySelector('.loading-overlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.className = 'loading-overlay';
        overlay.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
        document.querySelector('.chart-card').appendChild(overlay);
    }
    overlay.style.display = 'flex';
}

function hideLoading() {
    const overlay = document.querySelector('.loading-overlay');
    if (overlay) {
        overlay.style.display = 'none';
    }
}

function showError(message) {
    console.error(message);
    alert(message);
}

if (typeof autoRefresh !== 'undefined' && autoRefresh) {
    setInterval(refreshChartData, 300000); // 5 minutes
}

window.initializeDashboardChart = initializeDashboardChart;
window.updateChartType = updateChartType;
window.refreshChartData = refreshChartData;