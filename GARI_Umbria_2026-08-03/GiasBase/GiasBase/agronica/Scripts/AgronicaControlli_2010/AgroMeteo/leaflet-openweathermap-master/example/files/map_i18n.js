// --------------------------------------------------------
// ------- LABEL PER IL MENU DI DESTRA DELLA MAPPA --------
// --------------------------------------------------------
function getI18n(key, lang) {
    var i18n = {
        // l'inglese è diventata la lingua italiana
		en: {
              maps: 'Tipologie di Mappa'
            , layers: 'Rilevamenti'
			, current: 'Punti di Interesse'

            , clouds: 'Nuvole'
            , cloudscls: 'Nuvole (Modalità classica)'
			, precipitation: 'Precipitazioni'
            , precipitationcls: 'Precipitazioni (Modalità classica)'
			, rain: 'Pioggia'
            , raincls: 'Pioggia (Modalità classica)'
			, snow: 'Neve'
			, temp: 'Temperature'
			, windspeed: 'Intensità del Vento'
			, pressure: 'Pressione (Gradiente)'
            , presscont: 'Pressione (Contorni)'

			, city: 'Città'
            , station: 'Stazioni di Rilevamento'
			, windrose: 'Rosa dei Venti'
		}
		, de: {
			  maps: 'Karten'
			, layers: 'Ebenen'
			, current: 'Aktuelles Wetter'

			, clouds: 'Wolken'
			, cloudscls: 'Wolken (classic)'
			, precipitation: 'Niederschläge'
			, precipitationcls: 'Niederschläge (classic)'
			, rain: 'Regen'
			, raincls: 'Regen (classic)'
			, snow: 'Schnee'
			, temp: 'Temperatur'
			, windspeed: 'Windgeschwindigkeit'
			, pressure: 'Luftdruck'
			, presscont: 'Isobaren'

			, city: 'Städte'
			, station: 'Stationen'
			, windrose: 'Windrose'
		}
		, fr: {
			  maps: 'Carte'
			, layers: 'Couches'
			, current: 'Temps actuel'

			, clouds: 'Nuage'
			, cloudscls: 'Nuage (classique)'
			, precipitation: 'Précipitations'
			, precipitationcls: 'Précipitations (classique)'
			, rain: 'Pluie'
			, raincls: 'Pluie (classique)'
			, snow: 'Neiges'
			, temp: 'Température'
			, windspeed: 'Vitesse du vent'
			, pressure: 'Pression de l\'air'
			, presscont: 'Isobare'

			, city: 'Villes'
			, station: 'Stations'
			, windrose: 'Boussole'
		}
		, ru: {
			  maps: 'карта'
			, layers: 'слой'
			, current: 'текущая погода'

			, clouds: 'о́блачность'
			, cloudscls: 'о́блачность (класси́ческий)'
			, precipitation: 'оса́дки'
			, precipitationcls: 'оса́дки (класси́ческий)'
			, rain: 'дождь'
			, raincls: 'дождь (класси́ческий)'
			, snow: 'снег'
			, temp: 'температу́ра'
			, windspeed: 'ско́рость ве́тра'
			, pressure: 'давле́ние'
			, presscont: 'изоба́ра'

			, city: 'города'
			, station: 'станции'
			, windrose: 'направление ветра'
		}
		, nl: {
			  maps: 'Kaarten'
			, layers: 'Lagen'
			, current: 'Actuele Weer'

			, clouds: 'Wolken'
			, cloudscls: 'Wolken (classic)'
			, precipitation: 'Neerslag'
			, precipitationcls: 'Neerslag (classic)'
			, rain: 'Regen'
			, raincls: 'Regen (classic)'
			, snow: 'Sneeuw'
			, temp: 'Temparatuur'
			, windspeed: 'Windsnelheid'
			, pressure: 'Luchtdruk'
			, presscont: 'Isobare'

			, city: 'Steden'
			, station: 'Stations'
			, windrose: 'Wind roos'
		}
	};

	if (typeof i18n[lang] != 'undefined'
			&& typeof i18n[lang][key] != 'undefined') {
		return  i18n[lang][key];
	}
	return key;
}