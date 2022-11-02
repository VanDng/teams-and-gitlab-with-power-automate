// See https://aka.ms/new-console-template for more information
using Base;
using Caller;
using Caller.Callers;
using Connector.Connectors;

Console.WriteLine("Hello, World!");

GitlabCaller gitlab = new GitlabCaller();
HttpResponseMessage response = await gitlab.GetMergeRequests();
await response.ConsolePrint();