using System.Net;

namespace StudentCourseManagement.CORE
{
    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}
