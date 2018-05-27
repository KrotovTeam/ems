ymaps.ready(init);


let Map = {};

Map.points = [];

function getPoints() {
    return [{
        Latitude: Map.points[0].geometry._coordinates[0],
        Longitude: Map.points[0].geometry._coordinates[1]
    }, {
        Latitude: Map.points[1].geometry._coordinates[0],
        Longitude: Map.points[1].geometry._coordinates[1]
    }];
};

Map.init = function () {
    Map.map = new ymaps.Map("map", {
        center: [44.547521, 33.705542],
        zoom: 10
    });
    Map.map.events.add('click', function (e) {
        if (Map.points.length < 2) {
            var coords = e.get('coords');
            var placemark = new ymaps.Placemark([coords[0], coords[1]], {
                preset: 'islands#icon',
                iconColor: '#0095b6'
            }, {
                draggable: true
            });

            placemark.events.add('click', function () {
                Map.map.geoObjects.remove(placemark);
                var index = Map.points.findIndex(obj => obj.geometry._coordinates[0] === placemark.geometry._coordinates[0] &&
                    obj.geometry._coordinates[1] === placemark.geometry._coordinates[1]);
                Map.points.splice(index, 1);
                if (Map.points.length === 1) {
                    $("#map").trigger("removeRectangle");
                }
            });

            placemark.events.add("dragend", function () {
                Map.map.geoObjects.remove(Map.rectangle);
                $("#map").trigger("addRectangle");
            });

            Map.map.geoObjects.add(placemark);
            Map.points.push(placemark);
            if (Map.points.length === 2) {
                $("#map").trigger("addRectangle");
            }
        }
    });


};


function init() {

    Map.map = new ymaps.Map("map", {
        center: [44.547521, 33.705542],
        zoom: 10
    });

    Map.map.setType('yandex#satellite');

    Map.map.events.add('click', function (e) {
        if (Map.points.length < 2) {
            var coords = e.get('coords');
            var placemark = new ymaps.Placemark([coords[0], coords[1]], {
                preset: 'islands#icon',
                iconColor: '#0095b6'
            }, {
                draggable: true
            });

            placemark.events.add('click', function () {
                Map.map.geoObjects.remove(placemark);
                var index = Map.points.findIndex(obj => obj.geometry._coordinates[0] === placemark.geometry._coordinates[0] &&
                    obj.geometry._coordinates[1] === placemark.geometry._coordinates[1]);
                Map.points.splice(index, 1);
                if (Map.points.length === 1) {
                    $("#map").trigger("removeRectangle");
                }
            });

            placemark.events.add("dragend", function () {
                Map.map.geoObjects.remove(Map.rectangle);
                $("#map").trigger("addRectangle");
            });

            Map.map.geoObjects.add(placemark);
            Map.points.push(placemark);
            if (Map.points.length === 2) {
                $("#map").trigger("addRectangle");
            }
        }
    });

    $("#map").on("addRectangle", function () {
        let coordPoint1 = Map.points[0].geometry._coordinates;
        let coordPoint2 = Map.points[1].geometry._coordinates;

        const arrLat = [coordPoint1[0], coordPoint2[0]].sort();
        const arrLot = [coordPoint1[1], coordPoint2[1]].sort();



        Map.rectangle = new ymaps.Rectangle([coordPoint1, coordPoint2],
            {
                strokeColor: '#0000FF',
                strokeWidth: 2
            });
        $("#ex1").attr("value", arrLat[0]);
        $("#ex2").attr("value", arrLot[0]);
        $("#ex3").attr("value", arrLat[1]);
        $("#ex4").attr("value", arrLot[1]);


        Map.map.geoObjects.add(Map.rectangle);
    });

    $("#map").on("removeRectangle", function () {
        Map.map.geoObjects.remove(Map.rectangle);
    });
}

