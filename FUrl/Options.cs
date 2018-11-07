using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace FUrl
{
// Define a class to receive parsed values
    public enum Method
    {
        Get, Post, Put, Delete, Patch
    }

    
    class Options {
        [Value(0, MetaName = "url", Required = true, HelpText = "Url")]
        public string Url { get; set; }
    
        [Option('f', "form", HelpText = "Form datas")]
        public IEnumerable<string> FormData { get; set; }
        
        [Option('h', "headers", HelpText = "Headers")]
        public IEnumerable<string> Headers { get; set; }

        private static (string, string) SplitOrThrow(string value, char delimiter = '=')
        {
            var tokens = value.Split(delimiter);
            if (tokens.Length != 2)
            {
                throw new Exception($"Invalid format: {value}");
            }

            return (tokens[0], tokens[1]);
        }

//        [Option('q', "query-param", HelpText = "Query params")]
//        public  IEnumerable<string> QueryParam { get; set; }

        [Option('m', "method", HelpText = "HTTP Method", Default = Method.Get)]
        public Method Method { get; set; }
            
//        [Option('v', "verbose", Default = true,
//            HelpText = "Prints all messages to standard output.")]
//        public bool Verbose { get; set; }
        
        public IEnumerable<(string, string)> HeadersParsed
        {
            get { return Headers.Select(x => SplitOrThrow(x, ':')); }
        }
        
//        public IEnumerable<(string, string)> QueryParamParsed
//        {
//            get { return QueryParam.Select(x => SplitOrThrow(x)); }
//        }
        
        public IEnumerable<(string, string)> FormDataParsed
        {
            get { return FormData.Select(x => SplitOrThrow(x)); }
        }
    }
}