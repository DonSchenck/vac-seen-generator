using System;

namespace vac_seen_generator
{
    class VaccinationEvent
    {
        public Guid? Id;
        public String? RecipientID;
        public DateTimeOffset EventTimestamp;
        public string? CountryCode;
        public String? VaccinationType;
        public int ShotNumber;
    }
}
