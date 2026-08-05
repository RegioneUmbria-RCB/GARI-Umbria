namespace InData.Note
{
    public class SalvaGruppoNoteCompleto_In
    {
        public int NotaGruppoCod { get; set; }
        public int[] NoteUtilizzoCod { get; set; }
        public bool Visible { get; set; }
        public NoteVisibili_In[] NoteVisibili { get; set; }
    }
}