using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RobsSpacePatrol.DTO;

public class ApiResponse<TData>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public TData? Data { get; set; }
    public object? Errors { get; set; }
    public bool Damaged { get; set; } = false;
    public bool InfoOnly { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<TData> Create(bool successful, int statusCode, string message, TData? data = default,
        object? errors = null)
    {
        return new ApiResponse<TData>
        {
            Success = successful,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Errors = errors
        };
    }

    public static ApiResponse<TData> NotFound(string message = "Resource Not Found") =>
        Create(false, 404, message);

    public static ApiResponse<TData> NoContent(string message = "Op done") =>
        Create(false, (int)HttpStatusCode.NoContent, message);

    public static ApiResponse<TData> BadRequest(string message, object? errors = null) =>
        Create(false, 400, message, errors: errors);

    public static ApiResponse<TData> Conflict(string message) =>
        Create(false, (int)HttpStatusCode.Conflict, message);

    public static ApiResponse<TData> Error(int statusCode, string message, object? errors = null) =>
        Create(false, statusCode, message, errors: errors);

    public static ApiResponse<TData> OK(TData data, string message) =>
        Create(true, (int)HttpStatusCode.OK, message, data);

    public static ApiResponse<TData> Info(string message)
    {
        return new ApiResponse<TData>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Success = true,
            InfoOnly = true,
            Message = message,
        };
    }

    public static ApiResponse<TData> CreatedAt(TData data, string message) =>
        Create(true, (int)HttpStatusCode.Created, message, data);

    public static ApiResponse<TData> IsDamaged
    {
        get
        {
            return new ApiResponse<TData>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Success = true,
                Damaged = true,
            };
        }
    }
}
