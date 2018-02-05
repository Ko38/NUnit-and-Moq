using NUnit.Framework;
using NunitAndMoq.Section1;
using NunitAndMoq.Section3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Section2
{
    public class PlayerTest
    {
        private Player Phillip;
        private Player Flash;

        [SetUp]
        public void SetUp()
        {
            Phillip = new Player(null) {Cash= 100 };
            Flash = new Player(null) { Cash = 1000 };
        }

        [Test]
        [TestCase(100, 0)]
        [TestCase(101, 100)]
        public void WithdrawlPhillipCash(int withdrawalAmount, int result)
        {
            Phillip.Withdraw(withdrawalAmount);
            Assert.That(Phillip.Cash, Is.EqualTo(result));
        }

        [Test]
        public void TestPlayersNormalCase()
        {
            //Arange
            var phillipGesture = Phillip.ThrowGesture("Rock");
            var flashGesture = Flash.ThrowGesture("Paper");
            //Act
            var result = phillipGesture.Beats(flashGesture);
            //Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void TestFlashException()
        {
            Assert.Throws<FlashException>(() =>
            {
                Flash.ThrowGesture(null);
            });
        }

        [Test]
        public void TestNotImplementedException()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() =>
            {
                Flash.ThrowGesture("YA");
            });
            StringAssert.StartsWith("This gesture is not implemented", ex.Message);
        }

        [Test]
        public void InconclusiveTest()
        {
            Assert.Inconclusive("Inconclusive");
        }
    }

    public class FlashException : Exception { }

    public class Player
    {
        public int Cash { get; set; }

        private IMakeMoney MakeMoney;

        public Player(IMakeMoney makeMoney)
        {
            this.MakeMoney = makeMoney;
        }

        public int MakingMoney()
        {
            return MakeMoney.MakeMoney();
        }

        public void Withdraw(int amount)
        {
            if (amount <= Cash)
                Cash -= amount;
        }

        protected virtual int GetRandomNumber()
        {
            return new Random().Next(0, 2);
        }

        public Gesture ThrowRandomGesture()
        {
            var random = GetRandomNumber();
            var gestures = new Gesture[] { new Rock(), new Paper(), new Scissors() };
            return gestures[random];
        }

        public Gesture ThrowGesture(string gestureName)
        {
            if (string.IsNullOrEmpty(gestureName))
                throw new FlashException();
            var gestureDict = new Dictionary<string, Gesture>
                {
                    { "Rock", new Rock()},
                    { "Paper", new Paper()},
                    { "Scissors", new Scissors()}
                };

            Gesture gesture;
            try
            {
                gesture = gestureDict[gestureName];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("This gesture is not implemented!!!");
            }

            return gesture;
        }
    }
}
