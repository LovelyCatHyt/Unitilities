using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unitilities.Test
{
    public class Bounds2D_Test
    {
        private Bounds2D one = new Bounds2D(Vector2.zero, 2 * Vector2.one);

        [Test]
        public void Test_Init()
        {
            // 新建一个中心在原点, 半长宽均为1的矩形范围
            var b = new Bounds2D(Vector2.zero, 2 * Vector2.one);
            Assert.AreEqual(Vector2.one * 2, b.Size);
            Assert.AreEqual(Vector2.one, b.Extents);
            Assert.AreEqual(-Vector2.one, b.Min);
            Assert.AreEqual(Vector2.one, b.Max);
        }

        [Test]
        public void Test_Encapsulate()
        {
            var temp = one.Encapsulate(new Vector2(2, 2));
            Assert.AreEqual(new Vector2(2, 2), temp.Max);
            Assert.AreEqual(-Vector2.one, temp.Min);
        }

        [Test]
        public void Test_Contains()
        {
            Assert.True(one.Contains(Vector2.zero));
            Assert.True(one.Contains(Vector2.one));
            bool temp = true;
            for (int i = 0; i < 10; i++)
            {
                temp &= one.Contains(new Vector2(Random.value, Random.value));
            }
            Assert.True(temp);
        }

        //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        //// `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator Bounds2D_TestWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }

}