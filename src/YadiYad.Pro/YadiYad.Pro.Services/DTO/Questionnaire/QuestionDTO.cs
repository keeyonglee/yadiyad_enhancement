using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Questionnaire
{
    public class QuestionDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [Description("This is question statement")]
        public string Title { get; set; }
        public List<AnswerChoice> Choices { get; set; } = new List<AnswerChoice>();
        public object Answers { get; set; }
    }
}
