using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using KubeServiceBinding;
using System.Globalization;

namespace vac_seen_generator.Controllers;

[ApiController]
[Route("[controller]")]
public class GeneratorController : ControllerBase
{
    // Location code is hard-coded on purpose: This is the
    // microservice that reports vaccinations for The United States.
    // Each country has its own microservice.
    private const string CountryCode = "US";

    // List of vaccine types available in this location
    public static readonly string[] vTypes = { "Pfizer", "Moderna", "Johnson & Johnson" };

    // Maximum number of vaccinations to be simulated at one time
    private const int MaxVaccines = 40;

    // Kafka topic written to. This is typically the country code in lower case.
    private const string KafkaTopic = "us";

    [HttpPost]
    //    public async Task<ActionResult<int>> PostTodoItem(DateOnly generation_date)
    public async Task<ActionResult<int>> GenerateVaccinationEvents([FromForm]DateTimeOffset generation_date)
    {
        Console.WriteLine("REQUEST RECEIVED");

        // Get service binding information, i.e. the stuff we need
        // in order to connect to Kafka
        Dictionary<string, string> bindingsKVP = GetDotnetServiceBindings();

        // Generate data

            // Number of vaccinations is random integer from 1 to MaxVaccines
            Random rnd = new Random();
            int numberOfVaccinations = rnd.Next(1, MaxVaccines + 1);

            // Create a unique recipientID (a GUID) for each recipient
            // and send the data over to Kafka
            for (int i = 0; i < numberOfVaccinations - 1; i++)
            {
                Guid recipientID = Guid.NewGuid();

                // Vaccination type is random
                int vaccinationTypeID = rnd.Next(0, vTypes.Length);

                // Shot number is 1 or 2
                int shotNumber = rnd.Next(1, 3);

                VaccinationEvent ve = new VaccinationEvent
                {
                    Id = Guid.NewGuid(),
                    RecipientID = recipientID.ToString(),
                    ShotNumber = shotNumber,
                    VaccinationType = vTypes.GetValue(vaccinationTypeID).ToString(),
                    EventTimestamp = generation_date,
                    CountryCode = CountryCode,
                };

                // Convert object to JSON so it can be sent to Kafka
                string veJson = JsonConvert.SerializeObject(ve);

                // Write to Console for fun
                Console.WriteLine(veJson);

                // Send event to Kafka
                ProducerConfig conf = new ProducerConfig
                {
                    BootstrapServers = bindingsKVP["bootstrapservers"],
                    SecurityProtocol = ToSecurityProtocol(bindingsKVP["securityProtocol"]),
                    SaslMechanism = ToSaslMechanism(bindingsKVP["saslMechanism"]),
                    SaslUsername = bindingsKVP["user"],
                    SaslPassword = bindingsKVP["clientSecret"],
                };

                // DEBUGGING: Write to Console just to make sure values are correct
                // Console.WriteLine("bootstrapservers value is {0}", conf.BootstrapServers);
                // Console.WriteLine("securityProtocol value is {0}", conf.SecurityProtocol.ToString());
                // Console.WriteLine("saslMechanism    value is {0}", conf.SaslMechanism.ToString());
                // Console.WriteLine("user             value is {0}", conf.SaslUsername);
                // Console.WriteLine("secret           value is {0}", conf.SaslPassword);
                // Console.WriteLine("clientId         value is {0}", conf.ClientId);

                Action<DeliveryReport<Null, string>> handler =
                    r =>
                        Console
                            .WriteLine(!r.Error.IsError
                                ? $"Delivered message to {r.TopicPartitionOffset}"
                                : $"Delivery Error: {r.Error.Reason}");

                using (var p = new ProducerBuilder<Null, string>(conf).Build())
                {
                    p.Produce(KafkaTopic, new Message<Null, string> { Value = veJson }, handler);

                    // wait for up to 3 seconds for any inflight messages to be delivered.
                    p.Flush(TimeSpan.FromSeconds(3));
                }
            }

        // Return count
        return numberOfVaccinations - 1;
    }
    private static Dictionary<string, string> GetDotnetServiceBindings()
    {
        int count = 0;
        int maxTries = 999;
        while (true)
        {
            try
            {
                DotnetServiceBinding sc = new DotnetServiceBinding();
                Dictionary<string, string> d = sc.GetBindings("kafka");
                return d;
                // At this point, we have the information needed to bind to our Kafka
                // bootstrap server.
            }
            catch (Exception e)
            {
                // handle exception
                Console.WriteLine("Waiting for service binding...");
                System.Threading.Thread.Sleep(1000);
                if (++count == maxTries) throw e;
            }
        }
    }

    public static SecurityProtocol ToSecurityProtocol(string bindingValue) => bindingValue switch
    {
        "SASL_SSL" => SecurityProtocol.SaslSsl,
        "PLAIN" => SecurityProtocol.Plaintext,
        "SASL_PLAINTEXT" => SecurityProtocol.SaslPlaintext,
        "SSL" => SecurityProtocol.Ssl,
        _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SecurityProtocol value: {bindingValue}"),
    };
    public static SaslMechanism ToSaslMechanism(string bindingValue) => bindingValue switch
    {
        "GSSAPI" => SaslMechanism.Gssapi,
        "PLAIN" => SaslMechanism.Plain,
        "SCRAM-SHA-256" => SaslMechanism.ScramSha256,
        "SCRAM-SHA-512" => SaslMechanism.ScramSha512,
        _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SaslMechanism value: {bindingValue}"),
    };
}

