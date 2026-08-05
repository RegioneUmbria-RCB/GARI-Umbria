namespace AgronicaNetCore.Base.Models
{
    public enum Boolean_Operators
    {
        None = -1,
        First = 0,
        And = 1,
        Or = 2,
        NAnd = 4,
        NOr = 5,
    }
    
    public enum Comparison_Operators
    {
        None = 0,
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
        Like = 7,
        NotLike = 8,
        In = 9,
        NotIn = 10,
        Between = 11,
        NotBetween = 12,
        IsNull = 13,
        IsNotNull = 14,
    }

    public class Filter
    {
        private readonly Type[] SUPPORTED_TYPES = { typeof(int), typeof(double), typeof(decimal), typeof(float), typeof(string), typeof(DateTime) };

        private string FieldName { get; set; }
        private string ParamName { get; set; }
        private Type Type { get; set; }
        private object Value { get; set; }
        private Comparison_Operators Comparison_Operator { get; set; }
        private Boolean_Operators Boolean_Operator { get; set; }

        public Filter(string field, string param, Comparison_Operators cop, Boolean_Operators bop, object value, Type type)
        {
            FieldName = field;
            ParamName = param;

            if (cop is Comparison_Operators.None)
                throw new Exception("L'operatore di confronto non può essere settato come None.");

            Comparison_Operator = cop;

            if (bop is Boolean_Operators.None)
                throw new Exception("L'operatore booleano non può essere settato come None.");

            Boolean_Operator = bop;
            Value = value;
            Type = type;
        }

        public string GetComparisonOp()
        {
            return this.Comparison_Operator switch
            {
                Comparison_Operators.Equal => "=",
                Comparison_Operators.NotEqual => "<>",
                Comparison_Operators.GreaterThan => ">",
                Comparison_Operators.GreaterThanOrEqual => ">=",
                Comparison_Operators.LessThan => "<",
                Comparison_Operators.LessThanOrEqual => "<=",
                //case enumOperatori.Like:
                //    return "LIKE";
                //case enumOperatori.NotLike:
                //    return "NOT LIKE";
                //case enumOperatori.In:
                //    return "IN";
                //case enumOperatori.NotIn:
                //    return "NOT IN";
                //case enumOperatori.Between:
                //    return "BETWEEN";
                //case enumOperatori.NotBetween:
                //    return "NOT BETWEEN";
                Comparison_Operators.IsNull => "IS NULL",
                Comparison_Operators.IsNotNull => "IS NOT NULL",
                Comparison_Operators.None => string.Empty,
                _ => string.Empty,
            };
        }

        public string GetBooleanOp()
        {
            return this.Boolean_Operator switch
            {
                Boolean_Operators.And => "AND",
                Boolean_Operators.Or => "OR",
                Boolean_Operators.NAnd => "AND NOT",
                Boolean_Operators.NOr => "OR NOT",
                Boolean_Operators.First => string.Empty,
                Boolean_Operators.None => string.Empty,
                _ => string.Empty,
            };
        }

        public bool IsSupportedType()
        {
            return SUPPORTED_TYPES.Contains(this.Type);
        }

        public string GetFormattedFilter(ref Dictionary<string, object> sqlParams, bool isFirst, string prefix = "")
        {
            string strFilter = "";

            try
            {
                if (isFirst)
                    this.Boolean_Operator = Boolean_Operators.First;

                if (!IsSupportedType())
                    throw new Exception("Tipo non gestito nel filtro aggiuntivo.");

                if (prefix != string.Empty)
                {
                    this.ParamName = $"@{prefix}_{this.ParamName.Replace("@","")}";
                }
                    
                switch (this.Comparison_Operator)
                {
                    case Comparison_Operators.Equal:
                    case Comparison_Operators.NotEqual:
                        sqlParams.TryAdd(this.ParamName, this.Value);
                        strFilter = $"    {GetBooleanOp()} {this.FieldName} {GetComparisonOp()} {this.ParamName} ";
                        break;

                    case Comparison_Operators.GreaterThan:
                    case Comparison_Operators.GreaterThanOrEqual:
                    case Comparison_Operators.LessThan:
                    case Comparison_Operators.LessThanOrEqual:
                        if (this.Type == typeof(string))
                            throw new Exception("Tipo stringa non supportato per gli operatori di confronto.");

                        sqlParams.TryAdd(this.ParamName, this.Value);
                        strFilter = $"    {GetBooleanOp()} {this.FieldName} {GetComparisonOp()} {this.ParamName} ";
                        break;

                    case Comparison_Operators.IsNull:
                    case Comparison_Operators.IsNotNull:
                        sqlParams.TryAdd(this.ParamName, this.Value);
                        strFilter = $"    {GetBooleanOp()} {this.FieldName} {GetComparisonOp()} ";
                        break;

                    //case enumOperatori.Like:
                    //    break;
                    //case enumOperatori.NotLike:
                    //    break;
                    //case enumOperatori.In:
                    //    break;
                    //case enumOperatori.NotIn:
                    //    break;
                    //case enumOperatori.Between:
                    //    break;
                    //case enumOperatori.NotBetween:
                    //    break;
                    case Comparison_Operators.None:
                    default:
                        throw new Exception("Operatore di confronto non supportato.");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Errore nella creazione del filtro su {this.FieldName}: " + e.Message);
            }

            return strFilter;
        }

    }

    public class FiltroAggiuntivo
    {
        private const int MAXIMUM_FILTER_NUMBER = 500;

        public readonly List<Filter> filters;

        public Boolean_Operators defaultBooleanOp;

        public FiltroAggiuntivo()
        {
            filters = new List<Filter>();
            defaultBooleanOp = Boolean_Operators.None;
        }

        public FiltroAggiuntivo(Boolean_Operators bOperator)
        {
            filters = new List<Filter>();
            defaultBooleanOp = bOperator;
        }

        public bool CheckMaximumFiltersNumber()
        {
            return filters.Count < MAXIMUM_FILTER_NUMBER;
        }

        public void AddFilter(string field, string param, Comparison_Operators cop, Boolean_Operators bop, object value, Type type)
        {
            if (defaultBooleanOp != Boolean_Operators.None && defaultBooleanOp != bop)
                throw new Exception("Operatore booleano differente da quello impostato di default.");
            filters.Add(new Filter(field, param, cop, bop, value, type));
        }
    }
}
