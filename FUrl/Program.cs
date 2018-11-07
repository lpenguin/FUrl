using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace FUrl
{
  class Program
    {
        static async Task Main(string[] args)
        {
            var result =  Parser.Default.ParseArguments<Options>(args);
            await result.MapResult(
                async opts => await RunOptionsAndReturnExitCode(opts),
                errors => Task.FromResult(0)
            );
        }

        private static async Task<int> RunOptionsAndReturnExitCode(Options opts)
        {
            Console.WriteLine(opts.Method);
            var res = await Upload(ParseOptions(opts));
            using (var reader = new StreamReader(res, Encoding.UTF8))
            {
                var data = await reader.ReadToEndAsync();
                Console.WriteLine(data);
            }

            return 0;
        }

        private static HttpRequestMessage ParseOptions(Options options)
        {
            var method = new HttpMethod(options.Method.ToString());
            var request = new HttpRequestMessage(method, options.Url);
            
            var formData = new MultipartFormDataContent();
            foreach (var (name, value) in options.HeadersParsed)
            {
                request.Headers.TryAddWithoutValidation(name, value);
            }

            foreach (var (name, value) in options.FormDataParsed)
            {
                if (value.StartsWith("@"))
                {
                    var assetFileName = value.TrimStart('@');
                    var fs = File.OpenRead(assetFileName);
                    var baseName = Path.GetFileName(assetFileName);
                    formData.Add(new StreamContent(fs), name, baseName);
                }
                else
                {
                    formData.Add(new StringContent(name), value);    
                }
                
            }
            request.Content = formData;
            return request;
        }
        
        private static async Task<Stream> Upload(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                return await response.Content.ReadAsStreamAsync();
            }
        }
    }
}