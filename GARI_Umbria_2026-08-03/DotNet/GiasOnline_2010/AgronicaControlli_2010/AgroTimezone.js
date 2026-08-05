var AGRO_JS_TIMEZONE =
{
    Common: class {
        static LocalStorageProcessableDatasKey = "AGRO_JS_TIMEZONE_PROCESSABLE_DATA_CACHE";
        static LocalStorageKey = "AGRO_JS_TIMEZONE_CACHE";
        static MaxStorageKeys = 30000;
        static UseCache = true;
    },

    AGRO_JS_TIMEZONE_CACHE_ITEM: class {

        constructor(parsedDate, dateConverted, hasTime) {
            this.ParsedDate = parsedDate;
            this.DateConverted = dateConverted;
            this.HasTime = hasTime;
        }
    },

    TZ_CUSTOME_REQUEST_HEADER: class {

        constructor(clientTimeZoneId, clientCountryCode, utcOffset) {
            this.ClientTimeZoneId = clientTimeZoneId;
            this.ClientCountryCode = clientCountryCode;
            this.UTCOffset = utcOffset;
        }
    }

};



var TimeZoneCacheFactory = (function () {

    AGRO_JS_TIMEZONE.Cache = function () {

        var dictionaryCache = new Object();

         let sessionCache = localStorage.getItem(AGRO_JS_TIMEZONE.Common.LocalStorageKey);
         if (sessionCache !== null && sessionCache != undefined) {
             dictionaryCache = JSON.parse(sessionCache);
         }
        this.CACHE = dictionaryCache;

        this.tryGetFromCache = function (key) {

            if (!AGRO_JS_TIMEZONE.Common.UseCache)
                return undefined;

            if (dictionaryCache.hasOwnProperty(key)) {
                return dictionaryCache[key];
            } else {
                return undefined;
            }
        };

        this.tyrPutInCache = function (key, value) {

            if (!AGRO_JS_TIMEZONE.Common.UseCache)
                return;

            if (!dictionaryCache.hasOwnProperty(key)) 
                dictionaryCache[key] = value;
        }

    }

    var instance = null;

    return {
        getInstance: function () {
            if (!instance) {

                $(window).bind('beforeunload', function () {

                    // quando viene scaricata la pagina (quindi viene distrutta l'istanza corrente) salvo la cache in localStorage
                    // oppure la distruggo (svuoto) se le chiavi memorizzate superano il limite imposto da costante
                    if (Object.keys(instance.CACHE).length > AGRO_JS_TIMEZONE.Common.MaxStorageKeys)
                        localStorage.removeItem(AGRO_JS_TIMEZONE.Common.LocalStorageKey);
                    else {
                        var sessionCache = JSON.stringify(instance.CACHE);
                        localStorage.setItem(AGRO_JS_TIMEZONE.Common.LocalStorageKey, sessionCache);
                    }
                });

                instance = new AGRO_JS_TIMEZONE.Cache();
                delete instance.constructor;
            }
            return instance;
        }
    };
})();

var ProcessableDataCacheFactory = (function () {

    AGRO_JS_TIMEZONE.ProcessableDataCache = function () {

        var processableDataCache = new Object();

        let sessionCache = localStorage.getItem(AGRO_JS_TIMEZONE.Common.LocalStorageProcessableDatasKey);
        if (sessionCache !== null && sessionCache != undefined) {
            processableDataCache = JSON.parse(sessionCache);
        }
        this.CACHE = processableDataCache;

        this.tryGetFromCache = function (key) {

            if (!AGRO_JS_TIMEZONE.Common.UseCache)
                return undefined;

            if (processableDataCache.hasOwnProperty(key)) {
                return processableDataCache[key];
            } else {
                return undefined;
            }
        };

        this.tyrPutInCache = function (key, value) {

            if (!AGRO_JS_TIMEZONE.Common.UseCache)
                return;

            if (!processableDataCache.hasOwnProperty(key))
                processableDataCache[key] = value;
        }

    }

    var instance = null;

    return {
        getInstance: function () {
            if (!instance) {

                $(window).bind('beforeunload', function () {

                    // quando viene scaricata la pagina (quindi viene distrutta l'istanza corrente) salvo la cache in localStorage
                    // oppure la distruggo (svuoto) se le chiavi memorizzate superano il limite imposto da costante
                    if (Object.keys(instance.CACHE).length > AGRO_JS_TIMEZONE.Common.MaxStorageKeys)
                        localStorage.removeItem(AGRO_JS_TIMEZONE.Common.LocalStorageProcessableDatasKey);
                    else {
                        var sessionCache = JSON.stringify(instance.CACHE);
                        localStorage.setItem(AGRO_JS_TIMEZONE.Common.LocalStorageProcessableDatasKey, sessionCache);
                    }
                });

                instance = new AGRO_JS_TIMEZONE.Cache();
                delete instance.constructor;
            }
            return instance;
        }
    };
})();

