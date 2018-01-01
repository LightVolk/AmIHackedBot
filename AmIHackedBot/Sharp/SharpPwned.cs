﻿using AmIHackedBot.Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmIHackedBot
{
    public class HaveIBeenPwnedRestClient
    {
        private readonly string URL = @"https://haveibeenpwned.com/api/v2";

        public HaveIBeenPwnedRestClient()
        {

        }

        public async Task<List<Paste>> GetPasteAccount(string account)
        {
            string api = "pasteaccount";
            var response = await GETRequestAsync($"{api}/{account}");

            List<Paste> allPastes = new List<Paste>();

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                allPastes = JsonConvert.DeserializeObject<List<Paste>>(body);
                return allPastes;
            }
            else
            {
                return null;
            }
        }

        public async Task<Breach> GetBreach(string site)
        {
            string api = "breach";
            var response = await GETRequestAsync($"{api}/{site}");
            Breach breach = new Breach();

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                breach = JsonConvert.DeserializeObject<Breach>(body);
                return breach;
            }
            else
            {
                return null;
            }

        }

        public async Task<List<Breach>> GetAllBreaches()
        {
            string api = "breaches";
            var response = await GETRequestAsync(api);

            List<Breach> AllBreaches = new List<Breach>();
            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                AllBreaches = JsonConvert.DeserializeObject<List<Breach>>(body);
                return AllBreaches;
            }
            else
            {
                return AllBreaches;
            }

        }

        public async Task<List<Breach>> GetAccountBreaches(string account)
        {
            string api = "breachedaccount";
            var response = await GETRequestAsync($"{api}/{account}");

            List<Breach> AllBreaches = new List<Breach>();

            if (response.StatusCode == "OK")
            {
                string body = response.Body;
                AllBreaches = JsonConvert.DeserializeObject<List<Breach>>(body);
                return AllBreaches;
            }
            else
            {
                return AllBreaches;
            }
        }

        public async Task<bool> IsPasswordPwned(string password)
        {
            string api = "pwnedpassword";
            var response = await GETRequestAsync($"{api}/{password}");

            if (response.StatusCode == "OK")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task<Response> GETRequestAsync(string parameters)
        {
            Response RestResponse = new Response();
            Uri request = new Uri($"{URL}/{parameters}");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "SharpPwned.NET");

            try
            {
                response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                string statusCode = response.StatusCode.ToString();

                RestResponse.Body = responseBody;
                RestResponse.StatusCode = statusCode;

                //Must dispose
                client.Dispose();
                return RestResponse;
            }
            catch (HttpRequestException e)
            {
                RestResponse.Body = null;
                RestResponse.StatusCode = response.StatusCode.ToString();
                RestResponse.HttpException = e.Message;
                return RestResponse;
            }
        }
    }
}
