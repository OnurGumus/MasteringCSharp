using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration
{
    [JsonObject(MemberSerialization.Fields)]
    public abstract class Message
    {
    }

    public abstract class Command
    {
        readonly DateTime dateTime = DateTime.Now;

        public DateTime DateTime => dateTime;
    }

    public abstract class Event
    {

    }

    public class RegisterUserCommand : Command
    {
        readonly string userName;

        public RegisterUserCommand(string userName)
            => this.userName = userName;

        public string UserName => this.userName;
    }
    
    public class UserRegistered : Event
    {
        readonly string userName;

        public UserRegistered(string userName)
            => this.userName = userName;

        public string UserName => this.userName;
    }

    public class UserAlreadyExists : Event
    {
        readonly string userName;

        public UserAlreadyExists(string userName)
            => this.userName = userName;

        public string UserName => this.userName;
    }
}
