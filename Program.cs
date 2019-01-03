using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BackupVideos
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => options = opts)
                .WithNotParsed(errs =>
                {
                    throw new ArgumentException("Errors when parsing command line!");
                });

            var curTime = DateTime.Now;
            var saveDir = options.OutputDir;

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var signInToken = GetSignInToken(options.Username, options.Password);

            var dateList = new List<string>();
            for (var index = 0; index < options.DaysToScrape; ++index)
            {
                dateList.Add(ConvertDateToString(curTime.AddDays(-index)));
            }

            Parallel.ForEach(dateList, new ParallelOptions() { MaxDegreeOfParallelism = 7 }, scrapeDay =>
            {
                var folderPath = Path.Combine(saveDir, scrapeDay);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                IEnumerable<Video> source = PostData(signInToken, scrapeDay, scrapeDay);

                Parallel.ForEach(source, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, item =>
                {
                    var name = item.name;
                    var presignedContentUrl = item.presignedContentUrl;
                    var filePath = Path.Combine(folderPath, name + ".mp4");
                    var flag = false;
                    while (!flag)
                    {
                        try
                        {
                            if (!File.Exists(filePath))
                            {
                                using (WebClient webClient = new WebClient())
                                {
                                    webClient.DownloadFile(presignedContentUrl, filePath);
                                }
                            }
                            flag = true;
                        }
                        catch
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                });
            });
        }

        static public string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }


        public static string ConvertDateToString(DateTime curDay)
        {
            var year = curDay.Year.ToString("00");
            var month = curDay.Month.ToString("00");
            var day = curDay.Day.ToString("00");
            return $"{year}{month}{day}";
        }

        public static string GetSignInToken(string email, string pass)
        {
            var client = new HttpClient();
            var dict = new Dictionary<string, string>()
            {
                { "email", email },
                { "password", EncodeTo64(pass) }
            };
            client.DefaultRequestHeaders.Add("Password-Encoded", "true");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            var stringContent = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");

            var sendPostTask = client.PostAsync("https://arlo.netgear.com/hmsweb/login/v2", stringContent);
            sendPostTask.Wait();

            var readResponseTask = sendPostTask.Result.Content.ReadAsStringAsync();
            readResponseTask.Wait();
            return JsonConvert.DeserializeObject<LoginResponse>(readResponseTask.Result).data.token;
        }

        public static IEnumerable<Video> PostData(string token, string dateFrom, string dateTo)
        {
            var httpClient = new HttpClient();
            var stringContent = new StringContent(JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                { nameof (dateFrom), dateFrom },
                { nameof (dateTo), dateTo }
            }), Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            Task<HttpResponseMessage> task1 = httpClient.PostAsync("https://arlo.netgear.com/hmsweb/users/library", stringContent);
            task1.Wait();
            Task<string> task2 = task1.Result.Content.ReadAsStringAsync();
            task2.Wait();
            return JsonConvert.DeserializeObject<GetVids>(task2.Result).data;
        }
    }
}