// function init() {
//     var myMap = new ymaps.Map('map', {
//         center: [46.97, 33.82],
//         zoom: 9
//     }, {
//         searchControlProvider: 'yandex#search'
//     });
//     myMap.setType('yandex#satellite');
//
//
//     myMap.events.add('click', function (e) {
//
//         var coords = e.get('coords');
//         console.log(coords);
//     });
//
//
//     // Создаем многоугольник без вершин.
//     var myPolygon = new ymaps.Polygon([], {}, {
//         // Курсор в режиме добавления новых вершин.
//         editorDrawingCursor: 'crosshair',
//         // Максимально допустимое количество вершин.
//         editorMaxPoints: 5,
//         // Цвет заливки.
//         fillColor: '#00FF00',
//         // Цвет обводки.
//         strokeColor: '#0000FF',
//         // Ширина обводки.
//         strokeWidth: 4,
//         fill: false
//     });
//     // Добавляем многоугольник на карту.
//     myMap.geoObjects.add(myPolygon);
//
//     // В режиме добавления новых вершин меняем цвет обводки многоугольника.
//     var stateMonitor = new ymaps.Monitor(myPolygon.editor.state);
//     stateMonitor.add("drawing", function (newValue) {
//         myPolygon.options.set("strokeColor", newValue ? '#FF0000' : '#0000FF');
//     });
//
//
//
//     myPolygon.events.add([
//         'geometrychange'
//     ], function (e) {
//         const arr = e.originalEvent.originalEvent.originalEvent.newCoordinates[0];
//         console.log(arr);
//
//         if (arr && arr.length === 5) {
//             let needPoints = getVertex(arr);
//
//             $("#ex1").attr("value", needPoints.lowerLeft[0]);
//             $("#ex2").attr("value", needPoints.lowerLeft[1]);
//             $("#ex3").attr("value", needPoints.upperRight[0]);
//             $("#ex4").attr("value", needPoints.upperRight[1]);
//
//             // arr[2][0] = arr[2][0] + 1;
//             // myPolygon.geometry.set(1, [45, 30]);
//             // console.log();
//         }
//     });
//
//
//     // Включаем режим редактирования с возможностью добавления новых вершин.
//     myPolygon.editor.startDrawing();
//     setInterval(function () {
//         console.log(myPolygon.geometry.getBounds());
//     }, 5000);
// }


function getVertex(arr) {
    const points = arr.slice(1);
    const vartex = {
        lowerLeft: 0,
        upperLeft: 0,
        lowerRight: 0,
        upperRight: 0
    };

    // Находим самые верхние b нижние точки
    const pointUpperLower = points.sort((a, b) => {
        if (a[0] < b[0]) {
            return -1;
        }
        if (a[0] > b[0]) {
            return 1;
        }
        // a должно быть равным b
        return 0;
    });

    const vertexUpper = pointUpperLower.slice(2);
    const vertexLower = pointUpperLower.slice(0, 2);


    // Находим самые левые и правые точки
    const pointRightLeft = points.sort((a, b) => {
        if (a[1] < b[1]) {
            return -1;
        }
        if (a[1] > b[1]) {
            return 1;
        }
        // a должно быть равным b
        return 0;
    });

    const vertexRight = pointRightLeft.slice(2);
    const vertexLeft = pointRightLeft.slice(0, 2);


    vartex.lowerLeft = _findSameVertex(vertexLower, vertexLeft);
    vartex.upperLeft = _findSameVertex(vertexUpper, vertexLeft);
    vartex.lowerRight = _findSameVertex(vertexLower, vertexRight);
    vartex.upperRight = _findSameVertex(vertexUpper, vertexRight);

    return vartex;
}

function _findSameVertex(arr1, arr2) {
    for (let i = 0; i < arr1.length; i++) {
        for (let j = 0; j < arr2.length; j++) {
            if (arr1[i][0] === arr2[j][0] && arr1[i][1] === arr2[j][1]) {
                return arr1[i];
            }
        }
    }

}


function getAnglePosition(points) {
    const returnObj = {
        lowerLeft: null,
        // upperLeft: null,
        // lowerRight: null,
        upperRight: null
    };

    let lowerLeft = points[0];
    let upperRight = points[0];
    for (let i = 1; i < points.length; i++) {
        if (lowerLeft[0] > points[i][0] && lowerLeft[1] > points[i][1]) {
            lowerLeft = points[i];
        }
        if (upperRight[0] < points[i][0] && upperRight[1] < points[i][1]) {
            upperRight = points[i];
        }

    }

    returnObj.lowerLeft = lowerLeft;
    returnObj.upperRight = upperRight;

    return returnObj;
}

Date.prototype.toDateInputValue = (function () {
    var local = new Date(this);
    local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
    return local.toJSON().slice(0, 10);
});
$(document).ready(function () {
    // Устанавливаем текущую дату
    $('#dateEnd').val(new Date().toDateInputValue());
});
