
namespace Models.Responses
{
    public class ApiResponse<T>
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
