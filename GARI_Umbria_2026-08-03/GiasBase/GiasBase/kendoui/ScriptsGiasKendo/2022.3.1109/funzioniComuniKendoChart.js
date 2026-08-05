
/**
* Inizializza il grafico con i parametri passati
*
* @param {dati} data Dati in formato kendo chart
* @param {string} divchart JQuery Selector per il div del grafico
* @param {string} group Proprietà dataSource: Group
* @param {string} category Proprietà series: Category Field
* @param {string} format formato etichetta asse ordinate
* @param {string} type tipo di grafico (lines, column..)
* @param {boolean} sort indica se occorre ordinare i valori sull'asse delle categorie (orizzontale)
*
*/
function initChart(data, divchart, group, category, format, type, sort, tooltip, theme) {

    var chart = $(divchart).data("kendoChart");

    if (type === undefined || type === null)
        type = "column";

    if (theme === undefined || theme === null)
        theme = "bootstrap";

    if (tooltip === undefined || tooltip === null)
        tooltip = { visible: false };

    chart = $(divchart).kendoChart({
        theme: theme,
        dataSource: {
            data: data,
            group: group
        },
        series: [{
            type: type,
            field: "measure",
            name: "#= group.value #",
            categoryField: category
        }],
        legend: {
            position: "bottom"
        },
        categoryAxis: {
            labels: {
                rotation: 310,
                dateFormats: {
                    days: "d/M/yyyy"
                }

            }
        },
        valueAxis: {
            labels: {
                format: format
            }
        },
        dataBound: function (e) {
            var categoryAxis = e.sender.options.categoryAxis;
            if (categoryAxis && categoryAxis.categories && sort) {
                categoryAxis.categories.sort();
            }
        },
        tooltip: tooltip
    });

    return chart;

}


//gather the collapsed members
function collapseMember(e) {
    var axis = collapsed[e.axis];
    var path = e.path;

    if (indexOfPath(path, axis) === -1) {
        axis.push(path);
    }
}

//gather the expanded members
function expandMember(e) {
    var axis = collapsed[e.axis];
    var index = indexOfPath(e.path, axis);

    if (index !== -1) {
        axis.splice(index, 1);
    }
}

//function flatten the tree of tuples that datasource returns
function flattenTree(tuples) {
    tuples = tuples.slice();
    var result = [];
    var tuple = tuples.shift();
    var idx, length, spliceIndex, children, member;

    while (tuple) {
        //required for multiple measures
        if (tuple.dataIndex !== undefined) {
            result.push(tuple);
        }

        spliceIndex = 0;
        for (idx = 0, length = tuple.members.length; idx < length; idx++) {
            member = tuple.members[idx];
            children = member.children;
            if (member.measure) {
                [].splice.apply(tuples, [0, 0].concat(expandMeasures(children, tuple)));
            } else {
                [].splice.apply(tuples, [spliceIndex, 0].concat(children));
            }
            spliceIndex += children.length;
        }

        tuple = tuples.shift();
    }

    return result;
}

function clone(tuple, dataIndex) {
    var members = tuple.members.slice();

    return {
        dataIndex: dataIndex,
        members: $.map(members, function (m) {
            return $.extend({}, m, { children: [] });
        })
    };
}

function replaceLastMember(tuple, member) {
    tuple.members[tuple.members.length - 1] = member;
    return tuple;
};

function expandMeasures(measures, tuple) {
    return $.map(measures, function (measure) {
        return replaceLastMember(clone(tuple, measure.dataIndex), measure);
    });
}

//Check whether the tuple has been collapsed
function isCollapsed(tuple, collapsed) {
    var members = tuple.members;

    for (var idx = 0, length = collapsed.length; idx < length; idx++) {
        var collapsedPath = collapsed[idx];
        if (indexOfPath(fullPath(members, collapsedPath.length - 1), [collapsedPath]) !== -1) {
            return true;
        }
    }

    return false;
}

//Work with tuple names/captions
function indexOfPath(path, paths) {
    var path = path.join(",");

    for (var idx = 0; idx < paths.length; idx++) {
        if (paths[idx].join(",") === path) {
            return idx;
        }
    }

    return -1;
}

function fullPath(members, idx) {
    var path = [];
    var parentName = members[idx].parentName;

    idx -= 1;

    while (idx > -1) {
        path.push(members[idx].name);
        idx -= 1;
    }

    path.push(parentName);

    return path;
}

function memberCaption(member) { return member.caption };

function extractCaption(members) {
    return $.map(members, memberCaption).join(" ");
};

function fullPathCaptionName(members, dLength, idx) {
    var result = extractCaption(members.slice(0, idx + 1));
    var measureName = extractCaption(members.slice(dLength, members.mLength));

    if (measureName) {
        result += " - " + measureName;
    }

    return result;
}

//the main function that convert PivotDataSource data into understandable for the Chart widget format
function convertData(dataSource, collapsed) {
    var columnDimensionsLength = dataSource.columns().length;
    var rowDimensionsLength = dataSource.rows().length;

    var columnTuples = flattenTree(dataSource.axes().columns.tuples || []/*, collapsed.columns*/);
    var rowTuples = flattenTree(dataSource.axes().rows.tuples || []/*, collapsed.rows*/);
    var data = dataSource.data();
    var rowTuple, columnTuple;

    var idx = 0;
    var result = [];
    var columnsLength = columnTuples.length;

    for (var i = 0; i < rowTuples.length; i++) {
        rowTuple = rowTuples[i];

        if (!isCollapsed(rowTuple, collapsed.rows)) {
            for (var j = 0; j < columnsLength; j++) {
                columnTuple = columnTuples[j];

                if (!isCollapsed(columnTuple, collapsed.columns)) {
                    if (idx > columnsLength && idx % columnsLength !== 0) {
                        for (var ri = 0; ri < rowTuple.members.length; ri++) {
                            for (var ci = 0; ci < columnTuple.members.length; ci++) {
                                //do not add root tuple members, e.g. [CY 2005, All Employees]
                                //Add only children
                                if (!columnTuple.members[ci].parentName || !rowTuple.members[ri].parentName) {
                                    continue;
                                }

                                result.push({
                                    measure: Number(data[idx].value),
                                    column: fullPathCaptionName(columnTuple.members, columnDimensionsLength, ci),
                                    row: fullPathCaptionName(rowTuple.members, rowDimensionsLength, ri)
                                });
                            }
                        }
                    }
                }
                idx += 1;
            }
        }
    }

    return result;
}

// funzione per esportare grafico in pdf, png e svg
function exportChart(divchart, type) {
    var chart = $(divchart).getKendoChart();
    if (type === "pdf") {
        chart.exportPDF({ paperSize: "auto", margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" } }).done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "chart.pdf"
            });
        });
    } else if (type === "img") {
        chart.exportImage().done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "chart.png"
            });
        });
    } else if (type === "svg") {
        chart.exportSVG().done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "chart.svg"
            });
        });
    }
}
