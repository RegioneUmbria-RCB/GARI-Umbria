

function demoold() {
    //url = "http://api.statbank.dk/v1/data/AKU100/JSONSTAT?lang=en&valuePresentation=Default&timeOrder=Ascending&Tid=(-n%2B41)&BESKSTATUS=BESTOT%2CAKUL",

    url = "./jsontest.json",
    //"Tid" contains time; "BESKSTATUS", employment status
				ds = JSONstat(url).Dataset(0),
				tid = ds.Dimension("Tid").id,
				time = [],
				empl = [],
				act = []
			;

    tid.forEach(function (t) {
        var 
        //employment
					e = ds.Data({ "Tid": t, "BESKSTATUS": "BESTOT" }).value * 1000,
        //unemployment
					u = ds.Data({ "Tid": t, "BESKSTATUS": "AKUL" }).value * 1000
				;

        time.push(t.replace(/K/, "")); //Normalize time for Visual
        empl.push(e);
        act.push(e + u);
    });

    visual({
        lang: "en",
        title: "Labor market",
        geo: "Denmark",
        time: time,
        footer: "Source: Statistics Denmark.",
        unit: { label: "people" },
        fixed: [900, 400],
        dec: 0,
        grid: {
            line: 5,
            shadow: 6,
            point: 0
        },
        type: "tsline",
        data: [
					{ label: "Labor force", val: act },
					{ label: "Employment", val: empl }
				]
    });
}

function demo() {

    
    url = "./jsontest2.json",
    //"Tid" contains time; "BESKSTATUS", employment status
				ds = JSONstat(url).Dataset(0),
				tid = ds.Dimension("Tempo").id,
				time = [],
				empl = [],
				act = []
			;

    tid.forEach(function (t) {
        var 
        //employment
					e = ds.Data({ "Tempo": t, "Avversita": "Bassa" }).value,
        //unemployment
					u = ds.Data({ "Tempo": t, "Avversita": "Alta" }).value
				;

        time.push(t.replace(/K/, "")); //Normalize time for Visual
        empl.push(e);
        act.push(e + u);
    });

    visual({
        lang: "en",
        title: "Avversità",
        geo: "",
        time: time,
        footer: "Fonte: Consorzio.",
        unit: { label: "Avversità" },
        dec: 0,
        fixed: [900, 400],        
        grid: {
            line: 3,
            shadow: 6,
            point: 0
        },
        type: "tsline",
        data: [
					{ label: "Erwinia", val: act },
                    { label: "Oidio", val: empl }
				]
    });
}