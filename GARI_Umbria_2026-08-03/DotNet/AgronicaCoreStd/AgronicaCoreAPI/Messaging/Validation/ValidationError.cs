namespace AgronicaCoreAPI.Messaging.Validation
{
    public class ValidationError
    {
        public string Field { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; }

        public ValidationError(string field, string errorCode, string errorMessage)
        {
            Field = field;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
