using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ_Messages.Configuration;
using RabbitMQ_Messages.Models;
using System;
using System.Text;

namespace RabbitMQ_Messages.Controller
{
    [ApiController]
    [Route("[Controller]")]
    public class MessagesController : ControllerBase
    {
        private static Counter _Conter = new Counter();

        [HttpGet]
        public object Get()
        {
            return new
            {
                QtMessageSent = _Conter.ActualValue
            };
        }

        [HttpPost]
        public object Post(
            [FromServices]RabbitMQEntityConfiguration configurations,
            [FromBody]Content content)
        {
            lock (_Conter)
            {
                _Conter.Increment();

                var factory = new ConnectionFactory()
                {
                    HostName = configurations.HostName,
                    Port = configurations.Port,
                    UserName = configurations.UserName,
                    Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TestAspCore",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Message content: {content.Message}";

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "TestAspCore",
                                         basicProperties: null,
                                         body: body);
                }

                return new
                {
                    Result = "Message sent with sucess"
                };
            }
        }
    }
}
