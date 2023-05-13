using System.Text.Json.Serialization;

namespace Dhicoin.Models
{
    public class Root
    {
        public string type { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string dob { get; set; }
        public string email { get; set; }
        public string applicant_id { get; set; }   
    }
}
