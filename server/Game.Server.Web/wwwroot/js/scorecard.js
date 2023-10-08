const data = JSON.parse(document.getElementById('data').innerHTML);
const ctx = document.getElementById('scorecard').getContext('2d');
const chart = new Chart(ctx, {
    type: 'line',
    data: data,
    options: {
        scales: {
            y: {
                type: 'linear',
                display: true,
                position: 'left',
            },
            y1: {
                type: 'linear',
                display: false,
                position: 'left',
                grid: {
                    drawOnChartArea: false, // only want the grid lines for one axis to show up
                }
            }
        },
        animation: {
            duration: 0
        }
    }
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl(data.url)
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    await start();
});

connection.on("addChartData", function (point) {

    chart.data.labels.push(point.label);
    chart.data.datasets.forEach((dataset) => {
        dataset.label = point.teamname;
        dataset.borderColor = Utils.CHART_COLORS.red,
        dataset.backgroundColor = Utils.transparentize(Utils.CHART_COLORS.red, 0.5),
        dataset.data.push(point.score);
    });

    chart.update();

    if (chart.data.labels.length > data.limit) {
        chart.data.labels.splice(0, 1);
        chart.data.datasets.forEach((dataset) => {
            dataset.data.splice(0, 1);
        });
        chart.update();
    }
});

// Start the connection.
start().then(() => { });
