using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bandit.Utilities;
using System;
using System.Collections.Generic;

namespace Bandit.Tests.Utilities
{
    [TestClass()]
    public class DriverUtilityTests
    {
        [TestMethod()]
        public void IsExistsVersionTest()
        {
            DriverUtility driverUtility = new DriverUtility();
            bool result = driverUtility.IsExistsVersion(new Version("2.0"));

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void GetVersionsTest()
        {
            DriverUtility driverUtility = new DriverUtility();
            List<Version> versions = driverUtility.GetVersions();

            bool errorFlag = false;

            for (int count = 0; count < 5; count++)
            {
                Random random = new Random();
                Version targetVersion = versions[random.Next(0, versions.Count - 1)];
                
                if (!driverUtility.IsExistsVersion(targetVersion))
                {
                    errorFlag = true;
                }
            }

            Assert.IsFalse(errorFlag);
        }

        [TestMethod()]
        public void GetLatestVersionTest()
        {
            DriverUtility driverUtility = new DriverUtility();
            Version latestVersion = driverUtility.GetLatestVersion();

            Assert.IsTrue(driverUtility.IsExistsVersion(latestVersion));
        }
    }
}