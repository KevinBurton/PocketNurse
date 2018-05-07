using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PocketNurse.Models.UploadViewModel
{
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1
    public class UploadViewModel
    {
        IFormFile UploadFile { get; set; }
    }
}
