using System.ComponentModel.DataAnnotations;

namespace RabbitMQ_Messages.Models
{
    public class Content
    {
        [Required]
        public string Message { get; set; }
    }
}