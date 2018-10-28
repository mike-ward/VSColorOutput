using NUnit.Framework;
using VSColorOutput.State;

namespace Tests
{
    [SetUpFixture]
    public class Startup
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            Runtime.RunningUnitTests = true;
        }
    }
}