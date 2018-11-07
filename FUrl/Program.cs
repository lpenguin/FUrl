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
//                .WithParsed(RunOptionsAndReturnExitCode)
//                .WithNotParsed(HandleParseError);
           
//            
//            var assetId = "5"; //args[0];
//            var version = "xyz"; //args[1];
//            var assetFileName = "/Users/nikita/Sources/python/maxquant_download/manage.py"; // args[2];
//            var baseUrl = "http://127.0.0.1:8000/api/assetversions/";
//            var token = "573e199074c94184a7dc3799b054166af6cd6bae";
//            var headers = new Dictionary<string, string>
//            {
//                {"Authorization", $"Token {token}"},
//                {"Accept" , "*/*"},
//            };
//
//            var parameters = new Dictionary<string, string>()
//            {
//                {"asset", assetId},
//                {"version", version}
//            };
//            using (var fs = File.OpenRead(assetFileName))
//            {
//                var files = new Dictionary<string, (string, FileStream)>()
//                {
//                    {"file", (Path.GetFileName(assetFileName), fs)}
//                };
//                var res = await Upload(baseUrl, headers, parameters, files);
//                using (StreamReader reader = new StreamReader(res, Encoding.UTF8))
//                {
//                    var data = await reader.ReadToEndAsync();
//                    Console.WriteLine(data);
//                }
//
//                
//            } 
           
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
//            Console.WriteLine(errs);
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