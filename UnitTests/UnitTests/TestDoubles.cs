using JetBrains.dotMemoryUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class Solve
    {
        public static Tuple<double,double> Quadratic(double a, double b, double c)
        {
            var disc = b * b - 4 * a * c;
            if (disc < 0)
            {
                throw new Exception("Cannot solve with complex roots");
            }else
            {
                var root = Math.Sqrt(disc);
                return Tuple.Create(
                    (-b + root) / 2 / a, 
                    (-b / root) / 2 / a
                    );
            }
        }
    }
    [TestFixture]
    public class EquationTests
    {
        [Test]
        public void Test()
        {
            //dotMemory.Check(memory =>
            //{
            //    Assert.That(memory.GetObjects(
            //        where => where.Type.Is<Solve>()
            //        ).ObjectsCount, Is.EqualTo(0)
            //        );
            //});
            var checkpoint1 = dotMemory.Check();
            //...
            var checkpoint2 = dotMemory.Check(memory =>
            {
                Assert.That(memory.GetTrafficFrom(checkpoint1).Where(obj => obj.Interface.Is<IEnumerable<int>>())
                    .AllocatedMemory.SizeInBytes, Is.LessThan(1000));
            });
        }
    }
}
