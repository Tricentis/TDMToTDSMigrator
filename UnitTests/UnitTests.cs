using System.IO;
using TDMtoTDSMigrator;

using UnitTests.Properties;
using NUnit.Framework;
    
namespace UnitTests
{
    [TestFixture]
    public class UnitTests {
        private static TdmDataDocument data;
        private static string tddPath = "resources/temp.tdd";
        [SetUp]
        public void Startup() {
            byte[] byteArray = Resources.Dump_110718_0118;
            File.WriteAllBytes(tddPath, byteArray);
            data = new TdmDataDocument(tddPath);
        }

        [TearDown]
        public void Cleanup(){
            File.Delete(tddPath);
        }

        [Test]
        public void TestFindAttributeName() {
            Assert.AreEqual("First", data.FindAttributeName("1"));
            Assert.AreEqual("Last", data.FindAttributeName("2"));
            Assert.AreEqual("Address", data.FindAttributeName("3"));
            Assert.AreEqual("Status", data.FindAttributeName("4"));
            Assert.AreEqual("DateofBirth", data.FindAttributeName("5"));
            Assert.AreEqual("Number", data.FindAttributeName("6"));
            Assert.AreEqual("ID", data.FindAttributeName("7"));
            Assert.AreEqual("Name", data.FindAttributeName("8"));
            Assert.AreEqual("Status",data.FindAttributeName("9"));
            Assert.AreEqual("ID",data.FindAttributeName("10"));
            Assert.AreEqual("Locaction",data.FindAttributeName("11"));
        }

        [Test]
        public void TestFindCategoryName() {
            Assert.AreEqual("Customer", data.FindCategoryName("1"));
            Assert.AreEqual("Product", data.FindCategoryName("2"));
            Assert.AreEqual("Storage", data.FindCategoryName("3"));
        }

        [Test]
        public void TestFindCategoryId() {
            Assert.AreEqual("1", data.FindCategoryId("1"));
            Assert.AreEqual("1", data.FindCategoryId("2"));
            Assert.AreEqual("1", data.FindCategoryId("3"));
            Assert.AreEqual("1", data.FindCategoryId("4"));
            Assert.AreEqual("1", data.FindCategoryId("5"));
            Assert.AreEqual("1", data.FindCategoryId("6"));
            Assert.AreEqual("2", data.FindCategoryId("7"));
            Assert.AreEqual("2", data.FindCategoryId("8"));
            Assert.AreEqual("2", data.FindCategoryId("9"));
            Assert.AreEqual("3", data.FindCategoryId("10"));
            Assert.AreEqual("3", data.FindCategoryId("11"));
        }

        [Test]
        public void TestDataArrangement() {
            data.CreateDataDictionary();
            Assert.AreEqual(3, data.TestData.Keys.Count);
            Assert.AreEqual(3, data.TestData["Customer"].ElementCount);
            Assert.AreEqual(2, data.TestData["Product"].ElementCount);
            Assert.AreEqual(3, data.TestData["Storage"].ElementCount);
        }

        [Test]
        public void TestNumberOfObjects() {
            Assert.AreEqual(8,data.StringAttributes.Count);
        }


        [Test]
        public void TestDecompression() {
            TdmDataDocument.DecompressTddFileIntoXml(tddPath);
            Assert.IsTrue(File.Exists(tddPath.Replace(".tdd",".xml")));
            File.Delete(tddPath.Replace(".tdd", ".xml"));
        }
    }
}
