using Dhicoin.Models;
using Newtonsoft.Json;
using System.Text;

namespace Dhicoin.Utility
{
    public class KycRequests
    {
        public async Task<string> GetApplicantID(CustomerMasterProfile user)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kycaid.com/applicants");
            request.Headers.Add("Authorization", "a3b555b608d3744fe6285427eca44adeab7f");
            Root person = new Root
            {
                type = "PERSON",
                first_name = user.FirstName,
                last_name = user.LastName,
                dob = user.DateOfBirth,
                email = user.Email
            };

            string json = JsonConvert.SerializeObject(person);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json"); // create a StringContent instance with the serialized JSON
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var applicantId = (string)responseObject.applicant_id;
            return applicantId;
        }
        public async Task<(string form_url, string verification_id)> GetFormUrl(CustomerMasterProfile user, string appId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.kycaid.com/forms/ea62bf0a0088644a8d287de72614e1a151e8/urls");
            request.Headers.Add("Authorization", "a3b555b608d3744fe6285427eca44adeab7f");
            Root person = new Root
            {
                applicant_id = appId,
            };

            string json = JsonConvert.SerializeObject(person);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json"); // create a StringContent instance with the serialized JSON
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var form_url = (string)responseObject.form_url;
            var verification_id = (string)responseObject.verification_id;
            return (form_url, verification_id);
        }
        private async Task<string> GetProfileStatus(string verification_id)
        {
            var client = new HttpClient();
            var url = string.Format("https://api.kycaid.com/verifications/{0}", verification_id);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", "a3b555b608d3744fe6285427eca44adeab7f");
            var content = new StringContent("", null, "text/plain");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var status = (string)responseObject.status;
            return status;
        }
    }
}
