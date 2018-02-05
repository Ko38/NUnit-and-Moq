using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Exercise
{
    [TestFixture]
    public class Exercise1
    {
        [Test]
        public void Exercise1Test()
        {
            Gesture paper = new Paper();
            Gesture rock = new Rock();
            Assert.IsTrue(paper.Beats(rock) > 0);
            Assert.IsFalse(rock.Beats(paper) > 0);
        }

        [Test]
        public void Exercise1Test1()
        {
            Gesture paper = new Paper();
            Gesture paper1 = new Paper();
            Assert.That(paper.Beats(paper1), Is.EqualTo(0));
        }

        [Test]
        public void Exercise1Test2()
        {
            Gesture paper = new Paper();
            Assert.AreEqual(paper.Beats(null),-2);
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
