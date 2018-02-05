using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NunitAndMoq.Section2;

namespace NunitAndMoq.Section3
{
    public interface IMakeMoney
    {
        int MakeMoney();
    }

    public class FakeMoney : IMakeMoney
    {
        public int MakeMoney()
        {
            return 0;
        }
    }

    public class StubMoney : IMakeMoney
    {
        private int _expectedAmount;

        public StubMoney(int expectedAmount)
        {
            _expectedAmount = expectedAmount;
        }
        public int MakeMoney()
        {
            return _expectedAmount;
        }
    }

    public class MockMoney : IMakeMoney
    {
        private int _expectedAmount;
        public Dictionary<string, int> MethodCallCount = new Dictionary<string, int>();


        public MockMoney(int expectedAmount)
        {
            _expectedAmount = expectedAmount;
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

        public int MakeMoney()
        {
            AddOrIncrement(nameof(MakeMoney));
            return _expectedAmount;
        }
    }

    [TestFixture]
    public class TestDoubles
    {
        [Test]
        public void TestFake()
        {
            Player player = new Player(new FakeMoney()) { Cash = 100};
            player.Withdraw(10);
            player.Cash += player.MakingMoney();
            Assert.That(player.Cash, Is.EqualTo(90));
        }

        [Test]
        public void TestStubs()
        {
            Player player = new Player(new StubMoney(50)) { Cash = 100 };
            player.Withdraw(10);
            player.Cash += player.MakingMoney();
            Assert.That(player.Cash, Is.EqualTo(140));
        }

        [Test]
        public void TestMock()
        {
            var makeMoney = new MockMoney(50);
            Player player = new Player(makeMoney) { Cash = 100 };
            player.Withdraw(10);
            player.Cash += player.MakingMoney();
            player.Cash += player.MakingMoney();
            Assert.Multiple(() =>
            {
                Assert.That(player.Cash, Is.EqualTo(190));
                Assert.That(makeMoney.MethodCallCount["MakeMoney"], Is.EqualTo(2));
            });
            
        }
    }
}