AGRO_JS_TIMEZONE.Data_Conversion_Service = function () {
       

    // variabili private
    const AGRO_TS_DATAINIZIO = new Date(1900, 0, 1, 0, 0, 0, 0);
    const AGRO_TS_DATAFINE = new Date(2100, 11, 31, 0, 0, 0, 0);
    var cacheManager = TimeZoneCacheFactory.getInstance();
    var locale_date_time_format = null;

    const enum_Data_Direction = {
        "FROM_SERVER_TO_CLIENT": 1,
        "FROM_CLIENT_TO_SERVER": 2
    };

    // variabili pubbliche
    this.AGRO_DATA_INIZIO = AGRO_TS_DATAINIZIO;
    this.AGRO_DATA_FINE = AGRO_TS_DATAFINE;
    this.CACHE = cacheManager.CACHE;
    this.LOCALE_DATE_TIME_FORMAT = null;

    var zip = function* (objectData, direction, arrays) {
        let iterators = arrays.map(a => a[Symbol.iterator]());
        while (true) {
            let results = iterators.map(it => it.next());
            if (results.some(r => r.done)) return;
            yield results.map((r) => {
                let risultato = null;
                if (direction === enum_Data_Direction.FROM_SERVER_TO_CLIENT)
                    risultato = trasformServerData(objectData[r.value], r.value);
                else
                    risultato = trasformClientData(objectData[r.value], r.value);
                objectData[r.value] = risultato;
            });
        }
    };

    var isJSON = function (str) {
        if (/^\s*$/.test(str)) return false;
        str = str.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@');
        str = str.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']');
        str = str.replace(/(?:^|:|,)(?:\s*\[)+/g, '');
        return (/^[\],:{}\s]*$/).test(str);
    };

    var serverTimeZoneIsDefined = function () {

        if (typeof SERVER_TIME_ZONE_ID != 'undefined' &&
            SERVER_TIME_ZONE_ID != undefined &&
            SERVER_TIME_ZONE_ID != null)
            return true;
        else
            return false;
    };

    var isIsoDate = function (stringDate) {

        if (!/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z/.test(stringDate))
            return false;
        var d = new Date(stringDate);
        return d instanceof Date && d.toISOString() === stringDate;

    };

    var getLocaleDateTimeFormat = function () {

        let countryCode = Intl.DateTimeFormat().resolvedOptions().locale;
        let dateFormat = moment.localeData(countryCode).longDateFormat("L");
        let timeFormat = moment.localeData(countryCode).longDateFormat("LTS").replace("A", "");
        return dateFormat + " " + timeFormat;
    };

    var handleAGRODATAINIZIOFromServer = function (date) {
        let millisecondsxday = 1000 * 60 * 60 * 24;
        let agro_inizio_top = new Date();
        agro_inizio_top.setTime(AGRO_TS_DATAINIZIO.getTime() + (millisecondsxday * 2));
        let agro_inizio_bottom = new Date();
        agro_inizio_bottom.setTime(AGRO_TS_DATAFINE.getTime() - (millisecondsxday * 2));

        if (date >= agro_inizio_bottom && date <= agro_inizio_top) {
            let d = new Date(1900, 0, 1, 0, 0, 0, 0);
            return d;
        }
        return date;
    };

    var adjustDateFromServer = function (convParams) {
        let serverTzMilliseconds = convParams.serverUtcOffset * 60 * 1000;
        let clientTzMilliseconds = convParams.clientUtcOffset * 60 * 1000;
        convParams.newData.setTime(convParams.newData.getTime() - (clientTzMilliseconds + serverTzMilliseconds));
        convParams.newData = handleAGRODATAINIZIOFromServer(convParams.newData);
    };

    var conversionParams = function (newDate, serverTz, serverUtcOffset, clientTz, clientUtcOffset) {
        this.originalData = newDate;
        this.newData = newDate;
        this.serverTz = serverTz;
        this.serverUtcOffset = serverUtcOffset;
        this.clientTz = clientTz;
        this.clientUtcOffset = clientUtcOffset;
    };

    var getConversionaParams = function (theDate) {
        let dateVal = theDate; //kendo.parseDate(theDate);
        let newData = dateVal;
        let serverTz = moment(dateVal).tz(SERVER_TIME_ZONE_ID);
        let serverUtcOffset = serverTz.utcOffset();
        let clientTz = moment.tz.guess();
        let clientUtcOffset = moment(dateVal).tz(clientTz).utcOffset();

        return new conversionParams(newData, serverTz, serverUtcOffset, clientTz, clientUtcOffset);
    };

    var splitToNChunks = function (array, n) {
        let result = [];
        for (let i = n; i > 0; i--) {
            result.push(array.splice(0, Math.ceil(array.length / i)));
        }
        return result;
    };

    var hasTimeValues = function (objectData) {

        return (objectData.getHours() + objectData.getMinutes() + objectData.getSeconds() + objectData.getMilliseconds()) > 0;
    };

    var isParseableField = function (objectData) {
        return typeof objectData === 'string';
    };


    var isADate = function (objectData, fieldName) {

        let parsedDate = null;

        if (typeof objectData == 'string' && (objectData === "" || objectData.trim() === ""))
            return null;

        try {

            if (typeof objectData == 'string' && (objectData).includes('Date(')) {
                parsedDate = kendo.parseDate(objectData);
            }
            else {
                let inertedObjectData = objectData.replace(/([0-9]+)\/([0-9]+)/, '$2/$1');
                if (isNaN(Date.parse(objectData)) && isNaN(Date.parse(inertedObjectData)))
                    return null;

                parsedDate = kendo.parseDate(objectData);
            }

        //    if(parsedDate !== null)
        //        console.log(" PROCESSABILE ->" + fieldName + " " + objectData);
        }
        catch (e) { }

        return parsedDate;
    };

    var convertServerData_Parallel = function (objectData) {

        const toRemove = new Set(['kendo_columns', 'kendo_model', '__type']);

        let props = null;
        let filteredPros = null;
        try {
            props = Object.getOwnPropertyNames(objectData);
            if (props !== undefined && props !== null && props.length > 0)
                filteredPros = props.filter(x => !toRemove.has(x));

        } catch (e) {
            return objectData;
        }

        //var chunks = splitToNChunks(filteredPros, 1);
        if (filteredPros !== null && filteredPros.length > 0) {
            var chunks = splitToNChunks(filteredPros, Math.round(filteredPros.length / 2));
            const resultGenerator = zip(objectData, enum_Data_Direction.FROM_SERVER_TO_CLIENT, ...[chunks]);
            for (let results of resultGenerator) { }
        }
        return objectData;

    };
    
    var trasformServerData = function (objectData, fieldName) {

        if (objectData === null || objectData === undefined)
            return objectData;

        if (typeof objectData == 'object') {
            objectData = convertServerData_Parallel(objectData);
        }

        if (!isParseableField(objectData))
            return objectData;

        let cachedItem = null;
        let parsedDate = null;
        let isValidMoment = false;
        let hasTime = false;

        cachedItem = cacheManager.tryGetFromCache(objectData);
        if (cachedItem === undefined) 
            parsedDate = isADate(objectData, fieldName);
        else 
            parsedDate = cachedItem.ParsedDate;

        if (parsedDate === null || parsedDate === undefined)
            return objectData;

        if (cachedItem === undefined)
            isValidMoment = moment(parsedDate, moment.ISO_8601, true).isValid();
        else
            isValidMoment = true;

        if (!isValidMoment)
            return objectData;

        if (cachedItem === undefined)
            hasTime = hasTimeValues(parsedDate);
        else
            hasTime = cachedItem.HasTime;

        if (isValidMoment && hasTime) {

            //console.log(" SERVER -> CLIENT PROCESSATO ->" + fieldName + " " + objectData);
            if (cachedItem !== undefined) {
                objectData = cachedItem.DateConverted;
                return objectData;
            }

            let convParams = getConversionaParams(parsedDate);
            let dateVal = convParams.originalData;

            // CASO 1 Oggetto serializzato direttamente da VB (rispostastandardof<T>)
            if (typeof objectData == 'string' && (objectData).includes('Date(')) {

                //if (dateVal != null && dateVal.getMilliseconds() != 0) {
                if (dateVal != null ) {
                    adjustDateFromServer(convParams);
                }
                dateVal = convParams.newData;

                if (dateVal != null) {

                    let formattedDate = moment(dateVal).format(locale_date_time_format);
                    let ci = new AGRO_JS_TIMEZONE.AGRO_JS_TIMEZONE_CACHE_ITEM(parsedDate, formattedDate, hasTime);
                    cacheManager.tyrPutInCache(objectData, ci);
                    objectData = formattedDate; // dateVal;
                }

            }
            else {

                // 2.1 Non CoreAPI
                //if (dateVal != null && (dateVal.getHours() != 0 || dateVal.getMinutes() != 0 || dateVal.getSeconds() != 0 || dateVal.getMilliseconds() != 0)) {
                if (dateVal != null ) {

                    if (convParams.serverUtcOffset != convParams.clientUtcOffset) {
                        adjustDateFromServer(convParams);
                    }
                    dateVal = convParams.newData;
                }

                // 2.2 da CoreAPI
                if (objectData.length > 20 && (objectData[19] == '+' || objectData[19] == '-')) {
                    if (convParams.serverUtcOffset != convParams.clientUtcOffset && convParams.originalData.getMilliseconds() == 0) {
                        adjustDateFromServer(convParams);
                    }
                    dateVal = convParams.newData;
                }

                if (dateVal != null) {

                    let formattedDate = moment(dateVal).format(locale_date_time_format);
                    let ci = new AGRO_JS_TIMEZONE.AGRO_JS_TIMEZONE_CACHE_ITEM(parsedDate, formattedDate, hasTime);
                    cacheManager.tyrPutInCache(objectData, ci);
                    objectData = formattedDate; // dateVal;
                }

                return objectData;
            }
        }
        else {
            if (cachedItem === undefined && isValidMoment) {
                let ci = new AGRO_JS_TIMEZONE.AGRO_JS_TIMEZONE_CACHE_ITEM(objectData, objectData, hasTime);
                cacheManager.tyrPutInCache(objectData, ci);
            }
        }

        return objectData;

    }

    var convertClientData_Parallel = function (objectData) {

        const toRemove = new Set(['kendo_columns', 'kendo_model']);

        let props;
        try {
            props = Object.getOwnPropertyNames(objectData);
            if (props !== undefined && props !== null && props.length > 0) {
                let myArray = props.filter(x => !toRemove.has(x));
            }
        } catch (e) {
            return stringData;
        }

        var chunks = splitToNChunks(props, 1);
        const resultGenerator = zip(objectData, enum_Data_Direction.FROM_CLIENT_TO_SERVER, ...[chunks]);
        for (let results of resultGenerator) { }
        return objectData;

    };

    var trasformClientData = function (objectData, fieldName) {

        if (objectData === null || objectData === undefined)
            return objectData;

        if (typeof objectData == 'object') {
            objectData = convertClientData_Parallel(objectData);
        }

        if (typeof objectData == 'string' && objectData.includes('Date('))
            objectData = kendo.parseDate(objectData);

        let parsedDate = isADate(objectData, fieldName);
        if (parsedDate === null)
            return objectData;

        if (typeof objectData == 'string') {

            if (objectData.substring(0, 4) == '0001')
                return objectData;

            let isValidMoment = moment(parsedDate, moment.ISO_8601, true).isValid() && hasTimeValues(parsedDate);
            if (!isValidMoment)
                return objectData;

            //console.log(" CLIENT -> SERVER PROCESSATO ->" + fieldName + " " + objectData);

            let data = new Date(objectData);
            if (data.getMilliseconds() == 0) {
                let year = ('' + data.getFullYear()).padStart(2, '0');
                let month = ('' + (data.getMonth() + 1)).padStart(2, '0');
                let day = ('' + data.getDate()).padStart(2, '0');
                let hours = ('' + data.getHours()).padStart(2, '0');
                let minutes = ('' + data.getMinutes()).padStart(2, '0');
                let seconds = ('' + data.getSeconds()).padStart(2, '0');
                let res = year + '-' + month + '-' + day + 'T' + hours + ':' + minutes + ':' + seconds;
                objectData = res;
            }
            else
                objectData = data.toISOString();

        }

        return objectData;

    }

    ///////////////////////////////////////////////////////////////////////////////////////
    //
    // FUNZIONI PUBBLICHE
    //
    ///////////////////////////////////////////////////////////////////////////////////////

    this.Init = function () {
        this.LOCALE_DATE_TIME_FORMAT = getLocaleDateTimeFormat().trim();
        locale_date_time_format = this.LOCALE_DATE_TIME_FORMAT;
    }

    // funzione che converte da CLIENT -> SERVER
    this.Manage_TZ_From_Client_Data = function (stringData) {

        if (AreClientServerTZEquals()) {
            return stringData;
        }

        if (stringData == null || stringData == undefined)
            return null;

        if (stringData == "")
            return stringData;

        let objectData = null;
        let isValidJSON = true;

        if (String(typeof (stringData)).toLowerCase() === 'string' && stringData != "" && isJSON(stringData)) {
            try {
                objectData = JSON.parse(stringData);
            }
            catch (e) {
                isValidJSON = false;
                objectData = stringData;
            }
        }
        else {
            objectData = stringData;
            isValidJSON = false;
        }

        let startTime = new Date();
        var risposta = null;
        try {
            var convertedObject = convertClientData_Parallel(objectData);
            if (isValidJSON)
                risposta = JSON.stringify(convertedObject);
            else
                risposta = convertedObject;
        }
        catch (ex) {
            risposta = stringData;
            console.log(ex);
        }

        /*
         
        ------------------------- LOGGING FOR DEBUG ----------------------------------
        
        let endTime = new Date();
        let timeElapsed = endTime - startTime;

        console.log("------------------ CLIENT -> SERVER -----------------------");
        console.log("   Total time elapsed times: " + timeElapsed + " milliseconds.");
        var bytes = new TextEncoder().encode(JSON.stringify(cacheManager.CACHE)).length
        console.log(Object.keys(cacheManager.CACHE).length);
        console.log("   Size of cache = " + bytes);
        bytes = new TextEncoder().encode(risposta).length;
        console.log("   Size of response = " + bytes);
        console.log("-----------------------------------------------------------");

        ------------------------- LOGGING FOR DEBUG ----------------------------------

        */

        return risposta;

    };

    // Funzione che converte da SERVER _> CLIENT
    this.Manage_TZ_From_Server_Data = function (stringData) {

        if (AreClientServerTZEquals()) {
            return stringData;
        }

        if (stringData == null || stringData == undefined) {
            return null;
        }

        let objectData = null;
        let isValidJSON = true;

        if (String(typeof (stringData)).toLowerCase() === 'string' && stringData != "" && isJSON(stringData)) {
            try {
                objectData = JSON.parse(stringData);
            }
            catch (e) {
                isValidJSON = false;
                objectData = stringData;
            }
        }
        else {
            objectData = stringData;
            isValidJSON = false;
        }
                

        let startTime = new Date();
        var risposta = null;
        try {
            var convertedObject = convertServerData_Parallel(objectData);
            if (isValidJSON)
                risposta = JSON.stringify(convertedObject);
            else
                risposta = convertedObject;
        }
        catch (ex) {
            risposta = stringData;
            console.log(ex);
        }

        let endTime = new Date();
        let timeElapsed = endTime - startTime;

        console.log("------------------ SERVER -> CLIENT -----------------------");
        console.log("   Total time elapsed times: " + timeElapsed + " milliseconds.");


        /*
        
        ------------------------- LOGGING FOR DEBUG -----------------------------------
        
        let endTime = new Date();
        let timeElapsed = endTime - startTime;

        console.log("------------------ SERVER -> CLIENT -----------------------");
        console.log("   Total time elapsed times: " + timeElapsed + " milliseconds.");
        var bytes = new TextEncoder().encode(JSON.stringify(cacheManager.CACHE)).length
        console.log(Object.keys(cacheManager.CACHE).length);
        console.log("   Size of cache = " + bytes);
        bytes = new TextEncoder().encode(risposta).length;
        console.log("   Size of response = " + bytes);
        console.log("-----------------------------------------------------------");

        ------------------------- LOGGING FOR DEBUG -----------------------------------

        */

        return risposta;

    };

    this.CLIENT_TIME_ZONE_INFO = function () {

        if (typeof moment === 'undefined' || moment === undefined || moment === null)
            return null;

        let clientTz = moment.tz.guess();
        let countryCode = Intl.DateTimeFormat().resolvedOptions().locale;
        return new AGRO_JS_TIMEZONE.TZ_CUSTOME_REQUEST_HEADER(clientTz, countryCode, moment(new Date()).tz(clientTz).utcOffset());

    };

    this.AreClientServerTZEquals = function () {
        let CLIENT_TIME_ZONE_ID = null;
        let areClientServerTZEquals = true;

        // se la server time zone non è definita mi comporto come se fosse lo stesso TZ 
        if (!serverTimeZoneIsDefined())
            return true;

        if (typeof moment != 'undefined' && moment != undefined && moment != null)
            CLIENT_TIME_ZONE_ID = moment.tz.guess();

        if (serverTimeZoneIsDefined()  && CLIENT_TIME_ZONE_ID != null)
            areClientServerTZEquals = (CLIENT_TIME_ZONE_ID == SERVER_TIME_ZONE_ID);

        return areClientServerTZEquals;

    };

}

