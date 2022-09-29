using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using InstrumentClient;
using ClientServerCommLibrary;
namespace InstrumentClientTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetEnum()
        {
            var orbiResTest = OrbitrapResolution.X_120000;
            var expected = 120000;
            string result = String.Empty;
            var enumType = typeof(OrbitrapResolution);
            if (enumType == typeof(OrbitrapResolution))
            {
                var temp = (int)Enum.Parse(typeof(OrbitrapResolution), orbiResTest.ToString()); 
                result = temp.ToString();
            }

            Assert.That(expected.ToString(), Is.EqualTo(result)); 
        }

        [Test]
        [TestCase(AnalyzerType.IonTrap, "IonTrap")]
        [TestCase(ScanType.Full, "Full")]
        [TestCase(Polarity.Negative, "Negative")]
        [TestCase(DataType.Centroid, "Centroid")]
        [TestCase(IsolationType.IonTrap, "IonTrap")]
        [TestCase(OrbitrapResolution.X_30000, "X_30000")]
        [TestCase(IonTrapScanRate.Enhanced, "Enhanced")]
        [TestCase(ActivationType.ETD, "ETD")]
        public void TestGetEnumPart2<T>(T enumTest, object expectedVal)
        {
            var result = Enum.Parse(typeof(T), enumTest.ToString()).ToString();
            Assert.That(result, Is.EqualTo(expectedVal));
            
        }

        [Test]
        [TestCase(AnalyzerType.IonTrap, "IonTrap")]
        [TestCase(ScanType.Full, "Full")]
        [TestCase(Polarity.Negative, "Negative")]
        [TestCase(DataType.Centroid, "Centroid")]
        [TestCase(IsolationType.IonTrap, "IonTrap")]
        [TestCase(OrbitrapResolution.X_30000, "30000")]
        [TestCase(IonTrapScanRate.Enhanced, "Enhanced")]
        [TestCase(ActivationType.ETD, "ETD")]
        public void TestGetEnumString<T>(T enumTest, object expectedVal) where T: Enum
        {
            var result = GetEnumString(enumTest); 
            Assert.That(result, Is.EqualTo(expectedVal));
        }

        private static string GetEnumString<T>(T enumOfType) where T : Enum
        {
            var enumType = typeof(T);
            if (enumType == typeof(OrbitrapResolution))
            {
                var temp = (int)Enum.Parse(typeof(OrbitrapResolution), enumOfType.ToString());
                return temp.ToString();
            }
            return Enum.Parse(typeof(T), enumOfType.ToString()).ToString();
        }

        [Test]
        public void TestTranslateSsdo()
        {
            SingleScanDataObject ssdo = new SingleScanDataObject();
            ScanInstructions si = new ScanInstructions();
            ssdo.ScanInstructions = si; 
            ssdo.ScanInstructions.ScanType = ScanType.Full;

            PropertyInfo[] properties = typeof(ScanInstructions).GetProperties();
            foreach(PropertyInfo property in properties)
            {
                var result = property.GetValue(ssdo.ScanInstructions); 
            }
        }
    }
}