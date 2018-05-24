const GeoTIFF = require("geotiff");
const fs = require("fs");

// fs.readFile('C:\\Users\\User\\Downloads\\LC08_L1TP_179021_20170208_20170216_01_T1\\LC08_L1TP_179021_20170208_20170216_01_T1_B1.TIF', (err, data)=> {
//     if (err) throw err;
//     const dataArray = data.buffer.slice(data.byteOffset, data.byteOffset + data.byteLength);
//     const tiff = GeoTIFF.parse(dataArray);
//
//     const image = tiff.getImage();
//     const a =image.getFileDirectory();
//     const b =image.getGeoKeys();
//     console.log(image.getFileDirectory(), );
//
//     // const rasterWindow = [50, 50, 100, 100]; // left, top, right, bottom
//     // const samples = [0, 1, 2, 3];
//     // var rasters = image.readRasters({window: rasterWindow, samples: samples});
//     // for (var i = 0; i < rasters.length; ++i) {
//     //     console.log(rasters[i]);
//     // }
//
//     const rasters = image.readRasters();
//     var array = image.readRasters({interleave: true});
//     console.log();
//
// });

var gdal = require("gdal");
var dataset = gdal.open("C:\\Users\\User\\Downloads\\2015-07-22\\LC08_L1TP_178029_20150722_20170406_01_T1_B5.TIF");
//var dataset = gdal.open("C:\\Users\\User\\Downloads\\2015-07-22\\LC08_L1TP_178029_20150722_20170406_01_T1_MTL.txt");

console.log("number of bands: " + dataset.bands.count());



// geoTransform[0] /* координата x верхнего левого угла */
// geoTransform[1] /* ширина пиксела */
// geoTransform[2] /* поворот, 0, если изображение ориентировано на север */
// geoTransform[3] /* координата y верхнего левого угла */
// geoTransform[4] /* поворот, 0, если изображение ориентировано на север */
// geoTransform[5] /* высота пиксела */


const abc = dataset.driver.getMetadata();
const dataset1 = dataset.driver.createCopy('C:\\Users\\User\\Downloads\\2015-07-22\\123.TIF', dataset);

const gt1 = dataset.geoTransform;
const rotation = (Math.PI / 180) * -66;
const newGeotransform = [gt1[0],
    Math.cos(rotation) * gt1[1],
    -Math.sin(rotation) * gt1[1],
    gt1[3],
    Math.sin(rotation) * gt1[5],
    Math.cos(rotation) * gt1[5]];

dataset1.geoTransform = newGeotransform;
//dataset1.buildOverviews('CUBIC', newGeotransform);
dataset1.flush();
dataset1.close();
console.log("width: " + dataset.rasterSize.x);
console.log("height: " + dataset.rasterSize.y);
console.log("geotransform: " + dataset.geoTransform);
console.log("srs: " + (dataset.srs ? dataset.srs.toWKT() : 'null'));


// var mapnikOmnivore = require('@mapbox/mapnik-omnivore'),
//
// mapnikOmnivore.digest(file, function(err, metadata){
//     if (err) return callback(err);
//     else {
//         console.log('Metadata returned!');
//         console.log(metadata);
//     }
// });



