using AutoFixture;
using AutoFixture.Xunit2;

namespace Conduit.Business.Tests
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute()
            : base(() => new Fixture().Customize(new OmitOnRecursionCustomization()))
        {
        }

        private class OmitOnRecursionCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            }
        }
    }
}
