using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace JoyOI.Forum.Helpers
{
    public static class OjHelper
    {
        private static HttpClient client = new HttpClient() { BaseAddress = new Uri(Startup.Config["JoyOI:OnlineJudgeApiUrl"]) };

        public static async Task<string> GetProblemTitleAsync(string problemId, CancellationToken token = default(CancellationToken))
        {
            var response = await client.GetAsync("/api/problem/" + problemId, token);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                var problem = JsonConvert.DeserializeObject<dynamic>(body);
                return problem.data.title;
            }
        }
    }
}
