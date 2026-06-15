using System;
using Microsoft.AspNetCore.Http;

namespace FFmpeg.API.DTOs
{
    public class MergeVideosDto
    {
        public IFormFile FirstVideo { get; set; }
        public IFormFile SecondVideo { get; set; }
        public string Mode { get; set; } 
        public string Format { get; set; } = "mp4"; 
    }
}