using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Otc.AspNetCore.ApiBoot
{
    [Serializable]
    public class SerilogBase
    {
        public SerilogConfiguration Serilog { get; set; }
    }

    [Serializable]
    public class SerilogConfiguration
    {
        public IEnumerable<string> Using { get; set; }
        //    = new string[]
        //{
        //    "Serilog.Sinks.Async",
        //    //"Serilog.Sinks.Console",
        //    //"Serilog.Sinks.File",
        //    //"Serilog.Enrichers.Environment",
        //    //"Serilog.Enrichers.Process",
        //    //"Serilog.Enrichers.Thread"
        //};

        public SerilogConfigurationMinimumLevel MinimumLevel { get; set; }
            = new SerilogConfigurationMinimumLevel()
            {
                Default = "Warning",
                Override = new Dictionary<string, string>()
            {
                { "System", "Error" },
                { "Microsoft", "Error" },
                { "Ole", "Information" },
                { "Otc", "Information" }
            }
            };

        public IEnumerable<SerilogConfigurationWriteTo> WriteTo { get; set; }
            = new SerilogConfigurationWriteTo[]
        {
            new SerilogConfigurationWriteTo()
            {
                Name = "Async",
                Args = new Dictionary<string, object>()
                {
                    {
                        "configure", new object[]
                        {
                            new SerilogConfigurationWriteTo()
                            {
                                Name = "Console",
                                Args = new Dictionary<string, object>()
                                {
                                    { "formatter", "Serilog.Formatting.Json.JsonFormatter, Serilog" }
                                }
                            }
                        }
                    }
                }
            }
        };

        public IEnumerable<string> Enrich { get; set; }
            = new string[]
                {
                    "FromLogContext",
                    "WithMachineName",
                    "WithEnvironmentUserName",
                    "WithProcessId",
                    "WithProcessName",
                    "WithThreadId"
                };

    }

    //public class SerilogConfigurationEnrichCollection : Collection<string>
    //{
    //    public SerilogConfigurationEnrichCollection AddItems(IEnumerable<string> items)
    //    {
    //        foreach (var item in items)
    //            Items.Add(item);

    //        return this;
    //    }
    //    //public SerilogConfigurationEnrichCollection(IEnumerable<string> items)
    //    //{
    //    //    Items.Clear();
    //    //    foreach (var item in items)
    //    //        Items.Add(item);
    //    //}
    //}
}