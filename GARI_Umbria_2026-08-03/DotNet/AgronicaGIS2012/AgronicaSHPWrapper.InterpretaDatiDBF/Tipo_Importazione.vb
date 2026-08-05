Public Class Tipo_Importazione

    Public Enum Tipo_Importazione_ShapeFile
        Importazione_Agrea_Crea_Planning = 1
        Importazione_Trimble = 2
        Importazione_VecchiDatiGIAS = 3
        importaCatasto_DXF = 4
        importaGeneric_SHP = 5
        importazione_iMotion = 6
        Importa_KmlKmz = 7
        Importa_Gias2JohnDeere = 8
    End Enum

    Public Enum Tipo_GestioneRiportoDatiInGias
        NessunRiporto = 0
        RiportoAutomaticoDeiDati = 1
        RiportoSuAllegati = 2
    End Enum

End Class
