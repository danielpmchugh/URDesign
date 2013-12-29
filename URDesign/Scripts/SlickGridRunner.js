var dataView;
var grid;
var data = [];
var options = {
    enableCellNavigation: true,
    showHeaderRow: true,
    headerRowHeight: 30,
    rowHeight: 60,
    explicitInitialization: true,
    forceFitColumns: true
};
var columns = [];
var columnFilters = {};

var columns = [
            { id: 0, name: "Name", field: "Name", maxWidth: 160, minWidth: 160, width: 160 },
            { id: 1, name: "Vendor", field: "Vendor", maxWidth: 100, minWidth: 100, width: 100 },
            {
                id: 2, name: "Image", field: "ImagePath",
                formatter: linkFormatter = function (row, cell, value, columnDef, dataContext) {
                    return '<img src=' + value + '>';
                }, minWidth: 280, maxWidth: 280, width: 280

            },
            { id: 3, name: "Price", field: "Price", minWidth: 60, minWidth: 60, width: 60 },
            { id: 4, field: 'DrawingFile', name: 'DrawingFile', minWidth: 90, minWidth: 90, width: 90, formatter: buttonFormatter },
            {

                id: 5, name: "Update", field: "IdFurniture",
                formatter: linkFormatter = function (row, cell, value, columnDef, dataContext) {
                    return '<a href="/Furniture/Edit?id=' + value + '">Edit</a>' +
                        ' | <a href="/Furniture/Details?id=' + value + '">Details</a>' +
                        ' | <a href="/Furniture/Delete?id=' + value + '">Delete</a>'
                }, minWidth: 170, maxWidth: 170, width: 170

            }
];


function filter(item) {
    for (var columnId in columnFilters) {
        if (columnId !== undefined && columnFilters[columnId] !== "") {
            var c = grid.getColumns()[grid.getColumnIndex(columnId)];
            if(String(item[c.field]).indexOf(String(columnFilters[columnId])) == -1)
            {
                return false;
            }
        }
    }
    return true;
}
//Now define your buttonFormatter function
function buttonFormatter(row, cell, value, columnDef, dataContext) {
    if (dataContext.DrawingFile != null) {
        var button = "<input class='DrawingFile' type='button' id='" + dataContext.IdFurniture + "' value=" + dataContext.DrawingFile + " />";
        //the id is so that you can identify the row when the particular button is clicked
        return button;
    }
    else {
        var button = "<input class='DrawingFile' type='button' style='visibility:hidden' id='" + dataContext.IdFurniture + "' value=" + dataContext.DrawingFile + "disabled />";
        //the id is so that you can identify the row when the particular button is clicked
        return button;
    }
    return null;
    //Now the row will display your button
}

$(function () {
    $.getJSON('/Furniture/ArsenalX11', function (indata) {
        data = indata;

        dataView = new Slick.Data.DataView();
        grid = new Slick.Grid("#teamGrid", dataView, columns, options);

        grid.onClick.subscribe(function (e, args) {
            if (args.grid.getColumns()[args.cell].field == 'DrawingFile') {
                if (args.grid.getDataItem(args.row).DrawingFile != null) {
                    var loc = '/Furniture/Download?id=' + args.grid.getDataItem(args.row).IdFurniture;
                    window.location.replace(loc);
                }
            }
        });


        dataView.onRowCountChanged.subscribe(function (e, args) {
            grid.updateRowCount();
            grid.render();
        });

        dataView.onRowsChanged.subscribe(function (e, args) {
            grid.invalidateRows(args.rows);
            grid.render();
        });


        $(grid.getHeaderRow()).delegate(":input", "change keyup", function (e) {
            var columnId = $(this).data("columnId");
            if (columnId != null) {
                columnFilters[columnId] = $.trim($(this).val());
                dataView.refresh();
            }
        });

        grid.onHeaderRowCellRendered.subscribe(function (e, args) {
            $(args.node).empty();
            $("<input type='text'>")
               .data("columnId", args.column.id)
               .val(columnFilters[args.column.id])
               .appendTo(args.node);
        });

        grid.init();

        dataView.beginUpdate();
        dataView.setItems(data, "IdFurniture");
        dataView.setFilter(filter);
        dataView.endUpdate();
    });
})
