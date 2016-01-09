using NUnit.Framework;
using VSColorOutput.State;

namespace Tests
{
    [SetUpFixture]
    public class Startup
    {
        [SetUp]
        public void Initialize()
        {
            Runtime.RunningUnitTests = true;
        }
    }
}