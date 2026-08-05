
// spiegazione di controllo_data() nello script originale (vedi link sopra)
function controllo_data(stringa){
    var espressione = /^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/;
    if (!espressione.test(stringa))
    {
        return false;
    }else{
        anno = parseInt(stringa.substr(6),10);
        mese = parseInt(stringa.substr(3, 2),10);
        giorno = parseInt(stringa.substr(0, 2),10);
        
        var data=new Date(anno, mese-1, giorno);
        if(data.getFullYear()==anno && data.getMonth()+1==mese && data.getDate()==giorno){
            return true;
        }else{
            return false;
        }
    }
}



function confronta_data(data1, data2){

	// controllo validità formato data
    if(controllo_data(data1) &&controllo_data(data2)){
		//trasformo le date nel formato aaaammgg (es. 20081103)
        data1str = data1.substr(6)+data1.substr(3, 2)+data1.substr(0, 2);
		data2str = data2.substr(6)+data2.substr(3, 2)+data2.substr(0, 2);
		//controllo se la seconda data è successiva alla prima
        if (data2str-data1str<0) {
            return false;
        }else{
			return true;
        }
    }else{
        return false;
    }
}


