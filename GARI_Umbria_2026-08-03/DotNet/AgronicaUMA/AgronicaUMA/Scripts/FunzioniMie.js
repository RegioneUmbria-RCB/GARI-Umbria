function uniqueID() {
    var today = new Date();
    return "ID"+today.getgetMinutes + "_" + today.getSeconds + "_" + today.getMilliseconds;
}
 
function roundNumber(num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}
