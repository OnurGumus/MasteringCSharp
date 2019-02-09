using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using System;
using System.Threading;

namespace BlueGreen.Client
{
    class Program
    {
        const string configString = @"
akka{
    actor {
        provider = cluster
        serializers {
            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
        }
    serialization-bindings {
            ""System.Object"" = hyperion
}
    }
    remote {
        dot-netty.tcp {
            public-hostname = ""localhost""
            hostname = ""localhost""
            port =  5000
        }
    }

    cluster {
        seed-nodes = [ ""akka.tcp://cluster-system@localhost:5000/""]
seed-node-timeout = 30s
    }

    persistence {
        journal.plugin = ""akka.persistence.journal.inmem""
        snapshot-store.plugin = ""akka.persistence.snapshot-store.local""
    }
}
";
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(configString).WithFallback(ClusterSingletonManager.DefaultConfig());
            var system = ActorSystem.Create("cluster-system", config);

            var client =
                system
                    .ActorOf(
                        ClusterSingletonProxy.Props("/user/consumer", 
                        ClusterSingletonProxySettings.Create(system).WithRole("worker")));
            while(true)
            {
                Thread.Sleep(1000);
                client.Tell(DateTime.Now.ToLongTimeString());
            }

            Console.ReadKey();
        }
    }
}
