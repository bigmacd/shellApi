using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Confluent.Kafka.Serialization;
using Confluent.Kafka;
using System.Text;

namespace shellApi
{
    public interface IAuditPublisher
    {
        void SendMessageAsync(string topic, string key, string auditRecord);
        void SendUnformatedMessageAsync(string topic, string key, string auditRecord);
    }
    
    public class AuditPublisher : IAuditPublisher
    {

        public string topic { get; set; }
        Dictionary<string, object> kafkaConfig = new Dictionary<string, object>();
        Producer<string, string> _producer;
        private ILogger _logger;

        public AuditPublisher(IConfiguration config, string environment, ILogger logger)
        {
            _logger = logger;
            string broker = string.Format("{0}:{1}",
                                          config[string.Format("auditer:{0}:host", environment)],
                                          config[string.Format("auditer:{0}:port", environment)]);
            kafkaConfig.Add("bootstrap.servers", broker);
            _producer = new Producer<string, string>(kafkaConfig,
                                                   new StringSerializer(Encoding.UTF8),
                                                   new StringSerializer(Encoding.UTF8));
        }

        public async void SendMessageAsync(string topic, string key, string message)
        {
            await _producer.ProduceAsync(topic, key, message);
        }

        // Expectings a string like "user: 'username', id: 'id'"  
        public async void SendUnformatedMessageAsync(string topic, string key, string message)
        {
            string auditmessageT = "{ \n" + message + "\n}";
            try
            {
                JObject o = JObject.Parse(auditmessageT);
                await _producer.ProduceAsync(topic, key, message);
            }
            catch(Exception ex) {
                await _producer.ProduceAsync("auditError", key, message);
            }
        }
    }
}
