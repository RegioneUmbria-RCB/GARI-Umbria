function createChartPrelievi(dataChart) {
    var data = JSON.parse(dataChart);

    var series = prepareSeries(data.yValues);
    var chartRef = $("#chart").getKendoChart();

    if (chartRef !== undefined) {
        chartRef.destroy();
    }

        $("#chart").kendoChart({
            title: {
                text: data.title
            },
            legend: {
                position: "bottom"
            },
            seriesDefaults: {
                type: "line",
                stack: false
            },
            series: series,
            valueAxis: {
                line: {
                    visible: false
                },
                title: {
                    text: "m3/Ha"
                }
            },
            categoryAxis: {
                title: {
                    text: "Settimane"
                },
                categories: prepareCategories(data.xValues,data.xValueDateType),
                majorGridLines: {
                    visible: false
                }
            },
            tooltip: {
                visible: true,
                format: "{0}"
            }
        });
}

function createChartVolume(dataChart) {
    var data = JSON.parse(dataChart);

    var series = prepareSeries(data.yValues);
    var chartRef = $("#chart-vol").getKendoChart();

    if (chartRef !== undefined) {
        chartRef.destroy();
    }

    $("#chart-vol").kendoChart({
        title: {
            text: data.title
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "line",
            stack: false
        },
        series: series,
        valueAxis: {
            line: {
                visible: false
            },
            title: {
                text: "m3"
            }
        },
        categoryAxis: {
            title: {
                text: "Settimane"
            },
            categories: prepareCategories(data.xValues, data.xValueDateType),
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: "{0}"
        }
    });
}


function prepareSeries(yValues, xValueDateType) {
    var series = [];
    if (xValueDateType === true) {
        for (i = 0; i < yValues.length; i++) {
            series.push({ name: yValues[i].name, data: yValues[i].data, markers: { type: "square" }, color: yValues[i].color });
        }
    } else {
        for (i = 0; i < yValues.length; i++) {
            series.push({ name: yValues[i].name, dashType: yValues[i].dashType , data: yValues[i].data, markers: { visible: false }, color: yValues[i].color });
        }
    }
    
    return series;
}

function prepareCategories(xValues,xValueDateType) {
    if (xValueDateType === true) {
        var newValues = [];
        for (i = 0; i < xValues.length; i++) {
            newValues.push(new Date(xValues[i]));
        }
        return newValues;
    } else {
        return xValues;
    }
}

function createWaterChart(dataChart, idcomizio) {
    var data = JSON.parse(dataChart);

    var series = prepareSeries(data.yValues, data.xValuesDateType);
    var chartRef = $("#detailChart").getKendoChart();

    if (chartRef !== undefined) {
        chartRef.destroy();
    }

        $("#detailChart").kendoChart({
            title: {
                text: data.title + " comizio " + idcomizio
            },
            legend: {
                position: "bottom"
            },
            seriesDefaults: {
                type: "column",
                stack: true
            },
            series: series,
            valueAxis: {
                line: {
                    visible: false
                },
                title: {
                    text: "m3"
                }
            },
            categoryAxis: {
                categories: prepareCategories(data.xValues, data.xValuesDateType),
                majorGridLines: {
                    visible: false
                }
            },
            tooltip: {
                visible: true,
                format: "{0}"
            }
        });
}

function createVolumeChart(dataChart, idcomizio) {

}

function createPressureChart(dataChart,idcomizio) {
    var data = JSON.parse(dataChart);

    var series = prepareSeries(data.yValues, data.xValuesDateType);
    var chartRef = $("#detailChart").getKendoChart();

    if (chartRef !== undefined) {
        chartRef.destroy();
    }


        $("#detailChart").kendoChart({
            title: {
                text: data.title + " comizio " + idcomizio
            },
            legend: {
                position: "bottom"
            },
            seriesDefaults: {
                type: "line",
                stack: true
            },
            series: series,
            valueAxis: {
                line: {
                    visible: false
                },
                title: {
                    text: "bar"
                }
            },
            categoryAxis: {
                categories: prepareCategories(data.xValues, data.xValuesDateType),
                labels: { rotation: 90 },
                majorGridLines: {
                    visible: false
                }
            },
            tooltip: {
                visible: true,
                format: "{0}"
            }
        });
}