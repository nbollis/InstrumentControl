using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MassSpectrometry;
using Newtonsoft.Json;
using WorkflowServer;
using UsefulProteomicsDatabases;
using WorkflowServer.Test.Testing;

namespace Tests
{
    public static class TestInstrumentControlIO
    {

        public static double[] testArray1;
        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            Loaders.LoadElements();
            Directory.CreateDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles"));
            testArray1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            string doublePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray1.GetType().ToString());
            Directory.CreateDirectory(doublePath);
            InstrumentControlIO.Serialize(doublePath + @"/testArray1.txt", testArray1);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Directory.Delete(Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles"), true);
        }

        [Test]
        public static void TestSerializeDeserialize()
        {
            double[] testArray2 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\something.txt");
            InstrumentControlIO.Serialize(filePath, testArray2);
            Assert.That(File.Exists(filePath), Is.True);

            Type temp1 = testArray2.GetType();
            string remp11 = temp1.ToString();
            var tem2 = filePath.GetType();

            var testcase1 = InstrumentControlIO.Deserialize<double[]>(filePath);
            Assert.That(testcase1 != null);
            string testObject;
            var stream = File.Open(filePath, FileMode.Open);
            using (StreamReader writer = new StreamReader(stream))
            {
                testObject = writer.ReadLine() ?? throw new ArgumentNullException();
            }
            stream.Close();
            var json = JsonConvert.DeserializeObject<double[]>(testObject) ?? throw new ArgumentNullException();
            for (int i = 0; i < testcase1.Length; i++)
            {
                Assert.That(json[i].Equals(testcase1[i]));
            }
        }

