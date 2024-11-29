using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Common
{
    public class PictureDTO
    {
        public IFormFile File { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
    }
}
