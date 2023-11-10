namespace AdopPixAPI.DTOs
{
    public class ResponseDto<T>
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
