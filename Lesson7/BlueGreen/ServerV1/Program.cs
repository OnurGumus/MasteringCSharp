using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using System;
using System.Threading;

namespace ServerV1
{
    class Program
    {
        const string configString = @"
        akka {
             coordinated-shutdown {
                phases {
                  # Shutdown cluster singletons
                  cluster-exiting {
                    timeout = 60 s
                    depends-on = [cluster-leave]
                  }
                }
            }

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
              port =  5001
            }
          }

          cluster {
                 seed-nodes = [ ""akka.tcp://cluster-system@localhost:5000/""]
            roles = [worker]
          }

          persistence {
            journal.plugin = ""akka.persistence.journal.inmem""
            snapshot-store.plugin = ""akka.persistence.snapshot-store.local""
          }
        }
";
        class Server : UntypedActor
        {
            protected override void OnReceive(object message)
            {
                Thread.Sleep(1500);
                Console.WriteLine($"!!! {message} !!!");
            }
        }
        static void Main()
        {
            var config = ConfigurationFactory.ParseString(configString).WithFallback(ClusterSingletonManager.DefaultConfig());
            var system = ActorSystem.Create("cluster-system", config);
            var server = system.ActorOf(ClusterSingletonManager.Props(
                    singletonProps: Props.Create<Server>(),
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(system).WithRole("worker")),
                    name: "consumer");
            Console.ReadKey();
        }
    }
}
