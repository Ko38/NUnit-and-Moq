using NUnit.Framework;
using System;
using ImpromptuInterface;
using System.Dynamic;
using System.Collections.Generic;

namespace UnitTests.TestDoubles
{
    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount ba;
        [Test]
        public void DepositIntegrationTest()
        {
            ba = new BankAccount(new ConsoleLog()) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithFake()
        {
            var log = new NullLog();
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithDynamicFake()
        {
            var log = Null<ILog>.Instance;
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithStubs()
        {
            var log = new NullLogWithResult(true);
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithMocks()
        {
            var log = new LogMock(true);
            ba = new BankAccount(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.Multiple(() =>
            {
                Assert.That(ba.Balance, Is.EqualTo(200));
                Assert.That(log.MethodCallCount[nameof(LogMock.Write)], Is.EqualTo(1));
            });
        }
    }

    public class NullLog : ILog
    {
        public bool Write(string msg)
        {
            return true;
        }
    }

    public class NullLogWithResult : ILog
    {
        private bool expectedResult;

        public NullLogWithResult(bool expectedResult)
        {
            this.expectedResult = expectedResult;
        }
        public bool Write(string msg)
        {
            return expectedResult;
        }
    }

    public class LogMock : ILog
    {
        private bool expectedResult;
        public Dictionary<string, int> MethodCallCount;

        public LogMock(bool expectedResult)
        {
            this.expectedResult = expectedResult;
            MethodCallCount = new Dictionary<string, int>();
        }

        private void AddOrIncrement(string methodName)
        {
            if (MethodCallCount.ContainsKey(methodName))
            {
                MethodCallCount[methodName]++;
            }
            else
            {
                MethodCallCount.Add(methodName, 1);
            }
        }

        public bool Write(string msg)
        {
            AddOrIncrement(nameof(Write));
            return expectedResult;
        }
    }

    public class Null<T> : DynamicObject where T : class
    {
        public static T Instance => new Null<T>().ActLike<T>();

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Activator.CreateInstance(
                typeof(T).GetMethod(binder.Name).ReturnType);
            return true;
        }
    }

    public interface ILog
    {
        bool Write(string msg);
    }

    public class ConsoleLog:ILog
    {
        public bool Write(string msg)
        {
            Console.WriteLine(msg);
            return true;
        }
    }
    
    public class BankAccount
    {
        public int Balance { get; set; }
        private readonly ILog log;

        public BankAccount(ILog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            if(log.Write($"Depositing {amount}"))
                Balance += amount;
        }
    }
}
