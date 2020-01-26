using Microsoft.AspNetCore.Mvc;
using RabbitMQ_Messages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ_Messages.Controller
{
    [ApiController]
    [Route("Controller")]
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
    }
}
