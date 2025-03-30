

namespace AOAIQuota
{
    public class QuotaInfo
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string CompanyEmail { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string CompanyCity { get; set; }
        public required string CompanyPostalCode { get; set; }
        public required string CompanyCountry { get; set; }
        public required string[] SubscriptionIdList { get; set; }
        public required string[] RegionList { get; set; }
        public required string Deployment { get; set; }
        public required string Model { get; set; }
        public required string AskQuota { get; set; }
    }
}