using LaYumba.Functional;
using System;

namespace LaYumba
{
    using static F;

    public class AccountState
    {
        public decimal Balance { get; }
        public AccountState(decimal balance) { Balance = balance; }
    }


    public static class Account
    {
        public static Option<AccountState> Debit
           (this AccountState acc, decimal amount)
           => (acc.Balance < amount)
              ? None
              : Some(new AccountState(acc.Balance - amount));
    }

    public interface IRepository<T>
    {
        Option<T> Get(Guid id);
        void Save(Guid id, T t);
    }


    interface ISwiftService
    {
        void Wire(MakeTransfer transfer, AccountState account);
    }
    interface IValidator<T>
    {
        bool IsValid(T t);
    }
    class MyAPI 
    {

        IValidator<MakeTransfer> validator;
        IRepository<AccountState> accounts;
        ISwiftService swift;

        public MyAPI(IValidator<MakeTransfer> validator, IRepository<AccountState> accounts, ISwiftService swiftService)
        {
            this.validator = validator;
            this.accounts = accounts;
            this.swift = swiftService;
        }

        public void MakeTransfer( MakeTransfer transfer)
           => Some(transfer)
              .Map(Normalize)
              .Where(validator.IsValid)
              .ForEach(Book);



        void Book(MakeTransfer transfer)
           => accounts.Get(transfer.DebitedAccountId)
              .Bind(account => account.Debit(transfer.Amount))
              .ForEach(newState =>
              {
                  accounts.Save(transfer.DebitedAccountId, newState);
                  swift.Wire(transfer, newState);
              });



        MakeTransfer Normalize(MakeTransfer request)
           => request; // remove whitespace, toUpper, etc.

    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