        [Test]
        public static void TestCheckforTypeFolderGetFilePath()
        {
            Type folderToTest = testArray1.GetType();
            string testFoldername = "abc";
            Type folderNotExisted = testFoldername.GetType();
            Assert.That(InstrumentControlIO.CheckforTypeFolder(folderToTest));
            string testPath = InstrumentControlIO.GetFilePath(folderToTest, "testArray1");
            string doublePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray1.GetType().ToString()) + @"/testArray1.txt";
            Assert.That(testPath.Equals(doublePath));
            Assert.That(!InstrumentControlIO.CheckforTypeFolder(folderNotExisted));
        }

        [Test]
        public static void TestSave()
        {
            DeleteandCreateFiles();
            double[] testArray2 = new double[] { 0, 2, 4, 6, 8, 10 };
            Type doubleType = testArray2.GetType();
            string nameToTest = "testArray2";
            string filePath = InstrumentControlIO.GetFilePath(doubleType, nameToTest);
            InstrumentControlIO.Save("nameNotExisted", testArray2);
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");
            Assert.That(Directory.Exists(directoryPath), Is.True);
            Assert.That(!File.Exists(filePath), Is.True);
            InstrumentControlIO.Save(nameToTest, testArray2);
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public static void TestLoadAllOfTypeT()
        {
            Type folderToTest = testArray1.GetType();
            double[][] listToTest = InstrumentControlIO.LoadAllOfTypeT<double[]>(folderToTest).ToArray();
            for (int i = 0; i < listToTest.Length; i++)
            {
                Assert.That(listToTest[0][i].Equals(testArray1[i]));
            }
            Assert.That(listToTest[0].SequenceEqual(testArray1));

            DeleteandCreateFiles();
            testArray1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            string doublePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray1.GetType().ToString());
            Directory.CreateDirectory(doublePath);
            InstrumentControlIO.Serialize(doublePath + @"/testArray1.txt", testArray1);
            double[] testArray2 = new double[] { 0, 2, 4, 6, 8, 10 };
            string doublePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray2.GetType().ToString());
            Directory.CreateDirectory(doublePath2);
            InstrumentControlIO.Serialize(doublePath2 + @"/testArray2.txt", testArray2);
            double[] testArray3 = new double[] { 1, 3, 5, 7, 9, 11 };
            string doublePath3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray3.GetType().ToString());
            Directory.CreateDirectory(doublePath3);
            InstrumentControlIO.Serialize(doublePath3 + @"/testArray3.txt", testArray3);
            Type folderToTest2 = testArray1.GetType();
            double[][] listToTest2 = InstrumentControlIO.LoadAllOfTypeT<double[]>(folderToTest2).ToArray();
            for (int i = 0; i < listToTest2.Length; i++)
            {
                Assert.That(listToTest2[0][i].Equals(testArray1[i]));
            }
        }

        [Test]
        public static void TestLoadAll()
        {
            DeleteandCreateFiles();
            testArray1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            string doublePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray1.GetType().ToString());
            Directory.CreateDirectory(doublePath);
            InstrumentControlIO.Serialize(doublePath + @"/testArray1.txt", testArray1);
            int[] testArray2 = new int[] { 0, 2, 4, 6, 8, 10 };
            string intPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray2.GetType().ToString());
            Directory.CreateDirectory(intPath);
            InstrumentControlIO.Serialize(intPath + @"/testArray2.txt", testArray2);
            string[] testArray3 = new string[] { "1", "3", "5", "7", "9", "11" };
            string stringPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray3.GetType().ToString());
            Directory.CreateDirectory(stringPath);
            InstrumentControlIO.Serialize(stringPath + @"/testArray3.txt", testArray3);

            Dictionary<Type, List<object>> dictionary = InstrumentControlIO.LoadAll();
            var temp1 = dictionary[typeof(double[])];
            Type folderToTest1 = testArray1.GetType();
            double[][] listToTest1 = InstrumentControlIO.LoadAllOfTypeT<double[]>(folderToTest1).ToArray();
            for (int i = 0; i < listToTest1.Length; i++)
            Assert.That(listToTest1[0].SequenceEqual(testArray1));

            var temp2 = dictionary[typeof(int[])];
            Type folderToTest2 = testArray2.GetType();
            int[][] listToTest2 = InstrumentControlIO.LoadAllOfTypeT<int[]>(folderToTest2).ToArray();
            Assert.That(listToTest2[0].SequenceEqual(testArray2));

            var temp3 = dictionary[typeof(string[])];
            Type folderToTest3 = testArray3.GetType();
            string[][] listToTest3 = InstrumentControlIO.LoadAllOfTypeT<string[]>(folderToTest3).ToArray();
            Assert.That(listToTest3[0].SequenceEqual(testArray3));
        }

        [Test]
        public static void TestDelete()
        {
            double[] testArray2 = new double[] { 0, 2, 4, 6, 8, 10 };
            Type doubleType = testArray2.GetType();
            string nameToTest = "testArray2";
            string filePath = InstrumentControlIO.GetFilePath(doubleType, nameToTest);
            InstrumentControlIO.Save(nameToTest, testArray2);
            Assert.That(File.Exists(filePath), Is.True);
            InstrumentControlIO.Delete<double[]>(doubleType, nameToTest);
            Assert.That(!File.Exists(filePath), Is.True);
        }

        private static void DeleteandCreateFiles()
        {
            Directory.Delete(Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles"), true);
            Directory.CreateDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles"));
        }

        [Test]
        public static void TestLoadAllNamesOfType()
        {
            DeleteandCreateFiles();
            testArray1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            string doublePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray1.GetType().ToString());
            Directory.CreateDirectory(doublePath);
            InstrumentControlIO.Serialize(doublePath + @"/testArray1.txt", testArray1);
            double[] testArray2 = new double[] { 0, 2, 4, 6, 8, 10 };
            string doublePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray2.GetType().ToString());
            Directory.CreateDirectory(doublePath2);
            InstrumentControlIO.Serialize(doublePath2 + @"/testArray2.txt", testArray2);
            double[] testArray3 = new double[] { 1, 3, 5, 7, 9, 11 };
            string doublePath3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", testArray3.GetType().ToString());
            Directory.CreateDirectory(doublePath3);
            InstrumentControlIO.Serialize(doublePath3 + @"/testArray3.txt", testArray3);
            string [] testTypes = InstrumentControlIO.LoadAllNamesOfType(testArray1.GetType());
            Assert.That(testTypes.Length == 3);
        }

        [Test]
        public static void GeneralTest()
        {
            DeleteandCreateFiles();
            //2 double arr
            double[] doubleArr1 = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5 };
            double[] doubleArr2 = new double[] { 0.6, 0.7, 0.8, 0.9, 1.0 };

            //4 int list
            List<int> intList1 = new List<int>();
            intList1.Add(1);
            intList1.Add(2);
            intList1.Add(3);
            intList1.Add(4);
            intList1.Add(5);
            List<int> intList2 = new List<int>();
            intList2.Add(6);
            intList2.Add(7);
            intList2.Add(8);
            intList2.Add(9);
            intList2.Add(10);
            List<int> intList3 = new List<int>();
            intList3.Add(11);
            intList3.Add(12);
            intList3.Add(13);
            intList3.Add(14);
            intList3.Add(15);
            List<int> intList4 = new List<int>();
            intList4.Add(16);
            intList4.Add(17);
            intList4.Add(18);
            intList4.Add(19);
            intList4.Add(20);
            string listPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "DataFiles", intList1.GetType().ToString());
            Directory.CreateDirectory(listPath);

            //1 Dummy
            Dummy dummy = new Dummy();

            //2 tuple
            (double, int) tuple1 = (1.0, 3);
            (double, int) tuple2 = (2.0, 4);

            InstrumentControlIO.Save("doubleArr1", doubleArr1);
            InstrumentControlIO.Save("doubleArr2", doubleArr2);
            InstrumentControlIO.Save("intList1", intList1);
            InstrumentControlIO.Save("intList2", intList1);
            InstrumentControlIO.Save("intList3", intList1);
            InstrumentControlIO.Save("intList4", intList1);
            InstrumentControlIO.Save("Dummy", dummy);
            InstrumentControlIO.Save("tuple1", tuple1);
            InstrumentControlIO.Save("tuple2", tuple2);


            string doublePath1 = InstrumentControlIO.GetFilePath(doubleArr1.GetType(), "doubleArr1");
            string doublePath2 = InstrumentControlIO.GetFilePath(doubleArr2.GetType(), "doubleArr2");
            string listPath1 = InstrumentControlIO.GetFilePath(intList1.GetType(), "intList1");
            string listPath2 = InstrumentControlIO.GetFilePath(intList2.GetType(), "intList2");
            string listPath3 = InstrumentControlIO.GetFilePath(intList3.GetType(), "intList3");
            string listPath4 = InstrumentControlIO.GetFilePath(intList4.GetType(), "intList4");
            string dummyPath1 = InstrumentControlIO.GetFilePath(dummy.GetType(), "dummy");
            string tuplePath1 = InstrumentControlIO.GetFilePath(tuple1.GetType(), "tuple1");
            string tuplePath2 = InstrumentControlIO.GetFilePath(tuple2.GetType(), "tuple2");

            Assert.That(File.Exists(doublePath1), Is.True);
            Assert.That(File.Exists(doublePath2), Is.True);
            Assert.That(File.Exists(listPath1), Is.True);
            Assert.That(File.Exists(listPath2), Is.True);
            Assert.That(File.Exists(listPath3), Is.True);
            Assert.That(File.Exists(listPath4), Is.True);
            Assert.That(File.Exists(dummyPath1), Is.True);
            Assert.That(File.Exists(tuplePath1), Is.True);
            Assert.That(File.Exists(tuplePath2), Is.True);

            InstrumentControlIO.Delete<List<int>>(intList1.GetType(), "intList1");
            InstrumentControlIO.Delete<List<int>>(intList2.GetType(), "intList2");
            InstrumentControlIO.Delete<List<int>>(intList3.GetType(), "intList3");
            InstrumentControlIO.Delete<List<int>>(intList4.GetType(), "intList4");
            Assert.That(!File.Exists(listPath1), Is.True);
            Assert.That(!File.Exists(listPath2), Is.True);
            Assert.That(!File.Exists(listPath3), Is.True);
            Assert.That(!File.Exists(listPath4), Is.True);

            
            string[] testTypes = InstrumentControlIO.LoadAllNamesOfType(doubleArr1.GetType());
            Assert.That(testTypes.Length == 2);
            double[][] listToTest = InstrumentControlIO.LoadAllOfTypeT<double[]>(doubleArr1.GetType()).ToArray();
            Assert.That(listToTest[0].SequenceEqual(doubleArr1));

            Dictionary<Type, List<object>> dictionary = InstrumentControlIO.LoadAll();
            var temp1 = dictionary[typeof(double[])];
            Type folderToTest1 = doubleArr1.GetType();
            double[][] listToTest1 = InstrumentControlIO.LoadAllOfTypeT<double[]>(folderToTest1).ToArray();
            Assert.That(listToTest1[0].SequenceEqual(doubleArr1));
            Assert.That(listToTest1[1].SequenceEqual(doubleArr2));

            

            var temp3 = dictionary[typeof((double, int))];
            Type folderToTest3 = tuple1.GetType();
            (double, int)[] listToTest3 = InstrumentControlIO.LoadAllOfTypeT<(double, int)>(folderToTest3).ToArray();
            Assert.That(listToTest3[0].Equals(tuple1));
            Assert.That(listToTest3[1].Equals(tuple2));
        }
    }   
}