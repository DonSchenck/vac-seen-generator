using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using KubeServiceBinding;

namespace vac_seen_generator
{
    class Program
    {
        public static readonly string[]
            vTypes =
            { "Pfizer", "Moderna", "Johnson & Johnson" };

        static void Main(string[] args)
        {
            DotnetServiceBinding sc = new DotnetServiceBinding();
            Dictionary<string,string> bindingsKVP = sc.GetBindings("kafka");
            
            // At this point, we have the information needed to bind to our Kafka
            // bootstrap server.
            

            // Location code is random integer from 1 to 4
            // Number of vaccinations is random integer from 1 to 125            
            Random rnd = new Random();

            // Hard-coded country code: United States
            string countryCode = "us";

            // Create random vaccination count
            int numberOfVaccinations = rnd.Next(1, 126);

            // Create a unique recipientID (a GUID) for each recipient
            // and send the data over to Kafka
            for (int i = 0; i < numberOfVaccinations - 1; i++)
            {
                Guid recipientID = Guid.NewGuid();

                // Vaccination type is random: Pfizer, Moderna, J&J
                int vaccinationTypeID = rnd.Next(0, 3);

                // Shot number is 1 or 2
                int shotNumber = rnd.Next(1, 3);

                VaccinationEvent ve = new VaccinationEvent();
                ve.RecipientID = recipientID.ToString();
                ve.ShotNumber = shotNumber;
                ve.VaccinationType = vTypes.GetValue(vaccinationTypeID).ToString();
                ve.EventTimestamp = DateTime.Now;
                ve.CountryCode = countryCode;

                // Convert object to JSON so it can be sent to Kafka
                string veJson = JsonConvert.SerializeObject(ve);

                // Write to Console for fun
                Console.WriteLine(veJson);
                
                // Send event to Kafka
                var conf = new ProducerConfig { 
                    BootstrapServers = bindingsKVP["bootstrapservers"], 
                    SecurityProtocol = ToSecurityProtocol(bindingsKVP["securityProtocol"]),
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = bindingsKVP["user"],
                    SaslPassword = bindingsKVP["password"]
                    };
  
                
                Action<DeliveryReport<Null, string>> handler =
                    r =>
                        Console
                            .WriteLine(!r.Error.IsError
                                ? $"Delivered message to {r.TopicPartitionOffset}"
                                : $"Delivery Error: {r.Error.Reason}");

                using (var p = new ProducerBuilder<Null, string>(conf).Build())
                {
                    p.Produce("us",new Message<Null, string> { Value = veJson },handler);

                    // wait for up to 10 seconds for any inflight messages to be delivered.
                    p.Flush(TimeSpan.FromSeconds(10));
                }
            }
        }

    public static SecurityProtocol ToSecurityProtocol(string bindingValue) => bindingValue switch
    {
        "SASL_SSL"          => SecurityProtocol.SaslSsl,
        "PLAIN"             => SecurityProtocol.Plaintext,
        "SASL_PLAINTEXT"    => SecurityProtocol.SaslPlaintext,
        "SSL"               => SecurityProtocol.Ssl,
        _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SecurityProtocol value: {bindingValue}"),
    };        
    public static SaslMechanism ToSaslMechanism(string bindingValue) => bindingValue switch
    {
        "GSSAPI"        => SaslMechanism.Gssapi,
        "PLAIN"         => SaslMechanism.Plain,
        "SCRAM-SHA-256" => SaslMechanism.ScramSha256,
        "SCRAM-SHA-512" => SaslMechanism.ScramSha512,
        _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SaslMechanism value: {bindingValue}"),
    };        
    }
}
