namespace Formation_Ecommerce_11_2025.API.Models
{
    public class ApiResponse<T> 
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}
