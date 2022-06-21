using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection; 

namespace Tests
{
    
    enum TestEnum
    {
        Fast, 
        Slow, 
        Normal
    }
    enum OrbiResolutionTest
    {
        R7500 = 7500, 
        R120000 = 120000
    }
    internal class TestReflectionPropertyWorkflow
    {
        public OrbiResolutionTest OrbiResolutionTest { get; set; }
        [Test]
        public void TestEnumName()
        {
            OrbiResolutionTest = OrbiResolutionTest.R7500;
            string propertyName = typeof(OrbiResolutionTest).Name; 
            string value = ((int)this.GetType().GetProperty("OrbiResolutionTest").GetValue(this)).ToString();
            string name = Enum.GetName(OrbiResolutionTest);
            string value2 = this.GetType().GetProperty("OrbiResolutionTest").GetValue(this, null).ToString();
            foreach(var property in this.GetType().GetProperties())
            {
                string name2 = property.GetValue(this).ToString();
                Console.WriteLine(name2); 
            }

            //Console.WriteLine(name);
            //Console.WriteLine(value);
            //Console.WriteLine(value2);  
            Console.WriteLine(propertyName); 
        }
    }
}
