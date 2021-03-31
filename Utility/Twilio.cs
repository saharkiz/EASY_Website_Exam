using System;
using RestSharp;

namespace EASY.Website.OTP
{
    public class TwilioOTP
    {
        public static string sendOTP(string to)
        {
            var client = new RestClient("https://verify.twilio.com/v2/Services/VA45e8dee016f7979db73f26a09702d067/Verifications");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QUNmZGY4N2VmZGQzMDdiODlkZWFiN2RlNTUxYTU4MzFjNDo0NTJiNjc2MmI1OTZkNTM5NjY5MTA0OWU3Yzc3ZDJkYQ==");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("To", to);//"+639178083576"
            request.AddParameter("Channel", "sms");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        public static string validateOTP(string to, string code)
        {
            var client = new RestClient("https://verify.twilio.com/v2/Services/VA45e8dee016f7979db73f26a09702d067/VerificationCheck");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QUNmZGY4N2VmZGQzMDdiODlkZWFiN2RlNTUxYTU4MzFjNDo0NTJiNjc2MmI1OTZkNTM5NjY5MTA0OWU3Yzc3ZDJkYQ==");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("To", to);//"+639178083576"
            request.AddParameter("Code", code); 
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}