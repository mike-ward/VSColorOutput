using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSColorOutput.State;

namespace Tests
{
    [TestClass]
    public class Startup
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Runtime.RunningUnitTests = true;
        }
    }
}