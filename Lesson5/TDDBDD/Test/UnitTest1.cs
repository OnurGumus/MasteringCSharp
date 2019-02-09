using FsCheck.Xunit;
using SpecLight;
using System;
using Xunit;

namespace Test
{
    public class UnitTest1
    {
        /*
            Given I enter `5`
            And I enter `6`
            When I press Add
            Then the result should be `11`
        */
        [Fact]
        public void Test1()
        {
            new Spec(@"In order to know how much money I can save
				As a Math Idiot
				I want to add two numbers")
                .Given(IEnter_, 5)
                .And(IEnter_, 6)
                .When(IPressAdd)
                .Then(TheResultShouldBe_, 11)
                .Execute();

        }

        [Property]
        public void Test2(int x, int y)
        {
            new Spec(@"In order to know how much money I can save
				As a Math Idiot
				I want to add two numbers")
                .Given(IEnter_, x)
                .And(IEnter_, y)
                .When(IPressAdd)
                .Then(TheResultShouldBe_, x + y)
                .Execute();

        }

        void IEnter_(int o0)
        {
            throw new NotImplementedException();
        }

        void IPressAdd()
        {
            throw new NotImplementedException();
        }

        void TheResultShouldBe_(int o0)
        {
            throw new NotImplementedException();
        }
    }
}
