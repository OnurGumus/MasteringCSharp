using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserRegistration
{
    class Users : UntypedPersistentActor
    {
        readonly HashSet<string> userNames = new HashSet<string>();
        public override string PersistenceId { get; } = "Users";

        private void Apply(Event e)
        {
            switch (e)
            {
                case UserRegistered registered:
                    this.userNames.Add(registered.UserName);
                    break;
            }
        }
        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case RegisterUserCommand { UserName : var userName }:
                    Event e;
                    if (this.userNames.Contains(userName))
                        e = new UserAlreadyExists(userName);
                    else
                        e = new UserRegistered(userName);
                    Persist(e, Apply);
                    Context.System.EventStream.Publish(e);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch(message)
            {
                case Event e:
                    this.Apply(e);
                    break;
            }
        }
    }
    class Program
    {
     
        const string configString = @"
akka.persistence{
	journal {
	        plugin = ""akka.persistence.journal.sql-server""
		sql-server {
			# qualified type name of the SQL Server persistence journal actor
			class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""

			# dispatcher used to drive journal actor
			plugin-dispatcher = ""akka.actor.default-dispatcher""

			# connection string used for database access
			connection-string = ""Data Source = (localdb)\\ProjectsV13;Initial Catalog = Registration; Integrated Security = True; Connect Timeout = 30;""

			# default SQL commands timeout
			connection-timeout = 30s

            # SQL server schema name to table corresponding with persistent journal
        schema-name = dbo

            # SQL server table corresponding with persistent journal
        table-name = EventJournal

            # should corresponding journal table be initialized automatically
        auto-initialize = on

            # timestamp provider used for generation of journal entries timestamps
        timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""

			# metadata table
			metadata-table-name = Metadata
    }
}

snapshot-store {
	        plugin = ""akka.persistence.snapshot-store.sql-server""
		sql-server {

			# qualified type name of the SQL Server persistence journal actor
			class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""

			# dispatcher used to drive journal actor
			plugin-dispatcher = """"akka.actor.default-dispatcher""""

			# connection string used for database access
		    connection-string = ""Data Source = (localdb)\\ProjectsV13;Initial Catalog = Registration; Integrated Security = True; Connect Timeout = 30;""



			# default SQL commands timeout
			connection-timeout = 30s

            # SQL server schema name to table corresponding with persistent journal
schema-name = dbo

            # SQL server table corresponding with persistent journal
table-name = SnapshotStore

            # should corresponding journal table be initialized automatically
auto-initialize = on
		}
	}
}
";
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(configString);

            var actorSystem = ActorSystem.Create("MySite", config);
            var users = actorSystem.ActorOf<Users>();
            var usersToRegister =
                new List<string> { "Naruto", "Itachi", "Kisame" }
                .Select(u => new RegisterUserCommand(u));

            foreach (var command in usersToRegister)
            {
                users.Tell(command);
            }
            Console.ReadKey();
        }
    }
}
