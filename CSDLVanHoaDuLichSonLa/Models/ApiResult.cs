namespace CSDLVanHoaDuLichSonLa.Models
{
    public class ApiResult<T>
    {
        public bool IsSuccessed { get; set; }

        public string Message { get; set; }

        public T ResultObj { get; set; }
    }

    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T resultObj, string message = "")
        {
            IsSuccessed = true;
            ResultObj = resultObj;
            Message = message;
        }

        public ApiSuccessResult(string message = "")
        {
            IsSuccessed = true;
            Message = message;
        }
    }

    public class ApiErrorResult<T> : ApiResult<T>
    {
        public ApiErrorResult(string message)
        {
            IsSuccessed = false;
            Message = message;
        }
    }
}
