namespace Dhicoin.Models
{
    public class KYCVerificationTokens
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string applicant_id { get; set; }
        public string form_url { get; set; }
        public string verification_id { get; set; }
        public string status { get; set; }
       
        
    }
}
