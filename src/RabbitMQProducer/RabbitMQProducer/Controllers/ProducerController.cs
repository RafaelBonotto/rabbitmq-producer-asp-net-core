using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQProducer.Models;
using System.Text;
using System.Text.Json;

namespace RabbitMQProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        [HttpPost]
        public IActionResult InsertMensagem([FromBody] Mensagem msg)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "minhafila1",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                    var message = JsonSerializer.Serialize(msg);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "minhafila1",
                                         basicProperties: properties,
                                         body: body);
                }

                return Ok("Mensagem cadastrada na fila");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
