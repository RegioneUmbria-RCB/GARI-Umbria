Aggiornamento XSD

1. Scaricare i nuovi xsd dal sito dell'agenzia delle entrate
2. Nel caso rinominare i file di versione in modo da togliere la versione e che siano quindi Schema_VFPR12.xsd e Schema_VFSM10.xsd
3. Copiare Schema_VFPR12.xsd con il nome Schema_VFPR12_Validator.xsd e all'interno rimuovere l'import della firma alla riga 8
4. Lanciare _GeneratoreClassi.cmd per far rigenerare le classi
5. Eliminare i fle vb che vengono creati nella cartellina degli XSD (questi non vanno archiviati)
6. Modificare nei file FatturaPA.vb e FatturaSemplificata.vb la riga
		Partial Public Class FatturaElettronicaType
	per collegare l'interfaccia
		Partial Public Class FatturaElettronicaType : Implements IFatturaElettronica
7. Copiare i nuovi xsd e i file FatturaPA.vb e FatturaSemplificata.vb (vanno cambiate le loro proprietà in modo da mettere "Azione di compilazione = Nessuno") in una nuova cartellina delll'appostita versione