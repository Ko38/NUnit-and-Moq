using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Exercise2
{
    [TestFixture]
    public class Exercise2
    {
        [Test]
        public void Exercise2Test()
        {
            Gesture paper = new Paper();
            Gesture rock = new Rock();
            Assert.Multiple(() =>
            {
                Assert.IsTrue(paper.Beats(rock) > 0);
                Assert.IsFalse(rock.Beats(paper) > 0);
            });
        }

        [Test]
        public void Exercise2Test1()
        {
            Gesture paper = new Paper();
            Gesture paper1 = new Paper();
            Assert.That(paper.Beats(paper1), Is.EqualTo(0));
        }

        [Test]
        public void Exercise2Test2()
        {
            Gesture paper = new Paper();
            Assert.AreEqual(paper.Beats(null),-2);
        }

        [Test]
        public void Exercise2Test3()
        {
            Gesture paper = new Paper();
            Gesture gesture = null;
            Warn.If(paper.Beats(gesture) == -2);
        }

        private Player _flash;

        [SetUp]
        public void Setup()
        {
            _flash = new Player();
        }

        [Test]
        public void Exercise2Test4()
        {
            Assert.Throws<FlashException>(() =>
            {
                _flash.ThrowGesture(null);
            });
        }

        [Test]
        public void Exercise2Test5()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() =>
            {
                _flash.ThrowGesture("YA");
            });
            StringAssert.StartsWith("This gesture is not implemented", ex.Message);
        }
    }

    public class FlashException : Exception { }

    public class Player
    {
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

    public abstract class Gesture
    {
        public int Beats(Gesture gesture)
        {
            if (gesture is Rock) return Beats(gesture as Rock);
            if (gesture is Paper) return Beats(gesture as Paper);
            if (gesture is Scissors) return Beats(gesture as Scissors);
            return -2;
        }
        abstract protected int Beats(Scissors scissors);
        abstract protected int Beats(Rock rock);
        abstract protected int Beats(Paper paper);
    }


    public class Rock: Gesture
    {
        protected override int Beats(Scissors scissors) => 1;
        protected override int Beats(Rock rock) => 0;
        protected override int Beats(Paper paper) => -1;
    }

    public class Paper : Gesture
    {
        protected override int Beats(Scissors scissors) => -1;
        protected override int Beats(Rock rock) => 1;
        protected override int Beats(Paper paper) => 0;
    }

    public class Scissors : Gesture
    {
        protected override int Beats(Scissors scissors) => 0;
        protected override int Beats(Rock rock) => -1;
        protected override int Beats(Paper paper) => 1;
    }
}
