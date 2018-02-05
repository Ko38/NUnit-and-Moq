using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Section1
{
    [TestFixture]
    public class RockPaperScissorsTest
    {
        private static object[] MyCaseSource =
        {
            new object[] { new Rock(), new Paper(), -1 },
        };

        [Test]
        [TestCaseSource("MyCaseSource")]
        //[TestCase(new Rock(),new Paper(), 1)]
        public void TestFlows(Gesture gesture1, Gesture gesture2, int result)
        {
            Assert.That(gesture1.Beats(gesture2), Is.EqualTo(result));
        }

        [Test]
        public void TestPaperBeatsRock()
        {
            Gesture rock = new Rock();
            Gesture paper = new Paper();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(paper.Beats(rock) < 0);
                Assert.IsTrue(rock.Beats(paper) > 0);
            });
            //Assert.IsTrue(paper.Beats(rock) > 0);
            //Assert.IsFalse(rock.Beats(paper) > 0);
        }

        [Test]
        public void TestScissorsBeatPaper()
        {
            Gesture scissors = new Scissors();
            Gesture paper = new Paper();

            Assert.IsTrue(scissors.Beats(paper) > 0);
            Assert.IsFalse(paper.Beats(scissors) > 0);
        }

        [Test]
        public void TestRockBeatsScissors()
        {
            Gesture scissors = new Scissors();
            Gesture rock = new Rock();

            Assert.IsTrue(rock.Beats(scissors) > 0);
            Assert.IsFalse(scissors.Beats(rock) > 0);
        }

        [Test]
        public void TestRockTies()
        {
            Gesture rock2 = new Rock();
            Gesture rock1 = new Rock();

            Assert.IsTrue(rock1.Beats(rock2) == 0);
            Assert.IsTrue(rock2.Beats(rock1) == 0);
        }

        [Test]
        public void TestScissorsTie()
        {
            Gesture scissors1 = new Scissors();
            Gesture scissors2 = new Scissors();

            Assert.IsTrue(scissors1.Beats(scissors2) == 0);
            Assert.IsTrue(scissors2.Beats(scissors1) == 0);
        }

        [Test]
        public void TestPaperTies()
        {
            Gesture paper1 = new Paper();
            Gesture paper2 = new Paper();
            Assert.IsTrue(paper1.Beats(paper2) == 0);
            Assert.IsTrue(paper2.Beats(paper1) == 0);
        }

        [Test]
        public void NoResult()
        {
            Gesture gesture = null;
            Gesture rock = new Rock();
            
            Warn.Unless(rock.Beats(gesture) != -2);
            Warn.If(rock.Beats(gesture) == -2); 
        }
    }

    public abstract class Gesture
    {
        public int Beats(Gesture gesture)
        {
            if (gesture is Paper)
                return Beats(gesture as Paper);
            else if (gesture is Scissors)
                return Beats(gesture as Scissors);
            else if (gesture is Rock)
                return Beats(gesture as Rock);
            return -2;
        }
        public abstract int Beats(Paper paper);
        public abstract int Beats(Scissors scissors);
        public abstract int Beats(Rock rock);
    }

    public class Rock : Gesture
    {
        public override int Beats(Paper paper)
        {
            return -1;
        }

        public override int Beats(Scissors scissors)
        {
            return 1;
        }

        public override int Beats(Rock rock)
        {
            return 0;
        }
    }

    public class Paper : Gesture
    {  
        public override int Beats(Rock rock)
        {
            return 1;
        }

        public override int Beats(Scissors scissors)
        {
            return -1;
        }

        public override int Beats(Paper paper)
        {
            return 0;
        }
    }

    public class Scissors : Gesture
    {
        public override int Beats(Paper paper)
        {
            return 1;
        }

        public override int Beats(Rock rock)
        {
            return -1;
        }

        public override int Beats(Scissors scissors)
        {
            return 0;
        }
    }
}
