using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static BSMulti_Installer2.Utilities.Utilities;

namespace BSMulti_Installer_Tests.UtilitiesTests
{
    [TestClass]
    public class VersionCompareTests
    {
        [TestMethod]
        public void GreaterLeft_Minor()
        {
            int[] left = new int[] { 0, 1, 1 };
            int[] right = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result < 0);
        }
        [TestMethod]
        public void GreaterLeft_Major()
        {
            int[] left = new int[] { 1, 0, 1 };
            int[] right = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result < 0);
        }
        [TestMethod]
        public void GreaterLeft_DiffLength()
        {
            int[] left = new int[] { 0, 0, 1, 1 };
            int[] right = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void GreaterRight_Minor()
        {
            int[] right = new int[] { 0, 1, 1 };
            int[] left = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result > 0);
        }
        [TestMethod]
        public void GreaterRight_Major()
        {
            int[] right = new int[] { 1, 0, 1 };
            int[] left = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result > 0);
        }
        [TestMethod]
        public void GreaterRight_DiffLength()
        {
            int[] right = new int[] { 0, 0, 1, 1 };
            int[] left = new int[] { 0, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Equal()
        {
            int[] left = new int[] { 1, 0, 1 };
            int[] right = new int[] { 1, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result == 0);
        }

        [TestMethod]
        public void Equal_DiffLengthRight()
        {
            int[] left = new int[] { 1, 0, 1 };
            int[] right = new int[] { 1, 0, 1, 0 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result == 0);
        }
        [TestMethod]
        public void Equal_DiffLengthLeft()
        {
            int[] left = new int[] { 1, 0, 1, 0 };
            int[] right = new int[] { 1, 0, 1 };
            int result = CompareVersions(left, right);
            Assert.IsTrue(result == 0);
        }
    }
}
