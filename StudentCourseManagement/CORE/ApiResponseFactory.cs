using System.Net;

namespace StudentCourseManagement.CORE
{
    public class ApiResponseFactory
    {
        public static ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Request Successful")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.OK,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> CreateNotFoundResponse<T>(string message = "Resource not Found")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.NotFound,
                Data = default,
                Message = message
            };
        }

        public static ApiResponse<T> CreateBadRequestResponse<T>(string message = "Bad Request")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.BadRequest,
                Data = default,
                Message = message
            };
        }

        public static ApiResponse<T> CreateConflictResponse<T>(string message = "Resource already exists")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.Conflict,
                Data = default,
                Message = message
            };
        }

        public static ApiResponse<T> CreateErrorResponse<T>(string message = "Something went wrong")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.InternalServerError,
                Data = default,
                Message = message
            };
        }
    }
}
