using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using System;
using Newtonsoft.Json;

namespace vac_seen_generator
{
    class Program
    {
        public static readonly string[]
            vTypes =
            { "Pfizer", "Moderna", "Johnson & Johnson", "Astra-Zeneca" };

        static void Main(string[] args)
        {
            Console.WriteLine("Starting simulation...");

            // Location code is random integer from 1 to 4
            // Number of vaccinations is random integer from 1 to 125
            Random rnd = new Random();

            //Create random location code
            int locationID = rnd.Next(1, 5);

            // Create random vaccination count
            int numberOfVaccinations = rnd.Next(1, 126);

            // Create a unique recipientID (a GUID) for each recipient
            // and send the data over to Kafka
            for (int i = 0; i < numberOfVaccinations - 1; i++)
            {
                Guid recipientID = Guid.NewGuid();

                // Vaccination type is random: Pfizer, Moderna, J&J, Astra-Zeneca
                int vaccinationTypeID = rnd.Next(0, 4);

                // Shot number is 1 or 2
                int shotNumber = rnd.Next(1, 3);

                VaccinationEvent ve = new VaccinationEvent();
                ve.RecipientID = recipientID.ToString();
                ve.ShotNumber = shotNumber;
                ve.VaccinationType =
                    vTypes.GetValue(vaccinationTypeID).ToString();
                ve.EventTimestamp = DateTime.Now;

                // Convert object to JSON so it can be sent to Kafka
                string veJson = JsonConvert.SerializeObject(ve);

                // Send event to Kafka
                var conf =
                    new ProducerConfig { BootstrapServers = "localhost:9092" };

                Action<DeliveryReport<Null, string>> handler =
                    r =>
                        Console
                            .WriteLine(!r.Error.IsError
                                ? $"Delivered message to {r.TopicPartitionOffset}"
                                : $"Delivery Error: {r.Error.Reason}");

                using (var p = new ProducerBuilder<Null, string>(conf).Build())
                {
                    p.Produce("vaccinations",new Message<Null, string> { Value = veJson },handler);

                    // wait for up to 10 seconds for any inflight messages to be delivered.
                    p.Flush(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}
